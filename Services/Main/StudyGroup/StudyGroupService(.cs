using Mapster;
using Microsoft.EntityFrameworkCore;
using StatusGeneric;
using StudyGroup.Api.Common.Constants;
using StudyGroup.Api.Common.Dtos.Main;
using StudyGroup.Api.Common.Extensions;
using StudyGroup.Api.Common.FilterOptions;
using StudyGroup.Api.Common.Models.Main.StudyGroup;
using StudyGroup.Api.Common.Models.Shared;
using StudyGroup.Api.Data.Entities.MainEntities;
using StudyGroup.Api.Data.Repositories.Interfaces;
using StudyGroup.Api.Services.Common.Interfaces;
using StudyGroup.Api.Services.Main.StudyGroup.Base.Interfaces;
using StudyGroup.Api.Services.Main.StudyGroup.Base.QueryObjects;
using StudyGroup.Api.Services.Public.Content.Interfaces;

namespace StudyGroup.Api.Services.Main.StudyGroup
{
    public class StudyGroupService(
            IUnitOfWork unitOfWork,
            IUserHelper userHelper,
            IContentService contentService)
            : StatusGenericHandler, IStudyGroupService
    {
        public async Task<Guid> CreateAsync(CreateStudyGroupModel model)
        {
            var userId = userHelper.GetUserId();
            if (!userId.HasValue)
            {
                AddError("User not found.");
                return Guid.Empty;
            }

            await using var transaction = unitOfWork.BeginTransaction();
            try
            {
                long? coverImgId = null;
                if (model.CoverImage is not null)
                {
                    coverImgId = await contentService.CreateContentForImage(model.CoverImage, "study-groups");
                    if (coverImgId is null)
                    {
                        AddError("Cover image upload failed.");
                        return Guid.Empty;
                    }
                }

                var group = new StudyGroupEntity
                {
                    Id = Guid.NewGuid(),
                    Name = model.Name,
                    Description = model.Description,
                    CategoryCode = model.CategoryCode,
                    PrivacyCode = model.PrivacyCode,
                    JoinPolicyCode = model.JoinPolicyCode,
                    MaxMembers = model.MaxMembers,
                    MaxMissDays = model.MaxMissDays,
                    UploadDeadlineHour = model.UploadDeadlineHour,
                    StartDate = model.StartDate,
                    EndDate = model.EndDate,
                    JoinDeadline = model.JoinDeadline,
                    CoverImgId = coverImgId,
                    CreatorId = userId.Value,
                    StatusCode = StatusConstants.ActiveStatusCode,
                    CreatedUserId = userId.Value
                };

                await unitOfWork.StudyGroupRepository().Add(group);
                await unitOfWork.SaveChanges();

                if (model.TagCodes?.Any() == true)
                {
                    var tags = model.TagCodes.Select(code => new StudyGroupTag
                    {
                        GroupId = group.Id,
                        TagCode = code,
                        CreatedUserId = userId.Value
                    }).ToList();

                    foreach (var tag in tags)
                        await unitOfWork.StudyGroupTagRepository().Add(tag);
                }

                // Auto-add creator as admin member
                var member = new Data.Entities.MainEntities.GroupMember
                {
                    GroupId = group.Id,
                    UserId = userId.Value,
                    StatusCode = StatusConstants.ActiveStatusCode,
                    JoinedDateTime = DateTimeOffset.UtcNow,
                    MissCount = 0,
                    CreatedUserId = userId.Value
                };

                await unitOfWork.GroupMemberRepository().Add(member);
                await unitOfWork.SaveChanges();
                await transaction.CommitAsync();

                return group.Id;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                AddError(ex.Message);
                return Guid.Empty;
            }
        }

        public async Task<StudyGroupDto?> GetByIdAsync(Guid id)
        {
            var userId = userHelper.GetUserId();

            var group = await unitOfWork.StudyGroupRepository()
                .GetAll(
                    g => g.Creator!,
                    g => g.Status!,
                    g => g.Category!,
                    g => g.Privacy!,
                    g => g.JoinPolicy!,
                    g => g.CoverImg!)
                .Include(g => g.Tags!).ThenInclude(t => t.Tag)
                .Include(g => g.Members!.Where(m => m.StatusCode == StatusConstants.ActiveStatusCode))
                .FirstOrDefaultAsync(g => g.Id == id
                    && g.StatusCode != StatusConstants.DeletedStatusCode);

            if (group is null)
            {
                AddError("Study group not found.");
                return null;
            }

            if (group.PrivacyCode == PrivacyConstants.PrivateCode)
            {
                var isMember = userId.HasValue &&
                    group.Members!.Any(m => m.UserId == userId.Value);
                if (!isMember)
                {
                    AddError("You do not have access to this group.");
                    return null;
                }
            }

            return group.MapToDto<StudyGroupEntity, StudyGroupDto>(GetCustomConfig());
        }

        public async Task<PaginationModel<StudyGroupDto>> GetAllAsync(StudyGroupFilterOptions filterOptions)
        {
            var query = unitOfWork.StudyGroupRepository()
                .GetAll(
                    g => g.Creator!,
                    g => g.Status!,
                    g => g.Category!,
                    g => g.Privacy!,
                    g => g.JoinPolicy!,
                    g => g.CoverImg!)
                .Include(g => g.Tags!).ThenInclude(t => t.Tag)
                .Include(g => g.Members!.Where(m => m.StatusCode == StatusConstants.ActiveStatusCode))
                .Where(g => g.PrivacyCode == PrivacyConstants.PublicCode
                         && g.StatusCode != StatusConstants.DeletedStatusCode);

            return query
                .ApplyFilter(filterOptions)
                .MapToDtos<StudyGroupEntity, StudyGroupDto>(GetCustomConfig())
                .ToPaginationModel(filterOptions.Page, filterOptions.PageSize);
        }

        public async Task<PaginationModel<StudyGroupDto>> GetMyGroupsAsync(StudyGroupFilterOptions filterOptions)
        {
            var userId = userHelper.GetUserId();
            if (!userId.HasValue)
            {
                AddError("User not found.");
                return new PaginationModel<StudyGroupDto>();
            }

            var query = unitOfWork.StudyGroupRepository()
                .GetAll(
                    g => g.Creator!,
                    g => g.Status!,
                    g => g.Category!,
                    g => g.Privacy!,
                    g => g.JoinPolicy!,
                    g => g.CoverImg!)
                .Include(g => g.Tags!).ThenInclude(t => t.Tag)
                .Include(g => g.Members!.Where(m => m.StatusCode == StatusConstants.ActiveStatusCode))
                .Where(g => g.Members!.Any(m => m.UserId == userId.Value
                         && m.StatusCode == StatusConstants.ActiveStatusCode)
                         && g.StatusCode != StatusConstants.DeletedStatusCode);

            return query
                .ApplyFilter(filterOptions)
                .MapToDtos<StudyGroupEntity, StudyGroupDto>(GetCustomConfig())
                .ToPaginationModel(filterOptions.Page, filterOptions.PageSize);
        }

        public async Task<string?> UpdateAsync(Guid id, UpdateStudyGroupModel model)
        {
            var (group, isExist) = await GetGroupIfCreatorAsync(id);
            if (!isExist || group is null) return null;

            if (!StatusConstants.CanApply(group.StatusCode, StatusConstants.UpdatedStatusCode))
            {
                AddError("Cannot update group in its current status.");
                return null;
            }

            var userId = userHelper.GetUserId()!.Value;

            group.Name = model.Name ?? group.Name;
            group.Description = model.Description ?? group.Description;
            group.CategoryCode = model.CategoryCode ?? group.CategoryCode;
            group.PrivacyCode = model.PrivacyCode ?? group.PrivacyCode;
            group.JoinPolicyCode = model.JoinPolicyCode ?? group.JoinPolicyCode;
            group.MaxMembers = model.MaxMembers ?? group.MaxMembers;
            group.MaxMissDays = model.MaxMissDays ?? group.MaxMissDays;
            group.UploadDeadlineHour = model.UploadDeadlineHour ?? group.UploadDeadlineHour;
            group.StartDate = model.StartDate ?? group.StartDate;
            group.EndDate = model.EndDate ?? group.EndDate;
            group.JoinDeadline = model.JoinDeadline ?? group.JoinDeadline;
            group.StatusCode = StatusConstants.UpdatedStatusCode;
            group.ModifiedUserId = userId;
            group.ModifiedDateTime = DateTime.UtcNow;

            if (model.TagCodes is not null)
            {
                var existing = await unitOfWork.StudyGroupTagRepository()
                    .GetAll()
                    .Where(t => t.GroupId == id)
                    .ToListAsync();

                foreach (var tag in existing)
                    await unitOfWork.StudyGroupTagRepository().Delete(tag);

                foreach (var code in model.TagCodes)
                    await unitOfWork.StudyGroupTagRepository().Add(new StudyGroupTag
                    {
                        GroupId = id,
                        TagCode = code,
                        CreatedUserId = userId
                    });
            }

            await unitOfWork.StudyGroupRepository().Update(group);
            await unitOfWork.SaveChanges();
            return "Study group updated successfully.";
        }

        public async Task<string?> UpdateCoverImageAsync(Guid id, IFormFile img)
        {
            var (group, isExist) = await GetGroupIfCreatorAsync(id);
            if (!isExist || group is null) return null;

            var userId = userHelper.GetUserId()!.Value;

            group.CoverImgId = group.CoverImgId.HasValue
                ? await contentService.UpdateContentForImage(group.CoverImgId.Value, img)
                : await contentService.CreateContentForImage(img, "study-groups");

            if (group.CoverImgId is null)
            {
                AddError("Image upload failed.");
                return null;
            }

            group.ModifiedUserId = userId;
            group.ModifiedDateTime = DateTime.UtcNow;

            await unitOfWork.StudyGroupRepository().Update(group);
            await unitOfWork.SaveChanges();
            return "Cover image updated successfully.";
        }

        public async Task<string?> ActivateAsync(Guid id) =>
            await UpdateStatusAsync(id, StatusConstants.ActiveStatusCode, "Study group activated successfully.");

        public async Task<string?> DeactivateAsync(Guid id) =>
            await UpdateStatusAsync(id, StatusConstants.PassiveStatusCode, "Study group deactivated successfully.");

        public async Task<string?> DeleteAsync(Guid id) =>
            await UpdateStatusAsync(id, StatusConstants.DeletedStatusCode, "Study group deleted successfully.");

        // ── helpers ──────────────────────────────────────────────────────────
        private async Task<string?> UpdateStatusAsync(Guid id, int targetStatusCode, string message)
        {
            var (group, isExist) = await GetGroupIfCreatorAsync(id);
            if (!isExist || group is null) return null;

            if (!StatusConstants.CanApply(group.StatusCode, targetStatusCode))
            {
                AddError($"Cannot transition to status {targetStatusCode} from current status.");
                return null;
            }

            group.StatusCode = targetStatusCode;
            group.ModifiedUserId = userHelper.GetUserId();
            group.ModifiedDateTime = DateTime.UtcNow;

            await unitOfWork.StudyGroupRepository().Update(group);
            await unitOfWork.SaveChanges();
            return message;
        }

        private async Task<(StudyGroupEntity? group, bool isExist)> GetGroupIfCreatorAsync(Guid id)
        {
            var userId = userHelper.GetUserId();
            if (!userId.HasValue)
            {
                AddError("User not found.");
                return (null, false);
            }

            var group = await unitOfWork.StudyGroupRepository()
                .GetAll()
                .FirstOrDefaultAsync(g => g.Id == id
                    && g.StatusCode != StatusConstants.DeletedStatusCode);

            if (group is null)
            {
                AddError("Study group not found.");
                return (null, false);
            }

            if (group.CreatorId != userId.Value)
            {
                AddError("You are not authorized to perform this action.");
                return (null, false);
            }

            return (group, true);
        }

        private static TypeAdapterConfig GetCustomConfig()
        {
            var config = new TypeAdapterConfig();
            config.NewConfig<StudyGroupEntity, StudyGroupDto>()
                .Map(dest => dest.CreatorUsername, src => src.Creator!.Username)
                .Map(dest => dest.StatusName, src => src.Status!.FullName)
                .Map(dest => dest.CategoryName, src => src.Category!.FullName)
                .Map(dest => dest.PrivacyName, src => src.Privacy!.FullName)
                .Map(dest => dest.JoinPolicyName, src => src.JoinPolicy!.FullName)
                .Map(dest => dest.CoverImageUrl, src => src.CoverImg != null
                    ? src.CoverImg.FileId.GetFileUrl() : null)
                .Map(dest => dest.Tags, src => src.Tags != null
                    ? src.Tags.Select(t => t.Tag!.FullName).ToList() : new List<string>())
                .Map(dest => dest.MemberCount, src => src.Members != null
                    ? src.Members.Count : 0);
            return config;
        }
    }
}

