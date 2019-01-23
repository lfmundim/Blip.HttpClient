using Lime.Protocol;
using Serilog;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Take.Blip.Client.Extensions.Broadcast;

namespace Blip.HttpClient.Services
{
    public interface IBroadcastService : IBroadcastExtension
    {
        Task AddRecipientAsync(string listName, Identity recipientIdentity, CancellationToken cancellationToken, ILogger logger);

        Task CreateDistributionListAsync(string listName, CancellationToken cancellationToken, ILogger logger);

        Task DeleteDistributionListAsync(string listName, CancellationToken cancellationToken, ILogger logger);

        Task DeleteRecipientAsync(string listName, Identity recipientIdentity, CancellationToken cancellationToken, ILogger logger);

        Identity GetListIdentity(string listName, ILogger logger);

        Task<DocumentCollection> GetRecipientsAsync(ILogger logger, CancellationToken cancellationToken, string listName, int skip = 0, int take = 100);

        Task<DocumentCollection> GetRecipientsAsynGetAllDistributionListsAsync(ILogger logger, CancellationToken cancellationToken, int skip = 0, int take = 100);

        Task<bool> HasRecipientAsync(string listName, Identity recipientIdentity, CancellationToken cancellationToken, ILogger logger);

        Task SendMessageAsync(ILogger logger, CancellationToken cancellationToken, string listName, Document content, string id = null);
    }
}
