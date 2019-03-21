using Blip.HttpClient.Exceptions;
using Lime.Protocol;
using Serilog;
using System;
using System.Threading;
using System.Threading.Tasks;
using Take.Blip.Client.Extensions.Broadcast;

namespace Blip.HttpClient.Services.Broadcast
{
    /// <summary>
    /// Service responsible for managing broadcast lists and sending messages to them
    /// </summary>
    public interface IBroadcastService : IBroadcastExtension
    {
        /// <summary>
        /// Adds a recipient to a broadcast list based on the recipient's identity and the list's name
        /// </summary>
        /// <param name="listName"></param>
        /// <param name="recipientIdentity"></param>
        /// <param name="cancellationToken"></param>
        /// <param name="logger"></param>
        /// <returns>Lime <c>Command</c> response object</returns>
        /// <exception cref="BlipHttpClientException">Failure Adding identity to the broadcast list</exception>
        /// <exception cref="Exception">Unknown error</exception>
        Task<Command> AddRecipientAsync(string listName, Identity recipientIdentity, CancellationToken cancellationToken, ILogger logger);

        /// <summary>
        /// Creates a distribution list using the given name. The <param nameref="listName"/> should ideally have the bot's <c>Identifier</c> as a prefix
        /// </summary>
        /// <param name="listName"></param>
        /// <param name="cancellationToken"></param>
        /// <param name="logger"></param>
        /// <returns>Lime <c>Command</c> response object</returns>
        /// <exception cref="BlipHttpClientException">Failure creating broadcast list</exception>
        /// <exception cref="Exception">Unknown error</exception>
        Task<Command> CreateDistributionListAsync(string listName, CancellationToken cancellationToken, ILogger logger);

        /// <summary>
        /// Delete a distribution list using the given <paramref name="listName"/>.
        /// </summary>
        /// <param name="listName"></param>
        /// <param name="cancellationToken"></param>
        /// <param name="logger"></param>
        /// <returns>Lime <c>Command</c> response object</returns>
        /// <exception cref="BlipHttpClientException">Failure deleting broadcast list</exception>
        /// <exception cref="Exception">Unknown error</exception>
        Task<Command> DeleteDistributionListAsync(string listName, CancellationToken cancellationToken, ILogger logger);

        /// <summary>
        /// Removes given <paramref name="recipientIdentity"/> from the broadcast list with the name <paramref name="listName"/>
        /// </summary>
        /// <param name="listName"></param>
        /// <param name="recipientIdentity"></param>
        /// <param name="cancellationToken"></param>
        /// <param name="logger"></param>
        /// <returns>Lime <c>Command</c> response object</returns>
        /// <exception cref="BlipHttpClientException">Failure removing user from distribution list</exception>
        /// <exception cref="Exception">Unknown error</exception>
        Task<Command> DeleteRecipientAsync(string listName, Identity recipientIdentity, CancellationToken cancellationToken, ILogger logger);

        /// <summary>
        /// Returns a list of recipients for the list with the name <paramref name="listName"/>
        /// </summary>
        /// <param name="listName"></param>
        /// <param name="skip">Recipients to skip from the start</param>
        /// <param name="take">Amount of recipients to get</param>
        /// <param name="logger"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>DocumentCollection with the items being the recovered recipients</returns>
        /// <exception cref="BlipHttpClientException">Failure getting recipients for the given distribution list</exception>
        /// <exception cref="Exception">Unknown error</exception>
        Task<DocumentCollection> GetRecipientsAsync(ILogger logger, string listName, int skip = 0, int take = 100,
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Sends a message to the given broadcast list
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="cancellationToken"></param>
        /// <param name="listName"></param>
        /// <param name="content">Message to be sent</param>
        /// <param name="id"></param>
        /// <returns>Lime <c>Command</c> response object</returns>
        /// <exception cref="BlipHttpClientException">Failure sending message to distribution list</exception>
        /// <exception cref="Exception">Unknown error</exception>
        Task SendMessageAsync(string listName, Document content, ILogger logger, string id = null,
            CancellationToken cancellationToken = default(CancellationToken));
    }
}
