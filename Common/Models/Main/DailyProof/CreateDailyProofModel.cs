using System.ComponentModel.DataAnnotations;

namespace StudyGroup.Api.Common.Models.Main.DailyProof
{
    public record class CreateDailyProofModel
    {
        [Required]
        public Guid GroupId { get; set; }
        [Required]
        public int ProofTypeCode { get; set; }
        [MaxLength(4000)]
        public string? TextContent { get; set; }
        public List<IFormFile>? Files { get; set; }
    }
}
