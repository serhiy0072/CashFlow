using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity;

namespace Domain.Entities
{
    /// <summary>
    /// Користувач додатку.
    /// Наслідується від IdentityUser, який вже має 
    /// Id, UserName, Email, PasswordHash та інші поля.
    /// </summary>
    public class AppUser:IdentityUser
    {
        /// <summary>
        /// Ім'я користувача (відображуване)
        /// </summary>
        public string FirstName { get; set; } = String.Empty;
        /// <summary>
        /// Прізвище користувача
        /// </summary>
        public string LastName { get; set; } = String.Empty;

        /// <summary>
        /// Дата реєстрації
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
