using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using StudyGroup.Api.Data.Entities.BaseEntities;
using StudyGroup.Api.Data.Entities.InfoEntities;

namespace StudyGroup.Api.Data.Entities.MainEntities;

[Table("users")]
    [Index(nameof(RoleCode), Name = "ix_users_role_code")]
    [Index(nameof(StateCode), Name = "ix_users_status_code")]
    [Index(nameof(ImgId), Name = "ix_users_img_id")]
    [Index(nameof(Username), Name = "ix_users_username")]
    [Index(nameof(Email), Name = "ix_users_email")]
    public class User : BaseCommonEntity
    {
        [Key]
        [Required]
        [Column("id")]
        public Guid Id { get; set; }

        [Required]
        [Column("username")]
        [MaxLength(50)]
        public string Username { get; set; } = string.Empty;

        [Required]
        [Column("email")]
        [MaxLength(200)]
        public string Email { get; set; } = string.Empty;

        [Required]
        [Column("password_hash")]
        [MaxLength(500)]
        public string PasswordHash { get; set; } = string.Empty;

        [Required]
        [Column("first_name")]
        [MaxLength(100)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [Column("last_name")]
        [MaxLength(100)]
        public string LastName { get; set; } = string.Empty;

        [Column("bio")]
        [MaxLength(1000)]
        public string? Bio { get; set; }

        [Column("img_id")]
        public long? ImgId { get; set; }
        [ForeignKey(nameof(ImgId))]
        public virtual Content? Img { get; set; }

        [Required]
        [Column("role_code")]
        public int RoleCode { get; set; }
        [ForeignKey(nameof(RoleCode))]
        public virtual Role? Role { get; set; }

        [Required]
        [Column("state_code")]  // ← state_code not status_code
        public int StateCode { get; set; }
        [ForeignKey(nameof(StateCode))]
        public virtual State? State { get; set; }

        [Column("refresh_token")]
        public string? RefreshToken { get; set; }

        [Required]
        [Column("refresh_token_expiry_time")]
        public DateTimeOffset RefreshTokenExpireTime { get; set; }

        [InverseProperty(nameof(UserChat.User))]
        public virtual List<UserChat>? UserChats { get; set; }

        [InverseProperty(nameof(StudyGroupEntity.Creator))]
        public virtual List<StudyGroupEntity>? CreatedGroups { get; set; }

        [InverseProperty(nameof(GroupMember.User))]
        public virtual List<GroupMember>? GroupMemberships { get; set; }

        [InverseProperty(nameof(Message.FromUser))]
        public virtual List<Message>? Messages { get; set; }
    }