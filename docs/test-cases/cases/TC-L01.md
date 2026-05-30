# TC-L01 — Buenos Aires: ambas jurisdicciones (CABA y PBA)

## Metadatos

| Campo | Valor |
|-------|-------|
| **ID** | TC-L01 |
| **Título** | Ambiguous search: term 'Buenos Aires' |
| **Clasificación** | Límite (Boundary) |
| **Test data** | TD-05 |

## Automatización

| Campo | Valor |
|-------|-------|
| **Método C#** | `TC_L01_BuenosAires_DebeOfrecerAmbasJurisdicciones` |
| **Categoría** | `CABA-Bug` |

## Resultado esperado

Mostrar **por separado** `Autonomous City of Buenos Aires` y `Province of Buenos Aires` / `Buenos Aires Province`.

## Resultado actual

| Campo | Valor |
|-------|-------|
| **Última ejecución** | 2026-05-30 |
| **Observado** | 10 sugerencias; **solo PBA** (`Buenos Aires Province`, GBA, ciudades/partidos de provincia). **Sin** `Autonomous City of Buenos Aires`. Incluye `Buenos Aires, Buenos Aires Province, Argentina` (anidación incorrecta). |
| **Estado** | ❌ FAIL |
| **Evidencia** | `TestResults/baseline/TC_L01_BuenosAires_DebeOfrecerAmbasJurisdicciones/` |

## Notas

- Coincide con PDF: solo aparece Provincia; CABA no existe como entidad.
- TC-P02 Palermo confirmado FAIL (2026-05-30).
