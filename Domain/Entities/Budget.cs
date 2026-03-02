using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities
{
    /// <summary>
    /// Бюджет - місячний ліміт витрат на категорію
    /// </summary>
    public class Budget
    {
        /// <summary>
        /// Унікальний ідентифікатор
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Ліміт суми на місяць
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// Місяць бюджету
        /// </summary>
        public DateTime Month { get; set; }

        /// <summary>
        /// Зовнішній ключ на категорію
        /// </summary>
        public Guid CategoryId { get; set; }

        /// <summary>
        /// Навігаційна властивість: категорія
        /// </summary>
        public Category Category { get; set; } = null!;

        /// <summary>
        /// Id користувача-власника
        /// </summary>
        public string UserId { get; set; } = string.Empty;
    }
}
