using System;
using System.Collections.Generic;
using System.Text;

namespace Application.DTOs.Statistics
{
    /// <summary>
    /// Зведена статистика для дашборду
    /// </summary>
    public class DashboardDto
    {
        /// <summary>
        /// Загальний дохід за період
        /// </summary>
        public decimal TotalIncome { get; set; }

        /// <summary>
        /// Загальні витрати за період
        /// </summary>
        public decimal TotalExpense { get; set; }

        /// <summary>
        /// Баланс (дохід - витрати)
        /// </summary>
        public decimal Balance { get; set; }

        /// <summary>
        /// Витрати по категоріях
        /// </summary>
        public List<CategoryStatDto> ExpensesByCategory { get; set; } = new();

        /// <summary>
        /// Доходи по категоріях
        /// </summary>
        public List<CategoryStatDto> IncomesByCategory { get; set; } = new();
    }
}
