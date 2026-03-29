using Microsoft.AspNetCore.Mvc;
using OnlineLearningPlatform.Application.DTOs;
using System.Net;

namespace OnlineLearningPlatform.API.Controllers.Base
{
    public class BaseController : ControllerBase
    {
        protected IActionResult HandleResult<T>(ServiceResult<T> result) 
        {
            if (!result.IsSuccess)
            {
                var response = APIResponse<T>.Builder()
                    .WithSuccess(false)
                    .WithStatusCode(HttpStatusCode.BadRequest)
                    .WithMessage(result.ErrorMessage!)
                    .Build();
                return BadRequest(response);
            }

            var successResponse = APIResponse<T>.Builder()
                .WithSuccess(true)
                .WithStatusCode(HttpStatusCode.OK)
                .WithResult(result.Data!)
                .Build();
            return Ok(successResponse);
        }
    }
}
