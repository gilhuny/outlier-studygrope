using StudyGroup.Api.Common.Extensions;
using StudyGroup.Api.Common.FilterOptions;

namespace StudyGroup.Api.Services.Main.GroupMember.Base.QueryObjects
{
    public static class GroupMemberFilterExtension
    {
        public static IQueryable<Data.Entities.MainEntities.GroupMember> ApplyFilter(
            this IQueryable<Data.Entities.MainEntities.GroupMember> query,
            GroupMemberFilterOptions options)
        {
            if (options.GroupId.HasValue)
                query = query.Where(m => m.GroupId == options.GroupId);

            if (options.StatusCode.HasValue)
                query = query.Where(m => m.StatusCode == options.StatusCode);

            if (!string.IsNullOrEmpty(options.Username))
                query = query.Where(m => m.User!.Username.Contains(options.Username));

            return query.ApplyBaseFilter(options);
        }
    }
}
