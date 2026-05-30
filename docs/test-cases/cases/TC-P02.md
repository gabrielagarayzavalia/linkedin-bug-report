# TC-P02 — Palermo debe asociarse a CABA

## Metadatos

| Campo | Valor |
|-------|-------|
| **ID** | TC-P02 |
| **Título** | Search for a neighborhood belonging to CABA |
| **Clasificación** | Positivo |
| **Módulo** | User Profile — Location |
| **Referencia PDF** | Sección 1 — Positive Test Cases CABA |
| **Test data** | TD-03 |

## Automatización

| Campo | Valor |
|-------|-------|
| **Método C#** | `TC_P02_Palermo_DebeAsociarseACABA` |
| **Archivo test** | `Tests/LinkedIn/CabaLocationBugTest.cs` |
| **Categoría NUnit** | `CABA-Bug` |
| **Comando** | `dotnet test --filter "Name=TC_P02_Palermo_DebeAsociarseACABA" --settings .runsettings` |

## Pasos

1. Ir al campo Location.
2. Escribir `Palermo`.
3. Observar sugerencias del typeahead.

## Resultado esperado

`Palermo, Autonomous City of Buenos Aires, Argentina`

## Resultado actual

| Campo | Valor |
|-------|-------|
| **Última ejecución** | 2026-05-30 |
| **Observado** | 10 sugerencias; **ninguna** con CABA ni Argentina. Aparecen: Palermo, Sicily, Italy (varias), Greater Palermo Metropolitan Area, Palermo Huila Colombia, Palermo Maine EE.UU. |
| **PDF (documentado)** | `Palermo, Province of Buenos Aires, Argentina` |
| **Estado** | ❌ FAIL |
| **Evidencia** | `TestResults/baseline/TC_P02_Palermo_DebeAsociarseACABA/` |

## Notas

- Falla distinta al PDF: hoy no aparece Palermo argentino (ni CABA ni PBA) en las 10 primeras sugerencias.
- Bug confirmado por usuario (2026-05-30).
