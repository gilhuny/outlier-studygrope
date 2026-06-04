namespace StudyGroup.Api.Common.FilterOptions
{
    public class UserChatFilterOptions : BaseFilterOptions
    {
        public Guid? ChatId { get; set; }
        public Guid? UserId { get; set; }
        public int? StatusCode { get; set; }
    }
}
