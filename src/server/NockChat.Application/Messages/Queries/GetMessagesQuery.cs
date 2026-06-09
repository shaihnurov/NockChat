using System.Text.Json;
using MediatR;
using NockChat.Application.Common.Exceptions;
using NockChat.Application.Common.Interfaces;
using NockChat.Application.Common.Pagination;
using NockChat.Application.DTOs.Responses;

namespace NockChat.Application.Messages.Queries
{
    /// <summary>
    /// Запрос на получение постраничного списка сообщений текущей комнаты
    /// </summary>
    /// <param name="Page">Номер страницы</param>
    /// <param name="PageSize">Количество сообщений на странице</param>
    public record GetMessagesQuery(int Page, int PageSize) : IRequest<PagedResult<MessageResponse>>;

    /// <summary>
    /// Обработчик <see cref="GetMessagesQuery"/>. Возвращает сообщения комнаты,
    /// определяя принадлежность каждого сообщения текущему пользователю через <see cref="IUserContext"/> 
    /// </summary>
    public class GetMessagesHandler(IMessageRepository repository, IUserContext userContext) : IRequestHandler<GetMessagesQuery, PagedResult<MessageResponse>>
    {
        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        /// <summary>
        /// Загружает сообщения комнаты из репозитория и формирует постраничный ответ
        /// </summary>
        /// <param name="request">Параметры пагинации</param>
        /// <param name="cancellationToken">Токен</param>
        /// <returns>Постраничный список сообщений с флагом <c>IsOwn</c> для каждого элемента</returns>
        /// <exception cref="ForbiddenException">JWT-токен не содержит идентификатора комнаты</exception>
        public async Task<PagedResult<MessageResponse>> Handle(GetMessagesQuery request, CancellationToken cancellationToken)
        {
            var roomId = userContext.RoomId;

            if (roomId == 0)
                throw new ForbiddenException("Токен не содержит информации о комнате");

            var (messages, totalCount) = await repository.GetByRoomAsync(roomId, request.Page, request.PageSize, cancellationToken);

            var currentUserId = userContext.ChatUserId;

            var responseItems = messages.Select(m =>
            {
                var payload = JsonSerializer.Deserialize<EncryptedPayloadResponse>(m.EncryptedPayload, JsonOptions)
                    ?? throw new InvalidOperationException("Не удалось десериализовать payload");

                return new MessageResponse(
                    Id: m.Id,
                    SenderId: m.ChatUserId,
                    Username: m.ChatUser?.Username ?? "Unknown",
                    EncryptedPayload: payload,
                    IsOwn: m.ChatUserId == currentUserId,
                    SentAt: m.SentAt);
            }).ToList();

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