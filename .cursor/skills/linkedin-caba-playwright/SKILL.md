---
name: linkedin-caba-playwright
description: >-
  Ejecutar tests de automatización Playwright+C# del bug LinkedIn CABA vs PBA,
  un test case a la vez, con sesión reutilizada, baseline y política de consulta
  antes de reportar bugs. Usar cuando el usuario pida correr, revisar o ampliar
  casos TC-P*, TC-L*, TC-PBA*, TC-N*, TC-V* en cabaVsPBA.
---

# LinkedIn CABA vs PBA — Automatización Playwright

Proyecto: `cabaVsPBA` (C#, NUnit, Playwright .NET).

## Antes de ejecutar

1. `dotnet build`
2. Sesión: `Auth/state.json` debe existir. Si no:
   ```powershell
   dotnet test --filter "Name=CapturarSesion"
   ```
3. Navegadores: `pwsh bin/Debug/net10.0/playwright.ps1 install` (si hace falta)
4. Config: `--settings .runsettings`

## Regla de ejecución: un test case a la vez

**Siempre** filtrar por nombre del método (un solo TC por corrida):

```powershell
cd C:\Users\gabri\projects\linkedin\cabaVsPBA
dotnet test --filter "Name=<NombreDelMetodo>" --settings .runsettings
```

No correr suites completas salvo que el usuario lo pida explícitamente.

## Catálogo de tests (TC → método → expectativa)

| TC | Método | Categoría | Expectativa hoy |
|----|--------|-----------|-----------------|
| TC-P01 | `TC_P01_NombreCompleto_DebeSugerirCABA` | CABA-Bug | FAIL (bug documentado) |
| TC-P01 | `TC_P01_Sigla_CABA_DebeMapearACABA` | CABA-Bug | FAIL |
| TC-P02 | `TC_P02_Palermo_DebeAsociarseACABA` | CABA-Bug | FAIL |
| TC-L01 | `TC_L01_BuenosAires_DebeOfrecerAmbasJurisdicciones` | CABA-Bug | FAIL |
| TC-L02 | `TC_L02_VillaRiachuelo_DebeAsociarseACABA` | CABA-Bug | FAIL |
| TC-L04 | `TC_L04_Comuna9_DebeAsociarseACABA` | CABA-Bug | FAIL |
| TC-PBA01 | `TC_PBA01_MarDelPlata_EsProvincia` | PBA-Regression | PASS |
| TC-PBA02 | `TC_PBA02_LaPlata_EsProvincia` | PBA-Regression | PASS |
| TC-PBA03 | `TC_PBA03_PartidoGeneralPueyrredon_EsProvincia` | PBA-Regression | PASS |
| TC-L03 | `TC_L03_Avellaneda_EsProvincia` | PBA-Regression | PASS |
| TC-PBA04 | `TC_PBA04_Lanus_EsProvincia` | PBA-Regression | PASS |
| TC-N01..N06 | `TC_N01_*` … `TC_N06_*` | Negative | Consultar si falla |
| TC-V01 | `TC_V01_SaveConLocationVacio_RespetaObligatoriedad` | Save-Validation | PASS (Location no requerido) |
| TC-P01-Save | `TC_P01_Save_NombreCompleto_VerificarPersistido` | Save-Persistence | Consultar tras ejecutar |

Documentación QA: `docs/TestCases_TestData_LinkedIn_CABA.pdf`, `docs/test-cases/INDEX.md`.

## Documentación por TC (obligatorio)

- **Índice:** `docs/test-cases/INDEX.md` → TC → archivo `.md` → método C#.
- **Plantilla:** `docs/test-cases/_TEMPLATE.md` (base para generar templates).
- **Por caso:** `docs/test-cases/cases/TC-*.md`.

Antes de ejecutar un TC: **leer** su `.md`. Después: **actualizar** Resultado actual, Estado y Evidencia. Si no existe el `.md`, crearlo desde la plantilla y registrar en INDEX.

## Políticas obligatorias

### Assertion fallida → consultar, no alucinar

Si un test **falla** (o el resultado sorprende):

1. **NO** reescribir assertions ni concluir solo que "la expectativa era estricta".
2. **NO** reportar bug ni commitear cambios de interpretación sin confirmación del usuario.
3. Presentar: dato de entrada, sugerencias/resultado observado, evidencia (PNG + trace), dos hipótesis (bug vs test), **preguntar y esperar respuesta**.
4. **NO** ejecutar el siguiente test case hasta que el usuario confirme cómo seguir.

### Tras cada test case (pase o falle)

Siempre **detenerse y preguntar al usuario** antes de continuar con otro TC, para evitar alucinaciones.

### Perfil LinkedIn (tests que mutan Location)

Tests que heredan `LinkedInMutableLocationTestBase`:

- SetUp captura `LocationAlInicioDelTest`.
- Tras Save exitoso: TearDown verifica valor **persistido** antes de restaurar.
- TearDown **siempre** restaura al valor al inicio del test.

### Git

- Commit local automático tras cambios significativos (mensaje en español, enfoque en el porqué).
- **Nunca** `git push` salvo pedido explícito.
- **Nunca** commitear `Auth/state.json` ni secretos.

### Proxy / internet

- Preferir Playwright (no toca proxy del sistema).
- Hook usuario: `~/.cursor/hooks/reset-proxy.ps1` en evento `stop`.
- Manual: `pwsh tools/reset-proxy.ps1`.

## Evidencia (baseline)

Tras cada test:

```
TestResults/baseline/<NombreDelMetodo>/
├── 01-*.png … XX-final.png
└── trace.zip
```

Ver trace:

```powershell
pwsh bin/Debug/net10.0/playwright.ps1 show-trace "TestResults\baseline\<NombreDelMetodo>\trace.zip"
```

Evidencia versionada del bug: `docs/bugs/caba-location-typeahead/evidence/`.

## Datos de dominio LinkedIn

- Provincia en typeahead: **`Buenos Aires Province`** (no "Province of Buenos Aires").
- CABA esperada: **`Autonomous City of Buenos Aires`** (hoy no existe como entidad).
- Campo UI: **Location** (formulario experiencia).
- URL base del formulario: ver `LinkedInTestBase.PositionFormUrl`.

## Flujo recomendado (test case by test case)

1. Confirmar con el usuario **qué TC** ejecutar (o seguir orden del PDF).
2. Ejecutar **solo ese** test con `--filter "Name=..."`.
3. Revisar log `[SUGERENCIAS]`, screenshots y trace.
4. **Detenerse y preguntar al usuario** antes de continuar (sin repasar todo el proyecto).
5. **Alcance mínimo por TC:** solo ejecutar test → actualizar `cases/TC-*.md` del caso → resultado breve → preguntar. **No** revisar el repo entero, skill, backlog ni otros archivos salvo que el usuario lo pida.
6. Si pasa/falla según expectativa y el usuario confirma: registrar; commit **solo** del `.md` del caso (si hubo ejecución).
7. Si falla inesperado: presentar evidencia + dos hipótesis; **esperar respuesta** antes de actuar.
8. Pasar al siguiente TC **solo** cuando el usuario lo indique.

## Pendientes conocidos (consultar con usuario)

- **TC-N01**, **TC-N05**: fallaron en corrida inicial; requieren confirmación de hipótesis antes de ajustar assertions o reportar bug.
- **Reporte de riesgos R01–R15**: no iniciar hasta que el usuario lo pida.
- **Post casos establecidos:** recordar `docs/todo/BACKLOG.md` — set de datos, locale/idioma (EN y ES), **restaurar perfil**.
