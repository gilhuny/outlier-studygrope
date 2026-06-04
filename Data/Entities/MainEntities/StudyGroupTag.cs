using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using StudyGroup.Api.Data.Entities.BaseEntities;
using StudyGroup.Api.Data.Entities.InfoEntities;

namespace StudyGroup.Api.Data.Entities.MainEntities;

[Table("study_group_tags")]
[Index(nameof(GroupId), Name = "ix_study_group_tags_group_id")]
[Index(nameof(TagCode), Name = "ix_study_group_tags_tag_code")]
public class StudyGroupTag : BaseCommonEntity
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
    [Column("tag_code")]
    public int TagCode { get; set; }
    [ForeignKey(nameof(TagCode))]
    public virtual Tag? Tag { get; set; }
}