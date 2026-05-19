using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NockChat.Domain.Entities;

namespace NockChat.Persistence.Configurations
{
    public class MessageConfiguration : IEntityTypeConfiguration<Message>
    {
        public void Configure(EntityTypeBuilder<Message> builder)
        {
            builder.HasKey(m => m.Id);

            builder.HasIndex(m => new { m.RoomId, m.SentAt });

            builder.Property(m => m.Text)
                .IsRequired()
                .HasMaxLength(4000);
        }
    }
}