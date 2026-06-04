using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using StatusGeneric;
using StudyGroup.Api.Common.Constants;
using StudyGroup.Api.Common.Dtos.Auth;
using StudyGroup.Api.Common.Models.Auth;
using StudyGroup.Api.Common.Models.User;
using StudyGroup.Api.Data.Entities.MainEntities;
using StudyGroup.Api.Data.Repositories.Interfaces;
using StudyGroup.Api.Services.Auth.Interfaces;
using StudyGroup.Api.Services.Common.Interfaces;
using StudyGroup.Api.Services.Public.Content.Interfaces;
using Mapster;
using MapsterMapper;
using StudyGroup.Api.Common.Extensions;
using Microsoft.EntityFrameworkCore.Storage;

namespace StudyGroup.Api.Services.Auth
{
    public class AuthService(
        IUnitOfWork unitOfWork,
        IContentService contentService,
        JwtService jwtService,
        IUserHelper userHelper)
        : StatusGenericHandler, IAuthService
    {
        public async Task<UserDto?> RegisterAsync(RegisterModel model)
        {
            await using var transaction = unitOfWork.BeginTransaction();
            try
            {
                var usernameExists = await unitOfWork.UserRepository()
                    .GetAll()
                    .AnyAsync(u => u.Username == model.Username);
                if (usernameExists)
                {
                    AddError("Username is already taken.");
                    return null;
                }

                var emailExists = await unitOfWork.UserRepository()
                    .GetAll()
                    .AnyAsync(u => u.Email == model.Email);
                if (emailExists)
                {
                    AddError("Email is already taken.");
                    return null;
                }

                var userId = Guid.NewGuid();

                long? contentId = null;
                if (model.ImageFile is not null)
                {
                    contentId = await contentService.CreateContentForImage(model.ImageFile, "profile");
                    if (contentId.HasValue)
                        await unitOfWork.ContentRepository().GetAll()
                            .Where(c => c.Id == contentId)
                            .ExecuteUpdateAsync(c => c.SetProperty(x => x.CreatedUserId, userId));
                }

                var user = new User
                {
                    Id = userId,
                    Username = model.Username,
                    Email = model.Email,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Bio = model.Bio,
                    ImgId = contentId,
                    RoleCode = RoleConstants.UserRoleCode,
                    StateCode = StateConstants.Active,
                    CreatedUserId = userId,
                    RefreshTokenExpireTime = DateTimeOffset.UtcNow.AddHours(1)
                };
                user.PasswordHash = new PasswordHasher<User>().HashPassword(user, model.Password);

                await unitOfWork.UserRepository().Add(user);
                await unitOfWork.SaveChanges();
                await transaction.CommitAsync();

                var loaded = await unitOfWork.UserRepository()
                    .GetAll(u => u.Role!, u => u.State!, u => u.Img!)
                    .FirstOrDefaultAsync(u => u.Id == userId);

                if (loaded is null)
                {
                    AddError("User created but could not be loaded.");
                    return null;
                }

                return loaded.MapToDto<User, UserDto>(GetUserMapConfig());
            }
            catch (Exception ex)
            {
                if (transaction.GetDbTransaction().Connection != null)
                    await transaction.RollbackAsync();
                AddError(ex.Message);
                throw;
            }
        }

        public async Task<TokenDto?> LoginAsync(LoginModel model)
        {
            await using var transaction = unitOfWork.BeginTransaction();
            try
            {
                var user = await unitOfWork.UserRepository()
                    .GetAll(u => u.Role!, u => u.State!)
                    .FirstOrDefaultAsync(u => u.Username.Trim() == model.Username.Trim());

                if (user is null)
                {
                    AddError("User not found or username is incorrect.");
                    return null;
                }

                if (user.StateCode != StateConstants.Active)
                {
                    AddError("Your account is not active.");
                    return null;
                }

                var result = new PasswordHasher<User>().VerifyHashedPassword(user, user.PasswordHash, model.Password);
                if (result == PasswordVerificationResult.Failed)
                {
                    AddError("Password is incorrect.");
                    return null;
                }

                var tokenDto = jwtService.GenerateToken(user, true);

                await unitOfWork.UserRepository().Update(user);
                await unitOfWork.SaveChanges();
                await transaction.CommitAsync();

                return tokenDto;
            }
            catch
            {
                await transaction.RollbackAsync();
                return null;
            }
        }

        public async Task<UserDto?> GetProfileAsync()
        {
            var userId = userHelper.GetUserId();
            if (!userId.HasValue)
            {
                AddError("User not found.");
                return null;
            }

            var user = await unitOfWork.UserRepository()
                .GetAll(u => u.Role!, u => u.State!, u => u.Img!)
                .FirstOrDefaultAsync(u => u.Id == userId.Value);

            if (user is null)
            {
                AddError("User not found.");
                return null;
            }

            return user.MapToDto<User, UserDto>(GetUserMapConfig());
        }

        public async Task<TokenDto?> RefreshTokenAsync(TokenDto tokenDto)
        {
            var (isValid, username) = jwtService.ValidateAndGetUser(tokenDto.AccessToken);
            if (!isValid)
            {
                AddError("Invalid access token.");
                return null;
            }

            var user = await unitOfWork.UserRepository()
                .GetAll(u => u.Role!)
                .FirstOrDefaultAsync(u => u.Username == username
                                       && u.StateCode == StateConstants.Active);

            if (user is null || user.RefreshToken != tokenDto.RefreshToken
                || user.RefreshTokenExpireTime <= DateTimeOffset.UtcNow)
            {
                AddError("Refresh token is invalid or expired.");
                return null;
            }

            return jwtService.GenerateToken(user, false);
        }

        public async Task<string?> UpdateProfileAsync(UpdateUserModel model)
        {
            var (user, isExist) = await GetUserByIdAsync();
            if (!isExist || user is null) return null;

            var userId = userHelper.GetUserId()!.Value;

            user.FirstName = model.FirstName ?? user.FirstName;
            user.LastName = model.LastName ?? user.LastName;
            user.Bio = model.Bio ?? user.Bio;
            user.ModifiedUserId = userId;
            user.ModifiedDateTime = DateTime.UtcNow;

            await unitOfWork.UserRepository().Update(user);
            await unitOfWork.SaveChanges();
            return "Profile updated successfully.";
        }

        public async Task<string?> UpdateUsernameAsync(string newUsername)
        {
            var (user, isExist) = await GetUserByIdAsync();
            if (!isExist || user is null) return null;

            var usernameExists = await unitOfWork.UserRepository()
                .GetAll()
                .AnyAsync(u => u.Username == newUsername);
            if (usernameExists)
            {
                AddError("Username already exists.");
                return null;
            }

            var userId = userHelper.GetUserId()!.Value;
            user.Username = newUsername;
            user.ModifiedUserId = userId;
            user.ModifiedDateTime = DateTime.UtcNow;

            await unitOfWork.UserRepository().Update(user);
            await unitOfWork.SaveChanges();
            return "Username updated successfully.";
        }

        public async Task<string?> UpdateProfileImageAsync(IFormFile img)
        {
            var (user, isExist) = await GetUserByIdAsync();
            if (!isExist || user is null) return null;

            var userId = userHelper.GetUserId()!.Value;

            user.ImgId = user.ImgId.HasValue
                ? await contentService.UpdateContentForImage(user.ImgId.Value, img)
                : await contentService.CreateContentForImage(img, "profile");

            if (user.ImgId is null)
            {
                AddError("Image upload failed.");
                return null;
            }

            user.ModifiedUserId = userId;
            user.ModifiedDateTime = DateTime.UtcNow;

            await unitOfWork.UserRepository().Update(user);
            await unitOfWork.SaveChanges();
            return "Profile image updated successfully.";
        }

        // ── helpers ──────────────────────────────────────────────────────────────

        private async Task<(User? user, bool isExist)> GetUserByIdAsync()
        {
            var userId = userHelper.GetUserId();
            if (!userId.HasValue)
            {
                AddError("User not found.");
                return (null, false);
            }

            var user = await unitOfWork.UserRepository().GetById(userId.Value);
            if (user is null)
            {
                AddError("User not found.");
                return (null, false);
            }

            return (user, true);
        }

        private static TypeAdapterConfig GetUserMapConfig()
        {
            var config = new TypeAdapterConfig();
            config.NewConfig<User, UserDto>()
                .Map(dest => dest.Role, src => src.Role!.FullName)
                .Map(dest => dest.StateName, src => src.State!.FullName)
                .Map(dest => dest.ImgUrl, src => src.Img != null
                    ? src.Img.FileId.GetFileUrl() : null);
            return config;
        }
    }
}
