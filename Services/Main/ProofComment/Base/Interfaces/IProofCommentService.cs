using StatusGeneric;
using StudyGroup.Api.Common.Dtos.Main;
using StudyGroup.Api.Common.FilterOptions;
using StudyGroup.Api.Common.Models.Main.ProofComment;
using StudyGroup.Api.Common.Models.Shared;

namespace StudyGroup.Api.Services.Main.ProofComment.Base.Interfaces
{
    public interface IProofCommentService : IStatusGeneric
    {
        Task<PaginationModel<ProofCommentDto>> GetAllAsync(ProofCommentFilterOptions filterOptions);
        Task<ProofCommentDto?> CreateAsync(long proofId, CreateProofCommentModel model);
        Task<string?> UpdateAsync(long commentId, UpdateProofCommentModel model);
        Task<string?> DeleteAsync(long commentId);
    }
}
