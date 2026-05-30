# Casos de prueba NEGATIVOS — Campo "Location" (LinkedIn)

> Complemento del documento `TestCases_TestData_LinkedIn_CABA.pdf`, que cubre
> casos Positivos y Límite. Estos negativos se agregan según el estándar de QA
> (datos inválidos, vacíos/nulos, caracteres especiales) y validan **robustez**.
> Automatizados en `Tests/LinkedIn/NegativeLocationTest.cs` (categoría `Negative`).

**Módulo:** User Profile — Location (formulario de Experiencia)
**Precondición:** Usuario logueado, editando el campo Location.
**Resultado esperado general:** la entrada inválida NO produce una ubicación válida
(ninguna sugerencia con "Argentina") y NO rompe la página (el campo sigue usable).

| ID | Título | Clasificación | Dato de entrada | Resultado esperado | Estado esperado |
|----|--------|---------------|-----------------|--------------------|-----------------|
| TC-N01 | Campo vacío | Negativo | `` (vacío) | Sin sugerencias de ubicación | ✅ PASA |
| TC-N02 | Solo espacios | Negativo | `"     "` | Sin sugerencias de ubicación | ✅ PASA |
| TC-N03 | Caracteres especiales | Negativo | `@#$%^&*()_+{}<>` | Sin ubicación válida; campo usable | ✅ PASA |
| TC-N04 | Inyección de script | Negativo (seguridad) | `<script>alert('xss')</script>` | Tratado como texto: sin diálogo/JS, sin ubicación válida | ✅ PASA |
| TC-N05 | Cadena extremadamente larga | Límite/Negativo | 300 × `a` | Sin ubicación válida; sin crash | ✅ PASA |
| TC-N06 | Cadena sin sentido | Negativo | `zzzqqqxywv123` | Sin sugerencias de ubicación | ✅ PASA |

## Validación Save vs. obligatoriedad (TC-V01)

Automatizado en `Tests/LinkedIn/LocationSaveValidationTest.cs` (categoría `Save-Validation`).

| ID | Título | Regla |
|----|--------|-------|
| TC-V01 | Save con Location vacío | Si el campo es **requerido** (* en label / `required` / `aria-required`) → Save **no** debe guardar. Si guarda = **bug**. |
| TC-V01 | Save con Location vacío | Si el campo **no** es requerido → Save **debe** permitir guardar. Si bloquea = **bug**. |

```bash
dotnet test --filter "TestCategory=Save-Validation" --settings .runsettings
```

> El test intenta **restaurar** el valor original de Location si el Save alteró el perfil.

### Ciclo de vida del perfil (cada test)

1. **SetUp**: captura `LocationAlInicioDelTest` (screenshot `location-al-inicio-del-test`).
2. **Tras Save exitoso**: **TearDown** reabre el formulario y verifica que el valor **persistido** coincide con lo guardado (`valor-persistido-antes-de-restaurar`).
3. **TearDown**: siempre restaura el valor del paso 1 y confirma que quedó aplicado.

Clase base: `LinkedInMutableLocationTestBase` (reutilizable en otros tests que muten Location).

## Ejecución

```bash
dotnet test --filter "TestCategory=Negative" --settings .runsettings
```

## Notas

- Si alguno de estos **fallara**, sería un hallazgo de robustez/seguridad
  (p. ej. TC-N04 ejecutando un diálogo = posible XSS, o entradas basura que
  devuelven ubicaciones espurias).
- Evidencia (screenshots + `trace.zip`) en `TestResults/baseline/<NombreDelTest>/`.
