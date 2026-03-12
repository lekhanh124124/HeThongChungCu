using HeThongChungCu.Application.Features.Seeder.Commands.SeedDatabase;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace HeThongChungCu.WebAPI.Controllers;

[ApiController]
[Route("api/seeder")]
public class SeederController : ControllerBase
{
    private readonly ISender _sender;

    public SeederController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>
    /// Seeds the database with a specified number of mock records.
    /// </summary>
    /// <param name="command">The seeding parameters.</param>
    /// <returns>A success message or error.</returns>
    [HttpPost]
    public async Task<IActionResult> SeedDatabase([FromBody] SeedDatabaseCommand command)
    {
        var result = await _sender.Send(command);

        if (result.IsFailure)
        {
            return BadRequest(result.Errors);
        }

        return Ok(new { Message = result.Value });
    }
}
