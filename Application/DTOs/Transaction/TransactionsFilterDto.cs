namespace Application.DTOs.Transaction
{
    /// <summary>
    /// Параметри фільтрації та пагінації транзакцій
    /// </summary>
    public class TransactionsFilterDto
    {
        /// <summary>
        /// Фільтр по типу "Income" або "Expense"
        /// </summary>
        public string? Type { get; set; }

        /// <summary>
        /// Фільтр по категорії
        /// </summary>
        public Guid? CategoryId { get; set; }

        /// <summary>
        /// Дата від (включно)
        /// </summary>
        public DateTime? DateFrom { get; set; }

        /// <summary>
        /// Мінімальна сума
        /// </summary>
        public decimal? MinAmount { get; set; }

        /// <summary>
        /// Максимальна сума
        /// </summary>
        public decimal? MaxAmount { get; set; }

        /// <summary>
        /// Номер сторінки (починається з 1)
        /// </summary>
        public int Page { get; set; } = 1;

        /// <summary>
        /// Кількість записів на сторінку
        /// </summary>
        public int PageSize { get; set; } = 10;

        /// <summary>
        /// Поле для сортування: "date", "amount"
        /// </summary>
        public string SortBy { get; set; } = "date";

        /// <summary>
        /// Напрямок сортування: "asc" або "desc"
        /// </summary>
        public string SortDirection { get; set; } = "desc";
    }
}
