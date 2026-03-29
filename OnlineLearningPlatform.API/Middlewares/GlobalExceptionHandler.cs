using Microsoft.AspNetCore.Diagnostics;
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
            _logger.LogError(exception, "System Failure: {Message}", exception.Message);

            httpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            httpContext.Response.ContentType = "application/json";

            var response = APIResponse<object>.Builder()
                .WithSuccess(false)
                .WithStatusCode(HttpStatusCode.InternalServerError)
                .WithMessage("Hệ thống đang gặp sự cố kỹ thuật. Vui lòng thử lại sau.")
                .Build();

            await httpContext.Response.WriteAsJsonAsync(response, cancellationToken);

            return true; 
        }
    }
}
