using System.Threading.Tasks;
using cabaVsPBA.Tests.LinkedIn.TestData;
using Microsoft.Playwright;
using NUnit.Framework;

namespace cabaVsPBA.Tests.LinkedIn;

/// <summary>
/// Regresión PBA. Datos: docs/test-data/location-test-data.json
/// </summary>
[TestFixture]
[Category("PBA-Regression")]
[Explicit("Regresión contra LinkedIn en vivo: requiere sesión y red.")]
public class PbaLocationRegressionTest : LinkedInTestBase
{
    [Test]
    [Description("TC-PBA01 / TD-11: 'Mar del Plata' debe figurar bajo PBA.")]
    public async Task TC_PBA01_MarDelPlata_EsProvincia()
    {
        var td = LocationTestData.GetByTdId("TD-11");
        var field = await AbrirFormularioLocationAsync();
        var sugerencias = await ObtenerSugerenciasAsync(field, td.ResolveQuery(), "TC-PBA01-mar-del-plata");

        Assert.That(AlgunaContiene(sugerencias, td.PlaceName!, EtiquetaPba), Is.True,
            $"TC-PBA01: se esperaba '{td.PlaceName}, {EtiquetaPba}, Argentina'. " +
            $"Sugerencias: {Evidencia(sugerencias)}");
    }

    [Test]
    [Description("TC-PBA02 / TD-12: 'La Plata' debe figurar bajo PBA.")]
    public async Task TC_PBA02_LaPlata_EsProvincia()
    {
        var td = LocationTestData.GetByTdId("TD-12");
        var field = await AbrirFormularioLocationAsync();
        var sugerencias = await ObtenerSugerenciasAsync(field, td.ResolveQuery(), "TC-PBA02-la-plata");

        Assert.That(AlgunaContiene(sugerencias, td.PlaceName!, EtiquetaPba), Is.True,
            $"TC-PBA02: se esperaba '{td.PlaceName}, {EtiquetaPba}, Argentina'. " +
            $"Sugerencias: {Evidencia(sugerencias)}");
    }

    [Test]
    [Description("TC-PBA03 / TD-13: 'Partido de General Pueyrredón' debe figurar bajo PBA.")]
    public async Task TC_PBA03_PartidoGeneralPueyrredon_EsProvincia()
    {
        var td = LocationTestData.GetByTdId("TD-13");
        var field = await AbrirFormularioLocationAsync();
        var sugerencias = await ObtenerSugerenciasAsync(field, td.ResolveQuery(), "TC-PBA03-partido-pueyrredon");

        Assert.That(AlgunaContiene(sugerencias, EtiquetaPba), Is.True,
            $"TC-PBA03: 'Partido' es nomenclatura exclusiva de PBA; debe figurar bajo '{EtiquetaPba}'. " +
            $"Sugerencias: {Evidencia(sugerencias)}");
    }

    [Test]
    [Description("TC-L03 / TD-07: 'Avellaneda' debe figurar bajo PBA.")]
    public async Task TC_L03_Avellaneda_EsProvincia()
    {
        var td = LocationTestData.GetByTdId("TD-07");
        var field = await AbrirFormularioLocationAsync();
        var sugerencias = await ObtenerSugerenciasAsync(field, td.ResolveQuery(), "TC-L03-avellaneda");

        Assert.That(AlgunaContiene(sugerencias, td.PlaceName!, EtiquetaPba), Is.True,
            $"TC-L03: 'Avellaneda' debe figurar bajo '{EtiquetaPba}', sin confundirse con CABA. " +
            $"Sugerencias: {Evidencia(sugerencias)}");
    }

    [Test]
    [Description("TC-PBA04 / TD-08: 'Lanús' debe figurar bajo PBA.")]
    public async Task TC_PBA04_Lanus_EsProvincia()
    {
        var td = LocationTestData.GetByTdId("TD-08");
        var field = await AbrirFormularioLocationAsync();
        var sugerencias = await ObtenerSugerenciasAsync(field, td.ResolveQuery(), "TC-PBA04-lanus");

        Assert.That(AlgunaContiene(sugerencias, td.PlaceName!, EtiquetaPba), Is.True,
            $"TC-PBA04: 'Lanús' debe figurar bajo '{EtiquetaPba}', sin confundirse con CABA. " +
            $"Sugerencias: {Evidencia(sugerencias)}");
    }
}
