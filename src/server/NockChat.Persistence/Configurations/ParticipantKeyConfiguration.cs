using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NockChat.Domain.Entities;

namespace NockChat.Persistence.Configurations
{
    /// <summary>
    /// Конфигурация EF Core для сущности <see cref="ParticipantKey"/>
    /// </summary>
    public class ParticipantKeyConfiguration : IEntityTypeConfiguration<ParticipantKey>
    {
        /// <inheritdoc/>
        public void Configure(EntityTypeBuilder<ParticipantKey> builder)
        {
            builder.HasKey(k => k.Id);

            builder.Property(k => k.EphemeralPublicKey).IsRequired().HasMaxLength(64);
            builder.Property(k => k.PublishedAt).IsRequired();

            builder.HasIndex(k => new
            {
                k.RoomId,
                k.ChatUserId
            }).IsUnique();

            builder.HasOne(k => k.Room).WithMany(r => r.ParticipantKeys).HasForeignKey(k => k.RoomId).OnDelete(DeleteBehavior.Cascade);
            builder.HasOne(k => k.ChatUser).WithOne(u => u.ParticipantKey).HasForeignKey<ParticipantKey>(k => k.ChatUserId).OnDelete(DeleteBehavior.Cascade);
        }
    }
}