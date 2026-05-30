# To-do — Pendientes (post casos establecidos)

> **Recordatorio:** retomar estos ítems cuando terminemos de ejecutar/revisar
> todos los casos ya establecidos del PDF y automatizados (TC-P*, TC-L*, TC-PBA*,
> TC-N*, TC-V*, TC-*-Save). El agente debe recordar al usuario al cerrar esa fase.

---

## 1. Set de datos (Test Data Set)

- [ ] Armar / completar set de datos de prueba estructurado (más allá del PDF actual).
- [ ] Alinear con `docs/TestCases_TestData_LinkedIn_CABA.pdf` y ampliar TD-* faltantes.
- [ ] Formato reutilizable (JSON/CSV o `.md` tabular) para no hardcodear en tests.
- [ ] Vincular cada TD con su TC en `docs/test-cases/cases/` e `INDEX.md`.

**Objetivo:** datos centralizados para automatización y futuro template.

---

## 2. Locale y lenguaje de presentación del perfil

**Alcance inicial (obligatorio por ahora):** verificar solo en **inglés** y **español**.

**Alcance futuro (opcional / backlog):**

- [ ] Set de pruebas para **locale** (región, timezone, formato de ubicación).
- [ ] Set de pruebas para **lenguaje de presentación del perfil** (UI del formulario, etiquetas, sugerencias del typeahead).
- [ ] Matriz: locale × idioma de perfil × query de Location (ej. `Buenos Aires` en EN vs ES).
- [ ] Documentar si el bug CABA vs PBA se manifiesta distinto según idioma/locale.

**Notas:**

- Hoy los tests usan `Locale = es-AR` en contexto Playwright; falta variante **en-US / English (Primary profile)** explícita.
- LinkedIn puede mostrar `Buenos Aires Province` (EN) vs `Provincia de Buenos Aires` (ES).

---

## 3. Recordatorio para el agente

Cuando el usuario indique que **terminó la revisión de casos establecidos**, preguntar:

> «¿Retomamos el to-do de **set de datos** y **locale/idioma (EN + ES)**?»

---

## Estado

| Ítem | Prioridad | Cuándo |
|------|-----------|--------|
| Set de datos | Media | Después de casos establecidos |
| Locale / idioma (EN, ES) | Media | Después de casos establecidos |
| Locale / idioma (otros) | Baja | Opcional, más adelante |
