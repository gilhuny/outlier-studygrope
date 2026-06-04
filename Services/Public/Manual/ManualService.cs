using StudyGroup.Api.Common.Constants;
using StudyGroup.Api.Common.Models.Manual;
using StudyGroup.Api.Data.Repositories.Interfaces;
using StudyGroup.Api.Services.Common.Interfaces;
using StudyGroup.Api.Services.Public.Manual.Extensions;
using StudyGroup.Api.Services.Public.Manual.Interfaces;

namespace StudyGroup.Api.Services.Public.Manual
{
    public class ManualService(IUnitOfWork unitOfWork, IUserHelper userHelper) : IManualService
    {
        public async Task<SelectList<int>> CategoriesSelectAsync() =>
            unitOfWork.CategoryRepository().GetAll()
                .Where(c => c.StateCode == StateConstants.Active)
                .AsSelectList();

        public async Task<SelectList<int>> TagsSelectAsync() =>
            unitOfWork.TagRepository().GetAll()
                .Where(t => t.StateCode == StateConstants.Active)
                .AsSelectList();

        public async Task<SelectList<int>> PrivacySelectAsync() =>
            unitOfWork.GroupPrivacyRepository().GetAll()
                .Where(p => p.StateCode == StateConstants.Active)
                .AsSelectList();

        public async Task<SelectList<int>> JoinPoliciesSelectAsync() =>
            unitOfWork.JoinPolicyRepository().GetAll()
                .Where(j => j.StateCode == StateConstants.Active)
                .AsSelectList();

        public async Task<SelectList<int>> StatusSelectAsync() =>
            unitOfWork.StatusRepository().GetAll()
                .Where(s => s.StateCode == StateConstants.Active)
                .AsSelectList();

        public async Task<SelectList<int>> ProofTypesSelectAsync() =>
            unitOfWork.ProofTypeRepository().GetAll()
                .Where(p => p.StateCode == StateConstants.Active)
                .AsSelectList();

        public async Task<SelectList<int>> ChatTypesSelectAsync() =>
            unitOfWork.ChatTypeRepository().GetAll()
                .Where(c => c.StateCode == StateConstants.Active)
                .AsSelectList();

        public async Task<SelectList<int>> ContentTypesSelectAsync() =>
            unitOfWork.ContentTypeRepository().GetAll()
                .Where(c => c.StateCode == StateConstants.Active)
                .AsSelectList();

        public async Task<SelectList<int>> RolesSelectAsync()
        {
            if (userHelper.GetUserRoleCode() != RoleConstants.AdminRoleCode)
                return new SelectList<int>();

            return unitOfWork.RoleRepository().GetAll()
                .Where(r => r.StateCode == StateConstants.Active)
                .AsSelectList();
        }

        public async Task<SelectList<int>> StatesSelectAsync()
        {
            if (userHelper.GetUserRoleCode() != RoleConstants.AdminRoleCode)
                return new SelectList<int>();

            return unitOfWork.StateRepository().GetAll().AsSelectList();
        }

        public async Task<SelectList<int>> InfoTablesSelectAsync()
        {
            if (userHelper.GetUserRoleCode() != RoleConstants.AdminRoleCode)
                return new SelectList<int>();

            return unitOfWork.InfoTableRepository().GetAll()
                .Where(t => t.StateCode == StateConstants.Active)
                .AsSelectList();
        }
    }
}
