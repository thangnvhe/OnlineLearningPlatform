using System.Net;

namespace OnlineLearningPlatform.API
{
    public class APIResponse<T> 
    {
        public HttpStatusCode StatusCode { get; set; }
        public bool IsSuccess { get; set; } = true;
        public List<string>? ErrorMessages { get; set; }
        public T? Result { get; set; }
        private APIResponse() { }
        public static APIResponseBuilder<T> Builder() => new APIResponseBuilder<T>();
        public class APIResponseBuilder<TBuilder> 
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

            public APIResponseBuilder<TBuilder> WithErrors(List<string> errors)
            {
                _response.ErrorMessages = errors;
                return this;
            }

            public APIResponseBuilder<TBuilder> WithMessage(string message)
            {
                if (_response.ErrorMessages == null)
                {
                    _response.ErrorMessages = new List<string>();
                }

                _response.ErrorMessages.Add(message);
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
