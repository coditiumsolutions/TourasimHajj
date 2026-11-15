from pathlib import Path
ROOT = Path(r"D:\Dotnet\CoreCursor\Tourism Hajj")
VIEWS = ROOT / "Views"

def write(path: Path, content: str):
    path.parent.mkdir(parents=True, exist_ok=True)
    path.write_text(content.strip() + "\n", encoding="utf-8")
    print("wrote", path.relative_to(ROOT))

write(VIEWS / "TravelPackages" / "Index.cshtml", r'''
@model IEnumerable<TourismHajj.Models.Entities.TravelPackage>
@{ ViewData["Title"] = ViewData["Title"] ?? "Travel Packages"; }
@await Html.PartialAsync("_Alerts")
<div class="page-header"><div><h2>@ViewData["Title"]</h2></div>
<div class="d-flex gap-2">
<form method="get" class="d-flex gap-2">
<input type="hidden" name="type" value="@ViewBag.Type" />
<input class="form-control form-control-sm" name="q" value="@ViewBag.Q" placeholder="Search..." />
<button class="btn btn-sm btn-outline-secondary">Search</button></form>
<a class="btn btn-sm btn-accent" asp-action="Create"><i class="fa-solid fa-plus"></i> New Package</a></div></div>
<div class="card-panel"><div class="table-responsive"><table class="table table-hover mb-0">
<thead><tr><th>Code</th><th>Name</th><th>Type</th><th>Days</th><th>Adult</th><th>Featured</th><th>Active</th><th></th></tr></thead>
<tbody>@foreach (var p in Model){<tr>
<td>@p.PackageCode</td><td>@p.PackageName</td><td><span class="badge badge-soft">@p.PackageType</span></td>
<td>@p.DurationDays / @p.DurationNights</td><td>@((p.AdultPrice??0).ToString("N2")) @p.CurrencyCode</td>
<td>@(p.IsFeatured==true?"Yes":"No")</td><td>@(p.IsActive==true?"Yes":"No")</td>
<td class="text-end"><a class="btn btn-sm btn-outline-primary" asp-action="Edit" asp-route-id="@p.PackageId"><i class="fa-solid fa-pen"></i></a>
<a class="btn btn-sm btn-outline-secondary" asp-action="Details" asp-route-id="@p.PackageId"><i class="fa-solid fa-eye"></i></a></td></tr>}
</tbody></table></div>@await Html.PartialAsync("_Pager")</div>
''')

write(VIEWS / "TravelPackages" / "Create.cshtml", r'''
@model TourismHajj.Models.Entities.TravelPackage
@{ ViewData["Title"] = "New Package"; }
<div class="page-header"><h2>New Travel Package</h2></div>
<div class="card-panel"><div class="card-panel-body">
<form asp-action="Create" method="post">@Html.AntiForgeryToken()
<div class="row g-3">
<div class="col-md-3"><label class="form-label">Code</label><input asp-for="PackageCode" class="form-control" /></div>
<div class="col-md-5"><label class="form-label">Name</label><input asp-for="PackageName" class="form-control" /></div>
<div class="col-md-4"><label class="form-label">Type</label>
<select asp-for="PackageType" class="form-select"><option>Tourism</option><option>Domestic</option><option>International</option><option>Hajj</option><option>Umrah</option></select></div>
<div class="col-md-12"><label class="form-label">Description</label><textarea asp-for="Description" class="form-control" rows="3"></textarea></div>
<div class="col-md-2"><label class="form-label">Days</label><input asp-for="DurationDays" class="form-control" /></div>
<div class="col-md-2"><label class="form-label">Nights</label><input asp-for="DurationNights" class="form-control" /></div>
<div class="col-md-2"><label class="form-label">Adult</label><input asp-for="AdultPrice" class="form-control" /></div>
<div class="col-md-2"><label class="form-label">Child</label><input asp-for="ChildPrice" class="form-control" /></div>
<div class="col-md-2"><label class="form-label">Infant</label><input asp-for="InfantPrice" class="form-control" /></div>
<div class="col-md-2"><label class="form-label">Currency</label><input asp-for="CurrencyCode" class="form-control" /></div>
<div class="col-md-2"><label class="form-label">Min Persons</label><input asp-for="MinimumPersons" class="form-control" /></div>
<div class="col-md-2"><label class="form-label">Max Persons</label><input asp-for="MaximumPersons" class="form-control" /></div>
<div class="col-md-2 form-check mt-4"><input asp-for="IsFeatured" class="form-check-input" /><label class="form-check-label">Featured</label></div>
<div class="col-md-2 form-check mt-4"><input asp-for="IsActive" class="form-check-input" /><label class="form-check-label">Active</label></div>
</div>
<div class="mt-3"><button class="btn btn-primary" type="submit">Save & Continue</button></div>
</form></div></div>
''')

write(VIEWS / "TravelPackages" / "Edit.cshtml", r'''
@model TourismHajj.ViewModels.PackageEditViewModel
@{ ViewData["Title"] = "Edit Package"; var p = Model.Package; }
@await Html.PartialAsync("_Alerts")
<div class="page-header"><h2>@p.PackageName <small class="text-muted">(@p.PackageCode)</small></h2></div>
<ul class="nav nav-tabs mb-3" role="tablist">
<li class="nav-item"><button class="nav-link active" data-bs-toggle="tab" data-bs-target="#basic" type="button">Basic</button></li>
<li class="nav-item"><button class="nav-link" data-bs-toggle="tab" data-bs-target="#itinerary" type="button">Itinerary</button></li>
<li class="nav-item"><button class="nav-link" data-bs-toggle="tab" data-bs-target="#services" type="button">Services</button></li>
<li class="nav-item"><button class="nav-link" data-bs-toggle="tab" data-bs-target="#departures" type="button">Departures</button></li>
</ul>
<div class="tab-content">
<div class="tab-pane fade show active" id="basic">
<div class="card-panel"><div class="card-panel-body">
<form asp-action="EditBasic" method="post">@Html.AntiForgeryToken()
<input type="hidden" asp-for="Package.PackageId" />
<input type="hidden" asp-for="Package.CreatedDate" />
<div class="row g-3">
<div class="col-md-3"><label class="form-label">Code</label><input asp-for="Package.PackageCode" class="form-control" /></div>
<div class="col-md-5"><label class="form-label">Name</label><input asp-for="Package.PackageName" class="form-control" /></div>
<div class="col-md-4"><label class="form-label">Type</label><input asp-for="Package.PackageType" class="form-control" /></div>
<div class="col-md-12"><label class="form-label">Description</label><textarea asp-for="Package.Description" class="form-control" rows="3"></textarea></div>
<div class="col-md-2"><label class="form-label">Days</label><input asp-for="Package.DurationDays" class="form-control" /></div>
<div class="col-md-2"><label class="form-label">Nights</label><input asp-for="Package.DurationNights" class="form-control" /></div>
<div class="col-md-2"><label class="form-label">Adult</label><input asp-for="Package.AdultPrice" class="form-control" /></div>
<div class="col-md-2"><label class="form-label">Child</label><input asp-for="Package.ChildPrice" class="form-control" /></div>
<div class="col-md-2"><label class="form-label">Infant</label><input asp-for="Package.InfantPrice" class="form-control" /></div>
<div class="col-md-2"><label class="form-label">Currency</label><input asp-for="Package.CurrencyCode" class="form-control" /></div>
<div class="col-md-2 form-check mt-4"><input asp-for="Package.IsFeatured" class="form-check-input" /><label class="form-check-label">Featured</label></div>
<div class="col-md-2 form-check mt-4"><input asp-for="Package.IsActive" class="form-check-input" /><label class="form-check-label">Active</label></div>
</div>
<button class="btn btn-primary mt-3" type="submit">Save Basic Info</button>
</form></div></div></div>

<div class="tab-pane fade" id="itinerary">
<div class="card-panel"><div class="card-panel-header"><h5>Day-wise Itinerary</h5></div>
<div class="card-panel-body">
<table class="table table-sm"><thead><tr><th>Day</th><th>Title</th><th>Description</th></tr></thead>
<tbody>@foreach (var i in Model.Itineraries){<tr><td>@i.DayNumber</td><td>@i.Title</td><td>@i.Description</td></tr>}
@if(!Model.Itineraries.Any()){<tr><td colspan="3" class="text-muted">No itinerary yet</td></tr>}</tbody></table>
<form asp-action="AddItinerary" method="post" class="row g-2">@Html.AntiForgeryToken()
<input type="hidden" name="PackageId" value="@p.PackageId" />
<div class="col-md-2"><input name="DayNumber" class="form-control" placeholder="Day #" /></div>
<div class="col-md-4"><input name="Title" class="form-control" placeholder="Title" /></div>
<div class="col-md-4"><input name="Description" class="form-control" placeholder="Description" /></div>
<div class="col-md-2"><button class="btn btn-primary w-100" type="submit">Add</button></div>
</form></div></div></div>

<div class="tab-pane fade" id="services">
<div class="card-panel"><div class="card-panel-header"><h5>Package Services</h5></div>
<div class="table-responsive"><table class="table table-sm mb-0"><thead><tr><th>Service</th><th>Included</th><th>Extra Price</th></tr></thead>
<tbody>@foreach (var s in Model.Services){<tr><td>@s.Service?.ServiceName</td><td>@(s.IsIncluded==true?"Yes":"No")</td><td>@s.AdditionalPrice</td></tr>}
@if(!Model.Services.Any()){<tr><td colspan="3" class="text-muted text-center">No services linked</td></tr>}</tbody></table></div></div></div>

<div class="tab-pane fade" id="departures">
<div class="card-panel"><div class="card-panel-header"><h5>Departures & Seats</h5></div>
<div class="card-panel-body">
<table class="table table-sm"><thead><tr><th>Departure</th><th>Return</th><th>Seats</th><th>Available</th><th>Adult</th><th>Status</th></tr></thead>
<tbody>@foreach (var d in Model.Departures){<tr><td>@d.DepartureDate</td><td>@d.ReturnDate</td><td>@d.TotalSeats</td><td>@d.AvailableSeats</td><td>@d.AdultPrice</td><td>@d.DepartureStatus</td></tr>}
@if(!Model.Departures.Any()){<tr><td colspan="6" class="text-muted">No departures</td></tr>}</tbody></table>
<form asp-action="AddDeparture" method="post" class="row g-2">@Html.AntiForgeryToken()
<input type="hidden" name="PackageId" value="@p.PackageId" />
<div class="col-md-2"><input name="DepartureDate" type="date" class="form-control" required /></div>
<div class="col-md-2"><input name="ReturnDate" type="date" class="form-control" required /></div>
<div class="col-md-2"><input name="TotalSeats" class="form-control" placeholder="Total seats" /></div>
<div class="col-md-2"><input name="AdultPrice" class="form-control" placeholder="Adult price" /></div>
<div class="col-md-2"><input name="CurrencyCode" class="form-control" value="@(p.CurrencyCode ?? "PKR")" /></div>
<div class="col-md-2"><button class="btn btn-primary w-100" type="submit">Add Departure</button></div>
</form></div></div></div>
</div>
''')

write(VIEWS / "TravelPackages" / "Details.cshtml", r'''
@model TourismHajj.ViewModels.PackageEditViewModel
@{ ViewData["Title"] = "Package Details"; var p = Model.Package; }
<div class="page-header"><h2>@p.PackageName</h2><a class="btn btn-sm btn-primary" asp-action="Edit" asp-route-id="@p.PackageId">Edit</a></div>
<div class="card-panel"><div class="card-panel-body">
<p><strong>@p.PackageCode</strong> · @p.PackageType · @p.DurationDays Days / @p.DurationNights Nights</p>
<p>@p.Description</p>
<p>Adult @((p.AdultPrice??0).ToString("N2")) · Child @((p.ChildPrice??0).ToString("N2")) · Infant @((p.InfantPrice??0).ToString("N2")) @p.CurrencyCode</p>
<h5 class="mt-3">Departures</h5>
<ul>@foreach (var d in Model.Departures){<li>@d.DepartureDate → @d.ReturnDate (Available: @d.AvailableSeats)</li>}</ul>
</div></div>
''')

write(VIEWS / "Bookings" / "Index.cshtml", r'''
@model IEnumerable<TourismHajj.Models.Entities.Booking>
@{ ViewData["Title"] = "Bookings"; }
@await Html.PartialAsync("_Alerts")
<div class="page-header"><div><h2>All Bookings</h2></div>
<div class="d-flex gap-2">
<form method="get" class="d-flex gap-2">
<input class="form-control form-control-sm" name="q" value="@ViewBag.Q" placeholder="Search..." />
<input type="hidden" name="type" value="@ViewBag.Type" /><input type="hidden" name="status" value="@ViewBag.Status" />
<button class="btn btn-sm btn-outline-secondary">Search</button></form>
<a class="btn btn-sm btn-accent" asp-action="Create"><i class="fa-solid fa-plus"></i> New Booking</a></div></div>
<div class="card-panel"><div class="table-responsive"><table class="table table-hover mb-0">
<thead><tr><th>Booking No</th><th>Date</th><th>Customer</th><th>Package</th><th>Net</th><th>Paid</th><th>Balance</th><th>Status</th><th></th></tr></thead>
<tbody>@foreach (var b in Model){<tr>
<td>@b.BookingNo</td><td>@b.BookingDate?.ToString("dd-MMM-yyyy")</td><td>@b.Customer?.CustomerName</td><td>@b.Package?.PackageName</td>
<td>@((b.NetAmount??0).ToString("N2"))</td><td>@((b.PaidAmount??0).ToString("N2"))</td><td>@((b.BalanceAmount??0).ToString("N2"))</td>
<td><span class="badge badge-soft">@b.BookingStatus</span></td>
<td><a class="btn btn-sm btn-outline-secondary" asp-action="Details" asp-route-id="@b.BookingId"><i class="fa-solid fa-eye"></i></a></td></tr>}
</tbody></table></div>@await Html.PartialAsync("_Pager")</div>
''')

write(VIEWS / "Bookings" / "Create.cshtml", r'''
@model TourismHajj.ViewModels.BookingCreateViewModel
@{ ViewData["Title"] = "New Booking"; }
@await Html.PartialAsync("_Alerts")
<div class="page-header"><h2>New Booking</h2></div>
<div class="card-panel"><div class="card-panel-body">
<form asp-action="Create" method="post">@Html.AntiForgeryToken()
<div class="row g-3">
<div class="col-md-6"><label class="form-label">Customer</label><select asp-for="CustomerId" class="form-select" asp-items="ViewBag.Customers"></select></div>
<div class="col-md-6"><label class="form-label">Package</label><select asp-for="PackageId" class="form-select" asp-items="ViewBag.Packages"></select></div>
<div class="col-md-6"><label class="form-label">Departure</label><select asp-for="DepartureId" class="form-select" asp-items="ViewBag.Departures"><option value="">-- Optional --</option></select></div>
<div class="col-md-6"><label class="form-label">Booking Type</label><input asp-for="BookingType" class="form-control" placeholder="Tourism / Hajj / Umrah" /></div>
<div class="col-md-2"><label class="form-label">Adults</label><input asp-for="TotalAdults" class="form-control" /></div>
<div class="col-md-2"><label class="form-label">Children</label><input asp-for="TotalChildren" class="form-control" /></div>
<div class="col-md-2"><label class="form-label">Infants</label><input asp-for="TotalInfants" class="form-control" /></div>
<div class="col-md-3"><label class="form-label">Discount</label><input asp-for="DiscountAmount" class="form-control" /></div>
<div class="col-md-3"><label class="form-label">Tax</label><input asp-for="TaxAmount" class="form-control" /></div>
<div class="col-md-12"><label class="form-label">Travelers</label><select asp-for="TravelerIds" class="form-select" asp-items="ViewBag.Travelers" multiple size="6"></select></div>
<div class="col-md-4"><label class="form-label">Initial Payment</label><input asp-for="InitialPayment" class="form-control" /></div>
<div class="col-md-4"><label class="form-label">Payment Method</label><select asp-for="PaymentMethod" class="form-select"><option>Cash</option><option>Bank Transfer</option><option>Card</option></select></div>
<div class="col-md-12"><label class="form-label">Remarks</label><textarea asp-for="Remarks" class="form-control" rows="2"></textarea></div>
</div>
<div class="mt-3"><button class="btn btn-primary" type="submit">Save Booking</button> <a class="btn btn-outline-secondary" asp-action="Index">Cancel</a></div>
</form></div></div>
''')

write(VIEWS / "Bookings" / "Details.cshtml", r'''
@model TourismHajj.ViewModels.BookingDetailsViewModel
@{ ViewData["Title"] = "Booking Details"; var b = Model.Booking; }
@await Html.PartialAsync("_Alerts")
<div class="page-header">
<div><h2>@b.BookingNo</h2><p class="text-muted mb-0">@b.Customer?.CustomerName · @b.Package?.PackageName</p></div>
<div class="d-flex gap-2">
<a class="btn btn-sm btn-outline-primary" asp-action="PrintConfirmation" asp-route-id="@b.BookingId" target="_blank"><i class="fa-solid fa-print"></i> Print</a>
<a class="btn btn-sm btn-accent" asp-controller="Payments" asp-action="Create" asp-route-bookingId="@b.BookingId">Add Payment</a>
@if (b.BookingStatus != "Cancelled") {
<form asp-action="Cancel" method="post">@Html.AntiForgeryToken()<input type="hidden" name="id" value="@b.BookingId" />
<button class="btn btn-sm btn-outline-danger" data-confirm="Cancel this booking?" type="submit">Cancel Booking</button></form>}
</div></div>
<div class="row g-3">
<div class="col-lg-4"><div class="card-panel"><div class="card-panel-body">
<dl class="row mb-0">
<dt class="col-5">Date</dt><dd class="col-7">@b.BookingDate?.ToString("dd-MMM-yyyy")</dd>
<dt class="col-5">Departure</dt><dd class="col-7">@b.Departure?.DepartureDate</dd>
<dt class="col-5">Pax</dt><dd class="col-7">A:@b.TotalAdults C:@b.TotalChildren I:@b.TotalInfants</dd>
<dt class="col-5">Total</dt><dd class="col-7">@((b.TotalAmount??0).ToString("N2"))</dd>
<dt class="col-5">Discount</dt><dd class="col-7">@((b.DiscountAmount??0).ToString("N2"))</dd>
<dt class="col-5">Tax</dt><dd class="col-7">@((b.TaxAmount??0).ToString("N2"))</dd>
<dt class="col-5">Net</dt><dd class="col-7 fw-bold">@((b.NetAmount??0).ToString("N2"))</dd>
<dt class="col-5">Paid</dt><dd class="col-7 text-success">@((b.PaidAmount??0).ToString("N2"))</dd>
<dt class="col-5">Balance</dt><dd class="col-7 text-danger">@((b.BalanceAmount??0).ToString("N2"))</dd>
<dt class="col-5">Status</dt><dd class="col-7">@b.BookingStatus / @b.PaymentStatus</dd>
</dl></div></div></div>
<div class="col-lg-8">
<div class="card-panel mb-3"><div class="card-panel-header"><h5>Travelers</h5></div>
<table class="table table-sm mb-0"><thead><tr><th>Name</th><th>Category</th><th>Price</th><th>Net</th></tr></thead>
<tbody>@foreach (var t in Model.Travelers){<tr><td>@t.Traveler?.FullName</td><td>@t.TravelerCategory</td><td>@t.PackagePrice</td><td>@t.NetAmount</td></tr>}</tbody></table></div>
<div class="card-panel"><div class="card-panel-header"><h5>Payments</h5></div>
<table class="table table-sm mb-0"><thead><tr><th>Date</th><th>Amount</th><th>Method</th><th>Type</th></tr></thead>
<tbody>@foreach (var p in Model.Payments){<tr><td>@p.PaymentDate?.ToString("dd-MMM-yyyy")</td><td>@p.Amount.ToString("N2")</td><td>@p.PaymentMethod</td><td>@p.PaymentType</td></tr>}
@if(!Model.Payments.Any()){<tr><td colspan="4" class="text-muted text-center">No payments</td></tr>}</tbody></table></div>
</div></div>
''')

write(VIEWS / "Bookings" / "PrintConfirmation.cshtml", r'''
@model TourismHajj.ViewModels.BookingDetailsViewModel
@{ Layout = "_Layout"; ViewData["Title"] = "Booking Confirmation"; var b = Model.Booking; }
<div class="card-panel">
<div class="card-panel-body">
<div class="d-flex justify-content-between align-items-start mb-4">
<div><h3 class="mb-1">Tourism Hajj</h3><div class="text-muted">Travel & Pilgrimage Management</div><div>admin@tourismhajj.com</div></div>
<div class="text-end"><h4>Booking Confirmation</h4><div>@b.BookingNo</div><div>@b.BookingDate?.ToString("dd-MMM-yyyy")</div></div>
</div>
<p><strong>Customer:</strong> @b.Customer?.CustomerName<br /><strong>Package:</strong> @b.Package?.PackageName<br /><strong>Departure:</strong> @b.Departure?.DepartureDate</p>
<table class="table"><thead><tr><th>Traveler</th><th>Category</th><th>Amount</th></tr></thead>
<tbody>@foreach (var t in Model.Travelers){<tr><td>@t.Traveler?.FullName</td><td>@t.TravelerCategory</td><td>@((t.NetAmount??0).ToString("N2"))</td></tr>}</tbody>
<tfoot><tr><th colspan="2">Net Amount</th><th>@((b.NetAmount??0).ToString("N2"))</th></tr>
<tr><th colspan="2">Paid</th><th>@((b.PaidAmount??0).ToString("N2"))</th></tr>
<tr><th colspan="2">Balance</th><th>@((b.BalanceAmount??0).ToString("N2"))</th></tr></tfoot>
</table>
<button class="btn btn-primary no-print" onclick="window.print()">Print</button>
</div></div>
''')

# Simple list/create for remaining modules
for folder, title, headers, cells in [
    ("PilgrimGroups", "Pilgrim Groups", "<th>Code</th><th>Name</th><th>Season</th><th>Leader</th><th>Pilgrims</th><th>Status</th>",
     "<td>@item.GroupCode</td><td>@item.GroupName</td><td>@item.HajjSeason?.SeasonName</td><td>@item.GroupLeaderName</td><td>@item.TotalPilgrims</td><td>@item.GroupStatus</td>"),
    ("VisaApplications", "Visa Applications", "<th>Traveler</th><th>Type</th><th>Country</th><th>Status</th><th>App No</th>",
     "<td>@item.BookingTraveler?.Traveler?.FullName</td><td>@item.VisaType</td><td>@item.Country?.CountryName</td><td>@item.VisaStatus</td><td>@item.ApplicationNumber</td>"),
    ("Suppliers", "Suppliers", "<th>Name</th><th>Type</th><th>Contact</th><th>Mobile</th><th>Active</th>",
     "<td>@item.SupplierName</td><td>@item.SupplierType</td><td>@item.ContactPerson</td><td>@item.MobileNo</td><td>@(item.IsActive==true?\"Yes\":\"No\")</td>"),
    ("Hotels", "Hotels", "<th>Name</th><th>City</th><th>Stars</th><th>Haram KM</th><th>Nabawi KM</th>",
     "<td>@item.HotelName</td><td>@item.City?.CityName</td><td>@item.StarRating</td><td>@item.DistanceFromHaramKM</td><td>@item.DistanceFromNabawiKM</td>"),
    ("TransportProviders", "Transport Providers", "<th>Name</th><th>Vehicle</th><th>Capacity</th><th>Contact</th>",
     "<td>@item.ProviderName</td><td>@item.VehicleType</td><td>@item.VehicleCapacity</td><td>@item.ContactNo</td>"),
    ("Payments", "Payment Receipts", "<th>Date</th><th>Booking</th><th>Customer</th><th>Amount</th><th>Method</th>",
     "<td>@item.PaymentDate?.ToString(\"dd-MMM-yyyy\")</td><td>@item.Booking?.BookingNo</td><td>@item.Booking?.Customer?.CustomerName</td><td>@item.Amount.ToString(\"N2\")</td><td>@item.PaymentMethod</td>"),
    ("Refunds", "Refunds", "<th>Date</th><th>Booking</th><th>Amount</th><th>Status</th><th>Reason</th>",
     "<td>@item.RefundDate</td><td>@item.Booking?.BookingNo</td><td>@item.RefundAmount.ToString(\"N2\")</td><td>@item.RefundStatus</td><td>@item.RefundReason</td>"),
    ("Expenses", "Expenses", "<th>Date</th><th>Category</th><th>Description</th><th>Amount</th><th>Method</th>",
     "<td>@item.ExpenseDate</td><td>@item.ExpenseCategory</td><td>@item.Description</td><td>@item.Amount.ToString(\"N2\")</td><td>@item.PaymentMethod</td>"),
    ("SupplierPayments", "Supplier Payments", "<th>Date</th><th>Supplier</th><th>Amount</th><th>Method</th><th>Ref</th>",
     "<td>@item.PaymentDate</td><td>@item.Supplier?.SupplierName</td><td>@item.Amount.ToString(\"N2\")</td><td>@item.PaymentMethod</td><td>@item.ReferenceNo</td>"),
]:
    entity = {
        "PilgrimGroups": "PilgrimGroup",
        "VisaApplications": "VisaApplication",
        "Suppliers": "Supplier",
        "Hotels": "Hotel",
        "TransportProviders": "TransportProvider",
        "Payments": "Payment",
        "Refunds": "Refund",
        "Expenses": "Expense",
        "SupplierPayments": "SupplierPayment",
    }[folder]
    idprop = {
        "PilgrimGroups": "PilgrimGroupId",
        "VisaApplications": "VisaApplicationId",
        "Suppliers": "SupplierId",
        "Hotels": "HotelId",
        "TransportProviders": "TransportProviderId",
        "Payments": "PaymentId",
        "Refunds": "RefundId",
        "Expenses": "ExpenseId",
        "SupplierPayments": "SupplierPaymentId",
    }[folder]
    write(VIEWS / folder / "Index.cshtml", f'''
@model IEnumerable<TourismHajj.Models.Entities.{entity}>
@{{ ViewData["Title"] = "{title}"; }}
@await Html.PartialAsync("_Alerts")
<div class="page-header"><div><h2>{title}</h2></div>
<div class="d-flex gap-2">
<form method="get" class="d-flex gap-2"><input class="form-control form-control-sm" name="q" value="@ViewBag.Q" placeholder="Search..." /><button class="btn btn-sm btn-outline-secondary">Search</button></form>
<a class="btn btn-sm btn-accent" asp-action="Create"><i class="fa-solid fa-plus"></i> Add</a></div></div>
<div class="card-panel"><div class="table-responsive"><table class="table table-hover mb-0">
<thead><tr>{headers}<th></th></tr></thead>
<tbody>@foreach (var item in Model){{<tr>{cells}
<td class="text-end"><a class="btn btn-sm btn-outline-secondary" asp-action="Details" asp-route-id="@item.{idprop}"><i class="fa-solid fa-eye"></i></a>
<a class="btn btn-sm btn-outline-primary" asp-action="Edit" asp-route-id="@item.{idprop}"><i class="fa-solid fa-pen"></i></a></td></tr>}}
</tbody></table></div>@await Html.PartialAsync("_Pager")</div>
''')

# Create forms for key modules
write(VIEWS / "Suppliers" / "Create.cshtml", r'''
@model TourismHajj.Models.Entities.Supplier
@{ ViewData["Title"] = "New Supplier"; }
<div class="page-header"><h2>New Supplier</h2></div>
<div class="card-panel"><div class="card-panel-body">
<form asp-action="Create" method="post">@Html.AntiForgeryToken()
<div class="row g-3">
<div class="col-md-6"><label class="form-label">Name</label><input asp-for="SupplierName" class="form-control" /></div>
<div class="col-md-6"><label class="form-label">Type</label>
<select asp-for="SupplierType" class="form-select"><option>Hotel</option><option>Airline</option><option>Transport</option><option>Visa Agent</option><option>Other</option></select></div>
<div class="col-md-4"><label class="form-label">Contact Person</label><input asp-for="ContactPerson" class="form-control" /></div>
<div class="col-md-4"><label class="form-label">Mobile</label><input asp-for="MobileNo" class="form-control" /></div>
<div class="col-md-4"><label class="form-label">Email</label><input asp-for="Email" class="form-control" /></div>
<div class="col-md-4"><label class="form-label">Country</label><select asp-for="CountryId" class="form-select" asp-items="ViewBag.Countries"><option value="">--</option></select></div>
<div class="col-md-4"><label class="form-label">City</label><select asp-for="CityId" class="form-select" asp-items="ViewBag.Cities"><option value="">--</option></select></div>
<div class="col-md-12"><label class="form-label">Address</label><textarea asp-for="Address" class="form-control"></textarea></div>
<div class="col-md-3 form-check mt-4"><input asp-for="IsActive" class="form-check-input" /><label class="form-check-label">Active</label></div>
</div>
<button class="btn btn-primary mt-3" type="submit">Save</button>
</form></div></div>
''')

write(VIEWS / "Suppliers" / "Edit.cshtml", r'''
@model TourismHajj.Models.Entities.Supplier
@{ ViewData["Title"] = "Edit Supplier"; }
<div class="page-header"><h2>Edit Supplier</h2></div>
<div class="card-panel"><div class="card-panel-body">
<form asp-action="Edit" method="post">@Html.AntiForgeryToken()<input type="hidden" asp-for="SupplierId" />
<div class="row g-3">
<div class="col-md-6"><label class="form-label">Name</label><input asp-for="SupplierName" class="form-control" /></div>
<div class="col-md-6"><label class="form-label">Type</label><input asp-for="SupplierType" class="form-control" /></div>
<div class="col-md-4"><label class="form-label">Contact Person</label><input asp-for="ContactPerson" class="form-control" /></div>
<div class="col-md-4"><label class="form-label">Mobile</label><input asp-for="MobileNo" class="form-control" /></div>
<div class="col-md-4"><label class="form-label">Email</label><input asp-for="Email" class="form-control" /></div>
<div class="col-md-4"><label class="form-label">Country</label><select asp-for="CountryId" class="form-select" asp-items="ViewBag.Countries"><option value="">--</option></select></div>
<div class="col-md-4"><label class="form-label">City</label><select asp-for="CityId" class="form-select" asp-items="ViewBag.Cities"><option value="">--</option></select></div>
<div class="col-md-12"><label class="form-label">Address</label><textarea asp-for="Address" class="form-control"></textarea></div>
<div class="col-md-3 form-check mt-4"><input asp-for="IsActive" class="form-check-input" /><label class="form-check-label">Active</label></div>
</div>
<button class="btn btn-primary mt-3" type="submit">Update</button>
</form></div></div>
''')

write(VIEWS / "Suppliers" / "Details.cshtml", r'''
@model TourismHajj.Models.Entities.Supplier
@{ ViewData["Title"] = "Supplier Details"; }
<div class="page-header"><h2>@Model.SupplierName</h2><a class="btn btn-sm btn-primary" asp-action="Edit" asp-route-id="@Model.SupplierId">Edit</a></div>
<div class="card-panel"><div class="card-panel-body"><dl class="row mb-0">
<dt class="col-sm-3">Type</dt><dd class="col-sm-9">@Model.SupplierType</dd>
<dt class="col-sm-3">Contact</dt><dd class="col-sm-9">@Model.ContactPerson · @Model.MobileNo</dd>
<dt class="col-sm-3">Email</dt><dd class="col-sm-9">@Model.Email</dd>
<dt class="col-sm-3">Address</dt><dd class="col-sm-9">@Model.Address</dd>
</dl></div></div>
''')

write(VIEWS / "Hotels" / "Create.cshtml", r'''
@model TourismHajj.Models.Entities.Hotel
@{ ViewData["Title"] = "New Hotel"; }
<div class="page-header"><h2>New Hotel</h2></div>
<div class="card-panel"><div class="card-panel-body">
<form asp-action="Create" method="post">@Html.AntiForgeryToken()
<div class="row g-3">
<div class="col-md-6"><label class="form-label">Hotel Name</label><input asp-for="HotelName" class="form-control" /></div>
<div class="col-md-3"><label class="form-label">City</label><select asp-for="CityId" class="form-select" asp-items="ViewBag.Cities"></select></div>
<div class="col-md-3"><label class="form-label">Supplier</label><select asp-for="SupplierId" class="form-select" asp-items="ViewBag.Suppliers"><option value="">--</option></select></div>
<div class="col-md-3"><label class="form-label">Star Rating</label><input asp-for="StarRating" class="form-control" /></div>
<div class="col-md-3"><label class="form-label">Haram Distance KM</label><input asp-for="DistanceFromHaramKM" class="form-control" /></div>
<div class="col-md-3"><label class="form-label">Nabawi Distance KM</label><input asp-for="DistanceFromNabawiKM" class="form-control" /></div>
<div class="col-md-3"><label class="form-label">Contact</label><input asp-for="ContactNo" class="form-control" /></div>
<div class="col-md-12"><label class="form-label">Address</label><textarea asp-for="Address" class="form-control"></textarea></div>
<div class="col-md-3 form-check mt-4"><input asp-for="IsActive" class="form-check-input" /><label class="form-check-label">Active</label></div>
</div>
<button class="btn btn-primary mt-3" type="submit">Save</button>
</form></div></div>
''')

write(VIEWS / "Hotels" / "Edit.cshtml", open(VIEWS / "Hotels" / "Create.cshtml", encoding="utf-8").read().replace("Create", "Edit").replace("New Hotel", "Edit Hotel").replace('<form asp-action="Edit"', '<form asp-action="Edit"').replace('<div class="page-header"><h2>Edit Hotel</h2></div>', '<div class="page-header"><h2>Edit Hotel</h2></div>\n<input type="hidden" asp-for="HotelId" />' if False else open(VIEWS / "Hotels" / "Create.cshtml", encoding="utf-8").read()))

# Fix Hotels Edit properly
write(VIEWS / "Hotels" / "Edit.cshtml", r'''
@model TourismHajj.Models.Entities.Hotel
@{ ViewData["Title"] = "Edit Hotel"; }
<div class="page-header"><h2>Edit Hotel</h2></div>
<div class="card-panel"><div class="card-panel-body">
<form asp-action="Edit" method="post">@Html.AntiForgeryToken()<input type="hidden" asp-for="HotelId" />
<div class="row g-3">
<div class="col-md-6"><label class="form-label">Hotel Name</label><input asp-for="HotelName" class="form-control" /></div>
<div class="col-md-3"><label class="form-label">City</label><select asp-for="CityId" class="form-select" asp-items="ViewBag.Cities"></select></div>
<div class="col-md-3"><label class="form-label">Supplier</label><select asp-for="SupplierId" class="form-select" asp-items="ViewBag.Suppliers"><option value="">--</option></select></div>
<div class="col-md-3"><label class="form-label">Star Rating</label><input asp-for="StarRating" class="form-control" /></div>
<div class="col-md-3"><label class="form-label">Haram Distance KM</label><input asp-for="DistanceFromHaramKM" class="form-control" /></div>
<div class="col-md-3"><label class="form-label">Nabawi Distance KM</label><input asp-for="DistanceFromNabawiKM" class="form-control" /></div>
<div class="col-md-3"><label class="form-label">Contact</label><input asp-for="ContactNo" class="form-control" /></div>
<div class="col-md-12"><label class="form-label">Address</label><textarea asp-for="Address" class="form-control"></textarea></div>
<div class="col-md-3 form-check mt-4"><input asp-for="IsActive" class="form-check-input" /><label class="form-check-label">Active</label></div>
</div>
<button class="btn btn-primary mt-3" type="submit">Update</button>
</form></div></div>
''')

write(VIEWS / "Hotels" / "Details.cshtml", r'''
@model TourismHajj.Models.Entities.Hotel
@{ ViewData["Title"] = "Hotel Details"; }
<div class="page-header"><h2>@Model.HotelName</h2><a class="btn btn-sm btn-primary" asp-action="Edit" asp-route-id="@Model.HotelId">Edit</a></div>
<div class="card-panel"><div class="card-panel-body"><dl class="row mb-0">
<dt class="col-sm-3">City</dt><dd class="col-sm-9">@Model.City?.CityName</dd>
<dt class="col-sm-3">Stars</dt><dd class="col-sm-9">@Model.StarRating</dd>
<dt class="col-sm-3">Haram / Nabawi</dt><dd class="col-sm-9">@Model.DistanceFromHaramKM km / @Model.DistanceFromNabawiKM km</dd>
<dt class="col-sm-3">Address</dt><dd class="col-sm-9">@Model.Address</dd>
</dl></div></div>
''')

# Payments/Refunds/Expenses/Transport create views
write(VIEWS / "Payments" / "Create.cshtml", r'''
@model TourismHajj.Models.Entities.Payment
@{ ViewData["Title"] = "New Payment"; }
@await Html.PartialAsync("_Alerts")
<div class="page-header"><h2>New Payment Receipt</h2></div>
<div class="card-panel"><div class="card-panel-body">
<form asp-action="Create" method="post">@Html.AntiForgeryToken()
<div class="row g-3">
<div class="col-md-6"><label class="form-label">Booking</label><select asp-for="BookingId" class="form-select" asp-items="ViewBag.Bookings"></select></div>
<div class="col-md-3"><label class="form-label">Amount</label><input asp-for="Amount" class="form-control" /></div>
<div class="col-md-3"><label class="form-label">Date</label><input asp-for="PaymentDate" class="form-control" /></div>
<div class="col-md-4"><label class="form-label">Method</label><select asp-for="PaymentMethod" class="form-select"><option>Cash</option><option>Bank Transfer</option><option>Card</option></select></div>
<div class="col-md-4"><label class="form-label">Type</label><input asp-for="PaymentType" class="form-control" /></div>
<div class="col-md-4"><label class="form-label">Reference</label><input asp-for="ReferenceNo" class="form-control" /></div>
<div class="col-md-12"><label class="form-label">Remarks</label><textarea asp-for="Remarks" class="form-control"></textarea></div>
</div>
<button class="btn btn-primary mt-3" type="submit">Post Payment</button>
</form></div></div>
''')

write(VIEWS / "Refunds" / "Create.cshtml", r'''
@model TourismHajj.Models.Entities.Refund
@{ ViewData["Title"] = "New Refund"; }
@await Html.PartialAsync("_Alerts")
<div class="page-header"><h2>New Refund</h2></div>
<div class="card-panel"><div class="card-panel-body">
<form asp-action="Create" method="post">@Html.AntiForgeryToken()
<div class="row g-3">
<div class="col-md-6"><label class="form-label">Booking</label><select asp-for="BookingId" class="form-select" asp-items="ViewBag.Bookings"></select></div>
<div class="col-md-3"><label class="form-label">Amount</label><input asp-for="RefundAmount" class="form-control" /></div>
<div class="col-md-3"><label class="form-label">Date</label><input asp-for="RefundDate" type="date" class="form-control" /></div>
<div class="col-md-4"><label class="form-label">Method</label><input asp-for="PaymentMethod" class="form-control" /></div>
<div class="col-md-4"><label class="form-label">Status</label><input asp-for="RefundStatus" class="form-control" /></div>
<div class="col-md-12"><label class="form-label">Reason</label><textarea asp-for="RefundReason" class="form-control"></textarea></div>
</div>
<button class="btn btn-primary mt-3" type="submit">Save Refund</button>
</form></div></div>
''')

write(VIEWS / "Expenses" / "Create.cshtml", r'''
@model TourismHajj.Models.Entities.Expense
@{ ViewData["Title"] = "New Expense"; }
@await Html.PartialAsync("_Alerts")
<div class="page-header"><h2>New Expense</h2></div>
<div class="card-panel"><div class="card-panel-body">
<form asp-action="Create" method="post">@Html.AntiForgeryToken()
<div class="row g-3">
<div class="col-md-3"><label class="form-label">Date</label><input asp-for="ExpenseDate" type="date" class="form-control" /></div>
<div class="col-md-3"><label class="form-label">Category</label><input asp-for="ExpenseCategory" class="form-control" /></div>
<div class="col-md-3"><label class="form-label">Amount</label><input asp-for="Amount" class="form-control" /></div>
<div class="col-md-3"><label class="form-label">Method</label><input asp-for="PaymentMethod" class="form-control" /></div>
<div class="col-md-6"><label class="form-label">Supplier</label><select asp-for="SupplierId" class="form-select" asp-items="ViewBag.Suppliers"><option value="">--</option></select></div>
<div class="col-md-6"><label class="form-label">Booking</label><select asp-for="BookingId" class="form-select" asp-items="ViewBag.Bookings"><option value="">--</option></select></div>
<div class="col-md-12"><label class="form-label">Description</label><textarea asp-for="Description" class="form-control"></textarea></div>
</div>
<button class="btn btn-primary mt-3" type="submit">Save</button>
</form></div></div>
''')

write(VIEWS / "SupplierPayments" / "Create.cshtml", r'''
@model TourismHajj.Models.Entities.SupplierPayment
@{ ViewData["Title"] = "New Supplier Payment"; }
@await Html.PartialAsync("_Alerts")
<div class="page-header"><h2>New Supplier Payment</h2></div>
<div class="card-panel"><div class="card-panel-body">
<form asp-action="Create" method="post">@Html.AntiForgeryToken()
<div class="row g-3">
<div class="col-md-6"><label class="form-label">Supplier</label><select asp-for="SupplierId" class="form-select" asp-items="ViewBag.Suppliers"></select></div>
<div class="col-md-6"><label class="form-label">Booking</label><select asp-for="BookingId" class="form-select" asp-items="ViewBag.Bookings"><option value="">--</option></select></div>
<div class="col-md-3"><label class="form-label">Date</label><input asp-for="PaymentDate" type="date" class="form-control" /></div>
<div class="col-md-3"><label class="form-label">Amount</label><input asp-for="Amount" class="form-control" /></div>
<div class="col-md-3"><label class="form-label">Method</label><input asp-for="PaymentMethod" class="form-control" /></div>
<div class="col-md-3"><label class="form-label">Reference</label><input asp-for="ReferenceNo" class="form-control" /></div>
<div class="col-md-12"><label class="form-label">Remarks</label><textarea asp-for="Remarks" class="form-control"></textarea></div>
</div>
<button class="btn btn-primary mt-3" type="submit">Save</button>
</form></div></div>
''')

write(VIEWS / "TransportProviders" / "Create.cshtml", r'''
@model TourismHajj.Models.Entities.TransportProvider
@{ ViewData["Title"] = "New Transport Provider"; }
<div class="page-header"><h2>New Transport Provider</h2></div>
<div class="card-panel"><div class="card-panel-body">
<form asp-action="Create" method="post">@Html.AntiForgeryToken()
<div class="row g-3">
<div class="col-md-6"><label class="form-label">Provider Name</label><input asp-for="ProviderName" class="form-control" /></div>
<div class="col-md-6"><label class="form-label">Supplier</label><select asp-for="SupplierId" class="form-select" asp-items="ViewBag.Suppliers"><option value="">--</option></select></div>
<div class="col-md-4"><label class="form-label">Vehicle Type</label><input asp-for="VehicleType" class="form-control" /></div>
<div class="col-md-4"><label class="form-label">Capacity</label><input asp-for="VehicleCapacity" class="form-control" /></div>
<div class="col-md-4"><label class="form-label">Contact</label><input asp-for="ContactNo" class="form-control" /></div>
<div class="col-md-3 form-check mt-4"><input asp-for="IsActive" class="form-check-input" /><label class="form-check-label">Active</label></div>
</div>
<button class="btn btn-primary mt-3" type="submit">Save</button>
</form></div></div>
''')

write(VIEWS / "TransportProviders" / "Edit.cshtml", r'''
@model TourismHajj.Models.Entities.TransportProvider
@{ ViewData["Title"] = "Edit Transport Provider"; }
<div class="page-header"><h2>Edit Transport Provider</h2></div>
<div class="card-panel"><div class="card-panel-body">
<form asp-action="Edit" method="post">@Html.AntiForgeryToken()<input type="hidden" asp-for="TransportProviderId" />
<div class="row g-3">
<div class="col-md-6"><label class="form-label">Provider Name</label><input asp-for="ProviderName" class="form-control" /></div>
<div class="col-md-6"><label class="form-label">Supplier</label><select asp-for="SupplierId" class="form-select" asp-items="ViewBag.Suppliers"><option value="">--</option></select></div>
<div class="col-md-4"><label class="form-label">Vehicle Type</label><input asp-for="VehicleType" class="form-control" /></div>
<div class="col-md-4"><label class="form-label">Capacity</label><input asp-for="VehicleCapacity" class="form-control" /></div>
<div class="col-md-4"><label class="form-label">Contact</label><input asp-for="ContactNo" class="form-control" /></div>
<div class="col-md-3 form-check mt-4"><input asp-for="IsActive" class="form-check-input" /><label class="form-check-label">Active</label></div>
</div>
<button class="btn btn-primary mt-3" type="submit">Update</button>
</form></div></div>
''')

write(VIEWS / "TransportProviders" / "Details.cshtml", r'''
@model TourismHajj.Models.Entities.TransportProvider
@{ ViewData["Title"] = "Transport Details"; }
<div class="page-header"><h2>@Model.ProviderName</h2></div>
<div class="card-panel"><div class="card-panel-body"><p>@Model.VehicleType · Capacity @Model.VehicleCapacity · @Model.ContactNo</p></div></div>
''')

# Pilgrim groups + visa create/details/edit minimal
write(VIEWS / "PilgrimGroups" / "Create.cshtml", r'''
@model TourismHajj.Models.Entities.PilgrimGroup
@{ ViewData["Title"] = "New Pilgrim Group"; }
<div class="page-header"><h2>New Pilgrim Group</h2></div>
<div class="card-panel"><div class="card-panel-body">
<form asp-action="Create" method="post">@Html.AntiForgeryToken()
<div class="row g-3">
<div class="col-md-4"><label class="form-label">Season</label><select asp-for="HajjSeasonId" class="form-select" asp-items="ViewBag.Seasons"></select></div>
<div class="col-md-4"><label class="form-label">Group Code</label><input asp-for="GroupCode" class="form-control" /></div>
<div class="col-md-4"><label class="form-label">Group Name</label><input asp-for="GroupName" class="form-control" /></div>
<div class="col-md-4"><label class="form-label">Leader Name</label><input asp-for="GroupLeaderName" class="form-control" /></div>
<div class="col-md-4"><label class="form-label">Leader Mobile</label><input asp-for="GroupLeaderMobile" class="form-control" /></div>
<div class="col-md-4"><label class="form-label">Departure</label><select asp-for="DepartureId" class="form-select" asp-items="ViewBag.Departures"><option value="">--</option></select></div>
<div class="col-md-4"><label class="form-label">Status</label><input asp-for="GroupStatus" class="form-control" /></div>
<div class="col-md-12"><label class="form-label">Remarks</label><textarea asp-for="Remarks" class="form-control"></textarea></div>
</div>
<button class="btn btn-primary mt-3" type="submit">Save</button>
</form></div></div>
''')

write(VIEWS / "PilgrimGroups" / "Edit.cshtml", r'''
@model TourismHajj.Models.Entities.PilgrimGroup
@{ ViewData["Title"] = "Edit Group"; }
<div class="page-header"><h2>Edit Group</h2></div>
<div class="card-panel"><div class="card-panel-body">
<form asp-action="Edit" method="post">@Html.AntiForgeryToken()<input type="hidden" asp-for="PilgrimGroupId" />
<div class="row g-3">
<div class="col-md-4"><label class="form-label">Season</label><select asp-for="HajjSeasonId" class="form-select" asp-items="ViewBag.Seasons"></select></div>
<div class="col-md-4"><label class="form-label">Group Code</label><input asp-for="GroupCode" class="form-control" /></div>
<div class="col-md-4"><label class="form-label">Group Name</label><input asp-for="GroupName" class="form-control" /></div>
<div class="col-md-4"><label class="form-label">Leader Name</label><input asp-for="GroupLeaderName" class="form-control" /></div>
<div class="col-md-4"><label class="form-label">Leader Mobile</label><input asp-for="GroupLeaderMobile" class="form-control" /></div>
<div class="col-md-4"><label class="form-label">Departure</label><select asp-for="DepartureId" class="form-select" asp-items="ViewBag.Departures"><option value="">--</option></select></div>
<div class="col-md-4"><label class="form-label">Status</label><input asp-for="GroupStatus" class="form-control" /></div>
<div class="col-md-12"><label class="form-label">Remarks</label><textarea asp-for="Remarks" class="form-control"></textarea></div>
</div>
<button class="btn btn-primary mt-3" type="submit">Update</button>
</form></div></div>
''')

write(VIEWS / "PilgrimGroups" / "Details.cshtml", r'''
@model TourismHajj.Models.Entities.PilgrimGroup
@{ ViewData["Title"] = "Group Details"; }
@await Html.PartialAsync("_Alerts")
<div class="page-header"><div><h2>@Model.GroupCode — @Model.GroupName</h2></div>
<a class="btn btn-sm btn-primary" asp-action="Edit" asp-route-id="@Model.PilgrimGroupId">Edit</a></div>
<div class="card-panel mb-3"><div class="card-panel-body">
<p>Season: @Model.HajjSeason?.SeasonName · Leader: @Model.GroupLeaderName (@Model.GroupLeaderMobile) · Status: @Model.GroupStatus</p>
</div></div>
<div class="card-panel mb-3"><div class="card-panel-header"><h5>Members</h5></div>
<table class="table table-sm mb-0"><thead><tr><th>Pilgrim</th><th>Leader</th><th>Room</th><th>Transport</th><th></th></tr></thead>
<tbody>@foreach (var m in Model.PilgrimGroupMembers){<tr>
<td>@m.BookingTraveler?.Traveler?.FullName</td><td>@(m.IsGroupLeader==true?"Yes":"No")</td><td>@m.RoomGroup</td><td>@m.TransportGroup</td>
<td><form asp-action="RemoveMember" method="post" class="d-inline">@Html.AntiForgeryToken()
<input type="hidden" name="memberId" value="@m.PilgrimGroupMemberId" /><input type="hidden" name="groupId" value="@Model.PilgrimGroupId" />
<button class="btn btn-sm btn-outline-danger" data-confirm="Remove member?" type="submit">Remove</button></form></td></tr>}
</tbody></table></div>
<div class="card-panel"><div class="card-panel-header"><h5>Add Member</h5></div><div class="card-panel-body">
<form asp-action="AddMember" method="post" class="row g-2">@Html.AntiForgeryToken()
<input type="hidden" name="PilgrimGroupId" value="@Model.PilgrimGroupId" />
<div class="col-md-5"><select name="BookingTravelerId" class="form-select" asp-items="ViewBag.BookingTravelers"></select></div>
<div class="col-md-2"><input name="RoomGroup" class="form-control" placeholder="Room group" /></div>
<div class="col-md-2"><input name="TransportGroup" class="form-control" placeholder="Transport" /></div>
<div class="col-md-2 form-check mt-2"><input name="IsGroupLeader" type="checkbox" value="true" class="form-check-input" id="leader" /><label for="leader" class="form-check-label">Leader</label></div>
<div class="col-md-1"><button class="btn btn-primary w-100" type="submit">Add</button></div>
</form></div></div>
''')

write(VIEWS / "VisaApplications" / "Create.cshtml", r'''
@model TourismHajj.Models.Entities.VisaApplication
@{ ViewData["Title"] = "New Visa Application"; }
<div class="page-header"><h2>New Visa Application</h2></div>
<div class="card-panel"><div class="card-panel-body">
<form asp-action="Create" method="post">@Html.AntiForgeryToken()
<div class="row g-3">
<div class="col-md-6"><label class="form-label">Traveler (Booking)</label><select asp-for="BookingTravelerId" class="form-select" asp-items="ViewBag.BookingTravelers"></select></div>
<div class="col-md-3"><label class="form-label">Visa Type</label><input asp-for="VisaType" class="form-control" /></div>
<div class="col-md-3"><label class="form-label">Country</label><select asp-for="CountryId" class="form-select" asp-items="ViewBag.Countries"></select></div>
<div class="col-md-4"><label class="form-label">Status</label>
<select asp-for="VisaStatus" class="form-select">
<option>Pending</option><option>Documents Required</option><option>Submitted</option><option>Under Process</option><option>Approved</option><option>Rejected</option><option>Expired</option>
</select></div>
<div class="col-md-4"><label class="form-label">Application No</label><input asp-for="ApplicationNumber" class="form-control" /></div>
<div class="col-md-4"><label class="form-label">Visa Agent</label><select asp-for="SupplierId" class="form-select" asp-items="ViewBag.Suppliers"><option value="">--</option></select></div>
<div class="col-md-12"><label class="form-label">Remarks</label><textarea asp-for="Remarks" class="form-control"></textarea></div>
</div>
<button class="btn btn-primary mt-3" type="submit">Save</button>
</form></div></div>
''')

write(VIEWS / "VisaApplications" / "Edit.cshtml", r'''
@model TourismHajj.Models.Entities.VisaApplication
@{ ViewData["Title"] = "Edit Visa Application"; }
<div class="page-header"><h2>Edit Visa Application</h2></div>
<div class="card-panel"><div class="card-panel-body">
<form asp-action="Edit" method="post">@Html.AntiForgeryToken()<input type="hidden" asp-for="VisaApplicationId" />
<div class="row g-3">
<div class="col-md-6"><label class="form-label">Traveler</label><select asp-for="BookingTravelerId" class="form-select" asp-items="ViewBag.BookingTravelers"></select></div>
<div class="col-md-3"><label class="form-label">Visa Type</label><input asp-for="VisaType" class="form-control" /></div>
<div class="col-md-3"><label class="form-label">Country</label><select asp-for="CountryId" class="form-select" asp-items="ViewBag.Countries"></select></div>
<div class="col-md-4"><label class="form-label">Status</label><input asp-for="VisaStatus" class="form-control" /></div>
<div class="col-md-4"><label class="form-label">Application No</label><input asp-for="ApplicationNumber" class="form-control" /></div>
<div class="col-md-4"><label class="form-label">Visa No</label><input asp-for="VisaNumber" class="form-control" /></div>
<div class="col-md-4"><label class="form-label">Issue Date</label><input asp-for="VisaIssueDate" type="date" class="form-control" /></div>
<div class="col-md-4"><label class="form-label">Expiry Date</label><input asp-for="VisaExpiryDate" type="date" class="form-control" /></div>
<div class="col-md-4"><label class="form-label">Visa Agent</label><select asp-for="SupplierId" class="form-select" asp-items="ViewBag.Suppliers"><option value="">--</option></select></div>
<div class="col-md-12"><label class="form-label">Remarks</label><textarea asp-for="Remarks" class="form-control"></textarea></div>
</div>
<button class="btn btn-primary mt-3" type="submit">Update</button>
</form></div></div>
''')

write(VIEWS / "VisaApplications" / "Details.cshtml", r'''
@model TourismHajj.Models.Entities.VisaApplication
@{ ViewData["Title"] = "Visa Details"; }
@await Html.PartialAsync("_Alerts")
<div class="page-header"><h2>Visa — @Model.BookingTraveler?.Traveler?.FullName</h2>
<a class="btn btn-sm btn-primary" asp-action="Edit" asp-route-id="@Model.VisaApplicationId">Edit</a></div>
<div class="card-panel"><div class="card-panel-body">
<p>Type: @Model.VisaType · Country: @Model.Country?.CountryName · Status: <strong>@Model.VisaStatus</strong></p>
<p>App No: @Model.ApplicationNumber · Visa No: @Model.VisaNumber</p>
<form asp-action="UpdateStatus" method="post" class="row g-2 mt-3">@Html.AntiForgeryToken()
<input type="hidden" name="id" value="@Model.VisaApplicationId" />
<div class="col-md-4"><select name="status" class="form-select">
<option>Pending</option><option>Documents Required</option><option>Submitted</option><option>Under Process</option><option>Approved</option><option>Rejected</option><option>Expired</option>
</select></div>
<div class="col-md-6"><input name="remarks" class="form-control" placeholder="Remarks" /></div>
<div class="col-md-2"><button class="btn btn-primary w-100" type="submit">Update Status</button></div>
</form>
</div></div>
''')

write(VIEWS / "Reports" / "Index.cshtml", r'''
@{ ViewData["Title"] = "Reports"; }
<div class="page-header"><h2>Reports Center</h2></div>
<div class="row g-3">
@{
    var reports = new (string Title, string Action, string Icon)[] {
        ("Booking Register","BookingRegister","fa-clipboard-list"),
        ("Customer Ledger","CustomerLedger","fa-book"),
        ("Supplier Ledger","SupplierLedger","fa-book-open"),
        ("Daily Collection","DailyCollection","fa-calendar-day"),
        ("Monthly Collection","MonthlyCollection","fa-calendar"),
        ("Outstanding Payments","OutstandingPayments","fa-scale-unbalanced"),
        ("Package Sales","PackageSales","fa-chart-column"),
        ("Hajj Pilgrim Register","HajjPilgrimRegister","fa-kaaba"),
        ("Umrah Pilgrim Register","UmrahPilgrimRegister","fa-mosque"),
        ("Visa Status","VisaStatus","fa-passport"),
        ("Passport Expiry","PassportExpiry","fa-id-card"),
        ("Hotel Room Allocation","HotelRoomAllocation","fa-bed"),
        ("Supplier Payments","SupplierPayments","fa-money-bill-transfer"),
        ("Expense Register","ExpenseRegister","fa-wallet"),
        ("Profit & Loss","ProfitLoss","fa-chart-line"),
    };
}
@foreach (var r in reports) {
<div class="col-md-4 col-xl-3">
<a class="stat-card text-decoration-none d-block" asp-action="@r.Action">
<div class="d-flex align-items-center gap-3">
<div class="icon"><i class="fa-solid @r.Icon"></i></div>
<div class="value" style="font-size:1rem">@r.Title</div>
</div></a></div>}
</div>
''')

write(VIEWS / "Reports" / "Report.cshtml", r'''
@model TourismHajj.ViewModels.ReportResultViewModel
@{ ViewData["Title"] = Model.Title; }
@await Html.PartialAsync("_Alerts")
<div class="page-header no-print">
<div><h2>@Model.Title</h2></div>
<button class="btn btn-sm btn-primary" onclick="window.print()"><i class="fa-solid fa-print"></i> Print</button>
</div>
<div class="card-panel no-print mb-3"><div class="card-panel-body">
<form method="get" class="row g-2 align-items-end">
<div class="col-md-3"><label class="form-label">From</label><input type="date" name="FromDate" value="@(Model.Filter.FromDate?.ToString("yyyy-MM-dd"))" class="form-control" /></div>
<div class="col-md-3"><label class="form-label">To</label><input type="date" name="ToDate" value="@(Model.Filter.ToDate?.ToString("yyyy-MM-dd"))" class="form-control" /></div>
<div class="col-md-3"><label class="form-label">Booking Type</label><input name="BookingType" value="@Model.Filter.BookingType" class="form-control" /></div>
<div class="col-md-3"><button class="btn btn-primary w-100" type="submit">Apply Filters</button></div>
</form></div></div>
<div class="card-panel"><div class="card-panel-body">
<div class="d-flex justify-content-between mb-3">
<div><h4 class="mb-0">@Model.CompanyName</h4><div class="text-muted">@Model.CompanyAddress</div><div>@Model.CompanyContact</div></div>
<div class="text-end"><h5>@Model.Title</h5><div>@DateTime.Now.ToString("dd-MMM-yyyy HH:mm")</div></div>
</div>
<div class="table-responsive"><table class="table table-bordered table-sm">
<thead><tr>@foreach (var h in Model.Headers){<th>@h</th>}</tr></thead>
<tbody>
@foreach (var row in Model.Rows){<tr>@foreach (var cell in row){<td>@cell</td>}</tr>}
@if(!Model.Rows.Any()){<tr><td colspan="@Model.Headers.Count" class="text-center text-muted">No data</td></tr>}
</tbody></table></div>
</div></div>
''')

# Fix Details actions that may 404 for payments etc - add stub Details that redirect to Index for payment modules without Details
# Add minimal Details for payments modules by reusing Index pattern - controllers don't all have Details.
# PaymentsController has no Details - Index links should not use Details for those. Already Index for payments doesn't include Details link that 404s - wait, the generated Index includes Details and Edit links. PaymentsController has no Details/Edit - need to fix those Index views.

for folder in ["Payments", "Refunds", "Expenses", "SupplierPayments"]:
    # rewrite index without Details/Edit for payment-like modules that only have Index/Create
    content = (VIEWS / folder / "Index.cshtml").read_text(encoding="utf-8")
    # remove action buttons column content for missing actions - simplify
    pass

write(VIEWS / "Payments" / "Index.cshtml", r'''
@model IEnumerable<TourismHajj.Models.Entities.Payment>
@{ ViewData["Title"] = "Payment Receipts"; }
@await Html.PartialAsync("_Alerts")
<div class="page-header"><div><h2>Payment Receipts</h2></div>
<div class="d-flex gap-2">
<form method="get" class="d-flex gap-2"><input class="form-control form-control-sm" name="q" value="@ViewBag.Q" placeholder="Search..." /><button class="btn btn-sm btn-outline-secondary">Search</button></form>
<a class="btn btn-sm btn-accent" asp-action="Create"><i class="fa-solid fa-plus"></i> Add</a></div></div>
<div class="card-panel"><div class="table-responsive"><table class="table table-hover mb-0">
<thead><tr><th>Date</th><th>Booking</th><th>Customer</th><th>Amount</th><th>Method</th><th>Type</th></tr></thead>
<tbody>@foreach (var item in Model){<tr>
<td>@item.PaymentDate?.ToString("dd-MMM-yyyy")</td><td>@item.Booking?.BookingNo</td><td>@item.Booking?.Customer?.CustomerName</td>
<td>@item.Amount.ToString("N2")</td><td>@item.PaymentMethod</td><td>@item.PaymentType</td></tr>}
</tbody></table></div>@await Html.PartialAsync("_Pager")</div>
''')

write(VIEWS / "Refunds" / "Index.cshtml", r'''
@model IEnumerable<TourismHajj.Models.Entities.Refund>
@{ ViewData["Title"] = "Refunds"; }
@await Html.PartialAsync("_Alerts")
<div class="page-header"><div><h2>Refunds</h2></div>
<a class="btn btn-sm btn-accent" asp-action="Create"><i class="fa-solid fa-plus"></i> Add</a></div>
<div class="card-panel"><div class="table-responsive"><table class="table table-hover mb-0">
<thead><tr><th>Date</th><th>Booking</th><th>Amount</th><th>Status</th><th>Reason</th></tr></thead>
<tbody>@foreach (var item in Model){<tr>
<td>@item.RefundDate</td><td>@item.Booking?.BookingNo</td><td>@item.RefundAmount.ToString("N2")</td><td>@item.RefundStatus</td><td>@item.RefundReason</td></tr>}
</tbody></table></div>@await Html.PartialAsync("_Pager")</div>
''')

write(VIEWS / "Expenses" / "Index.cshtml", r'''
@model IEnumerable<TourismHajj.Models.Entities.Expense>
@{ ViewData["Title"] = "Expenses"; }
@await Html.PartialAsync("_Alerts")
<div class="page-header"><div><h2>Expenses</h2></div>
<a class="btn btn-sm btn-accent" asp-action="Create"><i class="fa-solid fa-plus"></i> Add</a></div>
<div class="card-panel"><div class="table-responsive"><table class="table table-hover mb-0">
<thead><tr><th>Date</th><th>Category</th><th>Description</th><th>Amount</th><th>Method</th></tr></thead>
<tbody>@foreach (var item in Model){<tr>
<td>@item.ExpenseDate</td><td>@item.ExpenseCategory</td><td>@item.Description</td><td>@item.Amount.ToString("N2")</td><td>@item.PaymentMethod</td></tr>}
</tbody></table></div>@await Html.PartialAsync("_Pager")</div>
''')

write(VIEWS / "SupplierPayments" / "Index.cshtml", r'''
@model IEnumerable<TourismHajj.Models.Entities.SupplierPayment>
@{ ViewData["Title"] = "Supplier Payments"; }
@await Html.PartialAsync("_Alerts")
<div class="page-header"><div><h2>Supplier Payments</h2></div>
<a class="btn btn-sm btn-accent" asp-action="Create"><i class="fa-solid fa-plus"></i> Add</a></div>
<div class="card-panel"><div class="table-responsive"><table class="table table-hover mb-0">
<thead><tr><th>Date</th><th>Supplier</th><th>Amount</th><th>Method</th><th>Ref</th></tr></thead>
<tbody>@foreach (var item in Model){<tr>
<td>@item.PaymentDate</td><td>@item.Supplier?.SupplierName</td><td>@item.Amount.ToString("N2")</td><td>@item.PaymentMethod</td><td>@item.ReferenceNo</td></tr>}
</tbody></table></div>@await Html.PartialAsync("_Pager")</div>
''')

# Identity viewstart
write(ROOT / "Areas" / "Identity" / "Pages" / "_ViewStart.cshtml", r'''
@{
    Layout = "/Views/Shared/_Layout.cshtml";
}
''')

print("views3 done")
