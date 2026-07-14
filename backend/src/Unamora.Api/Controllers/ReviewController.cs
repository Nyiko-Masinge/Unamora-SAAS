using MediatR;
using Microsoft.AspNetCore.Mvc;
using Unamora.Application.Modules.Bookings.Commands;

namespace Unamora.Api.Controllers;

[ApiController]
[Route("api/reviews")]
public class ReviewController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<Guid>> SubmitReview([FromBody] SubmitReviewCommand command)
    {
        var id = await mediator.Send(command);
        return Ok(id);
    }
}
