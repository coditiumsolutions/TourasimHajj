from pathlib import Path
ROOT = Path(r"D:\Dotnet\CoreCursor\Tourism Hajj")
VIEWS = ROOT / "Views"

def write(path: Path, content: str):
    path.parent.mkdir(parents=True, exist_ok=True)
    path.write_text(content.strip() + "\n", encoding="utf-8")
    print("wrote", path.relative_to(ROOT))

# Shared partials
write(VIEWS / "Shared" / "_Pager.cshtml", r'''
@{
    int page = (int)(ViewBag.Page ?? 1);
    int pageSize = (int)(ViewBag.PageSize ?? 10);
    int total = (int)(ViewBag.Total ?? 0);
    int pages = pageSize == 0 ? 1 : (int)Math.Ceiling(total / (double)pageSize);
}
@if (pages > 1)
{
<nav class="mt-3">
<ul class="pagination pagination-sm">
@for (int i = 1; i <= pages; i++)
{
<li class="page-item @(i==page?"active":"")"><a class="page-link" href="?page=@i&q=@ViewBag.Q">@i</a></li>
}
</ul>
</nav>
}
''')

write(VIEWS / "Shared" / "_Alerts.cshtml", r'''
@if (TempData["Success"] != null)
{
<script>toastr.success("@TempData["Success"]");</script>
}
@if (TempData["Error"] != null)
{
<script>toastr.error("@TempData["Error"]");</script>
}
''')

def index_view(title, model_type, columns, create_action="Create"):
    headers = "".join(f"<th>{c[0]}</th>" for c in columns)
    cells = "".join(f"<td>@{c[1]}</td>" for c in columns)
    return f'''
@model IEnumerable<{model_type}>
@{{ ViewData["Title"] = "{title}"; }}
@await Html.PartialAsync("_Alerts")
<div class="page-header">
  <div><h2>{title}</h2></div>
  <div class="d-flex gap-2">
    <form method="get" class="d-flex gap-2">
      <input class="form-control form-control-sm" name="q" value="@ViewBag.Q" placeholder="Search..." />
      <button class="btn btn-sm btn-outline-secondary" type="submit">Search</button>
    </form>
    <a class="btn btn-sm btn-accent" asp-action="{create_action}"><i class="fa-solid fa-plus"></i> Add</a>
  </div>
</div>
<div class="card-panel">
<div class="table-responsive">
<table class="table table-hover mb-0">
<thead><tr>{headers}<th class="text-end">Actions</th></tr></thead>
<tbody>
@foreach (var item in Model)
{{
<tr>{cells}
<td class="text-end text-nowrap">
<a class="btn btn-sm btn-outline-primary" asp-action="Details" asp-route-id="@item.GetType().GetProperty(item.GetType().Name.Replace(\"ViewModel\",\"\") + \"Id\")?.GetValue(item) ?? item.GetType().GetProperties().First(p=>p.Name.EndsWith(\"Id\")).GetValue(item)"><i class="fa-solid fa-eye"></i></a>
</td></tr>
}}
</tbody></table></div>
@await Html.PartialAsync("_Pager")
</div>
'''

# Better to write explicit views for reliability

# Countries
write(VIEWS / "Countries" / "Index.cshtml", r'''
@model IEnumerable<TourismHajj.Models.Entities.Country>
@{ ViewData["Title"] = "Countries"; }
@await Html.PartialAsync("_Alerts")
<div class="page-header"><div><h2>Countries</h2></div>
<div class="d-flex gap-2">
<form method="get" class="d-flex gap-2"><input class="form-control form-control-sm" name="q" value="@ViewBag.Q" placeholder="Search..." /><button class="btn btn-sm btn-outline-secondary">Search</button></form>
<a class="btn btn-sm btn-accent" asp-action="Create"><i class="fa-solid fa-plus"></i> Add</a></div></div>
<div class="card-panel"><div class="table-responsive"><table class="table table-hover mb-0">
<thead><tr><th>Name</th><th>Code</th><th>Active</th><th></th></tr></thead>
<tbody>
@foreach (var item in Model) {
<tr>
<td>@item.CountryName</td><td>@item.CountryCode</td><td>@(item.IsActive==true?"Yes":"No")</td>
<td class="text-end">
<a class="btn btn-sm btn-outline-primary" asp-action="Edit" asp-route-id="@item.CountryId"><i class="fa-solid fa-pen"></i></a>
<a class="btn btn-sm btn-outline-secondary" asp-action="Details" asp-route-id="@item.CountryId"><i class="fa-solid fa-eye"></i></a>
<form asp-action="Delete" asp-route-id="@item.CountryId" method="post" class="d-inline">@Html.AntiForgeryToken()
<button type="submit" class="btn btn-sm btn-outline-danger" data-confirm="Deactivate this country?"><i class="fa-solid fa-ban"></i></button></form>
</td></tr>}
</tbody></table></div>@await Html.PartialAsync("_Pager")</div>
''')

write(VIEWS / "Countries" / "Create.cshtml", r'''
@model TourismHajj.Models.Entities.Country
@{ ViewData["Title"] = "New Country"; }
<div class="page-header"><h2>New Country</h2></div>
<div class="card-panel"><div class="card-panel-body">
<form asp-action="Create" method="post">@Html.AntiForgeryToken()
<div class="row g-3">
<div class="col-md-6"><label asp-for="CountryName" class="form-label"></label><input asp-for="CountryName" class="form-control" /><span asp-validation-for="CountryName" class="text-danger"></span></div>
<div class="col-md-3"><label asp-for="CountryCode" class="form-label"></label><input asp-for="CountryCode" class="form-control" /></div>
<div class="col-md-3 form-check mt-4"><input asp-for="IsActive" class="form-check-input" /><label asp-for="IsActive" class="form-check-label"></label></div>
</div>
<div class="mt-3"><button class="btn btn-primary" type="submit">Save</button> <a class="btn btn-outline-secondary" asp-action="Index">Cancel</a></div>
</form></div></div>
@section Scripts { <partial name="_ValidationScriptsPartial" /> }
''')

write(VIEWS / "Countries" / "Edit.cshtml", r'''
@model TourismHajj.Models.Entities.Country
@{ ViewData["Title"] = "Edit Country"; }
<div class="page-header"><h2>Edit Country</h2></div>
<div class="card-panel"><div class="card-panel-body">
<form asp-action="Edit" method="post">@Html.AntiForgeryToken()<input type="hidden" asp-for="CountryId" />
<div class="row g-3">
<div class="col-md-6"><label asp-for="CountryName" class="form-label"></label><input asp-for="CountryName" class="form-control" /></div>
<div class="col-md-3"><label asp-for="CountryCode" class="form-label"></label><input asp-for="CountryCode" class="form-control" /></div>
<div class="col-md-3 form-check mt-4"><input asp-for="IsActive" class="form-check-input" /><label asp-for="IsActive" class="form-check-label"></label></div>
</div>
<div class="mt-3"><button class="btn btn-primary" type="submit">Update</button> <a class="btn btn-outline-secondary" asp-action="Index">Cancel</a></div>
</form></div></div>
''')

write(VIEWS / "Countries" / "Details.cshtml", r'''
@model TourismHajj.Models.Entities.Country
@{ ViewData["Title"] = "Country Details"; }
<div class="page-header"><h2>@Model.CountryName</h2><a class="btn btn-sm btn-primary" asp-action="Edit" asp-route-id="@Model.CountryId">Edit</a></div>
<div class="card-panel"><div class="card-panel-body">
<dl class="row mb-0"><dt class="col-sm-3">Code</dt><dd class="col-sm-9">@Model.CountryCode</dd>
<dt class="col-sm-3">Active</dt><dd class="col-sm-9">@(Model.IsActive==true?"Yes":"No")</dd></dl>
</div></div>
''')

# Generic helper for simple entity CRUD views
def crud_views(folder, entity, id_prop, fields, title):
    # fields: list of (label, prop, input_type)
    index_cols = "".join(f"<th>{f[0]}</th>" for f in fields[:4])
    index_cells = "".join(f"<td>@item.{f[1]}</td>" for f in fields[:4])
    write(VIEWS / folder / "Index.cshtml", f'''
@model IEnumerable<TourismHajj.Models.Entities.{entity}>
@{{ ViewData["Title"] = "{title}"; }}
@await Html.PartialAsync("_Alerts")
<div class="page-header"><div><h2>{title}</h2></div>
<div class="d-flex gap-2">
<form method="get" class="d-flex gap-2"><input class="form-control form-control-sm" name="q" value="@ViewBag.Q" placeholder="Search..." /><button class="btn btn-sm btn-outline-secondary">Search</button></form>
<a class="btn btn-sm btn-accent" asp-action="Create"><i class="fa-solid fa-plus"></i> Add</a></div></div>
<div class="card-panel"><div class="table-responsive"><table class="table table-hover mb-0">
<thead><tr>{index_cols}<th></th></tr></thead><tbody>
@foreach (var item in Model) {{
<tr>{index_cells}
<td class="text-end">
<a class="btn btn-sm btn-outline-primary" asp-action="Edit" asp-route-id="@item.{id_prop}"><i class="fa-solid fa-pen"></i></a>
<a class="btn btn-sm btn-outline-secondary" asp-action="Details" asp-route-id="@item.{id_prop}"><i class="fa-solid fa-eye"></i></a>
<form asp-action="Delete" asp-route-id="@item.{id_prop}" method="post" class="d-inline">@Html.AntiForgeryToken()
<button class="btn btn-sm btn-outline-danger" data-confirm="Deactivate this record?" type="submit"><i class="fa-solid fa-ban"></i></button></form>
</td></tr>}}
</tbody></table></div>@await Html.PartialAsync("_Pager")</div>
''')
    form_fields = []
    for label, prop, itype in fields:
        if itype == "checkbox":
            form_fields.append(f'<div class="col-md-3 form-check mt-4"><input asp-for="{prop}" class="form-check-input" /><label asp-for="{prop}" class="form-check-label">{label}</label></div>')
        elif itype == "select-country":
            form_fields.append(f'<div class="col-md-4"><label class="form-label">{label}</label><select asp-for="{prop}" class="form-select" asp-items="ViewBag.Countries"></select></div>')
        elif itype == "select-city":
            form_fields.append(f'<div class="col-md-4"><label class="form-label">{label}</label><select asp-for="{prop}" class="form-select" asp-items="ViewBag.Cities"></select></div>')
        elif itype == "select-supplier":
            form_fields.append(f'<div class="col-md-4"><label class="form-label">{label}</label><select asp-for="{prop}" class="form-select" asp-items="ViewBag.Suppliers"><option value="">--</option></select></div>')
        elif itype == "textarea":
            form_fields.append(f'<div class="col-md-12"><label class="form-label">{label}</label><textarea asp-for="{prop}" class="form-control" rows="3"></textarea></div>')
        elif itype == "date":
            form_fields.append(f'<div class="col-md-3"><label class="form-label">{label}</label><input asp-for="{prop}" class="form-control" type="date" /></div>')
        else:
            form_fields.append(f'<div class="col-md-4"><label class="form-label">{label}</label><input asp-for="{prop}" class="form-control" /></div>')
    fields_html = "\n".join(form_fields)
    write(VIEWS / folder / "Create.cshtml", f'''
@model TourismHajj.Models.Entities.{entity}
@{{ ViewData["Title"] = "New {title.rstrip('s') if title.endswith('s') else title}"; }}
<div class="page-header"><h2>@ViewData["Title"]</h2></div>
<div class="card-panel"><div class="card-panel-body">
<form asp-action="Create" method="post">@Html.AntiForgeryToken()
<div class="row g-3">{fields_html}</div>
<div class="mt-3"><button class="btn btn-primary" type="submit">Save</button> <a class="btn btn-outline-secondary" asp-action="Index">Cancel</a></div>
</form></div></div>
''')
    write(VIEWS / folder / "Edit.cshtml", f'''
@model TourismHajj.Models.Entities.{entity}
@{{ ViewData["Title"] = "Edit {title.rstrip('s') if title.endswith('s') else title}"; }}
<div class="page-header"><h2>@ViewData["Title"]</h2></div>
<div class="card-panel"><div class="card-panel-body">
<form asp-action="Edit" method="post">@Html.AntiForgeryToken()<input type="hidden" asp-for="{id_prop}" />
<div class="row g-3">{fields_html}</div>
<div class="mt-3"><button class="btn btn-primary" type="submit">Update</button> <a class="btn btn-outline-secondary" asp-action="Index">Cancel</a></div>
</form></div></div>
''')
    details = "".join(f'<dt class="col-sm-3">{f[0]}</dt><dd class="col-sm-9">@Model.{f[1]}</dd>' for f in fields)
    write(VIEWS / folder / "Details.cshtml", f'''
@model TourismHajj.Models.Entities.{entity}
@{{ ViewData["Title"] = "{title} Details"; }}
<div class="page-header"><h2>{title} Details</h2><a class="btn btn-sm btn-primary" asp-action="Edit" asp-route-id="@Model.{id_prop}">Edit</a></div>
<div class="card-panel"><div class="card-panel-body"><dl class="row mb-0">{details}</dl></div></div>
''')

crud_views("Cities", "City", "CityId", [
    ("City", "CityName", "text"),
    ("Country Id", "CountryId", "select-country"),
    ("Active", "IsActive", "checkbox"),
], "Cities")

crud_views("Destinations", "Destination", "DestinationId", [
    ("Name", "DestinationName", "text"),
    ("City", "CityId", "select-city"),
    ("Featured", "IsFeatured", "checkbox"),
    ("Active", "IsActive", "checkbox"),
    ("Description", "Description", "textarea"),
], "Destinations")

crud_views("Services", "Service", "ServiceId", [
    ("Name", "ServiceName", "text"),
    ("Category", "ServiceCategory", "text"),
    ("Active", "IsActive", "checkbox"),
    ("Description", "Description", "textarea"),
], "Services")

crud_views("Employees", "Employee", "EmployeeId", [
    ("Name", "EmployeeName", "text"),
    ("Designation", "Designation", "text"),
    ("Mobile", "MobileNo", "text"),
    ("Email", "Email", "text"),
    ("Joining", "JoiningDate", "date"),
    ("Salary", "Salary", "text"),
    ("Active", "IsActive", "checkbox"),
], "Employees")

crud_views("HajjSeasons", "HajjSeason", "HajjSeasonId", [
    ("Season", "SeasonName", "text"),
    ("Year", "HajjYear", "text"),
    ("Reg Start", "RegistrationStartDate", "date"),
    ("Reg End", "RegistrationEndDate", "date"),
    ("Dep Start", "DepartureStartDate", "date"),
    ("Dep End", "DepartureEndDate", "date"),
    ("Notes", "Notes", "textarea"),
    ("Active", "IsActive", "checkbox"),
], "Hajj Seasons")

print("basic CRUD views done")
