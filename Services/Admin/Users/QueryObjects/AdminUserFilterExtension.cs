using StudyGroup.Api.Common.Extensions;
using StudyGroup.Api.Common.FilterOptions;
using StudyGroup.Api.Data.Entities.MainEntities;

namespace StudyGroup.Api.Services.Admin.Users.QueryObjects
{
    public static class AdminUserFilterExtension
    {
        public static IQueryable<User> ApplyFilter(
            this IQueryable<User> query,
            AdminUserFilterOptions options)
        {
            if (options.StateCode.HasValue)
                query = query.Where(u => u.StateCode == options.StateCode);
            if (options.RoleCode.HasValue)
                query = query.Where(u => u.RoleCode == options.RoleCode);
            if (!string.IsNullOrEmpty(options.Username))
                query = query.Where(u => u.Username.Contains(options.Username));

            return query.ApplyBaseFilter(options);
        }
    }
}
