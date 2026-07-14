using MediatR;
using Microsoft.AspNetCore.Mvc;
using Unamora.Application.Modules.Profiles.Commands;

namespace Unamora.Api.Controllers;

[ApiController]
[Route("api/client")]
public class FavoritesController(IMediator mediator) : ControllerBase
{
    [HttpGet("favorites")]
    public async Task<ActionResult<IReadOnlyList<SavedTradespersonDto>>> GetFavorites()
    {
        return Ok(await mediator.Send(new GetSavedTradespersonsQuery()));
    }

    [HttpPost("favorites")]
    public async Task<ActionResult<bool>> ToggleFavorite([FromBody] ToggleFavoriteRequest request)
    {
        return Ok(await mediator.Send(new ToggleSaveTradespersonCommand(request.TradespersonProfileId)));
    }

    [HttpGet("recently-viewed")]
    public async Task<ActionResult<IReadOnlyList<RecentlyViewedDto>>> GetRecentlyViewed()
    {
        return Ok(await mediator.Send(new GetRecentlyViewedTradespersonsQuery()));
    }

    [HttpPost("recently-viewed")]
    public async Task<ActionResult<bool>> AddRecentlyViewed([FromBody] AddRecentlyViewedRequest request)
    {
        return Ok(await mediator.Send(new AddRecentlyViewedTradespersonCommand(request.TradespersonProfileId)));
    }

    [HttpGet("recommendations")]
    public async Task<ActionResult<IReadOnlyList<RecommendedTradespersonDto>>> GetRecommendations()
    {
        return Ok(await mediator.Send(new GetRecommendationsQuery()));
    }
}

public class ToggleFavoriteRequest
{
    public Guid TradespersonProfileId { get; set; }
}

public class AddRecentlyViewedRequest
{
    public Guid TradespersonProfileId { get; set; }
}
