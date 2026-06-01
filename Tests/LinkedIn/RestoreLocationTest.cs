using System.Threading.Tasks;
using NUnit.Framework;

namespace cabaVsPBA.Tests.LinkedIn;

/// <summary>
/// Restauración manual del Location baseline del perfil de prueba.
///
///   dotnet test --filter "Name=RestaurarLocationBaseline" --settings .runsettings
///   pwsh tools/restore-location.ps1
/// </summary>
[TestFixture]
[Explicit("Altera Location en LinkedIn en vivo: usar solo para recuperar baseline.")]
public class RestoreLocationTest : LinkedInTestBase
{
    [Test]
    [Description("Restaura Location a ProfileBaselineLocation (CABA, Argentina).")]
    public async Task RestaurarLocationBaseline()
    {
        var field = await AbrirFormularioLocationAsync();
        var actual = await LeerLocationCompletoAsync(field);

        TestContext.Progress.WriteLine($"[RESTORE-BASELINE] Valor actual: '{actual}'");
        TestContext.Progress.WriteLine($"[RESTORE-BASELINE] Objetivo: '{ProfileBaselineLocation}'");
        await CapturarAsync("antes-restore-baseline");

        if (LocationCoincide(actual, ProfileBaselineLocation))
        {
            TestContext.Progress.WriteLine("[RESTORE-BASELINE] Perfil ya en baseline; no hace falta restaurar.");
            return;
        }

        await RestaurarLocationAsync(field, ProfileBaselineLocation);

        field = await AbrirFormularioLocationAsync();
        var verificado = await LeerLocationCompletoAsync(field);

        TestContext.Progress.WriteLine($"[RESTORE-BASELINE] Tras restaurar: '{verificado}'");
        await CapturarAsync("despues-restore-baseline");

        Assert.That(
            LocationCoincide(verificado, ProfileBaselineLocation),
            Is.True,
            $"No se pudo restaurar a '{ProfileBaselineLocation}'. Quedó: '{verificado}'.");
    }
}
