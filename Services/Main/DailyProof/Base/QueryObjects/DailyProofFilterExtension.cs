using StudyGroup.Api.Common.Extensions;
using StudyGroup.Api.Common.FilterOptions;
using StudyGroup.Api.Data.Entities.MainEntities;

namespace StudyGroup.Api.Services.Main.DailyProof.Base.QueryObjects
{
    public static class DailyProofFilterExtension
    {
        public static IQueryable<Data.Entities.MainEntities.DailyProof> ApplyFilter(
            this IQueryable<Data.Entities.MainEntities.DailyProof> query,
            DailyProofFilterOptions options)
        {
            if (options.GroupId.HasValue)
                query = query.Where(p => p.GroupId == options.GroupId);

            if (options.UserId.HasValue)
                query = query.Where(p => p.UserId == options.UserId);

            if (options.ProofTypeCode.HasValue)
                query = query.Where(p => p.ProofTypeCode == options.ProofTypeCode);

            if (options.StatusCode.HasValue)
                query = query.Where(p => p.StatusCode == options.StatusCode);

            if (options.FromDate.HasValue)
                query = query.Where(p => p.ProofDate >= options.FromDate);

            if (options.ToDate.HasValue)
                query = query.Where(p => p.ProofDate <= options.ToDate);

            return query.ApplyBaseFilter(options);
        }
    }
}
