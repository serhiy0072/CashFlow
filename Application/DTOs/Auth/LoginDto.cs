using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Application.DTOs.Auth
{
    /// <summary>
    /// Дані для входу
    /// </summary>
    public class LoginDto
    {
        /// <summary>
        /// Email користувача
        /// </summary>
        [Required(ErrorMessage = "Email обов'язковий")]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Пароль користувача
        /// </summary>
        [Required(ErrorMessage = "Пароль обов'язковий")]
        public string Password { get; set; } = string.Empty;
    }
}
