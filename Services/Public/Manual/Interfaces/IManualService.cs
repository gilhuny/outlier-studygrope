using StudyGroup.Api.Common.Models.Manual;

namespace StudyGroup.Api.Services.Public.Manual.Interfaces
{
    public interface IManualService
    {
        Task<SelectList<int>> CategoriesSelectAsync();
        Task<SelectList<int>> TagsSelectAsync();
        Task<SelectList<int>> PrivacySelectAsync();
        Task<SelectList<int>> JoinPoliciesSelectAsync();
        Task<SelectList<int>> StatusSelectAsync();
        Task<SelectList<int>> ProofTypesSelectAsync();
        Task<SelectList<int>> ChatTypesSelectAsync();
        Task<SelectList<int>> ContentTypesSelectAsync();
        Task<SelectList<int>> RolesSelectAsync();
        Task<SelectList<int>> StatesSelectAsync();
        Task<SelectList<int>> InfoTablesSelectAsync();
    }
}
