using StatusGeneric;
using StudyGroup.Api.Common.Dtos.Main;
using StudyGroup.Api.Common.FilterOptions;
using StudyGroup.Api.Common.Models.Shared;

namespace StudyGroup.Api.Services.Main.UserChat.Base.Interfaces
{
    public interface IUserChatService : IStatusGeneric
    {
        Task<PaginationModel<UserChatDto>> GetParticipantsAsync(Guid chatId, UserChatFilterOptions filterOptions);
        Task<string?> AddParticipantAsync(Guid chatId, Guid targetUserId);
        Task<string?> RemoveParticipantAsync(Guid chatId, Guid targetUserId);
        Task<string?> LeaveAsync(Guid chatId);
    }
}
