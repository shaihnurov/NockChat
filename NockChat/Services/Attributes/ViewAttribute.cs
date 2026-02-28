using System;

namespace NockChat.Services.Attributes
{
    /// <summary>
    /// Атрибут для конфигурации страницы, используемой в системе навигации
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public sealed class ViewAttribute(string title) : Attribute
    {
        /// <summary>
        /// Заголовок страницы, который будет отображаться в UI
        /// </summary>
        public string Title { get; } = title;
    }
}