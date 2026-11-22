using Microsoft.AspNetCore.Mvc;
using Application.StatusCodes;
namespace API.Extensions;

public static class ServiceStatusCodeExtensions
{
    public static IActionResult ToActionResult<T>(this ControllerBase controller, ServiceStatusCode<T> result)
    {
        return result.Type switch
        {
            StatusCode.Ok => controller.Ok(result.Data),
            StatusCode.Created => controller.StatusCode(201, result.Data),
            StatusCode.NoContent => controller.NoContent(),
            StatusCode.BadRequest => controller.Problem(detail: result.Error, statusCode: 400),
            StatusCode.NotFound => controller.Problem(detail: result.Error, statusCode: 404),
            StatusCode.Conflict => controller.Problem(detail: result.Error, statusCode: 409),
            _ => controller.Problem(statusCode: 500)
        };
    }
}