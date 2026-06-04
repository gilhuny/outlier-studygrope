using StudyGroup.Api.Common.Dtos.Main;
using StudyGroup.Api.Common.FilterOptions;
using StudyGroup.Api.Common.Models.Main.StudyGroup;
using StudyGroup.Api.Common.Models.Shared;
using StudyGroup.Api.Services.Main.Base.Interfaces;

namespace StudyGroup.Api.Services.Main.StudyGroup.Base.Interfaces
{
    public interface IStudyGroupService
        : IBaseMainService<Guid, StudyGroupDto, CreateStudyGroupModel, UpdateStudyGroupModel, StudyGroupFilterOptions>
    {
        Task<string?> UpdateCoverImageAsync(Guid id, IFormFile img);
        Task<PaginationModel<StudyGroupDto>> GetMyGroupsAsync(StudyGroupFilterOptions filterOptions);
    }
}
