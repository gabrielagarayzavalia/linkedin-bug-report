using System;
using System.Threading.Tasks;
using Microsoft.Playwright;
using NUnit.Framework;

namespace cabaVsPBA.Tests.LinkedIn;

/// <summary>
/// Tests NEGATIVOS / de robustez del campo "Location" (typeahead de ubicación).
///
/// No están en el PDF de QA (que cubre Positivos y Límite); se agregan según el
/// estándar de la analista (datos vacíos/nulos, caracteres especiales, inyección,
/// cadenas largas y sin sentido). Verifican que una entrada inválida:
///   - NO produce una ubicación válida (ninguna sugerencia con "Argentina"), y
///   - NO rompe la página (sin diálogos/JS inyectado, el campo sigue usable).
/// Por lo tanto, lo ESPERADO es que PASEN.
///
///   dotnet test --filter "TestCategory=Negative" --settings .runsettings
///
/// Requiere sesión válida (Auth/state.json) y acceso a internet.
/// </summary>
[TestFixture]
[Category("Negative")]
[Explicit("Robustez contra LinkedIn en vivo: requiere sesión y red.")]
public class NegativeLocationTest : LinkedInTestBase
{
    [Test]
    [Description("TC-N01: campo vacío -> sin sugerencias de ubicación, sin error.")]
    public async Task TC_N01_CampoVacio_SinSugerencias()
    {
        var field = await AbrirFormularioLocationAsync();
        var sugerencias = await ObtenerSugerenciasAsync(field, string.Empty, "TC-N01-vacio");

        Assert.That(AlgunaContiene(sugerencias, "Argentina"), Is.False,
            $"TC-N01: un campo vacío no debería ofrecer ubicaciones. Sugerencias: {Evidencia(sugerencias)}");
    }

    [Test]
    [Description("TC-N02: solo espacios en blanco -> sin sugerencias de ubicación.")]
    public async Task TC_N02_SoloEspacios_SinSugerencias()
    {
        var field = await AbrirFormularioLocationAsync();
        var sugerencias = await ObtenerSugerenciasAsync(field, "     ", "TC-N02-espacios");

        Assert.That(AlgunaContiene(sugerencias, "Argentina"), Is.False,
            $"TC-N02: solo espacios no debería ofrecer ubicaciones. Sugerencias: {Evidencia(sugerencias)}");
    }

    [Test]
    [Description("TC-N03: caracteres especiales -> sin ubicaciones válidas, sin crash.")]
    public async Task TC_N03_CaracteresEspeciales_SinUbicacionValida()
    {
        var field = await AbrirFormularioLocationAsync();
        var sugerencias = await ObtenerSugerenciasAsync(field, "@#$%^&*()_+{}<>", "TC-N03-especiales");

        Assert.That(AlgunaContiene(sugerencias, "Argentina"), Is.False,
            $"TC-N03: caracteres especiales no deberían mapear a una ubicación. Sugerencias: {Evidencia(sugerencias)}");
        Assert.That(await field.IsVisibleAsync(), Is.True, "TC-N03: el campo dejó de estar usable.");
    }

    [Test]
    [Description("TC-N04: intento de inyección de script -> tratado como texto, sin ejecutar ni romper.")]
    public async Task TC_N04_InyeccionScript_NoEjecuta()
    {
        var field = await AbrirFormularioLocationAsync();

        var apareceDialogo = false;
        Page.Dialog += (_, dialog) =>
        {
            apareceDialogo = true;
            _ = dialog.DismissAsync();
        };

        var sugerencias = await ObtenerSugerenciasAsync(
            field, "<script>alert('xss')</script>", "TC-N04-inyeccion");

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
    [Description("TC-N05: cadena extremadamente larga -> sin ubicación válida, sin crash.")]
    public async Task TC_N05_CadenaLarga_SinCrash()
    {
        var field = await AbrirFormularioLocationAsync();
        var larga = new string('a', 300);
        var sugerencias = await ObtenerSugerenciasAsync(field, larga, "TC-N05-larga");

        Assert.That(AlgunaContiene(sugerencias, "Argentina"), Is.False,
            $"TC-N05: una cadena larga sin sentido no debería ofrecer ubicaciones. Sugerencias: {Evidencia(sugerencias)}");
        Assert.That(await field.IsVisibleAsync(), Is.True, "TC-N05: el campo dejó de estar usable.");
    }

    [Test]
    [Description("TC-N06: cadena sin sentido -> sin sugerencias de ubicación.")]
    public async Task TC_N06_Gibberish_SinSugerencias()
    {
        var field = await AbrirFormularioLocationAsync();
        var sugerencias = await ObtenerSugerenciasAsync(field, "zzzqqqxywv123", "TC-N06-gibberish");

        Assert.That(AlgunaContiene(sugerencias, "Argentina"), Is.False,
            $"TC-N06: una cadena sin sentido no debería ofrecer ubicaciones. Sugerencias: {Evidencia(sugerencias)}");
    }
}
