using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Application.DTOs.Budget
{
    /// <summary>
    /// Дані для створення бюджету
    /// </summary>
    public class BudgetCreateDto
    {
        /// <summary>
        /// Ліміт суми на місяць
        /// </summary>
        [Required(ErrorMessage = "Сума обов'язкова")]
        [Range(0.01, 999999999.99, ErrorMessage = "Сума має бути від 0.01 до 999999999.99")]
        public decimal Amount { get; set; }

        /// <summary>
        /// Ліміт суми на місяць
        /// </summary>
        [Required(ErrorMessage = "Місяць обов'язковий")]
        public DateTime Month { get; set; }

        /// <summary>
        /// Ліміт суми на місяць
        /// </summary>
        [Required(ErrorMessage = "Категорія обов'язкова")]
        public Guid CategoryId { get; set; }
    }
}
