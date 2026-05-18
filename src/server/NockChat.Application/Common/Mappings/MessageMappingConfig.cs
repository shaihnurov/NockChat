using Mapster;
using NockChat.Application.DTOs.Responses;
using NockChat.Domain.Entities;

namespace NockChat.Application.Common.Mappings
{
    internal class MessageMappingConfig : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<(Message Message, string Username), MessageResponse>()
                .Map(dest => dest.Id, src => src.Message.Id)
                .Map(dest => dest.Text, src => src.Message.Text)
                .Map(dest => dest.SentAt, src => src.Message.SentAt)
                .Map(dest => dest.Username, src => src.Username);
        }
    }
}