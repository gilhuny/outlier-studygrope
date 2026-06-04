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
using StudyGroup.Api.Services.Main.MemberRanking.Base.Interfaces;
using StudyGroup.Api.Services.Main.MemberRanking.Base.QueryObjects;

namespace StudyGroup.Api.Services.Main.MemberRanking
{
    public class MemberRankingService(
            IUnitOfWork unitOfWork,
            IUserHelper userHelper)
            : StatusGenericHandler, IMemberRankingService
    {
        public async Task<PaginationModel<MemberRankingDto>> GetGroupRankingsAsync(
            Guid groupId, MemberRankingFilterOptions filterOptions)
        {
            var userId = userHelper.GetUserId();

            // Respect group privacy
            var group = await unitOfWork.StudyGroupRepository()
                .GetAll()
                .FirstOrDefaultAsync(g => g.Id == groupId
                    && g.StatusCode != StatusConstants.DeletedStatusCode);

            if (group is null)
            {
                AddError("Study group not found.");
                return new PaginationModel<MemberRankingDto>();
            }

            if (group.PrivacyCode == PrivacyConstants.PrivateCode)
            {
                var isMember = userId.HasValue && await unitOfWork.GroupMemberRepository()
                    .GetAll()
                    .AnyAsync(m => m.GroupId == groupId
                        && m.UserId == userId.Value
                        && m.StatusCode == StatusConstants.ActiveStatusCode);

                if (!isMember)
                {
                    AddError("You do not have access to this group's rankings.");
                    return new PaginationModel<MemberRankingDto>();
                }
            }

            var query = unitOfWork.MemberRankingRepository()
                .GetAll(r => r.User!, r => r.StudyGroup!)
                .Include(r => r.User!.Img)
                .AsNoTracking()
                .Where(r => r.GroupId == groupId)
                .OrderBy(r => r.RankPosition);

            return query
                .ApplyFilter(filterOptions)
                .MapToDtos<Data.Entities.MainEntities.MemberRanking, MemberRankingDto>(GetCustomConfig())
                .ToPaginationModel(filterOptions.Page, filterOptions.PageSize);
        }

        public async Task<MemberRankingDto?> GetMyRankingAsync(Guid groupId)
        {
            var userId = userHelper.GetUserId();
            if (!userId.HasValue)
            {
                AddError("User not found.");
                return null;
            }

            var ranking = await unitOfWork.MemberRankingRepository()
                .GetAll(r => r.User!, r => r.StudyGroup!)
                .Include(r => r.User!.Img)
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.GroupId == groupId
                    && r.UserId == userId.Value);

            if (ranking is null)
            {
                AddError("Ranking not found.");
                return null;
            }

            return ranking.MapToDto<Data.Entities.MainEntities.MemberRanking, MemberRankingDto>(GetCustomConfig());
        }

        // ── internal — called by DailyProofService and ProofRatingService ────
        public async Task RecalculateAsync(Guid groupId, Guid userId)
        {
            var ranking = await unitOfWork.MemberRankingRepository()
                .GetAll()
                .FirstOrDefaultAsync(r => r.GroupId == groupId && r.UserId == userId);

            if (ranking is null)
            {
                // First time — create ranking entry
                ranking = new Data.Entities.MainEntities.MemberRanking
                {
                    GroupId = groupId,
                    UserId = userId,
                    CreatedUserId = userId
                };
                await unitOfWork.MemberRankingRepository().Add(ranking);
            }

            // Recalculate stats from source data
            var proofs = await unitOfWork.DailyProofRepository()
                .GetAll()
                .Include(p => p.Ratings)
                .Where(p => p.GroupId == groupId
                    && p.UserId == userId
                    && p.StatusCode != StatusConstants.DeletedStatusCode)
                .OrderBy(p => p.ProofDate)
                .ToListAsync();

            var member = await unitOfWork.GroupMemberRepository()
                .GetAll()
                .FirstOrDefaultAsync(m => m.GroupId == groupId
                    && m.UserId == userId
                    && m.StatusCode == StatusConstants.ActiveStatusCode);

            ranking.UploadCount = proofs.Count;
            ranking.MissCount = member?.MissCount ?? 0;

            // Avg rating across all proofs
            var allRatings = proofs
                .Where(p => p.Ratings != null && p.Ratings.Any())
                .SelectMany(p => p.Ratings!)
                .ToList();

            ranking.AvgRating = allRatings.Any()
                ? Math.Round((decimal)allRatings.Average(r => r.Rating), 2)
                : 0;

            // Streak calculation
            var (currentStreak, longestStreak) = CalculateStreaks(proofs.Select(p => p.ProofDate).ToList());
            ranking.CurrentStreak = currentStreak;
            ranking.LongestStreak = longestStreak;

            // Score formula: (AvgRating×40%) + (CurrentStreak×30%) + (UploadCount×30%) − (MissCount×2)
            ranking.TotalScore = Math.Round(
                (ranking.AvgRating * 0.4m) +
                (ranking.CurrentStreak * 0.3m) +
                (ranking.UploadCount * 0.3m) -
                (ranking.MissCount * 2m), 2);

            ranking.LastCalculatedAt = DateTimeOffset.UtcNow;
            ranking.ModifiedUserId = userId;
            ranking.ModifiedDateTime = DateTime.UtcNow;

            await unitOfWork.MemberRankingRepository().Update(ranking);

            // Recalculate rank positions for the whole group
            await RecalculatePositionsAsync(groupId);

            await unitOfWork.SaveChanges();
        }

        // ── helpers ──────────────────────────────────────────────────────────
        private async Task RecalculatePositionsAsync(Guid groupId)
        {
            var rankings = await unitOfWork.MemberRankingRepository()
                .GetAll()
                .Where(r => r.GroupId == groupId)
                .OrderByDescending(r => r.TotalScore)
                .ToListAsync();

            for (int i = 0; i < rankings.Count; i++)
            {
                rankings[i].RankPosition = i + 1;
                await unitOfWork.MemberRankingRepository().Update(rankings[i]);
            }
        }

        private static (int current, int longest) CalculateStreaks(List<DateOnly> proofDates)
        {
            if (!proofDates.Any()) return (0, 0);

            var sorted = proofDates.Distinct().OrderBy(d => d).ToList();
            var today = DateOnly.FromDateTime(DateTime.UtcNow);

            int current = 0;
            int longest = 0;
            int streak = 1;

            for (int i = 1; i < sorted.Count; i++)
            {
                if (sorted[i] == sorted[i - 1].AddDays(1))
                    streak++;
                else
                    streak = 1;

                longest = Math.Max(longest, streak);
            }

            // Current streak — only counts if last proof was today or yesterday
            var lastProof = sorted.Last();
            if (lastProof == today || lastProof == today.AddDays(-1))
            {
                current = streak;
            }
            else
            {
                current = 0;
            }

            longest = Math.Max(longest, streak);

            return (current, longest);
        }

        private static TypeAdapterConfig GetCustomConfig()
        {
            var config = new TypeAdapterConfig();
            config.NewConfig<Data.Entities.MainEntities.MemberRanking, MemberRankingDto>()
                .Map(dest => dest.Username, src => src.User!.Username)
                .Map(dest => dest.UserImageUrl, src => src.User!.Img != null
                    ? src.User.Img.FileId.GetFileUrl() : null);
            return config;
        }
    }
}
