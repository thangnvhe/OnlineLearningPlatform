using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OnlineLearningPlatform.Application.Abstract;
using OnlineLearningPlatform.Application.DTOs;
using OnlineLearningPlatform.Application.DTOs.Request;
using OnlineLearningPlatform.Application.DTOs.Response;
using System.Net;

namespace OnlineLearningPlatform.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
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
            var response = APIResponse<AuthResponse>.Builder()
                .WithResult(result)
                .WithSuccess(true)
                .Build();
            return Ok(response);
        }

    }
}
