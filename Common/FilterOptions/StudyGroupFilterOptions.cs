namespace StudyGroup.Api.Common.FilterOptions
{
    public class StudyGroupFilterOptions : BaseFilterOptions
    {
        public int? CategoryCode { get; set; }
        public int? PrivacyCode { get; set; }
        public int? JoinPolicyCode { get; set; }
        public int? StatusCode { get; set; }
        public int? TagCode { get; set; }
        public Guid? CreatorId { get; set; }
        public string? Name { get; set; }
    }
}
