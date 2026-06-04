using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using StudyGroup.Api.Data.Entities.BaseEntities;

namespace StudyGroup.Api.Data.Entities.MainEntities;

[Table("message_read_receipts")]
[Index(nameof(MessageId), Name = "ix_message_read_receipts_message_id")]
[Index(nameof(UserId), Name = "ix_message_read_receipts_user_id")]
public class MessageReadReceipt : BaseCommonEntity
{
    [Key]
    [Required]
    [Column("id")]
    public long Id { get; set; }

    [Required]
    [Column("message_id")]
    public long MessageId { get; set; }
    [ForeignKey(nameof(MessageId))]
    public virtual Message? Message { get; set; }

    [Required]
    [Column("user_id")]
    public Guid UserId { get; set; }
    [ForeignKey(nameof(UserId))]
    public virtual User? User { get; set; }

    [Required]
    [Column("read_at")]
    public DateTimeOffset ReadAt { get; set; } = DateTimeOffset.UtcNow;
}