using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using StudyGroup.Api.Data.Entities.BaseEntities;
using StudyGroup.Api.Data.Entities.InfoEntities;

namespace StudyGroup.Api.Data.Entities.MainEntities;

[Table("daily_proofs")]
[Index(nameof(GroupId), Name = "ix_daily_proofs_group_id")]
[Index(nameof(UserId), Name = "ix_daily_proofs_user_id")]
[Index(nameof(ProofDate), Name = "ix_daily_proofs_proof_date")]
[Index(nameof(StatusCode), Name = "ix_daily_proofs_status_code")]
[Index(nameof(GroupId), nameof(ProofDate), Name = "ix_daily_proofs_group_date")]
public class DailyProof : BaseCommonEntity
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
    [Column("proof_date")]
    public DateOnly ProofDate { get; set; }

    [Required]
    [Column("proof_type_code")]
    public int ProofTypeCode { get; set; }
    [ForeignKey(nameof(ProofTypeCode))]
    public virtual ProofType? ProofType { get; set; }

    [Column("text_content")]
    [MaxLength(4000)]
    public string? TextContent { get; set; }

    [Required]
    [Column("status_code")]
    public int StatusCode { get; set; }
    [ForeignKey(nameof(StatusCode))]
    public virtual Status? Status { get; set; }

    [InverseProperty(nameof(ProofContent.DailyProof))]
    public virtual List<ProofContent>? ProofContents { get; set; }

    [InverseProperty(nameof(ProofRating.DailyProof))]
    public virtual List<ProofRating>? Ratings { get; set; }

    [InverseProperty(nameof(ProofComment.DailyProof))]
    public virtual List<ProofComment>? Comments { get; set; }
}