using StatusGeneric;
using StudyGroup.Api.Common.Dtos.Auth;
using StudyGroup.Api.Common.FilterOptions;
using StudyGroup.Api.Common.Models.Shared;
using StudyGroup.Api.Common.Models.User;

namespace StudyGroup.Api.Services.Admin.Users.Interfaces
{
    public interface IAdminUserService : IStatusGeneric
    {
        Task<PaginationModel<UserDtoForAdmin>> GetAllAsync(AdminUserFilterOptions filterOptions);
        Task<UserDtoForAdmin?> GetByUsernameAsync(string username);
        Task<string?> ActivateAsync(Guid userId);
        Task<string?> DeactivateAsync(Guid userId);
        Task<string?> UpdateAsync(Guid userId, UpdateUserModelForAdmin model);
        Task<string?> UpdateUserImageAsync(Guid userId, IFormFile img);
        Task<string?> DeleteAsync(Guid userId);
    }
}
