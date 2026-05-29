# Captura el estado de sesion de LinkedIn (cookies + localStorage) para reutilizarlo
# en los tests, evitando automatizar el login.
#
# COMO USARLO:
#   1) Compila el proyecto al menos una vez:  dotnet build
#   2) Ejecuta este script:                   pwsh Auth/capture-session.ps1
#   3) Se abre un navegador -> logueate normalmente en LinkedIn.
#   4) Cuando estes logueado, cerra la ventana del navegador.
#   5) Se genera Auth/state.json (esta IGNORADO por git: contiene tu sesion).
#
# Nota: si cambias el TargetFramework, ajusta la ruta de playwright.ps1.

$ErrorActionPreference = "Stop"

$root = Split-Path -Parent $PSScriptRoot
$playwright = Join-Path $root "bin/Debug/net10.0/playwright.ps1"
$statePath = Join-Path $PSScriptRoot "state.json"

if (-not (Test-Path $playwright)) {
    Write-Error "No se encontro '$playwright'. Ejecuta 'dotnet build' primero."
    exit 1
}

Write-Host "Abriendo navegador para login manual en LinkedIn..." -ForegroundColor Cyan
Write-Host "Cuando termines de loguearte, cerra la ventana del navegador." -ForegroundColor Cyan

& $playwright codegen --save-storage="$statePath" "https://www.linkedin.com/login"

if (Test-Path $statePath) {
    Write-Host "Sesion guardada en: $statePath" -ForegroundColor Green
} else {
    Write-Warning "No se genero state.json. Volve a intentarlo y asegurate de cerrar la ventana del navegador."
}
