# {{TC_ID}} — {{TITULO}}

> Plantilla para casos de prueba. Copiar a `cases/{{TC_ID}}.md` y completar.
> Índice: `docs/test-cases/INDEX.md`

## Metadatos

| Campo | Valor |
|-------|-------|
| **ID** | {{TC_ID}} |
| **Título** | {{TITULO}} |
| **Clasificación** | Positivo / Negativo / Límite / Regresión / Save-Persistence |
| **Módulo** | User Profile — Location (Edit experience) |
| **Referencia PDF** | `docs/TestCases_TestData_LinkedIn_CABA.pdf` — sección {{SECCION_PDF}} |
| **Test data** | {{TD_ID}} (si aplica) |

## Automatización

| Campo | Valor |
|-------|-------|
| **Método C#** | `{{NOMBRE_METODO}}` |
| **Archivo test** | `Tests/LinkedIn/{{ARCHIVO_TEST}}.cs` |
| **Categoría NUnit** | `{{CATEGORIA}}` |
| **Comando** | `dotnet test --filter "Name={{NOMBRE_METODO}}" --settings .runsettings` |

## Precondiciones

- Usuario logueado (`Auth/state.json` válido).
- Formulario de experiencia accesible: `LinkedInTestBase.PositionFormUrl`.
- {{PRECONDICIONES_EXTRA}}

## Pasos

1. {{PASO_1}}
2. {{PASO_2}}
3. {{PASO_3}}

## Resultado esperado

{{RESULTADO_ESPERADO}}

## Resultado actual

| Campo | Valor |
|-------|-------|
| **Última ejecución** | {{FECHA_UTC}} |
| **Observado** | {{RESULTADO_ACTUAL}} |
| **Estado** | ⬜ Pendiente / ✅ PASS / ❌ FAIL / ⚠️ Consultar |
| **Evidencia** | `TestResults/baseline/{{NOMBRE_METODO}}/` |

## Reglas de negocio / validación

- **Location requerido:** {{SI_NO_NA}}
- **Save incluido:** {{SI_NO}}
- **Restaurar perfil tras test:** {{SI_NO}} (`LinkedInMutableLocationTestBase`)

## Notas

{{NOTAS}}

## Historial de ejecuciones

| Fecha | Resultado | Notas |
|-------|-----------|-------|
| | | |
