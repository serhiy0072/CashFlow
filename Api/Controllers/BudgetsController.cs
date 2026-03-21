namespace Api.Controllers;

using Application.DTOs.Budget;
using Application.Interfaces;
using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

/// <summary>
/// Контролер для роботи з бюджетами
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class BudgetsController : ControllerBase
{
    private readonly IBudgetRepository _repository;
    private readonly ICategoryRepository _categoryRepository;

    public BudgetsController(IBudgetRepository repository, ICategoryRepository categoryRepository)
    {
        _repository = repository;
        _categoryRepository = categoryRepository;
    }

    /// <summary>
    /// Отримати UserId
    /// </summary>
    private string GetUserId() => User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value;

    /// <summary>
    /// Допоміжний метод: створити ResponseDto з бюджету
    /// </summary>
    private async Task<BudgetResponseDto> BuildResponseDto(Budget budget, string categoryName, string userId)
    {
        var spent = await _repository.GetSpentAmountAsync(userId, budget.CategoryId, budget.Month);

        return new BudgetResponseDto
        {
            Id = budget.Id,
            Amount = budget.Amount,
            Month = budget.Month,
            CategoryId = budget.CategoryId,
            CategoryName = categoryName,
            Spent = spent,
            Remaining = budget.Amount - spent,
            IsExceeded = spent > budget.Amount
        };
    }

    /// <summary>
    /// Отримати всі бюджети користувача
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] DateTime? month)
    {
        var userId = GetUserId();
        var budgets = await _repository.GetAllAsync(userId, month);

        var result = new List<BudgetResponseDto>();
        foreach (var budget in budgets)
        {
            var dto = await BuildResponseDto(budget, budget.Category.Name, userId);
            result.Add(dto);
        }

        return Ok(result);
    }

    /// <summary>
    /// Отримати бюджет за Id
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var userId = GetUserId();

        var budget = await _repository.GetByIdAsync(id, userId);

        if (budget == null)
            return NotFound();

        var dto = await BuildResponseDto(budget, budget.Category.Name, userId);
        return Ok(dto);
    }

    /// <summary>
    /// Створити новий бюджет
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Create(BudgetCreateDto dto)
    {
        var userId = GetUserId();
        var category = await _categoryRepository.GetByIdAsync(dto.CategoryId, userId);

        if (category == null)
            return BadRequest("Категорію не знайдено");

        var normalizedMonth = new DateTime(dto.Month.Year, dto.Month.Month, 1, 0, 0, 0, DateTimeKind.Utc);

        // Перевіряємо чи бюджет на цю категорію і місяць вже існує
        var exists = await _repository.ExistsAsync(userId, dto.CategoryId, normalizedMonth);

        if (exists)
            return BadRequest("Бюджет на цю категорію і місяць вже існує");

        var budget = new Budget
        {
            Id = Guid.NewGuid(),
            Amount = dto.Amount,
            Month = normalizedMonth,
            CategoryId = dto.CategoryId,
            UserId = userId
        };

        await _repository.CreateAsync(budget);

        var response = await BuildResponseDto(budget, category.Name, userId);
        return CreatedAtAction(nameof(GetById), new { id = budget.Id }, response);
    }

    /// <summary>
    /// Оновити бюджет (тільки суму)
    /// </summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, BudgetUpdateDto dto)
    {
        var userId = GetUserId();
        var budget = await _repository.GetByIdAsync(id, userId);

        if (budget == null)
            return NotFound();

        budget.Amount = dto.Amount;
        await _repository.UpdateAsync(budget);

        return NoContent();
    }

    /// <summary>
    /// Видалити бюджет
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var userId = GetUserId();

        var budget = await _repository.GetByIdAsync(id, userId);

        if (budget == null)
            return NotFound();

        await _repository.DeleteAsync(budget);

        return NoContent();
    }
}