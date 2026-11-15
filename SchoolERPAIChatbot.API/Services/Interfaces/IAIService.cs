using SchoolERPAIChatbot.API.Models.Responses;

namespace SchoolERPAIChatbot.API.Services.Interfaces
{
    public interface IAIService
    {
        Task<string> GetChatResponseAsync(string message, 
            CancellationToken cancellationToken = default);
        Task<AIIntentResponse> ClassifyQuestionAsync(string message,
            CancellationToken cancellationToken);
    }
}
