using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Blip.HttpClient.Exceptions;
using Blip.HttpClient.Factories;
using Lime.Protocol;
using Serilog;
using Take.Blip.Client;
using Take.Blip.Client.Extensions.ArtificialIntelligence;
using Takenet.Iris.Messaging.Resources.ArtificialIntelligence;

namespace Blip.HttpClient.Services.ArtificialIntelligence
{
    /// <summary>
    /// allows the creation, training and publication of artificial intelligence models in the providers associated with the chatbot, 
    /// besides performing sentence analysis to identify intentions and entities
    /// </summary>
    public class ArtificialIntelligenceService : IArtificialIntelligenceService
    {
        private readonly ISender _sender;
        private readonly IArtificialIntelligenceExtension _aiExtension;

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="T:Blip.HttpClient.Services.ArtificialIntelligence.ArtificialIntelligenceService"/> class.
        /// </summary>
        /// <param name="sender">TCP or HTTP BLiP Client.</param>
        public ArtificialIntelligenceService(ISender sender)
        {
            _sender = sender;
            _aiExtension = new ArtificialIntelligenceExtension(_sender);
        }

        public ArtificialIntelligenceService(string authKey)
        {
            var factory = new BlipHttpClientFactory();
            _sender = factory.BuildBlipHttpClient(authKey);
            _aiExtension = new ArtificialIntelligenceExtension(_sender);
        }

        /// <summary>
        /// Runs an analysis on a given request using the preferred AI Engine
        /// </summary>
        /// <returns>AI Engine response</returns>
        /// <param name="analysisRequest"></param>
        /// <param name="cancellationToken"></param>
        public async Task<AnalysisResponse> AnalyzeAsync(AnalysisRequest analysisRequest, CancellationToken cancellationToken = default)
        {
            return await _aiExtension.AnalyzeAsync(analysisRequest, cancellationToken);
        }


        /// <summary>
        /// Runs an analysis on a given request using the preferred AI Engine.
        /// Adds logging funcionality.
        /// </summary>
        /// <returns>AI Engine response</returns>
        /// <param name="analysisRequest"></param>
        /// <param name="logger">Logger.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        public async Task<AnalysisResponse> AnalyzeAsync(AnalysisRequest analysisRequest, ILogger logger, CancellationToken cancellationToken = default)
        {
            try
            {
                logger.Information("[Analyze] Trying to get analyze {@analysisRequest}", analysisRequest);
                var command = new Command()
                {
                    Method = CommandMethod.Set,
                    Uri = new LimeUri($"/analysis"),
                    To = "postmaster@ai.msging.net",
                    Resource = analysisRequest
                };

                var analysisResponse = await _sender.ProcessCommandAsync(command, cancellationToken);
                if (analysisResponse.Status != CommandStatus.Success)
                {
                    throw new BlipHttpClientException("Failed to get analyze the request", analysisResponse);
                }
                logger.Information("[Analyze] Successfully analyzed {@analysisRequest}: {@analysisResponse}", 
                                   analysisRequest, analysisResponse.Resource as AnalysisResponse);

                return analysisResponse.Resource as AnalysisResponse;
            }
            catch (Exception ex)
            {
                logger.Error(ex, "[Analyze] Failed to analyze {@analysisRequest}", analysisRequest);
                throw;
            }
        }

        /// <summary>
        /// Gets the answer with the given <paramref name="answerId"/> for the requested <paramref name="intentionId"/>
        /// </summary>
        /// <returns>The answer object</returns>
        /// <param name="intentionId">Intention identifier.</param>
        /// <param name="answerId">Answer identifier.</param>
        /// <param name="cancellationToken"></param>
        public async Task<Answer> GetAnswerAsync(string intentionId, string answerId, CancellationToken cancellationToken = default)
        {
            return await _aiExtension.GetAnswerAsync(intentionId, answerId, cancellationToken);
        }

        /// <summary>
        /// Gets the possible answers from the given <paramref name="intentionId"/>
        /// </summary>
        /// <returns>Possible intention answers.</returns>
        /// <param name="intentionId">Intention identifier.</param>
        /// <param name="skip">Ammount of answers to skip from the start</param>
        /// <param name="take">Ammount of answers to recover</param>
        /// <param name="ascending">If set to <c>true</c> the answer order check is ascending.</param>
        /// <param name="cancellationToken"></param>
        public async Task<DocumentCollection> GetAnswersAsync(string intentionId, int skip = 0, int take = 100, bool ascending = true, CancellationToken cancellationToken = default)
        {
            return await _aiExtension.GetAnswersAsync(intentionId, skip, take, ascending, cancellationToken);
        }

        /// <summary>
        /// Gets the entities async.
        /// </summary>
        /// <returns>The entities async.</returns>
        /// <param name="skip">Ammount of entities to skip from the start. Default is 0</param>
        /// <param name="take">Ammount of entities to recover. Default is 100</param>
        /// <param name="ascending">If set to <c>true</c> the entity order check is ascending.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        public async Task<DocumentCollection> GetEntitiesAsync(int skip = 0, int take = 100, bool ascending = true, CancellationToken cancellationToken = default)
        {
            return await _aiExtension.GetEntitiesAsync(skip, take, ascending, cancellationToken);
        }

        /// <summary>
        /// Recovers an Entity with a given <paramref name="id"/>
        /// </summary>
        /// <returns>The entity</returns>
        /// <param name="id">Entity Identifier.</param>
        /// <param name="cancellationToken"></param>
        public async Task<Entity> GetEntityAsync(string id, CancellationToken cancellationToken = default)
        {
            return await _aiExtension.GetEntityAsync(id, cancellationToken);
        }

        /// <summary>
        /// Recovers an Intention with a given <paramref name="id"/>
        /// </summary>
        /// <returns>The intention.</returns>
        /// <param name="id">Intention Identifier.</param>
        /// <param name="cancellationToken"></param>
        public async Task<Intention> GetIntentionAsync(string id, CancellationToken cancellationToken = default)
        {
            return await _aiExtension.GetIntentionAsync(id, cancellationToken);
        }

        /// <summary>
        /// Gets <paramref name="take"/> bot's intentions from <paramref name="skip"/>
        /// </summary>
        /// <returns>The intentions async.</returns>
        /// <param name="skip">Intentions to skip from the start. Default is 0</param>
        /// <param name="take">Intentions to get from skip. Default is 100.</param>
        /// <param name="ascending">If set to <c>true</c> the intention order check is ascending.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        public async Task<DocumentCollection> GetIntentionsAsync(int skip = 0, int take = 100, bool ascending = true, CancellationToken cancellationToken = default)
        {
            return await _aiExtension.GetIntentionsAsync(skip, take, ascending, cancellationToken);
        }

        /// <summary>
        /// Gets the previously trained models
        /// </summary>
        /// <returns>The previous models.</returns>
        /// <param name="skip">Models to skip from the start. Default is 0</param>
        /// <param name="take">Models to get from skip. Default is 100.</param>
        /// <param name="ascending">If set to <c>true</c> the models order check is ascending.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        public async Task<DocumentCollection> GetModelsAsync(int skip = 0, int take = 100, bool ascending = true, CancellationToken cancellationToken = default)
        {
            return await _aiExtension.GetModelsAsync(skip, take, ascending, cancellationToken);
        }

        /// <summary>
        /// Recovers a question with a given <paramref name="questionId"/> for the given intention
        /// </summary>
        /// <returns>The question.</returns>
        /// <param name="intentionId">Intention identifier.</param>
        /// <param name="questionId">Question identifier.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        public async Task<Question> GetQuestionAsync(string intentionId, string questionId, CancellationToken cancellationToken = default)
        {
            return await _aiExtension.GetQuestionAsync(intentionId, questionId, cancellationToken);
        }

        /// <summary>
        /// Gets the questions from a given intention.
        /// </summary>
        /// <returns>The questions.</returns>
        /// <param name="intentionId">Intention identifier.</param>
        /// <param name="skip">Questions to skip from the start. Default is 0</param>
        /// <param name="take">Questions to get from skip. Default is 100.</param>
        /// <param name="ascending">If set to <c>true</c> the questions order check is ascending.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        public async Task<DocumentCollection> GetQuestionsAsync(string intentionId, int skip = 0, int take = 100, bool ascending = true, CancellationToken cancellationToken = default)
        {
            return await _aiExtension.GetQuestionsAsync(intentionId, skip, take, ascending, cancellationToken);
        }

        /// <summary>
        /// Publishes the model with id <paramref name="id"/>
        /// </summary>
        /// <param name="id">Model Identifier.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        public async Task PublishModelAsync(string id, CancellationToken cancellationToken = default)
        {
            await _aiExtension.PublishModelAsync(id, cancellationToken);
        }

        /// <summary>
        /// Sends an <paramref name="analysisFeedback"/> to a given past <paramref name="analysisId"/>
        /// </summary>
        /// <param name="analysisId">Analysis identifier.</param>
        /// <param name="analysisFeedback">Analysis feedback.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        public async Task SendFeedbackAsync(string analysisId, AnalysisFeedback analysisFeedback, CancellationToken cancellationToken = default)
        {
            await _aiExtension.SendFeedbackAsync(analysisId, analysisFeedback, cancellationToken);
        }

        /// <summary>
        /// Sets the possible <paramref name="answers"/> for a given intention
        /// </summary>
        /// <param name="intentionId">Intention identifier.</param>
        /// <param name="answers">Answers.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        public async Task SetAnswersAsync(string intentionId, IEnumerable<Answer> answers, CancellationToken cancellationToken = default)
        {
            await _aiExtension.SetAnswersAsync(intentionId, answers, cancellationToken);
        }

        /// <summary>
        /// Sets an <paramref name="entity"/>
        /// </summary>
        /// <param name="entity">Entity.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        public async Task<Entity> SetEntityAsync(Entity entity, CancellationToken cancellationToken = default)
        {
            return await _aiExtension.SetEntityAsync(entity, cancellationToken);
        }

        /// <summary>
        /// Sets an <paramref name="intention"/>
        /// </summary>
        /// <param name="intention">Intention.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        public async Task<Intention> SetIntentionAsync(Intention intention, CancellationToken cancellationToken = default)
        {
            return await _aiExtension.SetIntentionAsync(intention, cancellationToken);
        }

        /// <summary>
        /// Sets a list of <paramref name="questions"/> for a given intention
        /// </summary>
        /// <param name="intentionId">Intention identifier.</param>
        /// <param name="questions">Questions.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        public async Task SetQuestionsAsync(string intentionId, IEnumerable<Question> questions, CancellationToken cancellationToken = default)
        {
            await _aiExtension.SetQuestionsAsync(intentionId, questions, cancellationToken);
        }

        /// <summary>
        /// Trains the currently published model on the bot
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        public async Task TrainModelAsync(CancellationToken cancellationToken = default)
        {
            await _aiExtension.TrainModelAsync(cancellationToken);
        }
    }
}
