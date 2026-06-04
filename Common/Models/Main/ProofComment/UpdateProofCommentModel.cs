using System.ComponentModel.DataAnnotations;

namespace StudyGroup.Api.Common.Models.Main.ProofComment
{
    public record class UpdateProofCommentModel
    {
        [Required]
        [MaxLength(1000)]
        public string CommentText { get; set; } = string.Empty;
    }
}
