using HeThongChungCu.Domain.Common;
using HeThongChungCu.WebAPI.Common.Models;
using Microsoft.AspNetCore.Mvc;

namespace HeThongChungCu.WebAPI.Controllers;

[ApiController]
public abstract class ApiControllerBase : ControllerBase
{
    protected IActionResult HandleResult<T>(Result<T> result)
    {
        var response = new ApiResponse<T>
        {
            IsOk = result.IsSuccess,
            WarningMessages = result.Warnings?.ToList() ?? new List<string>(),
            Errors = result.Errors?.ToList() ?? new List<Error>()
        };

        if (result.IsSuccess)
        {
            response.Result = result.Value;
            return Ok(response);
        }

        return BadRequest(response);
    }
}
