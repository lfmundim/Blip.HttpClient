using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Blip.HttpClient.Exceptions;
using Lime.Protocol;
using Serilog;
using Takenet.Iris.Messaging.Resources;

namespace Blip.HttpClient.Services.Threads
{
    /// <summary>
    /// Service responsible for getting chat history
    /// </summary>
    public interface IThreadService
    {
        /// <summary>
        /// Gets the last chatbot's threads. By default, BLiP returns the last 50 threads.
        /// </summary>
        //Task<Command> GetLastThreadsAsync(); // To be studied and implemented

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
        Task<IEnumerable<ThreadMessage>> GetHistoryAsync(Identity userIdentity, Enumerations.ChatOrder order, ILogger logger, CancellationToken cancellationToken = default(CancellationToken), DateTime startDateTime = default(DateTime));
    }
}
