using MediatR;
using Microsoft.AspNetCore.Mvc;
using Unamora.Application.Modules.Bookings.Commands;

namespace Unamora.Api.Controllers;

[ApiController]
[Route("api/tracking")]
public class TrackingController(IMediator mediator) : ControllerBase
{
    [HttpGet("{bookingId:guid}")]
    public async Task<ActionResult<TrackingStateDto>> GetTracking(Guid bookingId)
    {
        var state = await mediator.Send(new GetTrackingStateQuery(bookingId));
        if (state is null) return NotFound("Tracking state not found for this booking.");
        return Ok(state);
    }

    [HttpPost]
    public async Task<ActionResult<bool>> UpdateTracking([FromBody] UpdateTrackingStateCommand command)
    {
        return Ok(await mediator.Send(command));
    }

    [HttpGet("{bookingId:guid}/timeline")]
    public async Task<ActionResult<IReadOnlyList<JobEventLogDto>>> GetTimeline(Guid bookingId)
    {
        return Ok(await mediator.Send(new GetJobEventLogsQuery(bookingId)));
    }
}
