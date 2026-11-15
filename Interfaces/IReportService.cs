using TourismHajj.ViewModels;

namespace TourismHajj.Interfaces;

public interface IReportService
{
    ReportIndexViewModel GetReportMenu();
    Task<IReadOnlyList<BookingRegisterRow>> GetBookingRegisterAsync(ReportFilterViewModel filter, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<CustomerLedgerRow>> GetCustomerLedgerReportAsync(ReportFilterViewModel filter, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<SupplierLedgerRow>> GetSupplierLedgerReportAsync(ReportFilterViewModel filter, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<CollectionRow>> GetDailyCollectionAsync(ReportFilterViewModel filter, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<MonthlyCollectionRow>> GetMonthlyCollectionAsync(ReportFilterViewModel filter, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<OutstandingPaymentRow>> GetOutstandingPaymentsAsync(ReportFilterViewModel filter, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<PackageSalesRow>> GetPackageSalesAsync(ReportFilterViewModel filter, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<HajjPilgrimRegisterRow>> GetHajjPilgrimRegisterAsync(ReportFilterViewModel filter, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<UmrahPilgrimRegisterRow>> GetUmrahPilgrimRegisterAsync(ReportFilterViewModel filter, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<VisaStatusReportRow>> GetVisaStatusAsync(ReportFilterViewModel filter, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<PassportExpiryRow>> GetPassportExpiryAsync(ReportFilterViewModel filter, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<HotelRoomAllocationRow>> GetHotelRoomAllocationAsync(ReportFilterViewModel filter, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<SupplierPaymentReportRow>> GetSupplierPaymentsReportAsync(ReportFilterViewModel filter, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ExpenseRegisterRow>> GetExpenseRegisterAsync(ReportFilterViewModel filter, CancellationToken cancellationToken = default);
    Task<ProfitLossSummaryViewModel> GetProfitLossReportAsync(ReportFilterViewModel filter, CancellationToken cancellationToken = default);
}