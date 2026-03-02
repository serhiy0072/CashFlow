using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Application.DTOs.Category
{
    /// <summary>
    /// Дані для створення нової категорії
    /// </summary>
    public class CategoryCreateDto
    {
        /// <summary>
        /// Назва категорії
        /// </summary>
        [Required(ErrorMessage = "Назва обов'язкова")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Назва має бути від 2 до 100 символів")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Тип: "Income" / "Expense"
        /// </summary>
        [Required(ErrorMessage = "Тип обов'язковий")]
        public string Type { get; set; } = string.Empty;

        /// <summary>
        /// Іконка (необов'язково)
        /// </summary>

        [StringLength(50, ErrorMessage = "Іконка максимум 50 символів")]
        public string? Icon { get; set; }
    }
}
