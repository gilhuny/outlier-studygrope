using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using StudyGroup.Api.Data.Entities.BaseEntities;

namespace StudyGroup.Api.Data.Entities.InfoEntities;

[Table("info_table", Schema = "info")]
public class InfoTable : BaseInfoEntity
{
    [Required]
    [Column("table_name")]
    [MaxLength(200)]
    public string TableName { get; set; } = string.Empty!;
}