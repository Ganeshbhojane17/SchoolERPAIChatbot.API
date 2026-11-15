using System.ComponentModel.DataAnnotations;

namespace SchoolERPAIChatbot.API.Models.Requests
{
    public class VectorSearchRequest
    {
        [Required]
        [StringLength(20000, MinimumLength = 1)]
        public string Query { get; set; } = string.Empty;

        [Range(1, 20)]
        public int TopK { get; set; } = 5;
    }
}
