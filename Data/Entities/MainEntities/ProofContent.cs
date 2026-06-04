using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using StudyGroup.Api.Data.Entities.BaseEntities;

namespace StudyGroup.Api.Data.Entities.MainEntities;

[Table("proof_contents")]
[Index(nameof(ProofId), Name = "ix_proof_contents_proof_id")]
[Index(nameof(ContentId), Name = "ix_proof_contents_content_id")]
public class ProofContent : BaseCommonEntity
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
    [Column("content_id")]
    public long ContentId { get; set; }
    [ForeignKey(nameof(ContentId))]
    public virtual Content? Content { get; set; }
}