from pathlib import Path
ROOT = Path(r"D:\Dotnet\CoreCursor\Tourism Hajj")
VIEWS = ROOT / "Views"

def write(path: Path, content: str):
    path.parent.mkdir(parents=True, exist_ok=True)
    path.write_text(content.strip() + "\n", encoding="utf-8")
    print("wrote", path.relative_to(ROOT))

write(VIEWS / "Customers" / "Index.cshtml", r'''
@model IEnumerable<TourismHajj.Models.Entities.Customer>
@{ ViewData["Title"] = "Customers"; }
@await Html.PartialAsync("_Alerts")
<div class="page-header"><div><h2>Customers</h2></div>
<div class="d-flex gap-2">
<form method="get" class="d-flex gap-2"><input class="form-control form-control-sm" name="q" value="@ViewBag.Q" placeholder="Search name, mobile, CNIC..." /><button class="btn btn-sm btn-outline-secondary">Search</button></form>
<a class="btn btn-sm btn-accent" asp-action="Create"><i class="fa-solid fa-plus"></i> Add Customer</a></div></div>
<div class="card-panel"><div class="table-responsive"><table class="table table-hover mb-0">
<thead><tr><th>Name</th><th>Mobile</th><th>WhatsApp</th><th>CNIC</th><th>City</th><th>Type</th><th>Active</th><th></th></tr></thead>
<tbody>
@foreach (var item in Model) {
<tr>
<td>@item.CustomerName</td><td>@item.MobileNo</td><td>@item.WhatsAppNo</td><td>@item.Cnic</td>
<td>@(item.City?.CityName)</td><td>@item.CustomerType</td><td>@(item.IsActive==true?"Yes":"No")</td>
<td class="text-end text-nowrap">
<a class="btn btn-sm btn-outline-secondary" asp-action="Details" asp-route-id="@item.CustomerId"><i class="fa-solid fa-eye"></i></a>
<a class="btn btn-sm btn-outline-primary" asp-action="Edit" asp-route-id="@item.CustomerId"><i class="fa-solid fa-pen"></i></a>
<form asp-action="Delete" method="post" class="d-inline">@Html.AntiForgeryToken()<input type="hidden" name="id" value="@item.CustomerId" />
<button class="btn btn-sm btn-outline-danger" data-confirm="Deactivate this customer?" type="submit"><i class="fa-solid fa-ban"></i></button></form>
</td></tr>}
</tbody></table></div>@await Html.PartialAsync("_Pager")</div>
''')

write(VIEWS / "Customers" / "Create.cshtml", r'''
@model TourismHajj.Models.Entities.Customer
@{ ViewData["Title"] = "New Customer"; }
<div class="page-header"><h2>New Customer</h2></div>
<div class="card-panel"><div class="card-panel-body">
<form asp-action="Create" method="post">@Html.AntiForgeryToken()
<div class="row g-3">
<div class="col-md-6"><label class="form-label">Customer Name</label><input asp-for="CustomerName" class="form-control" /><span asp-validation-for="CustomerName" class="text-danger"></span></div>
<div class="col-md-6"><label class="form-label">Father Name</label><input asp-for="FatherName" class="form-control" /></div>
<div class="col-md-4"><label class="form-label">Mobile</label><input asp-for="MobileNo" class="form-control" /></div>
<div class="col-md-4"><label class="form-label">WhatsApp</label><input asp-for="WhatsAppNo" class="form-control" /></div>
<div class="col-md-4"><label class="form-label">Email</label><input asp-for="Email" class="form-control" /></div>
<div class="col-md-4"><label class="form-label">CNIC</label><input asp-for="Cnic" class="form-control" /></div>
<div class="col-md-4"><label class="form-label">City</label><select asp-for="CityId" class="form-select" asp-items="ViewBag.Cities"><option value="">--</option></select></div>
<div class="col-md-4"><label class="form-label">Customer Type</label><select asp-for="CustomerType" class="form-select"><option>Individual</option><option>Family</option><option>Corporate</option><option>Agent</option></select></div>
<div class="col-md-12"><label class="form-label">Address</label><textarea asp-for="Address" class="form-control" rows="2"></textarea></div>
<div class="col-md-3 form-check mt-4"><input asp-for="IsActive" class="form-check-input" /><label class="form-check-label">Active</label></div>
</div>
<div class="mt-3"><button class="btn btn-primary" type="submit">Save</button> <a class="btn btn-outline-secondary" asp-action="Index">Cancel</a></div>
</form></div></div>
@section Scripts { <partial name="_ValidationScriptsPartial" /> }
''')

write(VIEWS / "Customers" / "Edit.cshtml", r'''
@model TourismHajj.Models.Entities.Customer
@{ ViewData["Title"] = "Edit Customer"; }
<div class="page-header"><h2>Edit Customer</h2></div>
<div class="card-panel"><div class="card-panel-body">
<form asp-action="Edit" method="post">@Html.AntiForgeryToken()<input type="hidden" asp-for="CustomerId" /><input type="hidden" asp-for="CreatedDate" />
<div class="row g-3">
<div class="col-md-6"><label class="form-label">Customer Name</label><input asp-for="CustomerName" class="form-control" /></div>
<div class="col-md-6"><label class="form-label">Father Name</label><input asp-for="FatherName" class="form-control" /></div>
<div class="col-md-4"><label class="form-label">Mobile</label><input asp-for="MobileNo" class="form-control" /></div>
<div class="col-md-4"><label class="form-label">WhatsApp</label><input asp-for="WhatsAppNo" class="form-control" /></div>
<div class="col-md-4"><label class="form-label">Email</label><input asp-for="Email" class="form-control" /></div>
<div class="col-md-4"><label class="form-label">CNIC</label><input asp-for="Cnic" class="form-control" /></div>
<div class="col-md-4"><label class="form-label">City</label><select asp-for="CityId" class="form-select" asp-items="ViewBag.Cities"><option value="">--</option></select></div>
<div class="col-md-4"><label class="form-label">Customer Type</label><input asp-for="CustomerType" class="form-control" /></div>
<div class="col-md-12"><label class="form-label">Address</label><textarea asp-for="Address" class="form-control" rows="2"></textarea></div>
<div class="col-md-3 form-check mt-4"><input asp-for="IsActive" class="form-check-input" /><label class="form-check-label">Active</label></div>
</div>
<div class="mt-3"><button class="btn btn-primary" type="submit">Update</button> <a class="btn btn-outline-secondary" asp-action="Index">Cancel</a></div>
</form></div></div>
''')

write(VIEWS / "Customers" / "Details.cshtml", r'''
@model TourismHajj.ViewModels.CustomerDetailsViewModel
@{ ViewData["Title"] = "Customer Details"; var c = Model.Customer; }
@await Html.PartialAsync("_Alerts")
<div class="page-header">
<div><h2>@c.CustomerName</h2><p class="text-muted mb-0">@c.CustomerType · @c.MobileNo</p></div>
<div><a class="btn btn-sm btn-primary" asp-action="Edit" asp-route-id="@c.CustomerId">Edit</a>
<a class="btn btn-sm btn-accent" asp-controller="Travelers" asp-action="Create" asp-route-customerId="@c.CustomerId">Add Traveler</a></div>
</div>
<div class="row g-3">
<div class="col-lg-4"><div class="card-panel"><div class="card-panel-header"><h5>Personal Information</h5></div><div class="card-panel-body">
<dl class="row mb-0">
<dt class="col-5">Father</dt><dd class="col-7">@c.FatherName</dd>
<dt class="col-5">WhatsApp</dt><dd class="col-7">@c.WhatsAppNo</dd>
<dt class="col-5">Email</dt><dd class="col-7">@c.Email</dd>
<dt class="col-5">CNIC</dt><dd class="col-7">@c.Cnic</dd>
<dt class="col-5">City</dt><dd class="col-7">@(c.City?.CityName)</dd>
<dt class="col-5">Address</dt><dd class="col-7">@c.Address</dd>
<dt class="col-5">Outstanding</dt><dd class="col-7 fw-bold text-danger">@Model.OutstandingBalance.ToString("N2")</dd>
</dl></div></div></div>
<div class="col-lg-8">
<div class="card-panel mb-3"><div class="card-panel-header"><h5>Registered Travelers</h5></div>
<div class="table-responsive"><table class="table table-sm mb-0"><thead><tr><th>Name</th><th>Passport</th><th>Expiry</th><th>Relation</th></tr></thead>
<tbody>@foreach (var t in Model.Travelers){<tr><td><a asp-controller="Travelers" asp-action="Details" asp-route-id="@t.TravelerId">@t.FullName</a></td><td>@t.PassportNo</td><td>@t.PassportExpiryDate</td><td>@t.Relationship</td></tr>}
@if(!Model.Travelers.Any()){<tr><td colspan="4" class="text-muted text-center">No travelers</td></tr>}</tbody></table></div></div>
<div class="card-panel mb-3"><div class="card-panel-header"><h5>Booking History</h5></div>
<div class="table-responsive"><table class="table table-sm mb-0"><thead><tr><th>Booking</th><th>Package</th><th>Net</th><th>Balance</th><th>Status</th></tr></thead>
<tbody>@foreach (var b in Model.Bookings){<tr><td><a asp-controller="Bookings" asp-action="Details" asp-route-id="@b.BookingId">@b.BookingNo</a></td><td>@b.Package.PackageName</td><td>@((b.NetAmount??0).ToString("N2"))</td><td>@((b.BalanceAmount??0).ToString("N2"))</td><td>@b.BookingStatus</td></tr>}
@if(!Model.Bookings.Any()){<tr><td colspan="5" class="text-muted text-center">No bookings</td></tr>}</tbody></table></div></div>
<div class="card-panel"><div class="card-panel-header"><h5>Payment History</h5></div>
<div class="table-responsive"><table class="table table-sm mb-0"><thead><tr><th>Date</th><th>Amount</th><th>Method</th><th>Ref</th></tr></thead>
<tbody>@foreach (var p in Model.Payments){<tr><td>@p.PaymentDate?.ToString("dd-MMM-yyyy")</td><td>@p.Amount.ToString("N2")</td><td>@p.PaymentMethod</td><td>@p.ReferenceNo</td></tr>}
@if(!Model.Payments.Any()){<tr><td colspan="4" class="text-muted text-center">No payments</td></tr>}</tbody></table></div></div>
</div></div>
''')

write(VIEWS / "Travelers" / "Index.cshtml", r'''
@model IEnumerable<TourismHajj.Models.Entities.Traveler>
@{ ViewData["Title"] = "Travelers"; }
@await Html.PartialAsync("_Alerts")
<div class="page-header"><div><h2>Travelers</h2></div>
<div class="d-flex gap-2">
<form method="get" class="d-flex gap-2"><input class="form-control form-control-sm" name="q" value="@ViewBag.Q" placeholder="Search..." /><button class="btn btn-sm btn-outline-secondary">Search</button></form>
<a class="btn btn-sm btn-outline-warning" asp-action="PassportAlerts">Passport Alerts</a>
<a class="btn btn-sm btn-accent" asp-action="Create"><i class="fa-solid fa-plus"></i> Add</a></div></div>
<div class="card-panel"><div class="table-responsive"><table class="table table-hover mb-0">
<thead><tr><th>Name</th><th>Customer</th><th>Passport</th><th>Expiry</th><th>Mobile</th><th></th></tr></thead>
<tbody>@foreach (var item in Model){<tr>
<td>@item.FullName</td><td>@item.Customer?.CustomerName</td><td>@item.PassportNo</td><td>@item.PassportExpiryDate</td><td>@item.MobileNo</td>
<td class="text-end"><a class="btn btn-sm btn-outline-secondary" asp-action="Details" asp-route-id="@item.TravelerId"><i class="fa-solid fa-eye"></i></a>
<a class="btn btn-sm btn-outline-primary" asp-action="Edit" asp-route-id="@item.TravelerId"><i class="fa-solid fa-pen"></i></a></td></tr>}
</tbody></table></div>@await Html.PartialAsync("_Pager")</div>
''')

write(VIEWS / "Travelers" / "Create.cshtml", r'''
@model TourismHajj.Models.Entities.Traveler
@{ ViewData["Title"] = "New Traveler"; }
<div class="page-header"><h2>New Traveler</h2></div>
<div class="card-panel"><div class="card-panel-body">
<form asp-action="Create" method="post">@Html.AntiForgeryToken()
<div class="row g-3">
<div class="col-md-6"><label class="form-label">Customer</label><select asp-for="CustomerId" class="form-select" asp-items="ViewBag.Customers"></select></div>
<div class="col-md-6"><label class="form-label">Full Name</label><input asp-for="FullName" class="form-control" /></div>
<div class="col-md-4"><label class="form-label">Father Name</label><input asp-for="FatherName" class="form-control" /></div>
<div class="col-md-4"><label class="form-label">Gender</label><select asp-for="Gender" class="form-select"><option>Male</option><option>Female</option></select></div>
<div class="col-md-4"><label class="form-label">Date of Birth</label><input asp-for="DateOfBirth" type="date" class="form-control" /></div>
<div class="col-md-4"><label class="form-label">Nationality</label><select asp-for="NationalityCountryId" class="form-select" asp-items="ViewBag.Countries"><option value="">--</option></select></div>
<div class="col-md-4"><label class="form-label">CNIC</label><input asp-for="Cnic" class="form-control" /></div>
<div class="col-md-4"><label class="form-label">Mobile</label><input asp-for="MobileNo" class="form-control" /></div>
<div class="col-md-4"><label class="form-label">Passport No</label><input asp-for="PassportNo" class="form-control" /></div>
<div class="col-md-4"><label class="form-label">Issue Date</label><input asp-for="PassportIssueDate" type="date" class="form-control" /></div>
<div class="col-md-4"><label class="form-label">Expiry Date</label><input asp-for="PassportExpiryDate" type="date" class="form-control" /></div>
<div class="col-md-4"><label class="form-label">Relationship</label><input asp-for="Relationship" class="form-control" /></div>
</div>
<div class="mt-3"><button class="btn btn-primary" type="submit">Save</button> <a class="btn btn-outline-secondary" asp-action="Index">Cancel</a></div>
</form></div></div>
''')

write(VIEWS / "Travelers" / "Edit.cshtml", r'''
@model TourismHajj.Models.Entities.Traveler
@{ ViewData["Title"] = "Edit Traveler"; }
<div class="page-header"><h2>Edit Traveler</h2></div>
<div class="card-panel"><div class="card-panel-body">
<form asp-action="Edit" method="post">@Html.AntiForgeryToken()<input type="hidden" asp-for="TravelerId" /><input type="hidden" asp-for="CreatedDate" />
<div class="row g-3">
<div class="col-md-6"><label class="form-label">Customer</label><select asp-for="CustomerId" class="form-select" asp-items="ViewBag.Customers"></select></div>
<div class="col-md-6"><label class="form-label">Full Name</label><input asp-for="FullName" class="form-control" /></div>
<div class="col-md-4"><label class="form-label">Father Name</label><input asp-for="FatherName" class="form-control" /></div>
<div class="col-md-4"><label class="form-label">Gender</label><input asp-for="Gender" class="form-control" /></div>
<div class="col-md-4"><label class="form-label">Date of Birth</label><input asp-for="DateOfBirth" type="date" class="form-control" /></div>
<div class="col-md-4"><label class="form-label">Nationality</label><select asp-for="NationalityCountryId" class="form-select" asp-items="ViewBag.Countries"><option value="">--</option></select></div>
<div class="col-md-4"><label class="form-label">CNIC</label><input asp-for="Cnic" class="form-control" /></div>
<div class="col-md-4"><label class="form-label">Mobile</label><input asp-for="MobileNo" class="form-control" /></div>
<div class="col-md-4"><label class="form-label">Passport No</label><input asp-for="PassportNo" class="form-control" /></div>
<div class="col-md-4"><label class="form-label">Issue Date</label><input asp-for="PassportIssueDate" type="date" class="form-control" /></div>
<div class="col-md-4"><label class="form-label">Expiry Date</label><input asp-for="PassportExpiryDate" type="date" class="form-control" /></div>
<div class="col-md-4"><label class="form-label">Relationship</label><input asp-for="Relationship" class="form-control" /></div>
</div>
<div class="mt-3"><button class="btn btn-primary" type="submit">Update</button> <a class="btn btn-outline-secondary" asp-action="Index">Cancel</a></div>
</form></div></div>
''')

write(VIEWS / "Travelers" / "Details.cshtml", r'''
@model TourismHajj.Models.Entities.Traveler
@{ ViewData["Title"] = "Traveler Details"; }
<div class="page-header"><h2>@Model.FullName</h2><a class="btn btn-sm btn-primary" asp-action="Edit" asp-route-id="@Model.TravelerId">Edit</a></div>
<div class="card-panel"><div class="card-panel-body">
<dl class="row">
<dt class="col-sm-3">Customer</dt><dd class="col-sm-9">@Model.Customer?.CustomerName</dd>
<dt class="col-sm-3">Gender / DOB</dt><dd class="col-sm-9">@Model.Gender / @Model.DateOfBirth</dd>
<dt class="col-sm-3">Passport</dt><dd class="col-sm-9">@Model.PassportNo (@Model.PassportIssueDate — @Model.PassportExpiryDate)</dd>
<dt class="col-sm-3">CNIC</dt><dd class="col-sm-9">@Model.Cnic</dd>
<dt class="col-sm-3">Mobile</dt><dd class="col-sm-9">@Model.MobileNo</dd>
<dt class="col-sm-3">Relationship</dt><dd class="col-sm-9">@Model.Relationship</dd>
</dl>
<h5>Documents</h5>
<table class="table table-sm"><thead><tr><th>Type</th><th>Number</th><th>Expiry</th><th>File</th></tr></thead>
<tbody>@foreach (var d in Model.TravelerDocuments){<tr><td>@d.DocumentType</td><td>@d.DocumentNumber</td><td>@d.ExpiryDate</td><td>@d.FilePath</td></tr>}
@if(!Model.TravelerDocuments.Any()){<tr><td colspan="4" class="text-muted">No documents uploaded</td></tr>}</tbody></table>
</div></div>
''')

write(VIEWS / "Travelers" / "PassportAlerts.cshtml", r'''
@model IEnumerable<TourismHajj.ViewModels.PassportAlertItem>
@{ ViewData["Title"] = "Passport Management"; }
<div class="page-header"><h2>Passport Expiry Alerts</h2></div>
<div class="card-panel"><div class="table-responsive"><table class="table table-hover mb-0">
<thead><tr><th>Traveler</th><th>Passport</th><th>Expiry</th><th></th></tr></thead>
<tbody>@foreach (var x in Model){<tr class="@(x.ExpiryDate <= DateOnly.FromDateTime(DateTime.Today) ? "table-danger" : "")">
<td>@x.FullName</td><td>@x.PassportNo</td><td>@x.ExpiryDate</td>
<td><a class="btn btn-sm btn-outline-primary" asp-action="Edit" asp-route-id="@x.TravelerId">Update</a></td></tr>}
@if(!Model.Any()){<tr><td colspan="4" class="text-center text-muted">No upcoming expiries</td></tr>}</tbody></table></div></div>
''')

print("customer/traveler views done")
