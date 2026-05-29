using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Playwright;
using NUnit.Framework;

namespace cabaVsPBA.Tests.LinkedIn;

/// <summary>
/// Captura manual de la sesion de LinkedIn (reemplaza al login automatizado).
///
/// Abre un navegador VISIBLE, espera a que te loguees a mano y, en cuanto detecta
/// la cookie de sesion (<c>li_at</c>), guarda el estado en <c>Auth/state.json</c>
/// llamando a <c>StorageStateAsync</c> directamente (mas robusto que codegen).
///
/// COMO EJECUTARLO:
///   dotnet test --filter "Name=CapturarSesion"
///
/// Es [Explicit]: NO corre con "dotnet test" normal, solo cuando lo filtras.
/// </summary>
[TestFixture]
[Explicit("Captura manual de sesion: abre un navegador visible y espera el login.")]
public class CaptureSessionTest
{
    private const int TimeoutMinutos = 5;

    [Test]
    public async Task CapturarSesion()
    {
        Directory.CreateDirectory(LinkedInPaths.AuthDir);

        using var playwright = await Playwright.CreateAsync();
        await using var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
        {
            Headless = false,
        });

        var context = await browser.NewContextAsync(new BrowserNewContextOptions
        {
            ViewportSize = new ViewportSize { Width = 1920, Height = 1080 },
            Locale = "es-AR",
            TimezoneId = "America/Argentina/Buenos_Aires",
        });

        var page = await context.NewPageAsync();
        await page.GotoAsync("https://www.linkedin.com/login");

        TestContext.Progress.WriteLine(
            $"Logueate en la ventana del navegador. Esperando hasta {TimeoutMinutos} min a detectar la sesion...");

        var deadline = DateTime.UtcNow.AddMinutes(TimeoutMinutos);
        var logueado = false;

        while (DateTime.UtcNow < deadline)
        {
            var cookies = await context.CookiesAsync();
            if (cookies.Any(c => c.Name == "li_at" && !string.IsNullOrEmpty(c.Value)))
            {
                logueado = true;
                break;
            }

            await Task.Delay(2000);
        }

        if (!logueado)
        {
            Assert.Fail(
                $"No se detecto la cookie de sesion 'li_at' en {TimeoutMinutos} min. " +
                "Volve a ejecutar e iniciar sesion mas rapido.");
            return;
        }

        // Pequena espera para que terminen de setearse cookies/localStorage post-login.
        await Task.Delay(3000);

        await context.StorageStateAsync(new BrowserContextStorageStateOptions
        {
            Path = LinkedInPaths.StorageStatePath,
        });

        await context.CloseAsync();

        Assert.That(File.Exists(LinkedInPaths.StorageStatePath), Is.True,
            "No se genero state.json.");
        TestContext.Progress.WriteLine($"Sesion guardada en: {LinkedInPaths.StorageStatePath}");
    }
}
