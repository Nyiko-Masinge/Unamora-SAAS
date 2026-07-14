using MediatR;
using Microsoft.AspNetCore.Mvc;
using Unamora.Application.Modules.Profiles.Commands;

namespace Unamora.Api.Controllers;

[ApiController]
[Route("api/client")]
public class ClientProfileController(IMediator mediator) : ControllerBase
{
    [HttpGet("profile")]
    public async Task<ActionResult<ClientProfileDto>> GetProfile()
    {
        var profile = await mediator.Send(new GetClientProfileQuery());
        if (profile is null) return NotFound("Client profile not found. Try updating your profile first.");
        return Ok(profile);
    }

    [HttpPut("profile")]
    public async Task<ActionResult<bool>> UpdateProfile([FromBody] UpdateClientProfileCommand command)
    {
        return Ok(await mediator.Send(command));
    }

    [HttpGet("addresses")]
    public async Task<ActionResult<IReadOnlyList<AddressDto>>> GetAddresses()
    {
        return Ok(await mediator.Send(new GetAddressesQuery()));
    }

    [HttpPost("addresses")]
    public async Task<ActionResult<Guid>> AddAddress([FromBody] AddAddressCommand command)
    {
        var id = await mediator.Send(command);
        return CreatedAtAction(nameof(GetAddresses), new { id }, id);
    }

    [HttpDelete("addresses/{id:guid}")]
    public async Task<ActionResult<bool>> DeleteAddress(Guid id)
    {
        return Ok(await mediator.Send(new DeleteAddressCommand(id)));
    }

    [HttpGet("payment-methods")]
    public async Task<ActionResult<IReadOnlyList<PaymentMethodDto>>> GetPaymentMethods()
    {
        return Ok(await mediator.Send(new GetPaymentMethodsQuery()));
    }

    [HttpPost("payment-methods")]
    public async Task<ActionResult<Guid>> AddPaymentMethod([FromBody] AddPaymentMethodCommand command)
    {
        var id = await mediator.Send(command);
        return CreatedAtAction(nameof(GetPaymentMethods), new { id }, id);
    }

    [HttpDelete("payment-methods/{id:guid}")]
    public async Task<ActionResult<bool>> DeletePaymentMethod(Guid id)
    {
        return Ok(await mediator.Send(new DeletePaymentMethodCommand(id)));
    }

    [HttpGet("preferences")]
    public async Task<ActionResult<ClientPreferenceDto>> GetPreferences()
    {
        return Ok(await mediator.Send(new GetClientPreferenceQuery()));
    }

    [HttpPut("preferences")]
    public async Task<ActionResult<bool>> UpdatePreferences([FromBody] UpdateClientPreferenceCommand command)
    {
        return Ok(await mediator.Send(command));
    }
}
