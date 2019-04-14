using Blip.HttpClient.Exceptions;
using Blip.HttpClient.Factories;
using Blip.HttpClient.Models;
using Lime.Protocol;
using Serilog;
using System;
using System.Threading;
using System.Threading.Tasks;
using Take.Blip.Client;
using Take.Blip.Client.Extensions.Broadcast;

namespace Blip.HttpClient.Services.Broadcast
{
    /// <summary>
    /// Service responsible for managing broadcast lists and sending messages to them
    /// </summary>
    public class BroadcastService : IBroadcastService
    {
        private const string PostMasterIdentity = "postmaster@broadcast.msging.net";
        private readonly ISender _sender;

        /// <summary>
        /// Creates an instance of the BroadcastService using a BLiP Client
        /// </summary>
        /// <param name="sender">ISender instance, from BlipHttpClientFactory</param>
        public BroadcastService(ISender sender)
        {
            _sender = sender;
        }

        /// <summary>
        /// Creates a BroadcastService instance using a given authKey.
        /// </summary>
        /// <param name="authKey">Bot authorization key</param>
        public BroadcastService(string authKey)
        {
            var factory = new BlipHttpClientFactory();
            _sender = factory.BuildBlipHttpClient(authKey);
        }

        /// <summary>
        /// Adds a recipient to a broadcast list based on the recipient's identity and the list's name
        /// </summary>
        /// <param name="listName"></param>
        /// <param name="recipientIdentity"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task AddRecipientAsync(string listName, Identity recipientIdentity, CancellationToken cancellationToken = default(CancellationToken))
        {
            var broadcastExtension = new BroadcastExtension(_sender);
            await broadcastExtension.AddRecipientAsync(listName, recipientIdentity, cancellationToken);
        }

        /// <summary>
        /// Adds a recipient to a broadcast list based on the recipient's identity and the list's name
        /// </summary>
        /// <param name="listName"></param>
        /// <param name="recipientIdentity"></param>
        /// <param name="cancellationToken"></param>
        /// <param name="logger"></param>
        /// <returns>Lime <c>Command</c> response object</returns>
        /// <exception cref="BlipHttpClientException">Failure Adding identity to the broadcast list</exception>
        /// <exception cref="Exception">Unknown error</exception>
        public async Task<Command> AddRecipientAsync(string listName, Identity recipientIdentity, CancellationToken cancellationToken, ILogger logger)
        {
            try
            {
                logger.Information("[AddRecipient] Trying Add {@identity} to the broadcast list {@broadcastList}", recipientIdentity, listName);
                var command = new Command()
                {
                    Method = CommandMethod.Set,
                    Uri = new LimeUri($"/lists/{listName}@broadcast.msging.net/recipients"),
                    Resource = new IdentityDocument(recipientIdentity),
                    To = PostMasterIdentity
                };
                
                var addRecipientResponse = await _sender.ProcessCommandAsync(command, cancellationToken);
                if (addRecipientResponse.Status != CommandStatus.Success)
                {
                    throw new BlipHttpClientException("Failed to add recipient to broadcast list.", addRecipientResponse);
                }
                logger.Information("[AddRecipient] Successfully added {@identity} to the broadcast list {@broadcastList}", recipientIdentity, listName);

                return addRecipientResponse;
            }
            catch (BlipHttpClientException bex)
            {
                logger.Error(bex, "[AddRecipient] Failed to add {@identity} to the broadcast list {@broadcastList}", recipientIdentity, listName);
                throw;
            }
            catch (Exception ex)
            {
                logger.Error(ex, "[AddRecipient] Failed to add {@identity} to the broadcast list {@broadcastList}", recipientIdentity, listName);
                throw;
            }
        }

        /// <summary>
        /// Creates a distribution list using the given name. The <param nameref="listName"/> should ideally have the bot's <c>Identifier</c> as a prefix
        /// </summary>
        /// <param name="listName"></param>
        /// <param name="cancellationToken"></param>
        public async Task CreateDistributionListAsync(string listName, CancellationToken cancellationToken = default(CancellationToken))
        {
            var broadcastExtension = new BroadcastExtension(_sender);
            await broadcastExtension.CreateDistributionListAsync(listName, cancellationToken);
        }

        /// <summary>
        /// Creates a distribution list using the given name. The <param nameref="listName"/> should ideally have the bot's <c>Identifier</c> as a prefix
        /// </summary>
        /// <param name="listName"></param>
        /// <param name="cancellationToken"></param>
        /// <param name="logger"></param>
        /// <returns>Lime <c>Command</c> response object</returns>
        /// <exception cref="BlipHttpClientException">Failure creating broadcast list</exception>
        /// <exception cref="Exception">Unknown error</exception>
        public async Task<Command> CreateDistributionListAsync(string listName, CancellationToken cancellationToken, ILogger logger)
        {
            try
            {
                logger.Information("[CreateDistributionList] Trying create broadcast list {@broadcastList}", listName);
                var command = new Command()
                {
                    Method = CommandMethod.Set,
                    Uri = new LimeUri($"/lists"),
                    Resource = new BroadcastListResource($"{listName}@broadcast.msging.net"),
                    To = PostMasterIdentity
                };
                var createListResponse = await _sender.ProcessCommandAsync(command, cancellationToken);
                if (createListResponse.Status != CommandStatus.Success)
                {
                    throw new BlipHttpClientException("Failed to create broadcast list.", createListResponse);
                }
                logger.Information("[CreateDistributionList] Created broadcast list {@broadcastList}", listName);

                return createListResponse;
            }
            catch (BlipHttpClientException bex)
            {
                logger.Error(bex, "[CreateDistributionList] Failed to create broadcast list {@broadcastList}", listName);
                throw;
            }
            catch (Exception ex)
            {
                logger.Error(ex, "[CreateDistributionList] Failed to create broadcast list {@broadcastList}", listName);
                throw;
            }
        }

        /// <summary>
        /// Deletes a given distribution list
        /// </summary>
        /// <param name="listName"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task DeleteDistributionListAsync(string listName, CancellationToken cancellationToken = default(CancellationToken))
        {
            var broadcastExtension = new BroadcastExtension(_sender);
            await broadcastExtension.DeleteDistributionListAsync(listName, cancellationToken);
        }

        /// <summary>
        /// Delete a distribution list using the given <paramref name="listName"/>.
        /// </summary>
        /// <param name="listName"></param>
        /// <param name="cancellationToken"></param>
        /// <param name="logger"></param>
        /// <returns>Lime <c>Command</c> response object</returns>
        /// <exception cref="BlipHttpClientException">Failure deleting broadcast list</exception>
        /// <exception cref="Exception">Unknown error</exception>
        public async Task<Command> DeleteDistributionListAsync(string listName, CancellationToken cancellationToken, ILogger logger)
        {
            try
            {
                logger.Information("[DeleteDistributionList] Trying delete broadcast list {@broadcastList}", listName);
                var command = new Command()
                {
                    Method = CommandMethod.Delete,
                    Uri = new LimeUri($"/lists/{listName}"),
                    Resource = new BroadcastListResource($"{listName}@broadcast.msging.net"),
                    To = PostMasterIdentity
                };
                var deleteListResponse = await _sender.ProcessCommandAsync(command, cancellationToken);
                if (deleteListResponse.Status != CommandStatus.Success)
                {
                    throw new BlipHttpClientException("Failed to delete broadcast list.", deleteListResponse);
                }
                logger.Information("[DeleteDistributionList] Deleted broadcast list {@broadcastList}", listName);

                return deleteListResponse;
            }
            catch (BlipHttpClientException bex)
            {
                logger.Error(bex, "[DeleteDistributionList] Failed to delete broadcast list {@broadcastList}", listName);
                throw;
            }
            catch (Exception ex)
            {
                logger.Error(ex, "[DeleteDistributionList] Failed to delete broadcast list {@broadcastList}", listName);
                throw;
            }
        }

        /// <summary>
        /// Removes given <paramref name="recipientIdentity"/> from the broadcast list with the name <paramref name="listName"/>
        /// </summary>
        /// <param name="listName"></param>
        /// <param name="recipientIdentity"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task DeleteRecipientAsync(string listName, Identity recipientIdentity, CancellationToken cancellationToken = default(CancellationToken))
        {
            var broadcastExtension = new BroadcastExtension(_sender);
            await broadcastExtension.DeleteRecipientAsync(listName, recipientIdentity, cancellationToken);
        }

        /// <summary>
        /// Removes given <paramref name="recipientIdentity"/> from the broadcast list with the name <paramref name="listName"/>
        /// </summary>
        /// <param name="listName"></param>
        /// <param name="recipientIdentity"></param>
        /// <param name="cancellationToken"></param>
        /// <param name="logger"></param>
        /// <returns>Lime <c>Command</c> response object</returns>
        /// <exception cref="BlipHttpClientException">Failure removing user from distribution list</exception>
        /// <exception cref="Exception">Unknown error</exception>
        public async Task<Command> DeleteRecipientAsync(string listName, Identity recipientIdentity, CancellationToken cancellationToken, ILogger logger)
        {
            try
            {
                logger.Information("[DeleteRecipient] Trying delete recipient {@identity} from list {@broadcastList}", recipientIdentity, listName);
                var command = new Command()
                {
                    Method = CommandMethod.Delete,
                    Uri = new LimeUri($"/lists/{listName}@broadcast.msging.net/recipients/{recipientIdentity}"),
                    To = PostMasterIdentity
                };
                var deleteRecipientResponse = await _sender.ProcessCommandAsync(command, cancellationToken);
                if (deleteRecipientResponse.Status != CommandStatus.Success)
                {
                    throw new BlipHttpClientException("Failed to delete recipient from broadcast list.", deleteRecipientResponse);
                }
                logger.Information("[DeleteRecipient] Deleted recipient {@identity} from list {@broadcastList}", recipientIdentity, listName);

                return deleteRecipientResponse;
            }
            catch (BlipHttpClientException bex)
            {
                logger.Error(bex, "[DeleteRecipient] Failed to delete recipient {@identity} from broadcast list {@broadcastList}", recipientIdentity, listName);
                throw;
            }
            catch (Exception ex)
            {
                logger.Error(ex, "[DeleteRecipient] Failed to delete recipient {@identity} from broadcast list {@broadcastList}", recipientIdentity, listName);
                throw;
            }
        }

        /// <summary>
        /// Gets the list's identity based on the <paramref name="listName"/>
        /// </summary>
        /// <param name="listName"></param>
        /// <returns></returns>
        public Identity GetListIdentity(string listName)
        {
            var broadcastExtension = new BroadcastExtension(_sender);
            var listIdentity = broadcastExtension.GetListIdentity(listName);
            return listIdentity;
        }

        /// <summary>
        /// Returns a list of recipients for the list with the name <paramref name="listName"/>
        /// </summary>
        /// <param name="listName"></param>
        /// <param name="skip">Recipients to skip from the start</param>
        /// <param name="take">Amount of recipients to get</param>
        /// <param name="cancellationToken"></param>
        /// <returns>DocumentCollection with the items being the recovered recipients</returns>
        public async Task<DocumentCollection> GetRecipientsAsync(string listName, int skip = 0, int take = 100, CancellationToken cancellationToken = default(CancellationToken))
        {
            var broadcastExtension = new BroadcastExtension(_sender);
            var recipients = await broadcastExtension.GetRecipientsAsync(listName, skip, take, cancellationToken);
            return recipients;
        }

        /// <summary>
        /// Returns a list of recipients for the list with the name <paramref name="listName"/>
        /// </summary>
        /// <param name="listName"></param>
        /// <param name="skip">Recipients to skip from the start</param>
        /// <param name="take">Amount of recipients to get</param>
        /// <param name="logger"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>DocumentCollection with the items being the recovered recipients</returns>
        /// <exception cref="BlipHttpClientException">Failure getting recipients for the given distribution list</exception>
        /// <exception cref="Exception">Unknown error</exception>
        public async Task<DocumentCollection> GetRecipientsAsync(ILogger logger, string listName, int skip = 0, int take = 100, CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                logger.Information("[GetRecipients] Trying to get recipients from list {@broadcastList} indexed from {@first} to {@last}", listName, skip, skip + take);
                var command = new Command()
                {
                    Method = CommandMethod.Get,
                    Uri = new LimeUri($"/lists/{listName}@broadcast.msging.net/recipients?skip={skip}&take={take}"),
                    To = PostMasterIdentity
                };
                var getRecipientsResponse = await _sender.ProcessCommandAsync(command, cancellationToken);
                if (getRecipientsResponse.Status != CommandStatus.Success)
                {
                    throw new BlipHttpClientException("Failed to get recipients from broadcast list.", getRecipientsResponse);
                }
                logger.Information("[GetRecipients] Got recipients from list {@broadcastList} indexed from {@first} to {@last}", listName, skip, skip + take);

                return getRecipientsResponse.Resource as DocumentCollection;
            }
            catch (BlipHttpClientException bex)
            {
                logger.Error(bex, "[GetRecipients] Failed get recipients from broadcast list {@broadcastList}", listName);
                throw;
            }
            catch (Exception ex)
            {
                logger.Error(ex, "[GetRecipients] Failed get recipients from broadcast list {@broadcastList}", listName);
                throw;
            }
        }

        /// <summary>
        /// Returns a given number <paramref name="take"/> (with a <paramref name="skip"/> offset) of recipients
        /// </summary>
        /// <param name="skip"></param>
        /// <param name="take"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<DocumentCollection> GetRecipientsAsynGetAllDistributionListsAsync(int skip = 0, int take = 100, CancellationToken cancellationToken = default(CancellationToken))
        {
            var broadcastExtension = new BroadcastExtension(_sender);
            var allDistributionListsRecipients = await broadcastExtension.GetRecipientsAsynGetAllDistributionListsAsync(skip, take, cancellationToken);
            return allDistributionListsRecipients;
        }

        /// <summary>
        /// Checks if the given <paramref name="listName"/> has the recipient <paramref name="recipientIdentity"/>
        /// </summary>
        /// <param name="listName"></param>
        /// <param name="recipientIdentity"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>Boolean indicating if the recipient is or not on the list</returns>
        public async Task<bool> HasRecipientAsync(string listName, Identity recipientIdentity, CancellationToken cancellationToken = default(CancellationToken))
        {
            var broadcastExtension = new BroadcastExtension(_sender);
            var hasRecipient = await broadcastExtension.HasRecipientAsync(listName, recipientIdentity, cancellationToken);
            return hasRecipient;
        }

        /// <summary>
        /// Sends a message to the given broadcast list
        /// </summary>
        /// <param name="listName"></param>
        /// <param name="content">Message to be sent</param>
        /// <param name="id"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task SendMessageAsync(string listName, Document content, string id = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            var broadcastExtension = new BroadcastExtension(_sender);
            await broadcastExtension.SendMessageAsync(listName, content, id, cancellationToken);
        }

        /// <summary>
        /// Sends a message to the given broadcast list
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="cancellationToken"></param>
        /// <param name="listName"></param>
        /// <param name="content">Message to be sent</param>
        /// <param name="id"></param>
        /// <returns>Lime <c>Command</c> response object</returns>
        /// <exception cref="BlipHttpClientException">Failure sending message to distribution list</exception>
        /// <exception cref="Exception">Unknown error</exception>
        public async Task SendMessageAsync(string listName, Document content, ILogger logger, string id = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                logger.Information("[SendBroadcast] Trying to send message to the recipients of the list {@broadcastList}", listName, content);
                var message = new Message()
                {
                    To = $"{listName}@broadcast.msging.net",
                    Content = content
                };
                await _sender.SendMessageAsync(message, cancellationToken);
                logger.Information("[SendBroadcast] Sent message to the recipients of the list {@broadcastList}", listName, content);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "[SendBroadcast] Failed send message to the recipients of the list {@broadcastList}", listName, content);
                throw;
            }
        }
    }
}
