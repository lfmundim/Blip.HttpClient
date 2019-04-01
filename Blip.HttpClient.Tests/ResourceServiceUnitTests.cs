using Blip.HttpClient.Factories;
using Blip.HttpClient.Services.Resources;
using Lime.Messaging.Contents;
using NSubstitute;
using Serilog;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Blip.HttpClient.Tests
{
    public class ResourceServiceUnitTests
    {
        public readonly ResourceService _resourceService;
        public readonly ILogger _logger;
        public ResourceServiceUnitTests()
        {
            var clientFactory = new BlipHttpClientFactory();
            var sender = clientFactory.BuildBlipHttpClient("dGVzdGluZ2JvdHM6OU8zZEpWbHVaSWZNYmVnOWZaZzM=");
            _resourceService = new ResourceService(sender);
            _logger = Substitute.For<ILogger>();
        }


        #region Methods Unit Tests

        [Fact]
        public async Task GetAll_UnitTests()
        {
            var resources = await _resourceService.GetAllAsync();

            resources.ShouldNotBeNull();
        }

        #endregion
    }
}
