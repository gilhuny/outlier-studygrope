using System.ComponentModel.DataAnnotations;

namespace StudyGroup.Api.Common.Models.Main.StudyGroup
{
    public record class UpdateStudyGroupModel
    {
        [MaxLength(200)]
        public string? Name { get; set; }

        [MaxLength(4000)]
        public string? Description { get; set; }

        public int? CategoryCode { get; set; }
        public int? PrivacyCode { get; set; }
        public int? JoinPolicyCode { get; set; }
        public int? MaxMembers { get; set; }
        public int? MaxMissDays { get; set; }
        public int? UploadDeadlineHour { get; set; }
        public DateOnly? StartDate { get; set; }
        public DateOnly? EndDate { get; set; }
        public DateOnly? JoinDeadline { get; set; }
        public List<int>? TagCodes { get; set; }
    }
}
