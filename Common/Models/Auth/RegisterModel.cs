using System.ComponentModel.DataAnnotations;

namespace StudyGroup.Api.Common.Models.Auth;

public record class RegisterModel
{
    [Required]
    [MaxLength(50)]
    public string Username { get; set; } = string.Empty;
 
    [Required]
    [EmailAddress]
    [MaxLength(200)]
    public string Email { get; set; } = string.Empty;
 
    [Required]
    [MinLength(6)]
    public string Password { get; set; } = string.Empty;
 
    [Required]
    [MaxLength(100)]
    public string FirstName { get; set; } = string.Empty;
 
    [Required]
    [MaxLength(100)]
    public string LastName { get; set; } = string.Empty;
 
    [MaxLength(1000)]
    public string? Bio { get; set; }
 
    public IFormFile? ImageFile { get; set; }
}