namespace StudyGroup.Api.Common.FilterOptions
{
    public class ChatFilterOptions : BaseFilterOptions
    {
        public int? ChatTypeCode { get; set; }
        public Guid? GroupId { get; set; }
        public int? StatusCode { get; set; }
    }
}
