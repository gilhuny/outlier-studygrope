using Microsoft.EntityFrameworkCore;
using StudyGroup.Api.Data.Entities.InfoEntities;
using StudyGroup.Api.Data.Entities.MainEntities;
using MainEntities_User = StudyGroup.Api.Data.Entities.MainEntities.User;
using User = StudyGroup.Api.Data.Entities.MainEntities.User;

namespace StudyGroup.Api.Data.Context;

public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

       
        public DbSet<State> States { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<GroupPrivacy> GroupPrivacies { get; set; }
        public DbSet<JoinPolicy> JoinPolicies { get; set; }
        public DbSet<Status> Statuses { get; set; }
        public DbSet<ContentType> ContentTypes { get; set; }
        public DbSet<ProofType> ProofTypes { get; set; }
        public DbSet<ChatType> ChatTypes { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<InfoTable> InfoTables { get; set; }

        
        public DbSet<User> Users { get; set; }
        public DbSet<Content> Contents { get; set; }
        public DbSet<StudyGroupEntity> StudyGroups { get; set; }
        public DbSet<StudyGroupTag> StudyGroupTags { get; set; }
        public DbSet<GroupMember> GroupMembers { get; set; }
        public DbSet<GroupInvitation> GroupInvitations { get; set; }
        public DbSet<DailyProof> DailyProofs { get; set; }
        public DbSet<ProofContent> ProofContents { get; set; }
        public DbSet<ProofRating> ProofRatings { get; set; }
        public DbSet<ProofComment> ProofComments { get; set; }
        public DbSet<MemberRanking> MemberRankings { get; set; }
        public DbSet<Chat> Chats { get; set; }
        public DbSet<UserChat> UserChats { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<MessageReadReceipt> MessageReadReceipts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            
            modelBuilder.Entity<State>().HasAlternateKey(s => s.Code);
            modelBuilder.Entity<Role>().HasAlternateKey(r => r.Code);
            modelBuilder.Entity<Category>().HasAlternateKey(c => c.Code);
            modelBuilder.Entity<GroupPrivacy>().HasAlternateKey(g => g.Code);
            modelBuilder.Entity<JoinPolicy>().HasAlternateKey(j => j.Code);
            modelBuilder.Entity<Status>().HasAlternateKey(s => s.Code);
            modelBuilder.Entity<ContentType>().HasAlternateKey(c => c.Code);
            modelBuilder.Entity<ProofType>().HasAlternateKey(p => p.Code);
            modelBuilder.Entity<ChatType>().HasAlternateKey(c => c.Code);
            modelBuilder.Entity<Tag>().HasAlternateKey(t => t.Code);

            
            modelBuilder.Entity<MainEntities_User>()
                .HasOne(u => u.Role)
                .WithMany()
                .HasForeignKey(u => u.RoleCode)
                .HasPrincipalKey(r => r.Code);

            modelBuilder.Entity<MainEntities_User>()
                .HasOne(u => u.State)
                .WithMany()
                .HasForeignKey(u => u.StateCode)
                .HasPrincipalKey(s => s.Code);

            
            modelBuilder.Entity<StudyGroupEntity>()
                .HasOne(g => g.Category)
                .WithMany()
                .HasForeignKey(g => g.CategoryCode)
                .HasPrincipalKey(c => c.Code);

            modelBuilder.Entity<StudyGroupEntity>()
                .HasOne(g => g.Privacy)
                .WithMany()
                .HasForeignKey(g => g.PrivacyCode)
                .HasPrincipalKey(p => p.Code);

            modelBuilder.Entity<StudyGroupEntity>()
                .HasOne(g => g.JoinPolicy)
                .WithMany()
                .HasForeignKey(g => g.JoinPolicyCode)
                .HasPrincipalKey(j => j.Code);

            modelBuilder.Entity<StudyGroupEntity>()
                .HasOne(g => g.Status)
                .WithMany()
                .HasForeignKey(g => g.StatusCode)
                .HasPrincipalKey(s => s.Code);

            
            modelBuilder.Entity<StudyGroupTag>()
                .HasOne(t => t.Tag)
                .WithMany()
                .HasForeignKey(t => t.TagCode)
                .HasPrincipalKey(t => t.Code);

            
            modelBuilder.Entity<GroupMember>()
                .HasOne(m => m.Status)
                .WithMany()
                .HasForeignKey(m => m.StatusCode)
                .HasPrincipalKey(s => s.Code);

            
            modelBuilder.Entity<GroupInvitation>()
                .HasOne(i => i.Status)
                .WithMany()
                .HasForeignKey(i => i.StatusCode)
                .HasPrincipalKey(s => s.Code);

            
            modelBuilder.Entity<GroupInvitation>()
                .HasOne(i => i.InvitedBy)
                .WithMany()
                .HasForeignKey(i => i.InvitedById);

            modelBuilder.Entity<GroupInvitation>()
                .HasOne(i => i.InvitedUser)
                .WithMany()
                .HasForeignKey(i => i.InvitedUserId);

            
            modelBuilder.Entity<DailyProof>()
                .HasOne(p => p.ProofType)
                .WithMany()
                .HasForeignKey(p => p.ProofTypeCode)
                .HasPrincipalKey(pt => pt.Code);

            modelBuilder.Entity<DailyProof>()
                .HasOne(p => p.Status)
                .WithMany()
                .HasForeignKey(p => p.StatusCode)
                .HasPrincipalKey(s => s.Code);

            
            modelBuilder.Entity<ProofComment>()
                .HasOne(c => c.Status)
                .WithMany()
                .HasForeignKey(c => c.StatusCode)
                .HasPrincipalKey(s => s.Code);

            
            modelBuilder.Entity<Chat>()
                .HasOne(c => c.ChatType)
                .WithMany()
                .HasForeignKey(c => c.ChatTypeCode)
                .HasPrincipalKey(ct => ct.Code);

            modelBuilder.Entity<Chat>()
                .HasOne(c => c.Status)
                .WithMany()
                .HasForeignKey(c => c.StatusCode)
                .HasPrincipalKey(s => s.Code);

            
            modelBuilder.Entity<UserChat>()
                .HasOne(uc => uc.Status)
                .WithMany()
                .HasForeignKey(uc => uc.StatusCode)
                .HasPrincipalKey(s => s.Code);

            
            modelBuilder.Entity<Message>()
                .HasOne(m => m.Status)
                .WithMany()
                .HasForeignKey(m => m.StatusCode)
                .HasPrincipalKey(s => s.Code);

            
            modelBuilder.Entity<StudyGroupEntity>()
                .HasIndex(g => g.CreatedDateTime)
                .HasDatabaseName("ix_study_groups_active_feed")
                .HasFilter("status_code = 2");

            
            modelBuilder.Entity<GroupMember>()
                .HasIndex(m => new { m.GroupId, m.UserId })
                .IsUnique();

            modelBuilder.Entity<GroupInvitation>()
                .HasIndex(i => new { i.GroupId, i.InvitedUserId })
                .IsUnique();

            modelBuilder.Entity<DailyProof>()
                .HasIndex(p => new { p.GroupId, p.UserId, p.ProofDate })
                .IsUnique();

            modelBuilder.Entity<ProofRating>()
                .HasIndex(r => new { r.ProofId, r.RatedByUserId })
                .IsUnique();

            modelBuilder.Entity<MemberRanking>()
                .HasIndex(r => new { r.GroupId, r.UserId })
                .IsUnique();

            modelBuilder.Entity<StudyGroupTag>()
                .HasIndex(t => new { t.GroupId, t.TagCode })
                .IsUnique();

            modelBuilder.Entity<MessageReadReceipt>()
                .HasIndex(r => new { r.MessageId, r.UserId })
                .IsUnique();
        }
    }