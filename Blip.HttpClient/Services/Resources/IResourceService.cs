using Lime.Protocol;
using Serilog;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Take.Blip.Client.Extensions.Resource;

namespace Blip.HttpClient.Services.Resources
{
    interface IResourceService : IResourceExtension
    {
        Task DeleteAsync(string id, ILogger logger, CancellationToken cancellationToken = default(CancellationToken));

        Task<T> GetAsync<T>(string id, ILogger logger, CancellationToken cancellationToken = default(CancellationToken)) where T : Document;

        Task<DocumentCollection> GetIdsAsync(ILogger logger, int skip = 0, int take = 100, CancellationToken cancellationToken = default(CancellationToken));

        Task SetAsync<T>(string id, T document, ILogger logger, TimeSpan expiration = default(TimeSpan), CancellationToken cancellationToken = default(CancellationToken)) where T : Document;
    }
}
