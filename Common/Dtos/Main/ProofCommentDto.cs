namespace StudyGroup.Api.Common.Dtos.Main
{
    public class ProofCommentDto
    {
        public long Id { get; set; }
        public long ProofId { get; set; }
        public Guid UserId { get; set; }
        public string Username { get; set; } = string.Empty;
        public string? UserImageUrl { get; set; }
        public string CommentText { get; set; } = string.Empty;
        public string StatusName { get; set; } = string.Empty;
        public DateTime CreatedDateTime { get; set; }
    }
}
