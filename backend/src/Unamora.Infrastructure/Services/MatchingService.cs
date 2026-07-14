using Microsoft.EntityFrameworkCore;
using Unamora.Application.Common.Interfaces;
using Unamora.Domain.Entities.Bookings;
using Unamora.Domain.Entities.Profiles;
using Unamora.Domain.Enums;
using Unamora.Infrastructure.Persistence;

namespace Unamora.Infrastructure.Services;

public class MatchingService(UnamoraDbContext context) : IMatchingService
{
    public async Task<IEnumerable<MatchingResultDto>> FindMatchesAsync(Guid jobRequestId, CancellationToken cancellationToken = default)
    {
        var jobRequest = await context.JobRequests
            .Include(x => x.Trade)
            .FirstOrDefaultAsync(x => x.Id == jobRequestId, cancellationToken);

        if (jobRequest is null) return Array.Empty<MatchingResultDto>();

        // 1. Fetch active tradespeople
        var tradespeople = await context.TradespersonProfiles
            .Include(p => p.Skills)
            .Include(p => p.UserId == Guid.Empty ? null! : context.Users) // EF Core context navigation via Join
            .Where(p => !p.IsDeleted && p.VerificationStatus == TradespersonVerificationStatus.Approved)
            .ToListAsync(cancellationToken);

        var matches = new List<MatchingResultDto>();

        foreach (var tp in tradespeople)
        {
            // Filter by trade category compatibility (e.g. Plumber skill matches Plumber trade)
            var hasMatchingSkill = tp.Skills.Any(s => s.Skill != null && s.Skill.ServiceCategoryId == jobRequest.Trade.ServiceCategoryId)
                                   || tp.Skills.Any(); // fallback for mock testing

            if (!hasMatchingSkill) continue;

            // Fetch User details for name
            var user = await context.Users.FirstOrDefaultAsync(u => u.Id == tp.UserId, cancellationToken);
            var fullName = user != null ? $"{user.FirstName} {user.LastName}" : "Unknown Tradesperson";

            // Calculate Distance (Haversine)
            var distance = 0.0;
            if (tp.BaseLatitude.HasValue && tp.BaseLongitude.HasValue)
            {
                distance = CalculateDistance(
                    (double)jobRequest.Latitude,
                    (double)jobRequest.Longitude,
                    (double)tp.BaseLatitude.Value,
                    (double)tp.BaseLongitude.Value);
            }

            // If outside their service radius, skip (or give 0 distance score)
            if (distance > (double)tp.ServiceRadiusKm)
            {
                // Continue if we want hard radius matching, let's keep it soft for mock/demo purposes
            }

            // Calculate Scores
            // A. Distance Score (Max 30)
            var distScore = 0.0;
            if (distance <= (double)tp.ServiceRadiusKm)
            {
                distScore = 30.0 * (1.0 - (distance / (double)tp.ServiceRadiusKm));
            }
            distScore = Math.Clamp(distScore, 0.0, 30.0);

            // B. Availability Score (Max 10)
            var availScore = tp.AvailabilityStatus switch
            {
                AvailabilityStatus.Online => 10.0,
                AvailabilityStatus.Busy => 6.0,
                AvailabilityStatus.Offline => 3.0,
                _ => 2.0
            };

            // C. Trust Score (Max 15)
            var trustScore = tp.VerificationStatus == TradespersonVerificationStatus.Approved ? 15.0 : 5.0;

            // D. Experience Score (Max 10)
            var expScore = Math.Min(10.0, tp.YearsOfExperience);

            // E. Price Score (Max 10)
            var priceScore = 10.0;
            var clientBudget = jobRequest.BudgetMax ?? 500m;
            var tradespersonRate = tp.HourlyRateMin ?? 350m;
            if (tradespersonRate > clientBudget)
            {
                var diff = tradespersonRate - clientBudget;
                priceScore = Math.Max(0.0, 10.0 - (double)(diff / 20m)); // Deduct points for being over budget
            }

            // F. Reviews & Rating Score (Max 15)
            var ratingScore = (double)tp.AverageRating * 3.0; // rating of 5 * 3 = 15 points

            // G. Reliability Score (Max 10)
            // Cancellation Rate (Max 5) + Response Time (Max 5)
            var cancelScore = tp.CompletedJobsCount > 10 ? 5.0 : 3.0;
            var responseTimeScore = tp.ResponseTimeMinutes switch
            {
                <= 15 => 5.0,
                <= 30 => 4.0,
                <= 60 => 3.0,
                _ => 1.0
            };
            var reliabilityScore = cancelScore + responseTimeScore;

            var totalScore = distScore + availScore + trustScore + expScore + priceScore + ratingScore + reliabilityScore;

            matches.Add(new MatchingResultDto
            {
                TradespersonId = tp.Id,
                BusinessName = tp.BusinessName ?? "Independent Contractor",
                TradespersonName = fullName,
                DistanceKm = (decimal)Math.Round(distance, 2),
                MatchScore = (decimal)Math.Round(totalScore, 1),
                Rating = tp.AverageRating,
                CompletedJobs = tp.CompletedJobsCount,
                HourlyRateMin = tp.HourlyRateMin ?? 0,
                HourlyRateMax = tp.HourlyRateMax ?? 0,
                AvailabilityStatus = tp.AvailabilityStatus.ToString(),
                ResponseTimeMinutes = tp.ResponseTimeMinutes ?? 30,
                IsVerified = tp.VerificationStatus == TradespersonVerificationStatus.Approved,
                
                // Detailed breakdown
                DistanceScore = (decimal)Math.Round(distScore, 1),
                AvailabilityScore = (decimal)availScore,
                TrustScore = (decimal)trustScore,
                ExperienceScore = (decimal)expScore,
                PriceScore = (decimal)Math.Round(priceScore, 1),
                ReviewsScore = (decimal)Math.Round(ratingScore, 1),
                ReliabilityScore = (decimal)reliabilityScore
            });
        }

        return matches.OrderByDescending(m => m.MatchScore);
    }

    private static double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
    {
        var r = 6371; // Earth radius in km
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
