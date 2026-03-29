using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineLearningPlatform.API.Controllers.Base;
using OnlineLearningPlatform.Application.Abstract;
using OnlineLearningPlatform.Application.DTOs;
using OnlineLearningPlatform.Application.DTOs.Request;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace OnlineLearningPlatform.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : BaseController
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("Login")]
        public async Task<IActionResult> LoginAsync([FromBody] LoginRequest request)
        {
            var result = await _authService.LoginAsync(request);
            return HandleResult(result);
        }

        [HttpPost("ConfirmEmail")]
        public async Task<IActionResult> ConfirmEmailAsync([FromBody] ConfirmEmailRequest request)
        {
            var result = await _authService.ConfirmEmailAsync(request);
            return HandleResult(result);
        }

        [HttpPost("Register")]
        public async Task<IActionResult> RegisterAsync([FromBody] RegisterRequest request)
        {
            var result = await _authService.RegisterAsync(request);
            return HandleResult(result);
        }

        [HttpPost("RefreshToken")]
        public async Task<IActionResult> RefreshTokenAsync()
        {
            var result = await _authService.RefreshTokenAsync(HttpContext.RequestAborted);
            return HandleResult(result);
        }

        [HttpPost("ForgotPassword")]
        public async Task<IActionResult> ForgotPasswordAsync([FromBody] ForgotPasswordRequest request)
        {
            var result = await _authService.ForgotPasswordAsync(request);
            return HandleResult(result);
        }

        [HttpPost("ResetPassword")]
        public async Task<IActionResult> ResetPasswordAsync([FromBody] ResetPasswordRequest request)
        {
            var result = await _authService.ResetPasswordAsync(request);
            return HandleResult(result);
        }

        [Authorize]
        [HttpPost("ChangePassword")]
        public async Task<IActionResult> ChangePasswordAsync([FromBody] ChangePasswordRequest request)
        {
            var result = await _authService.ChangePasswordAsync(request);
            return HandleResult(result);
        }

        [Authorize]
        [HttpPost("Logout")]
        public async Task<IActionResult> LogoutAsync()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                var bad = ServiceResult<bool>.Failure("Không tìm thấy userId trong token.");
                return HandleResult(bad);
            }

            var result = await _authService.Logout(userId!);
            return HandleResult(result);
        }

        [HttpPost("ResendConfirmationEmail")]
        public async Task<IActionResult> ResendConfirmationEmailAsync([FromBody] ResendConfirmationEmail request)
        {
            var result = await _authService.ResendConfirmationEmail(request);
            return HandleResult(result);
        }
    }
}
