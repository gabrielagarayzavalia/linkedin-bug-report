# Auth — Reutilización de sesión (sin automatizar el login)

Los tests de LinkedIn reutilizan tu sesión usando el **storageState** de Playwright
(cookies + `localStorage`), así no hace falta automatizar el login (ni manejar 2FA/captcha).

## Cómo capturar tu sesión

1. Compilá el proyecto (una vez):

```bash
dotnet build
```

2. Ejecutá el script de captura:

```powershell
pwsh Auth/capture-session.ps1
```

3. Se abre un navegador. **Logueate normalmente** en LinkedIn.
4. Cuando ya estés adentro, **cerrá la ventana** del navegador.
5. Se genera `Auth/state.json` con tu sesión.

> `Auth/state.json` está en `.gitignore`: **nunca** se commitea porque contiene tu sesión.

## Cómo lo usan los tests

`LinkedInTestBase` carga automáticamente `Auth/state.json` si existe
(ver `Tests/LinkedIn/LinkedInTestBase.cs`). Si no existe, te avisa por consola y
los tests verán LinkedIn como usuario deslogueado.

## Renovar la sesión

Las cookies de LinkedIn caducan. Si los tests empiezan a ver la pantalla de login,
volvé a ejecutar `pwsh Auth/capture-session.ps1`.
