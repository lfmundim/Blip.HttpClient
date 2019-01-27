using Blip.HttpClient.Exceptions;
using Blip.HttpClient.Factories;
using Lime.Protocol;
using Serilog;
using System;
using System.Threading;
using System.Threading.Tasks;
using Take.Blip.Client;
using Take.Blip.Client.Extensions.Bucket;

namespace Blip.HttpClient.Services.Bucket
{
    /// <summary>
    /// Allows the storage of documents in the server on an isolated chatbot's container
    /// </summary>
    public class BucketService : IBucketService
    {
        private readonly ISender _sender;

        /// <summary>
        /// Creates an instance of the BucketService using a BLiP Client
        /// </summary>
        /// <param name="sender">ISender instance, from BlipHttpClientFactory</param>
        public BucketService(ISender sender)
        {
            _sender = sender;
        }

        /// <summary>
        /// Creates a BucketService instance using a given authKey.
        /// </summary>
        /// <param name="authKey">Bot authorization key</param>
        public BucketService(string authKey)
        {
            var factory = new BlipHttpClientFactory();
            _sender = factory.BuildBlipHttpClient(authKey);
        }

        /// <summary>
        /// Gets a document from the bucket using the given <paramref name="id"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>Lime <c>Document</c> of the sub-type set on <c>T</c></returns>
        public async Task<T> GetAsync<T>(string id, CancellationToken cancellationToken = new CancellationToken()) where T : Document
        {
            var bucketExtension = new BucketExtension(_sender);
            return await bucketExtension.GetAsync<T>(id, cancellationToken);
        }

        /// <summary>
        /// Gets the bucket documents IDs from index <paramref name="skip"/> to <paramref name="take"/>
        /// </summary>
        /// <param name="skip"></param>
        /// <param name="take"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>A <c>DocumentCollection</c> of the recovered IDs</returns>
        public async Task<DocumentCollection> GetIdsAsync(int skip = 0, int take = 100, CancellationToken cancellationToken = new CancellationToken())
        {
            var bucketExtension = new BucketExtension(_sender);
            return await bucketExtension.GetIdsAsync(skip, take, cancellationToken);
        }

        /// <summary>
        /// Stores a <paramref name="document"/> on the bucket
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        /// <param name="document"></param>
        /// <param name="expiration"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task SetAsync<T>(string id, T document, TimeSpan expiration = new TimeSpan(), CancellationToken cancellationToken = new CancellationToken()) where T : Document
        {
            var bucketExtension = new BucketExtension(_sender);
            await bucketExtension.SetAsync(id, document, expiration, cancellationToken);
        }

        /// <summary>
        /// Deletes the document with the given <paramref name="id"/> from the Bucket
        /// </summary>
        /// <param name="id"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task DeleteAsync(string id, CancellationToken cancellationToken = new CancellationToken())
        {
            var bucketExtension = new BucketExtension(_sender);
            await bucketExtension.DeleteAsync(id, cancellationToken);
        }

        /// <summary>
        /// Gets a document from the bucket using the given <paramref name="id"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        /// <param name="logger"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>Lime <c>Document</c> of the sub-type set on <c>T</c></returns>
        /// <exception cref="BlipHttpClientException">Failure getting <c>Document</c> from the Bucket</exception>
        /// <exception cref="Exception">Unknown error</exception>
        public async Task<T> GetAsync<T>(string id, ILogger logger, CancellationToken cancellationToken = new CancellationToken()) where T : Document
        {
            try
            {
                logger.Information("[BucketGet] Trying to get document with id {@documentId} from the bucket", id);
                var command = new Command()
                {
                    Method = CommandMethod.Get,
                    Uri = new LimeUri($"/buckets/{id}")
                };

                var getDocumentResponse = await _sender.ProcessCommandAsync(command, cancellationToken);
                if (getDocumentResponse.Status != CommandStatus.Success)
                {
                    throw new BlipHttpClientException("Failed to get document from the bucket", getDocumentResponse);
                }
                logger.Information("[BucketGet] Successfully got document from bucket using id {@documentId}: {@document}", id, getDocumentResponse.Resource as T);

                return getDocumentResponse.Resource as T;
            }
            catch (BlipHttpClientException bex)
            {
                logger.Error(bex, "[BucketGet] Failed to get document with id {@documentId} from bucket", id);
                throw;
            }
            catch (Exception ex)
            {
                logger.Error(ex, "[BucketGet] Failed to get document with id {@documentId} from bucket", id);
                throw;
            }
        }

        /// <summary>
        /// Stores a <paramref name="document"/> on the bucket
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        /// <param name="document"></param>
        /// <param name="logger"></param>
        /// <param name="expiration"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>Lime <c>Command</c> response object</returns>
        /// <exception cref="BlipHttpClientException">Failure setting <c>Document</c> on the Bucket</exception>
        /// <exception cref="Exception">Unknown error</exception>
        public async Task<Command> SetAsync<T>(string id, T document, ILogger logger, TimeSpan expiration = new TimeSpan(), CancellationToken cancellationToken = new CancellationToken()) where T : Document
        {
            try
            {
                logger.Information("[BucketSet] Trying to set document {@document} on the bucket with id {@documentId}", document, id);
                var command = new Command()
                {
                    Method = CommandMethod.Set,
                    Uri = new LimeUri($"/buckets/{id}"),
                    Resource = document
                };

                var setDocumentResponse = await _sender.ProcessCommandAsync(command, cancellationToken);
                if (setDocumentResponse.Status != CommandStatus.Success)
                {
                    throw new BlipHttpClientException("Failed to set document on the bucket", setDocumentResponse);
                }
                logger.Information("[BucketSet] Successfully set document {@document} on the bucket using id {@documentId}", document, id);

                return setDocumentResponse;
            }
            catch (BlipHttpClientException bex)
            {
                logger.Error(bex, "[BucketSet] Failed to set document {@document} with id {@documentId} on the bucket", document, id);
                throw;
            }
            catch (Exception ex)
            {
                logger.Error(ex, "[BucketSet] Failed to set document {@document} with id {@documentId} on the bucket", document, id);
                throw;
            }
        }

        /// <summary>
        /// Deletes the document with the given <paramref name="id"/> from the Bucket
        /// </summary>
        /// <param name="id"></param>
        /// <param name="logger"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>Lime <c>Command</c> response object</returns>
        /// <exception cref="BlipHttpClientException">Failure deleting <c>Document</c> from the Bucket</exception>
        /// <exception cref="Exception">Unknown error</exception>
        public async Task<Command> DeleteAsync(string id, ILogger logger, CancellationToken cancellationToken = new CancellationToken())
        {
            try
            {
                logger.Information("[BucketDelete] Trying to delete document with id {@documentId} from the bucket", id);
                var command = new Command()
                {
                    Method = CommandMethod.Delete,
                    Uri = new LimeUri($"/buckets/{id}")
                };

                var deleteDocumentResponse = await _sender.ProcessCommandAsync(command, cancellationToken);
                if (deleteDocumentResponse.Status != CommandStatus.Success)
                {
                    throw new BlipHttpClientException("Failed to delete document from the bucket", deleteDocumentResponse);
                }
                logger.Information("[BucketDelete] Successfully deleted document from bucket using id {@documentId}", id);

                return deleteDocumentResponse;
            }
            catch (BlipHttpClientException bex)
            {
                logger.Error(bex, "[BucketDelete] Failed to delete document with id {@documentId} from bucket", id);
                throw;
            }
            catch (Exception ex)
            {
                logger.Error(ex, "[BucketDelete] Failed to delete document with id {@documentId} from bucket", id);
                throw;
            }
        }
    }
}
