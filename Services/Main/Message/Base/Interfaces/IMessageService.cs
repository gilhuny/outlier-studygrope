using StatusGeneric;
using StudyGroup.Api.Common.Dtos.Main;
using StudyGroup.Api.Common.FilterOptions;
using StudyGroup.Api.Common.Models.Message;
using StudyGroup.Api.Common.Models.Shared;

namespace StudyGroup.Api.Services.Main.Message.Base.Interfaces
{
    public interface IMessageService : IStatusGeneric
    {
        Task<PaginationModel<MessageDto>> GetAllAsync(MessageFilterOptions filterOptions);
        Task<MessageDto?> GetByIdAsync(long id);
        Task<MessageDto?> SendAsync(SendMessageModel model);
        Task<string?> UpdateAsync(long id, UpdateMessageModel model);
        Task<string?> DeleteAsync(long id);
    }
}
