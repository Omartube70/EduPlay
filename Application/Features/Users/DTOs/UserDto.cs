using System;
using System.Collections.Generic;
using System.Text;
using Application.Interfaces.Repositories;

namespace Application.Features.Users.DTOs
{
        public class UserDto
        {
            public int UserId { get; set; }
            public string UserName { get; set; } = string.Empty;
            public string Email { get; set; } = string.Empty;
            public string UserRole { get; set; } = string.Empty;
        }
}

