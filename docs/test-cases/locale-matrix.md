# Matriz locale × idioma — Location typeahead

Alcance: **es-AR** y **en-US** (locale Playwright). Última corrida: 2026-06-01.

Tests: `Tests/LinkedIn/LocaleLocationTest.cs` — categoría `Locale-Matrix`, `[Explicit]`.

```powershell
dotnet test --filter "TestCategory=Locale-Matrix" --settings .runsettings
```

## Matriz mínima (TD × locale)

| Query (TD) | es-AR | en-US | ¿Bug distinto por locale? |
|------------|-------|-------|---------------------------|
| Buenos Aires (TD-05 / L01) | ❌ FAIL — solo PBA | ❌ FAIL — solo PBA | No — mismo bug en ambos locales |
| Ciudad Autónoma de Buenos Aires (TD-01 / P01) | ✅ PASS — eco ES | ❌ FAIL — eco ES, sin EN canónico | Parcial — en-US no cambia sugerencia |
| Mar del Plata (TD-11 / PBA01) | ✅ PASS | ✅ PASS | No — PBA OK en ambos |

## Observaciones

- **L01:** sin `Autonomous City of Buenos Aires` ni `Ciudad Autónoma de Buenos Aires` en top 10; solo PBA y GBA.
- **P01 en-US:** typeahead devuelve `Ciudad Autónoma de Buenos Aires` (texto libre ES), no entidad EN — bug de datos, no de locale browser.
- **PBA01:** regresión estable en es-AR y en-US.

## Etiquetas esperadas

| Locale | CABA | PBA |
|--------|------|-----|
| en-US | Autonomous City of Buenos Aires | Buenos Aires Province |
| es-AR | Ciudad Autónoma de Buenos Aires (ideal) | Buenos Aires Province |

## Notas

- Perfil con **Primary language: English** puede requerir sesión separada para UI 100% EN.
- Ejecutar un test: `dotnet test --filter "Name=Locale_L01_BuenosAires_esAR" --settings .runsettings`

Ver [`docs/test-data/README.md`](../test-data/README.md).
