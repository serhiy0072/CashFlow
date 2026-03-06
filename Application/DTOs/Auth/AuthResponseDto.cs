using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Application.DTOs.Auth
{
    /// <summary>
    /// Відповідь після успішної аутентифікації
    /// </summary>
    public class AuthResponseDto
    {
        /// <summary>
        /// Jwt токен для доступу до API
        /// </summary>
        public string Token { get; set; } = string.Empty;

        /// <summary>
        /// Дата закінчення дії токена
        /// </summary>
        public DateTime Expiration { get; set; } 

        /// <summary>
        /// Email користувача
        /// </summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Ім'я користувача
        /// </summary>
        public string FirstName { get; set; } = string.Empty;
    }
}
