using System.Security.Claims;

namespace StudyGroup.Api.Services.Common.Interfaces
{
    public class UserHelper(IHttpContextAccessor httpContextAccessor) : IUserHelper
    {
        public Guid? GetUserId() => Guid.TryParse(
            httpContextAccessor.HttpContext?.User?
                .FindFirst(ClaimTypes.NameIdentifier)?.Value, out var id)
            ? id : null;

        public string GetUsername() =>
            httpContextAccessor.HttpContext!.User.FindFirst(ClaimTypes.Name)!.Value;

        public string GetUserRole() =>
            httpContextAccessor.HttpContext!.User.FindFirst(ClaimTypes.Role)!.Value;

        public int GetUserRoleCode() =>
            int.Parse(httpContextAccessor.HttpContext!.User.FindFirst("role_code")!.Value);
    }
}
