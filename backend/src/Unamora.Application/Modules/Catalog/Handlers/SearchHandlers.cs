using MediatR;
using Unamora.Application.Common.Interfaces;
using Unamora.Application.Modules.Catalog.Commands;
using Unamora.Domain.Entities.Profiles;
using Unamora.Domain.Enums;

namespace Unamora.Application.Modules.Catalog.Handlers;

public class AdvancedSearchQueryHandler(
    ITradespersonProfileRepository tradespersonRepo,
    ITradeRepository tradeRepo,
    IUserRepository userRepo) : IRequestHandler<AdvancedSearchQuery, IReadOnlyList<SearchResultDto>>
{
    public async Task<IReadOnlyList<SearchResultDto>> Handle(AdvancedSearchQuery request, CancellationToken cancellationToken)
    {
        var tradespersons = await tradespersonRepo.GetAllAsync(cancellationToken);

        // Filter by Trade / Skill
        if (request.TradeId.HasValue)
        {
            var trade = await tradeRepo.GetByIdAsync(request.TradeId.Value, cancellationToken);
            if (trade != null)
            {
                tradespersons = tradespersons.Where(x => x.Skills.Any(s => s.Skill != null && s.Skill.ServiceCategoryId == trade.ServiceCategoryId)).ToList();
            }
        }
        else if (!string.IsNullOrWhiteSpace(request.TradeName))
        {
            var tName = request.TradeName.ToLower();
            tradespersons = tradespersons.Where(x => 
                (x.BusinessName != null && x.BusinessName.ToLower().Contains(tName))
                || (x.Headline != null && x.Headline.ToLower().Contains(tName))
                || x.Skills.Any(s => s.Skill != null && s.Skill.Name.ToLower().Contains(tName))
            ).ToList();
        }

        // Filter by Availability
        if (!string.IsNullOrWhiteSpace(request.Availability))
        {
            if (Enum.TryParse<AvailabilityStatus>(request.Availability, true, out var status))
            {
                tradespersons = tradespersons.Where(x => x.AvailabilityStatus == status).ToList();
            }
        }

        // Filter by Rating
        if (request.MinRating.HasValue)
        {
            tradespersons = tradespersons.Where(x => x.AverageRating >= request.MinRating.Value).ToList();
        }

        // Filter by MaxPrice
        if (request.MaxPrice.HasValue)
        {
            tradespersons = tradespersons.Where(x => x.HourlyRateMin <= request.MaxPrice.Value).ToList();
        }

        // Filter by Emergency
        if (request.EmergencyOnly == true)
        {
            tradespersons = tradespersons.Where(x => 
                x.AvailabilityStatus == AvailabilityStatus.Online 
                || (x.Headline != null && x.Headline.ToLower().Contains("emergency")) 
                || (x.BusinessName != null && x.BusinessName.ToLower().Contains("express"))
            ).ToList();
        }

        var searchResults = new List<SearchResultDto>();

        foreach (var tp in tradespersons)
        {
            var user = await userRepo.GetByIdAsync(tp.UserId, cancellationToken);
            var distance = 0.0;

            if (request.Latitude.HasValue && request.Longitude.HasValue && tp.BaseLatitude.HasValue && tp.BaseLongitude.HasValue)
            {
                distance = CalculateDistance(
                    (double)request.Latitude.Value,
                    (double)request.Longitude.Value,
                    (double)tp.BaseLatitude.Value,
                    (double)tp.BaseLongitude.Value);
            }

            // Radius Filter
            if (request.RadiusKm.HasValue && distance > (double)request.RadiusKm.Value)
            {
                continue;
            }

            // Calculation of temporary match score
            var score = 100.0;
            if (distance > 0)
            {
                score -= Math.Min(30.0, distance * 1.5); // deduct points for distance
            }
            score += (double)tp.AverageRating * 2.0; // add rating weight
            score = Math.Min(100.0, Math.Max(0.0, score));

            searchResults.Add(new SearchResultDto
            {
                TradespersonId = tp.Id,
                BusinessName = tp.BusinessName ?? "Independent Contractor",
                TradespersonName = user != null ? $"{user.FirstName} {user.LastName}" : "Unknown",
                Headline = tp.Headline ?? string.Empty,
                Bio = tp.Bio ?? string.Empty,
                YearsOfExperience = tp.YearsOfExperience,
                AverageRating = tp.AverageRating,
                ReviewCount = tp.ReviewCount,
                CompletedJobs = tp.CompletedJobsCount,
                ServiceRadiusKm = tp.ServiceRadiusKm,
                HourlyRateMin = tp.HourlyRateMin ?? 0,
                HourlyRateMax = tp.HourlyRateMax ?? 0,
                AvailabilityStatus = tp.AvailabilityStatus.ToString(),
                HasPoliceClearance = tp.HasPoliceClearance,
                IsVerified = tp.VerificationStatus == TradespersonVerificationStatus.Approved,
                DistanceKm = (decimal)Math.Round(distance, 2),
                MatchScore = Math.Round(score, 1)
            });
        }

        return searchResults.OrderByDescending(r => r.MatchScore).ToList();
    }

    private static double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
    {
        var r = 6371; // km
        var dLat = ToRadians(lat2 - lat1);
        var dLon = ToRadians(lon2 - lon1);
        var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                Math.Cos(ToRadians(lat1)) * Math.Cos(ToRadians(lat2)) *
                Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
        var c = 2 * Math.Asin(Math.Sqrt(a));
        return r * c;
    }

    private static double ToRadians(double val) => (Math.PI / 180.0) * val;
}

public class AiSearchQueryHandler(
    IMediator mediator) : IRequestHandler<AiSearchQuery, IReadOnlyList<SearchResultDto>>
{
    public async Task<IReadOnlyList<SearchResultDto>> Handle(AiSearchQuery request, CancellationToken cancellationToken)
    {
        var prompt = request.Prompt.ToLower();
        int? matchedTradeId = null;
        string? tradeName = null;

        // Classify the natural language prompt
        if (prompt.Contains("leak") || prompt.Contains("pipe") || prompt.Contains("water") || prompt.Contains("clog") || prompt.Contains("toilet") || prompt.Contains("sink") || prompt.Contains("drain") || prompt.Contains("plumb"))
        {
            tradeName = "Plumbing";
        }
        else if (prompt.Contains("wire") || prompt.Contains("light") || prompt.Contains("power") || prompt.Contains("electricity") || prompt.Contains("db board") || prompt.Contains("circuit") || prompt.Contains("solar") || prompt.Contains("electric"))
        {
            tradeName = "Electrical";
        }
        else if (prompt.Contains("wood") || prompt.Contains("cabinet") || prompt.Contains("door") || prompt.Contains("deck") || prompt.Contains("furniture") || prompt.Contains("carpenter"))
        {
            tradeName = "Carpentry";
        }

        var searchRequest = new AdvancedSearchQuery(
            TradeName: tradeName,
            TradeId: matchedTradeId,
            Latitude: -26.145m, // default client location JHB
            Longitude: 28.033m,
            RadiusKm: 50m,
            Availability: null,
            MinRating: null,
            MaxPrice: null,
            Province: null,
            City: null,
            Business: null,
            LanguageCode: null,
            EmergencyOnly: prompt.Contains("emergency") || prompt.Contains("urgent") || prompt.Contains("burst") || prompt.Contains("outage")
        );

        return await mediator.Send(searchRequest, cancellationToken);
    }
}
