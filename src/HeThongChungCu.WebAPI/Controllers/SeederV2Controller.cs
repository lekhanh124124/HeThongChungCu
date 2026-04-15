// Demo Versioning: SeederController v1.0 giữ nguyên, thêm SeederV2Controller v2.0 với response có thêm ApiVersion để demo versioning.
using HeThongChungCu.Application.Features.Seeder.Commands.SeedDatabase;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Asp.Versioning;

namespace HeThongChungCu.WebAPI.Controllers;

[ApiController]
[ApiVersion("2.0")]
[Route("api/seeder")]
public class SeederV2Controller : ControllerBase
{
    private readonly ISender _sender;

    public SeederV2Controller(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>
    /// (v2) Ping endpoint để demo versioning.
    /// </summary>
    [HttpGet("ping")]
    public IActionResult Ping()
    {
        return Ok(new { ApiVersion = "2.0", Message = "Seeder v2 is alive" });
    }

    /// <summary>
    /// (v2) Seeds the database with a specified number of mock records.
    /// </summary>
    /// <remarks>
    /// Demo v2: giữ nguyên route/verb nhưng response có thêm ApiVersion.
    /// </remarks>
    [HttpPost]
    public async Task<IActionResult> SeedDatabase([FromBody] SeedDatabaseCommand command, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(new { ApiVersion = "2.0", Errors = result.Errors });
        }

        return Ok(new { ApiVersion = "2.0", Message = result.Value });
    }
}
