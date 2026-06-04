namespace StudyGroup.Api.Common.Dtos.Auth;

public class UserDto
{
    public Guid Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? Bio { get; set; }
    public string? ImgUrl { get; set; }
    public string Role { get; set; } = string.Empty;
    public string StateName { get; set; } = string.Empty;
}