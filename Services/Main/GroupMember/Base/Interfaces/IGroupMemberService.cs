using StatusGeneric;
using StudyGroup.Api.Common.Dtos.Main;
using StudyGroup.Api.Common.FilterOptions;
using StudyGroup.Api.Common.Models.Shared;

namespace StudyGroup.Api.Services.Main.GroupMember.Base.Interfaces
{
    public interface IGroupMemberService : IStatusGeneric
    {
        Task<PaginationModel<GroupMemberDto>> GetAllAsync(GroupMemberFilterOptions filterOptions);
        Task<GroupMemberDto?> GetByIdAsync(long id);
        Task<string?> JoinAsync(Guid groupId);
        Task<string?> LeaveAsync(Guid groupId);
        Task<string?> KickAsync(long memberId);
    }
}
