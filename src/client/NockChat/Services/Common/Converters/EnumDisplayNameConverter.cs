using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Avalonia.Data.Converters;

namespace NockChat.Services.Common.Converters
{
    /// <summary>
    /// Конвертер, который отображает значения перечислений (<see cref="Enum"/>)
    /// в человекочитаемые имена, используя атрибут <see cref="DisplayAttribute"/>.
    /// </summary>
    /// <remarks>
    /// Если у значения перечисления указан <see cref="DisplayAttribute"/>, возвращается его Name;
    /// иначе возвращается стандартное имя значения
    /// </remarks>
    public class EnumDisplayNameConverter : IValueConverter
    {
        /// <summary>
        /// Единичный экземпляр конвертера для использования в XAML
        /// </summary>
        public static readonly EnumDisplayNameConverter Instance = new();

        /// <summary>
        /// Преобразует значение перечисления в строку для отображения
        /// </summary>
        /// <param name="value">Значение перечисления</param>
        /// <param name="targetType">Целевой тип (обычно <see cref="string"/>)</param>
        /// <param name="culture">Информация о культуре</param>
        /// <returns>
        /// Имя из <see cref="DisplayAttribute"/> если задано, иначе стандартное имя значения перечисления
        /// </returns>
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value == null)
                return string.Empty;

            string valueStr = value.ToString() ?? string.Empty;

            FieldInfo? field = value.GetType().GetField(valueStr);
            if (field == null)
                return valueStr;

            var display = field.GetCustomAttributes(typeof(DisplayAttribute), false).Cast<DisplayAttribute>().FirstOrDefault();

            return display?.Name ?? valueStr;
        }

        /// <summary>
        /// Обратное преобразование не поддерживается
        /// </summary>
        /// <exception cref="NotImplementedException">Всегда выбрасывается</exception>
        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}