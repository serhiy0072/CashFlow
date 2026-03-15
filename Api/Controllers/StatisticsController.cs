using Application.DTOs.Statistics;
using Domain.Enums;
using Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Controllers
{
    /// <summary>
    /// Контролер статистики та дашборду
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class StatisticsController : ControllerBase
    {

        private readonly AppDbContext _context;
        public StatisticsController(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Отримати UserId
        /// </summary>
        private string GetUserId() => User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value;

        /// <summary>
        /// Зведена статистика за період
        /// Якщо період не вказано - за поточний місяць
        /// </summary>
        [HttpGet("dashboard")]
        public async Task<IActionResult> GetDashboard([FromQuery] DateTime? dateFrom, [FromQuery] DateTime? dateTo)
        {
            var userId = GetUserId();

            var from = dateFrom ?? new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1, 0, 0, 0, DateTimeKind.Utc);
            var to = dateTo ?? DateTime.UtcNow;

            var transactions = await _context.Transactions
                .Include(t => t.Category)
                .Where(t => t.UserId == userId && t.Date >= from && t.Date <= to)
                .ToListAsync();

            var totalIncome = transactions
                .Where(t => t.Type == TransactionType.Income)
                .Sum(t => t.Amount);

            var totalExpense = transactions
                .Where(t => t.Type == TransactionType.Expense)
                .Sum(t => t.Amount);

            var expensesByCategory = transactions
                .Where(t => t.Type == TransactionType.Expense)
                .GroupBy(t => new { t.CategoryId, t.Category.Name })
                .Select(g => new CategoryStatDto
                {
                    CategoryId = g.Key.CategoryId,
                    CategoryName = g.Key.Name,
                    Total = g.Sum(t => t.Amount),
                    Count = g.Count()
                })
                .OrderByDescending(c => c.Total)
                .ToList();

            var incomesByCategory = transactions
                .Where(t => t.Type == TransactionType.Income)
                .GroupBy(t => new { t.CategoryId, t.Category.Name })
                .Select(g => new CategoryStatDto
                {
                    CategoryId = g.Key.CategoryId,
                    CategoryName = g.Key.Name,
                    Total = g.Sum(t => t.Amount),
                    Count = g.Count()
                })
                .OrderByDescending(c => c.Total)
                .ToList();

            var dashboard = new DashboardDto
            {
                TotalIncome = totalIncome,
                TotalExpense = totalExpense,
                Balance = totalIncome - totalExpense,
                ExpensesByCategory = expensesByCategory,
                IncomesByCategory = incomesByCategory
            };

            return Ok(dashboard);
        }
    }
}
