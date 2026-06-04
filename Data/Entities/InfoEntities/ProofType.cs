using System.ComponentModel.DataAnnotations.Schema;
using StudyGroup.Api.Data.Entities.BaseEntities;

namespace StudyGroup.Api.Data.Entities.InfoEntities;

[Table("info_proof_type", Schema = "info")]
public class ProofType : BaseInfoEntity
{
}