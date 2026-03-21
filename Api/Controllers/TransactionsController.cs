using Application.DTOs.Transaction;
using Application.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Domain.Entities;
using Domain.Enums;
using Application.Interfaces;

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
        private readonly ITransactionRepository _repository;
        private readonly ICategoryRepository _categoryRepository;

        public TransactionsController(ITransactionRepository repository, ICategoryRepository categoryRepository)
        {
            _repository = repository;
            _categoryRepository = categoryRepository;
        }

        /// <summary>
        /// Отримати всі транзакції
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] TransactionsFilterDto filter)
        {
            var userId = GetUserId();
            var (transactions, totalCount) = await _repository.GetFilteredAsync(userId, filter);

            var response = transactions.Select(t => new TransactionResponseDto
            {
                Id = t.Id,
                Amount = t.Amount,
                Type = t.Type.ToString(),
                Description = t.Description,
                Date = t.Date,
                CategoryId = t.CategoryId,
                CategoryName = t.Category.Name
            }).ToList();

            return Ok(new PagedResponseDto<TransactionResponseDto>
            {
                Data = response,
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
            var transaction = await _repository.GetByIdAsync(id, userId);

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
            var category = await _categoryRepository.GetByIdAsync(dto.CategoryId, userId);
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

            await _repository.CreateAsync(transaction);

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

            return CreatedAtAction(nameof(GetById), new { id = transaction.Id }, response);
        }

        /// <summary>
        /// Оновити транзакцію
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, TransactionUpdateDto dto)
        {
            var userId = GetUserId();
            var transaction = await _repository.GetByIdAsync(id, userId);

            if (transaction == null)
                return NotFound();

            if (!Enum.TryParse<TransactionType>(dto.Type, true, out var type))
                return BadRequest("Type має бути 'Income' або 'Expense'");

            var category = await _categoryRepository.GetByIdAsync(dto.CategoryId, userId);
            if (category == null)
                return BadRequest("Категорію не знайдено");

            transaction.Amount = dto.Amount;
            transaction.Type = type;
            transaction.Description = dto.Description;
            transaction.Date = dto.Date;
            transaction.CategoryId = dto.CategoryId;

            await _repository.UpdateAsync(transaction);

            return NoContent();
        }

        /// <summary>
        /// Видалити транзакцію
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var userId = GetUserId();
            var transaction = await _repository.GetByIdAsync(id, userId);

            if (transaction == null)
                return NotFound();

            await _repository.DeleteAsync(transaction);

            return NoContent();
        }
    }
}
