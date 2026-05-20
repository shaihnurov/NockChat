using Mapster;
using NockChat.Application.DTOs.Responses;
using NockChat.Domain.Entities;

namespace NockChat.Application.Common.Mappings
{
    /// <summary>
    /// Конфигурация маппинга Mapster для сущности <see cref="ChatUser"/>
    /// </summary>
    public class RoomUsersMappingConfig : IRegister
    {
        /// <inheritdoc/>
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<ChatUser, RoomUsersResponse>()
                .Map(dest => dest.Username, src => src.Username)
                .Map(dest => dest.JoinedAt, src => src.JoinedAt);
        }
    }
}