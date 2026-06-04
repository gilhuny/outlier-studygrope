namespace StudyGroup.Api.Common.FilterOptions
{
    public class MessageReadReceiptFilterOptions : BaseFilterOptions
    {
        public long? MessageId { get; set; }
        public Guid? UserId { get; set; }
    }
}
