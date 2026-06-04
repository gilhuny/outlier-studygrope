namespace StudyGroup.Api.Common.Dtos.Main
{
    public class GroupInvitationDto
    {
        public long Id { get; set; }
        public Guid GroupId { get; set; }
        public string GroupName { get; set; } = string.Empty;
        public Guid InvitedById { get; set; }
        public string InvitedByUsername { get; set; } = string.Empty;
        public Guid InvitedUserId { get; set; }
        public string InvitedUsername { get; set; } = string.Empty;
        public string StatusName { get; set; } = string.Empty;
        public string? Message { get; set; }
        public DateTimeOffset? RespondedDateTime { get; set; }
        public DateTime CreatedDateTime { get; set; }
    }
}
