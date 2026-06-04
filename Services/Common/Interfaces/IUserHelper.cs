namespace StudyGroup.Api.Services.Common.Interfaces
{
    public interface IUserHelper
    {
        Guid? GetUserId();
        string GetUsername();
        string GetUserRole();
        int GetUserRoleCode();
    }
}
