using System;
using System.IO;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NockChat.Models.Settings;
using NockChat.Services.Common.Extensions;

namespace NockChat.Services.Common.DataStorage.Settings
{
    /// <summary>
    /// Реализация сервиса для управления настройками приложения
    /// </summary>
    public class SettingsService : ISettingsService
    {
        private readonly ILogger<SettingsService> _logger;

        /// <inheritdoc/>
        public AppSettingsModel Settings => _currentSettings;

        /// <summary>
        /// Путь к файлу настроек
        /// </summary>
        private readonly string _filePath;

        /// <summary>
        /// Текущие настройки приложения из файла
        /// </summary>
        private AppSettingsModel _currentSettings;

        /// <summary>
        /// Семафор для синхронизации доступа к файлу настроек.
        /// Обеспечивает потокобезопасность, предотвращая одновременную запись или чтение файла из разных потоков
        /// </summary>
        private readonly SemaphoreSlim _fileLock = new(1, 1);

        /// <summary>
        /// Настройки сериализации JSON.
        /// Экземпляр кэшируется как статический, чтобы избежать накладных расходов на повторную инициализацию
        /// и анализ типов (рефлексию) при каждой операции сохранения или загрузки
        /// </summary>
        private static readonly JsonSerializerOptions SerializerOptions = new()
        {
            WriteIndented = true
        };

        public SettingsService(ILogger<SettingsService> logger, string? filePath = null)
        {
            _logger = logger;
            _filePath = filePath ?? Path.Combine(AppPaths.DataFolder, "settings.json");

            if (string.IsNullOrWhiteSpace(_filePath))
                throw new ArgumentException("File path cannot be empty", nameof(filePath));

            _currentSettings = new AppSettingsModel();
        }

        /// <inheritdoc/>
        public async Task LoadAsync()
        {
            await _fileLock.WaitAsync();

            try
            {
                if (!File.Exists(_filePath))
                {
                    _currentSettings = new AppSettingsModel();
                    return;
                }

                using var stream = File.OpenRead(_filePath);
                _currentSettings = await JsonSerializer.DeserializeAsync<AppSettingsModel>(stream) ?? new AppSettingsModel();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading settings");
                _currentSettings = new AppSettingsModel();
            }
            finally
            {
                _fileLock.Release();
            }
        }

        /// <inheritdoc/>
        public async Task SaveAsync()
        {
            await _fileLock.WaitAsync();

            try
            {
                var directory = Path.GetDirectoryName(_filePath);
                if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                    Directory.CreateDirectory(directory);

                using var stream = File.Create(_filePath);
                await JsonSerializer.SerializeAsync(stream, _currentSettings, SerializerOptions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving settings");
                throw;
            }
            finally
            {
                _fileLock.Release();
            }
        }
    }
}