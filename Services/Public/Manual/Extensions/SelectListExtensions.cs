using StudyGroup.Api.Common.Models.Manual;
using StudyGroup.Api.Data.Entities.BaseEntities;
using StudyGroup.Api.Data.Entities.InfoEntities;

namespace StudyGroup.Api.Services.Public.Manual.Extensions
{
    public static class SelectListExtensions
    {
        public static SelectList<int> AsSelectList<T>(this IQueryable<T> source)
            where T : BaseInfoEntity
        {
            return new SelectList<int>(source.Select(a => new SelectListItem<int>
            {
                Value = a.Id,
                OrderCode = a.Code,
                Text = a.FullName
            }));
        }

        public static SelectList<int> AsSelectList(this IQueryable<State> source)
        {
            return new SelectList<int>(source.Select(a => new SelectListItem<int>
            {
                Value = a.Id,
                OrderCode = a.Code,
                Text = a.FullName
            }));
        }
    }
}
