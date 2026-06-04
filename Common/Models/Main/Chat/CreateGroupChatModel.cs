using System.ComponentModel.DataAnnotations;

namespace StudyGroup.Api.Common.Models.Main.Chat
{
    public record class CreateGroupChatModel
    {
        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        [Required]
        public Guid GroupId { get; set; }
    }
}
