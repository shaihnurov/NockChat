using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NockChat.Domain.Entities;

namespace NockChat.Persistence.Configurations
{
    /// <summary>
    /// Конфигурация EF Core для сущности <see cref="ChatUser"/>
    /// </summary>
    public class ChatUserConfiguration : IEntityTypeConfiguration<ChatUser>
    {
        /// <inheritdoc/>
        public void Configure(EntityTypeBuilder<ChatUser> builder)
        {
            builder.HasKey(u => u.Id);

            builder.Property(u => u.Username).IsRequired().HasMaxLength(50);

            builder.HasIndex(u => new { u.RoomId, u.Username }).IsUnique();
        }
    }
}