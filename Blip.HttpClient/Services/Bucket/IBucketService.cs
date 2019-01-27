using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Blip.HttpClient.Exceptions;
using Lime.Protocol;
using Serilog;
using Take.Blip.Client.Extensions.Bucket;

namespace Blip.HttpClient.Services.Bucket
{
    /// <summary>
    /// Allows the storage of documents in the server on an isolated chatbot's container
    /// </summary>
    public interface IBucketService : IBucketExtension
    {
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
        Task<T> GetAsync<T>(string id, ILogger logger, CancellationToken cancellationToken = new CancellationToken()) where T : Document;

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
        Task<Command> SetAsync<T>(string id, T document, ILogger logger, TimeSpan expiration = new TimeSpan(), CancellationToken cancellationToken = new CancellationToken()) where T : Document;

        /// <summary>
        /// Deletes the document with the given <paramref name="id"/> from the Bucket
        /// </summary>
        /// <param name="id"></param>
        /// <param name="logger"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>Lime <c>Command</c> response object</returns>
        /// <exception cref="BlipHttpClientException">Failure deleting <c>Document</c> from the Bucket</exception>
        /// <exception cref="Exception">Unknown error</exception>
        Task<Command> DeleteAsync(string id, ILogger logger, CancellationToken cancellationToken = new CancellationToken());
    }
}
