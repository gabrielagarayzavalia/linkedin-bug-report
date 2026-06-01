using System;
using System.Threading.Tasks;
using cabaVsPBA.Tests.LinkedIn.TestData;
using Microsoft.Playwright;
using NUnit.Framework;

namespace cabaVsPBA.Tests.LinkedIn;

/// <summary>
/// Tests negativos Location. Datos: docs/test-data/location-test-data.json
/// </summary>
[TestFixture]
[Category("Negative")]
[Explicit("Robustez contra LinkedIn en vivo: requiere sesión y red.")]
public class NegativeLocationTest : LinkedInTestBase
{
    [Test]
    [Description("TC-N01 / TD-N01: campo vacío -> sin sugerencias.")]
    public async Task TC_N01_CampoVacio_SinSugerencias()
    {
        var td = LocationTestData.GetByTdId("TD-N01");
        var field = await AbrirFormularioLocationAsync();
        var sugerencias = await ObtenerSugerenciasAsync(field, td.ResolveQuery(), "TC-N01-vacio");

        Assert.That(AlgunaContiene(sugerencias, "Argentina"), Is.False,
            $"TC-N01: un campo vacío no debería ofrecer ubicaciones. Sugerencias: {Evidencia(sugerencias)}");
    }

    [Test]
    [Description("TC-N02 / TD-N02: solo espacios -> sin sugerencias.")]
    public async Task TC_N02_SoloEspacios_SinSugerencias()
    {
        var td = LocationTestData.GetByTdId("TD-N02");
        var field = await AbrirFormularioLocationAsync();
        var sugerencias = await ObtenerSugerenciasAsync(field, td.ResolveQuery(), "TC-N02-espacios");

        Assert.That(AlgunaContiene(sugerencias, "Argentina"), Is.False,
            $"TC-N02: solo espacios no debería ofrecer ubicaciones. Sugerencias: {Evidencia(sugerencias)}");
    }

    [Test]
    [Description("TC-N03 / TD-N03: caracteres especiales -> sin ubicaciones válidas.")]
    public async Task TC_N03_CaracteresEspeciales_SinUbicacionValida()
    {
        var td = LocationTestData.GetByTdId("TD-N03");
        var field = await AbrirFormularioLocationAsync();
        var sugerencias = await ObtenerSugerenciasAsync(field, td.ResolveQuery(), "TC-N03-especiales");

        Assert.That(AlgunaContiene(sugerencias, "Argentina"), Is.False,
            $"TC-N03: caracteres especiales no deberían mapear a una ubicación. Sugerencias: {Evidencia(sugerencias)}");
        Assert.That(await field.IsVisibleAsync(), Is.True, "TC-N03: el campo dejó de estar usable.");
    }

    [Test]
    [Description("TC-N04 / TD-N04: inyección script -> sin ejecutar ni romper.")]
    public async Task TC_N04_InyeccionScript_NoEjecuta()
    {
        var td = LocationTestData.GetByTdId("TD-N04");
        var field = await AbrirFormularioLocationAsync();

        var apareceDialogo = false;
        Page.Dialog += (_, dialog) =>
        {
            apareceDialogo = true;
            _ = dialog.DismissAsync();
        };

        var sugerencias = await ObtenerSugerenciasAsync(field, td.ResolveQuery(), "TC-N04-inyeccion");

        Assert.Multiple(() =>
        {
            Assert.That(apareceDialogo, Is.False, "TC-N04: se ejecutó un diálogo: posible XSS.");
            Assert.That(AlgunaContiene(sugerencias, "Argentina"), Is.False,
                $"TC-N04: la inyección no debería mapear a una ubicación. Sugerencias: {Evidencia(sugerencias)}");
            Assert.That(field.IsVisibleAsync().GetAwaiter().GetResult(), Is.True,
                "TC-N04: el campo dejó de estar usable.");
        });
    }

    [Test]
    [Description("TC-N05 / TD-N05: cadena larga -> sin crash.")]
    public async Task TC_N05_CadenaLarga_SinCrash()
    {
        var td = LocationTestData.GetByTdId("TD-N05");
        var field = await AbrirFormularioLocationAsync();
        var sugerencias = await ObtenerSugerenciasAsync(field, td.ResolveQuery(), "TC-N05-larga");

        Assert.That(AlgunaContiene(sugerencias, "Argentina"), Is.False,
            $"TC-N05: una cadena larga sin sentido no debería ofrecer ubicaciones. Sugerencias: {Evidencia(sugerencias)}");
        Assert.That(await field.IsVisibleAsync(), Is.True, "TC-N05: el campo dejó de estar usable.");
    }

    [Test]
    [Description("TC-N06 / TD-N06: gibberish -> sin sugerencias.")]
    public async Task TC_N06_Gibberish_SinSugerencias()
    {
        var td = LocationTestData.GetByTdId("TD-N06");
        var field = await AbrirFormularioLocationAsync();
        var sugerencias = await ObtenerSugerenciasAsync(field, td.ResolveQuery(), "TC-N06-gibberish");

        Assert.That(AlgunaContiene(sugerencias, "Argentina"), Is.False,
            $"TC-N06: una cadena sin sentido no debería ofrecer ubicaciones. Sugerencias: {Evidencia(sugerencias)}");
    }
}
