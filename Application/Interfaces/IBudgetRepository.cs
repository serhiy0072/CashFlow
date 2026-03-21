using Domain.Entities;

namespace Application.Interfaces
{
    public interface IBudgetRepository
    {
        Task<List<Budget>> GetAllAsync(string userId, DateTime? month);
        Task<Budget?> GetByIdAsync(Guid Id, string userId);
        Task<Budget> CreateAsync(Budget category);
        Task UpdateAsync(Budget category);
        Task DeleteAsync(Budget category);

        Task<bool> ExistsAsync(string userId, Guid categoryId, DateTime month);
        Task<decimal> GetSpentAmountAsync(string userId, Guid categoryId, DateTime month);
    }
}
