using System.ComponentModel.DataAnnotations;

namespace StudyGroup.Api.Common.Models.Message
{
    public record class SendMessageModel
    {
        [Required]
        public Guid ChatId { get; set; }

        [Required]
        [MaxLength(4000)]
        public string MessageText { get; set; } = string.Empty;

        public long? ReplyToMessageId { get; set; }
    }
}
