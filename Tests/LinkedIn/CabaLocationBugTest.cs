using System.Threading.Tasks;
using Microsoft.Playwright;
using NUnit.Framework;

namespace cabaVsPBA.Tests.LinkedIn;

/// <summary>
/// Regresión del bug CABA vs PBA en el campo "Location" (typeahead de ubicación).
///
/// Estos tests afirman el COMPORTAMIENTO ESPERADO según la documentación de QA
/// (docs/TestCases_TestData_LinkedIn_CABA.pdf). Mientras el bug exista, FALLAN
/// a propósito: son evidencia ejecutable del defecto. Cuando LinkedIn modele
/// CABA como entidad independiente, pasarán a verde sin tocar el código.
///
/// Ejecutar solo este grupo:
///   dotnet test --filter "TestCategory=CABA-Bug" --settings .runsettings
///
/// Requiere sesión válida (Auth/state.json) y acceso a internet.
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
        var field = await AbrirFormularioLocationAsync();
        var sugerencias = await ObtenerSugerenciasAsync(
            field, "Ciudad Autónoma de Buenos Aires", "TC-P01-nombre-completo");

        Assert.That(AlgunaContiene(sugerencias, EtiquetaCaba), Is.True,
            $"TC-P01: se esperaba una sugerencia '{EtiquetaCaba}, Argentina' como entidad canónica " +
            $"(no texto libre). Sugerencias: {Evidencia(sugerencias)}");
    }

    [Test]
    [Description("TC-P01 / TD-02: la sigla 'CABA' debe mapear a Ciudad Autónoma de Buenos Aires.")]
    public async Task TC_P01_Sigla_CABA_DebeMapearACABA()
    {
        var field = await AbrirFormularioLocationAsync();
        var sugerencias = await ObtenerSugerenciasAsync(field, "CABA", "TC-P01-sigla");

        Assert.That(AlgunaContiene(sugerencias, EtiquetaCaba), Is.True,
            $"TC-P01: la sigla 'CABA' debe mapear a '{EtiquetaCaba}, Argentina'. " +
            $"Sugerencias: {Evidencia(sugerencias)}");
    }

    [Test]
    [Description("TC-P02 / TD-03: 'Palermo' debe asociarse a CABA, no a la Provincia.")]
    public async Task TC_P02_Palermo_DebeAsociarseACABA()
    {
        var field = await AbrirFormularioLocationAsync();
        var sugerencias = await ObtenerSugerenciasAsync(field, "Palermo", "TC-P02-palermo");

        Assert.That(AlgunaContiene(sugerencias, "Palermo", EtiquetaCaba), Is.True,
            $"TC-P02: se esperaba 'Palermo, {EtiquetaCaba}, Argentina'. " +
            $"Sugerencias: {Evidencia(sugerencias)}");
    }

    [Test]
    [Description("TC-L01 / TD-05: 'Buenos Aires' debe ofrecer CABA y PBA como opciones separadas.")]
    public async Task TC_L01_BuenosAires_DebeOfrecerAmbasJurisdicciones()
    {
        var field = await AbrirFormularioLocationAsync();
        var sugerencias = await ObtenerSugerenciasAsync(field, "Buenos Aires", "TC-L01-buenos-aires");

        Assert.Multiple(() =>
        {
            Assert.That(AlgunaContiene(sugerencias, EtiquetaCaba), Is.True,
                $"TC-L01: falta la opción '{EtiquetaCaba}'. Sugerencias: {Evidencia(sugerencias)}");
            Assert.That(AlgunaContiene(sugerencias, EtiquetaPba), Is.True,
                $"TC-L01: falta la opción '{EtiquetaPba}'. Sugerencias: {Evidencia(sugerencias)}");
        });
    }

    [Test]
    [Description("TC-L02 / TD-06: 'Villa Riachuelo' (barrio de CABA limítrofe con PBA) debe asociarse a CABA.")]
    public async Task TC_L02_VillaRiachuelo_DebeAsociarseACABA()
    {
        var field = await AbrirFormularioLocationAsync();
        var sugerencias = await ObtenerSugerenciasAsync(field, "Villa Riachuelo", "TC-L02-villa-riachuelo");

        Assert.That(AlgunaContiene(sugerencias, "Villa Riachuelo", EtiquetaCaba), Is.True,
            $"TC-L02: 'Villa Riachuelo' debe figurar bajo '{EtiquetaCaba}', no bajo la Provincia. " +
            $"Sugerencias: {Evidencia(sugerencias)}");
    }

    [Test]
    [Description("TC-L04 / TD-09: 'Comuna 9' (nomenclatura exclusiva de CABA) debe asociarse a CABA.")]
    public async Task TC_L04_Comuna9_DebeAsociarseACABA()
    {
        var field = await AbrirFormularioLocationAsync();
        var sugerencias = await ObtenerSugerenciasAsync(field, "Comuna 9", "TC-L04-comuna-9");

        Assert.That(AlgunaContiene(sugerencias, EtiquetaCaba), Is.True,
            $"TC-L04: 'Comuna' es nomenclatura exclusiva de CABA; debe mapear a '{EtiquetaCaba}'. " +
            $"Sugerencias: {Evidencia(sugerencias)}");
    }
}
