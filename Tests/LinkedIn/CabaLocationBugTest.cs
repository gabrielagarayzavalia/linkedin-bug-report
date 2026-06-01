using System.Threading.Tasks;
using cabaVsPBA.Tests.LinkedIn.TestData;
using Microsoft.Playwright;
using NUnit.Framework;

namespace cabaVsPBA.Tests.LinkedIn;

/// <summary>
/// Regresión del bug CABA vs PBA en el campo "Location" (typeahead de ubicación).
/// Datos: docs/test-data/location-test-data.json
/// </summary>
[TestFixture]
[Category("CABA-Bug")]
[Explicit("Regresión contra LinkedIn en vivo: requiere sesión y red.")]
public class CabaLocationBugTest : LinkedInTestBase
{
    [Test]
    [Description("TC-P01 / TD-01: 'Ciudad Autónoma de Buenos Aires' debe sugerir CABA como entidad canónica.")]
    public async Task TC_P01_NombreCompleto_DebeSugerirCABA()
    {
        var td = LocationTestData.GetByTdId("TD-01");
        var field = await AbrirFormularioLocationAsync();
        var sugerencias = await ObtenerSugerenciasAsync(field, td.ResolveQuery(), "TC-P01-nombre-completo");

        Assert.That(AlgunaContiene(sugerencias, td.ExpectedLabelEn!), Is.True,
            $"TC-P01: se esperaba una sugerencia '{td.ExpectedLabelEn}, Argentina' como entidad canónica " +
            $"(no texto libre). Sugerencias: {Evidencia(sugerencias)}");
    }

    [Test]
    [Description("TC-P01 / TD-02: la sigla 'CABA' debe mapear a Ciudad Autónoma de Buenos Aires.")]
    public async Task TC_P01_Sigla_CABA_DebeMapearACABA()
    {
        var td = LocationTestData.GetByTdId("TD-02");
        var field = await AbrirFormularioLocationAsync();
        var sugerencias = await ObtenerSugerenciasAsync(field, td.ResolveQuery(), "TC-P01-sigla");

        Assert.That(AlgunaContiene(sugerencias, td.ExpectedLabelEn!), Is.True,
            $"TC-P01: la sigla 'CABA' debe mapear a '{td.ExpectedLabelEn}, Argentina'. " +
            $"Sugerencias: {Evidencia(sugerencias)}");
    }

    [Test]
    [Description("TC-P02 / TD-03: 'Palermo' debe asociarse a CABA, no a la Provincia.")]
    public async Task TC_P02_Palermo_DebeAsociarseACABA()
    {
        var td = LocationTestData.GetByTdId("TD-03");
        var field = await AbrirFormularioLocationAsync();
        var sugerencias = await ObtenerSugerenciasAsync(field, td.ResolveQuery(), "TC-P02-palermo");

        Assert.That(AlgunaContiene(sugerencias, td.PlaceName!, td.ExpectedLabelEn!), Is.True,
            $"TC-P02: se esperaba '{td.PlaceName}, {td.ExpectedLabelEn}, Argentina'. " +
            $"Sugerencias: {Evidencia(sugerencias)}");
    }

    [Test]
    [Description("TC-L01 / TD-05: 'Buenos Aires' debe ofrecer CABA y PBA como opciones separadas.")]
    public async Task TC_L01_BuenosAires_DebeOfrecerAmbasJurisdicciones()
    {
        var td = LocationTestData.GetByTdId("TD-05");
        var field = await AbrirFormularioLocationAsync();
        var sugerencias = await ObtenerSugerenciasAsync(field, td.ResolveQuery(), "TC-L01-buenos-aires");

        Assert.Multiple(() =>
        {
            Assert.That(AlgunaContiene(sugerencias, td.ExpectedLabelEn!), Is.True,
                $"TC-L01: falta la opción '{td.ExpectedLabelEn}'. Sugerencias: {Evidencia(sugerencias)}");
            Assert.That(AlgunaContiene(sugerencias, EtiquetaPba), Is.True,
                $"TC-L01: falta la opción '{EtiquetaPba}'. Sugerencias: {Evidencia(sugerencias)}");
        });
    }

    [Test]
    [Description("TC-L02 / TD-06: 'Villa Riachuelo' debe asociarse a CABA.")]
    public async Task TC_L02_VillaRiachuelo_DebeAsociarseACABA()
    {
        var td = LocationTestData.GetByTdId("TD-06");
        var field = await AbrirFormularioLocationAsync();
        var sugerencias = await ObtenerSugerenciasAsync(field, td.ResolveQuery(), "TC-L02-villa-riachuelo");

        Assert.That(AlgunaContiene(sugerencias, td.PlaceName!, td.ExpectedLabelEn!), Is.True,
            $"TC-L02: '{td.PlaceName}' debe figurar bajo '{td.ExpectedLabelEn}', no bajo la Provincia. " +
            $"Sugerencias: {Evidencia(sugerencias)}");
    }

    [Test]
    [Description("TC-L04 / TD-09: 'Comuna 9' debe asociarse a CABA.")]
    public async Task TC_L04_Comuna9_DebeAsociarseACABA()
    {
        var td = LocationTestData.GetByTdId("TD-09");
        var field = await AbrirFormularioLocationAsync();
        var sugerencias = await ObtenerSugerenciasAsync(field, td.ResolveQuery(), "TC-L04-comuna-9");

        Assert.That(AlgunaContiene(sugerencias, td.ExpectedLabelEn!), Is.True,
            $"TC-L04: 'Comuna' es nomenclatura exclusiva de CABA; debe mapear a '{td.ExpectedLabelEn}'. " +
            $"Sugerencias: {Evidencia(sugerencias)}");
    }
}
