using MediatR;
using NockChat.Application.Common.Exceptions;
using NockChat.Application.Common.Interfaces;
using NockChat.Application.Common.Pagination;
using NockChat.Application.DTOs.Responses;

namespace NockChat.Application.Messages.Queries
{
    public record GetMessagesQuery(int Page, int PageSize) : IRequest<PagedResult<MessageResponse>>;

    public class GetMessagesHandler(IMessageRepository repository, IUserContext userContext) : IRequestHandler<GetMessagesQuery, PagedResult<MessageResponse>>
    {
        public async Task<PagedResult<MessageResponse>> Handle(GetMessagesQuery request, CancellationToken cancellationToken)
        {
            var roomId = userContext.RoomId;

            if (roomId == 0)
                throw new ForbiddenException("Токен не содержит информации о комнате");

            var (messages, totalCount) = await repository.GetByRoomAsync(roomId, request.Page, request.PageSize, cancellationToken);

            var currentUserId = userContext.ChatUserId;
            var responseItems = messages.Select(m => new MessageResponse(
                Id: m.Id,
                Text: m.Text,
                Username: m.ChatUser?.Username ?? "Unknown",
                IsOwn: m.ChatUserId == currentUserId,
                SentAt: m.SentAt
            )).ToList();

            return new PagedResult<MessageResponse>
            {
                Items = responseItems,
                TotalCount = totalCount,
                Page = request.Page,
                PageSize = request.PageSize
            };
        }
    }
}