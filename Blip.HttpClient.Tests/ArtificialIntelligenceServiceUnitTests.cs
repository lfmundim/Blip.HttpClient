using System.Threading;
using System.Threading.Tasks;
using Blip.HttpClient.Factories;
using Blip.HttpClient.Services.ArtificialIntelligence;
using Lime.Protocol;
using NSubstitute;
using Serilog;
using Shouldly;
using Takenet.Iris.Messaging.Resources.ArtificialIntelligence;
using Xunit;

namespace Blip.HttpClient.Tests
{
    public class ArtificialIntelligenceServiceUnitTests
    {
        private readonly IArtificialIntelligenceService _aiService;
        private readonly ILogger _logger;

        public ArtificialIntelligenceServiceUnitTests()
        {
            var clientFactory = new BlipHttpClientFactory();
            var sender = clientFactory.BuildBlipHttpClient("dGVzdGluZ2JvdHM6OU8zZEpWbHVaSWZNYmVnOWZaZzM=");
            _aiService = new ArtificialIntelligenceService(sender);
            _logger = Substitute.For<ILogger>();
        }

        #region Methods unit tests

        [Theory]
        [InlineData("")]
        [InlineData("dGVzdGluZ2JvdHM6OU8zZEpWbHVaSWZNYmVnOWZaZzM=")]
        public async Task Analyze_UnitTests(string authKey)
        {
            AnalysisResponse analyzeResponse;
            var analysisRequest = new AnalysisRequest
            {
                Text = "Unit tests"
            };

            if (authKey.Equals(""))
            {
                analyzeResponse = await _aiService.AnalyzeAsync(analysisRequest, _logger, CancellationToken.None);
            }
            else
            {
                var aiService = new ArtificialIntelligenceService(authKey);
                analyzeResponse = await aiService.AnalyzeAsync(analysisRequest, _logger, CancellationToken.None);
            }

            analyzeResponse.ShouldNotBeNull();
        }


        #endregion

        #region Log unit tests



        #endregion
    }
}
