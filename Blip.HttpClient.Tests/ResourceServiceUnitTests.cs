using Blip.HttpClient.Exceptions;
using Blip.HttpClient.Factories;
using Blip.HttpClient.Services.Resources;
using Lime.Messaging.Contents;
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
    public class ResourceServiceUnitTests
    {
        public readonly ResourceService _resourceService;
        public readonly ILogger _logger;
        public ResourceServiceUnitTests()
        {
            var clientFactory = new BlipClientFactory();
            var sender = clientFactory.BuildBlipClient("dGVzdGluZ2JvdHM6OU8zZEpWbHVaSWZNYmVnOWZaZzM=", Models.BlipProtocol.Tcp);
            _resourceService = new ResourceService(sender);
            _logger = Substitute.For<ILogger>();
        }


        #region Methods Unit Tests

        [Theory]
        [InlineData("")]
        [InlineData("dGVzdGluZ2JvdHM6OU8zZEpWbHVaSWZNYmVnOWZaZzM=")]
        public async Task SetAndGetResourceUnitTest(string authKey)
        {
            Command setResponse;
            PlainText getResponse;

            using (var cts = new CancellationTokenSource(TimeSpan.FromSeconds(10)))
            {
                if (authKey.Equals(""))
                {
                    setResponse = await _resourceService.SetAsync("UnitTest", PlainText.Parse("ResourceUnitTest"), _logger, cancellationToken: cts.Token);
                    getResponse = await _resourceService.GetAsync<PlainText>("UnitTest", _logger, cancellationToken: cts.Token);
                }
                else
                {
                    var resourceService = new ResourceService(authKey, Models.BlipProtocol.Tcp);
                    setResponse = await resourceService.SetAsync("UnitTest", PlainText.Parse("ResourceUnitTest"), _logger, cancellationToken: cts.Token);
                    getResponse = await resourceService.GetAsync<PlainText>("UnitTest", _logger, cancellationToken: cts.Token);
                }
            }

            setResponse.Status.ShouldBe(CommandStatus.Success);

            getResponse.ShouldNotBeNull();
            getResponse.Text.ShouldBe("ResourceUnitTest");
            getResponse.GetMediaType().ToString().ShouldBe("text/plain");
        }

        [Fact]
        public async Task GetIdsUnitTest()
        {
            DocumentCollection getResponse;
            using (var cts = new CancellationTokenSource(TimeSpan.FromSeconds(10)))
            {
                getResponse = await _resourceService.GetIdsAsync(_logger, cancellationToken: cts.Token);
            }

            getResponse.ShouldNotBeNull();
            getResponse.Total.ShouldBeGreaterThan(0);
        }

        [Fact]
        public async Task DeleteResourceUnitTest()
        {
            bool exceptionThrown = false;
            using (var cts = new CancellationTokenSource(TimeSpan.FromSeconds(10)))
            {
                await _resourceService.SetAsync("DeleteUnitTest", PlainText.Parse("Delete"), _logger, cancellationToken: cts.Token);
                var getResource = await _resourceService.GetAsync<PlainText>("DeleteUnitTest", _logger, cancellationToken: cts.Token);
                getResource.ShouldNotBeNull();

                var deleteResponse = await _resourceService.DeleteAsync("DeleteUnitTest", _logger, cts.Token);
                deleteResponse.ShouldNotBeNull();
                deleteResponse.Status.ShouldBe(CommandStatus.Success);
                try
                {
                    getResource = await _resourceService.GetAsync<PlainText>("DeleteUnitTest", _logger, cancellationToken: cts.Token);
                }
                catch (BlipHttpClientException bex)
                {
                    bex.Reason.Code.ShouldBe(67);
                    bex.Reason.Description.ShouldBe("The requested resource was not found");
                    exceptionThrown = true;
                }
            }
            exceptionThrown.ShouldBeTrue();
        }
        #endregion

        #region Log unit tests
        #region SetResource_Log
        [Fact]
        public async Task SetResourceLogUnitTest_ShouldSucceed()
        {
            var document = PlainText.Parse("LogUnitTest");
            var client = BuildSenderSubstitute_ReturnsSuccessStatus();
            var resourceService = new ResourceService(client);
            var logger = Substitute.For<ILogger>();
            await resourceService.SetAsync("LogUnitTest", document, logger: logger, cancellationToken: CancellationToken.None);

            logger.Received(1).Information(Arg.Any<string>(), Arg.Any<string>());
            logger.Received(1).Information(Arg.Any<string>(), Arg.Any<PlainText>(), Arg.Any<string>());
        }

        [Fact]
        public async Task SetResourceLogUnitTest_ShouldThrowBlipHttpClientEx()
        {
            var document = PlainText.Parse("LogUnitTest");
            var logger = Substitute.For<ILogger>();
            var client = BuildSenderSubstitute_ReturnsFailureStatus();
            var resourceService = new ResourceService(client);
            var exceptionThrown = false;
            try
            {
                await resourceService.SetAsync("LogUnitTest", document, logger: logger, cancellationToken: CancellationToken.None);
            }
            catch (BlipHttpClientException bex)
            {
                logger.Received(1).Error(bex, Arg.Any<string>(), Arg.Any<PlainText>(), Arg.Any<string>());
                exceptionThrown = true;
            }
            finally
            {
                exceptionThrown.ShouldBe(true);
            }
        }

        [Fact]
        public async Task SetResourceLogUnitTest_ShouldThrowEx()
        {
            var document = PlainText.Parse("LogUnitTest");
            var logger = Substitute.For<ILogger>();
            var client = BuildSenderSubstitute_ThrowsException();
            var resourceService = new ResourceService(client);
            var exceptionThrown = false;
            try
            {
                await resourceService.SetAsync("LogUnitTest", document, logger: logger, cancellationToken: CancellationToken.None);
            }
            catch (Exception ex)
            {
                logger.Received(1).Error(ex, Arg.Any<string>(), Arg.Any<PlainText>(), Arg.Any<string>());
                exceptionThrown = true;
            }
            finally
            {
                exceptionThrown.ShouldBe(true);
            }
        }
        #endregion

        #region GetResource_Log
        [Fact]
        public async Task GetResourceLogUnitTest_ShouldSucceed()
        {
            var client = BuildSenderSubstitute_ReturnsSuccessStatus();
            var resourceService = new ResourceService(client);
            var logger = Substitute.For<ILogger>();
            await resourceService.GetAsync<PlainText>("LogUnitTest", logger, CancellationToken.None);

            logger.Received(1).Information(Arg.Any<string>(), Arg.Any<string>());
            logger.Received(1).Information(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<PlainText>());
        }

        [Fact]
        public async Task GetResourceLogUnitTest_ShouldThrowBlipHttpClientEx()
        {
            var logger = Substitute.For<ILogger>();
            var client = BuildSenderSubstitute_ReturnsFailureStatus();
            var resourceService = new ResourceService(client);
            var exceptionThrown = false;
            try
            {
                await resourceService.GetAsync<PlainText>("LogUnitTest", logger, CancellationToken.None);
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
        public async Task GetResourceLogUnitTest_ShouldThrowEx()
        {
            var logger = Substitute.For<ILogger>();
            var client = BuildSenderSubstitute_ThrowsException();
            var resourceService = new ResourceService(client);
            var exceptionThrown = false;
            try
            {
                await resourceService.GetAsync<PlainText>("LogUnitTest", logger, CancellationToken.None);
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

        #region DeleteResource_Log
        [Fact]
        public async Task DeleteResourceLogUnitTest_ShouldSucceed()
        {
            var client = BuildSenderSubstitute_ReturnsSuccessStatus();
            var resourceService = new ResourceService(client);
            var logger = Substitute.For<ILogger>();

            var task = resourceService.DeleteAsync("LogUnitTest", logger, CancellationToken.None);

            await TestInfoLogsWithOneArg<string>(task, 2, logger);
        }

        [Fact]
        public async Task DeleteResourceLogUnitTest_ShouldThrowBlipHttpClientEx()
        {
            var client = BuildSenderSubstitute_ReturnsFailureStatus();
            var resourceService = new ResourceService(client);
            var logger = Substitute.For<ILogger>();
            var exceptionThrown = false;
            try
            {
                await resourceService.DeleteAsync("UnitTest", logger, CancellationToken.None);
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
        public async Task DeleteResourceLogUnitTest_ShouldThrowEx()
        {
            var client = BuildSenderSubstitute_ThrowsException();
            var resourceService = new ResourceService(client);
            var logger = Substitute.For<ILogger>();
            var exceptionThrown = false;
            try
            {
                await resourceService.DeleteAsync("UnitTest", logger, CancellationToken.None);
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

        #region GetIds_Log
        [Fact]
        public async Task GetIdsLogUnitTest_ShouldSucceed()
        {
            var client = BuildSenderSubstitute_ReturnsSuccessStatus();
            var resourceService = new ResourceService(client);
            var logger = Substitute.For<ILogger>();
            await resourceService.GetIdsAsync(logger: logger, cancellationToken: CancellationToken.None);

            logger.Received(1).Information(Arg.Any<string>());
            logger.Received(1).Information(Arg.Any<string>(), Arg.Any<DocumentCollection>());
        }

        [Fact]
        public async Task GetIdsLogUnitTest_ShouldThrowBlipHttpClientEx()
        {
            var logger = Substitute.For<ILogger>();
            var client = BuildSenderSubstitute_ReturnsFailureStatus();
            var resourceService = new ResourceService(client);
            var exceptionThrown = false;
            try
            {
                await resourceService.GetIdsAsync(logger: logger, cancellationToken: CancellationToken.None);
            }
            catch (BlipHttpClientException bex)
            {
                logger.Received(1).Error(bex, Arg.Any<string>());
                exceptionThrown = true;
            }
            finally
            {
                exceptionThrown.ShouldBe(true);
            }
        }

        [Fact]
        public async Task GetIdsLogUnitTest_ShouldThrowEx()
        {
            var logger = Substitute.For<ILogger>();
            var client = BuildSenderSubstitute_ThrowsException();
            var resourceService = new ResourceService(client);
            var exceptionThrown = false;
            try
            {
                await resourceService.GetIdsAsync(logger: logger, cancellationToken: CancellationToken.None);
            }
            catch (Exception ex)
            {
                logger.Received(1).Error(ex, Arg.Any<string>());
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
                },
                Resource = PlainText.Parse("Unit tests")
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

