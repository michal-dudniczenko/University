using Microsoft.AspNetCore.Mvc;
using Soundmates.Application.Common;

namespace Soundmates.Api.Extensions;

public static class ControllerExtensions
{
    public static ActionResult<T> ResultToHttpResponse<T>(
        this ControllerBase controller,
        Result<T> result)
    {
        if (result.IsSuccess)
        {
            return controller.Ok(result.Value);
        }

        return result.ErrorType switch
        {
            ErrorType.BadRequest => controller.BadRequest(new { message = result.ErrorMessage }),
            ErrorType.Unauthorized => controller.Unauthorized(new { message = result.ErrorMessage }),
            ErrorType.NotFound => controller.NotFound(new { message = result.ErrorMessage }),
            ErrorType.InternalServerError or _ => controller.StatusCode(500, new { message = result.ErrorMessage })
        };
    }

    public static ActionResult ResultToHttpResponse(
        this ControllerBase controller,
        Result result)
    {
        if (result.IsSuccess)
        {
            return controller.Ok();
        }

        return result.ErrorType switch
        {
            ErrorType.BadRequest => controller.BadRequest(new { message = result.ErrorMessage }),
            ErrorType.Unauthorized => controller.Unauthorized(new { message = result.ErrorMessage }),
            ErrorType.NotFound => controller.NotFound(new { message = result.ErrorMessage }),
            ErrorType.InternalServerError or _ => controller.StatusCode(500, new { message = result.ErrorMessage })
        };
    }
}
