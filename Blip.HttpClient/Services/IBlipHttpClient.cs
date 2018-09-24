using Lime.Messaging.Resources;
using Lime.Protocol;
using RestEase;
using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Take.Blip.Client;
using Takenet.Iris.Messaging.Resources;

namespace Blip.HttpClient.Services
{
    public interface IBlipHttpClient
    {
        [Header("Authorization")]
        AuthenticationHeaderValue Authorization { get; set; }

        [Post("commands")]
        Task<Command> ProcessCommandAsync([Body] Command requestCommand, CancellationToken cancellationToken);

        [Post("commands")]
        Task SendCommandAsync([Body] Command command, CancellationToken cancellationToken);

        [Post("messages")]
        Task SendMessageAsync([Body] Message message, CancellationToken cancellationToken);

        [Post("notifications")]
        Task SendNotificationAsync([Body] Notification notification, CancellationToken cancellationToken);
    }
}
