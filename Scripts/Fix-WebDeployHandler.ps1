$ErrorActionPreference = "Continue"
$log = "C:\Temp\webdeploy-fix.log"
New-Item -ItemType Directory -Force -Path C:\Temp | Out-Null
function L($m) { "$(Get-Date -Format o) $m" | Tee-Object -FilePath $log -Append }

L "START"
if (-not (Test-Path "HKLM:\SOFTWARE\Microsoft\WebManagement\Server")) {
  New-Item -Path "HKLM:\SOFTWARE\Microsoft\WebManagement\Server" -Force | Out-Null
}
New-ItemProperty -Path "HKLM:\SOFTWARE\Microsoft\WebManagement\Server" -Name EnableRemoteManagement -Value 1 -PropertyType DWord -Force | Out-Null

$guid = "{DE4FFB86-1944-4273-B040-92E920BD6861}"
L "Uninstalling existing Web Deploy $guid"
$p = Start-Process msiexec.exe -ArgumentList "/x $guid /qn /norestart /L*v C:\Temp\webdeploy-uninstall.log" -Wait -PassThru
L "Uninstall exit=$($p.ExitCode)"

$msi = "C:\Temp\WebDeploy_amd64_en-US.msi"
if (-not (Test-Path $msi) -or ((Get-Item $msi).Length -lt 1000000)) {
  L "Downloading Web Deploy MSI"
  try {
    [Net.ServicePointManager]::SecurityProtocol = [Net.SecurityProtocolType]::Tls12
    Invoke-WebRequest -Uri "https://download.microsoft.com/download/0/1/D/01DC28EA-638C-4A22-A57B-4CEF97755C6C/WebDeploy_amd64_en-US.msi" -OutFile $msi -UseBasicParsing
    L "Downloaded bytes=$((Get-Item $msi).Length)"
  } catch {
    L "Download failed: $($_.Exception.Message)"
  }
}

if (Test-Path $msi) {
  L "Installing Web Deploy ADDLOCAL=ALL"
  $p2 = Start-Process msiexec.exe -ArgumentList "/i `"$msi`" ADDLOCAL=ALL /qn /norestart /L*v C:\Temp\webdeploy-reinstall.log" -Wait -PassThru
  L "Install exit=$($p2.ExitCode)"
} else {
  L "MSI missing"
}

netsh advfirewall firewall delete rule name="Web Deploy WMSvc 8172 TCP-In" | Out-Null
netsh advfirewall firewall add rule name="Web Deploy WMSvc 8172 TCP-In" dir=in action=allow protocol=TCP localport=8172 | Out-Null

try { Set-Service WMSVC -StartupType Automatic; Restart-Service WMSVC -Force; L "WMSVC restarted" } catch { L $_.Exception.Message }
Start-Sleep -Seconds 2
try { Set-Service MsDepSvc -StartupType Automatic -ErrorAction SilentlyContinue; Start-Service MsDepSvc -ErrorAction SilentlyContinue; L "MsDepSvc ok" } catch { L $_.Exception.Message }

Get-Service WMSVC, MsDepSvc -ErrorAction SilentlyContinue | ForEach-Object { L ("SVC $($_.Name)=$($_.Status)") }

try {
  Add-Type @"
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
public class TrustAllCerts {
  public static bool Validate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors) { return true; }
}
"@ -ErrorAction SilentlyContinue
  [System.Net.ServicePointManager]::ServerCertificateValidationCallback = [System.Net.Security.RemoteCertificateValidationCallback]([TrustAllCerts]::Validate)
  $req = [System.Net.HttpWebRequest]::Create("https://127.0.0.1:8172/msdeploy.axd")
  $req.Timeout = 10000
  try {
    $resp = $req.GetResponse()
    L ("axd=" + [int]$resp.StatusCode)
    $resp.Close()
  } catch [System.Net.WebException] {
    if ($_.Exception.Response) { L ("axd=" + [int]$_.Exception.Response.StatusCode) } else { L $_.Exception.Message }
  }
} catch {
  L $_.Exception.Message
}

Import-Module WebAdministration -ErrorAction SilentlyContinue
Get-Website -ErrorAction SilentlyContinue | ForEach-Object { L ("SITE $($_.Name) $($_.State) $($_.PhysicalPath)") }
L "DONE"
