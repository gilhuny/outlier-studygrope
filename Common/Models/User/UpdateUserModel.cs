using System.ComponentModel.DataAnnotations;

namespace StudyGroup.Api.Common.Models.User;

public record class UpdateUserModel
{
    [MaxLength(100)]
    public string? FirstName { get; set; }
 
    [MaxLength(100)]
    public string? LastName { get; set; }
 
    [MaxLength(1000)]
    public string? Bio { get; set; }
}