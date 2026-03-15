namespace Application.DTOs
{
    /// <summary>
    /// Обгортка для відповіді з пагінацією
    /// </summary>
    public class PagedResponseDto<T>
    {
        /// <summary>
        /// Дані поточної сторінки
        /// </summary>
        public List<T> Data { get; set; } = new();

        /// <summary>
        /// Загальна кількість записів
        /// </summary>
        public int TotalCount { get; set; }

        /// <summary>
        /// Поточна сторінка
        /// </summary>
        public int Page { get; set; }

        /// <summary>
        /// Кількість записів на сторінку
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// Кількість сторінок
        /// </summary>
        public int TotalPages { get; set; }
    }
}
