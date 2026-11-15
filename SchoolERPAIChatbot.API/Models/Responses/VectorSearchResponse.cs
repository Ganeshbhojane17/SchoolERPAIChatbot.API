namespace SchoolERPAIChatbot.API.Models.Responses
{
    public class VectorSearchResponse
    {
        public string Query { get; set; } = string.Empty;

        public int ResultCount { get; set; }

        public List<string> Results { get; set; } = [];
    }
}
