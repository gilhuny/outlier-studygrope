namespace StudyGroup.Api.Common.Models.User;

public record class UpdateUserModelForAdmin : UpdateUserModel
{
    public int? RoleCode { get; set; }
    public int? StateCode { get; set; }
}