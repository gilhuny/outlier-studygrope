namespace StudyGroup.Api.Common.FilterOptions
{
    public class GroupMemberFilterOptions : BaseFilterOptions
    {
        public Guid? GroupId { get; set; }
        public int? StatusCode { get; set; }
        public string? Username { get; set; }
    }
}
