using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Blip.HttpClient.Exceptions;
using Blip.HttpClient.Factories;
using Lime.Messaging.Resources;
using Lime.Protocol;
using Serilog;
using Take.Blip.Client;
using Take.Blip.Client.Extensions.EventTracker;
using Takenet.Iris.Messaging.Resources;
using Takenet.Iris.Messaging.Resources.Analytics;

namespace Blip.HttpClient.Services.EventTracker
{
    /// <summary>
    /// Service responsible for managing trackings
    /// </summary>
    public class EventTrackService : IEventTrackService
    {
        private const string AnalyticsAddress = "postmaster@analytics.msging.net";
        private const string EventTrackUri = "/event-track";
        private readonly ISender _sender;

        /// <summary>
        /// Creates an instance of the EventTrackService using a BLiP Client
        /// </summary>
        /// <param name="sender">ISender instance, from BlipHttpClientFactory</param>
        public EventTrackService(ISender sender)
        {
            _sender = sender;
        }

        /// <summary>
        /// Creates a EventTrackService instance using a given authKey.
        /// </summary>
        /// <param name="authKey">Bot authorization key</param>
        public EventTrackService(string authKey)
        {
            var factory = new BlipHttpClientFactory();
            _sender = factory.BuildBlipHttpClient(authKey);
        }

        /// <summary>
        /// Add a tracking to AnalyticsAddress: postmaster@analytics.msging.net
        /// </summary>
        /// <param name="category">Category to aggregate the related events</param>
        /// <param name="action">The action associated to the event</param>
        /// <param name="extras">Extra information about the events</param>
        /// <param name="cancellationToken"></param>
        /// <param name="identity">The contact associated with this event</param>
        public async Task AddAsync(string category, string action, IDictionary<string, string> extras = null, CancellationToken cancellationToken = default(CancellationToken), Identity identity = null)
        {
            var eventTrackExtension = new EventTrackExtension(_sender);
            await eventTrackExtension.AddAsync(category, action, extras, cancellationToken, identity);
        }

        /// <summary>
        /// Add a tracking to AnalyticsAddress: postmaster@analytics.msging.net
        /// </summary>
        /// <param name="category">Category to aggregate the related events</param>
        /// <param name="action">The action associated to the event</param>
        /// <param name="logger"></param>
        /// <param name="extras">Extra information about the events</param>
        /// <param name="cancellationToken"></param>
        /// <param name="identity">The identity of contact associated to the event</param>
        public async Task<Command> AddAsync(string category, string action, ILogger logger, IDictionary<string, string> extras = null, CancellationToken cancellationToken = default(CancellationToken), Identity identity = null)
        {
            return await AddAsync(category, action, logger, contactIdentity: identity, extras: extras, cancellationToken: cancellationToken);
        }

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
        public async Task AddAsync(string category, string action, string label = null, Message message = null, Contact contact = null, string contactExternalId = null, decimal? value = null, IDictionary<string, string> extras = null, bool fireAndForget = false, CancellationToken cancellationToken = default(CancellationToken))
        {
            var eventTrackExtension = new EventTrackExtension(_sender);
            await eventTrackExtension.AddAsync(category, action, label, message, contact, contactExternalId, value, extras, fireAndForget, cancellationToken);
        }

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
        public async Task<Command> AddAsync(string category, string action, ILogger logger, string label = null, Message message = null, Contact contact = null, string contactExternalId = null, decimal? value = null, IDictionary<string, string> extras = null, bool fireAndForget = false, CancellationToken cancellationToken = default(CancellationToken))
        {
            return await AddAsync(category, action, logger, contactIdentity: contact?.Identity, extras: extras, cancellationToken: cancellationToken);
        }

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
        public async Task AddAsync(string category, string action, string label = null, string messageId = null, string contactIdentity = null, string contactSource = null, string contactGroup = null, string contactExternalId = null, decimal? value = null, IDictionary<string, string> extras = null, bool fireAndForget = false, CancellationToken cancellationToken = default(CancellationToken))
        {
            var eventTrackExtension = new EventTrackExtension(_sender);
            await eventTrackExtension.AddAsync(category, action, label, messageId, contactIdentity, contactSource, contactGroup, contactExternalId, value, extras, fireAndForget, cancellationToken);
        }

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
        public async Task<Command> AddAsync(string category, string action, ILogger logger, string label = null, string messageId = null, string contactIdentity = null, string contactSource = null, string contactGroup = null, string contactExternalId = null, decimal? value = null, IDictionary<string, string> extras = null, bool fireAndForget = false, CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                logger.Information("[AddAsync] Trying to add tracking: {@category} {@action}", category, action);

                var command = new Command
                {
                    Uri = new LimeUri(EventTrackUri),
                    To = AnalyticsAddress,
                    Resource = new EventTrack
                    {
                        Category = category,
                        Action = action,
                        Contact = new EventContact
                        {
                            ExternalId = contactExternalId,
                            Group = contactGroup,
                            Identity = contactIdentity,
                            Source = contactSource
                        },
                    },
                };

                if (fireAndForget)
                {
                    command.Method = CommandMethod.Observe;
                    return await _sender.ProcessCommandAsync(command, cancellationToken);
                }

                command.Method = CommandMethod.Set;
                command.Id = EnvelopeId.NewId();

                var addAsyncReponse = await _sender.ProcessCommandAsync(command, cancellationToken);
                if (addAsyncReponse.Status != CommandStatus.Success)
                {
                    throw new BlipHttpClientException("Failed to add tracking", addAsyncReponse);
                }

                logger.Information("[AddAsync] Added track {@category} {@action}", category, action);
                return addAsyncReponse;

            }
            catch (BlipHttpClientException bex)
            {
                logger.Error(bex, "[AddAsync] Failed to add track {@category} {@action}", category, action);
                throw;
            }
            catch (Exception ex)
            {
                logger.Error(ex, "[AddAsync] Failed to add track {@category} {@action}", category, action);
                throw;
            }
        }

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
        /// <returns><c>DocumentCollection</c> response object</returns>
        public async Task<DocumentCollection> GetAllAsync(DateTimeOffset startDate, DateTimeOffset endDate, string category, string action, int skip = 0, int take = 20, CancellationToken cancellationToken = default(CancellationToken))
        {
            var eventTrackExtension = new EventTrackExtension(_sender);
            return await eventTrackExtension.GetAllAsync(startDate, endDate, category, action, skip, take, cancellationToken);
        }


        /// <summary>
        /// Get all trackings categories
        /// </summary>
        /// <param name="take"></param>
        /// <param name="cancellationToken"></param>
        /// <returns><c>DocumentCollection</c></returns>
        public async Task<DocumentCollection> GetCategoriesAsync(int take = 20, CancellationToken cancellationToken = default(CancellationToken))
        {
            var eventTrackExtension = new EventTrackExtension(_sender);
            return await eventTrackExtension.GetCategoriesAsync(take, cancellationToken);
        }

        /// <summary>
        /// Get all actions for a tracking category
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="category"></param>
        /// <param name="take"></param>
        /// <param name="cancellationToken"></param>
        /// <returns><c>DocumentCollection</c></returns>
        public async Task<DocumentCollection> GetCategoryActionsCounterAsync(DateTimeOffset startDate, DateTimeOffset endDate, string category, int take = 20, CancellationToken cancellationToken = default(CancellationToken))
        {
            var eventTrackExtension = new EventTrackExtension(_sender);
            return await eventTrackExtension.GetCategoryActionsCounterAsync(startDate, endDate, category, take, cancellationToken);
        }
    }
}
