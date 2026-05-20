namespace NockChat.Application.Common.Pagination
{
    /// <summary>
    /// Обёртка для постраничного представления коллекции элементов
    /// </summary>
    /// <typeparam name="T">Тип элементов в коллекции</typeparam>
    public class PagedResult<T>
    {
        /// <summary>
        /// Элементы текущей страницы
        /// </summary>
        public IEnumerable<T> Items { get; init; } = [];

        /// <summary>
        /// Номер текущей страницы (начиная с 1)
        /// </summary>
        public int Page { get; init; }

        /// <summary>
        /// Максимальное количество элементов на странице
        /// </summary>
        public int PageSize { get; init; }

        /// <summary>
        /// Общее количество элементов во всей коллекции
        /// </summary>
        public int TotalCount { get; init; }

        /// <summary>
        /// Общее количество страниц, вычисленное на основе <see cref="TotalCount"/> и <see cref="PageSize"/>
        /// </summary>
        public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);

        /// <summary>
        /// <c>true</c>, если существует следующая страница
        /// </summary>
        public bool HasNextPage => Page < TotalPages;

        /// <summary>
        /// <c>true</c>, если существует предыдущая страница
        /// </summary>
        public bool HasPreviousPage => Page > 1;
    }
}