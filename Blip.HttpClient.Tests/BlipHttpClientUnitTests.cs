using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Blip.HttpClient.Decorators;
using Blip.HttpClient.Services;
using Lime.Messaging.Contents;
using Lime.Protocol;
using Shouldly;
using Take.Blip.Client;
using Takenet.Iris.Messaging.Contents;
using Takenet.Iris.Messaging.Resources;
using Xunit;

namespace Blip.HttpClient.Tests
{
    public class BlipHttpClientUnitTests
    {
        public ISender _client { get; set; }
        public BlipHttpClientUnitTests()
        {
            var factory = new BlipHttpClientFactory();
            _client = factory.BuildBlipHttpClient("dGVzdGluZ2JvdHM6OU8zZEpWbHVaSWZNYmVnOWZaZzM=");
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
        [InlineData("")]
        public async Task NotificationsUnitTest(string commandSuffix)
        {
            var notification = new Notification
            {
                From = "551100001111@0mn.io/4ac58r6e3",
                Event = Event.Consumed
            };

            var task = _client.SendNotificationAsync(notification, CancellationToken.None);
            task.Wait();
            task.IsCompleted.ShouldBeTrue();
        }

        [Theory]
        [InlineData("")]
        public async Task MessagesUnitTest(string commandSuffix)
        {
            var message = new Message
            {
                Content = PlainText.Parse("Unit Tests rock!")
            };

            var task = _client.SendMessageAsync(message, CancellationToken.None);
            task.Wait();
            task.IsCompleted.ShouldBeTrue();
        }
    }
}
