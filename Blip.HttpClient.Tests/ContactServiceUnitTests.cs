using Blip.HttpClient.Factories;
using Blip.HttpClient.Services;
using Lime.Messaging.Resources;
using Lime.Protocol;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.TestCorrelator;
using Shouldly;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using System.Linq;
using NSubstitute;
using System.Collections.Generic;
using Xunit;
using Take.Blip.Client;
using Blip.HttpClient.Exceptions;

namespace Blip.HttpClient.Tests
{
    public class ContactServiceUnitTests
    {
        private readonly IContactService _contactService;
        private readonly ILogger _logger;
        public ContactServiceUnitTests()
        {
            //tests de logs e de client separados
            var clientFactory = new BlipHttpClientFactory();
            var sender = clientFactory.BuildBlipHttpClient("dGVzdGluZ2JvdHM6OU8zZEpWbHVaSWZNYmVnOWZaZzM="); //Substitute.For<ISender>();
            _contactService = new ContactService(sender);
            _logger = Substitute.For<ILogger>();
        }

        [Theory]
        [InlineData("")]
        [InlineData("dGVzdGluZ2JvdHM6OU8zZEpWbHVaSWZNYmVnOWZaZzM=")] //set wrong auth key to force errors
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

        [Theory]
        [InlineData(true, false)]
        [InlineData(false, true)]

        public async Task SetAndGetContactUnitTest_ShouldThrowExceptions(bool shouldThrowBlipEx, bool shouldThrowGenericEx)
        {
            var client = Substitute.For<ISender>();
            client.WhenForAnyArgs(client.ProcessCommandAsync()).WhenCalled()
        }

        private async Task<T> TestLogsWithOneArg<T, T1>(Task<T> testTask, int expectedLogCount, LogEventLevel expectedLogLevel, T1 firstArg)
        {
            var taskResponse = await testTask;
            _logger.ReceivedWithAnyArgs().Information(Arg.Any<string>(), Arg.Any<T1>());
            return taskResponse;
        }
        private async Task<T> TestLogsWithTwoArgs<T, T1, T2>(Task<T> testTask, int expectedLogCount, LogEventLevel expectedLogLevel, T1 firstArg, T2 secondArg)
        {
            var taskResponse = await testTask;
            _logger.ReceivedWithAnyArgs().Information(Arg.Any<string>(), Arg.Any<T1>(), Arg.Any<T2>());
            return taskResponse;
        }
    }
}
