using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TourismHajj.Data;
using TourismHajj.Helpers;
using TourismHajj.Interfaces;
using TourismHajj.Models;
using TourismHajj.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services
    .AddIdentity<ApplicationUser, IdentityRole>(options =>
    {
        options.Password.RequireDigit = true;
        options.Password.RequireLowercase = true;
        options.Password.RequireUppercase = true;
        options.Password.RequireNonAlphanumeric = true;
        options.Password.RequiredLength = 8;
        options.User.RequireUniqueEmail = true;
        options.SignIn.RequireConfirmedAccount = false;
    })
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders()
    .AddDefaultUI();

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(AppRoles.AdminOnly, policy =>
        policy.RequireRole(AppRoles.Staff));
    options.AddPolicy(AppRoles.CustomerOnly, policy =>
        policy.RequireRole(AppRoles.Customer));
});

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/CustomerAccount/Login";
    options.AccessDeniedPath = "/Account/AccessDenied";
    options.SlidingExpiration = true;
});

builder.Services.Configure<CompanyProfileOptions>(
    builder.Configuration.GetSection(CompanyProfileOptions.SectionName));
builder.Services.Configure<HotelbedsOptions>(
    builder.Configuration.GetSection(HotelbedsOptions.SectionName));
builder.Services.Configure<AmadeusOptions>(
    builder.Configuration.GetSection(AmadeusOptions.SectionName));
builder.Services.Configure<PaymentGatewayOptions>(
    builder.Configuration.GetSection(PaymentGatewayOptions.SectionName));

builder.Services.AddScoped<IDashboardService, DashboardService>();
builder.Services.AddScoped<ICustomerService, CustomerService>();
builder.Services.AddScoped<ITravelerService, TravelerService>();
builder.Services.AddScoped<IPackageService, PackageService>();
builder.Services.AddScoped<IBookingService, BookingService>();
builder.Services.AddScoped<IHajjService, HajjService>();
builder.Services.AddScoped<IUmrahService, UmrahService>();
builder.Services.AddScoped<ISupplierService, SupplierService>();
builder.Services.AddScoped<IAccountsService, AccountsService>();
builder.Services.AddScoped<IReportService, ReportService>();
builder.Services.AddScoped<IMasterDataService, MasterDataService>();
builder.Services.AddScoped<IAuditService, AuditService>();
builder.Services.AddScoped<IPublicSiteService, PublicSiteService>();
builder.Services.AddScoped<ICustomerPortalService, CustomerPortalService>();
builder.Services.AddScoped<IInterfaceThemeService, InterfaceThemeService>();
builder.Services.AddScoped<IHotelSearchService, HotelSearchService>();
builder.Services.AddSingleton<TourismHajj.Services.Hotelbeds.DemoHotelProvider>();
builder.Services.AddHttpClient<TourismHajj.Services.Hotelbeds.HotelbedsProvider>((sp, client) =>
{
    var opts = sp.GetRequiredService<Microsoft.Extensions.Options.IOptions<HotelbedsOptions>>().Value;
    var baseUrl = string.IsNullOrWhiteSpace(opts.BaseUrl)
        ? "https://api.test.hotelbeds.com"
        : opts.BaseUrl.TrimEnd('/');
    client.BaseAddress = new Uri(baseUrl + "/");
    client.Timeout = TimeSpan.FromSeconds(45);
}).ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
{
    AutomaticDecompression = System.Net.DecompressionMethods.GZip | System.Net.DecompressionMethods.Deflate
});
builder.Services.AddScoped<IHotelProvider, TourismHajj.Services.Hotelbeds.CompositeHotelProvider>();
builder.Services.AddScoped<IFlightSearchService, FlightSearchService>();
builder.Services.AddSingleton<TourismHajj.Services.Amadeus.DemoFlightProvider>();
builder.Services.AddHttpClient<TourismHajj.Services.Amadeus.AmadeusProvider>((sp, client) =>
{
    var opts = sp.GetRequiredService<Microsoft.Extensions.Options.IOptions<AmadeusOptions>>().Value;
    var baseUrl = string.IsNullOrWhiteSpace(opts.BaseUrl)
        ? "https://test.api.amadeus.com"
        : opts.BaseUrl.TrimEnd('/');
    client.BaseAddress = new Uri(baseUrl + "/");
    client.Timeout = TimeSpan.FromSeconds(45);
});
builder.Services.AddScoped<IFlightProvider, TourismHajj.Services.Amadeus.CompositeFlightProvider>();
builder.Services.AddHttpContextAccessor();

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

await DbSeeder.SeedAsync(app.Services);

app.Run();
