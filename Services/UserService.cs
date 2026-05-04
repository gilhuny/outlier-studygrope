using StudyGroup.Api.Data;
using StudyGroup.Api.Entities;

namespace StudyGroup.Api.Services;

public class UserService {
    private readonly AppDbContext _context;
    public UserService(AppDbContext context) { _context = context; }

    // 비즈니스 로직 예시: 유저 점수 올리기
    public void AddUserScore(int userId, int points) {
        var user = _context.Users.Find(userId);
        if (user != null) {
            user.Score += points;
            _context.SaveChanges();
        }
    }
}