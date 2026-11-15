namespace SchoolERPAIChatbot.API.Services.Interfaces
{
    public interface IVectorStoreService
    {
        Task CreateCollectionAsync(CancellationToken cancellationToken);

        Task StoreVectorAsync(string text, float[] vector, CancellationToken cancellationToken);

        Task<List<string>> SearchSimilarAsync(float[] queryVector,
            int limit,CancellationToken cancellationToken);
    }
}
