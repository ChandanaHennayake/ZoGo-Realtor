using Microsoft.AspNetCore.Mvc;
using zogo.Application.Common;

namespace zogo.API.Controllers;

[ApiController]
public abstract class ApiControllerBase : ControllerBase
{
    protected ActionResult<ApiResponse<T>> OkResponse<T>(T data, string? message = null) =>
        Ok(ApiResponse<T>.Ok(data, message));

    protected ActionResult<ApiResponse> OkResponse(string? message = null) =>
        Ok(ApiResponse.Ok(message));

    protected ActionResult<ApiResponse<T>> CreatedResponse<T>(string actionName, object routeValues, T data) =>
        CreatedAtAction(actionName, routeValues, ApiResponse<T>.Ok(data));

    protected ActionResult<ApiResponse> NoContentResponse() =>
        Ok(ApiResponse.Ok("Operation completed successfully."));
}
