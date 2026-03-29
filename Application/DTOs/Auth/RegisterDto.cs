using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Application.DTOs.Auth
{
    /// <summary>
    /// Дані для реєстрації нового користувача
    /// </summary>
    public class RegisterDto
    {
        /// <summary>
        /// Email користувача
        /// </summary>
        [Required(ErrorMessage = "Email обов'язковий")]
        [EmailAddress(ErrorMessage = "Невірний формат email")]
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Пароль користувача
        /// </summary>
        [Required(ErrorMessage = "Пароль обов'язковий")]
        [MinLength(10, ErrorMessage = "Пароль мінімум 10 символів")]
        public string Password { get; set; } = string.Empty;

        /// <summary>
        /// Ім'я користувача
        /// </summary>
        [Required(ErrorMessage = "Ім'я обов'язкове")]
        [StringLength(50, MinimumLength = 2)]
        public string FirstName { get; set; } = string.Empty;

        /// <summary>
        /// Прізвище користувача (не обов'язково)
        /// </summary>
        [StringLength(50)]
        public string LastName { get; set; } = string.Empty;
    }
}
