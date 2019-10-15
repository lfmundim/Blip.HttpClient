using System.Collections.Generic;
using Blip.HttpClient.Factories;
using Blip.HttpClient.Models;
using Lime.Protocol;
using Microsoft.Extensions.DependencyInjection;
using Take.Blip.Client.Extensions.ArtificialIntelligence;
using Take.Blip.Client.Extensions.Broadcast;
using Take.Blip.Client.Extensions.Bucket;
using Take.Blip.Client.Extensions.Contacts;
using Take.Blip.Client.Extensions.Delegation;
using Take.Blip.Client.Extensions.Directory;
using Take.Blip.Client.Extensions.EventTracker;
using Take.Blip.Client.Extensions.HelpDesk;
using Take.Blip.Client.Extensions.Profile;
using Take.Blip.Client.Extensions.Resource;
using Take.Blip.Client.Extensions.Scheduler;
using Take.Blip.Client.Extensions.Threads;
using Take.Blip.Client.Extensions.Tunnel;

namespace Blip.HttpClient.Extensions
{
    /// <summary>
    /// Extensions for the IServiceCollection dependency injection interface
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Registers BLiP's extensions on the services collection
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection RegisterBlipExtensions(this IServiceCollection services)
        {
            services.AddSingleton<IBucketExtension, BucketExtension>()
                    .AddSingleton<IDirectoryExtension, DirectoryExtension>()
                    .AddSingleton<IContactExtension, ContactExtension>()
                    .AddSingleton<IResourceExtension, ResourceExtension>()
                    .AddSingleton<IArtificialIntelligenceExtension, ArtificialIntelligenceExtension>()
                    .AddSingleton<IEventTrackExtension, EventTrackExtension>()
                    .AddSingleton<IBroadcastExtension, BroadcastExtension>()
                    .AddSingleton<IDelegationExtension, DelegationExtension>()
                    .AddSingleton<IDirectoryExtension, DirectoryExtension>()
                    .AddSingleton<IBucketExtension, BucketExtension>()
                    .AddSingleton<ISchedulerExtension, SchedulerExtension>()
                    .AddSingleton<IArtificialIntelligenceExtension, ArtificialIntelligenceExtension>()
                    .AddSingleton<IProfileExtension, ProfileExtension>()
                    .AddSingleton<IEventTrackExtension, EventTrackExtension>()
                    .AddSingleton<IHelpDeskExtension, HelpDeskExtension>()
                    .AddSingleton<IThreadExtension, ThreadExtension>()
                    .AddSingleton<IResourceExtension, ResourceExtension>()
                    .AddSingleton<ITunnelExtension, TunnelExtension>();

            return services;
        }

        /// <summary>
        /// Updates the ServiceCollection with any custom documents and BLiP's extensions
        /// </summary>
        /// <param name="services"></param>
        /// <param name="authKey"></param>
        /// <param name="documentList"></param>
        /// <param name="protocol">Defaults to Http to avoid breaking changes</param>
        /// <param name="domain">Domain of request in blip. Default is 'msging.net'</param>
        public static IServiceCollection DefaultRegister(this IServiceCollection services, string authKey,
                                                         List<Document> documentList = null, BlipProtocol protocol = BlipProtocol.Http, string domain = null)
        {
            return new BlipClientFactory(domain).BuildServiceCollection(authKey, protocol, documentList, services);
        }
    }
}
