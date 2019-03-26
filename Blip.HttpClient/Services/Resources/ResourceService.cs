using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Blip.HttpClient.Factories;
using Lime.Protocol;
using Serilog;
using Take.Blip.Client;

namespace Blip.HttpClient.Services.Resources
{
    public class ResourceService : IResourceService
    {
        private readonly ISender _sender;
        public ResourceService(ISender sender)
        {
            _sender = sender;
        }

        public ResourceService(string authKey)
        {
            var factory = new BlipHttpClientFactory();
            _sender = factory.BuildBlipHttpClient(authKey);
        }

        public async Task DeleteAsync(string id, CancellationToken cancellationToken = default(CancellationToken))
        {
            var resourceExtension = new ResourceService(_sender);
            await resourceExtension.DeleteAsync(id, cancellationToken);
        }

        public async Task DeleteAsync(string id, ILogger logger, CancellationToken cancellationToken = default(CancellationToken))
        {
            throw new NotImplementedException();
        }

        public async Task<T> GetAsync<T>(string id, CancellationToken cancellationToken = default(CancellationToken)) where T : Document
        {
            var resourceExtension = new ResourceService(_sender);
            var resource = await resourceExtension.GetAsync<T>(id, cancellationToken);
            return resource;
        }

        public Task<T> GetAsync<T>(string id, ILogger logger, CancellationToken cancellationToken = default(CancellationToken)) where T : Document
        {
            throw new NotImplementedException();
        }

        public async Task<DocumentCollection> GetIdsAsync(int skip = 0, int take = 100, CancellationToken cancellationToken = default(CancellationToken))
        {
            var resourceExtension = new ResourceService(_sender);
            var resource = await resourceExtension.GetIdsAsync(skip, take, cancellationToken);
            return resource;
        }

        public Task<DocumentCollection> GetIdsAsync(ILogger logger, int skip = 0, int take = 100, CancellationToken cancellationToken = default(CancellationToken))
        {
            throw new NotImplementedException();
        }

        public async Task SetAsync<T>(string id, T document, TimeSpan expiration = default(TimeSpan), CancellationToken cancellationToken = default(CancellationToken)) where T : Document
        {
            var resourceExtension = new ResourceService(_sender);
            await resourceExtension.SetAsync<T>(id, document, expiration, cancellationToken);
        }

        public Task SetAsync<T>(string id, T document, ILogger logger, TimeSpan expiration = default(TimeSpan), CancellationToken cancellationToken = default(CancellationToken)) where T : Document
        {
            throw new NotImplementedException();
        }
    }
}
