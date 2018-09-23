using System;
using System.Threading;
using System.Threading.Tasks;
using Blip.HttpClient.Decorators;
using Blip.HttpClient.Services;
using Lime.Protocol;
using Shouldly;
using Take.Blip.Client;
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
        public async Task ProcessCommandUnitTest(string commandSuffix)
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
    }
}
