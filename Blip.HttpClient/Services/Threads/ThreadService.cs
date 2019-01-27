using Blip.HttpClient.Exceptions;
using Lime.Protocol;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Blip.HttpClient.Factories;
using Take.Blip.Client;
using Takenet.Iris.Messaging.Resources;

namespace Blip.HttpClient.Services.Threads
{
    /// <summary>
    /// Service responsible for getting chat history
    /// </summary>
    public class ThreadService : IThreadService
    {
        private readonly ISender _sender;

        /// <summary>
        /// Creates a ThreadService instance using a BLiP client
        /// </summary>
        /// <param name="sender">ISender instance, from BlipHttpClientFactory</param>
        public ThreadService(ISender sender)
        {
            _sender = sender;
        }

        /// <summary>
        /// Creates a ThreadService instance using a given authKey.
        /// </summary>
        /// <param name="authKey">Bot authorization key</param>
        public ThreadService(string authKey)
        {
            var factory = new BlipHttpClientFactory();
            _sender = factory.BuildBlipHttpClient(authKey);
        }

        /// <summary>
        /// Gets the last chatbot's messages in a specific thread. 
        /// </summary>
        /// <param name="userIdentity">User to recover messages</param>
        /// <param name="order"></param>
        /// <param name="logger"></param>
        /// <param name="cancellationToken"></param>
        /// <param name="startDateTime">Date and time for first message</param>
        /// <returns><c>IEnumerable</c> of Lime ThreadMessages</returns>
        /// <exception cref="BlipHttpClientException">Failure getting chat history</exception>
        /// <exception cref="Exception">Unknown error</exception>
        public async Task<IEnumerable<ThreadMessage>> GetHistoryAsync(Identity userIdentity, Enumerations.ChatOrder order, ILogger logger, CancellationToken cancellationToken = default(CancellationToken), DateTime startDateTime = default(DateTime))
        {
            try
            {
                logger.Information("[GetHistory] Trying to get history for user {@userIdentity}", userIdentity);
                var command = new Command
                {
                    Method = CommandMethod.Get,
                    Uri = new LimeUri($"/threads/{userIdentity}")
                };
                var threadsResponse = await _sender.ProcessCommandAsync(command, cancellationToken);
                if (threadsResponse.Status != CommandStatus.Success || threadsResponse.Resource == null)
                {
                    throw new BlipHttpClientException("Failed to get chat history.", threadsResponse);
                }
                logger.Information("[GetHistory] Got history for user {@userIdentity}", userIdentity);

                logger.Information("[GetHistory] Trying to order and trim history for user {@userIdentity}", userIdentity);

                var messages = (threadsResponse.Resource as DocumentCollection)
                    .Items
                    .OfType<ThreadMessage>()
                    .ToArray();

                var history = TrimAndSortMessages(messages, order, startDateTime);
                logger.Information("[GetHistory] Trimmed and ordered history for user {@userIdentity}: {@history}", userIdentity, history);

                return history;
            }
            catch (BlipHttpClientException bex)
            {
                logger.Error(bex, "[GetHistory] Failed to get chat history for user {@userIdentity}", userIdentity);
                throw;
            }
            catch (Exception ex)
            {
                logger.Error(ex, "[GetHistory] Failed to get chat history for user {@userIdentity}", userIdentity);
                throw;
            }
        }

        private IEnumerable<ThreadMessage> TrimAndSortMessages(IEnumerable<ThreadMessage> messages, Enumerations.ChatOrder order, DateTime startDateTime = default(DateTime))
        {
            if (startDateTime != default(DateTime))
            {
                var dateFilteredMessages = messages.TakeWhile(m => m.Date >= startDateTime);
                messages = dateFilteredMessages;
            }

            switch (order)
            {
                case Enumerations.ChatOrder.Desc:
                    return messages;
                case Enumerations.ChatOrder.Asc:
                default:
                    return messages.OrderBy(m => m.Date);
            }
        }
    }
}
