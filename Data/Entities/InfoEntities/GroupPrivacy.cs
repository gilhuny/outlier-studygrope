using System.ComponentModel.DataAnnotations.Schema;
using StudyGroup.Api.Data.Entities.BaseEntities;

namespace StudyGroup.Api.Data.Entities.InfoEntities;

[Table("info_group_privacy", Schema = "info")]
public class GroupPrivacy : BaseInfoEntity
{
}