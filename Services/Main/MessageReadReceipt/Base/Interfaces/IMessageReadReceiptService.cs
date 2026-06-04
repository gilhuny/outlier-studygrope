using StatusGeneric;
using StudyGroup.Api.Common.Dtos.Main;
using StudyGroup.Api.Common.FilterOptions;
using StudyGroup.Api.Common.Models.Shared;

namespace StudyGroup.Api.Services.Main.MessageReadReceipt.Base.Interfaces
{
    public interface IMessageReadReceiptService : IStatusGeneric
    {
        Task<PaginationModel<MessageReadReceiptDto>> GetAllAsync(MessageReadReceiptFilterOptions filterOptions);
        Task<string?> MarkAsReadAsync(long messageId);
        Task<string?> MarkChatAsReadAsync(Guid chatId);
    }
}
