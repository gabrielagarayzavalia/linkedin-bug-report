# Comandos de ejecución — cabaVsPBA

Referencia de comandos `dotnet test` con parámetros y paths. Índice TC → método: [`test-cases/INDEX.md`](test-cases/INDEX.md).

## Rutas del proyecto

| Concepto | Path |
|----------|------|
| Raíz del repo | `C:\Users\gabri\projects\linkedin\cabaVsPBA` |
| Config Playwright | `.runsettings` |
| Sesión LinkedIn | `Auth\state.json` |
| Captura de sesión | `Auth\capture-session.ps1` |
| Playwright CLI | `bin\Debug\net10.0\playwright.ps1` |
| Baseline (evidencia) | `TestResults\baseline\<NombreDelTest>\` |
| Restore Location | `tools\restore-location.ps1` |
| Export Word/PDF | `tools\export-requerimiento-argentina-api.ps1` |

Siempre ejecutar desde la raíz:

```powershell
cd C:\Users\gabri\projects\linkedin\cabaVsPBA
```

---

## Precondiciones

```powershell
cd C:\Users\gabri\projects\linkedin\cabaVsPBA
dotnet build
pwsh bin\Debug\net10.0\playwright.ps1 install
```

Sesión LinkedIn (si no existe `Auth\state.json`):

```powershell
pwsh Auth\capture-session.ps1
```

Alternativa vía test:

```powershell
dotnet test --filter "Name=CapturarSesion" --settings .runsettings
```

Smoke test de sesión:

```powershell
dotnet test --filter "Name=EntraLogueadoSinLogin" --settings .runsettings
```

Ver [`Auth/README.md`](../Auth/README.md).

---

## Parámetros de `dotnet test`

| Parámetro | Uso | Ejemplo |
|-----------|-----|---------|
| `--settings` | Config Playwright (headless, browser) | `--settings .runsettings` |
| `--filter "Name=..."` | Un test por nombre de método | `--filter "Name=TC_L01_BuenosAires_DebeOfrecerAmbasJurisdicciones"` |
| `--filter "TestCategory=..."` | Suite por categoría NUnit | `--filter "TestCategory=CABA-Bug"` |
| `--filter "FullyQualifiedName~..."` | Subcadena en nombre completo | `--filter "FullyQualifiedName~Locale_L01"` |

Los tests de LinkedIn tienen `[Explicit]`. Usar siempre `--settings .runsettings`.

---

## Todos los tests

```powershell
dotnet test --settings .runsettings
```

Para LinkedIn en vivo se recomienda **un caso a la vez** (ver abajo).

---

## Un caso de prueba (recomendado)

Patrón:

```powershell
dotnet test --filter "Name=<MetodoCSharp>" --settings .runsettings
```

### CABA-Bug (`TestCategory=CABA-Bug`)

| TC | Comando |
|----|---------|
| TC-P01 nombre | `dotnet test --filter "Name=TC_P01_NombreCompleto_DebeSugerirCABA" --settings .runsettings` |
| TC-P01 sigla | `dotnet test --filter "Name=TC_P01_Sigla_CABA_DebeMapearACABA" --settings .runsettings` |
| TC-P02 | `dotnet test --filter "Name=TC_P02_Palermo_DebeAsociarseACABA" --settings .runsettings` |
| TC-L01 | `dotnet test --filter "Name=TC_L01_BuenosAires_DebeOfrecerAmbasJurisdicciones" --settings .runsettings` |
| TC-L02 | `dotnet test --filter "Name=TC_L02_VillaRiachuelo_DebeAsociarseACABA" --settings .runsettings` |
| TC-L04 | `dotnet test --filter "Name=TC_L04_Comuna9_DebeAsociarseACABA" --settings .runsettings` |

### PBA-Regression (`TestCategory=PBA-Regression`)

| TC | Comando |
|----|---------|
| TC-PBA01 | `dotnet test --filter "Name=TC_PBA01_MarDelPlata_EsProvincia" --settings .runsettings` |
| TC-PBA02 | `dotnet test --filter "Name=TC_PBA02_LaPlata_EsProvincia" --settings .runsettings` |
| TC-PBA03 | `dotnet test --filter "Name=TC_PBA03_PartidoGeneralPueyrredon_EsProvincia" --settings .runsettings` |
| TC-L03 | `dotnet test --filter "Name=TC_L03_Avellaneda_EsProvincia" --settings .runsettings` |
| TC-PBA04 | `dotnet test --filter "Name=TC_PBA04_Lanus_EsProvincia" --settings .runsettings` |

### Negative (`TestCategory=Negative`)

```powershell
dotnet test --filter "Name=TC_N01_CampoVacio_SinSugerencias" --settings .runsettings
dotnet test --filter "Name=TC_N02_SoloEspacios_SinSugerencias" --settings .runsettings
dotnet test --filter "Name=TC_N03_CaracteresEspeciales_SinUbicacionValida" --settings .runsettings
dotnet test --filter "Name=TC_N04_InyeccionScript_NoEjecuta" --settings .runsettings
dotnet test --filter "Name=TC_N05_CadenaLarga_SinCrash" --settings .runsettings
dotnet test --filter "Name=TC_N06_Gibberish_SinSugerencias" --settings .runsettings
```

### Save-Validation / Save-Persistence

```powershell
dotnet test --filter "Name=TC_V01_SaveConLocationVacio_RespetaObligatoriedad" --settings .runsettings
dotnet test --filter "Name=TC_P01_Save_NombreCompleto_VerificarPersistido" --settings .runsettings
```

### Locale-Matrix (`TestCategory=Locale-Matrix`)

Suite completa:

```powershell
dotnet test --filter "TestCategory=Locale-Matrix" --settings .runsettings
```

Un caso:

```powershell
dotnet test --filter "Name=Locale_L01_BuenosAires_esAR" --settings .runsettings
dotnet test --filter "Name=Locale_L01_BuenosAires_enUS" --settings .runsettings
dotnet test --filter "Name=Locale_P01_CABA_esAR" --settings .runsettings
dotnet test --filter "Name=Locale_P01_CABA_enUS" --settings .runsettings
dotnet test --filter "Name=Locale_PBA01_MarDelPlata_esAR" --settings .runsettings
dotnet test --filter "Name=Locale_PBA01_MarDelPlata_enUS" --settings .runsettings
```

---

## Suites por categoría

```powershell
dotnet test --filter "TestCategory=CABA-Bug" --settings .runsettings
dotnet test --filter "TestCategory=PBA-Regression" --settings .runsettings
dotnet test --filter "TestCategory=Negative" --settings .runsettings
dotnet test --filter "TestCategory=Save-Validation" --settings .runsettings
dotnet test --filter "TestCategory=Save-Persistence" --settings .runsettings
dotnet test --filter "TestCategory=Locale-Matrix" --settings .runsettings
```

---

## Utilidades

**Restaurar Location baseline** (`CABA, Argentina`):

```powershell
pwsh tools\restore-location.ps1
dotnet test --filter "Name=RestaurarLocationBaseline" --settings .runsettings
```

**Exploración / baseline del bug:**

```powershell
dotnet test --filter "Name=ExplorarCampoLocation" --settings .runsettings
```

**Ver trace de un test:**

```powershell
pwsh bin\Debug\net10.0\playwright.ps1 show-trace "TestResults\baseline\TC_L01_BuenosAires_DebeOfrecerAmbasJurisdicciones\trace.zip"
```

**Modo visible (no headless):** editar `.runsettings` → `<Headless>false</Headless>`, o:

```powershell
$env:HEADED=1; dotnet test --filter "Name=TC_L01_BuenosAires_DebeOfrecerAmbasJurisdicciones" --settings .runsettings
```

**Exportar requerimiento Word/PDF:**

```powershell
pwsh tools\export-requerimiento-argentina-api.ps1
```

Salida: `docs\requirements\Requerimiento = Argentina-API.docx` y `.pdf`

---

## Documentación relacionada

| Archivo | Contenido |
|---------|-----------|
| [`../README.md`](../README.md) | Comandos genéricos del proyecto |
| [`.cursor/skills/linkedin-caba-playwright/SKILL.md`](../.cursor/skills/linkedin-caba-playwright/SKILL.md) | Política un TC a la vez + catálogo |
| [`test-cases/INDEX.md`](test-cases/INDEX.md) | TC → método C# |
| [`bugs/caba-location-typeahead/README.md`](bugs/caba-location-typeahead/README.md) | Bug + suites CABA/PBA |
