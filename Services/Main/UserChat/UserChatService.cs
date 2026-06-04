using Mapster;
using Microsoft.EntityFrameworkCore;
using StatusGeneric;
using StudyGroup.Api.Common.Constants;
using StudyGroup.Api.Common.Dtos.Main;
using StudyGroup.Api.Common.Extensions;
using StudyGroup.Api.Common.FilterOptions;
using StudyGroup.Api.Common.Models.Shared;
using StudyGroup.Api.Data.Repositories.Interfaces;
using StudyGroup.Api.Services.Common.Interfaces;
using StudyGroup.Api.Services.Main.UserChat.Base.Interfaces;
using StudyGroup.Api.Services.Main.UserChat.Base.QueryObjects;

namespace StudyGroup.Api.Services.Main.UserChat
{
    public class UserChatService(
                IUnitOfWork unitOfWork,
                IUserHelper userHelper)
                : StatusGenericHandler, IUserChatService
    {
        public async Task<PaginationModel<UserChatDto>> GetParticipantsAsync(
            Guid chatId, UserChatFilterOptions filterOptions)
        {
            var userId = userHelper.GetUserId();

            // Only existing participants can see the participant list
            var isParticipant = userId.HasValue && await unitOfWork.UserChatRepository()
                .GetAll()
                .AnyAsync(uc => uc.ChatId == chatId && uc.UserId == userId.Value);

            if (!isParticipant)
            {
                AddError("You do not have access to this chat.");
                return new PaginationModel<UserChatDto>();
            }

            filterOptions.ChatId = chatId;

            var query = unitOfWork.UserChatRepository()
                .GetAll(uc => uc.User!, uc => uc.Status!)
                .Include(uc => uc.User!.Img)
                .AsNoTracking();

            return query
                .ApplyFilter(filterOptions)
                .MapToDtos<Data.Entities.MainEntities.UserChat, UserChatDto>(GetCustomConfig())
                .ToPaginationModel(filterOptions.Page, filterOptions.PageSize);
        }

        public async Task<string?> AddParticipantAsync(Guid chatId, Guid targetUserId)
        {
            var userId = userHelper.GetUserId();
            if (!userId.HasValue)
            {
                AddError("User not found.");
                return null;
            }

            var chat = await unitOfWork.ChatRepository()
                .GetAll()
                .FirstOrDefaultAsync(c => c.Id == chatId
                    && c.StatusCode != StatusConstants.DeletedStatusCode);

            if (chat is null)
            {
                AddError("Chat not found.");
                return null;
            }

            // Only group chats allow adding participants
            if (chat.ChatTypeCode != ChatTypeConstants.GroupCode)
            {
                AddError("Participants can only be added to group chats.");
                return null;
            }

            // Requester must be a participant
            var isParticipant = await unitOfWork.UserChatRepository()
                .GetAll()
                .AnyAsync(uc => uc.ChatId == chatId && uc.UserId == userId.Value);

            if (!isParticipant)
            {
                AddError("You are not a participant of this chat.");
                return null;
            }

            // Target user must exist and be active
            var targetUser = await unitOfWork.UserRepository()
                .GetAll()
                .FirstOrDefaultAsync(u => u.Id == targetUserId
                    && u.StateCode == StateConstants.Active);

            if (targetUser is null)
            {
                AddError("Target user not found.");
                return null;
            }

            // Already a participant check
            var alreadyParticipant = await unitOfWork.UserChatRepository()
                .GetAll()
                .AnyAsync(uc => uc.ChatId == chatId && uc.UserId == targetUserId);

            if (alreadyParticipant)
            {
                AddError("This user is already a participant of this chat.");
                return null;
            }

            var userChat = new Data.Entities.MainEntities.UserChat
            {
                ChatId = chatId,
                UserId = targetUserId,
                StatusCode = StatusConstants.ActiveStatusCode,
                CreatedUserId = userId.Value
            };

            await unitOfWork.UserChatRepository().Add(userChat);
            await unitOfWork.SaveChanges();

            return "Participant added successfully.";
        }

        public async Task<string?> RemoveParticipantAsync(Guid chatId, Guid targetUserId)
        {
            var userId = userHelper.GetUserId();
            if (!userId.HasValue)
            {
                AddError("User not found.");
                return null;
            }

            var chat = await unitOfWork.ChatRepository()
                .GetAll()
                .FirstOrDefaultAsync(c => c.Id == chatId
                    && c.StatusCode != StatusConstants.DeletedStatusCode);

            if (chat is null)
            {
                AddError("Chat not found.");
                return null;
            }

            // Only the chat creator can remove participants
            if (chat.CreatedUserId != userId.Value)
            {
                AddError("Only the chat creator can remove participants.");
                return null;
            }

            // Cannot remove yourself — use LeaveAsync instead
            if (targetUserId == userId.Value)
            {
                AddError("You cannot remove yourself. Use leave instead.");
                return null;
            }

            var userChat = await unitOfWork.UserChatRepository()
                .GetAll()
                .FirstOrDefaultAsync(uc => uc.ChatId == chatId && uc.UserId == targetUserId);

            if (userChat is null)
            {
                AddError("Participant not found.");
                return null;
            }

            userChat.StatusCode = StatusConstants.PassiveStatusCode;
            userChat.ModifiedUserId = userId.Value;
            userChat.ModifiedDateTime = DateTime.UtcNow;

            await unitOfWork.UserChatRepository().Update(userChat);
            await unitOfWork.SaveChanges();

            return "Participant removed successfully.";
        }

        public async Task<string?> LeaveAsync(Guid chatId)
        {
            var userId = userHelper.GetUserId();
            if (!userId.HasValue)
            {
                AddError("User not found.");
                return null;
            }

            var userChat = await unitOfWork.UserChatRepository()
                .GetAll()
                .FirstOrDefaultAsync(uc => uc.ChatId == chatId && uc.UserId == userId.Value);

            if (userChat is null)
            {
                AddError("You are not a participant of this chat.");
                return null;
            }

            // Creator cannot leave — they must delete the chat
            var chat = await unitOfWork.ChatRepository()
                .GetAll()
                .FirstOrDefaultAsync(c => c.Id == chatId);

            if (chat is not null && chat.CreatedUserId == userId.Value)
            {
                AddError("You are the creator of this chat. Delete the chat instead of leaving.");
                return null;
            }

            userChat.StatusCode = StatusConstants.PassiveStatusCode;
            userChat.ModifiedUserId = userId.Value;
            userChat.ModifiedDateTime = DateTime.UtcNow;

            await unitOfWork.UserChatRepository().Update(userChat);
            await unitOfWork.SaveChanges();

            return "Left the chat successfully.";
        }

        // ── helpers ──────────────────────────────────────────────────────────

        private static TypeAdapterConfig GetCustomConfig()
        {
            var config = new TypeAdapterConfig();
            config.NewConfig<Data.Entities.MainEntities.UserChat, UserChatDto>()
                .Map(dest => dest.Username, src => src.User!.Username)
                .Map(dest => dest.UserImageUrl, src => src.User!.Img != null
                    ? src.User.Img.FileId.GetFileUrl() : null)
                .Map(dest => dest.StatusName, src => src.Status!.FullName);
            return config;
        }
    }
}
