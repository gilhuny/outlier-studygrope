namespace StudyGroup.Api.Common.Models.Manual
{
    public class SelectList<TValue> : List<SelectListItem<TValue>>
    {
        public SelectList()
        {
        }

        public SelectList(IEnumerable<SelectListItem<TValue>> collection)
            : base(collection)
        { }
    }
}
