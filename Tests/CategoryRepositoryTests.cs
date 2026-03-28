using Domain.Entities;
using Domain.Enums;
using Infrastructure.Data;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Tests
{
    public class CategoryRepositoryTests
    {
        // ---------------------------------------------------------------
        // Допоміжні методи
        // ---------------------------------------------------------------

        /// <summary>
        /// Створює свіжий InMemory DbContext для кожного тесту
        /// </summary>
        private AppDbContext CreateContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            return new AppDbContext(options);
        }

        private async Task<AppDbContext> CreateContextWithData()
        {
            var context = CreateContext();

            context.Categories.AddRange(
                new Category
                {
                    Id = Guid.Parse("aaaa0000-0000-0000-0000-000000000001"),
                    Name = "Їжа",
                    Type = TransactionType.Expense,
                    UserId = "user1"
                },
                new Category
                {
                    Id = Guid.Parse("aaaa0000-0000-0000-0000-000000000002"),
                    Name = "Зарплата",
                    Type = TransactionType.Income,
                    UserId = "user1"
                },
                new Category
                {
                    Id = Guid.Parse("aaaa0000-0000-0000-0000-000000000003"),
                    Name = "Транспорт",
                    Type = TransactionType.Expense,
                    UserId = "user2"
                }
            );

            await context.SaveChangesAsync();
            return context;
        }

        // ---------------------------------------------------------------
        // GetAllAsync
        // ---------------------------------------------------------------

        [Fact]
        public async Task GetAllAsync_ReturnsOnlyUserCategories()
        {
            var context = await CreateContextWithData();
            var repository = new CategoryRepository(context);

            var result = await repository.GetAllAsync("user1");

            Assert.Equal(2, result.Count);
            Assert.All(result, c => Assert.Equal("user1", c.UserId));
        }

        // ---------------------------------------------------------------
        // GetByIdAsync
        // ---------------------------------------------------------------

        [Fact]
        public async Task GetByIdAsync_ExistingCategory_ReturnsCategory()
        {
            var context = await CreateContextWithData();
            var repository = new CategoryRepository(context);

            var result = await repository.GetByIdAsync(
                Guid.Parse("aaaa0000-0000-0000-0000-000000000001"), "user1");

            Assert.NotNull(result);
            Assert.Equal("Їжа", result.Name);
        }

        [Fact]
        public async Task GetByIdAsync_OtherUserCategory_ReturnsNull()
        {
            // user2 намагається отримати категорію user1 — повинен отримати null
            var context = await CreateContextWithData();
            var repository = new CategoryRepository(context);

            var result = await repository.GetByIdAsync(
                Guid.Parse("aaaa0000-0000-0000-0000-000000000001"), "user2");

            Assert.Null(result);
        }

        // ---------------------------------------------------------------
        // CreateAsync / UpdateAsync / DeleteAsync
        // ---------------------------------------------------------------

        [Fact]
        public async Task CreateAsync_AddsCategory_ReturnsCreatedCategory()
        {
            var context = CreateContext();
            var repository = new CategoryRepository(context);

            var category = new Category
            {
                Id = Guid.NewGuid(),
                Name = "Розваги",
                Type = TransactionType.Expense,
                UserId = "user1"
            };

            var result = await repository.CreateAsync(category);

            Assert.NotNull(result);
            Assert.Equal("Розваги", result.Name);
            Assert.Equal(1, await context.Categories.CountAsync());
        }

        [Fact]
        public async Task UpdateAsync_ChangesCategory_SavesChanges()
        {
            var context = await CreateContextWithData();
            var repository = new CategoryRepository(context);

            var category = await repository.GetByIdAsync(
                Guid.Parse("aaaa0000-0000-0000-0000-000000000001"), "user1");

            category!.Name = "Продукти";
            await repository.UpdateAsync(category);

            var updated = await repository.GetByIdAsync(
                Guid.Parse("aaaa0000-0000-0000-0000-000000000001"), "user1");

            Assert.Equal("Продукти", updated!.Name);
        }

        [Fact]
        public async Task DeleteAsync_RemovesCategory_CategoryNotInDatabase()
        {
            var context = await CreateContextWithData();
            var repository = new CategoryRepository(context);

            var category = await repository.GetByIdAsync(
                Guid.Parse("aaaa0000-0000-0000-0000-000000000001"), "user1");

            await repository.DeleteAsync(category!);

            var deleted = await repository.GetByIdAsync(
                Guid.Parse("aaaa0000-0000-0000-0000-000000000001"), "user1");

            Assert.Null(deleted);
        }
    }
}