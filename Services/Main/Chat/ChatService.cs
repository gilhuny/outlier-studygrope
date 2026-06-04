using Mapster;
using Microsoft.EntityFrameworkCore;
using StatusGeneric;
using StudyGroup.Api.Common.Constants;
using StudyGroup.Api.Common.Dtos.Main;
using StudyGroup.Api.Common.Extensions;
using StudyGroup.Api.Common.FilterOptions;
using StudyGroup.Api.Common.Models.Main.Chat;
using StudyGroup.Api.Common.Models.Shared;
using StudyGroup.Api.Data.Entities.MainEntities;
using StudyGroup.Api.Data.Repositories.Interfaces;
using StudyGroup.Api.Services.Common.Interfaces;
using StudyGroup.Api.Services.Main.Chat.Base.Interface;
using StudyGroup.Api.Services.Main.Chat.Base.QueryObjects;

namespace StudyGroup.Api.Services.Main.Chat
{
    public class ChatService(
                IUnitOfWork unitOfWork,
                IUserHelper userHelper)
                : StatusGenericHandler, IChatService
    {
        public async Task<PaginationModel<ChatDto>> GetMyChatsAsync(ChatFilterOptions filterOptions)
        {
            var userId = userHelper.GetUserId();
            if (!userId.HasValue)
            {
                AddError("User not found.");
                return new PaginationModel<ChatDto>();
            }

            var query = unitOfWork.ChatRepository()
                .GetAll(c => c.ChatType!, c => c.Status!)
                .Include(c => c.StudyGroup!)
                .Include(c => c.UserChats!).ThenInclude(uc => uc.User).ThenInclude(u => u!.Img)
                .Include(c => c.Messages!.OrderByDescending(m => m.CreatedDateTime).Take(1))
                .AsNoTracking()
                .Where(c =>
                    c.StatusCode != StatusConstants.DeletedStatusCode &&
                    c.UserChats!.Any(uc => uc.UserId == userId.Value));

            return query
                .ApplyFilter(filterOptions)
                .MapToDtos<Data.Entities.MainEntities.Chat, ChatDto>(GetCustomConfig())
                .ToPaginationModel(filterOptions.Page, filterOptions.PageSize);
        }

        public async Task<ChatDto?> GetByIdAsync(Guid id)
        {
            var userId = userHelper.GetUserId();

            var chat = await unitOfWork.ChatRepository()
                .GetAll(c => c.ChatType!, c => c.Status!)
                .Include(c => c.StudyGroup!)
                .Include(c => c.UserChats!).ThenInclude(uc => uc.User).ThenInclude(u => u!.Img)
                .Include(c => c.Messages!.OrderByDescending(m => m.CreatedDateTime).Take(1))
                .AsNoTracking()
                .FirstOrDefaultAsync(c =>
                    c.Id == id &&
                    c.StatusCode != StatusConstants.DeletedStatusCode);

            if (chat is null)
            {
                AddError("Chat not found.");
                return null;
            }

            var isParticipant = userId.HasValue &&
                chat.UserChats!.Any(uc => uc.UserId == userId.Value);

            if (!isParticipant)
            {
                AddError("You do not have access to this chat.");
                return null;
            }

            return chat.MapToDto<Data.Entities.MainEntities.Chat, ChatDto>(GetCustomConfig());
        }

        public async Task<Guid?> CreateDirectChatAsync(Guid targetUserId)
        {
            var userId = userHelper.GetUserId();
            if (!userId.HasValue)
            {
                AddError("User not found.");
                return null;
            }

            if (userId.Value == targetUserId)
            {
                AddError("You cannot start a chat with yourself.");
                return null;
            }

            var targetUser = await unitOfWork.UserRepository()
                .GetAll()
                .FirstOrDefaultAsync(u => u.Id == targetUserId
                    && u.StateCode == StateConstants.Active);

            if (targetUser is null)
            {
                AddError("Target user not found.");
                return null;
            }

            // Return existing direct chat if one already exists between these two users
            var existingChat = await unitOfWork.ChatRepository()
                .GetAll()
                .Include(c => c.UserChats!)
                .FirstOrDefaultAsync(c =>
                    c.ChatTypeCode == ChatTypeConstants.DirectCode &&
                    c.StatusCode != StatusConstants.DeletedStatusCode &&
                    c.UserChats!.Any(uc => uc.UserId == userId.Value) &&
                    c.UserChats!.Any(uc => uc.UserId == targetUserId));

            if (existingChat is not null)
                return existingChat.Id;

            await using var transaction = unitOfWork.BeginTransaction();
            try
            {
                var chat = new Data.Entities.MainEntities.Chat
                {
                    Id = Guid.NewGuid(),
                    ChatTypeCode = ChatTypeConstants.DirectCode,
                    StatusCode = StatusConstants.ActiveStatusCode,
                    CreatedUserId = userId.Value
                };

                await unitOfWork.ChatRepository().Add(chat);
                await unitOfWork.SaveChanges();

                var participants = new[]
                {
                    new Data.Entities.MainEntities.UserChat { ChatId = chat.Id, UserId = userId.Value,  CreatedUserId = userId.Value },
                    new Data.Entities.MainEntities.UserChat { ChatId = chat.Id, UserId = targetUserId,  CreatedUserId = userId.Value }
                };

                foreach (var uc in participants)
                    await unitOfWork.UserChatRepository().Add(uc);

                await unitOfWork.SaveChanges();
                await transaction.CommitAsync();

                return chat.Id;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                AddError(ex.Message);
                return null;
            }
        }

        public async Task<Guid?> CreateGroupChatAsync(CreateGroupChatModel model)
        {
            var userId = userHelper.GetUserId();
            if (!userId.HasValue)
            {
                AddError("User not found.");
                return null;
            }

            // Must be an active member of the group
            var isMember = await unitOfWork.GroupMemberRepository()
                .GetAll()
                .AnyAsync(m => m.GroupId == model.GroupId
                    && m.UserId == userId.Value
                    && m.StatusCode == StatusConstants.ActiveStatusCode);

            if (!isMember)
            {
                AddError("You must be an active member of the group to create a group chat.");
                return null;
            }

            // Load all active members to add as participants
            var memberUserIds = await unitOfWork.GroupMemberRepository()
                .GetAll()
                .Where(m => m.GroupId == model.GroupId
                    && m.StatusCode == StatusConstants.ActiveStatusCode)
                .Select(m => m.UserId)
                .ToListAsync();

            await using var transaction = unitOfWork.BeginTransaction();
            try
            {
                var chat = new Data.Entities.MainEntities.Chat
                {
                    Id = Guid.NewGuid(),
                    ChatTypeCode = ChatTypeConstants.GroupCode,
                    Name = model.Name,
                    GroupId = model.GroupId,
                    StatusCode = StatusConstants.ActiveStatusCode,
                    CreatedUserId = userId.Value
                };

                await unitOfWork.ChatRepository().Add(chat);
                await unitOfWork.SaveChanges();

                foreach (var memberId in memberUserIds)
                    await unitOfWork.UserChatRepository().Add(new Data.Entities.MainEntities.UserChat
                    {
                        ChatId = chat.Id,
                        UserId = memberId,
                        CreatedUserId = userId.Value
                    });

                await unitOfWork.SaveChanges();
                await transaction.CommitAsync();

                return chat.Id;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                AddError(ex.Message);
                return null;
            }
        }

        public async Task<string?> DeleteAsync(Guid id)
        {
            var userId = userHelper.GetUserId();
            if (!userId.HasValue)
            {
                AddError("User not found.");
                return null;
            }

            var chat = await unitOfWork.ChatRepository()
                .GetAll()
                .Include(c => c.UserChats!)
                .FirstOrDefaultAsync(c => c.Id == id
                    && c.StatusCode != StatusConstants.DeletedStatusCode);

            if (chat is null)
            {
                AddError("Chat not found.");
                return null;
            }

            var isParticipant = chat.UserChats!.Any(uc => uc.UserId == userId.Value);
            if (!isParticipant)
            {
                AddError("You are not authorized to delete this chat.");
                return null;
            }

            chat.StatusCode = StatusConstants.DeletedStatusCode;
            chat.ModifiedUserId = userId.Value;
            chat.ModifiedDateTime = DateTime.UtcNow;

            await unitOfWork.ChatRepository().Update(chat);
            await unitOfWork.SaveChanges();

            return "Chat deleted successfully.";
        }

        // ── helpers ──────────────────────────────────────────────────────────

        private static TypeAdapterConfig GetCustomConfig()
        {
            var config = new TypeAdapterConfig();
            config.NewConfig<Data.Entities.MainEntities.Chat, ChatDto>()
                .Map(dest => dest.ChatTypeName, src => src.ChatType!.FullName)
                .Map(dest => dest.StatusName, src => src.Status!.FullName)
                .Map(dest => dest.GroupName, src => src.StudyGroup != null ? src.StudyGroup.Name : null)
                .Map(dest => dest.Participants, src => src.UserChats != null
                    ? src.UserChats
                        .Where(uc => uc.User != null)
                        .Select(uc => uc.Adapt<UserChatDto>())
                        .ToList()
                    : new List<UserChatDto>())
                .Map(dest => dest.LastMessageText, src => src.Messages != null && src.Messages.Any()
                    ? src.Messages.First().MessageText : null)
                .Map(dest => dest.LastMessageAt, src => src.Messages != null && src.Messages.Any()
                    ? src.Messages.First().CreatedDateTime : (DateTimeOffset?)null);
            return config;
        }
    }
}
