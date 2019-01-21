using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Blip.HttpClient.Exceptions;
using Blip.HttpClient.Factories;
using Lime.Protocol;
using Take.Blip.Client;
using Take.Blip.Client.Extensions.Broadcast;

namespace Blip.HttpClient.Services
{
    public class BroadcastService : IBroadcastService, IBroadcastExtension
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
                var command = 
            }
            catch (BlipHttpClientException bex)
            {

            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public Task CreateDistributionListAsync(string listName, CancellationToken cancellationToken = default(CancellationToken))
        {
            throw new NotImplementedException();
        }

        public Task DeleteDistributionListAsync(string listName, CancellationToken cancellationToken = default(CancellationToken))
        {
            throw new NotImplementedException();
        }

        public Task DeleteRecipientAsync(string listName, Identity recipientIdentity, CancellationToken cancellationToken = default(CancellationToken))
        {
            throw new NotImplementedException();
        }

        public Identity GetListIdentity(string listName)
        {
            throw new NotImplementedException();
        }

        public Task<DocumentCollection> GetRecipientsAsync(string listName, int skip = 0, int take = 100, CancellationToken cancellationToken = default(CancellationToken))
        {
            throw new NotImplementedException();
        }

        public Task<DocumentCollection> GetRecipientsAsynGetAllDistributionListsAsync(int skip = 0, int take = 100, CancellationToken cancellationToken = default(CancellationToken))
        {
            throw new NotImplementedException();
        }

        public Task<bool> HasRecipientAsync(string listName, Identity recipientIdentity, CancellationToken cancellationToken = default(CancellationToken))
        {
            throw new NotImplementedException();
        }

        public Task SendMessageAsync(string listName, Document content, string id = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            throw new NotImplementedException();
        }
    }
}
