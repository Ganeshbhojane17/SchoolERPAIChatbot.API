namespace SchoolERPAIChatbot.API.Models.Responses
{
    public class EmbeddingResponse
    {
        public string Model { get; set; } = string.Empty;
        public int Dimensions { get; set; }
        public float[] Preview { get; set; } = [];
    }
}
