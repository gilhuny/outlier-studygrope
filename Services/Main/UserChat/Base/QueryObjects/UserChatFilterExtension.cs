using StudyGroup.Api.Common.Extensions;
using StudyGroup.Api.Common.FilterOptions;

namespace StudyGroup.Api.Services.Main.UserChat.Base.QueryObjects
{
    public static class UserChatFilterExtension
    {
        public static IQueryable<Data.Entities.MainEntities.UserChat> ApplyFilter(
            this IQueryable<Data.Entities.MainEntities.UserChat> query,
            UserChatFilterOptions options)
        {
            if (options.ChatId.HasValue)
                query = query.Where(uc => uc.ChatId == options.ChatId);

            if (options.UserId.HasValue)
                query = query.Where(uc => uc.UserId == options.UserId);

            if (options.StatusCode.HasValue)
                query = query.Where(uc => uc.StatusCode == options.StatusCode);

            return query.ApplyBaseFilter(options);
        }
    }
}
