using System.Threading.Tasks;
using Microsoft.Playwright;
using NUnit.Framework;

namespace cabaVsPBA.Tests.LinkedIn;

/// <summary>
/// Validación del botón Save vs. obligatoriedad del campo Location.
///
/// Reglas de negocio (definidas por QA):
///   - Campo REQUERIDO + Save con Location vacío → NO debe guardar (si guarda = bug).
///   - Campo NO requerido + Save con Location vacío → SÍ debe permitir guardar (si bloquea = bug).
///
/// Ciclo de vida del perfil (por test):
///   1. SetUp captura Location al inicio (<see cref="LinkedInMutableLocationTestBase"/>).
///   2. Tras Save exitoso: TearDown verifica valor persistido antes de restaurar.
///   3. TearDown siempre deja el perfil con el valor del paso 1.
///
/// Si la assertion falla, tratar como POSIBLE bug y consultar al usuario antes de reportar.
///
///   dotnet test --filter "TestCategory=Save-Validation" --settings .runsettings
/// </summary>
[TestFixture]
[Category("Save-Validation")]
[Explicit("Validación Save en vivo: requiere sesión, red y puede alterar el perfil (se restaura al final).")]
public class LocationSaveValidationTest : LinkedInMutableLocationTestBase
{
    [Test]
    [Description("TC-V01: Save con Location vacío debe respetar si el campo es requerido o no.")]
    public async Task TC_V01_SaveConLocationVacio_RespetaObligatoriedad()
    {
        var field = await AbrirFormularioLocationAsync();
        var esRequerido = await LocationEsRequeridoAsync(field);

        TestContext.Progress.WriteLine(
            $"[TC-V01] Location requerido={esRequerido}, valor al inicio='{LocationAlInicioDelTest}'");

        await CapturarAsync("antes-de-limpiar");
        await LimpiarLocationAsync(field);
        await CapturarAsync("location-vacio-antes-save");

        GuardadoAlteroPerfil = await IntentarGuardarAsync();
        var modalAbierto = await ModalSigueAbiertoAsync();
        var hayError = await HayErrorValidacionLocationAsync(field);

        await CapturarAsync("despues-de-save");

        TestContext.Progress.WriteLine(
            $"[TC-V01] guardadoExitoso={GuardadoAlteroPerfil}, modalAbierto={modalAbierto}, hayError={hayError}");

        if (GuardadoAlteroPerfil)
        {
            // Tras Save: esperamos Location vacío persistido; TearDown lo verifica antes de restaurar.
            ValorEsperadoTrasSave = string.Empty;
        }

        if (esRequerido)
        {
            Assert.That(GuardadoAlteroPerfil, Is.False,
                "BUG: el campo Location está marcado como REQUERIDO pero Save guardó igual con el campo vacío. " +
                "Evidencia en baseline (capturas + trace). Consultar antes de reportar.");
            Assert.That(modalAbierto || hayError, Is.True,
                "BUG: campo requerido vacío pero no hubo bloqueo visible (modal sigue abierto o error de validación). " +
                "Consultar antes de reportar.");
        }
        else
        {
            Assert.That(GuardadoAlteroPerfil || !hayError, Is.True,
                "BUG: el campo Location NO está marcado como requerido pero Save no permitió guardar " +
                $"(modalAbierto={modalAbierto}, hayError={hayError}). Consultar antes de reportar.");
        }
    }
}
