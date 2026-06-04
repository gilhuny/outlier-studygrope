using StudyGroup.Api.Common.Extensions;
using StudyGroup.Api.Common.FilterOptions;

namespace StudyGroup.Api.Services.Main.MemberRanking.Base.QueryObjects
{
    public static class MemberRankingFilterExtension
    {
        public static IQueryable<Data.Entities.MainEntities.MemberRanking> ApplyFilter(
            this IQueryable<Data.Entities.MainEntities.MemberRanking> query,
            MemberRankingFilterOptions options)
        {
            if (options.GroupId.HasValue)
                query = query.Where(i => i.GroupId == options.GroupId);
            return query.ApplyBaseFilter(options);
        }
    }
}
