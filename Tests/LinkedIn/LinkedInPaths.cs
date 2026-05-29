using System.IO;
using NUnit.Framework;

namespace cabaVsPBA.Tests.LinkedIn;

/// <summary>Rutas compartidas del proyecto, resueltas relativas a la raiz.</summary>
public static class LinkedInPaths
{
    /// <summary>Raiz del proyecto (sube desde bin/Debug/netX.0).</summary>
    public static string ProjectRoot { get; } =
        Path.GetFullPath(Path.Combine(TestContext.CurrentContext.TestDirectory, "..", "..", ".."));

    public static string AuthDir { get; } = Path.Combine(ProjectRoot, "Auth");

    /// <summary>Estado de sesion capturado (cookies + localStorage de LinkedIn).</summary>
    public static string StorageStatePath { get; } = Path.Combine(AuthDir, "state.json");

    public static string TestResultsDir { get; } = Path.Combine(ProjectRoot, "TestResults");

    public static string BaselineDir { get; } = Path.Combine(TestResultsDir, "baseline");
}
