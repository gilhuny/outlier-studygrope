using Mapster;
using Microsoft.EntityFrameworkCore;
using StatusGeneric;
using StudyGroup.Api.Common.Constants;
using StudyGroup.Api.Common.Dtos.Main;
using StudyGroup.Api.Common.Extensions;
using StudyGroup.Api.Common.FilterOptions;
using StudyGroup.Api.Common.Models.Message;
using StudyGroup.Api.Common.Models.Shared;
using StudyGroup.Api.Data.Repositories.Interfaces;
using StudyGroup.Api.Services.Common.Interfaces;
using StudyGroup.Api.Services.Main.Message.Base.Interfaces;
using StudyGroup.Api.Services.Main.Message.Base.QueryObjects;

namespace StudyGroup.Api.Services.Main.Message
{
    public class MessageService(
                IUnitOfWork unitOfWork,
                IUserHelper userHelper)
                : StatusGenericHandler, IMessageService
    {
        public async Task<PaginationModel<MessageDto>> GetAllAsync(MessageFilterOptions filterOptions)
        {
            var userId = userHelper.GetUserId();

            // ChatId is required — caller must always filter by chat
            if (!filterOptions.ChatId.HasValue)
            {
                AddError("ChatId filter is required.");
                return new PaginationModel<MessageDto>();
            }

            // Only participants can read messages
            var isParticipant = userId.HasValue && await unitOfWork.UserChatRepository()
                .GetAll()
                .AnyAsync(uc => uc.ChatId == filterOptions.ChatId.Value
                    && uc.UserId == userId.Value);

            if (!isParticipant)
            {
                AddError("You do not have access to this chat.");
                return new PaginationModel<MessageDto>();
            }

            var query = unitOfWork.MessageRepository()
                .GetAll(m => m.FromUser!, m => m.Status!)
                .Include(m => m.FromUser!.Img)
                .Include(m => m.ReplyToMessage!)
                .Include(m => m.ReadReceipts!).ThenInclude(rr => rr.User)
                .AsNoTracking()
                .Where(m => m.StatusCode != StatusConstants.DeletedStatusCode);

            return query
                .ApplyFilter(filterOptions)
                .MapToDtos<Data.Entities.MainEntities.Message, MessageDto>(GetCustomConfig())
                .ToPaginationModel(filterOptions.Page, filterOptions.PageSize);
        }

        public async Task<MessageDto?> GetByIdAsync(long id)
        {
            var userId = userHelper.GetUserId();

            var message = await unitOfWork.MessageRepository()
                .GetAll(m => m.FromUser!, m => m.Status!)
                .Include(m => m.FromUser!.Img)
                .Include(m => m.ReplyToMessage!)
                .Include(m => m.ReadReceipts!).ThenInclude(rr => rr.User)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == id
                    && m.StatusCode != StatusConstants.DeletedStatusCode);

            if (message is null)
            {
                AddError("Message not found.");
                return null;
            }

            // Only participants can read messages
            var isParticipant = userId.HasValue && await unitOfWork.UserChatRepository()
                .GetAll()
                .AnyAsync(uc => uc.ChatId == message.ChatId
                    && uc.UserId == userId.Value);

            if (!isParticipant)
            {
                AddError("You do not have access to this chat.");
                return null;
            }

            return message.MapToDto<Data.Entities.MainEntities.Message, MessageDto>(GetCustomConfig());
        }

        public async Task<MessageDto?> SendAsync(SendMessageModel model)
        {
            var userId = userHelper.GetUserId();
            if (!userId.HasValue)
            {
                AddError("User not found.");
                return null;
            }

            // Chat must exist and be active
            var chat = await unitOfWork.ChatRepository()
                .GetAll()
                .FirstOrDefaultAsync(c => c.Id == model.ChatId
                    && c.StatusCode != StatusConstants.DeletedStatusCode);

            if (chat is null)
            {
                AddError("Chat not found.");
                return null;
            }

            // Must be a participant
            var isParticipant = await unitOfWork.UserChatRepository()
                .GetAll()
                .AnyAsync(uc => uc.ChatId == model.ChatId
                    && uc.UserId == userId.Value);

            if (!isParticipant)
            {
                AddError("You are not a participant of this chat.");
                return null;
            }

            // Validate reply target belongs to the same chat
            if (model.ReplyToMessageId.HasValue)
            {
                var replyTarget = await unitOfWork.MessageRepository()
                    .GetAll()
                    .AnyAsync(m => m.Id == model.ReplyToMessageId.Value
                        && m.ChatId == model.ChatId
                        && m.StatusCode != StatusConstants.DeletedStatusCode);

                if (!replyTarget)
                {
                    AddError("Reply target message not found in this chat.");
                    return null;
                }
            }

            var message = new Data.Entities.MainEntities.Message
            {
                ChatId = model.ChatId,
                FromUserId = userId.Value,
                MessageText = model.MessageText,
                ReplyToMessageId = model.ReplyToMessageId,
                StatusCode = StatusConstants.ActiveStatusCode,
                CreatedUserId = userId.Value
            };

            await unitOfWork.MessageRepository().Add(message);
            await unitOfWork.SaveChanges();

            // Load full message for DTO
            var created = await unitOfWork.MessageRepository()
                .GetAll(m => m.FromUser!, m => m.Status!)
                .Include(m => m.FromUser!.Img)
                .Include(m => m.ReplyToMessage!)
                .Include(m => m.ReadReceipts!)
                .FirstOrDefaultAsync(m => m.Id == message.Id);

            return created!.MapToDto<Data.Entities.MainEntities.Message, MessageDto>(GetCustomConfig());
        }

        public async Task<string?> UpdateAsync(long id, UpdateMessageModel model)
        {
            var userId = userHelper.GetUserId();
            if (!userId.HasValue)
            {
                AddError("User not found.");
                return null;
            }

            var message = await unitOfWork.MessageRepository()
                .GetAll()
                .FirstOrDefaultAsync(m => m.Id == id
                    && m.FromUserId == userId.Value
                    && m.StatusCode != StatusConstants.DeletedStatusCode);

            if (message is null)
            {
                AddError("Message not found.");
                return null;
            }

            message.MessageText = model.MessageText;
            message.StatusCode = StatusConstants.UpdatedStatusCode;
            message.ModifiedUserId = userId.Value;
            message.ModifiedDateTime = DateTime.UtcNow;

            await unitOfWork.MessageRepository().Update(message);
            await unitOfWork.SaveChanges();

            return "Message updated successfully.";
        }

        public async Task<string?> DeleteAsync(long id)
        {
            var userId = userHelper.GetUserId();
            if (!userId.HasValue)
            {
                AddError("User not found.");
                return null;
            }

            var message = await unitOfWork.MessageRepository()
                .GetAll()
                .FirstOrDefaultAsync(m => m.Id == id
                    && m.FromUserId == userId.Value
                    && m.StatusCode != StatusConstants.DeletedStatusCode);

            if (message is null)
            {
                AddError("Message not found.");
                return null;
            }

            message.StatusCode = StatusConstants.DeletedStatusCode;
            message.ModifiedUserId = userId.Value;
            message.ModifiedDateTime = DateTime.UtcNow;

            await unitOfWork.MessageRepository().Update(message);
            await unitOfWork.SaveChanges();

            return "Message deleted successfully.";
        }

        // ── helpers ──────────────────────────────────────────────────────────

        private static TypeAdapterConfig GetCustomConfig()
        {
            var config = new TypeAdapterConfig();
            config.NewConfig<Data.Entities.MainEntities.Message, MessageDto>()
                .Map(dest => dest.FromUsername, src => src.FromUser!.Username)
                .Map(dest => dest.FromUserImageUrl, src => src.FromUser!.Img != null
                    ? src.FromUser.Img.FileId.GetFileUrl() : null)
                .Map(dest => dest.StatusName, src => src.Status!.FullName)
                .Map(dest => dest.ReplyToMessageText, src => src.ReplyToMessage != null
                    ? src.ReplyToMessage.MessageText : null)
                .Map(dest => dest.ReadReceipts, src => src.ReadReceipts != null
                    ? src.ReadReceipts.Adapt<List<MessageReadReceiptDto>>()
                    : new List<MessageReadReceiptDto>());
            return config;
        }
    }
}
