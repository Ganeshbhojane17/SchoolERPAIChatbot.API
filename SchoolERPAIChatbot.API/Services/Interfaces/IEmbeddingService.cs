namespace SchoolERPAIChatbot.API.Services.Interfaces
{
    public interface IEmbeddingService
    {
        Task<float[]> GenerateEmbeddingAsync(string text, CancellationToken cancellationToken);
    }
}
