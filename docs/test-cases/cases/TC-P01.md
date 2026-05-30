# TC-P01 — Direct search for Autonomous City of Buenos Aires

## Metadatos

| Campo | Valor |
|-------|-------|
| **ID** | TC-P01 |
| **Título** | Direct search for Autonomous City of Buenos Aires |
| **Clasificación** | Positivo |
| **Módulo** | User Profile — Location |
| **Referencia PDF** | Sección 1 — Positive Test Cases CABA |
| **Test data** | TD-01 (nombre completo), TD-02 (sigla CABA) |

## Automatización

| Variante | Método C# | Archivo |
|----------|-----------|---------|
| Nombre completo | `TC_P01_NombreCompleto_DebeSugerirCABA` | `CabaLocationBugTest.cs` |
| Sigla CABA | `TC_P01_Sigla_CABA_DebeMapearACABA` | `CabaLocationBugTest.cs` |

**Categoría:** `CABA-Bug`

## Precondiciones

- Sesión válida (`Auth/state.json`).
- Formulario Edit experience abierto.

## Pasos (nombre completo)

1. Ir al campo Location.
2. Escribir `Ciudad Autónoma de Buenos Aires`.
3. Observar sugerencias del typeahead.

## Resultado esperado

Sugerencia canónica **`Autonomous City of Buenos Aires, Argentina`**, independiente de Province of Buenos Aires.

## Resultado actual — nombre completo

| Campo | Valor |
|-------|-------|
| **Última ejecución** | 2026-05-30 |
| **Observado** | 1 sugerencia: `Ciudad Autónoma de Buenos Aires` (eco/texto libre; sin entidad canónica en inglés) |
| **Estado** | ❌ FAIL |
| **Evidencia** | `TestResults/baseline/TC_P01_NombreCompleto_DebeSugerirCABA/` |

## Resultado actual — sigla CABA

| Campo | Valor |
|-------|-------|
| **Estado** | ⬜ Pendiente ejecución en esta sesión |
| **Evidencia** | — |

## Variante Save

Ver [TC-P01-Save.md](TC-P01-Save.md).

## Notas

- Assertion busca fragmento `Autonomous City of Buenos Aires` (`EtiquetaCaba`).
- Tras ejecutar: consultar al usuario antes del siguiente TC.
