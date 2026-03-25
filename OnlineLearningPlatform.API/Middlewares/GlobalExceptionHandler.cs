using Microsoft.AspNetCore.Diagnostics;
using OnlineLearningPlatform.Application.DTOs;
using OnlineLearningPlatform.Application.Exceptions;
using System.Net;

namespace OnlineLearningPlatform.API.Middlewares
{
    public class GlobalExceptionHandler : IExceptionHandler
    {
        private readonly ILogger<GlobalExceptionHandler> _logger;
        public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
        {
            this._logger = logger;
        }
        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            // Khởi tạo sẵn một Builder mặc định là thất bại
            var responseBuilder = APIResponse<object>.Builder().WithSuccess(false);

            // 1. Nếu đây là lỗi Nghiệp vụ do chính chúng ta chủ động ném ra (AppException)
            if (exception is AppException appException)
            {
                var error = appException.Error;

                // Gán Status Code cho cái Response của HTTP
                httpContext.Response.StatusCode = (int)error.StatusCode;

                // Gắn dữ liệu vào phong bì APIResponse
                responseBuilder
                    .WithStatusCode(error.StatusCode)
                    .WithMessage(error.Key, error.Message); // Dùng Key để thả đúng thông báo vào Dictionary
            }
            // 2. Nếu là các lỗi hệ thống không lường trước (Bug code, đứt mạng, sập DB...)
            else
            {
                _logger.LogError(exception, "Có lỗi hệ thống cực nghiêm trọng xảy ra.");

                httpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

                responseBuilder
                    .WithStatusCode(HttpStatusCode.InternalServerError)
                    .WithMessage("System", "Hệ thống đang gặp sự cố. Vui lòng thử lại sau.");
            }

            // Đóng gói phong bì
            var response = responseBuilder.Build();

            // Cấu hình kiểu dữ liệu trả về và đẩy xuống Frontend
            httpContext.Response.ContentType = "application/json";
            await httpContext.Response.WriteAsJsonAsync(response, cancellationToken);

            // Trả về true để ra hiệu: "Tôi (GlobalExceptionHandler) đã xử lý xong lỗi này rồi, đường ống dừng tại đây nhé!"
            return true;
        }
    }
}
