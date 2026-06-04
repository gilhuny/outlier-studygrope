namespace StudyGroup.Api.Common.Dtos.Main
{
    public class ProofRatingDto
    {
        public long Id { get; set; }
        public long ProofId { get; set; }
        public Guid RatedByUserId { get; set; }
        public string RatedByUsername { get; set; } = string.Empty;
        public short Rating { get; set; }
        public DateTime CreatedDateTime { get; set; }
    }
}
