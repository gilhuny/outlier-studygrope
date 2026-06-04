namespace StudyGroup.Api.Common.FilterOptions
{
    public class GroupInvitationFilterOptions : BaseFilterOptions
    {
        public Guid? GroupId { get; set; }
        public int? StatusCode { get; set; }
    }
}
