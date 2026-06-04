using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using StudyGroup.Api.Data.Entities.BaseEntities;
using StudyGroup.Api.Data.Entities.InfoEntities;

namespace StudyGroup.Api.Data.Entities.MainEntities;

[Table("group_invitations")]
[Index(nameof(GroupId), Name = "ix_group_invitations_group_id")]
[Index(nameof(InvitedUserId), Name = "ix_group_invitations_invited_user_id")]
[Index(nameof(StatusCode), Name = "ix_group_invitations_status_code")]
public class GroupInvitation : BaseCommonEntity
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
    [Column("invited_by_id")]
    public Guid InvitedById { get; set; }
    [ForeignKey(nameof(InvitedById))]
    public virtual User? InvitedBy { get; set; }

    [Required]
    [Column("invited_user_id")]
    public Guid InvitedUserId { get; set; }
    [ForeignKey(nameof(InvitedUserId))]
    public virtual User? InvitedUser { get; set; }

    [Required]
    [Column("status_code")]
    public int StatusCode { get; set; }
    [ForeignKey(nameof(StatusCode))]
    public virtual Status? Status { get; set; }

    [Column("message")]
    [MaxLength(500)]
    public string? Message { get; set; }

    [Column("responded_date_time")]
    public DateTimeOffset? RespondedDateTime { get; set; }
}