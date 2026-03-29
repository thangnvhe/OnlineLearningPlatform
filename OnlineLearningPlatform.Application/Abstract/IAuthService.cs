using OnlineLearningPlatform.Application.DTOs;
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
        Task<ServiceResult<AuthResponse>> LoginAsync(LoginRequest request);
        Task<ServiceResult<bool>> RegisterAsync(RegisterRequest request);
        Task<ServiceResult<AuthResponse>> RefreshTokenAsync(CancellationToken cancellationToken);
        Task<ServiceResult<bool>> ResetPasswordAsync(ResetPasswordRequest request);
        Task<ServiceResult<bool>> ChangePasswordAsync(ChangePasswordRequest request);
        Task<ServiceResult<bool>> ConfirmEmailAsync(ConfirmEmailRequest request);
        Task<ServiceResult<bool>> ForgotPasswordAsync(ForgotPasswordRequest request);
        Task<ServiceResult<bool>> ResendConfirmationEmail(ResendConfirmationEmail request);
        Task<ServiceResult<bool>> Logout(string userId);

    }
}
