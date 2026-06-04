using Microsoft.EntityFrameworkCore.Storage;
using StudyGroup.Api.Data.Context;
using StudyGroup.Api.Data.Entities.InfoEntities;
using StudyGroup.Api.Data.Entities.MainEntities;
using StudyGroup.Api.Data.Repositories.Interfaces;

namespace StudyGroup.Api.Data.Repositories
{
    public class UnitOfWork(
        AppDbContext context,
        IBaseRepository<State> stateRepository,
        IBaseRepository<Role> roleRepository,
        IBaseRepository<Category> categoryRepository,
        IBaseRepository<GroupPrivacy> groupPrivacyRepository,
        IBaseRepository<JoinPolicy> joinPolicyRepository,
        IBaseRepository<Status> statusRepository,
        IBaseRepository<ContentType> contentTypeRepository,
        IBaseRepository<ProofType> proofTypeRepository,
        IBaseRepository<ChatType> chatTypeRepository,
        IBaseRepository<Tag> tagRepository,
        IBaseRepository<InfoTable> infoTableRepository,
        IBaseRepository<User> userRepository,
        IBaseRepository<Content> contentRepository,
        IBaseRepository<StudyGroupEntity> studyGroupRepository,
        IBaseRepository<StudyGroupTag> studyGroupTagRepository,
        IBaseRepository<GroupMember> groupMemberRepository,
        IBaseRepository<GroupInvitation> groupInvitationRepository,
        IBaseRepository<DailyProof> dailyProofRepository,
        IBaseRepository<ProofContent> proofContentRepository,
        IBaseRepository<ProofRating> proofRatingRepository,
        IBaseRepository<ProofComment> proofCommentRepository,
        IBaseRepository<MemberRanking> memberRankingRepository,
        IBaseRepository<Chat> chatRepository,
        IBaseRepository<UserChat> userChatRepository,
        IBaseRepository<Message> messageRepository,
        IBaseRepository<MessageReadReceipt> messageReadReceiptRepository)
        : IUnitOfWork
    {
        public IBaseRepository<State> StateRepository() =>
            stateRepository ?? new BaseRepository<State>(context);
        public IBaseRepository<Role> RoleRepository() =>
            roleRepository ?? new BaseRepository<Role>(context);
        public IBaseRepository<Category> CategoryRepository() =>
            categoryRepository ?? new BaseRepository<Category>(context);
        public IBaseRepository<GroupPrivacy> GroupPrivacyRepository() =>
            groupPrivacyRepository ?? new BaseRepository<GroupPrivacy>(context);
        public IBaseRepository<JoinPolicy> JoinPolicyRepository() =>
            joinPolicyRepository ?? new BaseRepository<JoinPolicy>(context);
        public IBaseRepository<Status> StatusRepository() =>
            statusRepository ?? new BaseRepository<Status>(context);
        public IBaseRepository<ContentType> ContentTypeRepository() =>
            contentTypeRepository ?? new BaseRepository<ContentType>(context);
        public IBaseRepository<ProofType> ProofTypeRepository() =>
            proofTypeRepository ?? new BaseRepository<ProofType>(context);
        public IBaseRepository<ChatType> ChatTypeRepository() =>
            chatTypeRepository ?? new BaseRepository<ChatType>(context);
        public IBaseRepository<Tag> TagRepository() =>
            tagRepository ?? new BaseRepository<Tag>(context);
        public IBaseRepository<InfoTable> InfoTableRepository() =>
            infoTableRepository ?? new BaseRepository<InfoTable>(context);
        public IBaseRepository<User> UserRepository() =>
            userRepository ?? new BaseRepository<User>(context);
        public IBaseRepository<Content> ContentRepository() =>
            contentRepository ?? new BaseRepository<Content>(context);
        public IBaseRepository<StudyGroupEntity> StudyGroupRepository() =>
            studyGroupRepository ?? new BaseRepository<StudyGroupEntity>(context);
        public IBaseRepository<StudyGroupTag> StudyGroupTagRepository() =>
            studyGroupTagRepository ?? new BaseRepository<StudyGroupTag>(context);
        public IBaseRepository<GroupMember> GroupMemberRepository() =>
            groupMemberRepository ?? new BaseRepository<GroupMember>(context);
        public IBaseRepository<GroupInvitation> GroupInvitationRepository() =>
            groupInvitationRepository ?? new BaseRepository<GroupInvitation>(context);
        public IBaseRepository<DailyProof> DailyProofRepository() =>
            dailyProofRepository ?? new BaseRepository<DailyProof>(context);
        public IBaseRepository<ProofContent> ProofContentRepository() =>
            proofContentRepository ?? new BaseRepository<ProofContent>(context);
        public IBaseRepository<ProofRating> ProofRatingRepository() =>
            proofRatingRepository ?? new BaseRepository<ProofRating>(context);
        public IBaseRepository<ProofComment> ProofCommentRepository() =>
            proofCommentRepository ?? new BaseRepository<ProofComment>(context);
        public IBaseRepository<MemberRanking> MemberRankingRepository() =>
            memberRankingRepository ?? new BaseRepository<MemberRanking>(context);
        public IBaseRepository<Chat> ChatRepository() =>
            chatRepository ?? new BaseRepository<Chat>(context);
        public IBaseRepository<UserChat> UserChatRepository() =>
            userChatRepository ?? new BaseRepository<UserChat>(context);
        public IBaseRepository<Message> MessageRepository() =>
            messageRepository ?? new BaseRepository<Message>(context);
        public IBaseRepository<MessageReadReceipt> MessageReadReceiptRepository() =>
            messageReadReceiptRepository ?? new BaseRepository<MessageReadReceipt>(context);

        public async Task SaveChanges() => await context.SaveChangesAsync();
        public IDbContextTransaction BeginTransaction() => context.Database.BeginTransaction();
        public IDbContextTransaction? CurrentTransaction() => context.Database.CurrentTransaction;
    }
}
