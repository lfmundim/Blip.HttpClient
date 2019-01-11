using Blip.HttpClient.Exceptions;
using Lime.Messaging.Resources;
using Lime.Protocol;
using Serilog;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Blip.HttpClient.Services
{
    /// <summary>
    /// Http Abstraction of BLiP's /contact URI
    /// </summary>
    public interface IContactService
    {
        /// <summary>
        /// Gets the Bot's Contact using the <paramref name="identifier"/> param
        /// </summary>
        /// <param name="identifier">User identifier whose contact must be recovered</param>
        /// <param name="cancellationToken"></param>
        /// <param name="logger">Optional <c>ILogger</c> from <c>Serilog</c> to log useful information</param>
        /// <returns>Lime <c>Contact</c> response object</returns>
        /// <example>
        /// <code>
        /// Contact x = await _contactService.GetContactAsync(contact, cancellationToken, logger)
        /// </code>
        /// </example>
        /// <exception cref="BlipHttpClientException">Failure getting the contact</exception>
        /// <exception cref="Exception">Unknown error</exception>
        Task<Contact> GetContactAsync(string identifier, CancellationToken cancellationToken, ILogger logger = null);

        /// <summary>
        /// Sets the <paramref name="contact"/> on the Bot's agenda
        /// </summary>
        /// <param name="contact"></param>
        /// <param name="cancellationToken"></param>
        /// <param name="logger">Optional <c>ILogger</c> from <c>Serilog</c> to log useful information</param>
        /// <returns>Lime <c>Command</c> response object</returns>
        /// <example>
        /// <code>
        /// Command x = await _contactService.SetContactAsync(contact, cancellationToken, logger)
        /// </code>
        /// </example>
        /// <exception cref="BlipHttpClientException">Failure setting the contact</exception>
        /// <exception cref="Exception">Unknown error</exception>
        Task<Command> SetContactAsync(Contact contact, CancellationToken cancellationToken, ILogger logger = null);
    }
}
