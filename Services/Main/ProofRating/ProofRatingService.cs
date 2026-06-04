using Mapster;
using Microsoft.EntityFrameworkCore;
using StatusGeneric;
using StudyGroup.Api.Common.Constants;
using StudyGroup.Api.Common.Dtos.Main;
using StudyGroup.Api.Common.Extensions;
using StudyGroup.Api.Common.Models.Main.RateProof;
using StudyGroup.Api.Data.Repositories.Interfaces;
using StudyGroup.Api.Services.Common.Interfaces;
using StudyGroup.Api.Services.Main.MemberRanking.Base.Interfaces;
using StudyGroup.Api.Services.Main.ProofRating.Base.Interfaces;

namespace StudyGroup.Api.Services.Main.ProofRating
{
    public class ProofRatingService(
        IUnitOfWork unitOfWork,
        IUserHelper userHelper,IMemberRankingService memberRankingService)
        : StatusGenericHandler, IProofRatingService
    {
        public async Task<ProofRatingDto?> RateAsync(long proofId, RateProofModel model)
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

            // Can't rate your own proof
            if (proof.UserId == userId.Value)
            {
                AddError("You cannot rate your own proof.");
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
                AddError("You must be an active member of the group to rate proofs.");
                return null;
            }

            // Already rated check
            var existingRating = await unitOfWork.ProofRatingRepository()
                .GetAll()
                .FirstOrDefaultAsync(r => r.ProofId == proofId
                    && r.RatedByUserId == userId.Value);

            if (existingRating is not null)
            {
                AddError("You have already rated this proof. Use update instead.");
                return null;
            }

            var rating = new Data.Entities.MainEntities.ProofRating
            {
                ProofId = proofId,
                RatedByUserId = userId.Value,
                Rating = model.Rating,
                CreatedUserId = userId.Value
            };

            await unitOfWork.ProofRatingRepository().Add(rating);
            await unitOfWork.SaveChanges();

            await memberRankingService.RecalculateAsync(proof.GroupId, proof.UserId);

            // Load with user for DTO
            var created = await unitOfWork.ProofRatingRepository()
                .GetAll(r => r.RatedBy!)
                .FirstOrDefaultAsync(r => r.Id == rating.Id);

            return created!.MapToDto<Data.Entities.MainEntities.ProofRating, ProofRatingDto>(GetCustomConfig());
        }

        public async Task<string?> UpdateRatingAsync(long proofId, RateProofModel model)
        {
            var userId = userHelper.GetUserId();
            if (!userId.HasValue)
            {
                AddError("User not found.");
                return null;
            }

            var rating = await unitOfWork.ProofRatingRepository()
                .GetAll()
                .FirstOrDefaultAsync(r => r.ProofId == proofId
                    && r.RatedByUserId == userId.Value);



            if (rating is null)
            {
                AddError("Rating not found.");
                return null;
            }

            rating.Rating = model.Rating;
            rating.ModifiedUserId = userId.Value;
            rating.ModifiedDateTime = DateTime.UtcNow;

            await unitOfWork.ProofRatingRepository().Update(rating);
            await unitOfWork.SaveChanges();

            var proof = await unitOfWork.DailyProofRepository()
    .GetAll()
    .FirstOrDefaultAsync(p => p.Id == rating.ProofId);
            await memberRankingService.RecalculateAsync(proof!.GroupId, proof.UserId);

            return "Rating updated successfully.";
        }

        public async Task<string?> DeleteRatingAsync(long proofId)
        {
            var userId = userHelper.GetUserId();
            if (!userId.HasValue)
            {
                AddError("User not found.");
                return null;
            }

            var rating = await unitOfWork.ProofRatingRepository()
                .GetAll()
                .FirstOrDefaultAsync(r => r.ProofId == proofId
                    && r.RatedByUserId == userId.Value);

            if (rating is null)
            {
                AddError("Rating not found.");
                return null;
            }

            await unitOfWork.ProofRatingRepository().Delete(rating);
            await unitOfWork.SaveChanges();


            var proof = await unitOfWork.DailyProofRepository()
    .GetAll()
    .FirstOrDefaultAsync(p => p.Id == rating.ProofId);
            await memberRankingService.RecalculateAsync(proof!.GroupId, proof.UserId);

            return "Rating deleted successfully.";
        }

        // ── helpers ──────────────────────────────────────────────────────────
        private static TypeAdapterConfig GetCustomConfig()
        {
            var config = new TypeAdapterConfig();
            config.NewConfig<Data.Entities.MainEntities.ProofRating, ProofRatingDto>()
                .Map(dest => dest.RatedByUsername, src => src.RatedBy!.Username);
            return config;
        }
    }
}
