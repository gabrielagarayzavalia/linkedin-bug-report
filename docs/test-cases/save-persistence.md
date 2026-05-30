# Tests Save + persistencia (TC-*-Save)

Complemento del PDF: además del typeahead, **Save** y **verificación del valor persistido**.

Patrón reutilizable: `CabaLocationSaveTest` + `LinkedInMutableLocationTestBase`.

## TC-P01-Save — Ciudad Autónoma de Buenos Aires

| Campo | Contenido |
|-------|-----------|
| **ID** | TC-P01-Save / TD-01 |
| **Método** | `TC_P01_Save_NombreCompleto_VerificarPersistido` |
| **Categoría** | `Save-Persistence` |
| **Pasos** | 1. Capturar Location al inicio<br>2. Escribir `Ciudad Autónoma de Buenos Aires`<br>3. Seleccionar 1.ª sugerencia<br>4. Save<br>5. Reabrir y verificar persistido<br>6. Restaurar valor al inicio |
| **Esperado (datos PDF)** | Persiste entidad canónica `Autonomous City of Buenos Aires` |
| **Regla req/opcional** | Si **requerido**: no debe guardar sin entidad canónica. Si **opcional**: puede guardar, pero datos deben normalizarse a CABA (bug si no). |

```bash
dotnet test --filter "Name=TC_P01_Save_NombreCompleto_VerificarPersistido" --settings .runsettings
```

## Relación con otros tests

| Test | Typeahead | Save | Persistencia |
|------|-----------|------|--------------|
| `TC_P01_NombreCompleto_DebeSugerirCABA` | ✅ | ❌ | ❌ |
| `TC_P01_Save_NombreCompleto_VerificarPersistido` | ✅ | ✅ | ✅ |
| `TC_V01_SaveConLocationVacio_RespetaObligatoriedad` | ❌ (vacío) | ✅ | ✅ |

## Política

Tras ejecutar: **consultar al usuario** antes del siguiente TC. Si falla: no alucinar; presentar evidencia y esperar confirmación.
