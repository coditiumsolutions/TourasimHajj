using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TourismHajj.Helpers;
using TourismHajj.Models;

namespace TourismHajj.Data;

public static class DbSeeder
{
    public static async Task SeedAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        await db.Database.ExecuteSqlRawAsync("""
            IF OBJECT_ID(N'[dbo].[GalleryItems]', N'U') IS NULL
            BEGIN
                CREATE TABLE [dbo].[GalleryItems](
                    [GalleryItemId] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
                    [Title] NVARCHAR(200) NOT NULL,
                    [Description] NVARCHAR(1000) NULL,
                    [ImageUrl] NVARCHAR(500) NOT NULL,
                    [SortOrder] INT NOT NULL CONSTRAINT DF_GalleryItems_SortOrder DEFAULT(0),
                    [IsActive] BIT NOT NULL CONSTRAINT DF_GalleryItems_IsActive DEFAULT(1),
                    [CreatedDate] DATETIME2 NULL
                );
            END
            """);

        await db.Database.ExecuteSqlRawAsync("""
            IF OBJECT_ID(N'[dbo].[HotelPackages]', N'U') IS NULL
            BEGIN
                CREATE TABLE [dbo].[HotelPackages](
                    [HotelPackageId] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
                    [HotelName] NVARCHAR(150) NOT NULL,
                    [Description] NVARCHAR(2000) NULL,
                    [CityName] NVARCHAR(100) NULL,
                    [StarRating] INT NULL,
                    [Address] NVARCHAR(500) NULL,
                    [DistanceFromHaramKm] DECIMAL(10,2) NULL,
                    [DistanceFromNabawiKm] DECIMAL(10,2) NULL,
                    [ContactNo] NVARCHAR(30) NULL,
                    [CoverImageUrl] NVARCHAR(500) NULL,
                    [PricePerNight] DECIMAL(18,2) NULL,
                    [CurrencyCode] NVARCHAR(10) NULL CONSTRAINT DF_HotelPackages_Currency DEFAULT(N'PKR'),
                    [DurationNights] INT NULL,
                    [SortOrder] INT NOT NULL CONSTRAINT DF_HotelPackages_SortOrder DEFAULT(0),
                    [IsFeatured] BIT NOT NULL CONSTRAINT DF_HotelPackages_IsFeatured DEFAULT(0),
                    [IsActive] BIT NOT NULL CONSTRAINT DF_HotelPackages_IsActive DEFAULT(1),
                    [CreatedDate] DATETIME2 NULL
                );
            END
            """);

        await db.Database.ExecuteSqlRawAsync("""
            IF COL_LENGTH(N'dbo.Customers', N'TrackingNo') IS NULL
                ALTER TABLE [dbo].[Customers] ADD [TrackingNo] NVARCHAR(30) NULL;
            IF COL_LENGTH(N'dbo.Customers', N'GroupType') IS NULL
                ALTER TABLE [dbo].[Customers] ADD [GroupType] NVARCHAR(30) NULL;
            """);

        await db.Database.ExecuteSqlRawAsync("""
            IF OBJECT_ID(N'[dbo].[AppConfigs]', N'U') IS NULL
            BEGIN
                CREATE TABLE [dbo].[AppConfigs](
                    [AppConfigId] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
                    [ConfigKey] NVARCHAR(100) NOT NULL,
                    [ConfigValue] NVARCHAR(2000) NOT NULL,
                    [Description] NVARCHAR(500) NULL,
                    [IsActive] BIT NOT NULL CONSTRAINT DF_AppConfigs_IsActive DEFAULT(1),
                    [CreatedDate] DATETIME2 NULL,
                    [UpdatedDate] DATETIME2 NULL
                );
                CREATE UNIQUE INDEX IX_AppConfigs_ConfigKey ON [dbo].[AppConfigs]([ConfigKey]);
            END
            """);

        foreach (var role in AppRoles.All)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }

        const string adminEmail = "admin@tourismhajj.com";
        var admin = await userManager.FindByEmailAsync(adminEmail);
        if (admin == null)
        {
            admin = new ApplicationUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                EmailConfirmed = true,
                FullName = "System Administrator",
                Designation = "Super Admin",
                IsActive = true
            };

            var result = await userManager.CreateAsync(admin, "Admin@12345");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(admin, AppRoles.SuperAdmin);
                await userManager.AddToRoleAsync(admin, AppRoles.Admin);
            }
        }

        const string ownerEmail = "msghauri3@yahoo.com";
        const string ownerPassword = "$hahidGhauri786";
        var owner = await userManager.FindByEmailAsync(ownerEmail);
        if (owner == null)
        {
            owner = new ApplicationUser
            {
                UserName = ownerEmail,
                Email = ownerEmail,
                EmailConfirmed = true,
                FullName = "Shahid Ghauri",
                Designation = "Owner",
                IsActive = true
            };

            var ownerResult = await userManager.CreateAsync(owner, ownerPassword);
            if (ownerResult.Succeeded)
            {
                await userManager.AddToRoleAsync(owner, AppRoles.SuperAdmin);
                await userManager.AddToRoleAsync(owner, AppRoles.Admin);
            }
        }
        else
        {
            // Keep Fill My Login credentials working for the owner account.
            owner.EmailConfirmed = true;
            owner.IsActive = true;
            owner.UserName = ownerEmail;
            owner.NormalizedUserName = ownerEmail.ToUpperInvariant();
            owner.NormalizedEmail = ownerEmail.ToUpperInvariant();
            await userManager.UpdateAsync(owner);

            if (!await userManager.CheckPasswordAsync(owner, ownerPassword))
            {
                var token = await userManager.GeneratePasswordResetTokenAsync(owner);
                await userManager.ResetPasswordAsync(owner, token, ownerPassword);
            }

            if (!await userManager.IsInRoleAsync(owner, AppRoles.SuperAdmin))
                await userManager.AddToRoleAsync(owner, AppRoles.SuperAdmin);
            if (!await userManager.IsInRoleAsync(owner, AppRoles.Admin))
                await userManager.AddToRoleAsync(owner, AppRoles.Admin);
        }

        if (!await db.GalleryItems.AnyAsync())
        {
            var samples = new (string Title, string Description, string ImageUrl, int SortOrder)[]
            {
                ("Hunza Valley", "Emerald terraces and snow peaks of Hunza.", "/images/gallery/hunza-valley.jpg", 1),
                ("Mountain Lake Views", "Crystal waters framed by towering northern ranges.", "/images/gallery/attabad-lake.jpg", 2),
                ("Karakoram Peaks", "Rugged high-altitude peaks of Northern Pakistan.", "/images/gallery/karakoram-peaks.jpg", 3),
                ("Alpine Meadows", "Open meadows and ridgelines near Naran & Kaghan.", "/images/gallery/naran-kaghan.jpg", 4),
                ("Fairy Meadows Trail", "Pine forests and glacier views on the Fairy Meadows approach.", "/images/gallery/fairy-meadows.jpg", 5),
                ("Skardu Highlands", "Dramatic highland landscape of the Skardu region.", "/images/gallery/skardu-scape.jpg", 6),
                ("Northern Pass Sunrise", "Sunrise light over mountain passes in the north.", "/images/gallery/northern-pass.jpg", 7),
                ("Gilgit Vista", "Wide valley vistas from the Gilgit-Baltistan region.", "/images/gallery/gilgit-vista.jpg", 8)
            };

            foreach (var s in samples)
            {
                db.GalleryItems.Add(new Models.Entities.GalleryItem
                {
                    Title = s.Title,
                    Description = s.Description,
                    ImageUrl = s.ImageUrl,
                    SortOrder = s.SortOrder,
                    IsActive = true,
                    CreatedDate = DateTime.Now
                });
            }

            await db.SaveChangesAsync();
        }

        if (!await db.AppConfigs.AnyAsync())
        {
            var now = DateTime.Now;
            db.AppConfigs.AddRange(
                new Models.Entities.AppConfig
                {
                    ConfigKey = "Cities",
                    ConfigValue = "Karachi, Multan, Lahore, Rawalpindi, Islamabad, Peshawar, Quetta, Faisalabad",
                    Description = "Comma-separated city list used by setup / lookup options.",
                    IsActive = true,
                    CreatedDate = now,
                    UpdatedDate = now
                },
                new Models.Entities.AppConfig
                {
                    ConfigKey = "GroupTypes",
                    ConfigValue = "Family, Friends, Companion, Mixed",
                    Description = "Customer group type options.",
                    IsActive = true,
                    CreatedDate = now,
                    UpdatedDate = now
                },
                new Models.Entities.AppConfig
                {
                    ConfigKey = "DocumentTypes",
                    ConfigValue = "Passport, CNIC, Photo, Visa, Vaccination, Ticket, Other",
                    Description = "Traveler document type options.",
                    IsActive = true,
                    CreatedDate = now,
                    UpdatedDate = now
                },
                new Models.Entities.AppConfig
                {
                    ConfigKey = TourismHajj.Helpers.InterfaceThemes.ManagementConfigKey,
                    ConfigValue = TourismHajj.Helpers.InterfaceThemes.DefaultThemeId,
                    Description = "Selected management portal interface theme.",
                    IsActive = true,
                    CreatedDate = now,
                    UpdatedDate = now
                },
                new Models.Entities.AppConfig
                {
                    ConfigKey = TourismHajj.Helpers.InterfaceThemes.CustomerConfigKey,
                    ConfigValue = TourismHajj.Helpers.InterfaceThemes.DefaultThemeId,
                    Description = "Selected customers portal interface theme.",
                    IsActive = true,
                    CreatedDate = now,
                    UpdatedDate = now
                }
            );
            await db.SaveChangesAsync();
        }
        else
        {
            var now = DateTime.Now;
            async Task EnsureThemeConfig(string key, string description)
            {
                if (await db.AppConfigs.AnyAsync(c => c.ConfigKey == key)) return;
                db.AppConfigs.Add(new Models.Entities.AppConfig
                {
                    ConfigKey = key,
                    ConfigValue = TourismHajj.Helpers.InterfaceThemes.DefaultThemeId,
                    Description = description,
                    IsActive = true,
                    CreatedDate = now,
                    UpdatedDate = now
                });
            }

            await EnsureThemeConfig(TourismHajj.Helpers.InterfaceThemes.ManagementConfigKey, "Selected management portal interface theme.");
            await EnsureThemeConfig(TourismHajj.Helpers.InterfaceThemes.CustomerConfigKey, "Selected customers portal interface theme.");
            await db.SaveChangesAsync();
        }
    }
}

