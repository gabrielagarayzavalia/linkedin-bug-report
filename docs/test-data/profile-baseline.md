# Baseline del perfil de prueba — Location

Valores de referencia para restaurar el formulario de experiencia tras tests Save-Persistence.

| Campo | Valor |
|-------|-------|
| **Position ID** | `1864390597` |
| **URL** | `https://www.linkedin.com/in/gabriela-garayzavalia/edit/forms/position/1864390597/` |
| **Location baseline** | `CABA, Argentina` |
| **Constante C#** | `LinkedInTestBase.ProfileBaselineLocation` |

## Restauración manual

```powershell
pwsh tools/restore-location.ps1
```

O directamente:

```powershell
dotnet test --filter "Name=RestaurarLocationBaseline" --settings .runsettings
```

## Notas

- TC-P01-Save alteró el perfil a `Ciudad Autónoma de Buenos Aires` (texto libre). Restaurar antes de nuevas corridas Save.
- Los tests que heredan `LinkedInMutableLocationTestBase` restauran al valor capturado al inicio de cada test; este baseline aplica cuando el perfil quedó fuera de sync.

Ver también: [`docs/todo/BACKLOG.md`](../todo/BACKLOG.md) §3, [`docs/test-cases/cases/TC-P01-Save.md`](../test-cases/cases/TC-P01-Save.md).
