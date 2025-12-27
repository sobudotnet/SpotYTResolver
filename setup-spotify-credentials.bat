@echo off
REM SpotYT - Setup Script
REM Just run this as admin and it'll set up your credentials

echo.
echo ======================================
echo SpotYT Setup
echo ======================================
echo.

REM Check if running as administrator
net session >nul 2>&1
if %errorLevel% neq 0 (
    echo ERROR: You gotta run this as Administrator!
    echo Right-click this file and pick "Run as administrator"
    pause
    exit /b 1
)

echo.
echo Alright, let's get you set up.
echo.
echo First, go grab your credentials:
echo 1. Head to https://developer.spotify.com/dashboard
echo 2. Log in or create an account
echo 3. Create a new app
echo 4. IMPORTANT: Make sure to enable the Web API when setting it up!
echo 5. Copy your Client ID and Client Secret from the dashboard
echo.

set /p SPOTIFY_CLIENT_ID="Paste your Client ID here: "
set /p SPOTIFY_CLIENT_SECRET="Paste your Client Secret here: "

if "%SPOTIFY_CLIENT_ID%"=="" (
    echo Nope, you need to enter your Client ID
    pause
    exit /b 1
)

if "%SPOTIFY_CLIENT_SECRET%"=="" (
    echo Nope, you need to enter your Client Secret too
    pause
    exit /b 1
)

echo.
echo Setting up your environment variables...
setx SPOTIFY_CLIENT_ID "%SPOTIFY_CLIENT_ID%"
setx SPOTIFY_CLIENT_SECRET "%SPOTIFY_CLIENT_SECRET%"

if %errorLevel% equ 0 (
    echo.
    echo Done! Your credentials are saved.
    echo.
    echo Close this window and reopen SpotYT - you're all set!
    echo.
) else (
    echo Hmm, something went wrong setting those variables
    pause
    exit /b 1
)

pause
