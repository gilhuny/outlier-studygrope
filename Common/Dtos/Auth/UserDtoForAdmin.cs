namespace StudyGroup.Api.Common.Dtos.Auth;

public class UserDtoForAdmin : UserDto
{
    public int RoleCode { get; set; }
    public int StateCode { get; set; }
    public long? ImgId { get; set; }
    public DateTimeOffset RefreshTokenExpireTime { get; set; }
}