using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
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

    // ---------------------------------------------------------------------
    // Helpers de dominio: formulario de experiencia y campo "Location".
    // ---------------------------------------------------------------------

    /// <summary>Formulario de experiencia donde se reproduce el bug de ubicación.</summary>
    protected const string PositionFormUrl =
        "https://www.linkedin.com/in/gabriela-garayzavalia/edit/forms/position/1864390597/";

    /// <summary>
    /// Etiquetas (en inglés) tal cual las muestra el typeahead de LinkedIn.
    /// OJO: la Provincia aparece como "Buenos Aires Province", NO "Province of Buenos Aires".
    /// CABA hoy no existe como entidad; la etiqueta esperada sigue la convención de LinkedIn.
    /// </summary>
    protected const string EtiquetaCaba = "Autonomous City of Buenos Aires";
    protected const string EtiquetaPba = "Buenos Aires Province";

    /// <summary>
    /// Abre el formulario de experiencia y devuelve el locator del campo "Location".
    /// </summary>
    protected async Task<ILocator> AbrirFormularioLocationAsync()
    {
        await Page.GotoAsync(PositionFormUrl, new PageGotoOptions
        {
            WaitUntil = WaitUntilState.DOMContentLoaded,
            Timeout = 45000,
        });

        // El formulario carga via JS; esperamos un momento (el feed nunca llega a NetworkIdle).
        await Page.WaitForTimeoutAsync(4000);

        var field = Page
            .GetByLabel(new Regex("location|ubicaci[oó]n|city|ciudad", RegexOptions.IgnoreCase))
            .First;
        await field.ScrollIntoViewIfNeededAsync();
        return field;
    }

    /// <summary>
    /// Escribe <paramref name="query"/> en el campo de ubicación, espera la respuesta
    /// del typeahead, captura un screenshot y devuelve el texto de cada sugerencia.
    /// </summary>
    protected async Task<IReadOnlyList<string>> ObtenerSugerenciasAsync(
        ILocator field, string query, string etiqueta)
    {
        await field.ClickAsync();
        await field.FillAsync(string.Empty);
        await field.PressSequentiallyAsync(query, new LocatorPressSequentiallyOptions { Delay = 80 });

        // Damos tiempo a que llegue la respuesta de la API de typeahead.
        await Page.WaitForTimeoutAsync(2500);
        await CapturarAsync(etiqueta);

        var opciones = await ResolverOpcionesAsync(field);
        var total = await opciones.CountAsync();
        var sugerencias = new List<string>(total);

        TestContext.Progress.WriteLine($"[SUGERENCIAS] '{query}' -> {total} resultados:");
        for (var i = 0; i < total; i++)
        {
            var texto = (await opciones.Nth(i).InnerTextAsync()).Replace("\n", " ").Trim();
            sugerencias.Add(texto);
            TestContext.Progress.WriteLine($"    - {texto}");
        }

        return sugerencias;
    }

    /// <summary>
    /// Acota las opciones al listbox del typeahead de ubicación, evitando el ruido de
    /// los &lt;select&gt; nativos del formulario (meses, años, tipo de empleo, idiomas).
    /// </summary>
    private async Task<ILocator> ResolverOpcionesAsync(ILocator field)
    {
        // 1) Preferimos el listbox que el combobox referencia explícitamente.
        var listId = await field.GetAttributeAsync("aria-owns")
                     ?? await field.GetAttributeAsync("aria-controls");
        if (!string.IsNullOrWhiteSpace(listId))
        {
            var firstId = listId.Split(' ', StringSplitOptions.RemoveEmptyEntries)[0];
            var byId = Page.Locator($"[id='{firstId}'] [role='option']");
            if (await byId.CountAsync() > 0)
            {
                return byId;
            }
        }

        // 2) Fallback: listbox ARIA visible (los <select> nativos no generan [role=listbox]).
        return Page.Locator("[role='listbox']:visible [role='option']");
    }

    /// <summary>True si alguna sugerencia contiene TODOS los fragmentos indicados (case-insensitive).</summary>
    protected static bool AlgunaContiene(IEnumerable<string> sugerencias, params string[] fragmentos) =>
        sugerencias.Any(s => fragmentos.All(f => s.Contains(f, StringComparison.OrdinalIgnoreCase)));

    /// <summary>Une las sugerencias en una sola línea para los mensajes de error.</summary>
    protected static string Evidencia(IEnumerable<string> sugerencias)
    {
        var lista = sugerencias.ToList();
        return lista.Count == 0 ? "(sin sugerencias)" : string.Join(" | ", lista);
    }

    /// <summary>Botón Save del modal "Edit experience".</summary>
    protected ILocator BotonSave =>
        Page.GetByRole(AriaRole.Button, new() { Name = "Save" })
            .Or(Page.GetByRole(AriaRole.Button, new() { Name = "Guardar" }));

    /// <summary>Encabezado del modal de edición de experiencia (indica que sigue abierto).</summary>
    protected ILocator ModalEditExperience =>
        Page.GetByRole(AriaRole.Heading, new() { Name = "Edit experience" })
            .Or(Page.GetByRole(AriaRole.Heading, new() { Name = "Editar experiencia" }));

    /// <summary>
    /// Indica si LinkedIn marca el campo Location como obligatorio (* en label, required o aria-required).
    /// </summary>
    protected static async Task<bool> LocationEsRequeridoAsync(ILocator field)
    {
        var ariaRequired = await field.GetAttributeAsync("aria-required");
        if (string.Equals(ariaRequired, "true", StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        var htmlRequired = await field.GetAttributeAsync("required");
        if (htmlRequired != null)
        {
            return true;
        }

        return await field.EvaluateAsync<bool>(
            """
            el => {
                const id = el.id;
                let label = id ? document.querySelector(`label[for="${CSS.escape(id)}"]`) : null;
                if (!label) label = el.closest('label');
                if (!label) {
                    const labelledBy = el.getAttribute('aria-labelledby');
                    if (labelledBy) {
                        label = document.getElementById(labelledBy.split(' ')[0]);
                    }
                }
                const text = label?.textContent ?? '';
                return text.includes('*');
            }
            """);
    }

    /// <summary>Deja el campo Location vacío (input + valor seleccionado visible).</summary>
    protected static async Task LimpiarLocationAsync(ILocator field)
    {
        await field.ClickAsync();
        await field.PressAsync("Control+A");
        await field.PressAsync("Backspace");
        await field.FillAsync(string.Empty);
        await field.PressAsync("Backspace");
    }

    /// <summary>Lee el valor del input Location.</summary>
    protected static Task<string> LeerLocationAsync(ILocator field) => field.InputValueAsync();

    /// <summary>
    /// Lee Location completo: input + texto visible en el contenedor del campo
    /// (LinkedIn a veces muestra la ubicación persistida fuera del input).
    /// </summary>
    protected static async Task<string> LeerLocationCompletoAsync(ILocator field)
    {
        var input = (await field.InputValueAsync()).Trim();
        if (!string.IsNullOrWhiteSpace(input))
        {
            return input;
        }

        var display = await field.EvaluateAsync<string>(
            """
            el => {
                const root = el.closest('.artdeco-form-element, fieldset, [class*="location"]')
                          || el.parentElement?.parentElement;
                if (!root) return '';
                const clone = root.cloneNode(true);
                clone.querySelectorAll('input, button, select, textarea, label').forEach(n => n.remove());
                return (clone.textContent || '').replace(/\s+/g, ' ').trim();
            }
            """);

        return display.Trim();
    }

    /// <summary>
    /// Pulsa Save y devuelve si el guardado parece haberse completado (modal cerrado).
    /// </summary>
    protected async Task<bool> IntentarGuardarAsync()
    {
        await BotonSave.ScrollIntoViewIfNeededAsync();
        await BotonSave.ClickAsync();
        await Page.WaitForTimeoutAsync(4000);
        return !await ModalSigueAbiertoAsync();
    }

    /// <summary>True si el modal "Edit experience" sigue visible.</summary>
    protected async Task<bool> ModalSigueAbiertoAsync() =>
        await ModalEditExperience.IsVisibleAsync();

    /// <summary>
    /// True si hay mensaje de validación visible cerca del campo Location
    /// (error inline, aria-invalid o texto de campo requerido).
    /// </summary>
    protected async Task<bool> HayErrorValidacionLocationAsync(ILocator field)
    {
        if (string.Equals(await field.GetAttributeAsync("aria-invalid"), "true", StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        var errorCerca = Page.Locator(
            "[role='alert'], .artdeco-inline-feedback--error, [data-test-inline-feedback='error']");
        if (await errorCerca.CountAsync() > 0 && await errorCerca.First.IsVisibleAsync())
        {
            return true;
        }

        var textoError = Page.Locator("text=/required|obligatorio|please enter|please select|campo requerido/i");
        return await textoError.CountAsync() > 0 && await textoError.First.IsVisibleAsync();
    }

    /// <summary>Restaura Location y guarda, para revertir un Save que alteró el perfil.</summary>
    protected async Task RestaurarLocationAsync(ILocator field, string valorOriginal)
    {
        if (string.IsNullOrWhiteSpace(valorOriginal))
        {
            await LimpiarLocationAsync(field);
            await IntentarGuardarAsync();
            return;
        }

        await field.ClickAsync();
        await field.FillAsync(string.Empty);
        await field.PressSequentiallyAsync(valorOriginal, new LocatorPressSequentiallyOptions { Delay = 60 });
        await Page.WaitForTimeoutAsync(2000);

        var opciones = await ResolverOpcionesAsync(field);
        if (await opciones.CountAsync() > 0)
        {
            await opciones.First.ClickAsync();
        }

        await IntentarGuardarAsync();
    }
}
