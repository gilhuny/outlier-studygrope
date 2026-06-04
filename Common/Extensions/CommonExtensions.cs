using StudyGroup.Api.Common.Constants;
using StudyGroup.Api.Common.Models.Shared;

namespace StudyGroup.Api.Common.Extensions
{
    public static class CommonExtensions
    {
        public static string GetFileUrl(this Guid id) =>
            CommonConstants.FileBaseUrl + id.ToString();

        public static string GetFileUrl(this string id) =>
            CommonConstants.FileBaseUrl + id;

        public static PaginationModel<T> ToPaginationModel<T>(
            this IQueryable<T> query, int page, int pageSize) where T : class
        {
            return new PaginationModel<T>
            {
                Rows = query.AsEnumerable(),
                PageIndex = page,
                PageSize = pageSize,
                Total = query.Count()
            };
        }
    }
}
