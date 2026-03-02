using System;
using System.Collections.Generic;
using System.Text;

namespace Application.DTOs.Category
{
    /// <summary>
    /// Дані категорії у відповіді API
    /// </summary>
    public class CategoryResponseDto
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
        /// Тип: "Income" / "Expense"
        /// </summary>
        public string Type { get; set; } = string.Empty;

        /// <summary>
        /// Іконка
        /// </summary>
        public string? Icon { get; set; }
    }
}
