using Microsoft.EntityFrameworkCore.Storage;
using StudyGroup.Api.Data.Entities.InfoEntities;
using StudyGroup.Api.Data.Entities.MainEntities;

namespace StudyGroup.Api.Data.Repositories.Interfaces
{
    public interface IUnitOfWork
    {
        // Info repositories
        IBaseRepository<State> StateRepository();
        IBaseRepository<Role> RoleRepository();
        IBaseRepository<Category> CategoryRepository();
        IBaseRepository<GroupPrivacy> GroupPrivacyRepository();
        IBaseRepository<JoinPolicy> JoinPolicyRepository();
        IBaseRepository<Status> StatusRepository();
        IBaseRepository<ContentType> ContentTypeRepository();
        IBaseRepository<ProofType> ProofTypeRepository();
        IBaseRepository<ChatType> ChatTypeRepository();
        IBaseRepository<Tag> TagRepository();
        IBaseRepository<InfoTable> InfoTableRepository();

        // Main repositories
        IBaseRepository<User> UserRepository();
        IBaseRepository<Content> ContentRepository();
        IBaseRepository<StudyGroupEntity> StudyGroupRepository();
        IBaseRepository<StudyGroupTag> StudyGroupTagRepository();
        IBaseRepository<GroupMember> GroupMemberRepository();
        IBaseRepository<GroupInvitation> GroupInvitationRepository();
        IBaseRepository<DailyProof> DailyProofRepository();
        IBaseRepository<ProofContent> ProofContentRepository();
        IBaseRepository<ProofRating> ProofRatingRepository();
        IBaseRepository<ProofComment> ProofCommentRepository();
        IBaseRepository<MemberRanking> MemberRankingRepository();
        IBaseRepository<Chat> ChatRepository();
        IBaseRepository<UserChat> UserChatRepository();
        IBaseRepository<Message> MessageRepository();
        IBaseRepository<MessageReadReceipt> MessageReadReceiptRepository();

        Task SaveChanges();
        IDbContextTransaction BeginTransaction();
        IDbContextTransaction? CurrentTransaction();
    }
}
