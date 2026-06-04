using System.ComponentModel.DataAnnotations;

namespace StudyGroup.Api.Common.Models.Auth;

public record class LoginModel
{
    [Required]
    public string Username { get; set; } = string.Empty;
 
    [Required]
    public string Password { get; set; } = string.Empty;
}