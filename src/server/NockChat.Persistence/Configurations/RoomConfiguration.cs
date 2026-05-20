using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NockChat.Domain.Entities;

namespace NockChat.Persistence.Configurations
{
    /// <summary>
    /// Конфигурация EF Core для сущности <see cref="Room"/>
    /// Настраивает каскадное удаление сообщений и пользователей при удалении комнаты
    /// </summary>
    public class RoomConfiguration : IEntityTypeConfiguration<Room>
    {
        /// <inheritdoc/>
        public void Configure(EntityTypeBuilder<Room> builder)
        {
            builder.HasKey(r => r.Id);

            builder.HasIndex(r => r.AccessCode).IsUnique();

            builder.Property(r => r.Name).IsRequired().HasMaxLength(100);

            builder.HasMany(r => r.Messages).WithOne(m => m.Room).HasForeignKey(m => m.RoomId).OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(r => r.ChatUsers).WithOne(u => u.Room).HasForeignKey(u => u.RoomId).OnDelete(DeleteBehavior.Cascade);
        }
    }
}