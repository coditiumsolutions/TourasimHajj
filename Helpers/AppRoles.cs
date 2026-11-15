namespace TourismHajj.Helpers;

public static class AppRoles
{
    public const string SuperAdmin = "Super Admin";
    public const string Admin = "Admin";
    public const string BookingOfficer = "Booking Officer";
    public const string HajjManager = "Hajj Manager";
    public const string UmrahManager = "Umrah Manager";
    public const string AccountsOfficer = "Accounts Officer";
    public const string SalesOfficer = "Sales Officer";
    public const string OperationsOfficer = "Operations Officer";
    public const string Customer = "Customer";

    public const string AdminOnly = "AdminOnly";
    public const string CustomerOnly = "CustomerOnly";

    public static readonly string[] All =
    [
        SuperAdmin,
        Admin,
        BookingOfficer,
        HajjManager,
        UmrahManager,
        AccountsOfficer,
        SalesOfficer,
        OperationsOfficer,
        Customer
    ];

    public static readonly string[] Staff =
    [
        SuperAdmin,
        Admin,
        BookingOfficer,
        HajjManager,
        UmrahManager,
        AccountsOfficer,
        SalesOfficer,
        OperationsOfficer
    ];
}
