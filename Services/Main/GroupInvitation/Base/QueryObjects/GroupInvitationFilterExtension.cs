using StudyGroup.Api.Common.Extensions;
using StudyGroup.Api.Common.FilterOptions;

namespace StudyGroup.Api.Services.Main.GroupInvitation.Base.QueryObjects
{
    public static class GroupInvitationFilterExtension
    {
        public static IQueryable<Data.Entities.MainEntities.GroupInvitation> ApplyFilter(
            this IQueryable<Data.Entities.MainEntities.GroupInvitation> query,
            GroupInvitationFilterOptions options)
        {
            if (options.GroupId.HasValue)
                query = query.Where(i => i.GroupId == options.GroupId);

            if (options.StatusCode.HasValue)
                query = query.Where(i => i.StatusCode == options.StatusCode);

            return query.ApplyBaseFilter(options);
        }
    }
}
