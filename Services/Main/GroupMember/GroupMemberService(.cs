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
using StudyGroup.Api.Services.Main.GroupMember.Base.Interfaces;
using StudyGroup.Api.Services.Main.GroupMember.Base.QueryObjects;

namespace StudyGroup.Api.Services.Main.GroupMember
{
    public class GroupMemberService(
            IUnitOfWork unitOfWork,
            IUserHelper userHelper)
            : StatusGenericHandler, IGroupMemberService
    {
        public async Task<PaginationModel<GroupMemberDto>> GetAllAsync(GroupMemberFilterOptions filterOptions)
        {
            var query = unitOfWork.GroupMemberRepository()
                .GetAll(m => m.User!, m => m.Status!)
                .Include(m => m.User!.Img)
                .AsNoTracking();

            return query
                .ApplyFilter(filterOptions)
                .MapToDtos<Data.Entities.MainEntities.GroupMember, GroupMemberDto>(GetCustomConfig())
                .ToPaginationModel(filterOptions.Page, filterOptions.PageSize);
        }

        public async Task<GroupMemberDto?> GetByIdAsync(long id)
        {
            var member = await unitOfWork.GroupMemberRepository()
                .GetAll(m => m.User!, m => m.Status!)
                .Include(m => m.User!.Img)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == id);

            if (member is null)
            {
                AddError("Member not found.");
                return null;
            }

            return member.MapToDto<Data.Entities.MainEntities.GroupMember, GroupMemberDto>(GetCustomConfig());
        }

        public async Task<string?> JoinAsync(Guid groupId)
        {
            var userId = userHelper.GetUserId();
            if (!userId.HasValue)
            {
                AddError("User not found.");
                return null;
            }

            // Load group with member count
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

            if (group.StatusCode != StatusConstants.ActiveStatusCode)
            {
                AddError("This group is not active.");
                return null;
            }

            // Private groups require invitation
            if (group.PrivacyCode == PrivacyConstants.PrivateCode)
            {
                AddError("This group is private. You need an invitation to join.");
                return null;
            }

            // Join deadline check
            if (group.JoinDeadline.HasValue && group.JoinDeadline.Value < DateOnly.FromDateTime(DateTime.UtcNow))
            {
                AddError("The join deadline for this group has passed.");
                return null;
            }

            // Max members check
            var activeCount = group.Members?.Count ?? 0;
            if (group.MaxMembers.HasValue && activeCount >= group.MaxMembers.Value)
            {
                AddError("This group has reached its maximum number of members.");
                return null;
            }

            // Already a member check
            var alreadyMember = await unitOfWork.GroupMemberRepository()
                .GetAll()
                .AnyAsync(m => m.GroupId == groupId
                    && m.UserId == userId.Value
                    && m.StatusCode == StatusConstants.ActiveStatusCode);

            if (alreadyMember)
            {
                AddError("You are already a member of this group.");
                return null;
            }

            var member = new Data.Entities.MainEntities.GroupMember
            {
                GroupId = groupId,
                UserId = userId.Value,
                StatusCode = StatusConstants.ActiveStatusCode,
                JoinedDateTime = DateTimeOffset.UtcNow,
                MissCount = 0,
                CreatedUserId = userId.Value
            };

            await unitOfWork.GroupMemberRepository().Add(member);
            await unitOfWork.SaveChanges();

            return "Joined successfully.";
        }

        public async Task<string?> LeaveAsync(Guid groupId)
        {
            var userId = userHelper.GetUserId();
            if (!userId.HasValue)
            {
                AddError("User not found.");
                return null;
            }

            var member = await unitOfWork.GroupMemberRepository()
                .GetAll()
                .FirstOrDefaultAsync(m => m.GroupId == groupId
                    && m.UserId == userId.Value
                    && m.StatusCode == StatusConstants.ActiveStatusCode);

            if (member is null)
            {
                AddError("You are not a member of this group.");
                return null;
            }

            // Creator cannot leave — they must delete or transfer the group
            var group = await unitOfWork.StudyGroupRepository()
                .GetAll()
                .FirstOrDefaultAsync(g => g.Id == groupId);

            if (group is not null && group.CreatorId == userId.Value)
            {
                AddError("You are the creator of this group. Delete the group instead of leaving.");
                return null;
            }

            member.StatusCode = StatusConstants.PassiveStatusCode;
            member.LeftDateTime = DateTimeOffset.UtcNow;
            member.ModifiedUserId = userId.Value;
            member.ModifiedDateTime = DateTime.UtcNow;

            await unitOfWork.GroupMemberRepository().Update(member);
            await unitOfWork.SaveChanges();

            return "Left the group successfully.";
        }

        public async Task<string?> KickAsync(long memberId)
        {
            var userId = userHelper.GetUserId();
            if (!userId.HasValue)
            {
                AddError("User not found.");
                return null;
            }

            var member = await unitOfWork.GroupMemberRepository()
                .GetAll()
                .FirstOrDefaultAsync(m => m.Id == memberId
                    && m.StatusCode == StatusConstants.ActiveStatusCode);

            if (member is null)
            {
                AddError("Member not found.");
                return null;
            }

            // Only group creator can kick
            var group = await unitOfWork.StudyGroupRepository()
                .GetAll()
                .FirstOrDefaultAsync(g => g.Id == member.GroupId);

            if (group is null || group.CreatorId != userId.Value)
            {
                AddError("You are not authorized to kick members from this group.");
                return null;
            }

            // Creator cannot kick themselves
            if (member.UserId == userId.Value)
            {
                AddError("You cannot kick yourself from the group.");
                return null;
            }

            member.StatusCode = StatusConstants.PassiveStatusCode;
            member.LeftDateTime = DateTimeOffset.UtcNow;
            member.ModifiedUserId = userId.Value;
            member.ModifiedDateTime = DateTime.UtcNow;

            await unitOfWork.GroupMemberRepository().Update(member);
            await unitOfWork.SaveChanges();

            return "Member kicked successfully.";
        }

        // ── helpers ──────────────────────────────────────────────────────────
        private static TypeAdapterConfig GetCustomConfig()
        {
            var config = new TypeAdapterConfig();
            config.NewConfig<Data.Entities.MainEntities.GroupMember, GroupMemberDto>()
                .Map(dest => dest.Username, src => src.User!.Username)
                .Map(dest => dest.StatusName, src => src.Status!.FullName)
                .Map(dest => dest.UserImageUrl, src => src.User!.Img != null
                    ? src.User.Img.FileId.GetFileUrl() : null);
            return config;
        }
    }
}
