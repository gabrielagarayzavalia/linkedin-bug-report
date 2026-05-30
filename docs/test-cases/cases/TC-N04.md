# TC-N04 — Inyección de script (XSS)

| Campo | Valor |
|-------|-------|
| **Método C#** | `TC_N04_InyeccionScript_NoEjecuta` |
| **Archivo test** | `Tests/LinkedIn/NegativeLocationTest.cs` |
| **Categoría** | `Negative` |
| **Esperado** | Sin diálogo XSS; sin sugerencias de ubicación; campo usable |
| **Input** | `<script>alert('xss')</script>` |
| **Última ejecución** | 2026-05-29 |
| **Estado** | ✅ PASS (20 s) |
| **Evidencia** | `TestResults/baseline/TC_N04_InyeccionScript_NoEjecuta/` |

Ver PDF: `docs/TestCases_TestData_LinkedIn_CABA.pdf`.
