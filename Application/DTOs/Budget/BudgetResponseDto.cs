using System;
using System.Collections.Generic;
using System.Text;

namespace Application.DTOs.Budget
{
    /// <summary>
    /// Дані бюджету у відповіді API
    /// </summary>
    public class BudgetResponseDto
    {
        /// <summary>
        /// Унікальний ідентифікатор
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Ліміт суми
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// Місяць бюджету
        /// </summary>
        public DateTime Month { get; set; }

        /// <summary>
        /// Id категорії
        /// </summary>
        public Guid CategoryId { get; set; }

        /// <summary>
        /// Назва категорії
        /// </summary>
        public string CategoryName { get; set; } = string.Empty;

        /// <summary>
        /// Фактично витрачено за цей місяць
        /// </summary>
        public decimal Spent { get; set; }

        /// <summary>
        /// Залишок (ліміт - витрачено)
        /// </summary>
        public decimal Remaining { get; set; }

        /// <summary>
        /// Чи перевищено бюджет
        /// </summary>
        public bool IsExceeded { get; set; }
    }
}
