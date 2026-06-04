using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using StudyGroup.Api.Data.Entities.BaseEntities;
using StudyGroup.Api.Data.Entities.InfoEntities;

namespace StudyGroup.Api.Data.Entities.MainEntities;

[Table("contents")]
[Index(nameof(StatusCode), Name = "ix_contents_status_code")]
[Index(nameof(ContentTypeCode), Name = "ix_contents_content_type_code")]
public class Content : BaseCommonEntity
{
    [Key]
    [Required]
    [Column("id")]
    public long Id { get; set; }

    [Required]
    [Column("file_id")]
    public Guid FileId { get; set; }

    [Required]
    [Column("name")]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [Column("folder")]
    [MaxLength(200)]
    public string Folder { get; set; } = string.Empty;

    [Required]
    [Column("content_type_code")]
    public int ContentTypeCode { get; set; }
    [ForeignKey(nameof(ContentTypeCode))]
    public virtual ContentType? ContentType { get; set; }

    [Required]
    [Column("status_code")]
    public int StatusCode { get; set; }
    [ForeignKey(nameof(StatusCode))]
    public virtual Status? Status { get; set; }
}