using System;
using System.Collections.Generic;
using System.Text;
using Domain.Enums;

namespace Domain.Entities
{
    /// <summary>
    /// Категорія доходів або витрат
    /// </summary>
    public class Category
    {
        /// <summary>
        /// Унікальний ідентифікатор
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Назва категорії
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Тип: Income або Expense
        /// </summary>
        public TransactionType Type { get; set; }

        /// <summary>
        /// Іконка
        /// </summary>
        public string? Icon { get; set; }

        /// <summary>
        /// Якщо null - системна категорія, доступна всім.
        /// Якщо є значення - кастомна категорія конкретного користувача.
        /// </summary>
        public string? UserId { get; set; }

        /// <summary>
        /// Навігаційна властивість: список транзакцій цієї категороії
        /// </summary>\
        public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
    }
}
