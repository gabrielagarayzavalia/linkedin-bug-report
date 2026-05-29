using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using NUnit.Framework;

namespace cabaVsPBA.Tests.LinkedIn;

/// <summary>
/// Clase base para los tests de LinkedIn.
///
/// - Reutiliza la sesion del usuario (cookies + localStorage) mediante el
///   "storageState" de Playwright, evitando automatizar el login.
///   Captura el estado con: <c>Auth/capture-session.ps1</c> (ver README de Auth).
/// - Graba un trace por test y permite tomar screenshots de baseline, que se
///   guardan en <c>TestResults/baseline/&lt;NombreDelTest&gt;/</c>.
/// </summary>
public abstract class LinkedInTestBase : PageTest
{
    /// <summary>Estado de sesion capturado (cookies de LinkedIn).</summary>
    protected static string StorageStatePath { get; } = LinkedInPaths.StorageStatePath;

    /// <summary>Carpeta donde se guardan los reportes de baseline.</summary>
    protected string BaselineDir { get; private set; } = string.Empty;

    private int _screenshotIndex;

    public override BrowserNewContextOptions ContextOptions()
    {
        var options = new BrowserNewContextOptions
        {
            ViewportSize = new ViewportSize { Width = 1920, Height = 1080 },
            Locale = "es-AR",
            TimezoneId = "America/Argentina/Buenos_Aires",
        };

        if (File.Exists(StorageStatePath))
        {
            options.StorageStatePath = StorageStatePath;
        }
        else
        {
            TestContext.Progress.WriteLine(
                $"[ADVERTENCIA] No se encontro el estado de sesion en '{StorageStatePath}'. " +
                "Ejecuta Auth/capture-session.ps1 para loguearte una vez y guardar la sesion. " +
                "Sin esto los tests veran LinkedIn como usuario deslogueado.");
        }

        return options;
    }

    [SetUp]
    public async Task IniciarBaselineAsync()
    {
        var testName = TestContext.CurrentContext.Test.Name;
        BaselineDir = Path.Combine(LinkedInPaths.BaselineDir, Sanitizar(testName));
        Directory.CreateDirectory(BaselineDir);
        _screenshotIndex = 0;

        await Context.Tracing.StartAsync(new TracingStartOptions
        {
            Title = testName,
            Screenshots = true,
            Snapshots = true,
            Sources = true,
        });
    }

    [TearDown]
    public async Task FinalizarBaselineAsync()
    {
        var tracePath = Path.Combine(BaselineDir, "trace.zip");
        await Context.Tracing.StopAsync(new TracingStopOptions { Path = tracePath });

        // Screenshot final del estado de la pagina (util para ver el error en el baseline).
        if (!Page.IsClosed)
        {
            await CapturarAsync("final");
        }

        TestContext.Progress.WriteLine($"[BASELINE] Reporte en: {BaselineDir}");
        TestContext.Progress.WriteLine($"[BASELINE] Ver trace con: pwsh bin/Debug/net10.0/playwright.ps1 show-trace \"{tracePath}\"");
    }

    /// <summary>
    /// Toma un screenshot de pagina completa y lo guarda en la carpeta de baseline,
    /// numerado y con una etiqueta descriptiva del paso.
    /// </summary>
    protected async Task CapturarAsync(string etiqueta)
    {
        _screenshotIndex++;
        var fileName = $"{_screenshotIndex:00}-{Sanitizar(etiqueta)}.png";
        var path = Path.Combine(BaselineDir, fileName);
        await Page.ScreenshotAsync(new PageScreenshotOptions { Path = path, FullPage = true });
        TestContext.Progress.WriteLine($"[BASELINE] {fileName}");
    }

    private static string Sanitizar(string value)
    {
        foreach (var c in Path.GetInvalidFileNameChars())
        {
            value = value.Replace(c, '_');
        }

        return value.Replace(' ', '_');
    }
}
