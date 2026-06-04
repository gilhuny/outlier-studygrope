using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using StudyGroup.Api.Data.Entities.BaseEntities;

namespace StudyGroup.Api.Data.Entities.MainEntities;

[Table("member_rankings")]
[Index(nameof(GroupId), Name = "ix_member_rankings_group_id")]
[Index(nameof(UserId), Name = "ix_member_rankings_user_id")]
[Index(nameof(GroupId), nameof(TotalScore), Name = "ix_member_rankings_total_score")]
public class MemberRanking : BaseCommonEntity
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
    [Column("total_score", TypeName = "numeric(10,2)")]
    public decimal TotalScore { get; set; } = 0;

    [Required]
    [Column("avg_rating", TypeName = "numeric(3,2)")]
    public decimal AvgRating { get; set; } = 0;

    [Required]
    [Column("upload_count")]
    public int UploadCount { get; set; } = 0;

    [Required]
    [Column("miss_count")]
    public int MissCount { get; set; } = 0;

    [Required]
    [Column("current_streak")]
    public int CurrentStreak { get; set; } = 0;

    [Required]
    [Column("longest_streak")]
    public int LongestStreak { get; set; } = 0;

    [Column("rank_position")]
    public int? RankPosition { get; set; }

    [Required]
    [Column("last_calculated_at")]
    public DateTimeOffset LastCalculatedAt { get; set; } = DateTimeOffset.UtcNow;
}