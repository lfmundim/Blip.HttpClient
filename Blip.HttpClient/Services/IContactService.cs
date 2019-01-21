using Blip.HttpClient.Exceptions;
using Lime.Messaging.Resources;
using Lime.Protocol;
using Serilog;
using System;
using System.Threading;
using System.Threading.Tasks;
using Take.Blip.Client.Extensions.Contacts;

namespace Blip.HttpClient.Services
{
    /// <summary>
    /// Http Abstraction of BLiP's /contact URI
    /// </summary>
    public interface IContactService : IContactExtension
    {
        /// <summary>
        /// Gets the Bot's Contact using the <paramref name="identity"/> param
        /// </summary>
        /// <param name="identity">User identifier whose contact must be recovered</param>
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
        Task<Contact> GetAsync(Identity identity, CancellationToken cancellationToken, ILogger logger);

        /// <summary>
        /// Sets the <paramref name="contact"/> on the Bot's agenda
        /// </summary>
        /// <param name="contact"></param>
        /// <param name="cancellationToken"></param>
        /// <param name="logger"><c>ILogger</c> from <c>Serilog</c> to log useful information</param>
        /// <returns>Lime <c>Command</c> response object</returns>
        /// <example>
        /// <code>
        /// Command x = await _contactService.SetContactAsync(contact, cancellationToken, logger)
        /// </code>
        /// </example>
        /// <exception cref="BlipHttpClientException">Failure setting the contact</exception>
        /// <exception cref="Exception">Unknown error</exception>
        Task<Command> SetAsync(Contact contact, CancellationToken cancellationToken, ILogger logger);

        /// <summary>
        /// Merges the <paramref name="contact"/> with the existing one on the Bot's agenda
        /// </summary>
        /// <param name="identity"></param>
        /// <param name="contact"></param>
        /// <param name="cancellationToken"></param>
        /// <param name="logger"><c>ILogger</c> from <c>Serilog</c> to log useful information</param>
        /// <returns></returns>
        /// <exception cref="BlipHttpClientException">Failure setting the contact</exception>
        /// <exception cref="Exception">Unknown error</exception>
        Task MergeAsync(Identity identity, Contact contact, CancellationToken cancellationToken, ILogger logger);

        /// <summary>
        /// Deletes the <paramref name="identity"/> contact from the Bot's BLiP Agenda
        /// </summary>
        /// <param name="identity"></param>
        /// <param name="cancellationToken"></param>
        /// <param name="logger"><c>ILogger</c> from <c>Serilog</c> to log useful information</param>
        /// <returns></returns>
        /// <exception cref="BlipHttpClientException">Failure setting the contact</exception>
        /// <exception cref="Exception">Unknown error</exception>
        Task DeleteAsync(Identity identity, CancellationToken cancellationToken, ILogger logger);
    }
}
