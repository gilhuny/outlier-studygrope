using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using StudyGroup.Api.Data.Entities.BaseEntities;
using StudyGroup.Api.Data.Entities.InfoEntities;

namespace StudyGroup.Api.Data.Entities.MainEntities;

[Table("chats")]
[Index(nameof(StatusCode), Name = "ix_chats_status_code")]
[Index(nameof(GroupId), Name = "ix_chats_group_id")]
[Index(nameof(ChatTypeCode), Name = "ix_chats_chat_type_code")]
public class Chat : BaseCommonEntity
{
    [Key]
    [Required]
    [Column("id")]
    public Guid Id { get; set; }

    [Required]
    [Column("chat_type_code")]
    public int ChatTypeCode { get; set; }
    [ForeignKey(nameof(ChatTypeCode))]
    public virtual ChatType? ChatType { get; set; }

    [Column("name")]
    [MaxLength(200)]
    public string? Name { get; set; }

    [Column("group_id")]
    public Guid? GroupId { get; set; }
    [ForeignKey(nameof(GroupId))]
    public virtual StudyGroupEntity? StudyGroup { get; set; }

    [Required]
    [Column("status_code")]
    public int StatusCode { get; set; }
    [ForeignKey(nameof(StatusCode))]
    public virtual Status? Status { get; set; }

    [InverseProperty(nameof(UserChat.Chat))]
    public virtual List<UserChat>? UserChats { get; set; }

    [InverseProperty(nameof(Message.Chat))]
    public virtual List<Message>? Messages { get; set; }
}