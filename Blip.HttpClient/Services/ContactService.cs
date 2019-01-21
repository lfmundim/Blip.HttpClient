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
using Take.Blip.Client.Extensions.Contacts;

namespace Blip.HttpClient.Services
{
    /// <summary>
    /// Http Abstraction of BLiP's /contact URI
    /// </summary>
    public class ContactService : IContactService
    {
        private readonly ISender _sender;

        /// <summary>
        /// Creates a ContactService instance using a BLiP client
        /// </summary>
        /// <param name="sender">ISender instance, from BlipHttpClientFactory</param>
        public ContactService(ISender sender)
        {
            _sender = sender;
        }

        /// <summary>
        /// Creates a ContactService instance using a given authKey.
        /// </summary>
        /// <param name="authKey">Bot authorization key</param>
        public ContactService(string authKey)
        {
            var factory = new BlipHttpClientFactory();
            _sender = factory.BuildBlipHttpClient(authKey);
        }

        /// <summary>
        /// Deletes a contact from the bot's agenda
        /// </summary>
        /// <param name="identity"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="BlipHttpClientException">Failure deleting the contact</exception>
        /// <exception cref="Exception">Unknown error</exception>
        public async Task DeleteAsync(Identity identity, CancellationToken cancellationToken)
        {
            try
            {
                var contactExtension = new ContactExtension(_sender);
                await contactExtension.DeleteAsync(identity, cancellationToken);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task DeleteAsync(Identity identity, CancellationToken cancellationToken, ILogger logger)
        {
            try
            {
                logger.Information("[DeleteContact] Trying delete contact using {@identity}", identity);
                var command = new Command()
                {
                    Method = CommandMethod.Delete,
                    Uri = new LimeUri($"/contacts/{identity}")
                };
                var deleteResponse = await _sender.ProcessCommandAsync(command, cancellationToken);
                if (deleteResponse.Status != CommandStatus.Success)
                {
                    throw new BlipHttpClientException("Failed to delete contact from BLiP's agenda.", deleteResponse);
                }
                logger.Information("[DeleteContact] Successfully deleted {@identity} from BLiP's agenda", identity);
            }
            catch (BlipHttpClientException bex)
            {
                logger.Error(bex, "[DeleteContact] Failed to delete contact with identity {@identity}", identity);
                throw bex;
            }
            catch (Exception ex)
            {
                logger.Error(ex, "[DeleteContact] Failed to delete contact with identity {@identity}", identity);
                throw ex;
            }
        }

        /// <summary>
        /// Gets the Bot's Contact using the <paramref name="identity"/> param
        /// </summary>
        /// <param name="identity"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<Contact> GetAsync(Identity identity, CancellationToken cancellationToken)
        {
            try
            {
                var contactExtension = new ContactExtension(_sender);
                var contact = await contactExtension.GetAsync(identity, cancellationToken);
                return contact;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Gets the Bot's Contact using the <paramref name="identity"/> param
        /// </summary>
        /// <param name="identity">User identifier whose contact must be recovered</param>
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
        public async Task<Contact> GetAsync(Identity identity, CancellationToken cancellationToken, ILogger logger)
        {
            try
            {
                var command = new Command()
                {
                    Method = CommandMethod.Get,
                    Uri = new LimeUri($"/contacts/{identity}")
                };
                logger?.Information("[GetContact] Trying get contact using {@identity}'s", identity);
                var contactResponse = await _sender.ProcessCommandAsync(command, cancellationToken);
                if (contactResponse.Status != CommandStatus.Success)
                {
                    throw new BlipHttpClientException("Failed to get contact from BLiP's agenda.", contactResponse);
                }
                logger?.Information("[GetContact] Got contact using {@identity}'s contact: {@contact}", identity, contactResponse.Resource as Contact);
                return contactResponse.Resource as Contact;
            }
            catch (Exception ex)
            {
                logger?.Error(ex, "[GetContact] Failed to get contact using {@identity}'s contact", identity);
                throw ex;
            }
        }

        /// <summary>
        /// Updates the identity's contact with the new information, keeping any unchanged old data
        /// </summary>
        /// <param name="identity"></param>
        /// <param name="contact"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task MergeAsync(Identity identity, Contact contact, CancellationToken cancellationToken)
        {
            try
            {
                var contactExtension = new ContactExtension(_sender);
                await contactExtension.MergeAsync(identity, contact, cancellationToken);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task MergeAsync(Identity identity, Contact contact, CancellationToken cancellationToken, ILogger logger)
        {
            try
            {
                var command = new Command()
                {
                    Method = CommandMethod.Merge,
                    Uri = new LimeUri("/contacts"),
                    Resource = contact
                };
                logger?.Information("[MergeContact] Trying merge contact using {@identity} and {@contact}", identity, contact);
                var contactResponse = await _sender.ProcessCommandAsync(command, cancellationToken);
                if (contactResponse.Status != CommandStatus.Success)
                {
                    throw new BlipHttpClientException($"Failed to merge contact on BLiP's agenda using {identity} and {contact}", contactResponse);
                }
                logger?.Information("[MergeContact] Successfully merged contact using {@identity} and {@contact}", identity, contact);
            }
            catch (BlipHttpClientException bex)
            {
                logger?.Error(bex, "[MergeContact] Failed to merge contact using {@identity} and {@contact}", identity, contact);
                throw bex;
            }
            catch (Exception ex)
            {
                logger?.Error(ex, "[MergeContact] Failed to merge contact using {@identity} and {@contact}", identity, contact);
                throw ex;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="identity"></param>
        /// <param name="contact"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task SetAsync(Identity identity, Contact contact, CancellationToken cancellationToken)
        {
            try
            {
                var contactExtension = new ContactExtension(_sender);
                await contactExtension.SetAsync(identity, contact, cancellationToken);
            }
            catch (Exception ex)
            {
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
        public async Task<Command> SetAsync(Contact contact, CancellationToken cancellationToken, ILogger logger)
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
