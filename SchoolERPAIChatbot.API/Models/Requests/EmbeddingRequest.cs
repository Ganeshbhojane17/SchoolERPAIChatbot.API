using System.ComponentModel.DataAnnotations;

namespace SchoolERPAIChatbot.API.Models.Requests
{
    public class EmbeddingRequest
    {
        [Required]
        [StringLength(20000, MinimumLength = 1)]
        public string Text { get; set; } = string.Empty;
    }
}
