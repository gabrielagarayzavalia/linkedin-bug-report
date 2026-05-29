# cabaVsPBA

Proyecto de automatización web con **Playwright** y **C# (.NET 10)**, usando NUnit como runner de tests.

## Requisitos

- [.NET SDK 10](https://dotnet.microsoft.com/download) (verificado con `10.0.204`)
- Navegadores de Playwright (ya instalados durante el setup)

## Estructura

```
cabaVsPBA/
├── Tests/
│   └── ExampleTest.cs     # Tests de ejemplo (Playwright + NUnit)
├── .runsettings           # Configuración de navegador / headless / timeouts
├── cabaVsPBA.csproj
└── README.md
```

## Comandos

Compilar el proyecto:

```bash
dotnet build
```

Instalar / actualizar los navegadores de Playwright (tras compilar):

```bash
pwsh bin/Debug/net10.0/playwright.ps1 install
```

Ejecutar todos los tests:

```bash
dotnet test
```

Ejecutar con la configuración de `.runsettings`:

```bash
dotnet test --settings .runsettings
```

## Ver el navegador (modo no headless)

Editá `.runsettings` y poné:

```xml
<Headless>false</Headless>
```

O de forma puntual por variable de entorno:

```powershell
$env:HEADED=1; dotnet test
```

## Notas

- El runner usado es **NUnit** con `Microsoft.Playwright.NUnit`, la configuración oficial recomendada para Playwright en .NET.
- Los tests heredan de `PageTest`, que provee automáticamente `Page`, `Context` y `Browser` por test.
- La integración con Claude (API de Anthropic) queda pendiente para una etapa posterior.
