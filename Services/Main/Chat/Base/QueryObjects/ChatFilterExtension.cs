using StudyGroup.Api.Common.Extensions;
using StudyGroup.Api.Common.FilterOptions;

namespace StudyGroup.Api.Services.Main.Chat.Base.QueryObjects
{
    public static class ChatFilterExtension
    {
        public static IQueryable<Data.Entities.MainEntities.Chat> ApplyFilter(
            this IQueryable<Data.Entities.MainEntities.Chat> query,
            ChatFilterOptions options)
        {
            if (options.ChatTypeCode.HasValue)
                query = query.Where(c => c.ChatTypeCode == options.ChatTypeCode);

            if (options.GroupId.HasValue)
                query = query.Where(c => c.GroupId == options.GroupId);

            if (options.StatusCode.HasValue)
                query = query.Where(c => c.StatusCode == options.StatusCode);

            return query.ApplyBaseFilter(options);
        }
    }
}
