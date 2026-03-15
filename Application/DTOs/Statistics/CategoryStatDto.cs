using System;
using System.Collections.Generic;
using System.Text;

namespace Application.DTOs.Statistics
{
    /// <summary>
    /// Статистика по одній категорії
    /// </summary>
    public class CategoryStatDto
    {
        /// <summary>
        /// Id категорії
        /// </summary>
        public Guid CategoryId { get; set; }

        /// <summary>
        /// Назва категорії
        /// </summary>
        public string CategoryName { get; set; } = string.Empty;

        /// <summary>
        /// Загальна сума по категорії
        /// </summary>
        public decimal Total { get; set; }

        /// <summary>
        /// Кількість транзакцій
        /// </summary>
        public int Count { get; set; }

    }
}
