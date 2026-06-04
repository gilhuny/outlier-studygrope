using StudyGroup.Api.Common.Extensions;
using StudyGroup.Api.Common.FilterOptions;

namespace StudyGroup.Api.Services.Main.Message.Base.QueryObjects
{
    public static class MessageFilterExtension
    {
        public static IQueryable<Data.Entities.MainEntities.Message> ApplyFilter(
            this IQueryable<Data.Entities.MainEntities.Message> query,
            MessageFilterOptions options)
        {
            if (options.ChatId.HasValue)
                query = query.Where(m => m.ChatId == options.ChatId);

            if (options.FromUserId.HasValue)
                query = query.Where(m => m.FromUserId == options.FromUserId);

            if (options.StatusCode.HasValue)
                query = query.Where(m => m.StatusCode == options.StatusCode);

            return query.ApplyBaseFilter(options);
        }
    }
}
