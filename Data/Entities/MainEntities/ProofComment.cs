using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using StudyGroup.Api.Data.Entities.BaseEntities;
using StudyGroup.Api.Data.Entities.InfoEntities;

namespace StudyGroup.Api.Data.Entities.MainEntities;

[Table("proof_comments")]
[Index(nameof(ProofId), Name = "ix_proof_comments_proof_id")]
[Index(nameof(UserId), Name = "ix_proof_comments_user_id")]
[Index(nameof(StatusCode), Name = "ix_proof_comments_status_code")]
public class ProofComment : BaseCommonEntity
{
    [Key]
    [Required]
    [Column("id")]
    public long Id { get; set; }

    [Required]
    [Column("proof_id")]
    public long ProofId { get; set; }
    [ForeignKey(nameof(ProofId))]
    public virtual DailyProof? DailyProof { get; set; }

    [Required]
    [Column("user_id")]
    public Guid UserId { get; set; }
    [ForeignKey(nameof(UserId))]
    public virtual User? User { get; set; }

    [Required]
    [Column("comment_text")]
    [MaxLength(1000)]
    public string CommentText { get; set; } = string.Empty;

    [Required]
    [Column("status_code")]
    public int StatusCode { get; set; }
    [ForeignKey(nameof(StatusCode))]
    public virtual Status? Status { get; set; }
}