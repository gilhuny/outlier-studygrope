using StatusGeneric;
using StudyGroup.Api.Common.Dtos.Main;
using StudyGroup.Api.Common.Models.Main.RateProof;

namespace StudyGroup.Api.Services.Main.ProofRating.Base.Interfaces
{
    public interface IProofRatingService : IStatusGeneric
    {
        Task<ProofRatingDto?> RateAsync(long proofId, RateProofModel model);
        Task<string?> UpdateRatingAsync(long proofId, RateProofModel model);
        Task<string?> DeleteRatingAsync(long proofId);
    }
}
