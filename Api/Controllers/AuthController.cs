using Application.DTOs.Auth;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace Api.Controllers
{
    /// <summary>
    /// Контролер аутентифікації - реєстрація та вхід
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IConfiguration _configuration;

        /// <summary>
        /// UserManager - сервіс Identity для роботи з користувачами.
        /// IConfiguration - доступ до appsettings.json
        /// </summary>
        public AuthController(UserManager<AppUser> userManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _configuration = configuration;
        }

        /// <summary>
        /// Реєстрація нового користувача
        /// </summary>
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto dto)
        {
            var existingUser = await _userManager.FindByEmailAsync(dto.Email);
            if (existingUser != null)
                return BadRequest("Користувача з таким email вже існує");

            var user = new AppUser
            {
                UserName = dto.Email,
                Email = dto.Email,
                FirstName = dto.FirstName,
                LastName = dto.LastName ?? string.Empty
            };

            var result = await _userManager.CreateAsync(user, dto.Password);

            if (!result.Succeeded)
            {
                //Повертаємо всі помилки валідації від Identity
                var errors = result.Errors.Select(e => e.Description);
                return BadRequest(new { errors });
            }

            // Генеруємо токен і повертаєм
            var response = GenerateAuthResponse(user);
            return Ok(response);
        }

        /// <summary>
        /// Вхід існуючого користувача
        /// </summary>
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null)
                return Unauthorized("Невірний email або пароль");

            var isValidPassword = await _userManager.CheckPasswordAsync(user, dto.Password);
            if (!isValidPassword)
                return Unauthorized("Невірний email або пароль");

            var response = GenerateAuthResponse(user);
            return Ok(response);
        }

        /// <summary>
        /// Генерація JWT токена та формування відповіді
        /// </summary>
        private AuthResponseDto GenerateAuthResponse(AppUser user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Email, user.Email!),
                new Claim(ClaimTypes.Name, user.FirstName)
            };

            // Ключ підпису
            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));

            // Алгоритм підпису
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // Час закінчення дії токена
            var expiration = DateTime.UtcNow.AddMinutes(
                double.Parse(_configuration["Jwt:ExpiresInMinutes"]!));

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: expiration,
                signingCredentials: credentials
                );

            // Серіалізація токену в рядок
            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            return new AuthResponseDto
            {
                Token = tokenString,
                Expiration = expiration,
                Email = user.Email!,
                FirstName = user.FirstName
            };
        }
    }
}
