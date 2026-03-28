using Domain.Entities;
using Domain.Enums;
using Infrastructure.Data;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Tests
{
    public class BudgetRepositoryTests
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

            var category = new Category
            {
                Id = Guid.Parse("cccc0000-0000-0000-0000-000000000001"),
                Name = "Їжа",
                Type = TransactionType.Expense,
                UserId = "user1"
            };

            var categoryUser2 = new Category
            {
                Id = Guid.Parse("cccc0000-0000-0000-0000-000000000002"),
                Name = "Розваги",
                Type = TransactionType.Expense,
                UserId = "user2"
            };

            context.Categories.AddRange(category, categoryUser2);

            context.Budgets.AddRange(
                new Budget
                {
                    Id = Guid.Parse("dddd0000-0000-0000-0000-000000000001"),
                    Amount = 5000m,
                    Month = new DateTime(2026, 3, 1, 0, 0, 0, DateTimeKind.Utc),
                    CategoryId = category.Id,
                    UserId = "user1"
                },
                new Budget
                {
                    Id = Guid.Parse("dddd0000-0000-0000-0000-000000000002"),
                    Amount = 4000m,
                    Month = new DateTime(2026, 4, 1, 0, 0, 0, DateTimeKind.Utc),
                    CategoryId = category.Id,
                    UserId = "user1"
                },
                new Budget
                {
                    Id = Guid.Parse("dddd0000-0000-0000-0000-000000000003"),
                    Amount = 2000m,
                    Month = new DateTime(2026, 3, 1, 0, 0, 0, DateTimeKind.Utc),
                    CategoryId = categoryUser2.Id,
                    UserId = "user2"
                }
            );

            context.Transactions.AddRange(
                new Transaction
                {
                    Id = Guid.NewGuid(),
                    Amount = 1200m,
                    Type = TransactionType.Expense,
                    Date = new DateTime(2026, 3, 5, 0, 0, 0, DateTimeKind.Utc),
                    CategoryId = category.Id,
                    UserId = "user1"
                },
                new Transaction
                {
                    Id = Guid.NewGuid(),
                    Amount = 800m,
                    Type = TransactionType.Expense,
                    Date = new DateTime(2026, 3, 20, 0, 0, 0, DateTimeKind.Utc),
                    CategoryId = category.Id,
                    UserId = "user1"
                }
            );

            await context.SaveChangesAsync();
            return context;
        }

        // ---------------------------------------------------------------
        // GetAllAsync
        // ---------------------------------------------------------------

        [Fact]
        public async Task GetAllAsync_WithoutMonthFilter_ReturnsAllUserBudgets()
        {
            var context = await CreateContextWithData();
            var repository = new BudgetRepository(context);

            var result = await repository.GetAllAsync("user1", month: null);

            // user1 має 2 бюджети (березень + квітень)
            Assert.Equal(2, result.Count);
            Assert.All(result, b => Assert.Equal("user1", b.UserId));
        }

        [Fact]
        public async Task GetAllAsync_WithMonthFilter_ReturnsOnlyBudgetsForThatMonth()
        {
            var context = await CreateContextWithData();
            var repository = new BudgetRepository(context);

            var result = await repository.GetAllAsync("user1",
                month: new DateTime(2026, 3, 15, 0, 0, 0, DateTimeKind.Utc));

            Assert.Single(result);
            Assert.Equal(new DateTime(2026, 3, 1, 0, 0, 0, DateTimeKind.Utc), result[0].Month);
        }

        // ---------------------------------------------------------------
        // GetByIdAsync
        // ---------------------------------------------------------------

        [Fact]
        public async Task GetByIdAsync_ExistingBudget_ReturnsBudget()
        {
            var context = await CreateContextWithData();
            var repository = new BudgetRepository(context);

            var result = await repository.GetByIdAsync(
                Guid.Parse("dddd0000-0000-0000-0000-000000000001"), "user1");

            Assert.NotNull(result);
            Assert.Equal(5000m, result.Amount);
        }

        [Fact]
        public async Task GetByIdAsync_OtherUserBudget_ReturnsNull()
        {
            var context = await CreateContextWithData();
            var repository = new BudgetRepository(context);

            var result = await repository.GetByIdAsync(
                Guid.Parse("dddd0000-0000-0000-0000-000000000001"), "user2");

            Assert.Null(result);
        }

        // ---------------------------------------------------------------
        // CreateAsync / UpdateAsync / DeleteAsync
        // ---------------------------------------------------------------

        [Fact]
        public async Task CreateAsync_AddsBudget_ReturnsCreatedBudget()
        {
            var context = await CreateContextWithData();
            var repository = new BudgetRepository(context);

            var budget = new Budget
            {
                Id = Guid.NewGuid(),
                Amount = 3000m,
                Month = new DateTime(2026, 5, 1, 0, 0, 0, DateTimeKind.Utc),
                CategoryId = Guid.Parse("cccc0000-0000-0000-0000-000000000001"),
                UserId = "user1"
            };

            var result = await repository.CreateAsync(budget);

            Assert.NotNull(result);
            Assert.Equal(3000m, result.Amount);
            // Було 3 бюджети, тепер має бути 4
            Assert.Equal(4, await context.Budgets.CountAsync());
        }

        [Fact]
        public async Task UpdateAsync_ChangesBudgetAmount_SavesChanges()
        {
            var context = await CreateContextWithData();
            var repository = new BudgetRepository(context);

            var budget = await repository.GetByIdAsync(
                Guid.Parse("dddd0000-0000-0000-0000-000000000001"), "user1");

            budget!.Amount = 9999m;
            await repository.UpdateAsync(budget);

            var updated = await repository.GetByIdAsync(
                Guid.Parse("dddd0000-0000-0000-0000-000000000001"), "user1");

            Assert.Equal(9999m, updated!.Amount);
        }

        [Fact]
        public async Task DeleteAsync_RemovesBudget_BudgetNotInDatabase()
        {
            var context = await CreateContextWithData();
            var repository = new BudgetRepository(context);

            var budget = await repository.GetByIdAsync(
                Guid.Parse("dddd0000-0000-0000-0000-000000000001"), "user1");

            await repository.DeleteAsync(budget!);

            var deleted = await repository.GetByIdAsync(
                Guid.Parse("dddd0000-0000-0000-0000-000000000001"), "user1");

            Assert.Null(deleted);
        }

        // ---------------------------------------------------------------
        // ExistsAsync
        // ---------------------------------------------------------------

        [Fact]
        public async Task ExistsAsync_BudgetExists_ReturnsTrue()
        {
            var context = await CreateContextWithData();
            var repository = new BudgetRepository(context);

            var exists = await repository.ExistsAsync(
                "user1",
                Guid.Parse("cccc0000-0000-0000-0000-000000000001"),
                new DateTime(2026, 3, 15, 0, 0, 0, DateTimeKind.Utc));

            Assert.True(exists);
        }

        [Fact]
        public async Task ExistsAsync_BudgetDoesNotExist_ReturnsFalse()
        {
            var context = await CreateContextWithData();
            var repository = new BudgetRepository(context);

            var exists = await repository.ExistsAsync(
                "user1",
                Guid.Parse("cccc0000-0000-0000-0000-000000000001"),
                new DateTime(2026, 6, 1, 0, 0, 0, DateTimeKind.Utc));

            Assert.False(exists);
        }

        // ---------------------------------------------------------------
        // GetSpentAmountAsync
        // ---------------------------------------------------------------

        [Fact]
        public async Task GetSpentAmountAsync_ReturnsCorrectSum()
        {
            var context = await CreateContextWithData();
            var repository = new BudgetRepository(context);

            // В березні є дві транзакції: 1200 + 800 = 2000
            var spent = await repository.GetSpentAmountAsync(
                "user1",
                Guid.Parse("cccc0000-0000-0000-0000-000000000001"),
                new DateTime(2026, 3, 1, 0, 0, 0, DateTimeKind.Utc));

            Assert.Equal(2000m, spent);
        }

        [Fact]
        public async Task GetSpentAmountAsync_NoTransactions_ReturnsZero()
        {
            var context = await CreateContextWithData();
            var repository = new BudgetRepository(context);

            var spent = await repository.GetSpentAmountAsync(
                "user1",
                Guid.Parse("cccc0000-0000-0000-0000-000000000001"),
                new DateTime(2026, 4, 1, 0, 0, 0, DateTimeKind.Utc));

            Assert.Equal(0m, spent);
        }
    }
}