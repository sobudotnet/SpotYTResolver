# SpotYT - Setup Script (PowerShell)
# Just run this as admin and you're good

Write-Host ""
Write-Host "======================================" -ForegroundColor Green
Write-Host "SpotYT Setup" -ForegroundColor Green
Write-Host "======================================" -ForegroundColor Green
Write-Host ""

# Check if running as administrator
$isAdmin = ([Security.Principal.WindowsPrincipal] [Security.Principal.WindowsIdentity]::GetCurrent()).IsInRole([Security.Principal.WindowsBuiltInRole] "Administrator")

if (-not $isAdmin) {
    Write-Host "ERROR: You gotta run this as Administrator!" -ForegroundColor Red
    Write-Host "Right-click PowerShell and pick 'Run as administrator'" -ForegroundColor Red
    Read-Host "Press Enter to exit"
    exit 1
}

Write-Host "Alright, let's get you set up." -ForegroundColor Cyan
Write-Host ""
Write-Host "First, go grab your credentials:" -ForegroundColor Yellow
Write-Host "1. Head to https://developer.spotify.com/dashboard"
Write-Host "2. Log in or create an account"
Write-Host "3. Create a new app"
Write-Host "4. IMPORTANT: Make sure to enable the Web API when setting it up!"
Write-Host "5. Copy your Client ID and Client Secret from the dashboard"
Write-Host ""

$clientId = Read-Host "Paste your Client ID"
$clientSecret = Read-Host "Paste your Client Secret" -AsSecureString

# Convert SecureString to plain text for storage
$clientSecretPlain = [System.Runtime.InteropServices.Marshal]::PtrToStringAuto([System.Runtime.InteropServices.Marshal]::SecureStringToCoTaskMemUnicode($clientSecret))

if ([string]::IsNullOrWhiteSpace($clientId)) {
    Write-Host "Nope, you need to enter your Client ID" -ForegroundColor Red
    Read-Host "Press Enter to exit"
    exit 1
}

if ([string]::IsNullOrWhiteSpace($clientSecretPlain)) {
    Write-Host "Nope, you need to enter your Client Secret too" -ForegroundColor Red
    Read-Host "Press Enter to exit"
    exit 1
}

Write-Host ""
Write-Host "Setting up your environment variables..." -ForegroundColor Cyan

# Set environment variables for current user
[Environment]::SetEnvironmentVariable("SPOTIFY_CLIENT_ID", $clientId, "User")
[Environment]::SetEnvironmentVariable("SPOTIFY_CLIENT_SECRET", $clientSecretPlain, "User")

Write-Host ""
Write-Host "Done! Your credentials are saved." -ForegroundColor Green
Write-Host ""
Write-Host "Close this window and reopen SpotYT - you're all set!" -ForegroundColor Yellow
Write-Host ""

Read-Host "Press Enter to exit"
