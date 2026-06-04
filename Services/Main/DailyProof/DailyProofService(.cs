using Mapster;
using Microsoft.EntityFrameworkCore;
using StatusGeneric;
using StudyGroup.Api.Common.Constants;
using StudyGroup.Api.Common.Dtos.Main;
using StudyGroup.Api.Common.Extensions;
using StudyGroup.Api.Common.FilterOptions;
using StudyGroup.Api.Common.Models.Main.DailyProof;
using StudyGroup.Api.Common.Models.Shared;
using StudyGroup.Api.Data.Entities.MainEntities;
using StudyGroup.Api.Data.Repositories.Interfaces;
using StudyGroup.Api.Services.Common.Interfaces;
using StudyGroup.Api.Services.Main.DailyProof.Base.Interfaces;
using StudyGroup.Api.Services.Main.DailyProof.Base.QueryObjects;
using StudyGroup.Api.Services.Main.MemberRanking.Base.Interfaces;
using StudyGroup.Api.Services.Public.Content.Interfaces;

namespace StudyGroup.Api.Services.Main.DailyProof
{
    public class DailyProofService(
            IUnitOfWork unitOfWork,
            IUserHelper userHelper,
            IContentService contentService, IMemberRankingService memberRankingService)
            : StatusGenericHandler, IDailyProofService
    {
        public async Task<PaginationModel<DailyProofDto>> GetAllAsync(DailyProofFilterOptions filterOptions)
        {
            var userId = userHelper.GetUserId();

            var query = unitOfWork.DailyProofRepository()
                .GetAll(
                    p => p.User!,
                    p => p.StudyGroup!,
                    p => p.ProofType!,
                    p => p.Status!)
                .Include(p => p.User!.Img)
                .Include(p => p.ProofContents!).ThenInclude(pc => pc.Content)
                .Include(p => p.Ratings!)
                .Include(p => p.Comments!)
                .AsNoTracking()
                .Where(p => p.StatusCode != StatusConstants.DeletedStatusCode);

            // Respect group privacy
            query = query.Where(p =>
                p.StudyGroup!.PrivacyCode == PrivacyConstants.PublicCode ||
                (userId.HasValue && p.StudyGroup.Members!
                    .Any(m => m.UserId == userId.Value
                        && m.StatusCode == StatusConstants.ActiveStatusCode)));

            return query
                .ApplyFilter(filterOptions)
                .MapToDtos<Data.Entities.MainEntities.DailyProof, DailyProofDto>(GetCustomConfig())
                .ToPaginationModel(filterOptions.Page, filterOptions.PageSize);
        }

        public async Task<DailyProofDto?> GetByIdAsync(long id)
        {
            var userId = userHelper.GetUserId();

            var proof = await unitOfWork.DailyProofRepository()
                .GetAll(
                    p => p.User!,
                    p => p.StudyGroup!,
                    p => p.ProofType!,
                    p => p.Status!)
                .Include(p => p.User!.Img)
                .Include(p => p.ProofContents!).ThenInclude(pc => pc.Content)
                .Include(p => p.Ratings!)
                .Include(p => p.Comments!)
                .Include(p => p.StudyGroup!.Members!)
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == id
                    && p.StatusCode != StatusConstants.DeletedStatusCode);

            if (proof is null)
            {
                AddError("Proof not found.");
                return null;
            }

            // Respect group privacy
            if (proof.StudyGroup!.PrivacyCode == PrivacyConstants.PrivateCode)
            {
                var isMember = userId.HasValue &&
                    proof.StudyGroup.Members!.Any(m => m.UserId == userId.Value
                        && m.StatusCode == StatusConstants.ActiveStatusCode);
                if (!isMember)
                {
                    AddError("You do not have access to this proof.");
                    return null;
                }
            }

            return proof.MapToDto<Data.Entities.MainEntities.DailyProof, DailyProofDto>(GetCustomConfig());
        }

        public async Task<long?> CreateAsync(CreateDailyProofModel model)
        {
            var userId = userHelper.GetUserId();
            if (!userId.HasValue)
            {
                AddError("User not found.");
                return null;
            }

            // Must be an active member
            var group = await unitOfWork.StudyGroupRepository()
                .GetAll()
                .FirstOrDefaultAsync(g => g.Id == model.GroupId
                    && g.StatusCode != StatusConstants.DeletedStatusCode);

            if (group is null)
            {
                AddError("Study group not found.");
                return null;
            }

            var isMember = await unitOfWork.GroupMemberRepository()
                .GetAll()
                .AnyAsync(m => m.GroupId == model.GroupId
                    && m.UserId == userId.Value
                    && m.StatusCode == StatusConstants.ActiveStatusCode);

            if (!isMember)
            {
                AddError("You are not an active member of this group.");
                return null;
            }

            // Upload deadline check
            var now = DateTime.UtcNow;
            if (now.Hour >= group.UploadDeadlineHour)
            {
                AddError($"Upload deadline has passed. Proofs must be submitted before {group.UploadDeadlineHour}:00 UTC.");
                return null;
            }

            var today = DateOnly.FromDateTime(now);

            // One proof per day per group check
            var alreadySubmitted = await unitOfWork.DailyProofRepository()
                .GetAll()
                .AnyAsync(p => p.GroupId == model.GroupId
                    && p.UserId == userId.Value
                    && p.ProofDate == today
                    && p.StatusCode != StatusConstants.DeletedStatusCode);

            if (alreadySubmitted)
            {
                AddError("You have already submitted a proof for today in this group.");
                return null;
            }

            // Must have text or at least one file
            if (string.IsNullOrWhiteSpace(model.TextContent) && (model.Files is null || !model.Files.Any()))
            {
                AddError("Proof must contain text content or at least one file.");
                return null;
            }

            await using var transaction = unitOfWork.BeginTransaction();
            try
            {
                var proof = new Data.Entities.MainEntities.DailyProof
                {
                    GroupId = model.GroupId,
                    UserId = userId.Value,
                    ProofDate = today,
                    ProofTypeCode = model.ProofTypeCode,
                    TextContent = model.TextContent,
                    StatusCode = StatusConstants.ActiveStatusCode,
                    CreatedUserId = userId.Value
                };

                await unitOfWork.DailyProofRepository().Add(proof);
                await unitOfWork.SaveChanges();

                // Upload files
                if (model.Files?.Any() == true)
                {
                    foreach (var file in model.Files)
                    {
                        var contentId = await contentService.CreateContentForImage(file, "proofs");
                        if (contentId is null)
                        {
                            AddError($"Failed to upload file: {file.FileName}");
                            await transaction.RollbackAsync();
                            return null;
                        }

                        var proofContent = new ProofContent
                        {
                            ProofId = proof.Id,
                            ContentId = contentId.Value,
                            CreatedUserId = userId.Value
                        };

                        await unitOfWork.ProofContentRepository().Add(proofContent);
                    }

                    await unitOfWork.SaveChanges();
                }

                await transaction.CommitAsync();
                await memberRankingService.RecalculateAsync(model.GroupId, userId.Value);
                return proof.Id;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                AddError(ex.Message);
                return null;
            }
        }

        public async Task<string?> DeleteAsync(long id)
        {
            var userId = userHelper.GetUserId();
            if (!userId.HasValue)
            {
                AddError("User not found.");
                return null;
            }

            var proof = await unitOfWork.DailyProofRepository()
                .GetAll()
                .Include(p => p.ProofContents!).ThenInclude(pc => pc.Content)
                .FirstOrDefaultAsync(p => p.Id == id
                    && p.UserId == userId.Value
                    && p.StatusCode != StatusConstants.DeletedStatusCode);

            if (proof is null)
            {
                AddError("Proof not found.");
                return null;
            }

            // Same day only
            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            if (proof.ProofDate != today)
            {
                AddError("You can only delete a proof on the same day it was submitted.");
                return null;
            }

            await using var transaction = unitOfWork.BeginTransaction();
            try
            {
                // Delete files from MinIO + DB
                if (proof.ProofContents?.Any() == true)
                {
                    foreach (var pc in proof.ProofContents)
                    {
                        await contentService.DeleteContentForImage(pc.ContentId);
                        await unitOfWork.ProofContentRepository().Delete(pc);
                    }
                }

                proof.StatusCode = StatusConstants.DeletedStatusCode;
                proof.ModifiedUserId = userId.Value;
                proof.ModifiedDateTime = DateTime.UtcNow;

                await unitOfWork.DailyProofRepository().Update(proof);
                await unitOfWork.SaveChanges();
                await transaction.CommitAsync();
                await memberRankingService.RecalculateAsync(proof.GroupId, userId.Value);
                return "Proof deleted successfully.";
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                AddError(ex.Message);
                return null;
            }
        }

        // ── helpers ──────────────────────────────────────────────────────────
        private static TypeAdapterConfig GetCustomConfig()
        {
            var config = new TypeAdapterConfig();
            config.NewConfig<Data.Entities.MainEntities.DailyProof, DailyProofDto>()
                .Map(dest => dest.GroupName, src => src.StudyGroup!.Name)
                .Map(dest => dest.Username, src => src.User!.Username)
                .Map(dest => dest.UserImageUrl, src => src.User!.Img != null
                    ? src.User.Img.FileId.GetFileUrl() : null)
                .Map(dest => dest.ProofTypeName, src => src.ProofType!.FullName)
                .Map(dest => dest.StatusName, src => src.Status!.FullName)
                .Map(dest => dest.ContentUrls, src => src.ProofContents != null
                    ? src.ProofContents
                        .Where(pc => pc.Content != null)
                        .Select(pc => pc.Content!.FileId.GetFileUrl())
                        .ToList()
                    : new List<string>())
                .Map(dest => dest.AverageRating, src => src.Ratings != null && src.Ratings.Any()
                    ? Math.Round((double)src.Ratings.Average(r => r.Rating), 2) : 0)
                .Map(dest => dest.RatingCount, src => src.Ratings != null
                    ? src.Ratings.Count : 0)
                .Map(dest => dest.CommentCount, src => src.Comments != null
                    ? src.Comments.Count(c => c.StatusCode != StatusConstants.DeletedStatusCode) : 0);
            return config;
        }
    
        

    public async Task<string?> AddFileAsync(long proofId, IFormFile file)
        {
            var userId = userHelper.GetUserId();
            if (!userId.HasValue)
            {
                AddError("User not found.");
                return null;
            }

            var proof = await unitOfWork.DailyProofRepository()
                .GetAll()
                .FirstOrDefaultAsync(p => p.Id == proofId
                    && p.UserId == userId.Value
                    && p.StatusCode != StatusConstants.DeletedStatusCode);

            if (proof is null)
            {
                AddError("Proof not found.");
                return null;
            }

            // Same day only
            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            if (proof.ProofDate != today)
            {
                AddError("You can only add files to a proof on the same day it was submitted.");
                return null;
            }

            var contentId = await contentService.CreateContentForImage(file, "proofs");
            if (contentId is null)
            {
                AddError("File upload failed.");
                return null;
            }

            var proofContent = new ProofContent
            {
                ProofId = proof.Id,
                ContentId = contentId.Value,
                CreatedUserId = userId.Value
            };

            await unitOfWork.ProofContentRepository().Add(proofContent);
            await unitOfWork.SaveChanges();

            return "File added successfully.";
        }

        public async Task<string?> RemoveFileAsync(long proofContentId)
        {
            var userId = userHelper.GetUserId();
            if (!userId.HasValue)
            {
                AddError("User not found.");
                return null;
            }

            var proofContent = await unitOfWork.ProofContentRepository()
                .GetAll(pc => pc.DailyProof!, pc => pc.Content!)
                .FirstOrDefaultAsync(pc => pc.Id == proofContentId
                    && pc.DailyProof!.UserId == userId.Value
                    && pc.DailyProof.StatusCode != StatusConstants.DeletedStatusCode);

            if (proofContent is null)
            {
                AddError("File not found.");
                return null;
            }

            // Same day only
            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            if (proofContent.DailyProof!.ProofDate != today)
            {
                AddError("You can only remove files from a proof on the same day it was submitted.");
                return null;
            }

            // Prevent removing the last file if there is no text content
            if (string.IsNullOrWhiteSpace(proofContent.DailyProof.TextContent))
            {
                var fileCount = await unitOfWork.ProofContentRepository()
                    .GetAll()
                    .CountAsync(pc => pc.ProofId == proofContent.ProofId);

                if (fileCount <= 1)
                {
                    AddError("Cannot remove the last file when there is no text content. Delete the proof instead.");
                    return null;
                }
            }

            await contentService.DeleteContentForImage(proofContent.ContentId);
            await unitOfWork.ProofContentRepository().Delete(proofContent);
            await unitOfWork.SaveChanges();

            return "File removed successfully.";
        } 
    }
   
}

