using Domain.Entities;
using Domain.Enums;
using Infrastructure.Data;
using Infrastructure.Repositories;
using Application.DTOs.Transaction;
using Microsoft.EntityFrameworkCore;

namespace Tests
{
    public class TransactionRepositoryTests
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

            var categoryUser1 = new Category
            {
                Id = Guid.Parse("aaaa0000-0000-0000-0000-000000000001"),
                Name = "Зарплата",
                Type = TransactionType.Income,
                UserId = "user1"
            };

            var categoryUser2 = new Category
            {
                Id = Guid.Parse("aaaa0000-0000-0000-0000-000000000002"),
                Name = "Транспорт",
                Type = TransactionType.Expense,
                UserId = "user2"
            };

            context.Categories.AddRange(categoryUser1, categoryUser2);

            context.Transactions.AddRange(
                new Transaction
                {
                    Id = Guid.Parse("bbbb0000-0000-0000-0000-000000000001"),
                    Amount = 10000m,
                    Type = TransactionType.Income,
                    Date = new DateTime(2026, 3, 1, 0, 0, 0, DateTimeKind.Utc),
                    CategoryId = categoryUser1.Id,
                    UserId = "user1"
                },
                new Transaction
                {
                    Id = Guid.Parse("bbbb0000-0000-0000-0000-000000000002"),
                    Amount = 500m,
                    Type = TransactionType.Expense,
                    Date = new DateTime(2026, 3, 15, 0, 0, 0, DateTimeKind.Utc),
                    CategoryId = categoryUser1.Id,
                    UserId = "user1"
                },
                new Transaction
                {
                    Id = Guid.Parse("bbbb0000-0000-0000-0000-000000000003"),
                    Amount = 200m,
                    Type = TransactionType.Expense,
                    Date = new DateTime(2026, 3, 10, 0, 0, 0, DateTimeKind.Utc),
                    CategoryId = categoryUser2.Id,
                    UserId = "user2"
                }
            );

            await context.SaveChangesAsync();
            return context;
        }

        // ---------------------------------------------------------------
        // GetByIdAsync
        // ---------------------------------------------------------------

        [Fact]
        public async Task GetByIdAsync_ExistingTransaction_ReturnsTransaction()
        {
            var context = await CreateContextWithData();
            var repository = new TransactionRepository(context);

            var result = await repository.GetByIdAsync(
                Guid.Parse("bbbb0000-0000-0000-0000-000000000001"), "user1");

            Assert.NotNull(result);
            Assert.Equal(10000m, result.Amount);
        }

        [Fact]
        public async Task GetByIdAsync_OtherUserTransaction_ReturnsNull()
        {
            // user2 намагається отримати транзакцію user1 — повинен отримати null
            var context = await CreateContextWithData();
            var repository = new TransactionRepository(context);

            var result = await repository.GetByIdAsync(
                Guid.Parse("bbbb0000-0000-0000-0000-000000000001"), "user2");

            Assert.Null(result);
        }

        // ---------------------------------------------------------------
        // CreateAsync / UpdateAsync / DeleteAsync
        // ---------------------------------------------------------------

        [Fact]
        public async Task CreateAsync_AddsTransaction_ReturnsCreatedTransaction()
        {
            var context = await CreateContextWithData();
            var repository = new TransactionRepository(context);

            var transaction = new Transaction
            {
                Id = Guid.NewGuid(),
                Amount = 300m,
                Type = TransactionType.Expense,
                Date = new DateTime(2026, 3, 20, 0, 0, 0, DateTimeKind.Utc),
                CategoryId = Guid.Parse("aaaa0000-0000-0000-0000-000000000001"),
                UserId = "user1"
            };

            var result = await repository.CreateAsync(transaction);

            Assert.NotNull(result);
            Assert.Equal(300m, result.Amount);
            Assert.Equal(4, await context.Transactions.CountAsync());
        }

        [Fact]
        public async Task UpdateAsync_ChangesAmount_SavesChanges()
        {
            var context = await CreateContextWithData();
            var repository = new TransactionRepository(context);

            var transaction = await repository.GetByIdAsync(
                Guid.Parse("bbbb0000-0000-0000-0000-000000000002"), "user1");

            transaction!.Amount = 999m;
            await repository.UpdateAsync(transaction);

            var updated = await repository.GetByIdAsync(
                Guid.Parse("bbbb0000-0000-0000-0000-000000000002"), "user1");

            Assert.Equal(999m, updated!.Amount);
        }

        [Fact]
        public async Task DeleteAsync_RemovesTransaction_TransactionNotInDatabase()
        {
            var context = await CreateContextWithData();
            var repository = new TransactionRepository(context);

            var transaction = await repository.GetByIdAsync(
                Guid.Parse("bbbb0000-0000-0000-0000-000000000001"), "user1");

            await repository.DeleteAsync(transaction!);

            var deleted = await repository.GetByIdAsync(
                Guid.Parse("bbbb0000-0000-0000-0000-000000000001"), "user1");

            Assert.Null(deleted);
        }

        // ---------------------------------------------------------------
        // GetFilteredAsync
        // ---------------------------------------------------------------

        [Fact]
        public async Task GetFilteredAsync_FilterByType_ReturnsOnlyExpenses()
        {
            var context = await CreateContextWithData();
            var repository = new TransactionRepository(context);

            var filter = new TransactionsFilterDto { Type = "Expense" };

            var (transactions, totalCount) = await repository.GetFilteredAsync("user1", filter);

            Assert.Equal(1, totalCount);
            Assert.All(transactions, t => Assert.Equal(TransactionType.Expense, t.Type));
        }

        [Fact]
        public async Task GetFilteredAsync_FilterByDateFrom_ReturnsOnlyTransactionsAfterDate()
        {
            var context = await CreateContextWithData();
            var repository = new TransactionRepository(context);

            var filter = new TransactionsFilterDto
            {
                DateFrom = new DateTime(2026, 3, 10, 0, 0, 0, DateTimeKind.Utc)
            };

            var (transactions, totalCount) = await repository.GetFilteredAsync("user1", filter);

            Assert.Equal(1, totalCount);
            Assert.All(transactions, t => Assert.True(t.Date >= filter.DateFrom));
        }

        [Fact]
        public async Task GetFilteredAsync_Pagination_ReturnsCorrectPage()
        {
            var context = await CreateContextWithData();
            var repository = new TransactionRepository(context);

            var filter = new TransactionsFilterDto { Page = 2, PageSize = 1 };

            var (transactions, totalCount) = await repository.GetFilteredAsync("user1", filter);

            Assert.Equal(2, totalCount);   
            Assert.Single(transactions);  
        }
    }
}