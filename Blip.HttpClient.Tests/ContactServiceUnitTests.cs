using Blip.HttpClient.Factories;
using Blip.HttpClient.Services;
using Lime.Messaging.Resources;
using Lime.Protocol;
using Shouldly;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Blip.HttpClient.Tests
{
    public class ContactServiceUnitTests
    {
        private readonly IContactService _contactService;
        public ContactServiceUnitTests()
        {
            var clientFactory = new BlipHttpClientFactory();
            var sender = clientFactory.BuildBlipHttpClient("dGVzdGluZ2JvdHM6OU8zZEpWbHVaSWZNYmVnOWZaZzM=");
            _contactService = new ContactService(sender);
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
                setResponse = await _contactService.SetContactAsync(contact, CancellationToken.None);
                getResponse = await _contactService.GetContactAsync(identity, CancellationToken.None);
            }
            else
            {
                var contactService = new ContactService(authKey);
                setResponse = await contactService.SetContactAsync(contact, CancellationToken.None);
                getResponse = await contactService.GetContactAsync(identity, CancellationToken.None);
            }

            setResponse.Status.ShouldBe(CommandStatus.Success);

            getResponse.ShouldNotBeNull();
            getResponse.Name.ShouldBe(id);
            getResponse.Identity.ShouldBe(identity);
            getResponse.GetMediaType().ToString().ShouldBe("application/vnd.lime.contact+json");
        }
    }
}
