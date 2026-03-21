using System;
using System.Collections.Generic;
using System.Text;
using Domain.Enums;

namespace Domain.Entities
{
    /// <summary>
    /// Фінансова транзакція (дохід/витрата)
    /// </summary>
    public class Transaction
    {
        /// <summary>
        /// Унікальний ідентифікатор
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Сума транзакції (завжди додатна)
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// Тип категорії
        /// </summary>
        public TransactionType Type { get; set; }

        /// <summary>
        /// Опис транзакції
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Дата транзакції
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// Зовнішній ключ на категорію
        /// </summary>
        public Guid CategoryId { get; set; }

        /// <summary>
        /// Навігаційна властивість: категорія цієї транзакції
        /// </summary>
        public Category Category { get; set; } = null!;

        /// <summary>
        /// ID користувача власника
        /// </summary>
        public string UserId { get; set; } = string.Empty;
    }
}
