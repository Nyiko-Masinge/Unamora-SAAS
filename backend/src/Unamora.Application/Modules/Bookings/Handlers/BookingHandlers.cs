using MediatR;
using Unamora.Application.Common.Exceptions;
using Unamora.Application.Common.Interfaces;
using Unamora.Application.Modules.Bookings.Commands;
using Unamora.Domain.Entities.Bookings;
using Unamora.Domain.Enums;

namespace Unamora.Application.Modules.Bookings.Handlers;

public class CreateJobRequestCommandHandler(
    IClientProfileRepository clientRepo,
    IJobRequestRepository requestRepo,
    ICurrentUserService currentUser,
    IUnitOfWork uow) : IRequestHandler<CreateJobRequestCommand, Guid>
{
    public async Task<Guid> Handle(CreateJobRequestCommand request, CancellationToken cancellationToken)
    {
        var userId = currentUser.UserId ?? throw new UnauthorizedException("Not authenticated");
        var client = await clientRepo.GetByUserIdAsync(userId, cancellationToken) ?? throw new NotFoundException("Client profile not found");

        var jobRequest = new JobRequest
        {
            ClientProfileId = client.Id,
            TradeId = request.TradeId,
            Description = request.Description,
            AddressLine1 = request.AddressLine1,
            AddressLine2 = request.AddressLine2,
            City = request.City,
            Province = request.Province,
            PostalCode = request.PostalCode,
            Latitude = request.Latitude,
            Longitude = request.Longitude,
            PreferredDate = request.PreferredDate,
            BudgetMax = request.BudgetMax,
            Status = JobRequestStatus.Pending
        };

        client.TotalJobsPosted += 1;
        clientRepo.Update(client);

        await requestRepo.AddAsync(jobRequest, cancellationToken);
        await uow.SaveChangesAsync(cancellationToken);
        return jobRequest.Id;
    }
}

public class GetJobRequestQueryHandler(
    IJobRequestRepository requestRepo,
    IQuoteRepository quoteRepo,
    IUserRepository userRepo,
    IClientProfileRepository clientRepo) : IRequestHandler<GetJobRequestQuery, JobRequestDto?>
{
    public async Task<JobRequestDto?> Handle(GetJobRequestQuery request, CancellationToken cancellationToken)
    {
        var jr = await requestRepo.GetByIdAsync(request.JobRequestId, cancellationToken);
        if (jr is null) return null;

        var client = await clientRepo.GetByIdAsync(jr.ClientProfileId, cancellationToken);
        var clientUser = client != null ? await userRepo.GetByIdAsync(client.UserId, cancellationToken) : null;

        var quotes = await quoteRepo.GetByJobRequestIdAsync(jr.Id, cancellationToken);
        var quoteDtos = new List<QuoteDto>();

        foreach (var q in quotes)
        {
            var tpUser = await userRepo.GetByIdAsync(q.TradespersonProfile.UserId, cancellationToken);
            quoteDtos.Add(new QuoteDto
            {
                Id = q.Id,
                JobRequestId = q.JobRequestId,
                TradespersonProfileId = q.TradespersonProfileId,
                BusinessName = q.TradespersonProfile.BusinessName ?? "Independent Contractor",
                TradespersonName = tpUser != null ? $"{tpUser.FirstName} {tpUser.LastName}" : "Unknown",
                EstimatedHours = q.EstimatedHours,
                HourlyRate = q.HourlyRate,
                MaterialsCost = q.MaterialsCost,
                AdditionalNotes = q.AdditionalNotes,
                Status = q.Status.ToString(),
                SentAt = q.SentAt
            });
        }

        return new JobRequestDto
        {
            Id = jr.Id,
            ClientProfileId = jr.ClientProfileId,
            ClientName = clientUser != null ? $"{clientUser.FirstName} {clientUser.LastName}" : "Unknown",
            TradeName = jr.Trade?.Name ?? "Unknown Trade",
            Description = jr.Description,
            Address = $"{jr.AddressLine1}, {jr.City}, {jr.Province}",
            Latitude = jr.Latitude,
            Longitude = jr.Longitude,
            PreferredDate = jr.PreferredDate,
            BudgetMax = jr.BudgetMax,
            Status = jr.Status.ToString(),
            Quotes = quoteDtos
        };
    }
}

public class SubmitQuoteCommandHandler(
    IQuoteRepository quoteRepo,
    ITradespersonProfileRepository tpRepo,
    IUnitOfWork uow) : IRequestHandler<SubmitQuoteCommand, Guid>
{
    public async Task<Guid> Handle(SubmitQuoteCommand request, CancellationToken cancellationToken)
    {
        var quote = new Quote
        {
            JobRequestId = request.JobRequestId,
            TradespersonProfileId = request.TradespersonProfileId,
            EstimatedHours = request.EstimatedHours,
            HourlyRate = request.HourlyRate,
            MaterialsCost = request.MaterialsCost,
            AdditionalNotes = request.AdditionalNotes,
            Status = QuoteStatus.Sent,
            SentAt = DateTime.UtcNow
        };

        await quoteRepo.AddAsync(quote, cancellationToken);
        await uow.SaveChangesAsync(cancellationToken);
        return quote.Id;
    }
}

public class AcceptQuoteCommandHandler(
    IQuoteRepository quoteRepo,
    IBookingRepository bookingRepo,
    IJobRequestRepository requestRepo,
    IJobTrackingStateRepository trackingRepo,
    IJobEventLogRepository logRepo,
    IUnitOfWork uow) : IRequestHandler<AcceptQuoteCommand, Guid>
{
    public async Task<Guid> Handle(AcceptQuoteCommand request, CancellationToken cancellationToken)
    {
        var quote = await quoteRepo.GetByIdAsync(request.QuoteId, cancellationToken) ?? throw new NotFoundException("Quote not found");
        if (quote.Status != QuoteStatus.Sent) throw new BadRequestException("Quote is not open");

        var jobRequest = await requestRepo.GetByIdAsync(quote.JobRequestId, cancellationToken) ?? throw new NotFoundException("Job request not found");

        quote.Status = QuoteStatus.Accepted;
        quoteRepo.Update(quote);

        // Decline other quotes
        var otherQuotes = await quoteRepo.GetByJobRequestIdAsync(quote.JobRequestId, cancellationToken);
        foreach (var oq in otherQuotes.Where(q => q.Id != quote.Id))
        {
            oq.Status = QuoteStatus.Declined;
            quoteRepo.Update(oq);
        }

        jobRequest.Status = JobRequestStatus.Assigned;
        requestRepo.Update(jobRequest);

        // Create Booking
        var booking = new Booking
        {
            JobRequestId = jobRequest.Id,
            QuoteId = quote.Id,
            ClientProfileId = jobRequest.ClientProfileId,
            TradespersonProfileId = quote.TradespersonProfileId,
            ScheduledDate = jobRequest.PreferredDate ?? DateTime.UtcNow.AddDays(1),
            StartTime = new TimeOnly(9, 0), // Default 9 AM
            EndTime = new TimeOnly(12, 0),
            Status = BookingStatus.Accepted,
            AgreedHourlyRate = quote.HourlyRate,
            EstimatedCost = (quote.EstimatedHours * quote.HourlyRate) + quote.MaterialsCost
        };
        await bookingRepo.AddAsync(booking, cancellationToken);
        await uow.SaveChangesAsync(cancellationToken);

        // Tracking State initialization
        var tracking = new JobTrackingState
        {
            BookingId = booking.Id,
            Status = TrackingStatus.EnRoute,
            CurrentLatitude = quote.TradespersonProfile.BaseLatitude,
            CurrentLongitude = quote.TradespersonProfile.BaseLongitude,
            LastUpdatedAt = DateTime.UtcNow
        };
        await trackingRepo.AddAsync(tracking, cancellationToken);

        // Log timeline events
        await logRepo.AddAsync(new JobEventLog
        {
            BookingId = booking.Id,
            EventName = "BookingCreated",
            Description = "Job booking scheduled and accepted."
        }, cancellationToken);

        await uow.SaveChangesAsync(cancellationToken);
        return booking.Id;
    }
}

public class DeclineQuoteCommandHandler(
    IQuoteRepository quoteRepo,
    IUnitOfWork uow) : IRequestHandler<DeclineQuoteCommand, bool>
{
    public async Task<bool> Handle(DeclineQuoteCommand request, CancellationToken cancellationToken)
    {
        var quote = await quoteRepo.GetByIdAsync(request.QuoteId, cancellationToken) ?? throw new NotFoundException("Quote not found");
        quote.Status = QuoteStatus.Declined;
        quoteRepo.Update(quote);
        await uow.SaveChangesAsync(cancellationToken);
        return true;
    }
}

public class GetBookingQueryHandler(
    IBookingRepository bookingRepo,
    IJobEventLogRepository logRepo,
    IUserRepository userRepo) : IRequestHandler<GetBookingQuery, BookingDto?>
{
    public async Task<BookingDto?> Handle(GetBookingQuery request, CancellationToken cancellationToken)
    {
        var booking = await bookingRepo.GetByIdAsync(request.BookingId, cancellationToken);
        if (booking is null) return null;

        var clientUser = await userRepo.GetByIdAsync(booking.ClientProfile.UserId, cancellationToken);
        var tpUser = await userRepo.GetByIdAsync(booking.TradespersonProfile.UserId, cancellationToken);
        var logs = await logRepo.GetByBookingIdAsync(booking.Id, cancellationToken);

        return new BookingDto
        {
            Id = booking.Id,
            JobRequestId = booking.JobRequestId,
            ClientName = clientUser != null ? $"{clientUser.FirstName} {clientUser.LastName}" : "Unknown",
            TradespersonBusinessName = booking.TradespersonProfile.BusinessName ?? "Independent Contractor",
            TradespersonName = tpUser != null ? $"{tpUser.FirstName} {tpUser.LastName}" : "Unknown",
            TradeName = booking.JobRequest?.Trade?.Name ?? "Unknown Trade",
            JobDescription = booking.JobRequest?.Description ?? string.Empty,
            Address = $"{booking.JobRequest?.AddressLine1}, {booking.JobRequest?.City}",
            Latitude = booking.JobRequest?.Latitude ?? 0,
            Longitude = booking.JobRequest?.Longitude ?? 0,
            ScheduledDate = booking.ScheduledDate,
            TimeRange = $"{booking.StartTime} - {booking.EndTime}",
            Status = booking.Status.ToString(),
            AgreedHourlyRate = booking.AgreedHourlyRate,
            EstimatedCost = booking.EstimatedCost,
            TotalPaid = booking.TotalPaid,
            Timeline = logs.Select(l => new JobEventLogDto
            {
                EventName = l.EventName,
                Description = l.Description,
                Timestamp = l.Timestamp
            }).ToList()
        };
    }
}

public class GetClientBookingsQueryHandler(
    IClientProfileRepository clientRepo,
    IBookingRepository bookingRepo,
    ICurrentUserService currentUser,
    IUserRepository userRepo) : IRequestHandler<GetClientBookingsQuery, IReadOnlyList<BookingDto>>
{
    public async Task<IReadOnlyList<BookingDto>> Handle(GetClientBookingsQuery request, CancellationToken cancellationToken)
    {
        var userId = currentUser.UserId ?? throw new UnauthorizedException("Not authenticated");
        var client = await clientRepo.GetByUserIdAsync(userId, cancellationToken) ?? throw new NotFoundException("Client profile not found");
        var bookings = await bookingRepo.GetByClientProfileIdAsync(client.Id, cancellationToken);

        var list = new List<BookingDto>();
        foreach (var b in bookings)
        {
            var tpUser = await userRepo.GetByIdAsync(b.TradespersonProfile.UserId, cancellationToken);
            list.Add(new BookingDto
            {
                Id = b.Id,
                JobRequestId = b.JobRequestId,
                ClientName = currentUser.Email ?? "Nyiko Masinge",
                TradespersonBusinessName = b.TradespersonProfile.BusinessName ?? "Independent Contractor",
                TradespersonName = tpUser != null ? $"{tpUser.FirstName} {tpUser.LastName}" : "Unknown",
                TradeName = b.JobRequest?.Trade?.Name ?? "Unknown Trade",
                JobDescription = b.JobRequest?.Description ?? string.Empty,
                Address = $"{b.JobRequest?.AddressLine1}, {b.JobRequest?.City}",
                Latitude = b.JobRequest?.Latitude ?? 0,
                Longitude = b.JobRequest?.Longitude ?? 0,
                ScheduledDate = b.ScheduledDate,
                TimeRange = $"{b.StartTime} - {b.EndTime}",
                Status = b.Status.ToString(),
                AgreedHourlyRate = b.AgreedHourlyRate,
                EstimatedCost = b.EstimatedCost,
                TotalPaid = b.TotalPaid
            });
        }
        return list;
    }
}

public class CompleteBookingCommandHandler(
    IBookingRepository bookingRepo,
    IJobEventLogRepository logRepo,
    IUnitOfWork uow) : IRequestHandler<CompleteBookingCommand, bool>
{
    public async Task<bool> Handle(CompleteBookingCommand request, CancellationToken cancellationToken)
    {
        var booking = await bookingRepo.GetByIdAsync(request.BookingId, cancellationToken) ?? throw new NotFoundException("Booking not found");
        booking.Status = BookingStatus.Completed;
        booking.TotalPaid = booking.EstimatedCost; // Mock auto-payment for demo
        booking.ModifiedAt = DateTime.UtcNow;

        bookingRepo.Update(booking);

        await logRepo.AddAsync(new JobEventLog
        {
            BookingId = booking.Id,
            EventName = "JobCompleted",
            Description = "The tradesperson marked the job as fully completed. Payment settled."
        }, cancellationToken);

        // Update tradesperson stats
        booking.TradespersonProfile.CompletedJobsCount += 1;
        
        await uow.SaveChangesAsync(cancellationToken);
        return true;
    }
}

public class CancelBookingCommandHandler(
    IBookingRepository bookingRepo,
    IJobEventLogRepository logRepo,
    IUnitOfWork uow) : IRequestHandler<CancelBookingCommand, bool>
{
    public async Task<bool> Handle(CancelBookingCommand request, CancellationToken cancellationToken)
    {
        var booking = await bookingRepo.GetByIdAsync(request.BookingId, cancellationToken) ?? throw new NotFoundException("Booking not found");
        booking.Status = BookingStatus.Cancelled;
        booking.CancellationReason = request.Reason;
        booking.ModifiedAt = DateTime.UtcNow;

        bookingRepo.Update(booking);

        await logRepo.AddAsync(new JobEventLog
        {
            BookingId = booking.Id,
            EventName = "JobCancelled",
            Description = $"Booking was cancelled. Reason: {request.Reason}"
        }, cancellationToken);

        await uow.SaveChangesAsync(cancellationToken);
        return true;
    }
}

// Tracking
public class GetTrackingStateQueryHandler(
    IJobTrackingStateRepository trackingRepo) : IRequestHandler<GetTrackingStateQuery, TrackingStateDto?>
{
    public async Task<TrackingStateDto?> Handle(GetTrackingStateQuery request, CancellationToken cancellationToken)
    {
        var tracking = await trackingRepo.GetByBookingIdAsync(request.BookingId, cancellationToken);
        if (tracking is null) return null;

        return new TrackingStateDto
        {
            BookingId = tracking.BookingId,
            Status = tracking.Status.ToString(),
            CurrentLatitude = tracking.CurrentLatitude,
            CurrentLongitude = tracking.CurrentLongitude,
            LastUpdatedAt = tracking.LastUpdatedAt
        };
    }
}

public class UpdateTrackingStateCommandHandler(
    IJobTrackingStateRepository trackingRepo,
    IJobEventLogRepository logRepo,
    IUnitOfWork uow) : IRequestHandler<UpdateTrackingStateCommand, bool>
{
    public async Task<bool> Handle(UpdateTrackingStateCommand request, CancellationToken cancellationToken)
    {
        var tracking = await trackingRepo.GetByBookingIdAsync(request.BookingId, cancellationToken) ?? throw new NotFoundException("Tracking state not found");

        var oldStatus = tracking.Status;
        tracking.Status = request.Status;
        if (request.Latitude.HasValue) tracking.CurrentLatitude = request.Latitude.Value;
        if (request.Longitude.HasValue) tracking.CurrentLongitude = request.Longitude.Value;
        tracking.LastUpdatedAt = DateTime.UtcNow;

        trackingRepo.Update(tracking);

        if (oldStatus != request.Status)
        {
            var desc = request.Status switch
            {
                TrackingStatus.EnRoute => "Tradesperson is en route to your location.",
                TrackingStatus.Arrived => "Tradesperson has arrived at the location.",
                TrackingStatus.WorkStarted => "Work has commenced.",
                TrackingStatus.WorkCompleted => "Work is finished.",
                _ => "Status updated."
            };

            await logRepo.AddAsync(new JobEventLog
            {
                BookingId = tracking.BookingId,
                EventName = request.Status.ToString(),
                Description = desc
            }, cancellationToken);
        }

        await uow.SaveChangesAsync(cancellationToken);
        return true;
    }
}

public class GetJobEventLogsQueryHandler(
    IJobEventLogRepository logRepo) : IRequestHandler<GetJobEventLogsQuery, IReadOnlyList<JobEventLogDto>>
{
    public async Task<IReadOnlyList<JobEventLogDto>> Handle(GetJobEventLogsQuery request, CancellationToken cancellationToken)
    {
        var logs = await logRepo.GetByBookingIdAsync(request.BookingId, cancellationToken);
        return logs.Select(l => new JobEventLogDto
        {
            EventName = l.EventName,
            Description = l.Description,
            Timestamp = l.Timestamp
        }).ToList();
    }
}

// Review
public class SubmitReviewCommandHandler(
    IReviewRepository reviewRepo,
    IBookingRepository bookingRepo,
    ITradespersonProfileRepository tpRepo,
    IUnitOfWork uow) : IRequestHandler<SubmitReviewCommand, Guid>
{
    public async Task<Guid> Handle(SubmitReviewCommand request, CancellationToken cancellationToken)
    {
        var booking = await bookingRepo.GetByIdAsync(request.BookingId, cancellationToken) ?? throw new NotFoundException("Booking not found");

        var review = new Review
        {
            BookingId = request.BookingId,
            Rating = request.Rating,
            Comment = request.Comment,
            Direction = request.Direction
        };

        await reviewRepo.AddAsync(review, cancellationToken);
        await uow.SaveChangesAsync(cancellationToken);

        // Recalculate Tradesperson average rating if review is ClientToTradesperson
        if (request.Direction == ReviewDirection.ClientToTradesperson)
        {
            var tp = booking.TradespersonProfile;
            var allReviews = await reviewRepo.GetByTradespersonProfileIdAsync(tp.Id, cancellationToken);

            var totalRating = allReviews.Sum(r => r.Rating) + request.Rating;
            var count = allReviews.Count + 1;

            tp.AverageRating = (decimal)totalRating / count;
            tp.ReviewCount = count;
            
            tpRepo.Update(tp);
            await uow.SaveChangesAsync(cancellationToken);
        }

        return review.Id;
    }
}
