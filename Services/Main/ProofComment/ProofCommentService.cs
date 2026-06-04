using Mapster;
using Microsoft.EntityFrameworkCore;
using StatusGeneric;
using StudyGroup.Api.Common.Constants;
using StudyGroup.Api.Common.Dtos.Main;
using StudyGroup.Api.Common.Extensions;
using StudyGroup.Api.Common.FilterOptions;
using StudyGroup.Api.Common.Models.Main.ProofComment;
using StudyGroup.Api.Common.Models.Shared;
using StudyGroup.Api.Data.Repositories.Interfaces;
using StudyGroup.Api.Services.Common.Interfaces;
using StudyGroup.Api.Services.Main.ProofComment.Base.Interfaces;
using StudyGroup.Api.Services.Main.ProofComment.Base.QueryObjects;

namespace StudyGroup.Api.Services.Main.ProofComment
{
    public class ProofCommentService(
            IUnitOfWork unitOfWork,
            IUserHelper userHelper)
            : StatusGenericHandler, IProofCommentService
    {
        public async Task<PaginationModel<ProofCommentDto>> GetAllAsync(ProofCommentFilterOptions filterOptions)
        {
            var query = unitOfWork.ProofCommentRepository()
                .GetAll(c => c.User!, c => c.Status!)
                .Include(c => c.User!.Img)
                .AsNoTracking()
                .Where(c => c.StatusCode != StatusConstants.DeletedStatusCode);

            return query
                .ApplyFilter(filterOptions)
                .MapToDtos<Data.Entities.MainEntities.ProofComment, ProofCommentDto>(GetCustomConfig())
                .ToPaginationModel(filterOptions.Page, filterOptions.PageSize);
        }

        public async Task<ProofCommentDto?> CreateAsync(long proofId, CreateProofCommentModel model)
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
                    && p.StatusCode != StatusConstants.DeletedStatusCode);

            if (proof is null)
            {
                AddError("Proof not found.");
                return null;
            }

            // Must be an active member of the group
            var isMember = await unitOfWork.GroupMemberRepository()
                .GetAll()
                .AnyAsync(m => m.GroupId == proof.GroupId
                    && m.UserId == userId.Value
                    && m.StatusCode == StatusConstants.ActiveStatusCode);

            if (!isMember)
            {
                AddError("You must be an active member of the group to comment on proofs.");
                return null;
            }

            var comment = new Data.Entities.MainEntities.ProofComment
            {
                ProofId = proofId,
                UserId = userId.Value,
                CommentText = model.CommentText,
                StatusCode = StatusConstants.ActiveStatusCode,
                CreatedUserId = userId.Value
            };

            await unitOfWork.ProofCommentRepository().Add(comment);
            await unitOfWork.SaveChanges();

            // Load with user for DTO
            var created = await unitOfWork.ProofCommentRepository()
                .GetAll(c => c.User!, c => c.Status!)
                .Include(c => c.User!.Img)
                .FirstOrDefaultAsync(c => c.Id == comment.Id);

            return created!.MapToDto<Data.Entities.MainEntities.ProofComment, ProofCommentDto>(GetCustomConfig());
        }

        public async Task<string?> UpdateAsync(long commentId, UpdateProofCommentModel model)
        {
            var userId = userHelper.GetUserId();
            if (!userId.HasValue)
            {
                AddError("User not found.");
                return null;
            }

            var comment = await unitOfWork.ProofCommentRepository()
                .GetAll()
                .FirstOrDefaultAsync(c => c.Id == commentId
                    && c.UserId == userId.Value
                    && c.StatusCode != StatusConstants.DeletedStatusCode);

            if (comment is null)
            {
                AddError("Comment not found.");
                return null;
            }

            comment.CommentText = model.CommentText;
            comment.StatusCode = StatusConstants.UpdatedStatusCode;
            comment.ModifiedUserId = userId.Value;
            comment.ModifiedDateTime = DateTime.UtcNow;

            await unitOfWork.ProofCommentRepository().Update(comment);
            await unitOfWork.SaveChanges();

            return "Comment updated successfully.";
        }

        public async Task<string?> DeleteAsync(long commentId)
        {
            var userId = userHelper.GetUserId();
            if (!userId.HasValue)
            {
                AddError("User not found.");
                return null;
            }

            var comment = await unitOfWork.ProofCommentRepository()
                .GetAll()
                .FirstOrDefaultAsync(c => c.Id == commentId
                    && c.UserId == userId.Value
                    && c.StatusCode != StatusConstants.DeletedStatusCode);

            if (comment is null)
            {
                AddError("Comment not found.");
                return null;
            }

            comment.StatusCode = StatusConstants.DeletedStatusCode;
            comment.ModifiedUserId = userId.Value;
            comment.ModifiedDateTime = DateTime.UtcNow;

            await unitOfWork.ProofCommentRepository().Update(comment);
            await unitOfWork.SaveChanges();

            return "Comment deleted successfully.";
        }

        // ── helpers ──────────────────────────────────────────────────────────
        private static TypeAdapterConfig GetCustomConfig()
        {
            var config = new TypeAdapterConfig();
            config.NewConfig<Data.Entities.MainEntities.ProofComment, ProofCommentDto>()
                .Map(dest => dest.Username, src => src.User!.Username)
                .Map(dest => dest.StatusName, src => src.Status!.FullName)
                .Map(dest => dest.UserImageUrl, src => src.User!.Img != null
                    ? src.User.Img.FileId.GetFileUrl() : null);
            return config;
        }
    }
}
