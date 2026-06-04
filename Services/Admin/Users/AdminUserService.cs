using Mapster;
using Microsoft.EntityFrameworkCore;
using StatusGeneric;
using StudyGroup.Api.Common.Constants;
using StudyGroup.Api.Common.Dtos.Auth;
using StudyGroup.Api.Common.Extensions;
using StudyGroup.Api.Common.FilterOptions;
using StudyGroup.Api.Common.Models.Shared;
using StudyGroup.Api.Common.Models.User;
using StudyGroup.Api.Data.Entities.MainEntities;
using StudyGroup.Api.Data.Repositories.Interfaces;
using StudyGroup.Api.Services.Admin.Users.Interfaces;
using StudyGroup.Api.Services.Admin.Users.QueryObjects;
using StudyGroup.Api.Services.Common.Interfaces;
using StudyGroup.Api.Services.Public.Content.Interfaces;

namespace StudyGroup.Api.Services.Admin.Users
{
    public class AdminUserService(IUnitOfWork unitOfWork, IContentService contentService, IUserHelper userHelper)
        : StatusGenericHandler, IAdminUserService
    {
        public async Task<PaginationModel<UserDtoForAdmin>> GetAllAsync(AdminUserFilterOptions filterOptions)
        {
            var query = unitOfWork.UserRepository()
                .GetAll(u => u.Role!, u => u.State!, u => u.Img!)
                .AsNoTracking();

            var config = GetCustomConfig();

            return query
                .ApplyFilter(filterOptions)
                .MapToDtos<User, UserDtoForAdmin>(config)
                .ToPaginationModel(filterOptions.Page, filterOptions.PageSize);
        }

        public async Task<UserDtoForAdmin?> GetByUsernameAsync(string username)
        {
            var user = await unitOfWork.UserRepository()
                .GetAll(u => u.Role!, u => u.State!, u => u.Img!)
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Username == username);

            if (user is null)
            {
                AddError("User not found.");
                return null;
            }

            return user.MapToDto<User, UserDtoForAdmin>(GetCustomConfig());
        }

        public async Task<string?> ActivateAsync(Guid userId) =>
            await UpdateStateAsync(userId, StateConstants.Active, "User activated successfully.");

        public async Task<string?> DeactivateAsync(Guid userId) =>
            await UpdateStateAsync(userId, StateConstants.Passive, "User deactivated successfully.");

        public async Task<string?> UpdateAsync(Guid userId, UpdateUserModelForAdmin model)
        {
            var (user, isExist) = await GetUserByIdAsync(userId);
            if (!isExist || user is null) return null;

            if (!ValidateRoleCode(model.RoleCode)) return null;

            var updated = model.MapForUpdate(user);
            updated!.ModifiedUserId = userHelper.GetUserId();
            updated.ModifiedDateTime = DateTime.UtcNow;

            await unitOfWork.UserRepository().Update(updated);
            await unitOfWork.SaveChanges();
            return "User updated successfully.";
        }

        public async Task<string?> UpdateUserImageAsync(Guid userId, IFormFile img)
        {
            var (user, isExist) = await GetUserByIdAsync(userId, includeImg: true);
            if (!isExist || user is null) return null;

            if (!user.ImgId.HasValue)
            {
                AddError("User does not have an existing image to update.");
                return null;
            }

            user.ImgId = await contentService.UpdateContentForImage(user.ImgId.Value, img);
            user.ModifiedUserId = userHelper.GetUserId();
            user.ModifiedDateTime = DateTime.UtcNow;

            await unitOfWork.UserRepository().Update(user);
            await unitOfWork.SaveChanges();
            return "User image updated successfully.";
        }

        public async Task<string?> DeleteAsync(Guid userId)
        {
            var (user, isExist) = await GetUserByIdAsync(userId);
            if (!isExist || user is null) return null;

            await unitOfWork.UserRepository().Delete(user);
            await unitOfWork.SaveChanges();
            return "User deleted successfully.";
        }

        // ── helpers ──────────────────────────────────────────────────────────────

        private async Task<string?> UpdateStateAsync(Guid userId, int targetStateCode, string message)
        {
            var (user, isExist) = await GetUserByIdAsync(userId);
            if (!isExist || user is null) return null;

            user.StateCode = targetStateCode;
            user.ModifiedUserId = userHelper.GetUserId();
            user.ModifiedDateTime = DateTime.UtcNow;

            await unitOfWork.UserRepository().Update(user);
            await unitOfWork.SaveChanges();
            return message;
        }

        private async Task<(User? user, bool isExist)> GetUserByIdAsync(Guid userId, bool includeImg = false)
        {
            var query = includeImg
                ? unitOfWork.UserRepository().GetAll(u => u.Role!, u => u.State!, u => u.Img!)
                : unitOfWork.UserRepository().GetAll();

            var user = await query.FirstOrDefaultAsync(u => u.Id == userId);
            if (user is null)
            {
                AddError("User not found.");
                return (null, false);
            }

            return (user, true);
        }

        private bool ValidateRoleCode(int? roleCode)
        {
            if (!roleCode.HasValue) return true;
            if (roleCode < 0)
            {
                AddError("RoleCode must be a positive integer.");
                return false;
            }

            if (roleCode != RoleConstants.UserRoleCode)
            {
                AddError($"Invalid RoleCode. Allowed value is {RoleConstants.UserRoleCode}.");
                return false;
            }

            return true;
        }

        private static TypeAdapterConfig GetCustomConfig()
        {
            var config = new TypeAdapterConfig();
            config.NewConfig<User, UserDtoForAdmin>()
                .Map(dest => dest.Role, src => src.Role!.FullName)
                .Map(dest => dest.StateName, src => src.State!.FullName)
                .Map(dest => dest.ImgUrl, src => src.Img != null ? src.Img.FileId.GetFileUrl() : null)
                .Map(dest => dest.ImgId, src => src.ImgId)
                .Map(dest => dest.RoleCode, src => src.RoleCode)
                .Map(dest => dest.StateCode, src => src.StateCode)
                .Map(dest => dest.RefreshTokenExpireTime, src => src.RefreshTokenExpireTime);
            return config;
        }
    }
}
