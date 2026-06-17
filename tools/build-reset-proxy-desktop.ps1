# Genera accesos en el escritorio para reset-proxy (independiente del repo).
#
# Uso:  pwsh tools/build-reset-proxy-desktop.ps1
#
# Crea en el escritorio:
#   - Reset-Proxy\reset-proxy.ps1  (copia portable)
#   - Reset-Proxy.bat              (doble clic)
#   - Reset-Proxy.exe              (si está instalado el módulo ps2exe)

param(
    [switch]$SkipExe
)

$ErrorActionPreference = "Stop"

$root = Split-Path -Parent $PSScriptRoot
$source = Join-Path $PSScriptRoot "reset-proxy.ps1"
$desktop = [Environment]::GetFolderPath("Desktop")
$folder = Join-Path $desktop "Reset-Proxy"
$ps1Dest = Join-Path $folder "reset-proxy.ps1"
$batPath = Join-Path $desktop "Reset-Proxy.bat"
$exePath = Join-Path $desktop "Reset-Proxy.exe"

if (-not (Test-Path $source)) {
    Write-Error "No se encontró: $source"
}

New-Item -ItemType Directory -Force -Path $folder | Out-Null
Copy-Item -Force $source $ps1Dest

$pwsh = (Get-Command pwsh -ErrorAction SilentlyContinue)?.Source
if (-not $pwsh) {
    $pwsh = (Get-Command powershell -ErrorAction SilentlyContinue).Source
}

$bat = @"
@echo off
title Reset Proxy
"$pwsh" -NoProfile -ExecutionPolicy Bypass -File "%~dp0Reset-Proxy\reset-proxy.ps1"
echo.
pause
"@

Set-Content -Path $batPath -Value $bat -Encoding ASCII
Write-Host "Creado: $ps1Dest" -ForegroundColor Green
Write-Host "Creado: $batPath" -ForegroundColor Green

if (-not $SkipExe) {
    $ps2exe = Get-Command Invoke-ps2exe -ErrorAction SilentlyContinue
    if (-not $ps2exe) {
        Write-Host "`nPS2EXE no instalado; omitiendo .exe." -ForegroundColor Yellow
        Write-Host "Para generar Reset-Proxy.exe:" -ForegroundColor Yellow
        Write-Host "  Install-Module ps2exe -Scope CurrentUser -Force" -ForegroundColor Yellow
        Write-Host "  pwsh tools/build-reset-proxy-desktop.ps1" -ForegroundColor Yellow
    }
    else {
        & Invoke-ps2exe -inputFile $ps1Dest -outputFile $exePath -noConsole:$false
        Write-Host "Creado: $exePath" -ForegroundColor Green
    }
}

Write-Host "`nListo. Ejecutá Reset-Proxy.bat o Reset-Proxy.exe desde el escritorio." -ForegroundColor Cyan
