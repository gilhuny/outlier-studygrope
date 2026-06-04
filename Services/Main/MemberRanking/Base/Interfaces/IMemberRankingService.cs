using StatusGeneric;
using StudyGroup.Api.Common.Dtos.Main;
using StudyGroup.Api.Common.FilterOptions;
using StudyGroup.Api.Common.Models.Shared;

namespace StudyGroup.Api.Services.Main.MemberRanking.Base.Interfaces
{
    public interface IMemberRankingService : IStatusGeneric
    {
        public Task<PaginationModel<MemberRankingDto>> GetGroupRankingsAsync(Guid groupId, MemberRankingFilterOptions filterOptions);
        public Task<MemberRankingDto?> GetMyRankingAsync(Guid groupId);

        Task RecalculateAsync(Guid groupId, Guid userId);
    }
}
