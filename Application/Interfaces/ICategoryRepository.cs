using Domain.Entities;

namespace Application.Interfaces
{
    /// <summary>
    /// Інтерфейс репозиторію для роботи з категоріями
    /// </summary>
    public interface ICategoryRepository
    {
        Task<List<Category>> GetAllAsync(string userId);
        Task<Category?> GetByIdAsync(Guid Id, string userId);
        Task<Category> CreateAsync(Category category);
        Task UpdateAsync(Category category);
        Task DeleteAsync(Category category);
    }
}
