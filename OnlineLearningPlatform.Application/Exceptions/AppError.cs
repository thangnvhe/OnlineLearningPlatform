using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace OnlineLearningPlatform.Application.Exceptions
{
    public class AppException : Exception
    {
        public Error Error { get; }

        public AppException(Error error) : base(error.Message)
        {
            Error = error;
        }
    }
}
