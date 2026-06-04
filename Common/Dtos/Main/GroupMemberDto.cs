namespace StudyGroup.Api.Common.Dtos.Main
{
    public class GroupMemberDto
    {
        public long Id { get; set; }
        public Guid GroupId { get; set; }
        public Guid UserId { get; set; }
        public string Username { get; set; } = string.Empty;
        public string? UserImageUrl { get; set; }
        public string StatusName { get; set; } = string.Empty;
        public int MissCount { get; set; }
        public DateTimeOffset JoinedDateTime { get; set; }
        public DateTimeOffset? LeftDateTime { get; set; }
    }
}
