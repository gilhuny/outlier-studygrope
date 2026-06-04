namespace StudyGroup.Api.Common.FilterOptions
{
    public class DailyProofFilterOptions : BaseFilterOptions
    {
        public Guid? GroupId { get; set; }
        public Guid? UserId { get; set; }
        public int? ProofTypeCode { get; set; }
        public int? StatusCode { get; set; }
        public DateOnly? FromDate { get; set; }
        public DateOnly? ToDate { get; set; }
    }
}
