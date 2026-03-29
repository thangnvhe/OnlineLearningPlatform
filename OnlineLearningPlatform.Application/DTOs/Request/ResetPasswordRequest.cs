using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineLearningPlatform.Application.DTOs.Request
{
    public class ResetPasswordRequest
    {
        // User id as string ( Guid.ToString() )
        public required string UserId { get; set; }

        // URL-encoded reset token sent to user's email
        public required string Token { get; set; }

        // New password to set
        public required string NewPassword { get; set; }
    }
}
