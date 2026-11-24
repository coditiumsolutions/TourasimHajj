#Requires -RunAsAdministrator
<#
.SYNOPSIS
  Enables IIS Web Management Service (WMSVC) + Web Deploy and opens TCP 8172.
  Run this ON the tourism.coditium.com Windows server (RDP as Administrator).
#>
$ErrorActionPreference = 'Stop'

Write-Host "Enabling IIS Management Service feature..."
$features = @(
  'Web-Mgmt-Service',
  'IIS-ManagementService'
)
foreach ($f in $features) {
  try {
    Enable-WindowsOptionalFeature -Online -FeatureName $f -All -NoRestart -ErrorAction SilentlyContinue | Out-Null
  } catch { }
  try {
    Install-WindowsFeature -Name $f -IncludeManagementTools -ErrorAction SilentlyContinue | Out-Null
  } catch { }
}

Write-Host "Configuring WMSVC for remote connections..."
New-ItemProperty -Path 'HKLM:\SOFTWARE\Microsoft\WebManagement\Server' -Name 'EnableRemoteManagement' -Value 1 -PropertyType DWORD -Force | Out-Null
Set-Service -Name WMSVC -StartupType Automatic -ErrorAction SilentlyContinue
Start-Service WMSVC -ErrorAction SilentlyContinue

$webDeployMsiUrl = 'https://download.microsoft.com/download/b/d/0/bd0f4e1c-5c7e-4b3a-8f0a-5e0f5c1b0f5e/WebDeploy_amd64_en-US.msi'
$webDeployPaths = @(
  'C:\Program Files\IIS\Microsoft Web Deploy V4\msdeploy.exe',
  'C:\Program Files\IIS\Microsoft Web Deploy V3\msdeploy.exe',
  'C:\Program Files (x86)\IIS\Microsoft Web Deploy V3\msdeploy.exe'
)
$hasMsDeploy = $webDeployPaths | Where-Object { Test-Path $_ } | Select-Object -First 1
if (-not $hasMsDeploy) {
  Write-Host "Web Deploy not found. Attempting chocolatey install (if available)..."
  if (Get-Command choco -ErrorAction SilentlyContinue) {
    choco install webdeploy -y --no-progress
  } else {
    Write-Host "Install Web Deploy manually from: https://www.iis.net/downloads/microsoft/web-deploy"
    Write-Host "Or: winget install Microsoft.WebDeploy"
    if (Get-Command winget -ErrorAction SilentlyContinue) {
      winget install --id Microsoft.WebDeploy --accept-package-agreements --accept-source-agreements -e
    }
  }
}

Write-Host "Opening firewall TCP 8172..."
$ruleName = 'Web Deploy / WMSVC 8172'
Get-NetFirewallRule -DisplayName $ruleName -ErrorAction SilentlyContinue | Remove-NetFirewallRule -ErrorAction SilentlyContinue
New-NetFirewallRule -DisplayName $ruleName -Direction Inbound -Protocol TCP -LocalPort 8172 -Action Allow -Profile Any | Out-Null

# Also ensure WMSVC built-in rule is enabled if present
Get-NetFirewallRule -DisplayName '*Web Management*' -ErrorAction SilentlyContinue | Enable-NetFirewallRule -ErrorAction SilentlyContinue
Get-NetFirewallRule -DisplayName '*8172*' -ErrorAction SilentlyContinue | Enable-NetFirewallRule -ErrorAction SilentlyContinue

$sites = @()
try {
  Import-Module WebAdministration -ErrorAction Stop
  $sites = Get-Website | Select-Object Name, State, PhysicalPath, @{n='Bindings';e={ $_.Bindings.Collection.bindingInformation -join '; ' }}
  $sites | Format-Table -AutoSize | Out-String | Write-Host
} catch {
  Write-Warning "Could not list IIS sites: $($_.Exception.Message)"
}

Write-Host ""
Write-Host "Done. Verify from outside: Test-NetConnection 138.201.247.223 -Port 8172"
Write-Host "Use IIS site name (exact) as WEBDEPLOY_SITE_NAME."
if ($sites) {
  Write-Host "Detected sites: $(($sites | ForEach-Object { $_.Name }) -join ', ')"
}
