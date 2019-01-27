using Blip.HttpClient.Exceptions;
using Blip.HttpClient.Factories;
using Blip.HttpClient.Services;
using Lime.Messaging.Contents;
using Lime.Protocol;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Serilog;
using Shouldly;
using System;
using System.Threading;
using System.Threading.Tasks;
using Blip.HttpClient.Services.Broadcast;
using Take.Blip.Client;
using Xunit;

namespace Blip.HttpClient.Tests
{
    public class BroadcastServiceUnitTests
    {
        private const string FirstListName = "HttpClient_UnitTests";
        private const string SecondListName = "HttpClient_UnitTests1";
        private const string RecipientIdentity = "unittests.testingbots@0mn.io";
        private readonly IBroadcastService _broadcastService;
        private readonly ILogger _logger;

        public BroadcastServiceUnitTests()
        {
            var clientFactory = new BlipHttpClientFactory();
            var sender = clientFactory.BuildBlipHttpClient("dGVzdGluZ2JvdHM6OU8zZEpWbHVaSWZNYmVnOWZaZzM=");
            _broadcastService = new BroadcastService(sender);
            _logger = Substitute.For<ILogger>();
        }

        #region Methods unit tests

        [Theory]
        [InlineData("", FirstListName)]
        [InlineData("dGVzdGluZ2JvdHM6OU8zZEpWbHVaSWZNYmVnOWZaZzM=", SecondListName)]
        public async Task Full_Distribution_List_UnitTests(string authKey, string listName)
        {
            await Create_Distribution_List_UnitTest(authKey, listName);
            await Add_And_Check_Recipient_UnitTest(authKey, listName);
            await Get_Recipients_UnitTest(authKey, listName);
            await Send_Broadcast_UnitTest(listName);
            await Delete_And_Check_Recipient_UnitTest(authKey, listName);
            await Delete_Distribution_List_UnitTest(authKey, listName);
        }

        private async Task Create_Distribution_List_UnitTest(string authKey, string listName)
        {
            Command createResponse;

            if (authKey.Equals(""))
            {
                createResponse = await _broadcastService.CreateDistributionListAsync(listName, CancellationToken.None, _logger);
            }
            else
            {
                var broadcastService = new BroadcastService(authKey);
                createResponse = await broadcastService.CreateDistributionListAsync(listName, CancellationToken.None, _logger);
            }

            createResponse.Status.ShouldBe(CommandStatus.Success);
        }

        private async Task Add_And_Check_Recipient_UnitTest(string authKey, string listName)
        {
            Command addResponse;
            bool checkResponse;
            var firstIdentity = Identity.Parse(RecipientIdentity);
            var secondIdentity = Identity.Parse(RecipientIdentity + "1");

            if (authKey.Equals(""))
            {
                addResponse = await _broadcastService.AddRecipientAsync(listName, firstIdentity, CancellationToken.None, _logger);
                checkResponse = await _broadcastService.HasRecipientAsync(listName, firstIdentity, CancellationToken.None);
            }
            else
            {
                var broadcastService = new BroadcastService(authKey);
                addResponse = await broadcastService.AddRecipientAsync(listName, secondIdentity, CancellationToken.None, _logger);
                checkResponse = await broadcastService.HasRecipientAsync(listName, secondIdentity, CancellationToken.None);
            }

            addResponse.Status.ShouldBe(CommandStatus.Success);
            checkResponse.ShouldBeTrue();
        }

        private async Task Get_Recipients_UnitTest(string authKey, string listName)
        {
            DocumentCollection getResponse;

            if (authKey.Equals(""))
            {
                getResponse = await _broadcastService.GetRecipientsAsync(_logger, listName, 0, 100, CancellationToken.None);
            }
            else
            {
                var broadcastService = new BroadcastService(authKey);
                getResponse = await broadcastService.GetRecipientsAsync(_logger, listName, 0, 100, CancellationToken.None);
            }

            getResponse.Total.ShouldBeGreaterThan(0);
            getResponse.ItemType.ToString().ShouldBe("application/vnd.lime.identity");
        }

        private async Task Send_Broadcast_UnitTest(string listName)
        {
            var sender = Substitute.For<ISender>();
            var calls = 0;
            sender.When(x => x.SendMessageAsync(Arg.Any<Message>(), Arg.Any<CancellationToken>())).Do(x => calls++);
            var broadcastService = new BroadcastService(sender);

            await broadcastService.SendMessageAsync(listName, PlainText.Parse("UnitTests"), _logger);

            calls.ShouldBeGreaterThan(0);
        }

        private async Task Delete_And_Check_Recipient_UnitTest(string authKey, string listName)
        {
            Command deleteResponse;
            bool getResponse;
            var firstIdentity = Identity.Parse(RecipientIdentity);
            var secondIdentity = Identity.Parse(RecipientIdentity + "1");

            if (authKey.Equals(""))
            {
                deleteResponse = await _broadcastService.DeleteRecipientAsync(listName, firstIdentity, CancellationToken.None, _logger);
                getResponse = await _broadcastService.HasRecipientAsync(listName, firstIdentity, CancellationToken.None);
            }
            else
            {
                var broadcastService = new BroadcastService(authKey);
                deleteResponse = await broadcastService.DeleteRecipientAsync(listName, secondIdentity, CancellationToken.None, _logger);
                getResponse = await broadcastService.HasRecipientAsync(listName, secondIdentity, CancellationToken.None);
            }

            deleteResponse.Status.ShouldBe(CommandStatus.Success);
            getResponse.ShouldBeFalse();
        }

        private async Task Delete_Distribution_List_UnitTest(string authKey, string listName)
        {
            Command deleteResponse;

            if (authKey.Equals(""))
            {
                deleteResponse = await _broadcastService.DeleteDistributionListAsync(listName, CancellationToken.None, _logger);
            }
            else
            {
                var broadcastService = new BroadcastService(authKey);
                deleteResponse = await broadcastService.DeleteDistributionListAsync(listName, CancellationToken.None, _logger);
            }

            deleteResponse.Status.ShouldBe(CommandStatus.Success);
        }

        #endregion

        #region Log unit tests

        #region CreateList_Log

        [Fact]
        public async Task CreateDistributionListLogUnitTest_ShouldSucceed()
        {
            var client = BuildSenderSubstitute_ReturnsSuccessStatus();
            var broadcastService = new BroadcastService(client);
            var logger = Substitute.For<ILogger>();
            var task = broadcastService.CreateDistributionListAsync(FirstListName, CancellationToken.None, logger);

            await TestInfoLogsWithOneArg<string>(task, 2, logger);
        }

        [Fact]
        public async Task CreateDistributionListLogUnitTest_ShouldThrowBlipHttpClientEx()
        {
            var logger = Substitute.For<ILogger>();
            var client = BuildSenderSubstitute_ReturnsFailureStatus();
            var broadcastService = new BroadcastService(client);
            var exceptionThrown = false;
            try
            {
                await broadcastService.CreateDistributionListAsync(FirstListName, CancellationToken.None, logger);
            }
            catch (BlipHttpClientException bex)
            {
                logger.Received(1).Error(bex, Arg.Any<string>(), Arg.Any<string>());
                exceptionThrown = true;
            }
            finally
            {
                exceptionThrown.ShouldBe(true);
            }
        }

        [Fact]
        public async Task CreateDistributionListLogUnitTest_ShouldThrowEx()
        {
            var logger = Substitute.For<ILogger>();
            var client = BuildSenderSubstitute_ThrowsException();
            var broadcastService = new BroadcastService(client);
            var exceptionThrown = false;
            try
            {
                await broadcastService.CreateDistributionListAsync(FirstListName, CancellationToken.None, logger);
            }
            catch (Exception ex)
            {
                logger.Received(1).Error(ex, Arg.Any<string>(), Arg.Any<string>());
                exceptionThrown = true;
            }
            finally
            {
                exceptionThrown.ShouldBe(true);
            }
        }

        #endregion

        #region AddRecipient_Log

        [Fact]
        public async Task AddRecipientLogUnitTest_ShouldSucceed()
        {
            var identity = Identity.Parse(RecipientIdentity);
            var client = BuildSenderSubstitute_ReturnsSuccessStatus();
            var broadcastService = new BroadcastService(client);
            var logger = Substitute.For<ILogger>();
            var task = broadcastService.AddRecipientAsync(FirstListName, identity, CancellationToken.None, logger);

            await TestInfoLogsWithTwoArgs<Identity, string>(task, 2, logger);
        }

        [Fact]
        public async Task AddRecipientLogUnitTest_ShouldThrowBlipHttpClientEx()
        {
            var identity = Identity.Parse(RecipientIdentity);
            var logger = Substitute.For<ILogger>();
            var client = BuildSenderSubstitute_ReturnsFailureStatus();
            var broadcastService = new BroadcastService(client);
            var exceptionThrown = false;

            try
            {
                await broadcastService.AddRecipientAsync(FirstListName, identity, CancellationToken.None, logger);
            }
            catch (BlipHttpClientException bex)
            {
                logger.Received(1).Error(bex, Arg.Any<string>(), Arg.Any<Identity>(), Arg.Any<string>());
                exceptionThrown = true;
            }
            finally
            {
                exceptionThrown.ShouldBe(true);
            }
        }

        [Fact]
        public async Task AddRecipientLogUnitTest_ShouldThrowEx()
        {
            var identity = Identity.Parse(RecipientIdentity);
            var logger = Substitute.For<ILogger>();
            var client = BuildSenderSubstitute_ThrowsException();
            var broadcastService = new BroadcastService(client);
            var exceptionThrown = false;
            try
            {
                await broadcastService.AddRecipientAsync(FirstListName, identity, CancellationToken.None, logger);
            }
            catch (Exception ex)
            {
                logger.Received(1).Error(ex, Arg.Any<string>(), Arg.Any<Identity>(), Arg.Any<string>());
                exceptionThrown = true;
            }
            finally
            {
                exceptionThrown.ShouldBe(true);
            }
        }

        #endregion

        #region DeleteRecipient_log

        [Fact]
        public async Task DeleteRecipientLogUnitTest_ShouldSucceed()
        {
            var identity = Identity.Parse(RecipientIdentity);
            var client = BuildSenderSubstitute_ReturnsSuccessStatus();
            var broadcastService = new BroadcastService(client);
            var logger = Substitute.For<ILogger>();
            var task = broadcastService.DeleteRecipientAsync(FirstListName, identity, CancellationToken.None, logger);

            await TestInfoLogsWithTwoArgs<Identity, string>(task, 2, logger);
        }

        [Fact]
        public async Task DeleteRecipientLogUnitTest_ShouldThrowBlipHttpClientEx()
        {
            var identity = Identity.Parse(RecipientIdentity);
            var logger = Substitute.For<ILogger>();
            var client = BuildSenderSubstitute_ReturnsFailureStatus();
            var broadcastService = new BroadcastService(client);
            var exceptionThrown = false;

            try
            {
                await broadcastService.DeleteRecipientAsync(FirstListName, identity, CancellationToken.None, logger);
            }
            catch (BlipHttpClientException bex)
            {
                logger.Received(1).Error(bex, Arg.Any<string>(), Arg.Any<Identity>(), Arg.Any<string>());
                exceptionThrown = true;
            }
            finally
            {
                exceptionThrown.ShouldBe(true);
            }
        }

        [Fact]
        public async Task DeleteRecipientLogUnitTest_ShouldThrowEx()
        {
            var identity = Identity.Parse(RecipientIdentity);
            var logger = Substitute.For<ILogger>();
            var client = BuildSenderSubstitute_ThrowsException();
            var broadcastService = new BroadcastService(client);
            var exceptionThrown = false;
            try
            {
                await broadcastService.DeleteRecipientAsync(FirstListName, identity, CancellationToken.None, logger);
            }
            catch (Exception ex)
            {
                logger.Received(1).Error(ex, Arg.Any<string>(), Arg.Any<Identity>(), Arg.Any<string>());
                exceptionThrown = true;
            }
            finally
            {
                exceptionThrown.ShouldBe(true);
            }
        }

        #endregion

        #region DeleteList_Log

        [Fact]
        public async Task DeleteDistributionListLogUnitTest_ShouldSucceed()
        {
            var client = BuildSenderSubstitute_ReturnsSuccessStatus();
            var broadcastService = new BroadcastService(client);
            var logger = Substitute.For<ILogger>();
            var task = broadcastService.DeleteDistributionListAsync(FirstListName, CancellationToken.None, logger);

            await TestInfoLogsWithOneArg<string>(task, 2, logger);
        }

        [Fact]
        public async Task DeleteDistributionListLogUnitTest_ShouldThrowBlipHttpClientEx()
        {
            var logger = Substitute.For<ILogger>();
            var client = BuildSenderSubstitute_ReturnsFailureStatus();
            var broadcastService = new BroadcastService(client);
            var exceptionThrown = false;
            try
            {
                await broadcastService.DeleteDistributionListAsync(FirstListName, CancellationToken.None, logger);
            }
            catch (BlipHttpClientException bex)
            {
                logger.Received(1).Error(bex, Arg.Any<string>(), Arg.Any<string>());
                exceptionThrown = true;
            }
            finally
            {
                exceptionThrown.ShouldBe(true);
            }
        }

        [Fact]
        public async Task DeleteDistributionListLogUnitTest_ShouldThrowEx()
        {
            var logger = Substitute.For<ILogger>();
            var client = BuildSenderSubstitute_ThrowsException();
            var broadcastService = new BroadcastService(client);
            var exceptionThrown = false;
            try
            {
                await broadcastService.DeleteDistributionListAsync(FirstListName, CancellationToken.None, logger);
            }
            catch (Exception ex)
            {
                logger.Received(1).Error(ex, Arg.Any<string>(), Arg.Any<string>());
                exceptionThrown = true;
            }
            finally
            {
                exceptionThrown.ShouldBe(true);
            }
        }

        #endregion

        #region GetRecipients_Log

        [Fact]
        public async Task GetRecipientsLogUnitTest_ShouldSucceed()
        {
            var client = BuildSenderSubstitute_ReturnsSuccessStatus();
            var broadcastService = new BroadcastService(client);
            var logger = Substitute.For<ILogger>();
            var task = broadcastService.GetRecipientsAsync(logger, FirstListName);

            await TestInfoLogsWithThreeArgs<string, int, int>(task, 2, logger);
        }

        [Fact]
        public async Task GetRecipientsLogUnitTest_ShouldThrowBlipHttpClientEx()
        {
            var logger = Substitute.For<ILogger>();
            var client = BuildSenderSubstitute_ReturnsFailureStatus();
            var broadcastService = new BroadcastService(client);
            var exceptionThrown = false;
            try
            {
                await broadcastService.GetRecipientsAsync(logger, FirstListName);
            }
            catch (BlipHttpClientException bex)
            {
                logger.Received(1).Error(bex, Arg.Any<string>(), Arg.Any<string>());
                exceptionThrown = true;
            }
            finally
            {
                exceptionThrown.ShouldBe(true);
            }
        }

        [Fact]
        public async Task GetRecipientsLogUnitTest_ShouldThrowEx()
        {
            var logger = Substitute.For<ILogger>();
            var client = BuildSenderSubstitute_ThrowsException();
            var broadcastService = new BroadcastService(client);
            var exceptionThrown = false;
            try
            {
                await broadcastService.GetRecipientsAsync(logger, FirstListName);
            }
            catch (Exception ex)
            {
                logger.Received(1).Error(ex, Arg.Any<string>(), Arg.Any<string>());
                exceptionThrown = true;
            }
            finally
            {
                exceptionThrown.ShouldBe(true);
            }
        }

        #endregion

        #region SendMessageLog

        [Fact]
        public async Task SendBroadcastLogUnitTest_ShouldSucceed()
        {
            var client = BuildSenderSubstitute_ReturnsSuccessStatus();
            var broadcastService = new BroadcastService(client);
            var logger = Substitute.For<ILogger>();
            var task = broadcastService.SendMessageAsync(FirstListName, PlainText.Parse("UnitTests"), logger);

            await TestInfoLogsWithTwoArgs<string, Document>(task, 2, logger);
        }

        [Fact]
        public async Task SendBroadcastLogUnitTest_ShouldThrowEx()
        {
            var logger = Substitute.For<ILogger>();
            var client = BuildSenderSubstitute_ThrowsException();
            var broadcastService = new BroadcastService(client);
            var exceptionThrown = false;
            try
            {
                await broadcastService.SendMessageAsync(FirstListName, PlainText.Parse("UnitTests"), logger);
            }
            catch (Exception ex)
            {
                logger.Received(1).Error(ex, Arg.Any<string>(), Arg.Any<string>(), Arg.Any<Document>());
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
            client
                .SendMessageAsync(Arg.Any<Message>(), Arg.Any<CancellationToken>())
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

        private async Task TestInfoLogsWithThreeArgs<T1, T2, T3>(Task testTask, int expectedLogCount, ILogger logger)
        {
            await testTask;
            logger.Received(expectedLogCount).Information(Arg.Any<string>(), Arg.Any<T1>(), Arg.Any<T2>(), Arg.Any<T3>());
        }

        #endregion
    }
}
