using Blip.HttpClient.Factories;
using Blip.HttpClient.Services;
using Lime.Messaging.Resources;
using Lime.Protocol;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Take.Blip.Client;
using Xunit;

namespace Blip.HttpClient.Tests
{
    public class ContactServiceUnitTests
    {
        private readonly ISender _sender;
        private readonly IContactService _contactService;
        public ContactServiceUnitTests()
        {
            var clientFactory = new BlipHttpClientFactory();
            _sender = clientFactory.BuildBlipHttpClient("dGVzdGluZ2JvdHM6OU8zZEpWbHVaSWZNYmVnOWZaZzM=");
            _contactService = new ContactService(_sender);
        }

        [Theory]
        [InlineData("")] //Check on how to remove this using Xtest
        public async Task SetAndGetContactUnitTest(string empty)
        {
            var id = EnvelopeId.NewId();
            var identity = Identity.Parse($"{id}.testingbots@0mn.io");

            var contact = new Contact
{
                Name = id,
                Identity = identity
};

            var setResponse = await _contactService.SetContactAsync(contact, CancellationToken.None);

            var getResponse = await _contactService.GetContactAsync(identity, CancellationToken.None);

            setResponse.Status.ShouldBe(CommandStatus.Success);

            getResponse.ShouldNotBeNull();
            getResponse.Name.ShouldBe(id);
            getResponse.Identity.ShouldBe(identity);
            getResponse.GetMediaType().ToString().ShouldBe("application/vnd.lime.contact+json");
        }
    }
}
