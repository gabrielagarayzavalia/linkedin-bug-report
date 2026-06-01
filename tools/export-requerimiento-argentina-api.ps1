# Exporta IMPROV-REQ-001 a Word y PDF: "Requerimiento = Argentina-API"
#
# Uso: pwsh tools/export-requerimiento-argentina-api.ps1
#
# Requisitos:
#   - Pandoc (MD -> DOCX): winget install JohnMacFarlane.Pandoc
#   - PDF: Microsoft Word o LibreOffice en PATH

param(
    [string]$SourceMd = "",
    [string]$OutputBaseName = "Requerimiento = Argentina-API"
)

$ErrorActionPreference = "Stop"

$root = Split-Path -Parent $PSScriptRoot
$reqDir = Join-Path $root "docs/requirements"

if ([string]::IsNullOrWhiteSpace($SourceMd)) {
    $SourceMd = Join-Path $reqDir "IMPROV-REQ-001-location-caba-api-combobox.md"
}

$docxPath = Join-Path $reqDir "$OutputBaseName.docx"
$pdfPath = Join-Path $reqDir "$OutputBaseName.pdf"

function Get-PandocPath {
    $cmd = Get-Command pandoc -ErrorAction SilentlyContinue
    if ($cmd) { return $cmd.Source }

    $wingetPaths = @(
        "$env:LOCALAPPDATA\Pandoc\pandoc.exe",
        "$env:ProgramFiles\Pandoc\pandoc.exe"
    )
    foreach ($p in $wingetPaths) {
        if (Test-Path $p) { return $p }
    }
    return $null
}

function Convert-MdToDocx {
    param([string]$PandocExe)

    if (-not (Test-Path $SourceMd)) {
        throw "No se encontro fuente: $SourceMd"
    }

    Write-Host "Generando DOCX..." -ForegroundColor Cyan
    & $PandocExe $SourceMd `
        -o $docxPath `
        --from markdown `
        --toc `
        --metadata title="Requerimiento Argentina-API — Location CABA/PBA"

    if (-not (Test-Path $docxPath)) {
        throw "No se genero el DOCX en $docxPath"
    }
    Write-Host "DOCX: $docxPath" -ForegroundColor Green
}

function Convert-DocxToPdf-WordCom {
    if (-not (Test-Path $docxPath)) { return $false }

    try {
        Write-Host "Generando PDF via Microsoft Word..." -ForegroundColor Cyan
        $word = New-Object -ComObject Word.Application
        $word.Visible = $false
        $doc = $word.Documents.Open((Resolve-Path $docxPath).Path)
        # wdExportFormatPDF = 17
        $doc.ExportAsFixedFormat($pdfPath, 17)
        $doc.Close([ref]0)
        $word.Quit()
        [System.Runtime.InteropServices.Marshal]::ReleaseComObject($doc) | Out-Null
        [System.Runtime.InteropServices.Marshal]::ReleaseComObject($word) | Out-Null
        return (Test-Path $pdfPath)
    }
    catch {
        Write-Host "Word COM no disponible: $($_.Exception.Message)" -ForegroundColor Yellow
        return $false
    }
}

function Convert-DocxToPdf-LibreOffice {
    if (-not (Test-Path $docxPath)) { return $false }

    $soffice = Get-Command soffice -ErrorAction SilentlyContinue
    if (-not $soffice) { return $false }

    Write-Host "Generando PDF via LibreOffice..." -ForegroundColor Cyan
    $tempOut = Join-Path $env:TEMP "pandoc-pdf-export"
    New-Item -ItemType Directory -Force -Path $tempOut | Out-Null

    & $soffice.Source --headless --convert-to pdf --outdir $tempOut $docxPath
    $generated = Join-Path $tempOut ([IO.Path]::GetFileNameWithoutExtension($docxPath) + ".pdf")
    if (Test-Path $generated) {
        Move-Item -Force $generated $pdfPath
        return $true
    }
    return $false
}

Push-Location $root
try {
    $pandoc = Get-PandocPath
    if (-not $pandoc) {
        Write-Host "Pandoc no encontrado. Instalando con winget..." -ForegroundColor Yellow
        winget install --id JohnMacFarlane.Pandoc -e --accept-source-agreements --accept-package-agreements
        $pandoc = Get-PandocPath
    }

    if (-not $pandoc) {
        Write-Error @"
Pandoc es necesario para generar el DOCX.
Instalar manualmente: winget install JohnMacFarlane.Pandoc
Luego re-ejecutar: pwsh tools/export-requerimiento-argentina-api.ps1
"@
    }

    Convert-MdToDocx -PandocExe $pandoc

    $pdfOk = Convert-DocxToPdf-WordCom
    if (-not $pdfOk) {
        $pdfOk = Convert-DocxToPdf-LibreOffice
    }

    if (-not $pdfOk) {
        Write-Error @"
No se pudo generar el PDF automaticamente.
Opciones:
  1) Abrir '$docxPath' en Word y guardar como PDF en:
     '$pdfPath'
  2) Instalar LibreOffice y agregar soffice al PATH
"@
    }

    Write-Host "PDF: $pdfPath" -ForegroundColor Green
    Write-Host "Exportacion completada." -ForegroundColor Green
}
finally {
    Pop-Location
}
