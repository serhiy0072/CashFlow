using Application.DTOs.Transaction;
using Domain.Entities;

namespace Application.Interfaces
{
    public interface ITransactionRepository
    {
        Task<(List<Transaction> transactions, int totalCount)> GetFilteredAsync(string userId, TransactionsFilterDto filter);
        Task<List<Transaction>> GetAllAsync(string userId);
        Task<Transaction?> GetByIdAsync(Guid Id, string userId);

        Task<Transaction> CreateAsync(Transaction transaction);

        Task UpdateAsync(Transaction transaction);
        Task DeleteAsync(Transaction transaction);
    }
}
