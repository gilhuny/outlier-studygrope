using StatusGeneric;
using StudyGroup.Api.Common.Models.Shared;


namespace StudyGroup.Api.Services.Main.Base.Interfaces
{

    public interface IBaseMainService<TId, TDto, TCreateModel, TUpdateModel, TFilterOptions>
        : IStatusGeneric
        where TDto : class
    {
        Task<PaginationModel<TDto>> GetAllAsync(TFilterOptions filterOptions);
        Task<TDto?> GetByIdAsync(TId id);
        Task<TId> CreateAsync(TCreateModel model);
        Task<string?> UpdateAsync(TId id, TUpdateModel model);
        Task<string?> DeleteAsync(TId id);
        Task<string?> ActivateAsync(TId id);
        Task<string?> DeactivateAsync(TId id);
    }
}
