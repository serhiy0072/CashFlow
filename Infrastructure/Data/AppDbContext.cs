using Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data
{
    /// <summary>
    /// Головний контекст бази даних
    /// EF Core використовує цей клас для роботи з таблицями. 
    /// </summary>
    public class AppDbContext : IdentityDbContext<AppUser>
    {
        /// <summary>
        /// Конструктор приймає налаштування (connection string тощо)
        /// з Program.cs через Dependency Injection
        /// </summary>
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        /// <summary>
        /// Таблиця транзакцій
        /// </summary>
        public DbSet<Transaction> Transactions => Set<Transaction>();

        /// <summary>
        /// Таблиця категорій
        /// </summary>
        public DbSet<Category> Categories => Set<Category>();

        /// <summary>
        /// Таблиця бюджетів
        /// </summary>
        public DbSet<Budget> Budgets => Set<Budget>();

        /// <summary>
        /// Налаштування моделей та зв'язків між таблицями.
        /// Конфігурація правил для бази даних.
        /// </summary>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // --- Transaction ---
            modelBuilder.Entity<Transaction>(entity => 
            {
                // Сума зберігається з точністю до 2 знаків після коми
                entity.Property(t => t.Amount).HasColumnType("decimal(18,2)");

                // Enum зберігається як текст "Income"/"Expense", а не число
                entity.Property(t => t.Type).HasConversion<string>();

                // Зв'язок: Transaction -> Category (багато до одного)
                entity.HasOne(t => t.Category)
                      .WithMany(c => c.Transactions)
                      .HasForeignKey(t => t.CategoryId)
                      .OnDelete(DeleteBehavior.Restrict);

                // Зв'язок: Transaction -> User
                entity.HasOne<AppUser>()
                      .WithMany()
                      .HasForeignKey(t => t.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // --- Category ---
            modelBuilder.Entity<Category>(entity =>
            {
                entity.Property(c => c.Name).HasMaxLength(100);
                entity.Property(c => c.Type).HasConversion<string>();
                entity.Property(c => c.Icon).HasMaxLength(50);
            });

            // --- Budget ---
            modelBuilder.Entity<Budget>(entity =>
            {
                entity.Property(b => b.Amount).HasColumnType("decimal(18,2)");
                // Зв'язок Budget -> Category
                entity.HasOne(b => b.Category)
                      .WithMany()
                      .HasForeignKey(b => b.CategoryId)
                      .OnDelete(DeleteBehavior.Restrict);

                // Зв'язок: Budget -> User
                entity.HasOne<AppUser>()
                      .WithMany()
                      .HasForeignKey(b => b.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
