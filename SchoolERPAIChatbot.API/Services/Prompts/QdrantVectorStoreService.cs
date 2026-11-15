
using Qdrant.Client;
using Qdrant.Client.Grpc;
using SchoolERPAIChatbot.API.Services.Interfaces;

namespace SchoolERPAIChatbot.API.Services
{
    public class QdrantVectorStoreService : IVectorStoreService
    {
        private readonly QdrantClient _client;
        private readonly IConfiguration _configuration;

        public QdrantVectorStoreService(QdrantClient client,
            IConfiguration configuration)
        {
            _client = client;
            _configuration = configuration;
        }

        private string CollectionName => _configuration["Qdrant:CollectionName"]
            ?? "schoolerp_policies";

        public async Task CreateCollectionAsync(CancellationToken cancellationToken)
        {
            int vectorSize =_configuration.GetValue<int>("Qdrant:VectorSize");

            await _client.CreateCollectionAsync(collectionName: CollectionName,
                vectorsConfig: new VectorParams
                {
                    Size = (ulong)vectorSize,
                    Distance = Distance.Cosine
                },
                cancellationToken: cancellationToken);
        }

        public async Task StoreVectorAsync(string text,float[] vector,
            CancellationToken cancellationToken)
        {
            var point = new PointStruct
            {
                Id = Guid.NewGuid(),
                Vectors = vector,
                Payload =
                {
                    ["text"] = text,
                    ["source"] = "manual-input",
                    ["category"] = "SchoolPolicy"
                }
            };

            await _client.UpsertAsync(
                collectionName: CollectionName, points: new List<PointStruct> { point },
                cancellationToken: cancellationToken);
        }

        public async Task<List<string>> SearchSimilarAsync(float[] queryVector,
            int limit, CancellationToken cancellationToken)
        {
            var results = await _client.SearchAsync(
                collectionName: CollectionName,
                vector: queryVector,
                limit: (ulong)limit,
                cancellationToken: cancellationToken);

            return results
                .Where(x => x.Payload.ContainsKey("text"))
                .Select(x => x.Payload["text"].StringValue)
                .ToList();
        }
    }
}
