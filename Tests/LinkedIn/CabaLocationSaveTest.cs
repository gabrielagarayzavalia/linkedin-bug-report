using System.Threading.Tasks;
using cabaVsPBA.Tests.LinkedIn.TestData;
using Microsoft.Playwright;
using NUnit.Framework;

namespace cabaVsPBA.Tests.LinkedIn;

[TestFixture]
[Category("Save-Persistence")]
[Explicit("Save + persistencia en vivo: requiere sesión, red; altera y restaura perfil.")]
public class CabaLocationSaveTest : LinkedInMutableLocationTestBase
{
    [Test]
    [Description("TC-P01-Save / TD-01: guardar y verificar persistencia canónica.")]
    public async Task TC_P01_Save_NombreCompleto_VerificarPersistido()
    {
        var td = LocationTestData.GetByTdId("TD-01");
        var field = await AbrirFormularioLocationAsync();
        var esRequerido = await LocationEsRequeridoAsync(field);

        TestContext.Progress.WriteLine(
            $"[TC-P01-Save] requerido={esRequerido}, valor al inicio='{LocationAlInicioDelTest}'");

        var sugerencias = await EscribirYSeleccionarPrimeraSugerenciaAsync(
            field, td.ResolveQuery(), "TC-P01-save-typeahead");

        await CapturarAsync("antes-save");

        var guardadoExitoso = await IntentarGuardarAsync();
        var modalAbierto = await ModalSigueAbiertoAsync();

        TestContext.Progress.WriteLine(
            $"[TC-P01-Save] guardadoExitoso={guardadoExitoso}, modalAbierto={modalAbierto}");

        if (!guardadoExitoso && modalAbierto)
        {
            var hayError = await HayErrorValidacionLocationAsync(ObtenerCampoLocation());
            if (esRequerido)
            {
                TestContext.Progress.WriteLine(
                    "[TC-P01-Save] Save bloqueado con campo requerido (comportamiento de validación OK).");
            }
            else
            {
                Assert.That(hayError, Is.False,
                    "BUG validación: Location NO requerido pero Save no permitió guardar. Consultar antes de reportar.");
            }

            return;
        }

        GuardadoAlteroPerfil = guardadoExitoso;
        ConfigurarExpectativaTrasSave(esRequerido, guardadoExitoso, sugerencias);
    }
}
