using StudyGroup.Api.Common.Extensions;
using StudyGroup.Api.Common.FilterOptions;
using StudyGroup.Api.Data.Entities.MainEntities;

namespace StudyGroup.Api.Services.Main.StudyGroup.Base.QueryObjects
{
    public static class StudyGroupFilterExtension
    {
        public static IQueryable<StudyGroupEntity> ApplyFilter(
            this IQueryable<StudyGroupEntity> query,
            StudyGroupFilterOptions options)
        {
            if (options.CategoryCode.HasValue)
                query = query.Where(g => g.CategoryCode == options.CategoryCode);

            if (options.PrivacyCode.HasValue)
                query = query.Where(g => g.PrivacyCode == options.PrivacyCode);

            if (options.JoinPolicyCode.HasValue)
                query = query.Where(g => g.JoinPolicyCode == options.JoinPolicyCode);

            if (options.StatusCode.HasValue)
                query = query.Where(g => g.StatusCode == options.StatusCode);

            if (options.TagCode.HasValue)
                query = query.Where(g => g.Tags!.Any(t => t.TagCode == options.TagCode));

            if (options.CreatorId.HasValue)
                query = query.Where(g => g.CreatorId == options.CreatorId);

            if (!string.IsNullOrEmpty(options.Name))
                query = query.Where(g => g.Name.Contains(options.Name));

            return query.ApplyBaseFilter(options);
        }
    }
}
