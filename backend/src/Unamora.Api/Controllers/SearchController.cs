using MediatR;
using Microsoft.AspNetCore.Mvc;
using Unamora.Application.Modules.Catalog.Commands;

namespace Unamora.Api.Controllers;

[ApiController]
[Route("api/search")]
public class SearchController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<SearchResultDto>>> Search(
        [FromQuery] string? tradeName,
        [FromQuery] int? tradeId,
        [FromQuery] decimal? lat,
        [FromQuery] decimal? lon,
        [FromQuery] decimal? radius,
        [FromQuery] string? availability,
        [FromQuery] decimal? minRating,
        [FromQuery] decimal? maxPrice,
        [FromQuery] string? province,
        [FromQuery] string? city,
        [FromQuery] string? business,
        [FromQuery] string? languageCode,
        [FromQuery] bool? emergency)
    {
        var query = new AdvancedSearchQuery(
            TradeName: tradeName,
            TradeId: tradeId,
            Latitude: lat,
            Longitude: lon,
            RadiusKm: radius,
            Availability: availability,
            MinRating: minRating,
            MaxPrice: maxPrice,
            Province: province,
            City: city,
            Business: business,
            LanguageCode: languageCode,
            EmergencyOnly: emergency
        );

        return Ok(await mediator.Send(query));
    }

    [HttpPost("ai")]
    public async Task<ActionResult<IReadOnlyList<SearchResultDto>>> AiSearch([FromBody] AiSearchRequest request)
    {
        return Ok(await mediator.Send(new AiSearchQuery(request.Prompt)));
    }
}

public class AiSearchRequest
{
    public string Prompt { get; set; } = string.Empty;
}
