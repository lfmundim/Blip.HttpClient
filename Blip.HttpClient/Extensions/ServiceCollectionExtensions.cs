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
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection RegisterBlipExtensions(this IServiceCollection services)
        {
            services.AddSingleton<IBucketExtension, BucketExtension>();
            services.AddSingleton<IDirectoryExtension, DirectoryExtension>();
            services.AddSingleton<IContactExtension, ContactExtension>();
            services.AddSingleton<IResourceExtension, ResourceExtension>();
            services.AddSingleton<IArtificialIntelligenceExtension, ArtificialIntelligenceExtension>();
            services.AddSingleton<IEventTrackExtension, EventTrackExtension>();
            services.AddSingleton<IBroadcastExtension, BroadcastExtension>();
            services.AddSingleton<IDelegationExtension, DelegationExtension>();
            services.AddSingleton<IDirectoryExtension, DirectoryExtension>();
            services.AddSingleton<IBucketExtension, BucketExtension>();
            services.AddSingleton<ISchedulerExtension, SchedulerExtension>();
            services.AddSingleton<IArtificialIntelligenceExtension, ArtificialIntelligenceExtension>();
            services.AddSingleton<IProfileExtension, ProfileExtension>();
            services.AddSingleton<IEventTrackExtension, EventTrackExtension>();
            services.AddSingleton<IHelpDeskExtension, HelpDeskExtension>();
            services.AddSingleton<IThreadExtension, ThreadExtension>();
            services.AddSingleton<IResourceExtension, ResourceExtension>();
            services.AddSingleton<ITunnelExtension, TunnelExtension>();

            return services;
        }
    }
}
