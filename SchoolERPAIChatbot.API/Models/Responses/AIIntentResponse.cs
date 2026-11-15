namespace SchoolERPAIChatbot.API.Models.Responses
{
    public class AIIntentResponse
    {
        public string Intent { get; set; } = string.Empty;

        public string Category { get; set; } = string.Empty;

        public double Confidence { get; set; }

        public bool RequiresDatabaseLookup { get; set; }

        public string Reason { get; set; } = string.Empty;
    }
}
