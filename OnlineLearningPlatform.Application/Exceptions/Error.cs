using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace OnlineLearningPlatform.Application.Exceptions
{
    public class Error
    {
        public string Key { get; set; } // Thêm Key để FE biết lỗi ở ô nào
        public string Message { get; set; }
        public HttpStatusCode StatusCode { get; set; }

        // Gán giá trị mặc định cho key là "General"
        public Error(string message, HttpStatusCode statusCode, string key = "General")
        {
            Message = message;
            StatusCode = statusCode;
            Key = key;
        }
    }
}
