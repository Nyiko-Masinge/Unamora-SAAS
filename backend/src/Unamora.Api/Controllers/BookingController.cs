using MediatR;
using Microsoft.AspNetCore.Mvc;
using Unamora.Application.Modules.Bookings.Commands;

namespace Unamora.Api.Controllers;

[ApiController]
[Route("api/bookings")]
public class BookingController(IMediator mediator) : ControllerBase
{
    [HttpPost("job-requests")]
    public async Task<ActionResult<Guid>> CreateJobRequest([FromBody] CreateJobRequestCommand command)
    {
        var id = await mediator.Send(command);
        return Ok(id);
    }

    [HttpGet("job-requests/{id:guid}")]
    public async Task<ActionResult<JobRequestDto>> GetJobRequest(Guid id)
    {
        var jr = await mediator.Send(new GetJobRequestQuery(id));
        if (jr is null) return NotFound("Job request not found");
        return Ok(jr);
    }

    [HttpPost("quotes")]
    public async Task<ActionResult<Guid>> SubmitQuote([FromBody] SubmitQuoteCommand command)
    {
        var id = await mediator.Send(command);
        return Ok(id);
    }

    [HttpPost("quotes/{id:guid}/accept")]
    public async Task<ActionResult<Guid>> AcceptQuote(Guid id)
    {
        var bookingId = await mediator.Send(new AcceptQuoteCommand(id));
        return Ok(bookingId);
    }

    [HttpPost("quotes/{id:guid}/decline")]
    public async Task<ActionResult<bool>> DeclineQuote(Guid id)
    {
        return Ok(await mediator.Send(new DeclineQuoteCommand(id)));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<BookingDto>> GetBooking(Guid id)
    {
        var booking = await mediator.Send(new GetBookingQuery(id));
        if (booking is null) return NotFound("Booking not found");
        return Ok(booking);
    }

    [HttpGet("client")]
    public async Task<ActionResult<IReadOnlyList<BookingDto>>> GetClientBookings()
    {
        return Ok(await mediator.Send(new GetClientBookingsQuery()));
    }

    [HttpGet("tradesperson/{id:guid}")]
    public async Task<ActionResult<IReadOnlyList<BookingDto>>> GetTradespersonBookings(Guid id)
    {
        return Ok(await mediator.Send(new GetTradespersonBookingsQuery(id)));
    }

    [HttpPost("{id:guid}/complete")]
    public async Task<ActionResult<bool>> CompleteBooking(Guid id)
    {
        return Ok(await mediator.Send(new CompleteBookingCommand(id)));
    }

    [HttpPost("{id:guid}/cancel")]
    public async Task<ActionResult<bool>> CancelBooking(Guid id, [FromBody] CancelBookingRequest request)
    {
        return Ok(await mediator.Send(new CancelBookingCommand(id, request.Reason)));
    }
}

public class CancelBookingRequest
{
    public string Reason { get; set; } = string.Empty;
}
