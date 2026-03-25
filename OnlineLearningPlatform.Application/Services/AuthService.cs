using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using OnlineLearningPlatform.Application.Abstract;
using OnlineLearningPlatform.Application.DTOs.Request;
using OnlineLearningPlatform.Application.DTOs.Response;
using OnlineLearningPlatform.Application.Exceptions;
using OnlineLearningPlatform.Domain.Entities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace OnlineLearningPlatform.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;

        public AuthService(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IConfiguration configuration, IMapper mapper)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
            _mapper = mapper;
        }
        public async Task<AuthResponse> LoginAsync(LoginRequest request)
        {
            // 0. Validate request
            if (string.IsNullOrEmpty(request.Username) || string.IsNullOrEmpty(request.Password))
                throw new AppException(ErrorCodes.InvalidCredentials("Tên đăng nhập hoặc mật khẩu không được để trống."));
            // 1. Tìm kiếm user theo userName bằng UserManager
            var user = await _userManager.FindByNameAsync(request.Username);
            if (user == null)
                throw new AppException(ErrorCodes.InvalidCredentials("Tên đăng nhập hoặc mật khẩu không đúng."));
            // 2. Dùng SignInManager để check pass và xử lý Lockout tự động (Không set cookie vì đang làm API)
            var signInResult = await _signInManager.CheckPasswordSignInAsync(user, request.Password, lockoutOnFailure: true);

            if (signInResult.IsLockedOut)
            {
                var remainningLockOut = user.LockoutEnd!.Value - DateTimeOffset.UtcNow;
                var minutes = Math.Max(1, Math.Round(remainningLockOut.TotalMinutes));
                throw new AppException(ErrorCodes.LockAccount($"Tài khoản đã bị khóa. Vui lòng thử lại sau {minutes} phút."));
            }

            if (!signInResult.Succeeded)
            {
                throw new AppException(ErrorCodes.InvalidCredentials("Tên đăng nhập hoặc mật khẩu không đúng."));
            }

            // 3. Đúng pass rồi thì phát Token
            var roles = await _userManager.GetRolesAsync(user);
            var token = GenerateJwtToken(user, roles);

            var response = _mapper.Map<AuthResponse>(user);
            response.Token = token;
            response.Roles = roles.ToList();
            return response;
        }

        private string GenerateJwtToken(ApplicationUser user, IList<string> roles)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var secretKey = jwtSettings["SecretKey"];
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey ?? string.Empty));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.UserName ?? ""),
                new Claim(ClaimTypes.Name, user.FirstName ?? ""),
                new Claim(ClaimTypes.Email, user.Email ?? "")
            };

            // Add danh sách Role vào Token
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(Convert.ToDouble(jwtSettings["ExpireMinutes"])),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
