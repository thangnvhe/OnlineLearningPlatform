using System.Net;

namespace OnlineLearningPlatform.Application.DTOs
{
    public class APIResponse<T> where T : class
    {
        public HttpStatusCode StatusCode { get; set; }
        public bool IsSuccess { get; set; } = true;
        public Dictionary<string, string[]> Errors { get; set; } = new Dictionary<string, string[]>();
        public T? Result { get; set; }
        private APIResponse() { }
        public static APIResponseBuilder<T> Builder() => new APIResponseBuilder<T>();
        public class APIResponseBuilder<TBuilder> where TBuilder : class
        {
            private readonly APIResponse<TBuilder> _response;

            public APIResponseBuilder()
            {
                _response = new APIResponse<TBuilder>();
            }

            public APIResponseBuilder<TBuilder> WithStatusCode(HttpStatusCode statusCode)
            {
                _response.StatusCode = statusCode;
                return this;
            }

            public APIResponseBuilder<TBuilder> WithSuccess(bool isSuccess)
            {
                _response.IsSuccess = isSuccess;
                return this;
            }

            // MỚI: Truyền thẳng một Dictionary lỗi (Thường dùng khi hứng lỗi từ FluentValidation)
            public APIResponseBuilder<TBuilder> WithErrors(Dictionary<string, string[]> errors)
            {
                _response.Errors = errors;
                return this;
            }

            // SỬA LẠI: Nếu chỉ quăng 1 câu thông báo chung chung, tự động nhét vào key "General"
            public APIResponseBuilder<TBuilder> WithMessage(string message)
            {
                return WithMessage("General", message);
            }

            // MỚI: Thêm 1 thông báo lỗi theo Key chỉ định (Ví dụ: WithMessage("Email", "Email đã tồn tại"))
            public APIResponseBuilder<TBuilder> WithMessage(string key, string message)
            {
                // Nếu key đã có trong Dictionary, ta nối thêm thông báo vào mảng cũ
                if (_response.Errors.ContainsKey(key))
                {
                    var existingMessages = _response.Errors[key].ToList();
                    existingMessages.Add(message);
                    _response.Errors[key] = existingMessages.ToArray();
                }
                // Nếu key chưa có, tạo mảng mới
                else
                {
                    _response.Errors[key] = new string[] { message };
                }

                return this;
            }

            public APIResponseBuilder<TBuilder> WithResult(TBuilder result)
            {
                _response.Result = result;
                return this;
            }

            public APIResponse<TBuilder> Build()
            {
                return _response;
            }
        }
    }
}
