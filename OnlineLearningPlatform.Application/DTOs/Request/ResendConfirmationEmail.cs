using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineLearningPlatform.Application.DTOs.Request
{
    public class ResendConfirmationEmail
    {
        public required string Username { get; set; }
    }
}
