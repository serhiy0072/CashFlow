
using Application.Interfaces;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class BudgetRepository : IBudgetRepository
    {
        private readonly AppDbContext _context;

        public BudgetRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Budget>> GetAllAsync(string userId, DateTime? month)
        {
            var query = _context.Budgets.Include(b => b.Category).Where(b => b.UserId == userId);

            if (month.HasValue)
            {
                var startOfMonth = new DateTime(month.Value.Year, month.Value.Month, 1, 0, 0, 0, DateTimeKind.Utc);
                query = query.Where(b => b.Month == startOfMonth);
            }
            return await query.ToListAsync();
        }

        public async Task<Budget?> GetByIdAsync(Guid Id, string userId)
        {
            return await _context.Budgets.Include(b => b.Category).FirstOrDefaultAsync(b => b.Id == Id && b.UserId == userId);
        }

        public async Task<Budget> CreateAsync(Budget budget)
        {
            _context.Budgets.Add(budget);
            await _context.SaveChangesAsync();
            return budget;
        }

        public async Task DeleteAsync(Budget budget)
        {
            _context.Budgets.Remove(budget);
            await _context.SaveChangesAsync();
        }


        public async Task UpdateAsync(Budget budget)
        {
            _context.Budgets.Update(budget);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> ExistsAsync(string userId, Guid categoryId, DateTime month)
        {
            var startOfMonth = new DateTime(month.Year, month.Month, 1, 0, 0, 0, DateTimeKind.Utc);

            return await _context.Budgets.AnyAsync(b => b.UserId == userId
                                                     && b.CategoryId == categoryId
                                                     && b.Month == startOfMonth);
        }

        public async Task<decimal> GetSpentAmountAsync(string userId, Guid categoryId, DateTime month)
        {
            var startOfMonth = new DateTime(month.Year, month.Month, 1, 0, 0, 0, DateTimeKind.Utc);
            var endOfMonth = startOfMonth.AddMonths(1);

            return await _context.Transactions
                                .Where(t => t.UserId == userId
                                         && t.CategoryId == categoryId
                                         && t.Type == TransactionType.Expense
                                         && t.Date >= startOfMonth
                                         && t.Date < endOfMonth)
                                .SumAsync(t => t.Amount);
        }
    }
}
