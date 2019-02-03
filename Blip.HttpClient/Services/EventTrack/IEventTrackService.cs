using Blip.HttpClient.Exceptions;
using Lime.Messaging.Resources;
using Lime.Protocol;
using Serilog;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Take.Blip.Client.Extensions.EventTracker;

namespace Blip.HttpClient.Services.EventTracker
{
    /// <summary>
    /// Service responsible for managing trackings
    /// </summary>
    public interface IEventTrackService : IEventTrackExtension
    {
        /// <summary>
        /// Add a tracking to AnalyticsAddress: postmaster@analytics.msging.net
        /// </summary>
        /// <param name="category">Category to aggregate the related events</param>
        /// <param name="action">The action associated to the event</param>
        /// <param name="logger"></param>
        /// <param name="extras">Extra information about the events</param>
        /// <param name="cancellationToken"></param>
        /// <param name="identity">The identity of contact associated to the event</param>
        Task<Command> AddAsync(string category, string action, ILogger logger, IDictionary<string, string> extras = null, CancellationToken cancellationToken = default(CancellationToken), Identity identity = null);

        /// <summary>
        /// Add a tracking to AnalyticsAddress: postmaster@analytics.msging.net
        /// </summary>
        /// <param name="category">Category to aggregate the related events</param>
        /// <param name="action">The action associated to the event</param>
        /// <param name="logger"></param>
        /// <param name="label"></param>
        /// <param name="message"></param>
        /// <param name="contact">The contact associated to the event</param>
        /// <param name="contactExternalId"></param>
        /// <param name="value"></param>
        /// <param name="extras">Extra information about the events</param>
        /// <param name="fireAndForget"></param>
        /// <param name="cancellationToken"></param>
        Task<Command> AddAsync(string category, string action, ILogger logger, string label = null, Message message = null, Contact contact = null, string contactExternalId = null, decimal? value = null, IDictionary<string, string> extras = null, bool fireAndForget = false, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Add a tracking to AnalyticsAddress: postmaster@analytics.msging.net
        /// </summary>
        /// <param name="category">Category to aggregate the related events</param>
        /// <param name="action">The action associated to the event</param>
        /// <param name="logger"></param>
        /// <param name="label"></param>
        /// <param name="messageId"></param>
        /// <param name="contactIdentity">The identy of the contact associated to the event</param>
        /// <param name="contactSource"></param>
        /// <param name="contactGroup"></param>
        /// <param name="contactExternalId"></param>
        /// <param name="value"></param>
        /// <param name="extras">Extra information about the events</param>
        /// <param name="fireAndForget"></param>
        /// <param name="cancellationToken"></param>
        /// <exception cref="BlipHttpClientException">Failure sending message to distribution list</exception>
        /// <exception cref="Exception">Unknown error</exception>
        Task<Command> AddAsync(string category, string action, ILogger logger, string label = null, string messageId = null, string contactIdentity = null, string contactSource = null, string contactGroup = null, string contactExternalId = null, decimal? value = null, IDictionary<string, string> extras = null, bool fireAndForget = false, CancellationToken cancellationToken = default(CancellationToken));
    }
}
