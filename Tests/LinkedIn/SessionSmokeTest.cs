using System.Threading.Tasks;
using Microsoft.Playwright;
using NUnit.Framework;

namespace cabaVsPBA.Tests.LinkedIn;

/// <summary>
/// Verifica que la reutilizacion de sesion (storageState) funciona: entra a
/// LinkedIn ya logueado, sin automatizar el login.
/// Requiere haber capturado la sesion antes (Auth/state.json).
/// </summary>
[TestFixture]
public class SessionSmokeTest : LinkedInTestBase
{
    [Test]
    public async Task EntraLogueadoSinLogin()
    {
        await Page.GotoAsync("https://www.linkedin.com/feed/",
            new PageGotoOptions { WaitUntil = WaitUntilState.DOMContentLoaded });

        // Damos tiempo a que LinkedIn resuelva un eventual redirect a login.
        // (El feed nunca llega a NetworkIdle por su polling, asi que esperamos fijo.)
        await Page.WaitForTimeoutAsync(4000);

        await CapturarAsync("feed");

        // Si la sesion no fuera valida, LinkedIn redirige a /login o /authwall.
        var url = Page.Url;
        Assert.That(url, Does.Not.Contain("/login").And.Not.Contain("authwall"),
            $"Parece deslogueado (URL: {url}). Recapturá la sesión con CapturarSesion.");
        Assert.That(url, Does.Contain("/feed"),
            $"No llegó al feed (URL: {url}).");

        // Si estuviera deslogueado, habria un formulario con campo de password.
        var passwordInputs = await Page
            .Locator("input[name='session_password'], input#password").CountAsync();
        Assert.That(passwordInputs, Is.EqualTo(0),
            "Se mostró un formulario de login: la sesión no es válida.");
    }
}
