# To-do — Pendientes (post casos establecidos)

> Casos establecidos del PDF: **completados**. Ítems de backlog implementados en 2026-06-01.

---

## 1. Set de datos (Test Data Set)

- [x] Armar set de datos estructurado.
- [x] Alinear con PDF y TD-* en `docs/test-data/location-test-data.json`.
- [x] Formato JSON reutilizable + loader C# `LocationTestData.cs`.
- [x] Vincular TD con TC en `INDEX.md` y `docs/test-data/README.md`.

**Referencia:** [`docs/test-data/README.md`](../test-data/README.md)

---

## 2. Locale y lenguaje de presentación del perfil

**Alcance inicial (EN + ES):** implementado.

- [x] Parametrizar locale Playwright (`es-AR`, `en-US`) en `LinkedInTestBase`.
- [x] Suite `LocaleLocationTest` (L01, P01, PBA01 × 2 locales).
- [x] Documentar matriz en `docs/test-cases/locale-matrix.md`.

**Alcance futuro (opcional / baja prioridad):**

- [ ] Otros locales / timezone.
- [ ] Sesión con perfil Primary language English dedicada.
- [ ] Documentar si el bug CABA vs PBA varía por idioma de perfil (requiere corridas y registro en locale-matrix).

---

## 3. Restaurar perfil (Location)

- [x] TearDown restaura siempre (`finally` en `LinkedInMutableLocationTestBase`).
- [x] Test `[Explicit]` + `tools/restore-location.ps1`.
- [x] Baseline documentado en `docs/test-data/profile-baseline.md`.
- [x] Verificado tras TC-P01-Save: perfil vuelve a `CABA, Argentina`.

**Comando restore:** `pwsh tools/restore-location.ps1`

---

## Estado

| Ítem | Prioridad | Estado |
|------|-----------|--------|
| Set de datos | Media | ✅ Hecho |
| Locale / idioma (EN, ES) | Media | ✅ Hecho (tests Explicit; matriz para registrar corridas) |
| Restaurar perfil | Alta | ✅ Hecho |
| Locale / idioma (otros) | Baja | Pendiente opcional |
