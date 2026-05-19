using Mapster;
using NockChat.Application.DTOs.Responses;
using NockChat.Domain.Entities;

namespace NockChat.Application.Common.Mappings
{
    public class MessageMappingConfig : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<Message, MessageResponse>()
                .Map(dest => dest.Username, src => src.ChatUser.Username)
                .Map(dest => dest.Id, src => src.Id)
                .Map(dest => dest.Text, src => src.Text)
                .Map(dest => dest.SentAt, src => src.SentAt);

            config.NewConfig<(Message Message, string Username), MessageResponse>()
                .Map(dest => dest.Id, src => src.Message.Id)
                .Map(dest => dest.Text, src => src.Message.Text)
                .Map(dest => dest.SentAt, src => src.Message.SentAt)
                .Map(dest => dest.Username, src => src.Username);
        }
    }
}