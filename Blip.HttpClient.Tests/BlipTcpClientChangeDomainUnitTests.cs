using Blip.HttpClient.Extensions;
using Lime.Messaging.Contents;
using Lime.Protocol;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Take.Blip.Client;
using Takenet.Iris.Messaging.Resources;
using Xunit;

namespace Blip.HttpClient.Tests
{
    public class BlipTcpClientChangeDomainUnitTests
    {
        private readonly ISender _client;
        public BlipTcpClientChangeDomainUnitTests()
        {
            var documentList = new List<Document>();
            documentList.Add(new UserContext());
            var services = new ServiceCollection();
            //pagseguro hmg
            services.DefaultRegister("Key cGFnc2VndXJvYXRlbmRpbWVudG9obWc6TzVhSURNUURBSVhuT05rTVdORE4=", documentList, Models.BlipProtocol.Tcp, "pagseguro.msging.net");
            var provider = services.BuildServiceProvider();
            //_client = provider.GetService<ISender>();
        }

        [Theory]
        [InlineData("/contacts")]
        public async Task ContactsUnitTest(string commandSuffix)
        {
            var command = new Command
            {
                Method = CommandMethod.Get,
                Uri = new LimeUri(commandSuffix)
            };
            var resp = await _client.ProcessCommandAsync(command, CancellationToken.None);

            resp.Status.ShouldBe(CommandStatus.Success);
            resp.Resource.ShouldNotBeNull();
            resp.Type.ToString().ShouldBe("application/vnd.lime.collection+json");
            resp.To.Domain.ShouldBe("msging.net");
            resp.From.Name.ShouldBe("postmaster");
            resp.From.Domain.ShouldBe("crm.msging.net");
        }

        [Theory]
        [InlineData("/event-track")]
        public async Task EventTrackUnitTest(string commandSuffix)
        {
            var setTrackCommand = new Command
            {
                Method = CommandMethod.Set,
                Uri = new LimeUri(commandSuffix),
                Resource = new EventTrack()
                {
                    Action = "Attempt",
                    Category = "Unit Test"
                }
            };

            var resp = await _client.ProcessCommandAsync(setTrackCommand, CancellationToken.None);

            resp.Status.ShouldBe(CommandStatus.Success);
            resp.To.Domain.ShouldBe("msging.net");
            resp.From.Name.ShouldBe("postmaster");
            resp.From.Domain.ShouldBe("analytics.msging.net");
        }

        [Theory]
        [InlineData("/buckets")]
        public async Task BucketsUnitTest(string commandSuffix)
        {
            var jsonDocument = new JsonDocument();
            jsonDocument.Add("UnitTest", "success");

            var setBucketCommand = new Command
            {
                Method = CommandMethod.Set,
                Uri = new LimeUri($"{commandSuffix}/UnitTests"),
                Resource = jsonDocument
            };

            var getBucketCommand = new Command
            {
                Method = CommandMethod.Get,
                Uri = new LimeUri($"{commandSuffix}/UnitTests")
            };

            var deleteBucketCommand = new Command
            {
                Method = CommandMethod.Delete,
                Uri = new LimeUri($"{commandSuffix}/UnitTests")
            };

            await _client.SendCommandAsync(setBucketCommand, CancellationToken.None);
            var resp = await _client.ProcessCommandAsync(getBucketCommand, CancellationToken.None);
            await _client.SendCommandAsync(deleteBucketCommand, CancellationToken.None);

            resp.Status.ShouldBe(CommandStatus.Success);
            resp.To.Domain.ShouldBe("msging.net");
            resp.From.Name.ShouldBe("postmaster");
            resp.From.Domain.ShouldBe("msging.net");
            (resp.Resource as JsonDocument).ShouldNotBeSameAs(jsonDocument);
        }

        [Theory]
        [InlineData("551100001111@0mn.io/4ac58r6e3")]
        public void NotificationsUnitTest(string from)
        {
            var notification = new Notification
            {
                From = from,
                Event = Event.Consumed
            };

            var task = _client.SendNotificationAsync(notification, CancellationToken.None);
            task.Wait();
            task.IsCompleted.ShouldBeTrue();
        }

        [Theory]
        [InlineData("Unit tests rock!")]
        public void MessagesUnitTest(string messageText)
        {
            var message = new Message
            {
                Content = PlainText.Parse(messageText)
            };

            var task = _client.SendMessageAsync(message, CancellationToken.None);
            task.Wait();
            task.IsCompleted.ShouldBeTrue();
        }
    }
}
