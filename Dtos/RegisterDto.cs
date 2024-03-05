﻿using System.ComponentModel.DataAnnotations;

namespace UserAuthentication.Dtos
{
    public class RegisterDto
    {
        [Required]
        [EmailAddress]
        
        public string Email { get; set; } = string.Empty;

        public string UserName { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;

        public List<string>? Roles { get; set; }


    }
}
