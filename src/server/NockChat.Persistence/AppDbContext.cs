using Microsoft.EntityFrameworkCore;
using NockChat.Domain.Entities;

namespace NockChat.Persistence;

/// <summary>
/// Контекст базы данных приложения
/// </summary>
public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    /// <summary>
    /// Таблица комнат чата
    /// </summary>
    public DbSet<Room> Rooms => Set<Room>();

    /// <summary>
    /// Таблица сообщений
    /// </summary>
    public DbSet<Message> Messages => Set<Message>();

    /// <summary>
    /// Таблица пользователей чата
    /// </summary>
    public DbSet<ChatUser> ChatUsers => Set<ChatUser>();

    /// <summary>
    /// Таблица ключей шифрования участников
    /// </summary>
    public DbSet<ParticipantKey> ParticipantKeys => Set<ParticipantKey>();

    /// <inheritdoc/>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}