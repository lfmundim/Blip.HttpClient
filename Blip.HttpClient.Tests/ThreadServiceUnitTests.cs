using Blip.HttpClient.Exceptions;
using Blip.HttpClient.Factories;
using Blip.HttpClient.Services.Threads;
using FluentAssertions.Common;
using Lime.Protocol;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Serilog;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Take.Blip.Client;
using Takenet.Iris.Messaging.Resources;
using Xunit;

namespace Blip.HttpClient.Tests
{
    public class ThreadServiceUnitTests
    {
        private readonly IThreadService _threadService;
        private readonly ILogger _logger;
        private const string IdentityString = "a5ca8a7c-32db-4a80-8f03-3cd36131924c.testingbots@0mn.io";

        public ThreadServiceUnitTests()
        {
            var clientFactory = new BlipHttpClientFactory();
            var sender = clientFactory.BuildBlipHttpClient("dGVzdGluZ2JvdHM6OU8zZEpWbHVaSWZNYmVnOWZaZzM=");
            _threadService = new ThreadService(sender);
            _logger = Substitute.For<ILogger>();
        }

        #region Methods unit tests

        [Theory]
        [InlineData("")]
        [InlineData("dGVzdGluZ2JvdHM6OU8zZEpWbHVaSWZNYmVnOWZaZzM=")]
        public async Task Get_History_UnitTest(string authKey)
        {
            var identity = Identity.Parse(IdentityString);
            IEnumerable<ThreadMessage> history;

            if (authKey.Equals(""))
            {
                history = await _threadService.GetHistoryAsync(identity, Enumerations.ChatOrder.Asc, _logger, CancellationToken.None, DateTime.Parse("2018-12-06T10:16:50.088Z"));
            }
            else
            {
                var threadService = new ThreadService(authKey);
                history = await threadService.GetHistoryAsync(identity, Enumerations.ChatOrder.Asc, _logger, CancellationToken.None, DateTime.Parse("2018-12-06T10:16:50.088Z"));
            }

            var historyList = history.ToList();
            historyList.Count.ShouldBeGreaterThan(0);
            historyList.OrderBy(h => h.Date).IsSameOrEqualTo(historyList);
        }

        #endregion

        #region Log unit tests

        [Fact]
        public async Task GetHistoryUnitTest_ShouldSucceed()
        {
            var identity = Identity.Parse(IdentityString);
            var client = BuildSenderSubstitute_ReturnsSuccessStatus();
            var threadService = new ThreadService(client);
            var logger = Substitute.For<ILogger>();
            var task = threadService.GetHistoryAsync(identity, Enumerations.ChatOrder.Asc, logger, CancellationToken.None, DateTime.Parse("2018-12-06T10:16:50.088Z"));

            await TestInfoLogsWithOneArg<Identity>(task, 3, logger);
            await TestInfoLogsWithTwoArgs<Identity, IEnumerable<ThreadMessage>>(task, 1, logger);
        }

        [Fact]
        public async Task GetHistoryLogUnitTest_ShouldThrowBlipHttpClientEx()
        {
            var identity = Identity.Parse(IdentityString);
            var logger = Substitute.For<ILogger>();
            var client = BuildSenderSubstitute_ReturnsFailureStatus();
            var threadService = new ThreadService(client);
            var exceptionThrown = false;
            try
            {
                await threadService.GetHistoryAsync(identity, Enumerations.ChatOrder.Asc, logger, CancellationToken.None, DateTime.Parse("2018-12-06T10:16:50.088Z"));
            }
            catch (BlipHttpClientException bex)
            {
                logger.Received(1).Error(bex, Arg.Any<string>(), Arg.Any<Identity>());
                exceptionThrown = true;
            }
            finally
            {
                exceptionThrown.ShouldBe(true);
            }
        }

        [Fact]
        public async Task GetHistoryLogUnitTest_ShouldThrowEx()
        {
            var identity = Identity.Parse(IdentityString);
            var logger = Substitute.For<ILogger>();
            var client = BuildSenderSubstitute_ThrowsException();
            var threadService = new ThreadService(client);
            var exceptionThrown = false;
            try
            {
                await threadService.GetHistoryAsync(identity, Enumerations.ChatOrder.Asc, logger, CancellationToken.None, DateTime.Parse("2018-12-06T10:16:50.088Z"));
            }
            catch (Exception ex)
            {
                logger.Received(1).Error(ex, Arg.Any<string>(), Arg.Any<Identity>());
                exceptionThrown = true;
            }
            finally
            {
                exceptionThrown.ShouldBe(true);
            }
        }

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
                },
                Resource = new DocumentCollection
                {
                    Items = new Document[]
                    {
                        new ThreadMessage(),
                        new ThreadMessage()
                    }
                }
            };

            client
                .ProcessCommandAsync(Arg.Any<Command>(), Arg.Any<CancellationToken>())
                .Returns(responseCommand);
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

            client
                .ProcessCommandAsync(Arg.Any<Command>(), Arg.Any<CancellationToken>())
                .Returns(responseCommand);
            return client;
        }

        private static ISender BuildSenderSubstitute_ThrowsException()
        {
            var client = Substitute.For<ISender>();

            var exResponse = new Exception("Unit Tests");

            client
                .ProcessCommandAsync(Arg.Any<Command>(), Arg.Any<CancellationToken>())
                .Throws(exResponse);
            return client;
        }

        private async Task TestInfoLogsWithOneArg<T>(Task testTask, int expectedLogCount, ILogger logger)
        {
            await testTask;
            logger.Received(expectedLogCount).Information(Arg.Any<string>(), Arg.Any<T>());
        }

        private async Task TestInfoLogsWithTwoArgs<T1, T2>(Task testTask, int expectedLogCount, ILogger logger)
        {
            await testTask;
            logger.Received(expectedLogCount).Information(Arg.Any<string>(), Arg.Any<T1>(), Arg.Any<T2>());
        }

        #endregion
    }
}
