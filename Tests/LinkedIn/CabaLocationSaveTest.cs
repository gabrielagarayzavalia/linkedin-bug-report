using System.Threading.Tasks;
using Microsoft.Playwright;
using NUnit.Framework;

namespace cabaVsPBA.Tests.LinkedIn;

/// <summary>
/// Tests TC-*-Save: typeahead + Save + verificar persistencia + reglas req/opcional.
/// Complementa los tests de solo typeahead (CabaLocationBugTest) y TC-V01 (vacío).
///
/// Patrón por test:
///   1. Captura Location al inicio (LinkedInMutableLocationTestBase).
///   2. Escribe query, selecciona 1.ª sugerencia, Save.
///   3. Valida obligatoriedad vs. calidad de datos (entidad canónica).
///   4. TearDown: verifica valor persistido, restaura perfil.
///
///   dotnet test --filter "TestCategory=Save-Persistence" --settings .runsettings
/// </summary>
[TestFixture]
[Category("Save-Persistence")]
[Explicit("Save + persistencia en vivo: requiere sesión, red; altera y restaura perfil.")]
public class CabaLocationSaveTest : LinkedInMutableLocationTestBase
{
    [Test]
    [Description("TC-P01-Save / TD-01: guardar 'Ciudad Autónoma de Buenos Aires' y verificar persistencia canónica.")]
    public async Task TC_P01_Save_NombreCompleto_VerificarPersistido()
    {
        var field = await AbrirFormularioLocationAsync();
        var esRequerido = await LocationEsRequeridoAsync(field);

        TestContext.Progress.WriteLine(
            $"[TC-P01-Save] requerido={esRequerido}, valor al inicio='{LocationAlInicioDelTest}'");

        var sugerencias = await EscribirYSeleccionarPrimeraSugerenciaAsync(
            field, "Ciudad Autónoma de Buenos Aires", "TC-P01-save-typeahead");

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
