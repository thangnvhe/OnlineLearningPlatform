using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineLearningPlatform.Application.Exceptions
{
    public static class ErrorCodes
    {
        public static Error EntityNotFound(string entityName, object key)
        {
            return new Error($"{entityName} with ID {key} was not found.", System.Net.HttpStatusCode.NotFound, $"{entityName}Id");
        }
        public static Error ValidationError(string message, string key = "General")
        {
            return new Error(message, System.Net.HttpStatusCode.BadRequest, key);
        }
        public static Error InternalServerError(string message)
        {
            return new Error(message, System.Net.HttpStatusCode.InternalServerError);
        }
        public static Error InvalidCredentials(string message)
        {
            return new Error(message, System.Net.HttpStatusCode.Unauthorized, "Credentials");
        }
        public static Error LockAccount(string message)
        {
            return new Error(message, System.Net.HttpStatusCode.Forbidden, "Lockout");
        }
    }
}
