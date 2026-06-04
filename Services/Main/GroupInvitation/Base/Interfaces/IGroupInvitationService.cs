using StatusGeneric;
using StudyGroup.Api.Common.Dtos.Main;
using StudyGroup.Api.Common.FilterOptions;
using StudyGroup.Api.Common.Models.Shared;

namespace StudyGroup.Api.Services.Main.GroupInvitation.Base.Interfaces
{
    public interface IGroupInvitationService : IStatusGeneric
    {
        // Creator invites a user by username
        Task<string?> InviteAsync(Guid groupId, string username, string? message);
        // Invited user responds
        Task<string?> AcceptAsync(long invitationId);
        Task<string?> RejectAsync(long invitationId);
        // Creator cancels a pending invitation
        Task<string?> CancelAsync(long invitationId);
        // Get my received invitations
        Task<PaginationModel<GroupInvitationDto>> GetMyInvitationsAsync(GroupInvitationFilterOptions filterOptions);
        // Get invitations sent for a group (creator only)
        Task<PaginationModel<GroupInvitationDto>> GetGroupInvitationsAsync(Guid groupId, GroupInvitationFilterOptions filterOptions);
    }
}
