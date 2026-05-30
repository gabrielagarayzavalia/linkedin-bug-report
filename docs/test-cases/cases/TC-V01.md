# TC-V01 — Save con Location vacío (requerido vs opcional)

## Metadatos

| Campo | Valor |
|-------|-------|
| **ID** | TC-V01 |
| **Clasificación** | Save-Validation |
| **Módulo** | User Profile — Location |

## Automatización

| Campo | Valor |
|-------|-------|
| **Método C#** | `TC_V01_SaveConLocationVacio_RespetaObligatoriedad` |
| **Archivo test** | `Tests/LinkedIn/LocationSaveValidationTest.cs` |
| **Categoría NUnit** | `Save-Validation` |

## Pasos

1. Capturar Location al inicio.
2. Limpiar campo Location.
3. Save.
4. Verificar persistido y restaurar.

## Resultado esperado

- **Requerido:** Save bloqueado.
- **Opcional:** Save permitido con vacío.

## Resultado actual

| Campo | Valor |
|-------|-------|
| **Última ejecución** | 2026-05-30 |
| **Observado** | Location **no requerido**; Save guardó vacío; persistió `''`; restauró `CABA, Argentina` |
| **Estado** | ✅ PASS |
| **Evidencia** | `TestResults/baseline/TC_V01_SaveConLocationVacio_RespetaObligatoriedad/` |
