using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using StudyGroup.Api.Data.Entities.BaseEntities;

namespace StudyGroup.Api.Data.Entities.MainEntities;

[Table("proof_ratings")]
[Index(nameof(ProofId), Name = "ix_proof_ratings_proof_id")]
[Index(nameof(RatedByUserId), Name = "ix_proof_ratings_rated_by")]
public class ProofRating : BaseCommonEntity
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
    [Column("rated_by_user_id")]
    public Guid RatedByUserId { get; set; }
    [ForeignKey(nameof(RatedByUserId))]
    public virtual User? RatedBy { get; set; }

    [Required]
    [Column("rating")]
    public short Rating { get; set; }
}