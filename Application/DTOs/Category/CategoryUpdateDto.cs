using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Application.DTOs.Category
{
    /// <summary>
    /// Дані для оновлення категорії 
    /// </summary>
    public class CategoryUpdateDto
    {
        /// <summary>
        /// Нова назва категорії
        /// </summary>
        [Required(ErrorMessage = "Назва обов'язкова")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Назва має бути від 2 до 100 символів")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Новий тип: "Income" / "Expense"
        /// </summary>
        [Required(ErrorMessage = "Тип обов'язковий")]
        public string Type { get; set; } = string.Empty;

        /// <summary>
        /// Нова іконка
        /// </summary>
        [StringLength(50, ErrorMessage = "Іконка максимум 50 символів")]
        public string? Icon { get; set; }
    }

}
