using System;
using System.Threading;
using Blip.HttpClient.Exceptions;
using Blip.HttpClient.Factories;
using Blip.HttpClient.Services.Bucket;
using Lime.Protocol;
using NSubstitute;
using Serilog;
using Shouldly;
using System.Threading.Tasks;
using Lime.Messaging.Resources;
using NSubstitute.ExceptionExtensions;
using Take.Blip.Client;
using Xunit;

namespace Blip.HttpClient.Tests
{
    public class BucketServiceUnitTests
    {
        private const string FirstDocumentId = "UnitTestDocument";
        private const string SecondDocumentId = "UnitTestDocument1";
        private const string IdentityString = "unittests.testingbots@0mn.io";
        private readonly IBucketService _bucketService;
        private readonly ILogger _logger;

        public BucketServiceUnitTests()
        {
            var clientFactory = new BlipHttpClientFactory();
            var sender = clientFactory.BuildBlipHttpClient("dGVzdGluZ2JvdHM6OU8zZEpWbHVaSWZNYmVnOWZaZzM=");
            _bucketService = new BucketService(sender);
            _logger = Substitute.For<ILogger>();
        }

        #region Methods unit tests

        [Theory]
        [InlineData("")]
        [InlineData("dGVzdGluZ2JvdHM6OU8zZEpWbHVaSWZNYmVnOWZaZzM=")]
        public async Task Full_Bucket_UnitTests(string authKey)
        {
            await Store_Document_UnitTest(authKey);
            await Get_Document_UnitTest(authKey);
            await Delete_Document_UnitTest(authKey);
        }

        private async Task Store_Document_UnitTest(string authKey)
        {
            Command setResponse;

            var document = new IdentityDocument(IdentityString);

            if (authKey.Equals(""))
            {
                setResponse = await _bucketService.SetAsync(FirstDocumentId, document, _logger);
            }
            else
            {
                var bucketService = new BucketService(authKey);
                setResponse = await bucketService.SetAsync(SecondDocumentId, document, _logger);
            }

            setResponse.Status.ShouldBe(CommandStatus.Success);
        }

        private async Task Get_Document_UnitTest(string authKey)
        {
            IdentityDocument getResponse;
            var document = new IdentityDocument(IdentityString);

            if (authKey.Equals(""))
            {
                getResponse = await _bucketService.GetAsync<IdentityDocument>(FirstDocumentId, _logger);
            }
            else
            {
                var bucketService = new BucketService(authKey);
                getResponse = await bucketService.GetAsync<IdentityDocument>(SecondDocumentId, _logger);
            }

            getResponse.Value.Equals(document.Value).ShouldBeTrue();
        }

        private async Task Delete_Document_UnitTest(string authKey)
        {
            Command deleteResponse = new Command();
            IdentityDocument getResponse = new IdentityDocument();

            try
            {
                if (authKey.Equals(""))
                {
                    deleteResponse = await _bucketService.DeleteAsync(FirstDocumentId, _logger);
                    getResponse = await _bucketService.GetAsync<IdentityDocument>(FirstDocumentId, _logger);
                }
                else
                {
                    var bucketService = new BucketService(authKey);
                    deleteResponse = await bucketService.DeleteAsync(SecondDocumentId, _logger);
                    getResponse = await bucketService.GetAsync<IdentityDocument>(SecondDocumentId, _logger);
                }
            }
            catch (BlipHttpClientException)
            {
                getResponse.Value.ShouldBeNull();
            }
            finally
            {
                deleteResponse.Status.ShouldBe(CommandStatus.Success);
            }

        }

        #endregion

        #region Log unit tests

        #region Store_Log

        [Fact]
        public async Task BucketStoreLogUnitTest_ShouldSucceed()
        {
            var document = new IdentityDocument(IdentityString);
            var client = BuildSenderSubstitute_ReturnsSuccessStatus();
            var bucketService = new BucketService(client);
            var logger = Substitute.For<ILogger>();
            var task = bucketService.SetAsync(FirstDocumentId, document, logger);

            await TestInfoLogsWithTwoArgs<IdentityDocument, string>(task, 2, logger);
        }

        [Fact]
        public async Task BucketStoreLogUnitTest_ShouldThrowBlipHttpClientEx()
        {
            var document = new IdentityDocument(IdentityString);
            var logger = Substitute.For<ILogger>();
            var client = BuildSenderSubstitute_ReturnsFailureStatus();
            var bucketService = new BucketService(client);
            var exceptionThrown = false;
            try
            {
                await bucketService.SetAsync(FirstDocumentId, document, logger);
            }
            catch (BlipHttpClientException bex)
            {
                logger.Received(1).Error(bex, Arg.Any<string>(), Arg.Any<IdentityDocument>(), Arg.Any<string>());
                exceptionThrown = true;
            }
            finally
            {
                exceptionThrown.ShouldBe(true);
            }
        }

        [Fact]
        public async Task BucketStoreLogUnitTest_ShouldThrowEx()
        {
            var document = new IdentityDocument(IdentityString);
            var logger = Substitute.For<ILogger>();
            var client = BuildSenderSubstitute_ThrowsException();
            var bucketService = new BucketService(client);
            var exceptionThrown = false;
            try
            {
                await bucketService.SetAsync(FirstDocumentId, document, logger);
            }
            catch (Exception ex)
            {
                logger.Received(1).Error(ex, Arg.Any<string>(), Arg.Any<IdentityDocument>(), Arg.Any<string>());
                exceptionThrown = true;
            }
            finally
            {
                exceptionThrown.ShouldBe(true);
            }
        }

        #endregion

        #region Get_Log

        [Fact]
        public async Task BucketGetLogUnitTest_ShouldSucceed()
        {
            var client = BuildSenderSubstitute_ReturnsSuccessStatus();
            var bucketService = new BucketService(client);
            var logger = Substitute.For<ILogger>();
            var task = bucketService.GetAsync<IdentityDocument>(FirstDocumentId, logger);

            await TestInfoLogsWithTwoArgs<string, IdentityDocument>(task, 1, logger);
            await TestInfoLogsWithOneArg<string>(task, 1, logger);
        }

        [Fact]
        public async Task BucketGetLogUnitTest_ShouldThrowBlipHttpClientEx()
        {
            var logger = Substitute.For<ILogger>();
            var client = BuildSenderSubstitute_ReturnsFailureStatus();
            var bucketService = new BucketService(client);
            var exceptionThrown = false;
            try
            {
                await bucketService.GetAsync<IdentityDocument>(FirstDocumentId, logger);
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
        public async Task BucketGetLogUnitTest_ShouldThrowEx()
        {
            var logger = Substitute.For<ILogger>();
            var client = BuildSenderSubstitute_ThrowsException();
            var bucketService = new BucketService(client);
            var exceptionThrown = false;
            try
            {
                await bucketService.GetAsync<IdentityDocument>(FirstDocumentId, logger);
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

        #region Delete_Log

        [Fact]
        public async Task BucketDeleteLogUnitTest_ShouldSucceed()
        {
            var client = BuildSenderSubstitute_ReturnsSuccessStatus();
            var bucketService = new BucketService(client);
            var logger = Substitute.For<ILogger>();
            var task = bucketService.DeleteAsync(FirstDocumentId, logger);

            await TestInfoLogsWithOneArg<string>(task, 2, logger);
        }

        [Fact]
        public async Task BucketDeleteLogUnitTest_ShouldThrowBlipHttpClientEx()
        {
            var logger = Substitute.For<ILogger>();
            var client = BuildSenderSubstitute_ReturnsFailureStatus();
            var bucketService = new BucketService(client);
            var exceptionThrown = false;
            try
            {
                await bucketService.DeleteAsync(FirstDocumentId, logger);
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
        public async Task BucketDeleteLogUnitTest_ShouldThrowEx()
        {
            var logger = Substitute.For<ILogger>();
            var client = BuildSenderSubstitute_ThrowsException();
            var bucketService = new BucketService(client);
            var exceptionThrown = false;
            try
            {
                await bucketService.DeleteAsync(FirstDocumentId, logger);
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
                Resource = new Contact()
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
