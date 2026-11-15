using OpenAI.Responses;
using SchoolERPAIChatbot.API.Models.Responses;
using SchoolERPAIChatbot.API.Services.Interfaces;
using SchoolERPAIChatbot.API.Services.Prompts;
using SchoolERPAIChatbot.API.Services.Schemas;
using System.Text.Json;

namespace SchoolERPAIChatbot.API.Services
{
    public class AIService : IAIService
    {
        #pragma warning disable OPENAI001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
        private readonly ResponsesClient _client;
        private readonly IConfiguration _configuration;
        public AIService(ResponsesClient client, IConfiguration configuration)
        {
            _client = client;
            _configuration = configuration;
        }
        public async Task<string> GetChatResponseAsync(string message, 
            CancellationToken cancellationToken = default)
        {
            string model = _configuration["OpenAI:Model"] ?? throw new InvalidOperationException(
                    "OpenAI model is not configured.");

            var options = new CreateResponseOptions
            {
                Model = model,
                Instructions = PromptTemplates.SchoolERPAssistant
            };
            options.InputItems.Add(ResponseItem.CreateUserMessageItem(message));

            ResponseResult response = await _client.CreateResponseAsync(options, cancellationToken);
            return response.GetOutputText();
        }

        public async Task<AIIntentResponse> ClassifyQuestionAsync(string message,
                   CancellationToken cancellationToken)
        {
            string model = _configuration["OpenAI:Model"]
                ?? throw new InvalidOperationException("OpenAI model is not configured.");

            var options = new CreateResponseOptions
            {
                Model = model,

                Instructions = """
            You are a School ERP question classifier.

            Analyze the user's question and identify:
            1. User intent
            2. School ERP category
            3. Whether database lookup may be required
            4. A short reason

            Follow the supplied JSON schema exactly.

            Never assume that a database lookup has already
            been performed.
            """
            };

            options.InputItems.Add(
                ResponseItem.CreateUserMessageItem(message));

            options.TextOptions = new ResponseTextOptions
            {
                TextFormat = ResponseTextFormat.CreateJsonSchemaFormat(
                    "school_erp_intent",
                    AIIntentSchema.GetSchema(),
                    jsonSchemaIsStrict: true)
            };

            ResponseResult response = await _client.CreateResponseAsync(options, cancellationToken);

            string json = response.GetOutputText();

            var result = JsonSerializer.Deserialize<AIIntentResponse>(
                json,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

            if (result == null)
            {
                throw new InvalidOperationException(
                    "Unable to deserialize AI response.");
            }

            if (result.Confidence is < 0 or > 1)
            {
                throw new InvalidOperationException(
                    "Invalid confidence value.");
            }

            return result;
        }
    }
}
