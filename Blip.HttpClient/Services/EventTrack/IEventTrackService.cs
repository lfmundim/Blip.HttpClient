using Blip.HttpClient.Exceptions;
using Lime.Messaging.Resources;
using Lime.Protocol;
using Serilog;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Take.Blip.Client.Extensions.EventTracker;

namespace Blip.HttpClient.Services.EventTrack
{
    /// <summary>
    /// Service responsible for managing trackings
    /// </summary>
    public interface IEventTrackService : IEventTrackExtension
    {
        /// <summary>
        /// Add a tracking to AnalyticsAddress: postmaster@analytics.msging.net
        /// </summary>
        /// <param name="category"></param>
        /// <param name="action"></param>
        /// <param name="extras"></param>
        /// <param name="cancellationToken"></param>
        /// <param name="identity"></param>
        /// <param name="logger"></param>
        /// <exception cref="BlipHttpClientException">Failure sending message to distribution list</exception>
        /// <exception cref="Exception">Unknown error</exception>
        Task AddAsync(string category, string action, ILogger logger, IDictionary<string, string> extras = null, CancellationToken cancellationToken = default(CancellationToken), Identity identity = null);

        /// <summary>
        /// Add a tracking to AnalyticsAddress: postmaster@analytics.msging.net
        /// </summary>
        /// <param name="category"></param>
        /// <param name="action"></param>
        /// <param name="label"></param>
        /// <param name="message"></param>
        /// <param name="contact"></param>
        /// <param name="contactExternalId"></param>
        /// <param name="value"></param>
        /// <param name="extras"></param>
        /// <param name="fireAndForget"></param>
        /// <param name="cancellationToken"></param>
        /// <param name="logger"></param>
        /// <exception cref="BlipHttpClientException">Failure sending message to distribution list</exception>
        /// <exception cref="Exception">Unknown error</exception>
        Task AddAsync(string category, string action, ILogger logger, string label = null, Message message = null, Contact contact = null, string contactExternalId = null, decimal? value = null, IDictionary<string, string> extras = null, bool fireAndForget = false, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Add a tracking to AnalyticsAddress: postmaster@analytics.msging.net
        /// </summary>
        /// <param name="category"></param>
        /// <param name="action"></param>
        /// <param name="label"></param>
        /// <param name="messageId"></param>
        /// <param name="contactIdentity"></param>
        /// <param name="contactSource"></param>
        /// <param name="contactGroup"></param>
        /// <param name="contactExternalId"></param>
        /// <param name="value"></param>
        /// <param name="extras"></param>
        /// <param name="fireAndForget"></param>
        /// <param name="cancellationToken"></param>
        /// <param name="logger"></param>
        /// <exception cref="BlipHttpClientException">Failure sending message to distribution list</exception>
        /// <exception cref="Exception">Unknown error</exception>
        Task AddAsync(string category, string action, ILogger logger, string label = null, string messageId = null, string contactIdentity = null, string contactSource = null, string contactGroup = null, string contactExternalId = null, decimal? value = null, IDictionary<string, string> extras = null, bool fireAndForget = false, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Get all trackings of some category and action in a period of time
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="category"></param>
        /// <param name="action"></param>
        /// <param name="skip"></param>
        /// <param name="take"></param>
        /// <param name="cancellationToken"></param>
        /// <param name="logger"></param>
        /// <returns><c>DocumentCollection</c> response object</returns>
        /// <exception cref="BlipHttpClientException">Failure sending message to distribution list</exception>
        /// <exception cref="Exception">Unknown error</exception>
        Task<DocumentCollection> GetAllAsync(DateTimeOffset startDate, DateTimeOffset endDate, string category, string action, ILogger logger, int skip = 0, int take = 20, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Get all trackings categories
        /// </summary>
        /// <param name="take"></param>
        /// <param name="logger"></param>
        /// <param name="cancellationToken"></param>
        /// <returns><c>DocumentCollection</c></returns>
        /// <exception cref="BlipHttpClientException">Failure sending message to distribution list</exception>
        /// <exception cref="Exception">Unknown error</exception>
        Task<DocumentCollection> GetCategoriesAsync(int take = 20, ILogger logger, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Get all actions for a tracking category
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="category"></param>
        /// <param name="logger"></param>
        /// <param name="take"></param>
        /// <param name="cancellationToken"></param>
        /// <returns><c>DocumentCollection</c></returns>
        /// <exception cref="BlipHttpClientException">Failure sending message to distribution list</exception>
        /// <exception cref="Exception">Unknown error</exception>
        Task<DocumentCollection> GetCategoryActionsCounterAsync(DateTimeOffset startDate, DateTimeOffset endDate, string category, ILogger logger, int take = 20, CancellationToken cancellationToken = default(CancellationToken));
    }
}
