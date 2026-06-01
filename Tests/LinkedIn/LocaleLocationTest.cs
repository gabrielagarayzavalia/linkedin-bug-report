using System.Collections;
using System.Threading.Tasks;
using cabaVsPBA.Tests.LinkedIn.TestData;
using NUnit.Framework;

namespace cabaVsPBA.Tests.LinkedIn;

/// <summary>
/// Matriz locale × query (subset L01, P01, PBA01). Categoría Locale-Matrix, Explicit.
/// </summary>
[TestFixture]
[Category("Locale-Matrix")]
[Explicit("Matriz locale en vivo: requiere sesión y red.")]
public class LocaleLocationTest : LinkedInTestBase
{
    public static IEnumerable LocaleCases()
    {
        yield return new TestCaseData("es-AR", "TD-05").SetName("Locale_L01_BuenosAires_esAR");
        yield return new TestCaseData("en-US", "TD-05").SetName("Locale_L01_BuenosAires_enUS");
        yield return new TestCaseData("es-AR", "TD-01").SetName("Locale_P01_CABA_esAR");
        yield return new TestCaseData("en-US", "TD-01").SetName("Locale_P01_CABA_enUS");
        yield return new TestCaseData("es-AR", "TD-11").SetName("Locale_PBA01_MarDelPlata_esAR");
        yield return new TestCaseData("en-US", "TD-11").SetName("Locale_PBA01_MarDelPlata_enUS");
    }

    [Test]
    [TestCaseSource(nameof(LocaleCases))]
    [Description("Matriz locale: typeahead Location con etiquetas según locale.")]
    public async Task Locale_Matrix_Typeahead(string locale, string tdId)
    {
        var td = LocationTestData.GetByTdId(tdId);
        var etiquetaCaba = EtiquetaCabaParaLocale(locale);
        var etiquetaPba = EtiquetaPbaParaLocale(locale);

        TestContext.Progress.WriteLine($"[LOCALE] locale={locale}, td={tdId}, query='{td.ResolveQuery()}'");

        var field = await AbrirFormularioLocationAsync();
        var sugerencias = await ObtenerSugerenciasAsync(
            field, td.ResolveQuery(), $"locale-{locale}-{tdId}");

        switch (td.ExpectedJurisdiction)
        {
            case "CABA" when td.AlsoExpectPba:
                Assert.Multiple(() =>
                {
                    Assert.That(AlgunaContiene(sugerencias, etiquetaCaba), Is.True,
                        $"Falta CABA ({etiquetaCaba}). Sugerencias: {Evidencia(sugerencias)}");
                    Assert.That(AlgunaContiene(sugerencias, etiquetaPba), Is.True,
                        $"Falta PBA ({etiquetaPba}). Sugerencias: {Evidencia(sugerencias)}");
                });
                break;
            case "CABA":
                Assert.That(AlgunaContiene(sugerencias, etiquetaCaba), Is.True,
                    $"Falta CABA ({etiquetaCaba}). Sugerencias: {Evidencia(sugerencias)}");
                break;
            case "PBA":
                Assert.That(
                    AlgunaContiene(sugerencias, td.PlaceName ?? td.ResolveQuery(), etiquetaPba),
                    Is.True,
                    $"Falta PBA para '{td.PlaceName}'. Sugerencias: {Evidencia(sugerencias)}");
                break;
            default:
                Assert.Fail($"Jurisdicción no soportada en matriz locale: {td.ExpectedJurisdiction}");
                break;
        }
    }
}
