using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineLearningPlatform.Application.DTOs.Request
{
    public class ConfirmEmailRequest
    {
        public required string UserId { get; set; }
        public required string Token { get; set; }
    }
}
