using System;
using System.Threading;
using System.Threading.Tasks;
using Serilog;
using Take.Blip.Client.Extensions.ArtificialIntelligence;
using Takenet.Iris.Messaging.Resources.ArtificialIntelligence;

namespace Blip.HttpClient.Services.ArtificialIntelligence
{
    /// <summary>
    /// Overload interface for the already existant <c>IArtificalIntelligenceExtension</c> to add logs
    /// </summary>
    public interface IArtificialIntelligenceService : IArtificialIntelligenceExtension
    {
        /// <summary>
        /// Runs an analysis on a given request using the preferred AI Engine.
        /// Adds logging funcionality.
        /// </summary>
        /// <returns>AI Engine response</returns>
        /// <param name="analysisRequest"></param>
        /// <param name="logger">Logger.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        Task<AnalysisResponse> AnalyzeAsync(AnalysisRequest analysisRequest, ILogger logger, CancellationToken cancellationToken = default);
    }
}
