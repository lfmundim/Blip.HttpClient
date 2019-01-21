using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Blip.HttpClient.Exceptions;
using Blip.HttpClient.Factories;
using Lime.Protocol;
using Serilog;
using Take.Blip.Client;
using Take.Blip.Client.Extensions.Broadcast;

namespace Blip.HttpClient.Services
{
    public class BroadcastService : IBroadcastService
    {
        private readonly ISender _sender;

        public BroadcastService(ISender sender)
        {
            _sender = sender;
        }

        public BroadcastService(string authKey)
        {
            var factory = new BlipHttpClientFactory();
            _sender = factory.BuildBlipHttpClient(authKey);
        }

        public async Task AddRecipientAsync(string listName, Identity recipientIdentity, CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                var broadcastExtension = new BroadcastExtension(_sender);
                await broadcastExtension.AddRecipientAsync(listName, recipientIdentity, cancellationToken);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task AddRecipientAsync(string listName, Identity recipientIdentity, CancellationToken cancellationToken, ILogger logger)
        {
            throw new NotImplementedException();
        }

        public async Task CreateDistributionListAsync(string listName, CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                var broadcastExtension = new BroadcastExtension(_sender);
                await broadcastExtension.CreateDistributionListAsync(listName, cancellationToken);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task CreateDistributionListAsync(string listName, CancellationToken cancellationToken, ILogger logger)
        {
            throw new NotImplementedException();
        }

        public async Task DeleteDistributionListAsync(string listName, CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                var broadcastExtension = new BroadcastExtension(_sender);
                await broadcastExtension.DeleteDistributionListAsync(listName, cancellationToken);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task DeleteDistributionListAsync(string listName, CancellationToken cancellationToken, ILogger logger)
        {
            throw new NotImplementedException();
        }

        public async Task DeleteRecipientAsync(string listName, Identity recipientIdentity, CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                var broadcastExtension = new BroadcastExtension(_sender);
                await broadcastExtension.DeleteRecipientAsync(listName, recipientIdentity, cancellationToken);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task DeleteRecipientAsync(string listName, Identity recipientIdentity, CancellationToken cancellationToken, ILogger logger)
        {
            throw new NotImplementedException();
        }

        public Identity GetListIdentity(string listName)
        {
            try
            {
                var broadcastExtension = new BroadcastExtension(_sender);
                var listIdentity = broadcastExtension.GetListIdentity(listName);
                return listIdentity;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public Identity GetListIdentity(string listName, ILogger logger)
        {
            throw new NotImplementedException();
        }

        public async Task<DocumentCollection> GetRecipientsAsync(string listName, int skip = 0, int take = 100, CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                var broadcastExtension = new BroadcastExtension(_sender);
                var recipients = await broadcastExtension.GetRecipientsAsync(listName, skip, take, cancellationToken);
                return recipients;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<DocumentCollection> GetRecipientsAsync(ILogger logger, CancellationToken cancellationToken, string listName, int skip = 0, int take = 100)
        {
            throw new NotImplementedException();
        }

        public async Task<DocumentCollection> GetRecipientsAsynGetAllDistributionListsAsync(int skip = 0, int take = 100, CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                var broadcastExtension = new BroadcastExtension(_sender);
                var allDistributionListsRecipients = await broadcastExtension.GetRecipientsAsynGetAllDistributionListsAsync(skip, take, cancellationToken);
                return allDistributionListsRecipients;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<DocumentCollection> GetRecipientsAsynGetAllDistributionListsAsync(ILogger logger, CancellationToken cancellationToken, int skip = 0, int take = 100)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> HasRecipientAsync(string listName, Identity recipientIdentity, CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                var broadcastExtension = new BroadcastExtension(_sender);
                var hasRecipient = await broadcastExtension.HasRecipientAsync(listName, recipientIdentity, cancellationToken);
                return hasRecipient;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<bool> HasRecipientAsync(string listName, Identity recipientIdentity, CancellationToken cancellationToken, ILogger logger)
        {
            throw new NotImplementedException();
        }

        public async Task SendMessageAsync(string listName, Document content, string id = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                var broadcastExtension = new BroadcastExtension(_sender);
                await broadcastExtension.SendMessageAsync(listName, content, id, cancellationToken);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task SendMessageAsync(ILogger logger, CancellationToken cancellationToken, string listName, Document content, string id = null)
        {
            throw new NotImplementedException();
        }
    }
}
