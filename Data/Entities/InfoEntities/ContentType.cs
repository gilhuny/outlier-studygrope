using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using StudyGroup.Api.Data.Entities.BaseEntities;

namespace StudyGroup.Api.Data.Entities.InfoEntities;

[Table("info_content_type", Schema = "info")]
public class ContentType : BaseInfoEntity
{
    [Required]
    [Column("type_name")]
    [MaxLength(100)]
    public string TypeName { get; set; } = string.Empty!;
}