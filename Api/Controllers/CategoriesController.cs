using Application.DTOs.Category;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Domain.Entities;
using Domain.Enums;
using Application.Interfaces;


namespace Api.Controllers
{
    /// <summary>
    /// Контролер для роботи з категоріями
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryRepository _repository;

        /// <summary>
        /// Репозиторій приходить автоматично через Dependency Injection
        /// </summary>
        public CategoriesController(ICategoryRepository repository)
        {
            _repository = repository;
        }

        /// <summary>
        /// Отримати всі категорії
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var userId = GetUserId();
            var categories = await _repository.GetAllAsync(userId);

            var response = categories.Select(c => new CategoryResponseDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    Type = c.Type.ToString(),
                    Icon = c.Icon
                })
                .ToList();
            return Ok(response);
        }

        /// <summary>
        /// Отримати категорію за Id
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var userId = GetUserId();

            var category = await _repository.GetByIdAsync(id, userId);
            if (category == null)
                return NotFound();

            return Ok(new CategoryResponseDto
            {
                Id = category.Id,
                Name = category.Name,
                Type = category.Type.ToString(),
                Icon = category.Icon
            });
        }

        /// <summary>
        /// Отримати UserId 
        /// </summary>
        private string GetUserId() => User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value;

        /// <summary>
        /// Створити нову категорію
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Create(CategoryCreateDto dto)
        {
            if (!Enum.TryParse<TransactionType>(dto.Type, true, out var type))
                return BadRequest("Type має бути 'Income' або 'Expense'");

            var category = new Category
            {
                Id = Guid.NewGuid(),
                Name = dto.Name,
                Type = type,
                Icon = dto.Icon,
                UserId = GetUserId()
            };

            await _repository.CreateAsync(category);

            var response = new CategoryResponseDto
            {
                Id = category.Id,
                Name = category.Name,
                Type = category.Type.ToString(),
                Icon = category.Icon
            };

            return CreatedAtAction(nameof(GetById), new { id = category.Id }, response);
        }

        /// <summary>
        /// Оновити категорію
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, CategoryUpdateDto dto)
        {

            var userId = GetUserId();
            var category = await _repository.GetByIdAsync(id, userId);

            if (category == null)
                return NotFound();

            if (!Enum.TryParse<TransactionType>(dto.Type, true, out var type))
                return BadRequest("Type має бути 'Income' або 'Expense'");

            category.Name = dto.Name;
            category.Type = type;
            category.Icon = dto.Icon;

            await _repository.UpdateAsync(category);

            return NoContent();
        }

        /// <summary>
        /// Видалити категорію
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var userId = GetUserId();
            var category = await _repository.GetByIdAsync(id, userId);

            if (category == null)
                return NotFound();

            await _repository.DeleteAsync(category);

            return NoContent();
        }
    }
}
