using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using OnlineLearningPlatform.Application.Abstract;
using OnlineLearningPlatform.Application.DTOs;
using OnlineLearningPlatform.Application.DTOs.Events;
using OnlineLearningPlatform.Application.DTOs.Request;
using OnlineLearningPlatform.Application.DTOs.Response;
using OnlineLearningPlatform.Application.Helper;
using OnlineLearningPlatform.Domain.Abstract;
using OnlineLearningPlatform.Domain.Entities;
using OnlineLearningPlatform.Domain.Setting;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Web;

namespace OnlineLearningPlatform.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;
        private readonly IEmailService _emailService;
        private readonly IEventPublisher _eventPublisher;
        private readonly ICacheService _cacheService;
        private readonly string FrontEndUrl;

        public AuthService(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IConfiguration configuration, IMapper mapper,
            IHttpContextAccessor httpContextAccessor,
            IUnitOfWork unitOfWork,
            IEmailService emailService,
            IEventPublisher eventPublisher,
            ICacheService cacheService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
            _unitOfWork = unitOfWork;
            _emailService = emailService;
            FrontEndUrl = _configuration.GetValue<string>("FrontEndUrl") ?? "http://localhost:5173";
            _eventPublisher = eventPublisher;
            _cacheService = cacheService;
        }

        public async Task<ServiceResult<AuthResponse>> LoginAsync(LoginRequest request)
        {
            // 0. Validate request
            if (string.IsNullOrEmpty(request.Username) || string.IsNullOrEmpty(request.Password))
                return ServiceResult<AuthResponse>.Failure("Tên đăng nhập và mật khẩu không được để trống.");

            // 1. Tìm kiếm user theo userName bằng UserManager
            var user = await _userManager.FindByNameAsync(request.Username);
            if (user == null)
                return ServiceResult<AuthResponse>.Failure("Tên đăng nhập hoặc mật khẩu không đúng.");

            // 2. Dùng SignInManager để check pass 
            var signInResult = await _signInManager.CheckPasswordSignInAsync(user, request.Password, lockoutOnFailure: false);

            //if (signInResult.IsLockedOut)
            //{
            //    var remainningLockOut = user.LockoutEnd!.Value - DateTimeOffset.UtcNow;
            //    var minutes = Math.Max(1, Math.Round(remainningLockOut.TotalMinutes));
            //    return ServiceResult<AuthResponse>.Failure($"Tài khoản bị khóa do đăng nhập sai quá nhiều lần. Vui lòng thử lại sau {minutes} phút.");
            //}

            if (signInResult.IsNotAllowed)
            {
                return ServiceResult<AuthResponse>.Failure("Tài khoản chưa được xác nhận email. Vui lòng kiểm tra email để xác nhận tài khoản.");
            }

            if (!signInResult.Succeeded)
            {
                return ServiceResult<AuthResponse>.Failure("Tên đăng nhập hoặc mật khẩu không đúng.");
            }

            var cacheKey = $"user_roles_{user.Id}";
            var roles = await _cacheService.GetAsync<IList<string>>(cacheKey);

            if (roles == null)
            {
                roles = await _userManager.GetRolesAsync(user); // Đọc từ DB
                await _cacheService.SetAsync(cacheKey, roles, TimeSpan.FromDays(7)); // Lưu vào Redis 7 ngày
            }

            var accessToken = GenerateJwtToken(user, roles);

            var refreshToken = Utilityz.GenerateRefreshToken();

            var response = _mapper.Map<AuthResponse>(user);
            response.AccessToken = accessToken;

            var refreshTokenKey = $"refresh_token:{refreshToken}";

            await _cacheService.SetAsync(refreshTokenKey, user.Id.ToString(), TimeSpan.FromDays(7));

            _httpContextAccessor.HttpContext!.Response.Cookies.Append("refreshToken", refreshToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = user.RefresherTokenExpiry
            });

            response.Roles = roles.ToList();

            return ServiceResult<AuthResponse>.Success(response);
        }
        public async Task<ServiceResult<bool>> RegisterAsync(RegisterRequest request)
        {
            if (request == null)
                return ServiceResult<bool>.Failure("Dữ liệu đăng ký không hợp lệ.");

            var existingUserByEmail = await _userManager.FindByEmailAsync(request.Email);

            if (existingUserByEmail != null)
                return ServiceResult<bool>.Failure("Email đã tồn tại. Vui lòng sử dụng email khác.");

            var existingUserByUserName = await _userManager.FindByNameAsync(request.Username);

            if (existingUserByUserName != null)
                return ServiceResult<bool>.Failure("Tên đăng nhập đã tồn tại. Vui lòng chọn tên khác.");

            var newUser = _mapper.Map<ApplicationUser>(request);

            var createResult = await _userManager.CreateAsync(newUser, request.Password);

            if (!createResult.Succeeded)
            {
                return ServiceResult<bool>.Failure("Đăng ký thất bại.");
            }

            var roleResult = await _userManager.AddToRoleAsync(newUser, CommonConstant.Role_Student);

            if (!roleResult.Succeeded)
                return ServiceResult<bool>.Failure("Đăng ký thất bại. Lỗi phân quyền");

            try
            {
                var token = await _userManager.GenerateEmailConfirmationTokenAsync(newUser);
                var encodedToken = HttpUtility.UrlEncode(token);

                await _eventPublisher.PublishAsync(new SendEmailEvent
                {
                    To = newUser.Email!,
                    Subject = "Chào mừng bạn đến với Online Learning Platform",
                    TemplateName = "Welcome",
                    Model = new Dictionary<string, string>
                    {
                        { "UserName", newUser.UserName! }
                    }
                });

                var confirmationLink = $"{FrontEndUrl}?userId={newUser.Id}&token={encodedToken}";

                await _eventPublisher.PublishAsync(new SendEmailEvent
                {
                    To = newUser.Email!,
                    Subject = "Xác nhận email đăng ký",
                    TemplateName = "EmailConfirmation",
                    // Chuyển sang dùng Dictionary chứa nhiều cấu hình
                    Model = new Dictionary<string, string>
                    {
                        { "AppName", "Online Learning Platform" },
                        { "UserName", newUser.UserName! },
                        { "ConfirmationLink", confirmationLink }
                    }
                });
            } catch (Exception)
            {
                await _userManager.DeleteAsync(newUser);
                return ServiceResult<bool>.Failure("Hệ thống đang bận hoặc gặp sự cố kết nối. Vui lòng thử lại sau vài phút.");
            }

            return ServiceResult<bool>.Success(true);
        }
        public async Task<ServiceResult<AuthResponse>> RefreshTokenAsync(CancellationToken cancellationToken)
        {
            // 1. Lấy token từ Cookie
            var refreshToken = _httpContextAccessor.HttpContext?.Request.Cookies["refreshToken"];
            if (string.IsNullOrEmpty(refreshToken))
                return ServiceResult<AuthResponse>.Failure("Refresh token không tồn tại.");

            // 2. [REDIS] Kiểm tra Token trong Redis xem có tồn tại không và lấy UserId ra
            var refreshTokenCacheKey = $"refresh_token:{refreshToken}";
            var userIdString = await _cacheService.GetAsync<string>(refreshTokenCacheKey);

            if (string.IsNullOrEmpty(userIdString))
                return ServiceResult<AuthResponse>.Failure("Refresh token không hợp lệ hoặc đã hết hạn.");

            // 3. Tìm User chuẩn từ DB bằng Id (Chỉ đọc 1 bản ghi duy nhất, rất nhanh)
            var user = await _userManager.FindByIdAsync(userIdString);
            if (user == null)
                return ServiceResult<AuthResponse>.Failure("Người dùng không tồn tại.");

            // 4. Lấy Roles (Ưu tiên Redis như bạn đã làm)
            var roleCacheKey = $"user_roles_{user.Id}";
            var roles = await _cacheService.GetAsync<IList<string>>(roleCacheKey);
            if (roles == null)
            {
                roles = await _userManager.GetRolesAsync(user);
                await _cacheService.SetAsync(roleCacheKey, roles, TimeSpan.FromDays(7));
            }

            // 5. Tạo Access Token mới và Refresh Token mới
            var newAccessToken = GenerateJwtToken(user, roles);

            var newRefreshToken = Utilityz.GenerateRefreshToken();
            var refreshTokenExpiry = DateTime.UtcNow.AddDays(7);

            // 6. [REDIS] Xóa Token cũ và Lưu Token mới vào Redis
            await _cacheService.RemoveAsync(refreshTokenCacheKey); // Xóa cái cũ cho sạch
            var newRefreshTokenCacheKey = $"refresh_token:{newRefreshToken}";
            await _cacheService.SetAsync(newRefreshTokenCacheKey, user.Id.ToString(), TimeSpan.FromDays(7));

            // 7. Cập nhật Cookie
            _httpContextAccessor.HttpContext!.Response.Cookies.Append("refreshToken", newRefreshToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = refreshTokenExpiry
            });

            // 8. Map kết quả trả về
            var response = _mapper.Map<AuthResponse>(user);
            response.AccessToken = newAccessToken;
            response.Roles = roles.ToList();

            return ServiceResult<AuthResponse>.Success(response);
        }

        public async Task<ServiceResult<bool>> Logout(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return ServiceResult<bool>.Failure("Người dùng không tồn tại.");
            }

            user.RefresherToken = null;
            user.RefresherTokenExpiry = null;

            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                return ServiceResult<bool>.Failure("Lỗi hệ thống khi đăng xuất.");
            }

            return ServiceResult<bool>.Success(true);
        }

        public async Task<ServiceResult<bool>> ResetPasswordAsync(ResetPasswordRequest request)
        {
            if (request == null)
                return ServiceResult<bool>.Failure("Dữ liệu thay đổi mật khẩu không hợp lệ.");

            if (string.IsNullOrEmpty(request.UserId) || string.IsNullOrEmpty(request.Token) || string.IsNullOrEmpty(request.NewPassword))
                return ServiceResult<bool>.Failure("Dữ liệu thay đổi mật khẩu không hợp lệ.");

            var user = await _userManager.FindByIdAsync(request.UserId);
            if (user == null)
                return ServiceResult<bool>.Failure("Người dùng không tồn tại.");

            var decodedToken = HttpUtility.UrlDecode(request.Token);
            var resetResult = await _userManager.ResetPasswordAsync(user, decodedToken, request.NewPassword);

            if (!resetResult.Succeeded)
                return ServiceResult<bool>.Failure("Không thể thay đổi mật khẩu. " + string.Join("; ", resetResult.Errors.Select(e => e.Description)));

            await _cacheService.RemoveAsync($"user_roles_{request.UserId}");

            return ServiceResult<bool>.Success(true);
        }

        public async Task<ServiceResult<bool>> ForgotPasswordAsync(ForgotPasswordRequest request)
        {
            if (request == null || string.IsNullOrEmpty(request.Email))
                return ServiceResult<bool>.Failure("Email không hợp lệ.");

            var user = await _userManager.FindByEmailAsync(request.Email);

            if (user == null)
                return ServiceResult<bool>.Failure("Email không tồn tại.");

            if (!user.EmailConfirmed)
                return ServiceResult<bool>.Failure("Email chưa được xác thực, bạn cần xác thực email trước khi thực hiện thao tác này");

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var encodedToken = HttpUtility.UrlEncode(token);

            var resetLink = $"{FrontEndUrl}/reset-password?userId={user.Id}&token={encodedToken}";

            try
            {
                await _eventPublisher.PublishAsync(new SendEmailEvent
                {
                    To = user.Email!,
                    Subject = "Yêu cầu đặt lại mật khẩu",
                    TemplateName = "ResetPassword",
                    Model = new Dictionary<string, string>
                    {
                        { "AppName", "Online Learning Platform" },
                        { "UserId", user.Id.ToString() },
                        { "Token", encodedToken },
                        { "ResetLink", resetLink }
                    }
                });
            } catch (Exception)
            {
                return ServiceResult<bool>.Failure("Hệ thống đang bận hoặc gặp sự cố kết nối. Vui lòng thử lại sau vài phút.");
            }
            return ServiceResult<bool>.Success(true);
        }

        public async Task<ServiceResult<bool>> ChangePasswordAsync(ChangePasswordRequest request)
        {
            if (request == null || string.IsNullOrEmpty(request.UserId) || string.IsNullOrEmpty(request.CurrentPassword) || string.IsNullOrEmpty(request.NewPassword))
                return ServiceResult<bool>.Failure("Dữ liệu thay đổi mật khẩu không hợp lệ.");

            var user = await _userManager.FindByIdAsync(request.UserId);

            if (user == null)
                return ServiceResult<bool>.Failure("Người dùng không tồn tại.");

            var changeResult = await _userManager.ChangePasswordAsync(user, request.CurrentPassword, request.NewPassword);

            if (!changeResult.Succeeded)
                return ServiceResult<bool>.Failure("Không thể thay đổi mật khẩu. ");

            await _cacheService.RemoveAsync($"user_roles_{request.UserId}");

            return ServiceResult<bool>.Success(true);
        }

        public async Task<ServiceResult<bool>> ConfirmEmailAsync(ConfirmEmailRequest request)
        {
            if (request == null || string.IsNullOrEmpty(request.UserId) || string.IsNullOrEmpty(request.Token))
                return ServiceResult<bool>.Failure("Dữ liệu xác nhận email không hợp lệ.");

            var user = await _userManager.FindByIdAsync(request.UserId);
            if (user == null)
                return ServiceResult<bool>.Failure("Người dùng không tồn tại.");

            if (user.EmailConfirmed)
                return ServiceResult<bool>.Failure("Email đã được xác nhận.");

            var decodedToken = HttpUtility.UrlDecode(request.Token);
            var confirmResult = await _userManager.ConfirmEmailAsync(user, decodedToken);

            if (!confirmResult.Succeeded)
                return ServiceResult<bool>.Failure("Token xác nhận không hợp lệ hoặc đã hết hạn.");

            return ServiceResult<bool>.Success(true);
        }

        private string GenerateJwtToken(ApplicationUser user, IList<string> roles)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var secretKey = jwtSettings["SecretKey"] ?? throw new Exception("JWT Secret Key is missing!");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                // Dùng ClaimTypes.NameIdentifier cho UserId để đồng bộ với Identity
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName ?? ""),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()), // ID duy nhất cho mỗi Token
                new Claim(ClaimTypes.Name, user.FirstName ?? ""),
                new Claim(ClaimTypes.Email, user.Email ?? "")
            };

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            // SỬA QUAN TRỌNG: Dùng UtcNow và sửa đúng tên ExpiryMinutes
            var expiryMinutes = Convert.ToDouble(jwtSettings["ExpiryMinutes"] ?? "60");

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                notBefore: DateTime.UtcNow, // Token có hiệu lực ngay bây giờ
                expires: DateTime.UtcNow.AddMinutes(expiryMinutes), // Hết hạn theo giờ chuẩn UTC
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<ServiceResult<bool>> ResendConfirmationEmail(ResendConfirmationEmail request)
        {
           if(string.IsNullOrEmpty(request.Username))
                return ServiceResult<bool>.Failure("Tên đăng nhập không được để trống.");

            var user = await _userManager.FindByNameAsync(request.Username);

            if (user == null)
                return ServiceResult<bool>.Failure("Tên đăng nhập không tồn tại.");

            if (user.EmailConfirmed)
                return ServiceResult<bool>.Failure("Tài khoản này đã được xác nhận email từ trước. Vui lòng đăng nhập.");

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var encodedToken = HttpUtility.UrlEncode(token);
            var confirmationLink = $"{FrontEndUrl}?userId={user.Id}&token={encodedToken}";

            await _eventPublisher.PublishAsync(new SendEmailEvent
            {
                To = user.Email!,
                Subject = "Xác nhận email đăng ký",
                TemplateName = "EmailConfirmation",
                Model = new Dictionary<string, string> 
                { { "AppName", "Online Learning Platform" }, 
                  { "UserName", user.UserName! },
                  { "ConfirmationLink", confirmationLink } }
            });

            return ServiceResult<bool>.Success(true);
        }
    }
}
