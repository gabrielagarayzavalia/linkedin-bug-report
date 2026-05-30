using System.Threading.Tasks;
using Microsoft.Playwright;
using NUnit.Framework;

namespace cabaVsPBA.Tests.LinkedIn;

/// <summary>
/// Regresión de la Provincia de Buenos Aires (PBA) en el campo "Location".
///
/// Estos casos HOY funcionan bien (las localidades/partidos de PBA se clasifican
/// correctamente). Sirven como red de seguridad: cuando se corrija el bug de CABA,
/// deben seguir en verde para garantizar que el fix no reasigna mal a PBA.
/// (docs/TestCases_TestData_LinkedIn_CABA.pdf)
///
/// Ejecutar solo este grupo:
///   dotnet test --filter "TestCategory=PBA-Regression" --settings .runsettings
///
/// Requiere sesión válida (Auth/state.json) y acceso a internet.
/// </summary>
[TestFixture]
[Category("PBA-Regression")]
[Explicit("Regresión contra LinkedIn en vivo: requiere sesión y red.")]
public class PbaLocationRegressionTest : LinkedInTestBase
{
    [Test]
    [Description("TC-PBA01 / TD-11: 'Mar del Plata' debe figurar bajo la Provincia de Buenos Aires.")]
    public async Task TC_PBA01_MarDelPlata_EsProvincia()
    {
        var field = await AbrirFormularioLocationAsync();
        var sugerencias = await ObtenerSugerenciasAsync(field, "Mar del Plata", "TC-PBA01-mar-del-plata");

        Assert.That(AlgunaContiene(sugerencias, "Mar del Plata", EtiquetaPba), Is.True,
            $"TC-PBA01: se esperaba 'Mar del Plata, {EtiquetaPba}, Argentina'. " +
            $"Sugerencias: {Evidencia(sugerencias)}");
    }

    [Test]
    [Description("TC-PBA02 / TD-12: 'La Plata' (capital de PBA) debe figurar bajo la Provincia.")]
    public async Task TC_PBA02_LaPlata_EsProvincia()
    {
        var field = await AbrirFormularioLocationAsync();
        var sugerencias = await ObtenerSugerenciasAsync(field, "La Plata", "TC-PBA02-la-plata");

        Assert.That(AlgunaContiene(sugerencias, "La Plata", EtiquetaPba), Is.True,
            $"TC-PBA02: se esperaba 'La Plata, {EtiquetaPba}, Argentina'. " +
            $"Sugerencias: {Evidencia(sugerencias)}");
    }

    [Test]
    [Description("TC-PBA03 / TD-13: 'Partido de General Pueyrredón' debe figurar bajo la Provincia.")]
    public async Task TC_PBA03_PartidoGeneralPueyrredon_EsProvincia()
    {
        var field = await AbrirFormularioLocationAsync();
        var sugerencias = await ObtenerSugerenciasAsync(
            field, "Partido de General Pueyrredón", "TC-PBA03-partido-pueyrredon");

        Assert.That(AlgunaContiene(sugerencias, EtiquetaPba), Is.True,
            $"TC-PBA03: 'Partido' es nomenclatura exclusiva de PBA; debe figurar bajo '{EtiquetaPba}'. " +
            $"Sugerencias: {Evidencia(sugerencias)}");
    }

    [Test]
    [Description("TC-L03 / TD-07: 'Avellaneda' (municipio del GBA limítrofe con CABA) debe figurar bajo PBA.")]
    public async Task TC_L03_Avellaneda_EsProvincia()
    {
        var field = await AbrirFormularioLocationAsync();
        var sugerencias = await ObtenerSugerenciasAsync(field, "Avellaneda", "TC-L03-avellaneda");

        Assert.That(AlgunaContiene(sugerencias, "Avellaneda", EtiquetaPba), Is.True,
            $"TC-L03: 'Avellaneda' debe figurar bajo '{EtiquetaPba}', sin confundirse con CABA. " +
            $"Sugerencias: {Evidencia(sugerencias)}");
    }

    [Test]
    [Description("TC-PBA04 / TD-08: 'Lanús' (municipio del GBA limítrofe con CABA) debe figurar bajo PBA.")]
    public async Task TC_PBA04_Lanus_EsProvincia()
    {
        var field = await AbrirFormularioLocationAsync();
        var sugerencias = await ObtenerSugerenciasAsync(field, "Lanús", "TC-PBA04-lanus");

        Assert.That(AlgunaContiene(sugerencias, "Lanús", EtiquetaPba), Is.True,
            $"TC-PBA04: 'Lanús' debe figurar bajo '{EtiquetaPba}', sin confundirse con CABA. " +
            $"Sugerencias: {Evidencia(sugerencias)}");
    }
}
