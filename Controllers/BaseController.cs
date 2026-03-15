using Microsoft.AspNetCore.Mvc;
using Models;
    
namespace HomeSense.Api.Controllers;

[ApiController]
public abstract class BaseController : ControllerBase
{
    protected IActionResult FromResult(Result result)
    {
        if (result.IsSuccess)
            return Ok();

        return FromError(result.Error!);
    }

    protected IActionResult FromResult<T>(Result<T> result)
    {
        if (result.IsSuccess)
            return Ok(result.Data);

        return FromError(result.Error!);
    }

    protected IActionResult FromError(Error error)
    {
        return error.Code switch
        {
            "validation_error" => BadRequest(error),
            "not_found" => NotFound(error),
            "conflict" => Conflict(error),
            "unauthorized" => Unauthorized(error),
            _ => StatusCode(StatusCodes.Status500InternalServerError, error)
        };
    }
}