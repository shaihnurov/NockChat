using System.Collections.Generic;

namespace NockChat.Models.Pagination
{
    /// <summary>
    /// Представляет страницу результатов при постраничной загрузке данных
    /// </summary>
    /// <typeparam name="T">Тип элементов в коллекции</typeparam>
    public class PagedResult<T>
    {
        /// <summary>
        /// Коллекция элементов на текущей странице
        /// </summary>
        public List<T> Items { get; set; } = [];

        /// <summary>
        /// Номер текущей страницы
        /// </summary>
        public int Page { get; set; }

        /// <summary>
        /// Размер страницы
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// Общее количество элементов по всем страницам
        /// </summary>
        public int TotalCount { get; set; }

        /// <summary>
        /// Общее количество страниц
        /// </summary>
        public int TotalPages { get; set; }

        /// <summary>
        /// Указывает, существует ли следующая страница
        /// </summary>
        public bool HasNextPage { get; set; }

        /// <summary>
        /// Указывает, существует ли предыдущая страница
        /// </summary>
        public bool HasPreviousPage { get; set; }
    }
}