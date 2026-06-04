using System.ComponentModel.DataAnnotations;

namespace StudyGroup.Api.Common.Models.Main.StudyGroup
{
    public record class CreateStudyGroupModel
    {
        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [MaxLength(4000)]
        public string Description { get; set; } = string.Empty;

        [Required]
        public int CategoryCode { get; set; }

        [Required]
        public int PrivacyCode { get; set; }

        [Required]
        public int JoinPolicyCode { get; set; }

        public int? MaxMembers { get; set; }
        public int MaxMissDays { get; set; } = 3;
        public int UploadDeadlineHour { get; set; } = 23;

        [Required]
        public DateOnly StartDate { get; set; }
        public DateOnly? EndDate { get; set; }
        public DateOnly? JoinDeadline { get; set; }

        public List<int>? TagCodes { get; set; }
        public IFormFile? CoverImage { get; set; }
    }

}
