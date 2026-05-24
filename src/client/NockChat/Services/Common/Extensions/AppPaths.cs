using System;
using System.IO;

namespace NockChat.Services.Common.Extensions
{
    /// <summary>
    /// Статический класс, предоставляющий централизованный доступ к путям файловой системы приложения
    /// </summary>
    public static class AppPaths
    {
        /// <summary>
        /// Базовый путь к локальным данным приложения, специфичный для конкретной операционной системы
        /// </summary>
        private static readonly string BasePath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

        /// <summary>
        /// Корневой каталог приложения в хранилище пользователя
        /// </summary>
        public static readonly string BaseFolder = Path.Combine(BasePath, "NockChat");

        /// <summary>
        /// Путь к каталогу для хранения файлов журналов (логов)
        /// </summary>
        public static readonly string LogFolder = Path.Combine(BaseFolder, "logs");

        /// <summary>
        /// Путь к каталогу для основных данных приложения
        /// </summary>
        public static readonly string DataFolder = Path.Combine(BaseFolder, "cached");

        /// <summary>
        /// Путь к каталогу для кэширования или хранения изображений
        /// </summary>
        public static readonly string ImageFolder = Path.Combine(DataFolder, "images");

        static AppPaths()
        {
            EnsureDirectory(BaseFolder);
            EnsureDirectory(LogFolder);
            EnsureDirectory(DataFolder);
            EnsureDirectory(ImageFolder);
        }

        /// <summary>
        /// Проверяет существование директории и создает ее, если она отсутствует
        /// </summary>
        /// <param name="path">Полный путь к проверяемой директории</param>
        private static void EnsureDirectory(string path)
        {
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
        }
    }
}