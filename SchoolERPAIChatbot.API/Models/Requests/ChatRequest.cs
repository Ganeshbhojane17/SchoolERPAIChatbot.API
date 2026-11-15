using System.ComponentModel.DataAnnotations;

namespace SchoolERPAIChatbot.API.Models.Requests
{
    public class ChatRequest
    {
        [Required]
        [StringLength(4000, MinimumLength = 1)]
        public string Message { get; set; } = string.Empty;
    }
}
