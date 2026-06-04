namespace StudyGroup.Api.Common.FilterOptions
{
    public class AdminUserFilterOptions : BaseFilterOptions
    {
        public int? StateCode { get; set; }
        public int? RoleCode { get; set; }
        public string? Username { get; set; }
    }
}
