using Lime.Protocol;
using System.Threading;
using System.Threading.Tasks;
using Take.Blip.Client;

namespace Blip.HttpClient.Services
{
    public class BlipHttpClient : ISender
    {
        private readonly IBlipHttpClient _blipHttpClient;

        public BlipHttpClient(IBlipHttpClient blipHttpClient)
        {
            _blipHttpClient = blipHttpClient;
        }

        public Task<Command> ProcessCommandAsync(Command requestCommand, CancellationToken cancellationToken)
        {
            return _blipHttpClient.ProcessCommandAsync(requestCommand, cancellationToken);
        }

        public Task SendCommandAsync(Command command, CancellationToken cancellationToken)
        {
            return _blipHttpClient.SendCommandAsync(command, cancellationToken);
        }

        public Task SendMessageAsync(Message message, CancellationToken cancellationToken)
        {
            return _blipHttpClient.SendMessageAsync(message, cancellationToken);
        }

        public Task SendNotificationAsync(Notification notification, CancellationToken cancellationToken)
        {
            return _blipHttpClient.SendNotificationAsync(notification, cancellationToken);
        }
    }
}
