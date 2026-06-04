using System.ComponentModel.DataAnnotations;

namespace StudyGroup.Api.Common.Models.Message
{
    public record class UpdateMessageModel
    {
        [Required]
        [MaxLength(4000)]
        public string MessageText { get; set; } = string.Empty;
    }
}
