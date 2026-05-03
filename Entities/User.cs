namespace StudyGroup.Api.Entities;

public class User
{
    public int Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty; // 비밀번호는 암호화해서 저장!
    public string Name { get; set; } = string.Empty;
    public int Score { get; set; } = 0; // 4번 기능: 스코어 점수
}