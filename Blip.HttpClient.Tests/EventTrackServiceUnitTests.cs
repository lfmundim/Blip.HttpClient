using Blip.HttpClient.Exceptions;
using Blip.HttpClient.Factories;
using Blip.HttpClient.Services.EventTracker;
using Lime.Messaging.Resources;
using Lime.Protocol;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Serilog;
using Shouldly;
using System;
using System.Threading;
using System.Threading.Tasks;
using Take.Blip.Client;
using Xunit;

namespace Blip.HttpClient.Tests
{
    public class EventTrackServiceUnitTests
    {
        private const string RecipientIdentity = "unittests.testingbots@0mn.io";
        private const string Category = "categoryTest";
        private const string Action = "actionTest";
        private readonly Contact contact;
        private readonly IEventTrackService _eventTrackService;
        private readonly ILogger _logger;

        public EventTrackServiceUnitTests()
        {
            contact = new Contact();
            var clientFactory = new BlipHttpClientFactory();
            var sender = clientFactory.BuildBlipHttpClient("dGVzdGluZ2JvdHM6OU8zZEpWbHVaSWZNYmVnOWZaZzM=");
            _eventTrackService = new EventTrackService(sender);
            _logger = Substitute.For<ILogger>();
        }

        #region Methods unit tests

        [Theory]
        [InlineData("", false)]
        [InlineData("", true)]
        [InlineData("dGVzdGluZ2JvdHM6OU8zZEpWbHVaSWZNYmVnOWZaZzM=", false)]
        public async Task Add_UnitTest(string authKey, bool fireAndForget)
        {
            Command addResponse;
            Identity identity = Identity.Parse(RecipientIdentity);

            if (authKey == "")
            {
                addResponse = await _eventTrackService.AddAsync(Category, Action, _logger, identity: identity);
            }
            else if (fireAndForget)
            {
                addResponse = await _eventTrackService.AddAsync(Category, Action, _logger, contact: contact, fireAndForget: fireAndForget);
            }
            else
            {
                var eventTrackService = new EventTrackService(authKey);
                addResponse = await eventTrackService.AddAsync(Category, Action, _logger, contact: contact);
            }

            addResponse.Status.ShouldBe(CommandStatus.Success);
        }

        #endregion

        #region Log unit tests

        #region Add_Log

        [Fact]
        public async Task Add_LogUnitTest_ShouldSucceed()
        {
            Identity identity = Identity.Parse(RecipientIdentity);
            var client = BuildSenderSubstitute_ReturnsSuccessStatus();
            var eventTrackService = new EventTrackService(client);
            var logger = Substitute.For<ILogger>();
            var task = await eventTrackService.AddAsync(Category, Action, logger, identity: identity);

            logger.Received(2).Information(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>());
        }

        [Fact]
        public async Task Add_LogUnitTest_ShouldThrowBlipHttpClientException()
        {
            Identity identity = Identity.Parse(RecipientIdentity);
            var client = BuildSenderSubstitute_ReturnsFailureStatus();
            var eventTrackService = new EventTrackService(client);
            var logger = Substitute.For<ILogger>();
            var exceptionThrown = false;

            try
            {
                await eventTrackService.AddAsync(Category, Action, logger, identity: identity);
            }
            catch (BlipHttpClientException bex) 
            {
                logger.Received(1).Error(bex, Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>());
                exceptionThrown = true;
            }
            finally
            {
                exceptionThrown.ShouldBe(true);
            }
        }

        [Fact]
        public async Task Add_LogUnitTest_ShouldThrowException()
        {
            Identity identity = Identity.Parse(RecipientIdentity);
            var client = BuildSenderSubstittute_ThrowsException();
            var eventTrackService = new EventTrackService(client);
            var logger = Substitute.For<ILogger>();
            var exceptionThrown = false;

            try
            {
                await eventTrackService.AddAsync(Category, Action, logger, identity: identity);
            }
            catch (Exception ex)
            {
                logger.Received(1).Error(ex, Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>());
                exceptionThrown = true;
            }
            finally
            {
                exceptionThrown.ShouldBe(true);
            }
        }

        #endregion

        #endregion

        #region Private aux methods

        private static ISender BuildSenderSubstitute_ReturnsSuccessStatus()
        {
            var client = Substitute.For<ISender>();

            var responseCommand = new Command()
            {
                Status = CommandStatus.Success,
                Reason = new Reason()
                {
                    Description = "Unit Tests",
                    Code = 42
                }
            };

            client.ProcessCommandAsync(Arg.Any<Command>(), Arg.Any<CancellationToken>()).Returns(responseCommand);
            return client;
        }

        private static ISender BuildSenderSubstitute_ReturnsFailureStatus()
        {
            var client = Substitute.For<ISender>();

            var responseCommand = new Command()
            {
                Status = CommandStatus.Failure,
                Reason = new Reason()
                {
                    Description = "Unit Tests",
                    Code = 42
                }
            };

            client.ProcessCommandAsync(Arg.Any<Command>(), Arg.Any<CancellationToken>()).Returns(responseCommand);
            return client;
        }

        private static ISender BuildSenderSubstittute_ThrowsException()
        {
            var client = Substitute.For<ISender>();
            var responseEx = new Exception("Unit Tests");

            client.ProcessCommandAsync(Arg.Any<Command>(), Arg.Any<CancellationToken>()).Throws(responseEx);
            return client;
        }

        #endregion
    }
}
