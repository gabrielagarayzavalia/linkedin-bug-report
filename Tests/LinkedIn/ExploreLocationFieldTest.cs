using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Playwright;
using NUnit.Framework;

namespace cabaVsPBA.Tests.LinkedIn;

/// <summary>
/// Test EXPLORATORIO del bug de ubicación (CABA) en el formulario de experiencia.
///
/// No afirma nada todavía: navega al formulario, ubica el campo "Location",
/// tipea variantes de Buenos Aires / CABA y captura las sugerencias que ofrece
/// LinkedIn, para construir el baseline (screenshots + trace) y entender el bug.
///
///   dotnet test --filter "Name=ExplorarCampoLocation" --settings .runsettings
/// </summary>
[TestFixture]
[Explicit("Exploratorio: requiere sesión válida (Auth/state.json) y red.")]
public class ExploreLocationFieldTest : LinkedInTestBase
{
    private const string FormUrl =
        "https://www.linkedin.com/in/gabriela-garayzavalia/edit/forms/position/1864390597/";

    [Test]
    public async Task ExplorarCampoLocation()
    {
        await Page.GotoAsync(FormUrl, new PageGotoOptions
        {
            WaitUntil = WaitUntilState.DOMContentLoaded,
            Timeout = 45000,
        });

        await Page.WaitForTimeoutAsync(5000);
        await CapturarAsync("formulario-cargado");

        // El campo de ubicación se rotula "Location" (puede variar a Ubicación/City).
        var locationField = Page.GetByLabel(new Regex("location|ubicaci[oó]n|city|ciudad", RegexOptions.IgnoreCase));
        var count = await locationField.CountAsync();
        TestContext.Progress.WriteLine($"[EXPLORACION] Campos que matchean 'location': {count}");

        if (count == 0)
        {
            TestContext.Progress.WriteLine(
                "[EXPLORACION] No se encontró el campo por label. Reviso baseline 'formulario-cargado'.");
            return;
        }

        var field = locationField.First;
        await field.ScrollIntoViewIfNeededAsync();
        await CapturarAsync("campo-location");

        await ProbarConsultaAsync(field, "Buenos Aires", "sugerencias-buenos-aires");
        await ProbarConsultaAsync(field, "Ciudad Autónoma de Buenos Aires", "sugerencias-caba");
        await ProbarConsultaAsync(field, "CABA", "sugerencias-sigla-caba");
    }

    /// <summary>Limpia el campo, escribe la consulta y captura las sugerencias.</summary>
    private async Task ProbarConsultaAsync(ILocator field, string query, string etiqueta)
    {
        await field.ClickAsync();
        await field.FillAsync(string.Empty);
        await field.PressSequentiallyAsync(query, new LocatorPressSequentiallyOptions { Delay = 80 });

        // Damos tiempo a que llegue la respuesta de la API de typeahead.
        await Page.WaitForTimeoutAsync(2500);
        await CapturarAsync(etiqueta);

        // Logueamos el texto de las sugerencias visibles para el reporte.
        var opciones = Page.GetByRole(AriaRole.Option);
        var n = await opciones.CountAsync();
        TestContext.Progress.WriteLine($"[EXPLORACION] '{query}' -> {n} sugerencias:");
        for (var i = 0; i < n; i++)
        {
            var texto = (await opciones.Nth(i).InnerTextAsync()).Replace("\n", " ").Trim();
            TestContext.Progress.WriteLine($"    - {texto}");
        }
    }
}
