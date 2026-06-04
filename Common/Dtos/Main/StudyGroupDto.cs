namespace StudyGroup.Api.Common.Dtos.Main

{
    public class StudyGroupDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string CreatorUsername { get; set; } = string.Empty;
        public string? CoverImageUrl { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public string PrivacyName { get; set; } = string.Empty;
        public string JoinPolicyName { get; set; } = string.Empty;
        public string StatusName { get; set; } = string.Empty;
        public int? MaxMembers { get; set; }
        public int MaxMissDays { get; set; }
        public int UploadDeadlineHour { get; set; }
        public DateOnly StartDate { get; set; }
        public DateOnly? EndDate { get; set; }
        public DateOnly? JoinDeadline { get; set; }
        public int MemberCount { get; set; }
        public List<string> Tags { get; set; } = [];
        public DateTime CreatedDateTime { get; set; }
    }
}
