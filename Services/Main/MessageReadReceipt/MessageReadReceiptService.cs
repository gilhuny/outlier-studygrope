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
using StudyGroup.Api.Services.Main.MessageReadReceipt.Base.Interfaces;
using StudyGroup.Api.Services.Main.MessageReadReceipt.Base.QueryObjects;

namespace StudyGroup.Api.Services.Main.MessageReadReceipt
{
    public class MessageReadReceiptService(
                IUnitOfWork unitOfWork,
                IUserHelper userHelper)
                : StatusGenericHandler, IMessageReadReceiptService
    {
        public async Task<PaginationModel<MessageReadReceiptDto>> GetAllAsync(
            MessageReadReceiptFilterOptions filterOptions)
        {
            var userId = userHelper.GetUserId();

            // MessageId is required — caller must always filter by message
            if (!filterOptions.MessageId.HasValue)
            {
                AddError("MessageId filter is required.");
                return new PaginationModel<MessageReadReceiptDto>();
            }

            // Verify message exists and requester is a participant of its chat
            var message = await unitOfWork.MessageRepository()
                .GetAll()
                .FirstOrDefaultAsync(m => m.Id == filterOptions.MessageId.Value
                    && m.StatusCode != StatusConstants.DeletedStatusCode);

            if (message is null)
            {
                AddError("Message not found.");
                return new PaginationModel<MessageReadReceiptDto>();
            }

            var isParticipant = userId.HasValue && await unitOfWork.UserChatRepository()
                .GetAll()
                .AnyAsync(uc => uc.ChatId == message.ChatId && uc.UserId == userId.Value);

            if (!isParticipant)
            {
                AddError("You do not have access to this message.");
                return new PaginationModel<MessageReadReceiptDto>();
            }

            var query = unitOfWork.MessageReadReceiptRepository()
                .GetAll(r => r.User!)
                .Include(r => r.User!.Img)
                .AsNoTracking();

            return query
                .ApplyFilter(filterOptions)
                .MapToDtos<Data.Entities.MainEntities.MessageReadReceipt, MessageReadReceiptDto>(GetCustomConfig())
                .ToPaginationModel(filterOptions.Page, filterOptions.PageSize);
        }

        public async Task<string?> MarkAsReadAsync(long messageId)
        {
            var userId = userHelper.GetUserId();
            if (!userId.HasValue)
            {
                AddError("User not found.");
                return null;
            }

            var message = await unitOfWork.MessageRepository()
                .GetAll()
                .FirstOrDefaultAsync(m => m.Id == messageId
                    && m.StatusCode != StatusConstants.DeletedStatusCode);

            if (message is null)
            {
                AddError("Message not found.");
                return null;
            }

            // Must be a participant of the chat
            var isParticipant = await unitOfWork.UserChatRepository()
                .GetAll()
                .AnyAsync(uc => uc.ChatId == message.ChatId && uc.UserId == userId.Value);

            if (!isParticipant)
            {
                AddError("You do not have access to this message.");
                return null;
            }

            // No duplicate receipts
            var alreadyRead = await unitOfWork.MessageReadReceiptRepository()
                .GetAll()
                .AnyAsync(r => r.MessageId == messageId && r.UserId == userId.Value);

            if (alreadyRead)
                return "Message already marked as read.";

            var receipt = new Data.Entities.MainEntities.MessageReadReceipt
            {
                MessageId = messageId,
                UserId = userId.Value,
                ReadAt = DateTimeOffset.UtcNow,
                CreatedUserId = userId.Value
            };

            await unitOfWork.MessageReadReceiptRepository().Add(receipt);
            await unitOfWork.SaveChanges();

            return "Message marked as read.";
        }

        public async Task<string?> MarkChatAsReadAsync(Guid chatId)
        {
            var userId = userHelper.GetUserId();
            if (!userId.HasValue)
            {
                AddError("User not found.");
                return null;
            }

            // Must be a participant
            var isParticipant = await unitOfWork.UserChatRepository()
                .GetAll()
                .AnyAsync(uc => uc.ChatId == chatId && uc.UserId == userId.Value);

            if (!isParticipant)
            {
                AddError("You do not have access to this chat.");
                return null;
            }

            // Fetch all unread active messages in the chat for this user
            var alreadyReadIds = await unitOfWork.MessageReadReceiptRepository()
                .GetAll()
                .Where(r => r.UserId == userId.Value)
                .Select(r => r.MessageId)
                .ToListAsync();

            var unreadMessages = await unitOfWork.MessageRepository()
                .GetAll()
                .Where(m => m.ChatId == chatId
                    && m.StatusCode != StatusConstants.DeletedStatusCode
                    && !alreadyReadIds.Contains(m.Id))
                .Select(m => m.Id)
                .ToListAsync();

            if (!unreadMessages.Any())
                return "No unread messages.";

            var now = DateTimeOffset.UtcNow;
            foreach (var messageId in unreadMessages)
            {
                await unitOfWork.MessageReadReceiptRepository().Add(
                    new Data.Entities.MainEntities.MessageReadReceipt
                    {
                        MessageId = messageId,
                        UserId = userId.Value,
                        ReadAt = now,
                        CreatedUserId = userId.Value
                    });
            }

            await unitOfWork.SaveChanges();

            return $"{unreadMessages.Count} message(s) marked as read.";
        }

        // ── helpers ──────────────────────────────────────────────────────────

        private static TypeAdapterConfig GetCustomConfig()
        {
            var config = new TypeAdapterConfig();
            config.NewConfig<Data.Entities.MainEntities.MessageReadReceipt, MessageReadReceiptDto>()
                .Map(dest => dest.Username, src => src.User!.Username)
                .Map(dest => dest.UserImageUrl, src => src.User!.Img != null
                    ? src.User.Img.FileId.GetFileUrl() : null);
            return config;
        }
    }
}
