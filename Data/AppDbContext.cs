using Microsoft.EntityFrameworkCore;
using StudyGroup.Api.Entities;

namespace StudyGroup.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    // 우리가 만든 User 엔티티를 DB 테이블로 등록
    public DbSet<User> Users { get; set; }
}