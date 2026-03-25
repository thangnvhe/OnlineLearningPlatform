using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineLearningPlatform.Application.DTOs.Response
{
    public class AuthResponse
    {
        public string Token { get; set; }
        public string FullName { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public DateOnly Dob { get; set; }
        public string Address { get; set; }
        public bool IsMale { get; set; } = true;
        public string? AvatarUrl { get; set; } 
        public List<string> Roles { get; set; }
    }
}
