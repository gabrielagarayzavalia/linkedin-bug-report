# Índice de casos de prueba — TC → archivo .md

Cada TC tiene un `.md` en `cases/`. Plantilla: `_TEMPLATE.md`.

| TC | Archivo | Método C# | Categoría | Notas |
|----|---------|-----------|-----------|-------|
| TC-P01 | [cases/TC-P01.md](cases/TC-P01.md) | `TC_P01_NombreCompleto_DebeSugerirCABA` | CABA-Bug | Typeahead nombre completo |
| TC-P01 | [cases/TC-P01.md](cases/TC-P01.md) | `TC_P01_Sigla_CABA_DebeMapearACABA` | CABA-Bug | Typeahead sigla CABA |
| TC-P01-Save | [cases/TC-P01-Save.md](cases/TC-P01-Save.md) | `TC_P01_Save_NombreCompleto_VerificarPersistido` | Save-Persistence | Save + persistencia |
| TC-P02 | [cases/TC-P02.md](cases/TC-P02.md) | `TC_P02_Palermo_DebeAsociarseACABA` | CABA-Bug | |
| TC-L01 | [cases/TC-L01.md](cases/TC-L01.md) | `TC_L01_BuenosAires_DebeOfrecerAmbasJurisdicciones` | CABA-Bug | |
| TC-L02 | [cases/TC-L02.md](cases/TC-L02.md) | `TC_L02_VillaRiachuelo_DebeAsociarseACABA` | CABA-Bug | |
| TC-L03 | [cases/TC-L03.md](cases/TC-L03.md) | `TC_L03_Avellaneda_EsProvincia` | PBA-Regression | |
| TC-L04 | [cases/TC-L04.md](cases/TC-L04.md) | `TC_L04_Comuna9_DebeAsociarseACABA` | CABA-Bug | |
| TC-PBA01 | [cases/TC-PBA01.md](cases/TC-PBA01.md) | `TC_PBA01_MarDelPlata_EsProvincia` | PBA-Regression | |
| TC-PBA02 | [cases/TC-PBA02.md](cases/TC-PBA02.md) | `TC_PBA02_LaPlata_EsProvincia` | PBA-Regression | |
| TC-PBA03 | [cases/TC-PBA03.md](cases/TC-PBA03.md) | `TC_PBA03_PartidoGeneralPueyrredon_EsProvincia` | PBA-Regression | |
| TC-PBA04 | [cases/TC-PBA04.md](cases/TC-PBA04.md) | `TC_PBA04_Lanus_EsProvincia` | PBA-Regression | |
| TC-N01 | [cases/TC-N01.md](cases/TC-N01.md) | `TC_N01_CampoVacio_SinSugerencias` | Negative | Ver también [negative-location.md](negative-location.md) |
| TC-N02 | [cases/TC-N02.md](cases/TC-N02.md) | `TC_N02_SoloEspacios_SinSugerencias` | Negative | |
| TC-N03 | [cases/TC-N03.md](cases/TC-N03.md) | `TC_N03_CaracteresEspeciales_SinUbicacionValida` | Negative | |
| TC-N04 | [cases/TC-N04.md](cases/TC-N04.md) | `TC_N04_InyeccionScript_NoEjecuta` | Negative | |
| TC-N05 | [cases/TC-N05.md](cases/TC-N05.md) | `TC_N05_CadenaLarga_SinCrash` | Negative | ⏸ Consultar hipótesis |
| TC-N06 | [cases/TC-N06.md](cases/TC-N06.md) | `TC_N06_Gibberish_SinSugerencias` | Negative | |
| TC-V01 | [cases/TC-V01.md](cases/TC-V01.md) | `TC_V01_SaveConLocationVacio_RespetaObligatoriedad` | Save-Validation | Location vacío req/opcional |

## Documentos agrupados (legacy)

- [negative-location.md](negative-location.md) — resumen TC-N01..N06
- [save-persistence.md](save-persistence.md) — patrón TC-*-Save
- [../todo/BACKLOG.md](../todo/BACKLOG.md) — to-do post casos establecidos (datos, locale/idioma)

## Regla para el agente

Antes de ejecutar un TC: abrir su `.md` en `cases/`. Después: actualizar **Resultado actual**, **Estado** y **Evidencia**.
