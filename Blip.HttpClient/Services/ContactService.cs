using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Blip.HttpClient.Factories;
using Lime.Messaging.Resources;
using Lime.Protocol;
using Serilog;
using Take.Blip.Client;

namespace Blip.HttpClient.Services
{
    /// <summary>
    /// Http Abstraction of BLiP's /contact URI
    /// </summary>
    public class ContactService : IContactService
    {
        private readonly ISender _sender;

        /// <summary>
        /// Creates a ContactService instance to Get and Set contacts
        /// </summary>
        /// <param name="sender">ISender instance, from BlipHttpClientFactory</param>
        public ContactService(ISender sender)
        {
            _sender = sender;
        }

        /// <summary>
        /// Gets the botIdentifier's contact using the identifier param
        /// </summary>
        /// <param name="identifier">User identifier whose contact must be recovered</param>
        /// <param name="cancellationToken"></param>
        /// <param name="logger">Optional ILogger from Serilog to log useful information</param>s
        /// <returns></returns>
        public async Task<Contact> GetContactAsync(string identifier, CancellationToken cancellationToken, ILogger logger = null)
        {
            try
            {
                var command = new Command()
                {
                    Method = CommandMethod.Get,
                    Uri = new LimeUri($"/contacts/{identifier}")
                };
                if(logger != null) logger.Information("[GetContact] Trying get contact using {@identity}'s", identifier);
                var contactResponse = await _sender.ProcessCommandAsync(command, cancellationToken);
                if (logger != null) logger.Information("[GetContact] Got contact using {@identity}'s contact: {@contact}", identifier, contactResponse.Resource as Contact);
                return contactResponse.Resource as Contact;
            }
            catch (Exception ex)
            {
                if (logger != null) logger.Error(ex, "[GetContact] Failed to get contact using {@identity}'s contact", identifier);
                throw ex;
            }
        }

        /// <summary>
        /// Sets the contact on the botIdentifier's agenda
        /// </summary>
        /// <param name="contact"></param>
        /// <param name="cancellationToken"></param>
        /// <param name="logger">Optional ILogger from Serilog to log useful information</param>
        /// <returns></returns>
        public async Task<Command> SetContactAsync(Contact contact, CancellationToken cancellationToken, ILogger logger = null)
        {
            try
            { 
                var command = new Command()
                {
                    Method = CommandMethod.Set,
                    Uri = new LimeUri($"/contacts"),
                    Resource = contact
                };

                if (logger != null) logger.Information("[GetContact] Trying set contact {@contact}", contact);
                var contactResponse = await _sender.ProcessCommandAsync(command, cancellationToken);
                if (logger != null) logger.Information("[GetContact] Set contact {@contact}", contact);

                return contactResponse;
            }
            catch (Exception ex)
            {
                if (logger != null) logger.Error(ex, "[GetContact] Failed to set contact {@contact}", contact);
                throw ex;
            }
        }
    }
}
