using Lime.Protocol;
using System.Threading;
using System.Threading.Tasks;
using Take.Blip.Client;

namespace Blip.HttpClient.Services
{
    /// <summary>
    /// BLiP ISender implementation that uses Http calls instead of Tcp
    /// </summary>
    public class BlipHttpClient : ISender
    {
        private readonly IBlipHttpClient _blipHttpClient;

        /// <summary>
        /// Base ctor using a given http client
        /// </summary>
        /// <param name="blipHttpClient"></param>
        public BlipHttpClient(IBlipHttpClient blipHttpClient)
        {
            _blipHttpClient = blipHttpClient;
        }

        /// <summary>
        /// Sends a command to BLiP and returns its response.
        /// </summary>
        /// <param name="requestCommand"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<Command> ProcessCommandAsync(Command requestCommand, CancellationToken cancellationToken)
        {
            return _blipHttpClient.ProcessCommandAsync(requestCommand, cancellationToken);
        }

        /// <summary>
        /// Sends a command to BLiP
        /// </summary>
        /// <param name="command"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task SendCommandAsync(Command command, CancellationToken cancellationToken)
        {
            return _blipHttpClient.SendCommandAsync(command, cancellationToken);
        }

        /// <summary>
        /// Sends a message through BLiP to the set recipient. Message's content is a LIME Document Type.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task SendMessageAsync(Message message, CancellationToken cancellationToken)
        {
            return _blipHttpClient.SendMessageAsync(message, cancellationToken);
        }

        /// <summary>
        /// Sends a notification to BLiP
        /// </summary>
        /// <param name="notification"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task SendNotificationAsync(Notification notification, CancellationToken cancellationToken)
        {
            return _blipHttpClient.SendNotificationAsync(notification, cancellationToken);
        }
    }
}
