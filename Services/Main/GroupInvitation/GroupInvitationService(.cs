using Mapster;
using Microsoft.EntityFrameworkCore;
using StatusGeneric;
using StudyGroup.Api.Common.Constants;
using StudyGroup.Api.Common.Dtos.Main;
using StudyGroup.Api.Common.Extensions;
using StudyGroup.Api.Common.FilterOptions;
using StudyGroup.Api.Common.Models.Shared;
using StudyGroup.Api.Data.Repositories.Interfaces;
using StudyGroup.Api.Services.Common.Interfaces;
using StudyGroup.Api.Services.Main.GroupInvitation.Base.Interfaces;
using StudyGroup.Api.Services.Main.GroupInvitation.Base.QueryObjects;

namespace StudyGroup.Api.Services.Main.GroupInvitation
{
    public class GroupInvitationService(
            IUnitOfWork unitOfWork,
            IUserHelper userHelper)
            : StatusGenericHandler, IGroupInvitationService
    {
        public async Task<string?> InviteAsync(Guid groupId, string username, string? message)
        {
            var userId = userHelper.GetUserId();
            if (!userId.HasValue)
            {
                AddError("User not found.");
                return null;
            }

            // Only creator can invite
            var group = await unitOfWork.StudyGroupRepository()
                .GetAll()
                .Include(g => g.Members!.Where(m => m.StatusCode == StatusConstants.ActiveStatusCode))
                .FirstOrDefaultAsync(g => g.Id == groupId
                    && g.StatusCode != StatusConstants.DeletedStatusCode);

            if (group is null)
            {
                AddError("Study group not found.");
                return null;
            }

            if (group.CreatorId != userId.Value)
            {
                AddError("Only the group creator can send invitations.");
                return null;
            }

            // Only private groups use invitations
            if (group.PrivacyCode != PrivacyConstants.PrivateCode)
            {
                AddError("Invitations are only for private groups.");
                return null;
            }

            // Find target user
            var targetUser = await unitOfWork.UserRepository()
                .GetAll()
                .FirstOrDefaultAsync(u => u.Username == username
                    && u.StateCode == StateConstants.Active);

            if (targetUser is null)
            {
                AddError("User not found.");
                return null;
            }

            // Can't invite yourself
            if (targetUser.Id == userId.Value)
            {
                AddError("You cannot invite yourself.");
                return null;
            }

            // Already a member check
            var alreadyMember = group.Members?.Any(m => m.UserId == targetUser.Id) ?? false;
            if (alreadyMember)
            {
                AddError("This user is already a member of the group.");
                return null;
            }

            // Already has a pending invitation
            var pendingExists = await unitOfWork.GroupInvitationRepository()
                .GetAll()
                .AnyAsync(i => i.GroupId == groupId
                    && i.InvitedUserId == targetUser.Id
                    && i.StatusCode == InvitationConstants.PendingCode);

            if (pendingExists)
            {
                AddError("This user already has a pending invitation.");
                return null;
            }

            var invitation = new Data.Entities.MainEntities.GroupInvitation
            {
                GroupId = groupId,
                InvitedById = userId.Value,
                InvitedUserId = targetUser.Id,
                StatusCode = InvitationConstants.PendingCode,
                Message = message,
                CreatedUserId = userId.Value
            };

            await unitOfWork.GroupInvitationRepository().Add(invitation);
            await unitOfWork.SaveChanges();

            return "Invitation sent successfully.";
        }

        public async Task<string?> AcceptAsync(long invitationId)
        {
            var (invitation, isValid) = await GetPendingInvitationForCurrentUserAsync(invitationId);
            if (!isValid || invitation is null) return null;

            var userId = userHelper.GetUserId()!.Value;

            await using var transaction = unitOfWork.BeginTransaction();
            try
            {
                // Load group for max members and deadline checks
                var group = await unitOfWork.StudyGroupRepository()
                    .GetAll()
                    .Include(g => g.Members!.Where(m => m.StatusCode == StatusConstants.ActiveStatusCode))
                    .FirstOrDefaultAsync(g => g.Id == invitation.GroupId);

                if (group is null)
                {
                    AddError("Study group not found.");
                    return null;
                }

                if (group.JoinDeadline.HasValue && group.JoinDeadline.Value < DateOnly.FromDateTime(DateTime.UtcNow))
                {
                    AddError("The join deadline for this group has passed.");
                    return null;
                }

                var activeCount = group.Members?.Count ?? 0;
                if (group.MaxMembers.HasValue && activeCount >= group.MaxMembers.Value)
                {
                    AddError("This group has reached its maximum number of members.");
                    return null;
                }

                // Update invitation status
                invitation.StatusCode = InvitationConstants.AcceptedCode;
                invitation.RespondedDateTime = DateTimeOffset.UtcNow;
                invitation.ModifiedUserId = userId;
                invitation.ModifiedDateTime = DateTime.UtcNow;

                await unitOfWork.GroupInvitationRepository().Update(invitation);

                // Add as member
                var member = new Data.Entities.MainEntities.GroupMember
                {
                    GroupId = invitation.GroupId,
                    UserId = userId,
                    StatusCode = StatusConstants.ActiveStatusCode,
                    JoinedDateTime = DateTimeOffset.UtcNow,
                    MissCount = 0,
                    CreatedUserId = userId
                };

                await unitOfWork.GroupMemberRepository().Add(member);
                await unitOfWork.SaveChanges();
                await transaction.CommitAsync();

                return "Invitation accepted successfully.";
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                AddError(ex.Message);
                return null;
            }
        }

        public async Task<string?> RejectAsync(long invitationId)
        {
            var (invitation, isValid) = await GetPendingInvitationForCurrentUserAsync(invitationId);
            if (!isValid || invitation is null) return null;

            var userId = userHelper.GetUserId()!.Value;

            invitation.StatusCode = InvitationConstants.RejectedCode;
            invitation.RespondedDateTime = DateTimeOffset.UtcNow;
            invitation.ModifiedUserId = userId;
            invitation.ModifiedDateTime = DateTime.UtcNow;

            await unitOfWork.GroupInvitationRepository().Update(invitation);
            await unitOfWork.SaveChanges();

            return "Invitation rejected.";
        }

        public async Task<string?> CancelAsync(long invitationId)
        {
            var userId = userHelper.GetUserId();
            if (!userId.HasValue)
            {
                AddError("User not found.");
                return null;
            }

            var invitation = await unitOfWork.GroupInvitationRepository()
                .GetAll()
                .FirstOrDefaultAsync(i => i.Id == invitationId
                    && i.StatusCode == InvitationConstants.PendingCode);

            if (invitation is null)
            {
                AddError("Pending invitation not found.");
                return null;
            }

            // Only the creator (who sent it) can cancel
            if (invitation.InvitedById != userId.Value)
            {
                AddError("You are not authorized to cancel this invitation.");
                return null;
            }

            invitation.StatusCode = InvitationConstants.CancelledCode;
            invitation.ModifiedUserId = userId.Value;
            invitation.ModifiedDateTime = DateTime.UtcNow;

            await unitOfWork.GroupInvitationRepository().Update(invitation);
            await unitOfWork.SaveChanges();

            return "Invitation cancelled.";
        }

        public async Task<PaginationModel<GroupInvitationDto>> GetMyInvitationsAsync(
            GroupInvitationFilterOptions filterOptions)
        {
            var userId = userHelper.GetUserId();
            if (!userId.HasValue)
            {
                AddError("User not found.");
                return new PaginationModel<GroupInvitationDto>();
            }

            var query = unitOfWork.GroupInvitationRepository()
                .GetAll(i => i.StudyGroup!, i => i.InvitedBy!, i => i.InvitedUser!, i => i.Status!)
                .AsNoTracking()
                .Where(i => i.InvitedUserId == userId.Value);

            return query
                .ApplyFilter(filterOptions)
                .MapToDtos<Data.Entities.MainEntities.GroupInvitation, GroupInvitationDto>(GetCustomConfig())
                .ToPaginationModel(filterOptions.Page, filterOptions.PageSize);
        }

        public async Task<PaginationModel<GroupInvitationDto>> GetGroupInvitationsAsync(
            Guid groupId, GroupInvitationFilterOptions filterOptions)
        {
            var userId = userHelper.GetUserId();
            if (!userId.HasValue)
            {
                AddError("User not found.");
                return new PaginationModel<GroupInvitationDto>();
            }

            // Only creator can see group invitations
            var group = await unitOfWork.StudyGroupRepository()
                .GetAll()
                .FirstOrDefaultAsync(g => g.Id == groupId);

            if (group is null || group.CreatorId != userId.Value)
            {
                AddError("You are not authorized to view this group's invitations.");
                return new PaginationModel<GroupInvitationDto>();
            }

            var query = unitOfWork.GroupInvitationRepository()
                .GetAll(i => i.StudyGroup!, i => i.InvitedBy!, i => i.InvitedUser!, i => i.Status!)
                .AsNoTracking()
                .Where(i => i.GroupId == groupId);

            return query
                .ApplyFilter(filterOptions)
                .MapToDtos<Data.Entities.MainEntities.GroupInvitation, GroupInvitationDto>(GetCustomConfig())
                .ToPaginationModel(filterOptions.Page, filterOptions.PageSize);
        }

        // ── helpers ──────────────────────────────────────────────────────────
        private async Task<(Data.Entities.MainEntities.GroupInvitation? invitation, bool isValid)> GetPendingInvitationForCurrentUserAsync(
            long invitationId)
        {
            var userId = userHelper.GetUserId();
            if (!userId.HasValue)
            {
                AddError("User not found.");
                return (null, false);
            }

            var invitation = await unitOfWork.GroupInvitationRepository()
                .GetAll()
                .FirstOrDefaultAsync(i => i.Id == invitationId
                    && i.InvitedUserId == userId.Value
                    && i.StatusCode == InvitationConstants.PendingCode);

            if (invitation is null)
            {
                AddError("Pending invitation not found.");
                return (null, false);
            }

            return (invitation, true);
        }

        private static TypeAdapterConfig GetCustomConfig()
        {
            var config = new TypeAdapterConfig();
            config.NewConfig<Data.Entities.MainEntities.GroupInvitation, GroupInvitationDto>()
                .Map(dest => dest.GroupName, src => src.StudyGroup!.Name)
                .Map(dest => dest.InvitedByUsername, src => src.InvitedBy!.Username)
                .Map(dest => dest.InvitedUsername, src => src.InvitedUser!.Username)
                .Map(dest => dest.StatusName, src => src.Status!.FullName);
            return config;
        }
    }
}
