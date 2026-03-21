using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Application.DTOs.Budget
{
    /// <summary>
    /// Дані для оновлення бюджету
    /// </summary>
    public class BudgetUpdateDto
    {
        /// <summary>
        /// Новий ліміт суми
        /// </summary>
        [Required(ErrorMessage = "Сума обов'язкова")]
        [Range(0.01, 999999999.99, ErrorMessage = "Сума має бути від 0.01 до 999999999.99")]
        public decimal Amount { get; set; }
    }
}
