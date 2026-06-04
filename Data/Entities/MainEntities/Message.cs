using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using StudyGroup.Api.Data.Entities.BaseEntities;
using StudyGroup.Api.Data.Entities.InfoEntities;

namespace StudyGroup.Api.Data.Entities.MainEntities;

[Table("messages")]
[Index(nameof(ChatId), Name = "ix_messages_chat_id")]
[Index(nameof(FromUserId), Name = "ix_messages_from_user_id")]
[Index(nameof(StatusCode), Name = "ix_messages_status_code")]
[Index(nameof(ReplyToMessageId), Name = "ix_messages_reply_to")]
[Index(nameof(ChatId), nameof(CreatedDateTime), Name = "ix_messages_chat_created")]
[Index(nameof(ChatId), nameof(StatusCode), Name = "ix_messages_chat_status")]
public class Message : BaseCommonEntity
{
    [Key]
    [Required]
    [Column("id")]
    public long Id { get; set; }

    [Required]
    [Column("chat_id")]
    public Guid ChatId { get; set; }
    [ForeignKey(nameof(ChatId))]
    public virtual Chat? Chat { get; set; }

    [Required]
    [Column("from_user_id")]
    public Guid FromUserId { get; set; }
    [ForeignKey(nameof(FromUserId))]
    public virtual User? FromUser { get; set; }

    [Required]
    [Column("message_text")]
    [MaxLength(4000)]
    public string MessageText { get; set; } = string.Empty;

    [Column("reply_to_message_id")]
    public long? ReplyToMessageId { get; set; }
    [ForeignKey(nameof(ReplyToMessageId))]
    public virtual Message? ReplyToMessage { get; set; }

    [Column("read_at")]
    public DateTimeOffset? ReadAt { get; set; }

    [Required]
    [Column("status_code")]
    public int StatusCode { get; set; }
    [ForeignKey(nameof(StatusCode))]
    public virtual Status? Status { get; set; }

    [InverseProperty(nameof(MessageReadReceipt.Message))]
    public virtual List<MessageReadReceipt>? ReadReceipts { get; set; }
}