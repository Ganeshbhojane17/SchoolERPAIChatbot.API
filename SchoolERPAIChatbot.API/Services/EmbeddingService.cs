using OpenAI.Embeddings;
using SchoolERPAIChatbot.API.Services.Interfaces;

namespace SchoolERPAIChatbot.API.Services
{
    public class EmbeddingService : IEmbeddingService
    {
        private readonly EmbeddingClient _embeddingClient;

        public EmbeddingService(EmbeddingClient embeddingClient)
        {
            _embeddingClient = embeddingClient;
        }

        public async Task<float[]> GenerateEmbeddingAsync(string text, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                throw new ArgumentException("Text cannot be empty.", nameof(text));
            }

            OpenAIEmbedding embedding = await _embeddingClient.GenerateEmbeddingAsync(text,
                    cancellationToken: cancellationToken);

            ReadOnlyMemory<float> vector = embedding.ToFloats();

            return vector.ToArray();
        }
    }
}
