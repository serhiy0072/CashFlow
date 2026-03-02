using System;
using System.Collections.Generic;
using System.Text;

namespace Application.DTOs.Transaction
{
    /// <summary>
    /// Дані транзакції у відповіді API
    /// </summary>
    public class TransactionResponseDto
    {
        /// <summary>
        /// Унікальний ідентифікатор
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Сума транзакції
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// Тип: "Income" / "Expense"
        /// </summary>
        public string Type { get; set; } = string.Empty;

        /// <summary>
        /// Опис 
        /// </summary>
        public string? Description { get; set; }


        /// <summary>
        /// Дата транзакції
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// Id категорії
        /// </summary>
        public Guid CategoryId { get; set; }

        /// <summary>
        /// Назва категорії
        /// </summary>
        public string CategoryName { get; set; } = string.Empty;
    }
}
