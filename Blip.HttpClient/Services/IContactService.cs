using Lime.Messaging.Resources;
using Lime.Protocol;
using Serilog;
using System;
using System.Collections.Generic;
using System.Text;
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
        /// Gets the botIdentifier's contact using the identifier param
        /// </summary>
        /// <param name="identifier">User identifier whose contact must be recovered</param>
        /// <param name="cancellationToken"></param>
        /// <param name="logger"></param>
        /// <param name="botIdentifier">Bot identifier that has the user in its agenda</param>
        /// <returns></returns>
        Task<Contact> GetContactAsync(string identifier, CancellationToken cancellationToken, ILogger logger = null);

        /// <summary>
        /// Sets the contact on the botIdentifier's agenda
        /// </summary>
        /// <param name="contact"></param>
        /// <param name="cancellationToken"></param>
        /// <param name="logger"></param>
        /// <param name="botIdentifier">Bot identifier that has the user in its agenda</param>
        /// <returns></returns>
        Task<Command> SetContactAsync(Contact contact, CancellationToken cancellationToken, ILogger logger = null);
    }
}
