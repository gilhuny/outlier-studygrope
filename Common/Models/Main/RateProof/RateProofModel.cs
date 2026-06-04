using System.ComponentModel.DataAnnotations;

namespace StudyGroup.Api.Common.Models.Main.RateProof
{
    public record class RateProofModel
    {
        [Required]
        [Range(1, 5)]
        public short Rating { get; set; }
    }
}
