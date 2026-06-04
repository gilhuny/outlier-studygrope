namespace StudyGroup.Api.Common.Dtos.Main
{
    public class DailyProofDto
    {
        public long Id { get; set; }
        public Guid GroupId { get; set; }
        public string GroupName { get; set; } = string.Empty;
        public Guid UserId { get; set; }
        public string Username { get; set; } = string.Empty;
        public string? UserImageUrl { get; set; }
        public DateOnly ProofDate { get; set; }
        public string ProofTypeName { get; set; } = string.Empty;
        public string? TextContent { get; set; }
        public string StatusName { get; set; } = string.Empty;
        public List<string> ContentUrls { get; set; } = [];
        public double AverageRating { get; set; }
        public int RatingCount { get; set; }
        public int CommentCount { get; set; }
        public DateTime CreatedDateTime { get; set; }
    }
}
