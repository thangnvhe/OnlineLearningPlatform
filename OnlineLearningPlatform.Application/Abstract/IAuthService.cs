using OnlineLearningPlatform.Application.DTOs.Request;
using OnlineLearningPlatform.Application.DTOs.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineLearningPlatform.Application.Abstract
{
    public interface IAuthService
    {
        Task<AuthResponse> LoginAsync(LoginRequest request);
    }
}
