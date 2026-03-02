using Application.DTOs.Transaction;

using Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Domain.Entities;
using Domain.Enums;

namespace Api.Controllers
{
    /// <summary>
    /// Контролер для роботи з транзакціями
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class TransactionsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public TransactionsController(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Отримати всі транзакції
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var transactions = await _context.Transactions
                .Include(t => t.Category)
                .Select(t => new TransactionResponseDto
                {
                    Id = t.Id,
                    Amount = t.Amount,
                    Type = t.Type.ToString(),
                    Description = t.Description,
                    Date = t.Date,
                    CategoryId = t.CategoryId,
                    CategoryName = t.Category.Name
                })
                .ToListAsync();
            return Ok(transactions);
        }

        /// <summary>
        /// Отримати транзакцію за Id
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var transaction = await _context.Transactions
                .Include(t => t.Category)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (transaction == null)
                return NotFound();

            return Ok(new TransactionResponseDto
            {
                Id = transaction.Id,
                Amount = transaction.Amount,
                Type = transaction.Type.ToString(),
                Description = transaction.Description,
                Date = transaction.Date,
                CategoryId = transaction.CategoryId,
                CategoryName = transaction.Category.Name
            });
        }

        /// <summary>
        /// Створити нову транзакцію
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Create(TransactionCreateDto dto)
        {
            if (!Enum.TryParse<TransactionType>(dto.Type, true, out var type))
                return BadRequest("Type має бути 'Income' або 'Expense'");

            var category = await _context.Categories.FindAsync(dto.CategoryId);
            if (category == null)
                return BadRequest("Категорію не знайдено");

            var transaction = new Transaction
            {
                Id = Guid.NewGuid(),
                Amount = dto.Amount,
                Type = type,
                Description = dto.Description,
                Date = dto.Date,
                CategoryId = dto.CategoryId
            };

            _context.Transactions.Add(transaction);
            await _context.SaveChangesAsync();

            var response = new TransactionResponseDto
            {
                Id = transaction.Id,
                Amount = transaction.Amount,
                Type = transaction.Type.ToString(),
                Description = transaction.Description,
                Date = transaction.Date,
                CategoryId = transaction.CategoryId,
                CategoryName = category.Name
            };

            return CreatedAtAction(nameof(GetById), new { Id = transaction.Id }, response);
        }

        /// <summary>
        /// Оновити транзакцію
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, TransactionUpdateDto dto)
        {
            var transaction = await _context.Transactions.FindAsync(id);

            if (transaction == null)
                return NotFound();

            if(!Enum.TryParse<TransactionType>(dto.Type, true, out var type))
                return BadRequest("Type має бути 'Income' або 'Expense'");

            var categoryExist = await _context.Categories.AnyAsync(c => c.Id == dto.CategoryId);
            if (!categoryExist)
                return BadRequest("Категорію не знайдено");

            transaction.Amount = dto.Amount;
            transaction.Type = type;
            transaction.Description = dto.Description;
            transaction.Date = dto.Date;
            transaction.CategoryId = dto.CategoryId;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        /// <summary>
        /// Видалити транзакцію
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var transaction = await _context.Transactions.FindAsync(id);

            if (transaction == null)
                return NotFound();

            _context.Transactions.Remove(transaction);
            await _context.SaveChangesAsync();

            return NoContent();

        }
    }
}
