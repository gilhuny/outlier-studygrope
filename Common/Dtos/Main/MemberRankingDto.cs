namespace StudyGroup.Api.Common.Dtos.Main
{
    public class MemberRankingDto
    {
        public long Id { get; set; }
        public Guid GroupId { get; set; }
        public Guid UserId { get; set; }
        public string Username { get; set; } = string.Empty;
        public string? UserImageUrl { get; set; }
        public decimal TotalScore { get; set; }
        public decimal AvgRating { get; set; }
        public int UploadCount { get; set; }
        public int MissCount { get; set; }
        public int CurrentStreak { get; set; }
        public int LongestStreak { get; set; }
        public int? RankPosition { get; set; }
        public DateTimeOffset LastCalculatedAt { get; set; }
    }
}
