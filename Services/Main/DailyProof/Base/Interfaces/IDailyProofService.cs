using StatusGeneric;
using StudyGroup.Api.Common.Dtos.Main;
using StudyGroup.Api.Common.FilterOptions;
using StudyGroup.Api.Common.Models.Main.DailyProof;
using StudyGroup.Api.Common.Models.Shared;

namespace StudyGroup.Api.Services.Main.DailyProof.Base.Interfaces
{
    public interface IDailyProofService : IStatusGeneric
    {
        Task<PaginationModel<DailyProofDto>> GetAllAsync(DailyProofFilterOptions filterOptions);
        Task<DailyProofDto?> GetByIdAsync(long id);
        Task<long?> CreateAsync(CreateDailyProofModel model);
        Task<string?> DeleteAsync(long id);
        Task<string?> AddFileAsync(long proofId, IFormFile file);
        Task<string?> RemoveFileAsync(long proofContentId);
    }
}
