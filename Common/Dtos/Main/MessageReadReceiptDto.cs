namespace StudyGroup.Api.Common.Dtos.Main
{
    public class MessageReadReceiptDto
    {
        public long Id { get; set; }
        public long MessageId { get; set; }
        public Guid UserId { get; set; }
        public string Username { get; set; } = string.Empty;
        public string? UserImageUrl { get; set; }
        public DateTimeOffset ReadAt { get; set; }
    }
}
