using StudyGroup.Api.Common.Extensions;
using StudyGroup.Api.Common.FilterOptions;

namespace StudyGroup.Api.Services.Main.MessageReadReceipt.Base.QueryObjects
{
    public static class MessageReadReceiptFilterExtension
    {
        public static IQueryable<Data.Entities.MainEntities.MessageReadReceipt> ApplyFilter(
            this IQueryable<Data.Entities.MainEntities.MessageReadReceipt> query,
            MessageReadReceiptFilterOptions options)
        {
            if (options.MessageId.HasValue)
                query = query.Where(r => r.MessageId == options.MessageId);

            if (options.UserId.HasValue)
                query = query.Where(r => r.UserId == options.UserId);

            return query.ApplyBaseFilter(options);
        }
    }
}
