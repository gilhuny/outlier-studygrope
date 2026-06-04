using StatusGeneric;
using StudyGroup.Api.Common.Dtos.Auth;
using StudyGroup.Api.Common.Models.Auth;
using StudyGroup.Api.Common.Models.User;

namespace StudyGroup.Api.Services.Auth.Interfaces;

public interface IAuthService : IStatusGeneric
{
    Task<UserDto?> RegisterAsync(RegisterModel model);
    Task<TokenDto?> LoginAsync(LoginModel model);
    Task<UserDto?> GetProfileAsync();
    Task<TokenDto?> RefreshTokenAsync(TokenDto tokenDto);
    Task<string?> UpdateProfileAsync(UpdateUserModel model);
    Task<string?> UpdateUsernameAsync(string newUsername);
    Task<string?> UpdateProfileImageAsync(IFormFile img);
}