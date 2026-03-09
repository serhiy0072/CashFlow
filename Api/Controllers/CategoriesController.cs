using Application.DTOs.Category;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.Data;


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
        private readonly AppDbContext _context;

        /// <summary>
        /// DbContext приходить автоматично через Dependency Injection
        /// </summary>
        public CategoriesController(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Отримати всі категорії
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var userId = GetUserId();

            var categories = await _context.Categories
                .Where(c => c.UserId == userId)
                .Select(c => new CategoryResponseDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    Type = c.Type.ToString(),
                    Icon = c.Icon
                })
                .ToListAsync();
            return Ok(categories);
        }

        /// <summary>
        /// Отримати категорію за Id
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var userId = GetUserId();

            var category = await _context.Categories.FirstOrDefaultAsync(c => c.Id == id && c.UserId == userId);
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

            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

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
            var category = await _context.Categories.FirstOrDefaultAsync(c => c.Id == id && c.UserId == userId);

            if (category == null)
                return NotFound();

            if (!Enum.TryParse<TransactionType>(dto.Type, true, out var type))
                return BadRequest("Type має бути 'Income' або 'Expense'");

            category.Name = dto.Name;
            category.Type = type;
            category.Icon = dto.Icon;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        /// <summary>
        /// Видалити категорію
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var userId = GetUserId();
            var category = await _context.Categories.FirstOrDefaultAsync(c => c.Id == id && c.UserId == userId);

            if (category == null)
                return NotFound();

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
