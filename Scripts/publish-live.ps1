# Publishes to ./live_tourism and applies Production DB settings from Development config.
$ErrorActionPreference = "Stop"
$root = Split-Path -Parent $PSScriptRoot
$out = Join-Path $root "live_tourism"
$devSettings = Join-Path $root "appsettings.Development.json"

Write-Host "Publishing Release to $out ..."
dotnet publish (Join-Path $root "TourismHajj.csproj") -c Release -o $out --nologo
if ($LASTEXITCODE -ne 0) { throw "dotnet publish failed" }

if (-not (Test-Path $devSettings)) {
    throw "Missing appsettings.Development.json (needed for Production connection string)."
}

$dev = Get-Content $devSettings -Raw | ConvertFrom-Json
$conn = $dev.ConnectionStrings.DefaultConnection
if ([string]::IsNullOrWhiteSpace($conn) -or $conn -match "YOUR_SERVER") {
    throw "Development connection string is missing or still a placeholder."
}

$appSettingsPath = Join-Path $out "appsettings.json"
$app = Get-Content $appSettingsPath -Raw | ConvertFrom-Json
$app.ConnectionStrings.DefaultConnection = $conn
$app | ConvertTo-Json -Depth 8 | Set-Content $appSettingsPath -Encoding UTF8

$webConfig = @"
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <location path="." inheritInChildApplications="false">
    <system.webServer>
      <handlers>
        <add name="aspNetCore" path="*" verb="*" modules="AspNetCoreModuleV2" resourceType="Unspecified" />
      </handlers>
      <aspNetCore processPath="dotnet" arguments=".\TourismHajj.dll" stdoutLogEnabled="true" stdoutLogFile=".\logs\stdout" hostingModel="inprocess">
        <environmentVariables>
          <environmentVariable name="ASPNETCORE_ENVIRONMENT" value="Production" />
        </environmentVariables>
      </aspNetCore>
    </system.webServer>
  </location>
</configuration>
"@
Set-Content (Join-Path $out "web.config") -Value $webConfig -Encoding UTF8
New-Item -ItemType Directory -Force -Path (Join-Path $out "logs") | Out-Null

Write-Host "Done. Upload the contents of live_tourism to the IIS site root."
Write-Host "Confirm ASP.NET Core 8 Hosting Bundle is installed on the server."
