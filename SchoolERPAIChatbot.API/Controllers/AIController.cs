using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SchoolERPAIChatbot.API.Models.Requests;
using SchoolERPAIChatbot.API.Models.Responses;
using SchoolERPAIChatbot.API.Services.Interfaces;

namespace SchoolERPAIChatbot.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AIController : ControllerBase
    {
        private readonly IAIService _aiService;
        private readonly ILogger<AIController> _logger;
        private readonly IEmbeddingService _embeddingService;
        private readonly IVectorStoreService _vectorStoreService;

        public AIController(IAIService aiService, ILogger<AIController> logger, 
            IEmbeddingService embeddingService, IVectorStoreService vectorStoreService)
        {
            _aiService = aiService;
            _logger = logger;
            _embeddingService = embeddingService;
            _vectorStoreService = vectorStoreService;
        }

        [HttpPost("chat")]
        [ProducesResponseType(typeof(ChatResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Chat([FromBody] ChatRequest request,
            CancellationToken cancellationToken)
        {
            try
            {
                string answer = await _aiService.GetChatResponseAsync(request.Message,
                        cancellationToken);

                var response = new ChatResponse
                {
                    Answer = answer
                };

                return Ok(response);
            }
            catch (OperationCanceledException)
                when (cancellationToken.IsCancellationRequested)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error occurred while processing AI chat request.");

                return Problem(
                    title: "AI service error",
                    detail: "Unable to process your request.",
                    statusCode: StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPost("classify-question")]
        public async Task<IActionResult> ClassifyQuestion([FromBody] ChatRequest request,
                  CancellationToken cancellationToken)
        {
            try
            {
                AIIntentResponse result = await _aiService.ClassifyQuestionAsync(request.Message, cancellationToken);
                return Ok(result);
            }
            catch (OperationCanceledException)
                when (cancellationToken.IsCancellationRequested)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while classifying AI question.");

                return Problem(title: "AI classification failed",
                    statusCode: StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPost("generate-embedding")]
        public async Task<IActionResult> GenerateEmbedding([FromBody] EmbeddingRequest request,
                   CancellationToken cancellationToken)
        {
            try
            {
                float[] vector = await _embeddingService.GenerateEmbeddingAsync(
                        request.Text, cancellationToken);

                var response = new EmbeddingResponse
                {
                    Model = "text-embedding-3-small",
                    Dimensions = vector.Length,
                    Preview = vector.Take(10).ToArray()
                };

                return Ok(response);
            }
            catch (OperationCanceledException)
                when (cancellationToken.IsCancellationRequested)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error generating text embedding.");

                return Problem(
                    title: "Embedding generation failed",
                    statusCode: StatusCodes.Status500InternalServerError);
            }
        }

        // 1. Initialize Qdrant collection

        [HttpPost("vector/initialize")]
        public async Task<IActionResult> InitializeVectorCollection(
            CancellationToken cancellationToken)
        {
            await _vectorStoreService.CreateCollectionAsync(cancellationToken);

            return Ok(new
            {
                Message = "Vector collection created successfully."
            });
        }


        // 2. Generate embedding and store it in Qdrant

        [HttpPost("vector/store")]
        public async Task<IActionResult> StoreVector([FromBody] VectorStoreRequest request,
            CancellationToken cancellationToken)
        {
            float[] vector = await _embeddingService.GenerateEmbeddingAsync(
                    request.Text, cancellationToken);

            await _vectorStoreService.StoreVectorAsync(
                request.Text, vector, cancellationToken);

            return Ok(new
            {
                Message = "Document stored successfully.",
                Dimensions = vector.Length
            });
        }


        // 3. Generate query embedding and search Qdrant

        [HttpPost("vector/search")]
        public async Task<IActionResult> SearchVector([FromBody] VectorSearchRequest request,
            CancellationToken cancellationToken)
        {
            float[] queryVector = await _embeddingService.GenerateEmbeddingAsync(
                    request.Query, cancellationToken);

            List<string> results = await _vectorStoreService.SearchSimilarAsync(
                    queryVector, request.TopK, cancellationToken);

            return Ok(new VectorSearchResponse
            {
                Query = request.Query,
                ResultCount = results.Count,
                Results = results
            });
        }
    }
}
