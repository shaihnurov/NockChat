using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NockChat.Domain.Entities;

namespace NockChat.Persistence.Configurations
{
    /// <summary>
    /// Конфигурация EF Core для сущности <see cref="Message"/>
    /// </summary>
    public class MessageConfiguration : IEntityTypeConfiguration<Message>
    {
        /// <inheritdoc/>
        public void Configure(EntityTypeBuilder<Message> builder)
        {
            builder.HasKey(m => m.Id);

            builder.HasIndex(m => new { m.RoomId, m.SentAt });

            builder.Property(m => m.Text).IsRequired().HasMaxLength(4000);
        }
    }
}