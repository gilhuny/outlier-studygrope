using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using StudyGroup.Api.Data.Entities.BaseEntities;

namespace StudyGroup.Api.Data.Entities.InfoEntities;

[Table("info_state", Schema = "info")]
public class State : BaseCommonEntity
{
    [Required]
    [Column("id")]
    public int Id { get; set; }

    [Required]
    [Column("code")]
    public int Code { get; set; }

    [Required]
    [Column("short_name")]
    [MaxLength(15)]
    public string ShortName { get; set; } = string.Empty!;

    [Required]
    [Column("full_name")]
    [MaxLength(200)]
    public string FullName { get; set; } = string.Empty!;
}