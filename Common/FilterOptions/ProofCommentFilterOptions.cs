namespace StudyGroup.Api.Common.FilterOptions
{
    public class ProofCommentFilterOptions : BaseFilterOptions
    {
        public long? ProofId { get; set; }
        public int? StatusCode { get; set; }
    }
}
