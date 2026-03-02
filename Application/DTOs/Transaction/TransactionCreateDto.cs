using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Application.DTOs.Transaction
{
    /// <summary>
    /// Дані для створення нової транзакції
    /// </summary>
    public class TransactionCreateDto
    {
        /// <summary>
        /// Сума транзакції
        /// </summary>
        [Required(ErrorMessage = "Сума обов'язкова")]
        [Range(0.01, 999999999.99, ErrorMessage = "Сума має бути від 0.01 до 999999999.99")]
        public decimal Amount { get; set; }

        /// <summary>
        /// Тип: "Income" / "Expense"
        /// </summary>
        [Required(ErrorMessage = "Тип обов'язковий")]
        public string Type { get; set; } = string.Empty;

        /// <summary>
        /// Опис (необов'язково)
        /// </summary>
        [StringLength(500, ErrorMessage = "Максимум 500 символів")]
        public string? Description { get; set; }


        /// <summary>
        /// Дата транзакції
        /// </summary>
        [Required(ErrorMessage = "Дата обов'язкова")]
        public DateTime Date { get; set; }

        /// <summary>
        /// Id категорії
        /// </summary>
        [Required(ErrorMessage = "Категорія обов'язкова")]
        public Guid CategoryId { get; set; }
    }
}
