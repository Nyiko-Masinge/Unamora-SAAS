using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Unamora.Domain.Entities.Catalog;
using Unamora.Domain.Entities.Identity;
using Unamora.Domain.Entities.Profiles;
using Unamora.Domain.Enums;
using Unamora.Infrastructure.Persistence;

namespace Unamora.Api;

public static class DbInitializer
{
    public static async Task SeedAsync(UnamoraDbContext context, UserManager<ApplicationUser> userManager)
    {
        await context.Database.EnsureCreatedAsync();

        // 1. Seed Service Categories
        if (!await context.ServiceCategories.AnyAsync())
        {
            var categories = new List<ServiceCategory>
            {
                new() { Name = "Home Maintenance", Slug = "home-maintenance", Description = "General home repair and upkeep", DisplayOrder = 1 },
                new() { Name = "Emergency Services", Slug = "emergency-services", Description = "Urgent repair and safety callouts", DisplayOrder = 2 },
                new() { Name = "Renovation & Construction", Slug = "renovation-construction", Description = "Remodeling and construction work", DisplayOrder = 3 }
            };
            await context.ServiceCategories.AddRangeAsync(categories);
            await context.SaveChangesAsync();
        }

        // 2. Seed Trades
        if (!await context.Trades.AnyAsync())
        {
            var maintenanceCat = await context.ServiceCategories.FirstAsync(c => c.Slug == "home-maintenance");
            var emergencyCat = await context.ServiceCategories.FirstAsync(c => c.Slug == "emergency-services");
            
            var trades = new List<Trade>
            {
                new() { Name = "Plumbing", Slug = "plumbing", Description = "Leaky pipes, installation, drain cleaning", ServiceCategoryId = maintenanceCat.Id, DisplayOrder = 1 },
                new() { Name = "Electrical", Slug = "electrical", Description = "Wiring, distribution boards, lighting", ServiceCategoryId = maintenanceCat.Id, DisplayOrder = 2 },
                new() { Name = "Carpentry", Slug = "carpentry", Description = "Doors, cabinets, decking, furniture", ServiceCategoryId = maintenanceCat.Id, DisplayOrder = 3 },
                new() { Name = "Emergency Plumber", Slug = "emergency-plumbing", Description = "24/7 burst pipe and leakage service", ServiceCategoryId = emergencyCat.Id, DisplayOrder = 1 },
                new() { Name = "Emergency Electrician", Slug = "emergency-electrical", Description = "Power outage and fire hazard troubleshooting", ServiceCategoryId = emergencyCat.Id, DisplayOrder = 2 }
            };
            await context.Trades.AddRangeAsync(trades);
            await context.SaveChangesAsync();
        }

        // 3. Seed Skills
        if (!await context.Skills.AnyAsync())
        {
            var maintenanceCat = await context.ServiceCategories.FirstAsync(c => c.Slug == "home-maintenance");
            var skills = new List<Skill>
            {
                new() { Name = "Leak Repair", Description = "Fixing water leaks", ServiceCategoryId = maintenanceCat.Id },
                new() { Name = "Drain Unblocking", Description = "Unblocking toilets and drains", ServiceCategoryId = maintenanceCat.Id },
                new() { Name = "DB Board Installation", Description = "Installing distribution boards", ServiceCategoryId = maintenanceCat.Id },
                new() { Name = "Cabinet Making", Description = "Custom cabinet design and installation", ServiceCategoryId = maintenanceCat.Id },
                new() { Name = "Fault Finding", Description = "Locating electrical faults", ServiceCategoryId = maintenanceCat.Id }
            };
            await context.Skills.AddRangeAsync(skills);
            await context.SaveChangesAsync();
        }

        // 4. Seed Mock Client User & Profile
        var clientId = Guid.Parse(Services.CurrentUserService.SeededClientIdString);
        var clientUser = await userManager.FindByIdAsync(clientId.ToString());
        if (clientUser is null)
        {
            clientUser = new ApplicationUser
            {
                Id = clientId,
                UserName = "client@unamora.com",
                Email = "client@unamora.com",
                FirstName = "Nyiko",
                LastName = "Masinge",
                PhoneNumber = "0821234567",
                EmailConfirmed = true,
                PhoneNumberConfirmed = true,
                IsActive = true
            };
            
            var passwordHasher = new PasswordHasher<ApplicationUser>();
            clientUser.PasswordHash = passwordHasher.HashPassword(clientUser, "Password123!");
            await userManager.CreateAsync(clientUser);

            var clientProfile = new ClientProfile
            {
                UserId = clientId,
                DefaultAddressLine1 = "123 Jan Smuts Avenue",
                DefaultAddressLine2 = "Rosebank",
                City = "Johannesburg",
                Province = "Gauteng",
                PostalCode = "2196",
                CountryCode = "ZA",
                DefaultLatitude = -26.145m,
                DefaultLongitude = 28.033m
            };
            await context.ClientProfiles.AddAsync(clientProfile);

            var defaultAddress = new Address
            {
                ClientProfileId = clientProfile.Id,
                Label = "Home (Default)",
                AddressLine1 = "123 Jan Smuts Avenue",
                AddressLine2 = "Rosebank",
                City = "Johannesburg",
                Province = "Gauteng",
                PostalCode = "2196",
                Latitude = -26.145m,
                Longitude = 28.033m,
                IsDefault = true
            };
            await context.Addresses.AddAsync(defaultAddress);

            var defaultPref = new ClientPreference
            {
                ClientProfileId = clientProfile.Id,
                PreferredLanguage = "en",
                MaxDistancePreferenceKm = 30
            };
            await context.ClientPreferences.AddAsync(defaultPref);
            
            await context.SaveChangesAsync();
        }

        // 5. Seed Mock Tradespersons
        var plumberId = Guid.Parse(Services.CurrentUserService.SeededTradespersonIdString);
        var plumberUser = await userManager.FindByIdAsync(plumberId.ToString());
        if (plumberUser is null)
        {
            // Seed Plumber
            plumberUser = new ApplicationUser
            {
                Id = plumberId,
                UserName = "johndoe@plumbing.co.za",
                Email = "johndoe@plumbing.co.za",
                FirstName = "John",
                LastName = "Doe",
                PhoneNumber = "0711122334",
                EmailConfirmed = true,
                PhoneNumberConfirmed = true,
                IsActive = true
            };
            var passwordHasher = new PasswordHasher<ApplicationUser>();
            plumberUser.PasswordHash = passwordHasher.HashPassword(plumberUser, "Password123!");
            await userManager.CreateAsync(plumberUser);

            var plumberProfile = new TradespersonProfile
            {
                UserId = plumberId,
                BusinessName = "John's Express Plumbing",
                Headline = "Master Plumber - 24/7 Leak & Drain Expert",
                Bio = "With over 12 years of experience in residential and commercial plumbing. Fully insured and SA certified.",
                YearsOfExperience = 12,
                VerificationStatus = TradespersonVerificationStatus.Approved,
                AvailabilityStatus = AvailabilityStatus.Online,
                AverageRating = 4.85m,
                ReviewCount = 28,
                CompletedJobsCount = 145,
                ResponseTimeMinutes = 15,
                BaseLatitude = -26.150m, // close to client
                BaseLongitude = 28.040m,
                ServiceRadiusKm = 30,
                HourlyRateMin = 350m,
                HourlyRateMax = 550m,
                HasPoliceClearance = true,
                ProfileCompleteness = 95
            };
            await context.TradespersonProfiles.AddAsync(plumberProfile);
            await context.SaveChangesAsync();

            var plumbingSkill = await context.Skills.FirstAsync(s => s.Name == "Leak Repair");
            await context.TradespersonSkills.AddAsync(new TradespersonSkill
            {
                TradespersonProfileId = plumberProfile.Id,
                SkillId = plumbingSkill.Id,
                ProficiencyLevel = ProficiencyLevel.Expert
            });

            // Seed Electrician
            var elecId = Guid.Parse("c3d6b9a8-5678-4321-9876-54321fedcba9");
            var elecUser = new ApplicationUser
            {
                Id = elecId,
                UserName = "sparky@elec.co.za",
                Email = "sparky@elec.co.za",
                FirstName = "Sparky",
                LastName = "Jones",
                PhoneNumber = "0722233445",
                EmailConfirmed = true,
                PhoneNumberConfirmed = true,
                IsActive = true
            };
            elecUser.PasswordHash = passwordHasher.HashPassword(elecUser, "Password123!");
            await userManager.CreateAsync(elecUser);

            var elecProfile = new TradespersonProfile
            {
                UserId = elecId,
                BusinessName = "Jones Electrical Solutions",
                Headline = "Certified Electrical Contractor",
                Bio = "Specializing in smart home wiring, DB boards, solar backups, and safety inspections.",
                YearsOfExperience = 8,
                VerificationStatus = TradespersonVerificationStatus.Approved,
                AvailabilityStatus = AvailabilityStatus.Online,
                AverageRating = 4.72m,
                ReviewCount = 19,
                CompletedJobsCount = 88,
                ResponseTimeMinutes = 25,
                BaseLatitude = -26.180m, // a bit further
                BaseLongitude = 28.010m,
                ServiceRadiusKm = 25,
                HourlyRateMin = 400m,
                HourlyRateMax = 650m,
                HasPoliceClearance = true,
                ProfileCompleteness = 90
            };
            await context.TradespersonProfiles.AddAsync(elecProfile);
            await context.SaveChangesAsync();

            var dbSkill = await context.Skills.FirstAsync(s => s.Name == "DB Board Installation");
            await context.TradespersonSkills.AddAsync(new TradespersonSkill
            {
                TradespersonProfileId = elecProfile.Id,
                SkillId = dbSkill.Id,
                ProficiencyLevel = ProficiencyLevel.Advanced
            });

            await context.SaveChangesAsync();
        }
    }
}
