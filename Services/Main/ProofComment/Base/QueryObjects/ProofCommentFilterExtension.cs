using StudyGroup.Api.Common.Extensions;
using StudyGroup.Api.Common.FilterOptions;

namespace StudyGroup.Api.Services.Main.ProofComment.Base.QueryObjects
{
    public static class ProofCommentFilterExtension
    {
        public static IQueryable<Data.Entities.MainEntities.ProofComment> ApplyFilter(
            this IQueryable< Data.Entities.MainEntities.ProofComment> query,
            ProofCommentFilterOptions options)
        {
            if (options.ProofId.HasValue)
                query = query.Where(c => c.ProofId == options.ProofId);

            if (options.StatusCode.HasValue)
                query = query.Where(c => c.StatusCode == options.StatusCode);

            return query.ApplyBaseFilter(options);
        }
    }
}
