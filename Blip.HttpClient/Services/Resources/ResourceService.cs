using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Blip.HttpClient.Exceptions;
using Blip.HttpClient.Factories;
using Blip.HttpClient.Models;
using Lime.Protocol;
using Serilog;
using Take.Blip.Client;
using Take.Blip.Client.Extensions.Resource;
using Takenet.Iris.Messaging.Contents;

namespace Blip.HttpClient.Services.Resources
{
    /// <summary>
    /// Allows the storage of documents in the server in an isolated space for each chatbot
    /// </summary>
    public class ResourceService : IResourceService
    {
        private readonly ISender _sender;
        /// <summary>
        /// ResourceService builder using an already instantiated <paramref name="sender"/>
        /// </summary>
        /// <param name="sender"></param>
        public ResourceService(ISender sender)
        {
            _sender = sender;
        }

        /// <summary>
        /// ResourceService builder using a given <paramref name="authKey"/> to instantiate the sender on demand
        /// </summary>
        /// <param name="authKey"></param>
        /// <param name="blipProtocol">Protocol to be used</param>
        public ResourceService(string authKey, BlipProtocol blipProtocol = BlipProtocol.Tcp)
        {
            var factory = new BlipClientFactory();
            _sender = factory.BuildBlipClient(authKey, blipProtocol);
        }

        /// <summary>
        /// Deletes the resource with the given <paramref name="id"/>
        /// </summary>
        /// <param name="id">Unique id for the resource to be deleted</param>
        /// <param name="cancellationToken"></param>
        public async Task DeleteAsync(string id, CancellationToken cancellationToken = default(CancellationToken))
        {
            var resourceExtension = new ResourceExtension(_sender);
            await resourceExtension.DeleteAsync(id, cancellationToken);
        }

        /// <summary>
        /// Deletes the resource with the given <paramref name="id"/>
        /// </summary>
        /// <param name="id">Unique id for the resource to be deleted</param>
        /// <param name="logger"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>BLiP <c>Command</c> with the response from BLiP's API</returns>
        public async Task<Command> DeleteAsync(string id, ILogger logger, CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                logger.Information("[DeleteResource] Trying delete resource using {@id}", id);
                var command = new Command()
                {
                    Method = CommandMethod.Delete,
                    Uri = new LimeUri($"/resources/{id}")
                };
                var deleteResponse = await _sender.ProcessCommandAsync(command, cancellationToken).ConfigureAwait(false);
                if (deleteResponse.Status != CommandStatus.Success)
                {
                    throw new BlipHttpClientException("Failed to delete resource from BLiP.", deleteResponse);
                }
                logger.Information("[DeleteResource] Successfully deleted resource {@id} from BLiP", id);

                return deleteResponse;
            }
            catch (BlipHttpClientException bex)
            {
                logger.Error(bex, "[DeleteResource] Failed to delete resource with id {@id}", id);
                throw;
            }
            catch (Exception ex)
            {
                logger.Error(ex, "[DeleteResource] Failed to delete resource with id {@id}", id);
                throw;
            }
        }

        /// <summary>
        /// Gets a Resource by its <paramref name="id"/>
        /// </summary>
        /// <typeparam name="T">Type of the resource to be recovered</typeparam>
        /// <param name="id">Unique id for the resource to be recovered</param>
        /// <param name="cancellationToken"></param>
        /// <returns>Recovered resource of type <c>T</c></returns>
        public async Task<T> GetAsync<T>(string id, CancellationToken cancellationToken = default(CancellationToken)) where T : Document
        {
            var resourceExtension = new ResourceExtension(_sender);
            var resource = await resourceExtension.GetAsync<T>(id, cancellationToken).ConfigureAwait(false);
            return resource;
        }

        /// <summary>
        /// Gets a Resource by its <paramref name="id"/>
        /// </summary>
        /// <typeparam name="T">Type of the resource to be recovered</typeparam>
        /// <param name="id">Unique id for the resource to be recovered</param>
        /// <param name="logger"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>Recovered resource of type <c>T</c></returns>
        public async Task<T> GetAsync<T>(string id, ILogger logger, CancellationToken cancellationToken = default(CancellationToken)) where T : Document
        {
            try
            {
                var command = new Command()
                {
                    Method = CommandMethod.Get,
                    Uri = new LimeUri($"/resources/{id}")
                };
                logger?.Information("[GetResource] Trying to get resource using id {@id}", id);
                var resourceResponse = await _sender.ProcessCommandAsync(command, cancellationToken).ConfigureAwait(false);
                if (resourceResponse.Status != CommandStatus.Success)
                {
                    throw new BlipHttpClientException("Failed to get resource from BLiP", resourceResponse);
                }

                logger?.Information("[GetResource] Got resource using {@id}: {@resource}", id,
                    resourceResponse.Resource as T);
                return resourceResponse.Resource as T;
            }
            catch (BlipHttpClientException bex)
            {
                logger?.Error(bex, "[GetResource] Failed to get resource using {@id}", id);
                throw;
            }
            catch (Exception ex)
            {
                logger?.Error(ex, "[GetResource] Failed to get contact using {@id}", id);
                throw;
            }
        }

        /// <summary>
        /// Recovers a given number of Resource IDs 
        /// </summary>
        /// <param name="skip"></param>
        /// <param name="take"></param>
        /// <param name="cancellationToken"></param>
        /// <returns><c>DocumentCollection</c> with the resource IDs recovered</returns>
        public async Task<DocumentCollection> GetIdsAsync(int skip = 0, int take = 100, CancellationToken cancellationToken = default(CancellationToken))
        {
            var resourceExtension = new ResourceExtension(_sender);
            var resource = await resourceExtension.GetIdsAsync(skip, take, cancellationToken).ConfigureAwait(false);
            return resource;
        }

        /// <summary>
        /// Recovers a given number of Resource IDs 
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="skip"></param>
        /// <param name="take"></param>
        /// <param name="cancellationToken"></param>
        /// <returns><c>DocumentCollection</c> with the resource IDs recovered</returns>
        public async Task<DocumentCollection> GetIdsAsync(ILogger logger, int skip = 0, int take = 100, CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                var command = new Command()
                {
                    Method = CommandMethod.Get,
                    Uri = new LimeUri($"/resources")
                };
                logger?.Information("[GetResource] Trying to get resource ids");
                var resourceResponse = await _sender.ProcessCommandAsync(command, cancellationToken).ConfigureAwait(false);
                if (resourceResponse.Status != CommandStatus.Success)
                {
                    throw new BlipHttpClientException("Failed to get resource IDs from BLiP", resourceResponse);
                }

                logger?.Information("[GetResource] Got resource ids",
                    resourceResponse.Resource as DocumentCollection);
                return resourceResponse.Resource as DocumentCollection;
            }
            catch (BlipHttpClientException bex)
            {
                logger?.Error(bex, "[GetResource] Failed to get resource ids");
                throw;
            }
            catch (Exception ex)
            {
                logger?.Error(ex, "[GetResource] Failed to get resource ids");
                throw;
            }
        }

        /// <summary>
        /// Set a <paramref name="document"/> on BLiP's Resources using a given <paramref name="id"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        /// <param name="document"></param>
        /// <param name="expiration"></param>
        /// <param name="cancellationToken"></param>
        public async Task SetAsync<T>(string id, T document, TimeSpan expiration = default(TimeSpan), CancellationToken cancellationToken = default(CancellationToken)) where T : Document
        {
            var resourceExtension = new ResourceExtension(_sender);
            await resourceExtension.SetAsync<T>(id, document, expiration, cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Set a <paramref name="document"/> on BLiP's Resources using a given <paramref name="id"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        /// <param name="document"></param>
        /// <param name="logger"></param>
        /// <param name="expiration"></param>
        /// <param name="cancellationToken"></param>
        /// <returns><c>Command</c> with BLiP's response</returns>
        public async Task<Command> SetAsync<T>(string id, T document, ILogger logger, TimeSpan expiration = default(TimeSpan), CancellationToken cancellationToken = default(CancellationToken)) where T : Document
        {
            try
            {
                var command = new Command()
                {
                    Method = CommandMethod.Set,
                    Uri = new LimeUri($"/resources/{id}"),
                    Resource = document
                };
                logger?.Information("[SetResource] Trying to set resource [{@document}] with id {@id}", document, id);
                var resourceResponse = await _sender.ProcessCommandAsync(command, cancellationToken).ConfigureAwait(false);
                if (resourceResponse.Status != CommandStatus.Success)
                {
                    throw new BlipHttpClientException("Failed to set resource on BLiP", resourceResponse);
                }

                logger?.Information("[SetResource] Successfully set resource with id {@id}", id);
                return resourceResponse;
            }
            catch (BlipHttpClientException bex)
            {
                logger?.Error(bex, "Failed to set resource [{@document}] with id {@id} on BLiP", document, id);
                throw;
            }
            catch (Exception ex)
            {
                logger?.Error(ex, "Failed to set resource [{@document}] with id {@id} on BLiP", document, id);
                throw;
            }
        }
    }
}
