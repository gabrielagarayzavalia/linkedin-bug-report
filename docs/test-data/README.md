# Test data — Location typeahead

Datos centralizados en [`location-test-data.json`](location-test-data.json).

Loader C#: `Tests/LinkedIn/TestData/LocationTestData.cs`

```csharp
var td = LocationTestData.GetByTdId("TD-01");
var query = td.ResolveQuery();
```

## Índice TD → TC → método

| TD | TC | Método C# |
|----|-----|-----------|
| TD-01 | TC-P01, TC-P01-Save | `TC_P01_*`, `TC_P01_Save_*` |
| TD-02 | TC-P01 | `TC_P01_Sigla_CABA_DebeMapearACABA` |
| TD-03 | TC-P02 | `TC_P02_Palermo_DebeAsociarseACABA` |
| TD-05 | TC-L01 | `TC_L01_BuenosAires_DebeOfrecerAmbasJurisdicciones` |
| TD-06 | TC-L02 | `TC_L02_VillaRiachuelo_DebeAsociarseACABA` |
| TD-07 | TC-L03 | `TC_L03_Avellaneda_EsProvincia` |
| TD-08 | TC-PBA04 | `TC_PBA04_Lanus_EsProvincia` |
| TD-09 | TC-L04 | `TC_L04_Comuna9_DebeAsociarseACABA` |
| TD-11 | TC-PBA01 | `TC_PBA01_MarDelPlata_EsProvincia` |
| TD-12 | TC-PBA02 | `TC_PBA02_LaPlata_EsProvincia` |
| TD-13 | TC-PBA03 | `TC_PBA03_PartidoGeneralPueyrredon_EsProvincia` |
| TD-N01..N06 | TC-N01..N06 | `TC_N01_*` … `TC_N06_*` |

Baseline del perfil: [`profile-baseline.md`](profile-baseline.md)

Matriz locale: [`../test-cases/locale-matrix.md`](../test-cases/locale-matrix.md)
