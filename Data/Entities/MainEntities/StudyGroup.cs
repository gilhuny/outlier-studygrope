using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using StudyGroup.Api.Data.Entities.BaseEntities;
using StudyGroup.Api.Data.Entities.InfoEntities;

namespace StudyGroup.Api.Data.Entities.MainEntities;

    [Table("study_groups")]
    [Index(nameof(CreatorId), Name = "ix_study_groups_creator_id")]
    [Index(nameof(StatusCode), Name = "ix_study_groups_status_code")]
    [Index(nameof(CategoryCode), Name = "ix_study_groups_category_code")]
    [Index(nameof(PrivacyCode), Name = "ix_study_groups_privacy_code")]
    [Index(nameof(CreatedDateTime), Name = "ix_study_groups_created_date_time")]
    public class StudyGroupEntity : BaseCommonEntity
    {
        [Key]
        [Required]
        [Column("id")]
        public Guid Id { get; set; }

        [Required]
        [Column("name")]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [Column("description")]
        [MaxLength(4000)]
        public string Description { get; set; } = string.Empty;

        [Required]
        [Column("creator_id")]
        public Guid CreatorId { get; set; }
        [ForeignKey(nameof(CreatorId))]
        public virtual User? Creator { get; set; }

        [Column("cover_img_id")]
        public long? CoverImgId { get; set; }
        [ForeignKey(nameof(CoverImgId))]
        public virtual Content? CoverImg { get; set; }

        [Required]
        [Column("category_code")]
        public int CategoryCode { get; set; }
        [ForeignKey(nameof(CategoryCode))]
        public virtual Category? Category { get; set; }

        [Required]
        [Column("privacy_code")]
        public int PrivacyCode { get; set; }
        [ForeignKey(nameof(PrivacyCode))]
        public virtual GroupPrivacy? Privacy { get; set; }

        [Required]
        [Column("join_policy_code")]
        public int JoinPolicyCode { get; set; }
        [ForeignKey(nameof(JoinPolicyCode))]
        public virtual JoinPolicy? JoinPolicy { get; set; }

        [Required]
        [Column("status_code")]
        public int StatusCode { get; set; }
        [ForeignKey(nameof(StatusCode))]
        public virtual Status? Status { get; set; }

        [Column("max_members")]
        public int? MaxMembers { get; set; }

        [Required]
        [Column("max_miss_days")]
        public int MaxMissDays { get; set; } = 3;

        [Required]
        [Column("start_date")]
        public DateOnly StartDate { get; set; }

        [Column("end_date")]
        public DateOnly? EndDate { get; set; }

        [Column("join_deadline")]
        public DateOnly? JoinDeadline { get; set; }

        [Required]
        [Column("upload_deadline_hour")]
        public int UploadDeadlineHour { get; set; } = 23;

        [InverseProperty(nameof(GroupMember.StudyGroup))]
        public virtual List<GroupMember>? Members { get; set; }

        [InverseProperty(nameof(StudyGroupTag.StudyGroup))]
        public virtual List<StudyGroupTag>? Tags { get; set; }

        [InverseProperty(nameof(GroupInvitation.StudyGroup))]
        public virtual List<GroupInvitation>? Invitations { get; set; }

        [InverseProperty(nameof(DailyProof.StudyGroup))]
        public virtual List<DailyProof>? DailyProofs { get; set; }
    }