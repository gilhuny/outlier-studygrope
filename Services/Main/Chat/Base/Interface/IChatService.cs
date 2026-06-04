using StatusGeneric;
using StudyGroup.Api.Common.Dtos.Main;
using StudyGroup.Api.Common.FilterOptions;
using StudyGroup.Api.Common.Models.Main.Chat;
using StudyGroup.Api.Common.Models.Shared;

namespace StudyGroup.Api.Services.Main.Chat.Base.Interface
{
    public interface IChatService : IStatusGeneric
    {
        Task<PaginationModel<ChatDto>> GetMyChatsAsync(ChatFilterOptions filterOptions);
        Task<ChatDto?> GetByIdAsync(Guid id);
        Task<Guid?> CreateDirectChatAsync(Guid targetUserId);
        Task<Guid?> CreateGroupChatAsync(CreateGroupChatModel model);
        Task<string?> DeleteAsync(Guid id);
    }
}
