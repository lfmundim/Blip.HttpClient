using Blip.HttpClient.Exceptions;
using Blip.HttpClient.Factories;
using Blip.HttpClient.Services;
using Lime.Messaging.Resources;
using Lime.Protocol;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Serilog;
using Shouldly;
using System;
using System.Threading;
using System.Threading.Tasks;
using Blip.HttpClient.Services.Contacts;
using Take.Blip.Client;
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
            _logger = Substitute.For<ILogger>();
        }

        #region Methods unit tests
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

        [Fact]
        public async Task MergeContactUnitTest()
        {
            var id = EnvelopeId.NewId();
            var identity = Identity.Parse($"{id}.testingbots@0mn.io");

            var contact = new Contact
            {
                Name = id,
                Identity = identity
            };

            var mergeResponse = await _contactService.MergeAsync(identity, contact, CancellationToken.None, _logger);

            mergeResponse.Status.ShouldBe(CommandStatus.Success);
        }

        [Fact]
        public async Task DeleteContactUnitTest()
        {
            var identity = Identity.Parse("unittests.testingbots@0mn.io");

            var contact = new Contact
            {
                Identity = identity
            };
            await _contactService.SetAsync(identity, contact, CancellationToken.None);

            var deleteResponse = await _contactService.DeleteAsync(identity, CancellationToken.None, _logger);

            deleteResponse.Status.ShouldBe(CommandStatus.Success);
        }
        #endregion

        #region Log unit tests
        #region SetContact_Log
        [Fact]
        public async Task SetContactLogUnitTest_ShouldSucceed()
        {
            var contact = new Contact();
            var client = BuildSenderSubstitute_ReturnsSuccessStatus();
            var contactService = new ContactService(client);
            var logger = Substitute.For<ILogger>();
            var task = contactService.SetAsync(contact, CancellationToken.None, logger);

            await TestInfoLogsWithOneArg<Contact>(task, 2, logger);
        }

        [Fact]
        public async Task SetContactLogUnitTest_ShouldThrowBlipHttpClientEx()
        {
            var contact = new Contact();
            var logger = Substitute.For<ILogger>();
            var client = BuildSenderSubstitute_ReturnsFailureStatus();
            var contactService = new ContactService(client);
            var exceptionThrown = false;
            try
            {
                await contactService.SetAsync(contact, CancellationToken.None, logger);
            }
            catch (BlipHttpClientException bex)
            {
                logger.Received(1).Error(bex, Arg.Any<string>(), Arg.Any<Contact>());
                exceptionThrown = true;
            }
            finally
            {
                exceptionThrown.ShouldBe(true);
            }
        }

        [Fact]
        public async Task SetContactLogUnitTest_ShouldThrowEx()
        {
            var contact = new Contact();
            var logger = Substitute.For<ILogger>();
            var client = BuildSenderSubstitute_ThrowsException();
            var contactService = new ContactService(client);
            var exceptionThrown = false;
            try
            {
                await contactService.SetAsync(contact, CancellationToken.None, logger);
            }
            catch (Exception ex)
            {
                logger.Received(1).Error(ex, Arg.Any<string>(), Arg.Any<Contact>());
                exceptionThrown = true;
            }
            finally
            {
                exceptionThrown.ShouldBe(true);
            }
        }
        #endregion

        #region GetContact_Log
        [Fact]
        public async Task GetContactLogUnitTest_ShouldSucceed()
        {
            var identity = Identity.Parse("unittest.testingbots@0mn.io");
            var client = BuildSenderSubstitute_ReturnsSuccessStatus();
            var contactService = new ContactService(client);
            var logger = Substitute.For<ILogger>();
            await contactService.GetAsync(identity, CancellationToken.None, logger);

            logger.Received(1).Information(Arg.Any<string>(), Arg.Any<Identity>());
            logger.Received(1).Information(Arg.Any<string>(), Arg.Any<Identity>(), Arg.Any<Contact>());
        }

        [Fact]
        public async Task GetContactLogUnitTest_ShouldThrowBlipHttpClientEx()
        {
            var identity = Identity.Parse("unittest.testingbots@0mn.io");
            var logger = Substitute.For<ILogger>();
            var client = BuildSenderSubstitute_ReturnsFailureStatus();
            var contactService = new ContactService(client);
            var exceptionThrown = false;
            try
            {
                await contactService.GetAsync(identity, CancellationToken.None, logger);
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
        public async Task GetContactLogUnitTest_ShouldThrowEx()
        {
            var identity = Identity.Parse("unittest.testingbots@0mn.io");
            var logger = Substitute.For<ILogger>();
            var client = BuildSenderSubstitute_ThrowsException();
            var contactService = new ContactService(client);
            var exceptionThrown = false;
            try
            {
                await contactService.GetAsync(identity, CancellationToken.None, logger);
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

        #region MergeContact_Log
        [Fact]
        public async Task MergeContactLogUnitTest_ShouldSucceed()
        {
            var identity = Identity.Parse("unittest.testingbots@0mn.io");
            var contact = new Contact();
            var client = BuildSenderSubstitute_ReturnsSuccessStatus();
            var contactService = new ContactService(client);
            var logger = Substitute.For<ILogger>();

            var task = contactService.MergeAsync(identity, contact, CancellationToken.None, logger);

            await TestInfoLogsWithTwoArgs<Identity, Contact>(task, 2, logger);
        }

        [Fact]
        public async Task MergeContactLogUnitTest_ShouldThrowBlipHttpClientEx()
        {
            var identity = Identity.Parse("unittest.testingbots@0mn.io");
            var contact = new Contact();
            var client = BuildSenderSubstitute_ReturnsFailureStatus();
            var contactService = new ContactService(client);
            var logger = Substitute.For<ILogger>();
            var exceptionThrown = false;
            try
            {
                await contactService.MergeAsync(identity, contact, CancellationToken.None, logger);
            }
            catch (BlipHttpClientException bex)
            {
                logger.Received(1).Error(bex, Arg.Any<string>(), Arg.Any<Identity>(), Arg.Any<Contact>());
                exceptionThrown = true;
            }
            finally
            {
                exceptionThrown.ShouldBe(true);
            }
        }

        [Fact]
        public async Task MergeContactLogUnitTest_ShouldThrowEx()
        {
            var identity = Identity.Parse("unittest.testingbots@0mn.io");
            var contact = new Contact();
            var client = BuildSenderSubstitute_ThrowsException();
            var contactService = new ContactService(client);
            var logger = Substitute.For<ILogger>();
            var exceptionThrown = false;
            try
            {
                await contactService.MergeAsync(identity, contact, CancellationToken.None, logger);
            }
            catch (Exception ex)
            {
                logger.Received(1).Error(ex, Arg.Any<string>(), Arg.Any<Identity>(), Arg.Any<Contact>());
                exceptionThrown = true;
            }
            finally
            {
                exceptionThrown.ShouldBe(true);
            }
        }
        #endregion

        #region DeleteContact_Log
        [Fact]
        public async Task DeleteContactLogUnitTest_ShouldSucceed()
        {
            var identity = Identity.Parse("unittest.testingbots@0mn.io");
            var client = BuildSenderSubstitute_ReturnsSuccessStatus();
            var contactService = new ContactService(client);
            var logger = Substitute.For<ILogger>();

            var task = contactService.DeleteAsync(identity, CancellationToken.None, logger);

            await TestInfoLogsWithOneArg<Identity>(task, 2, logger);
        }

        [Fact]
        public async Task DeleteContactLogUnitTest_ShouldThrowBlipHttpClientEx()
        {
            var identity = Identity.Parse("unittest.testingbots@0mn.io");
            var client = BuildSenderSubstitute_ReturnsFailureStatus();
            var contactService = new ContactService(client);
            var logger = Substitute.For<ILogger>();
            var exceptionThrown = false;
            try
            {
                await contactService.DeleteAsync(identity, CancellationToken.None, logger);
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
        public async Task DeleteContactLogUnitTest_ShouldThrowEx()
        {
            var identity = Identity.Parse("unittest.testingbots@0mn.io");
            var client = BuildSenderSubstitute_ThrowsException();
            var contactService = new ContactService(client);
            var logger = Substitute.For<ILogger>();
            var exceptionThrown = false;
            try
            {
                await contactService.DeleteAsync(identity, CancellationToken.None, logger);
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
