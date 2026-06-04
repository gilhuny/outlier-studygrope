namespace StudyGroup.Api.Common.Dtos.Main
{
    public class ChatDto
    {
        public Guid Id { get; set; }
        public string ChatTypeName { get; set; } = string.Empty;
        public string? Name { get; set; }
        public Guid? GroupId { get; set; }
        public string? GroupName { get; set; }
        public string StatusName { get; set; } = string.Empty;
        public List<UserChatDto> Participants { get; set; } = [];
        public string? LastMessageText { get; set; }
        public DateTimeOffset? LastMessageAt { get; set; }
        public DateTime CreatedDateTime { get; set; }
    }

}
