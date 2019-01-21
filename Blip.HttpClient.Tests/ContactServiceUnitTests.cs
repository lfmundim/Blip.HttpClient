using Blip.HttpClient.Factories;
using Blip.HttpClient.Services;
using Lime.Messaging.Resources;
using Lime.Protocol;
using Serilog;
using Shouldly;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Blip.HttpClient.Tests
{
    public class ContactServiceUnitTests
    {
        private readonly IContactService _contactService;
        private readonly ILogger _logger;
        public ContactServiceUnitTests()
        {
            var clientFactory = new BlipHttpClientFactory();
            var sender = clientFactory.BuildBlipHttpClient("dGVzdGluZ2JvdHM6OU8zZEpWbHVaSWZNYmVnOWZaZzM=");
            _contactService = new ContactService(sender);
            _logger = new LoggerConfiguration()
                     .Enrich.WithProperty("Application", "UnitTests")
                     .MinimumLevel.Debug()
                     .CreateLogger();
        }

        [Theory]
        [InlineData("")]
        [InlineData("dGVzdGluZ2JvdHM6OU8zZEpWbHVaSWZNYmVnOWZaZzM=")]
        public async Task SetAndGetContactUnitTest(string authKey)
        {
            var id = EnvelopeId.NewId();
            var identity = Identity.Parse($"{id}.testingbots@0mn.io");

            Command setResponse;
            Contact getResponse;

            var contact = new Contact
            {
                Name = id,
                Identity = identity
            };

            if (authKey.Equals(""))
            {
                setResponse = await _contactService.SetAsync(contact, CancellationToken.None, _logger);
                getResponse = await _contactService.GetAsync(identity, CancellationToken.None, _logger);
            }
            else
            {
                var contactService = new ContactService(authKey);
                setResponse = await contactService.SetAsync(contact, CancellationToken.None, _logger);
                getResponse = await contactService.GetAsync(identity, CancellationToken.None, _logger);
            }

            setResponse.Status.ShouldBe(CommandStatus.Success);

            getResponse.ShouldNotBeNull();
            getResponse.Name.ShouldBe(id);
            getResponse.Identity.ShouldBe(identity);
            getResponse.GetMediaType().ToString().ShouldBe("application/vnd.lime.contact+json");
        }
    }
}
