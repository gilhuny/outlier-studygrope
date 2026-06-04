namespace StudyGroup.Api.Common.Dtos.Main
{
    public class UserChatDto
    {
        public long Id { get; set; }
        public Guid ChatId { get; set; }
        public Guid UserId { get; set; }
        public string Username { get; set; } = string.Empty;
        public string? UserImageUrl { get; set; }
        public string StatusName { get; set; } = string.Empty;
        public DateTimeOffset CreatedDateTime { get; set; }
    }
}
