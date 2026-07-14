using MediatR;
using Unamora.Application.Common.Exceptions;
using Unamora.Application.Common.Interfaces;
using Unamora.Application.Modules.Profiles.Commands;
using Unamora.Domain.Entities.Profiles;

namespace Unamora.Application.Modules.Profiles.Handlers;

public class GetClientProfileQueryHandler(
    IClientProfileRepository clientRepo,
    IUserRepository userRepo,
    ICurrentUserService currentUser) : IRequestHandler<GetClientProfileQuery, ClientProfileDto?>
{
    public async Task<ClientProfileDto?> Handle(GetClientProfileQuery request, CancellationToken cancellationToken)
    {
        var userId = currentUser.UserId ?? throw new UnauthorizedException("Not authenticated");
        var client = await clientRepo.GetByUserIdAsync(userId, cancellationToken);
        var user = await userRepo.GetByIdAsync(userId, cancellationToken);

        if (client is null) return null;

        return new ClientProfileDto
        {
            Id = client.Id,
            UserId = client.UserId,
            FirstName = user?.FirstName ?? string.Empty,
            LastName = user?.LastName ?? string.Empty,
            Email = user?.Email ?? string.Empty,
            PhoneNumber = user?.PhoneNumber,
            DefaultAddressLine1 = client.DefaultAddressLine1,
            DefaultAddressLine2 = client.DefaultAddressLine2,
            City = client.City,
            Province = client.Province,
            PostalCode = client.PostalCode,
            DefaultLatitude = client.DefaultLatitude,
            DefaultLongitude = client.DefaultLongitude,
            TotalJobsPosted = client.TotalJobsPosted,
            AverageClientRating = client.AverageClientRating
        };
    }
}

public class UpdateClientProfileCommandHandler(
    IClientProfileRepository clientRepo,
    ICurrentUserService currentUser,
    IUnitOfWork uow) : IRequestHandler<UpdateClientProfileCommand, bool>
{
    public async Task<bool> Handle(UpdateClientProfileCommand request, CancellationToken cancellationToken)
    {
        var userId = currentUser.UserId ?? throw new UnauthorizedException("Not authenticated");
        var client = await clientRepo.GetByUserIdAsync(userId, cancellationToken);
        if (client is null)
        {
            client = new ClientProfile { UserId = userId };
            await clientRepo.AddAsync(client, cancellationToken);
        }

        client.DefaultAddressLine1 = request.DefaultAddressLine1;
        client.DefaultAddressLine2 = request.DefaultAddressLine2;
        client.City = request.City;
        client.Province = request.Province;
        client.PostalCode = request.PostalCode;
        client.DefaultLatitude = request.DefaultLatitude;
        client.DefaultLongitude = request.DefaultLongitude;
        client.ModifiedAt = DateTime.UtcNow;

        clientRepo.Update(client);
        await uow.SaveChangesAsync(cancellationToken);
        return true;
    }
}

// Addresses
public class GetAddressesQueryHandler(
    IClientProfileRepository clientRepo,
    IAddressRepository addressRepo,
    ICurrentUserService currentUser) : IRequestHandler<GetAddressesQuery, IReadOnlyList<AddressDto>>
{
    public async Task<IReadOnlyList<AddressDto>> Handle(GetAddressesQuery request, CancellationToken cancellationToken)
    {
        var userId = currentUser.UserId ?? throw new UnauthorizedException("Not authenticated");
        var client = await clientRepo.GetByUserIdAsync(userId, cancellationToken) ?? throw new NotFoundException("Client profile not found");
        var addresses = await addressRepo.GetByClientProfileIdAsync(client.Id, cancellationToken);

        return addresses.Select(a => new AddressDto
        {
            Id = a.Id,
            Label = a.Label,
            AddressLine1 = a.AddressLine1,
            AddressLine2 = a.AddressLine2,
            City = a.City,
            Province = a.Province,
            PostalCode = a.PostalCode,
            Latitude = a.Latitude,
            Longitude = a.Longitude,
            IsDefault = a.IsDefault
        }).ToList();
    }
}

public class AddAddressCommandHandler(
    IClientProfileRepository clientRepo,
    IAddressRepository addressRepo,
    ICurrentUserService currentUser,
    IUnitOfWork uow) : IRequestHandler<AddAddressCommand, Guid>
{
    public async Task<Guid> Handle(AddAddressCommand request, CancellationToken cancellationToken)
    {
        var userId = currentUser.UserId ?? throw new UnauthorizedException("Not authenticated");
        var client = await clientRepo.GetByUserIdAsync(userId, cancellationToken) ?? throw new NotFoundException("Client profile not found");

        if (request.IsDefault)
        {
            var existing = await addressRepo.GetByClientProfileIdAsync(client.Id, cancellationToken);
            foreach (var addr in existing.Where(x => x.IsDefault))
            {
                addr.IsDefault = false;
                addressRepo.Update(addr);
            }
        }

        var newAddress = new Address
        {
            ClientProfileId = client.Id,
            Label = request.Label,
            AddressLine1 = request.AddressLine1,
            AddressLine2 = request.AddressLine2,
            City = request.City,
            Province = request.Province,
            PostalCode = request.PostalCode,
            Latitude = request.Latitude,
            Longitude = request.Longitude,
            IsDefault = request.IsDefault
        };

        await addressRepo.AddAsync(newAddress, cancellationToken);
        await uow.SaveChangesAsync(cancellationToken);
        return newAddress.Id;
    }
}

public class DeleteAddressCommandHandler(
    IAddressRepository addressRepo,
    IUnitOfWork uow) : IRequestHandler<DeleteAddressCommand, bool>
{
    public async Task<bool> Handle(DeleteAddressCommand request, CancellationToken cancellationToken)
    {
        var address = await addressRepo.GetByIdAsync(request.AddressId, cancellationToken) ?? throw new NotFoundException("Address not found");
        addressRepo.Delete(address);
        await uow.SaveChangesAsync(cancellationToken);
        return true;
    }
}

// Payment Methods
public class GetPaymentMethodsQueryHandler(
    IClientProfileRepository clientRepo,
    IPaymentMethodRepository paymentRepo,
    ICurrentUserService currentUser) : IRequestHandler<GetPaymentMethodsQuery, IReadOnlyList<PaymentMethodDto>>
{
    public async Task<IReadOnlyList<PaymentMethodDto>> Handle(GetPaymentMethodsQuery request, CancellationToken cancellationToken)
    {
        var userId = currentUser.UserId ?? throw new UnauthorizedException("Not authenticated");
        var client = await clientRepo.GetByUserIdAsync(userId, cancellationToken) ?? throw new NotFoundException("Client profile not found");
        var payments = await paymentRepo.GetByClientProfileIdAsync(client.Id, cancellationToken);

        return payments.Select(p => new PaymentMethodDto
        {
            Id = p.Id,
            CardBrand = p.CardBrand,
            LastFour = p.LastFour,
            ExpiryMonth = p.ExpiryMonth,
            ExpiryYear = p.ExpiryYear,
            IsDefault = p.IsDefault
        }).ToList();
    }
}

public class AddPaymentMethodCommandHandler(
    IClientProfileRepository clientRepo,
    IPaymentMethodRepository paymentRepo,
    ICurrentUserService currentUser,
    IUnitOfWork uow) : IRequestHandler<AddPaymentMethodCommand, Guid>
{
    public async Task<Guid> Handle(AddPaymentMethodCommand request, CancellationToken cancellationToken)
    {
        var userId = currentUser.UserId ?? throw new UnauthorizedException("Not authenticated");
        var client = await clientRepo.GetByUserIdAsync(userId, cancellationToken) ?? throw new NotFoundException("Client profile not found");

        if (request.IsDefault)
        {
            var existing = await paymentRepo.GetByClientProfileIdAsync(client.Id, cancellationToken);
            foreach (var p in existing.Where(x => x.IsDefault))
            {
                p.IsDefault = false;
                paymentRepo.Update(p);
            }
        }


        var pm = new PaymentMethod
        {
            ClientProfileId = client.Id,
            CardBrand = request.CardBrand,
            LastFour = request.LastFour,
            ExpiryMonth = request.ExpiryMonth,
            ExpiryYear = request.ExpiryYear,
            GatewayToken = request.GatewayToken,
            IsDefault = request.IsDefault
        };

        await paymentRepo.AddAsync(pm, cancellationToken);
        await uow.SaveChangesAsync(cancellationToken);
        return pm.Id;
    }
}

public class DeletePaymentMethodCommandHandler(
    IPaymentMethodRepository paymentRepo,
    IUnitOfWork uow) : IRequestHandler<DeletePaymentMethodCommand, bool>
{
    public async Task<bool> Handle(DeletePaymentMethodCommand request, CancellationToken cancellationToken)
    {
        var pm = await paymentRepo.GetByIdAsync(request.PaymentMethodId, cancellationToken) ?? throw new NotFoundException("Payment method not found");
        paymentRepo.Delete(pm);
        await uow.SaveChangesAsync(cancellationToken);
        return true;
    }
}

// Preference
public class GetClientPreferenceQueryHandler(
    IClientProfileRepository clientRepo,
    IClientPreferenceRepository preferenceRepo,
    ICurrentUserService currentUser) : IRequestHandler<GetClientPreferenceQuery, ClientPreferenceDto>
{
    public async Task<ClientPreferenceDto> Handle(GetClientPreferenceQuery request, CancellationToken cancellationToken)
    {
        var userId = currentUser.UserId ?? throw new UnauthorizedException("Not authenticated");
        var client = await clientRepo.GetByUserIdAsync(userId, cancellationToken) ?? throw new NotFoundException("Client profile not found");
        var pref = await preferenceRepo.GetByClientProfileIdAsync(client.Id, cancellationToken);

        if (pref is null) return new ClientPreferenceDto();

        return new ClientPreferenceDto
        {
            PreferredLanguage = pref.PreferredLanguage,
            MaxDistancePreferenceKm = pref.MaxDistancePreferenceKm,
            ReceiveNewsletter = pref.ReceiveNewsletter,
            SmsAlertsEnabled = pref.SmsAlertsEnabled
        };
    }
}

public class UpdateClientPreferenceCommandHandler(
    IClientProfileRepository clientRepo,
    IClientPreferenceRepository preferenceRepo,
    ICurrentUserService currentUser,
    IUnitOfWork uow) : IRequestHandler<UpdateClientPreferenceCommand, bool>
{
    public async Task<bool> Handle(UpdateClientPreferenceCommand request, CancellationToken cancellationToken)
    {
        var userId = currentUser.UserId ?? throw new UnauthorizedException("Not authenticated");
        var client = await clientRepo.GetByUserIdAsync(userId, cancellationToken) ?? throw new NotFoundException("Client profile not found");
        var pref = await preferenceRepo.GetByClientProfileIdAsync(client.Id, cancellationToken);

        if (pref is null)
        {
            pref = new ClientPreference { ClientProfileId = client.Id };
            await preferenceRepo.AddAsync(pref, cancellationToken);
        }

        pref.PreferredLanguage = request.PreferredLanguage;
        pref.MaxDistancePreferenceKm = request.MaxDistancePreferenceKm;
        pref.ReceiveNewsletter = request.ReceiveNewsletter;
        pref.SmsAlertsEnabled = request.SmsAlertsEnabled;
        pref.ModifiedAt = DateTime.UtcNow;

        preferenceRepo.Update(pref);
        await uow.SaveChangesAsync(cancellationToken);
        return true;
    }
}

// Saved Favorites & Recently Viewed
public class GetSavedTradespersonsQueryHandler(
    IClientProfileRepository clientRepo,
    ISavedTradespersonRepository savedRepo,
    ICurrentUserService currentUser,
    IUserRepository userRepo) : IRequestHandler<GetSavedTradespersonsQuery, IReadOnlyList<SavedTradespersonDto>>
{
    public async Task<IReadOnlyList<SavedTradespersonDto>> Handle(GetSavedTradespersonsQuery request, CancellationToken cancellationToken)
    {
        var userId = currentUser.UserId ?? throw new UnauthorizedException("Not authenticated");
        var client = await clientRepo.GetByUserIdAsync(userId, cancellationToken) ?? throw new NotFoundException("Client profile not found");
        var savedList = await savedRepo.GetByClientProfileIdAsync(client.Id, cancellationToken);

        var list = new List<SavedTradespersonDto>();
        foreach (var saved in savedList)
        {
            var user = await userRepo.GetByIdAsync(saved.TradespersonProfile.UserId, cancellationToken);
            list.Add(new SavedTradespersonDto
            {
                TradespersonProfileId = saved.TradespersonProfileId,
                BusinessName = saved.TradespersonProfile.BusinessName ?? "Independent Contractor",
                TradespersonName = user != null ? $"{user.FirstName} {user.LastName}" : "Unknown",
                Rating = saved.TradespersonProfile.AverageRating,
                CompletedJobs = saved.TradespersonProfile.CompletedJobsCount,
                SavedAt = saved.SavedAt
            });
        }
        return list;
    }
}

public class ToggleSaveTradespersonCommandHandler(
    IClientProfileRepository clientRepo,
    ISavedTradespersonRepository savedRepo,
    ICurrentUserService currentUser,
    IUnitOfWork uow) : IRequestHandler<ToggleSaveTradespersonCommand, bool>
{
    public async Task<bool> Handle(ToggleSaveTradespersonCommand request, CancellationToken cancellationToken)
    {
        var userId = currentUser.UserId ?? throw new UnauthorizedException("Not authenticated");
        var client = await clientRepo.GetByUserIdAsync(userId, cancellationToken) ?? throw new NotFoundException("Client profile not found");
        var existing = await savedRepo.GetAsync(client.Id, request.TradespersonProfileId, cancellationToken);

        if (existing is not null)
        {
            savedRepo.Delete(existing);
        }
        else
        {
            var saved = new SavedTradesperson
            {
                ClientProfileId = client.Id,
                TradespersonProfileId = request.TradespersonProfileId
            };
            await savedRepo.AddAsync(saved, cancellationToken);
        }

        await uow.SaveChangesAsync(cancellationToken);
        return true;
    }
}

public class GetRecentlyViewedTradespersonsQueryHandler(
    IClientProfileRepository clientRepo,
    IRecentlyViewedTradespersonRepository viewedRepo,
    ICurrentUserService currentUser) : IRequestHandler<GetRecentlyViewedTradespersonsQuery, IReadOnlyList<RecentlyViewedDto>>
{
    public async Task<IReadOnlyList<RecentlyViewedDto>> Handle(GetRecentlyViewedTradespersonsQuery request, CancellationToken cancellationToken)
    {
        var userId = currentUser.UserId ?? throw new UnauthorizedException("Not authenticated");
        var client = await clientRepo.GetByUserIdAsync(userId, cancellationToken) ?? throw new NotFoundException("Client profile not found");
        var viewed = await viewedRepo.GetRecentAsync(client.Id, 10, cancellationToken);

        return viewed.Select(x => new RecentlyViewedDto
        {
            TradespersonProfileId = x.TradespersonProfileId,
            BusinessName = x.TradespersonProfile.BusinessName ?? "Independent Contractor",
            Rating = x.TradespersonProfile.AverageRating,
            ViewedAt = x.ViewedAt
        }).ToList();
    }
}

public class AddRecentlyViewedTradespersonCommandHandler(
    IClientProfileRepository clientRepo,
    IRecentlyViewedTradespersonRepository viewedRepo,
    ICurrentUserService currentUser,
    IUnitOfWork uow) : IRequestHandler<AddRecentlyViewedTradespersonCommand, bool>
{
    public async Task<bool> Handle(AddRecentlyViewedTradespersonCommand request, CancellationToken cancellationToken)
    {
        var userId = currentUser.UserId ?? throw new UnauthorizedException("Not authenticated");
        var client = await clientRepo.GetByUserIdAsync(userId, cancellationToken) ?? throw new NotFoundException("Client profile not found");

        var recently = new RecentlyViewedTradesperson
        {
            ClientProfileId = client.Id,
            TradespersonProfileId = request.TradespersonProfileId
        };
        await viewedRepo.AddAsync(recently, cancellationToken);
        await uow.SaveChangesAsync(cancellationToken);
        return true;
    }
}

public class GetRecommendationsQueryHandler(
    IClientProfileRepository clientRepo,
    ITradespersonProfileRepository tradespersonRepo,
    ICurrentUserService currentUser,
    IUserRepository userRepo) : IRequestHandler<GetRecommendationsQuery, IReadOnlyList<RecommendedTradespersonDto>>
{
    public async Task<IReadOnlyList<RecommendedTradespersonDto>> Handle(GetRecommendationsQuery request, CancellationToken cancellationToken)
    {
        var userId = currentUser.UserId ?? throw new UnauthorizedException("Not authenticated");
        var client = await clientRepo.GetByUserIdAsync(userId, cancellationToken) ?? throw new NotFoundException("Client profile not found");
        var tradespersons = await tradespersonRepo.GetAllAsync(cancellationToken);

        // Simple Recommendation Engine: Pick top rated tradespersons
        var sorted = tradespersons
            .OrderByDescending(x => x.AverageRating)
            .Take(3)
            .ToList();

        var list = new List<RecommendedTradespersonDto>();
        foreach (var tp in sorted)
        {
            var user = await userRepo.GetByIdAsync(tp.UserId, cancellationToken);
            list.Add(new RecommendedTradespersonDto
            {
                TradespersonProfileId = tp.Id,
                BusinessName = tp.BusinessName ?? "Independent Contractor",
                TradespersonName = user != null ? $"{user.FirstName} {user.LastName}" : "Unknown",
                Rating = tp.AverageRating,
                Specialty = tp.Headline ?? "Home Services",
                Reason = "High Rating and Trust Score in your area"
            });
        }
        return list;
    }
}
