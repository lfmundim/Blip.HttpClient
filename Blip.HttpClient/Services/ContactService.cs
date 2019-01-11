using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Blip.HttpClient.Exceptions;
using Blip.HttpClient.Factories;
using Lime.Messaging.Resources;
using Lime.Protocol;
using Lime.Protocol.Network;
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
        /// Creates a ContactService instance to Get and Set contacts using a given authKey.
        /// </summary>
        /// <param name="authKey">Bot authorization key</param>
        public ContactService(string authKey)
        {
            var factory = new BlipHttpClientFactory();
            _sender = factory.BuildBlipHttpClient(authKey);
        }

        /// <summary>
        /// Gets the Bot's Contact using the <paramref name="identifier"/> param
        /// </summary>
        /// <param name="identifier">User identifier whose contact must be recovered</param>
        /// <param name="cancellationToken"></param>
        /// <param name="logger">Optional <c>ILogger</c> from <c>Serilog</c> to log useful information</param>
        /// <returns>Lime <c>Contact</c> response object</returns>
        /// <example>
        /// <code>
        /// Contact x = await _contactService.GetContactAsync(contact, cancellationToken, logger)
        /// </code>
        /// </example>
        /// <exception cref="BlipHttpClientException">Failure getting the contact</exception>
        /// <exception cref="Exception">Unknown error</exception>
        public async Task<Contact> GetContactAsync(string identifier, CancellationToken cancellationToken, ILogger logger = null)
        {
            try
            {
                var command = new Command()
                {
                    Method = CommandMethod.Get,
                    Uri = new LimeUri($"/contacts/{identifier}")
                };
                logger?.Information("[GetContact] Trying get contact using {@identity}'s", identifier);
                var contactResponse = await _sender.ProcessCommandAsync(command, cancellationToken);
                if (contactResponse.Status != CommandStatus.Success)
                {
                    throw new BlipHttpClientException("Failed to get contact from BLiP's agenda.", contactResponse);
                }
                logger?.Information("[GetContact] Got contact using {@identity}'s contact: {@contact}", identifier, contactResponse.Resource as Contact);
                return contactResponse.Resource as Contact;
            }
            catch (Exception ex)
            {
                logger?.Error(ex, "[GetContact] Failed to get contact using {@identity}'s contact", identifier);
                throw ex;
            }
        }

        /// <summary>
        /// Sets the <paramref name="contact"/> on the Bot's agenda
        /// </summary>
        /// <param name="contact"></param>
        /// <param name="cancellationToken"></param>
        /// <param name="logger">Optional <c>ILogger</c> from <c>Serilog</c> to log useful information</param>
        /// <returns>Lime <c>Command</c> response object</returns>
        /// <example>
        /// <code>
        /// Command x = await _contactService.SetContactAsync(contact, cancellationToken, logger)
        /// </code>
        /// </example>
        /// <exception cref="BlipHttpClientException">Failure setting the contact</exception>
        /// <exception cref="Exception">Unknown error</exception>
        public async Task<Command> SetContactAsync(Contact contact, CancellationToken cancellationToken, ILogger logger = null)
        {
            try
            {
                var command = new Command()
                {
                    Method = CommandMethod.Set,
                    Uri = new LimeUri("/contacts"),
                    Resource = contact
                };

                logger?.Information("[SetContact] Trying set contact {@contact}", contact);
                var contactResponse = await _sender.ProcessCommandAsync(command, cancellationToken);
                if (contactResponse.Status != CommandStatus.Success)
                {
                    throw new BlipHttpClientException("Failed to set contact on BLiP's agenda.", contactResponse);
                }
                logger?.Information("[SetContact] Set contact {@contact}", contact);

                return contactResponse;
            }
            catch (BlipHttpClientException bex)
            {
                logger?.Error(bex, "[SetContact] Failed to set contact {@contact}", contact);
                throw bex;
            }
            catch (Exception ex)
            {
                logger?.Error(ex, "[SetContact] Failed to set contact {@contact}", contact);
                throw ex;
            }
        }
    }
}
