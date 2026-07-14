using Microsoft.AspNetCore.Mvc;
using Unamora.Application.Common.Interfaces;

namespace Unamora.Api.Controllers;

[ApiController]
[Route("api/matching")]
public class MatchingController(IMatchingService matchingService) : ControllerBase
{
    [HttpGet("matches")]
    public async Task<ActionResult<IEnumerable<MatchingResultDto>>> GetMatches([FromQuery] Guid jobRequestId)
    {
        var matches = await matchingService.FindMatchesAsync(jobRequestId);
        return Ok(matches);
    }
}
