using System.ComponentModel.DataAnnotations.Schema;
using StudyGroup.Api.Data.Entities.BaseEntities;

namespace StudyGroup.Api.Data.Entities.InfoEntities;

[Table("info_join_policy", Schema = "info")]
public class JoinPolicy : BaseInfoEntity
{
}