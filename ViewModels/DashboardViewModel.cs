namespace TourismHajj.ViewModels;

public class DashboardViewModel
{
    public int TotalCustomers { get; set; }
    public int TotalTravelers { get; set; }
    public int TotalBookings { get; set; }
    public int TodaysBookings { get; set; }
    public int TotalTourismPackages { get; set; }
    public int TotalHajjPilgrims { get; set; }
    public int TotalUmrahPilgrims { get; set; }
    public decimal TotalRevenue { get; set; }
    public decimal OutstandingPayments { get; set; }
    public decimal TotalExpenses { get; set; }

    public List<ChartPoint> MonthlyBookings { get; set; } = new();
    public List<ChartPoint> MonthlyRevenue { get; set; } = new();
    public List<ChartPoint> PackageWiseSales { get; set; } = new();
    public List<ChartPoint> BookingTypeSplit { get; set; } = new();
    public List<ChartPoint> PaymentCollections { get; set; } = new();

    public List<RecentBookingItem> RecentBookings { get; set; } = new();
    public List<UpcomingDepartureItem> UpcomingDepartures { get; set; } = new();
    public List<PendingPaymentItem> PendingPayments { get; set; } = new();
    public List<PendingVisaItem> PendingVisas { get; set; } = new();
    public List<PassportAlertItem> PassportAlerts { get; set; } = new();
    public List<UpcomingDepartureItem> UpcomingHajjDepartures { get; set; } = new();
    public List<RecentCustomerItem> RecentCustomers { get; set; } = new();
}

public class ChartPoint
{
    public string Label { get; set; } = string.Empty;
    public decimal Value { get; set; }
}

public class RecentBookingItem
{
    public int BookingId { get; set; }
    public string BookingNo { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;
    public string PackageName { get; set; } = string.Empty;
    public DateTime? BookingDate { get; set; }
    public string? Status { get; set; }
    public decimal? NetAmount { get; set; }
}

public class UpcomingDepartureItem
{
    public int DepartureId { get; set; }
    public string PackageName { get; set; } = string.Empty;
    public DateOnly DepartureDate { get; set; }
    public int? AvailableSeats { get; set; }
    public string? Status { get; set; }
}

public class PendingPaymentItem
{
    public int BookingId { get; set; }
    public string BookingNo { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;
    public decimal Balance { get; set; }
}

public class PendingVisaItem
{
    public int VisaApplicationId { get; set; }
    public string TravelerName { get; set; } = string.Empty;
    public string? VisaType { get; set; }
    public string? VisaStatus { get; set; }
}

public class PassportAlertItem
{
    public int TravelerId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string? PassportNo { get; set; }
    public DateOnly? ExpiryDate { get; set; }
}

public class RecentCustomerItem
{
    public int CustomerId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public string? MobileNo { get; set; }
    public DateTime? CreatedDate { get; set; }
}
