namespace StudyGroup.Api.Common.Dtos.Main
{
    public class MessageDto
    {
        public long Id { get; set; }
        public Guid ChatId { get; set; }
        public Guid FromUserId { get; set; }
        public string FromUsername { get; set; } = string.Empty;
        public string? FromUserImageUrl { get; set; }
        public string MessageText { get; set; } = string.Empty;
        public long? ReplyToMessageId { get; set; }
        public string? ReplyToMessageText { get; set; }
        public string StatusName { get; set; } = string.Empty;
        public List<MessageReadReceiptDto> ReadReceipts { get; set; } = [];
        public DateTimeOffset CreatedDateTime { get; set; }
    }
}
