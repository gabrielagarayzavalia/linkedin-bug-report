using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Playwright;
using NUnit.Framework;

namespace cabaVsPBA.Tests.LinkedIn;

/// <summary>
/// Base para tests que pueden alterar Location en el formulario de experiencia.
///
/// - Al inicio de CADA test: captura y conserva el valor de Location (<see cref="LocationAlInicioDelTest"/>).
/// - Tras el test: si hubo Save exitoso, reabre el formulario y verifica que el valor
///   persistido coincide con lo guardado ANTES de restaurar.
/// - Al final: siempre deja el perfil con el valor que tenía al inicio del test.
/// </summary>
public abstract class LinkedInMutableLocationTestBase : LinkedInTestBase
{
    /// <summary>Valor de Location leído al abrir el formulario, antes de cualquier mutación.</summary>
    protected string LocationAlInicioDelTest { get; private set; } = string.Empty;

    /// <summary>True si un Save cerró el modal (el perfil pudo haber cambiado).</summary>
    protected bool GuardadoAlteroPerfil { get; set; }

    /// <summary>Valor que debería verse persistido tras Save (null = no verificar persistencia).</summary>
    protected string? ValorEsperadoTrasSave { get; set; }

    [SetUp]
    public async Task CapturarLocationAlInicioDelTestAsync()
    {
        var field = await AbrirFormularioLocationAsync();
        LocationAlInicioDelTest = await LeerLocationCompletoAsync(field);
        TestContext.Progress.WriteLine(
            $"[BASELINE-LOCATION] Valor al inicio del test: '{LocationAlInicioDelTest}'");
        await CapturarAsync("location-al-inicio-del-test");
    }

    [TearDown]
    public async Task VerificarPersistidoYRestaurarLocationAsync()
    {
        if (Page.IsClosed)
        {
            return;
        }

        try
        {
            var persistido = await ReleerLocationPersistidoAsync();
            TestContext.Progress.WriteLine(
                $"[PERSISTENCIA] Valor en perfil tras el test: '{persistido}'");

            await CapturarAsync("valor-persistido-antes-de-restaurar");

            if (GuardadoAlteroPerfil && ValorEsperadoTrasSave != null)
            {
                Assert.That(LocationCoincide(persistido, ValorEsperadoTrasSave), Is.True,
                    $"El valor GUARDADO no coincide con lo esperado. " +
                    $"Esperado tras Save: '{ValorEsperadoTrasSave}', Persistido en perfil: '{persistido}'. " +
                    "Consultar antes de reportar.");
            }
            else if (!GuardadoAlteroPerfil && !string.IsNullOrWhiteSpace(LocationAlInicioDelTest))
            {
                Assert.That(LocationCoincide(persistido, LocationAlInicioDelTest), Is.True,
                    $"El Save no debió alterar el perfil, pero Location cambió. " +
                    $"Al inicio: '{LocationAlInicioDelTest}', Ahora: '{persistido}'. " +
                    "Consultar antes de reportar.");
            }

            await RestaurarLocationAlInicioDelTestAsync();
        }
        catch (Exception ex)
        {
            TestContext.Progress.WriteLine(
                $"[RESTORE] Error en verificación/restauración: {ex.Message}");
            throw;
        }
    }

    /// <summary>Reabre el formulario y lee Location tal como quedó persistido.</summary>
    protected async Task<string> ReleerLocationPersistidoAsync()
    {
        await Page.GotoAsync(PositionFormUrl, new PageGotoOptions
        {
            WaitUntil = WaitUntilState.DOMContentLoaded,
            Timeout = 45000,
        });
        await Page.WaitForTimeoutAsync(4000);

        var field = ObtenerCampoLocation();
        await field.ScrollIntoViewIfNeededAsync();
        return await LeerLocationCompletoAsync(field);
    }

    /// <summary>Devuelve Location al valor capturado al inicio del test y guarda.</summary>
    protected async Task RestaurarLocationAlInicioDelTestAsync()
    {
        var persistido = await ReleerLocationPersistidoAsync();

        if (LocationCoincide(persistido, LocationAlInicioDelTest))
        {
            TestContext.Progress.WriteLine(
                "[RESTORE] Perfil ya tiene el valor al inicio del test; no hace falta restaurar.");
            return;
        }

        TestContext.Progress.WriteLine(
            $"[RESTORE] Restaurando Location de '{persistido}' → '{LocationAlInicioDelTest}'");

        var field = ObtenerCampoLocation();
        await RestaurarLocationAsync(field, LocationAlInicioDelTest);

        var verificado = await ReleerLocationPersistidoAsync();
        TestContext.Progress.WriteLine($"[RESTORE] Tras restaurar: '{verificado}'");

        Assert.That(LocationCoincide(verificado, LocationAlInicioDelTest), Is.True,
            $"No se pudo restaurar Location al valor al inicio del test. " +
            $"Esperado: '{LocationAlInicioDelTest}', Quedó: '{verificado}'.");
    }

    protected ILocator ObtenerCampoLocation() =>
        Page.GetByLabel(new Regex("location|ubicaci[oó]n|city|ciudad", RegexOptions.IgnoreCase)).First;

    /// <summary>
    /// Tras Save exitoso: valida reglas req/opcional y configura expectativa de persistencia
    /// (entidad canónica CABA según PDF). TearDown verifica el valor persistido.
    /// </summary>
    protected void ConfigurarExpectativaTrasSave(
        bool esRequerido,
        bool guardadoExitoso,
        IReadOnlyList<string> sugerenciasAlGuardar)
    {
        if (!guardadoExitoso)
        {
            return;
        }

        GuardadoAlteroPerfil = true;

        var hayEntidadCanonica = AlgunaContiene(sugerenciasAlGuardar, EtiquetaCaba);

        if (esRequerido)
        {
            Assert.That(hayEntidadCanonica, Is.True,
                "BUG validación: Location REQUERIDO pero Save guardó sin entidad canónica " +
                $"'{EtiquetaCaba}'. Sugerencias: {Evidencia(sugerenciasAlGuardar)}. Consultar antes de reportar.");
        }

        // Calidad de datos (PDF): debe persistir Autonomous City of Buenos Aires.
        ValorEsperadoTrasSave = EtiquetaCaba;
    }

    /// <summary>
    /// Compara dos valores de Location (input + texto visible del contenedor del campo).
    /// Vacío = sin texto relevante.
    /// </summary>
    protected static bool LocationCoincide(string actual, string esperado)
    {
        var a = NormalizarLocation(actual);
        var e = NormalizarLocation(esperado);

        if (string.IsNullOrEmpty(a) && string.IsNullOrEmpty(e))
        {
            return true;
        }

        if (string.IsNullOrEmpty(a) || string.IsNullOrEmpty(e))
        {
            return false;
        }

        return a.Contains(e, StringComparison.OrdinalIgnoreCase)
               || e.Contains(a, StringComparison.OrdinalIgnoreCase);
    }

    private static string NormalizarLocation(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return string.Empty;
        }

        return value.Trim()
            .Replace("\n", " ", StringComparison.Ordinal)
            .Replace("  ", " ", StringComparison.Ordinal);
    }
}
