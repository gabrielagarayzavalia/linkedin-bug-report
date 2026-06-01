# TC-P01-Save — Guardar Ciudad Autónoma de Buenos Aires y verificar persistencia

## Metadatos

| Campo | Valor |
|-------|-------|
| **ID** | TC-P01-Save |
| **Título** | Save + persistencia: Ciudad Autónoma de Buenos Aires |
| **Clasificación** | Save-Persistence |
| **Módulo** | User Profile — Location |
| **Referencia PDF** | TC-P01 / TD-01 + extensión Save |
| **Test data** | TD-01 |

## Automatización

| Campo | Valor |
|-------|-------|
| **Método C#** | `TC_P01_Save_NombreCompleto_VerificarPersistido` |
| **Archivo test** | `Tests/LinkedIn/CabaLocationSaveTest.cs` |
| **Categoría NUnit** | `Save-Persistence` |
| **Comando** | `dotnet test --filter "Name=TC_P01_Save_NombreCompleto_VerificarPersistido" --settings .runsettings` |

## Precondiciones

- Sesión válida.
- `LinkedInMutableLocationTestBase` captura Location al inicio del test.

## Pasos

1. Capturar Location al inicio (`location-al-inicio-del-test`).
2. Escribir `Ciudad Autónoma de Buenos Aires`.
3. Seleccionar 1.ª sugerencia del typeahead.
4. Click **Save**.
5. Reabrir formulario: verificar valor **persistido** (`valor-persistido-antes-de-restaurar`).
6. Restaurar Location al valor al inicio.

## Resultado esperado

- **Datos (PDF):** persiste entidad canónica `Autonomous City of Buenos Aires`.
- **Validación:** si Location es **requerido**, no debe guardar sin entidad canónica; si **opcional**, puede guardar pero datos deben normalizarse.

## Resultado actual

| Campo | Valor |
|-------|-------|
| **Última ejecución** | 2026-05-30 |
| **Observado** | Save **exitoso** (`guardadoExitoso=True`). Location **no requerido**. Persistido: `Ciudad Autónoma de Buenos Aires` (texto libre, **sin** `Autonomous City of Buenos Aires`) |
| **Estado** | ❌ FAIL (datos) — validación OK (opcional permitió guardar) |
| **Evidencia** | `TestResults/baseline/TC_P01_Save_NombreCompleto_VerificarPersistido/` |
| **Restauración** | ✅ TearDown restaura aunque falle assert de datos (2026-06-01). Perfil verificado en baseline `CABA, Argentina` |

## Historial de ejecuciones

| Fecha | Resultado | Notas |
|-------|-----------|-------|
| 2026-05-30 | ❌ FAIL datos | Persistió texto libre en español, no entidad canónica |
| 2026-06-01 | ❌ FAIL datos + ✅ restore | TearDown restaura perfil a baseline tras fallo de persistencia |

## Reglas de negocio

| Regla | Comportamiento esperado |
|-------|-------------------------|
| Requerido + Save sin canónica | ❌ Bug validación |
| Opcional + Save texto libre | Validación OK; ❌ bug datos si no normaliza a CABA |
| Persistido ≠ esperado | TearDown falla; consultar usuario |

## Notas

- Patrón reutilizable para TC-P02-Save, etc.
- Ver [save-persistence.md](../save-persistence.md).
