namespace StudyGroup.Api.Common.FilterOptions
{
    public class MessageFilterOptions : BaseFilterOptions
    {
        public Guid? ChatId { get; set; }
        public Guid? FromUserId { get; set; }
        public int? StatusCode { get; set; }
    }
}
