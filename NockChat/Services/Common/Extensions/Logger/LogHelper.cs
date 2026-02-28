using System;
using System.IO;

namespace NockChat.Services.Common.Extensions.Logger;

/// <summary>
/// Предоставляет вспомогательный метод для работы с файлами логов приложения
/// </summary>
/// <remarks>Класс LogHelper предлагает статические методы для поиска и извлечения файлов логов, сгенерированных приложением. 
/// Все члены являются потокобезопасными и могут использоваться без создания экземпляра класса</remarks>
public static class LogHelper
{
    /// <summary>
    /// Получает полный путь к самому последнему файлу логов в папке логов приложения, который соответствует шаблону "log-*.log"
    /// </summary>
    /// <remarks>Метод ищет файлы в папке, указанной <c>AppPaths.LogFolder</c>,
    /// с именами, соответствующими шаблону "log-*.log". Если присутствует несколько файлов, файл с наивысшим
    /// лексикографическим порядком считается самым последним. Этот метод не проверяет содержимое или временные метки файлов</remarks>
    /// <returns>Строка, содержащая полный путь к последнему файлу журнала, или <see langword="null"/>, если не найдено ни одного соответствующего файла журнала
    /// или папка журнала не существует</returns>
    public static string? GetLatestLogFile()
    {
        if (!Directory.Exists(AppPaths.LogFolder))
            return null;

        var files = Directory.GetFiles(AppPaths.LogFolder, "log-*.log");
        if (files.Length == 0)
            return null;

        Array.Sort(files, StringComparer.OrdinalIgnoreCase);
        return files[^1];
    }
}