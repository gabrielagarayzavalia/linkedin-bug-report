# Restaura Location del perfil de prueba al baseline (CABA, Argentina).
#
# Uso: pwsh tools/restore-location.ps1

$ErrorActionPreference = "Stop"

$root = Split-Path -Parent $PSScriptRoot

Push-Location $root
try {
    if (-not (Test-Path "bin/Debug/net10.0/cabaVsPBA.dll")) {
        Write-Host "Compilando proyecto..." -ForegroundColor Cyan
        dotnet build
    }

    Write-Host "Restaurando Location baseline..." -ForegroundColor Cyan
    dotnet test --filter "Name=RestaurarLocationBaseline" --settings .runsettings
}
finally {
    Pop-Location
}
