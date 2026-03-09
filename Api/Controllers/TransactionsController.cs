using Application.DTOs.Transaction;
using Application.DTOs;
using Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Domain.Entities;
using Domain.Enums;

namespace Api.Controllers
{
    /// <summary>
    /// Контролер для роботи з транзакціями
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
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
        public async Task<IActionResult> GetAll([FromQuery] TransactionsFilterDto filter)
        {
            var userId = GetUserId();

            var query = _context.Transactions
                .Include(t => t.Category)
                .Where(t => t.UserId == userId)
                .AsQueryable();

            // --- Фільтрація ---
            if (!string.IsNullOrEmpty(filter.Type) && Enum.TryParse<TransactionType>(filter.Type, true, out var type))
            {
                query = query.Where(t => t.Type == type);
            }

            if (filter.CategoryId.HasValue)
            {
                query = query.Where(t => t.CategoryId == filter.CategoryId.Value);
            }

            if (filter.DateFrom.HasValue)
            {
                query = query.Where(t => t.Date >= filter.DateFrom.Value);
            }

            if (filter.MinAmount.HasValue)
            {
                query = query.Where(t => t.Amount >= filter.MinAmount.Value);
            }

            if (filter.MaxAmount.HasValue)
            {
                query = query.Where(t => t.Amount <= filter.MaxAmount.Value);
            }

            // --- Сортування ---
            query = filter.SortBy.ToLower() switch
            {
                "amount" => filter.SortDirection.ToLower() == "asc"
                    ? query.OrderBy(t => t.Amount)
                    : query.OrderByDescending(t => t.Amount),
                _ => filter.SortDirection.ToLower() == "asc"
                    ? query.OrderBy(t => t.Date)
                    : query.OrderByDescending(t => t.Date)
            };

            // --- Підрахунок загальної кількості ---
            var totalCount = await query.CountAsync();

            // -- Пагінація ---
            var transactions = await query
                .Skip((filter.Page - 1) * filter.PageSize)
                .Take(filter.PageSize)
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

            return Ok(new PagedResponseDto<TransactionResponseDto>
            {
                Data = transactions,
                TotalCount = totalCount,
                Page = filter.Page,
                PageSize = filter.PageSize,
                TotalPages = (int)Math.Ceiling(totalCount / (double)filter.PageSize)
            });
        }

        /// <summary>
        /// Отримати транзакцію за Id
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var userId = GetUserId();
            var transaction = await _context.Transactions
                .Include(t => t.Category)
                .FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);

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
        /// Отримати UserId 
        /// </summary>
        private string GetUserId() => User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value;

        /// <summary>
        /// Створити нову транзакцію
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Create(TransactionCreateDto dto)
        {
            if (!Enum.TryParse<TransactionType>(dto.Type, true, out var type))
                return BadRequest("Type має бути 'Income' або 'Expense'");

            var userId = GetUserId();
            var category = await _context.Categories.FirstOrDefaultAsync(c => c.Id == dto.CategoryId && c.UserId == userId);
            if (category == null)
                return BadRequest("Категорію не знайдено");

            var transaction = new Transaction
            {
                Id = Guid.NewGuid(),
                Amount = dto.Amount,
                Type = type,
                Description = dto.Description,
                Date = dto.Date,
                CategoryId = dto.CategoryId,
                UserId = userId
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
            var userId = GetUserId();
            var transaction = await _context.Transactions.FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);

            if (transaction == null)
                return NotFound();

            if (!Enum.TryParse<TransactionType>(dto.Type, true, out var type))
                return BadRequest("Type має бути 'Income' або 'Expense'");

            var categoryExist = await _context.Categories.AnyAsync(c => c.Id == dto.CategoryId && c.UserId == userId);
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
            var userId = GetUserId();
            var transaction = await _context.Transactions.FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);

            if (transaction == null)
                return NotFound();

            _context.Transactions.Remove(transaction);
            await _context.SaveChangesAsync();

            return NoContent();

        }
    }
}
