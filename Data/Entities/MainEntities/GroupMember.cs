using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using StudyGroup.Api.Data.Entities.BaseEntities;
using StudyGroup.Api.Data.Entities.InfoEntities;

namespace StudyGroup.Api.Data.Entities.MainEntities;

[Table("group_members")]
[Index(nameof(GroupId), Name = "ix_group_members_group_id")]
[Index(nameof(UserId), Name = "ix_group_members_user_id")]
[Index(nameof(StatusCode), Name = "ix_group_members_status_code")]
[Index(nameof(GroupId), nameof(StatusCode), Name = "ix_group_members_group_status")]
public class GroupMember : BaseCommonEntity
{
    [Key]
    [Required]
    [Column("id")]
    public long Id { get; set; }

    [Required]
    [Column("group_id")]
    public Guid GroupId { get; set; }
    [ForeignKey(nameof(GroupId))]
    public virtual StudyGroupEntity? StudyGroup { get; set; }

    [Required]
    [Column("user_id")]
    public Guid UserId { get; set; }
    [ForeignKey(nameof(UserId))]
    public virtual User? User { get; set; }

    [Required]
    [Column("status_code")]
    public int StatusCode { get; set; }
    [ForeignKey(nameof(StatusCode))]
    public virtual Status? Status { get; set; }

    [Required]
    [Column("miss_count")]
    public int MissCount { get; set; } = 0;

    [Column("joined_date_time")]
    public DateTimeOffset JoinedDateTime { get; set; } = DateTimeOffset.UtcNow;

    [Column("left_date_time")]
    public DateTimeOffset? LeftDateTime { get; set; }
}