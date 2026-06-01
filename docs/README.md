# docs — Documentación de test cases y bugs

## Cómo pasarme tus documentos

En el chat solo podés adjuntar imágenes, pero hay opciones mejores para compartir
documentos con texto:

1. **Copiá los archivos a esta carpeta** (`docs/`) y avisame el nombre. Puedo leer:
   - `.pdf`  (recomendado para tus documentos armados)
   - `.md`, `.txt`
   - imágenes `.png`, `.jpg` (capturas de los documentos)

   > Si tenés `.docx`, exportalo a **PDF** o pegá el texto: el formato Word binario
   > no se lee bien directamente.

2. **Pegá el texto** directamente en el chat.

3. **Adjuntá capturas** de los documentos como imágenes en el chat.

## Estructura

```
docs/
├── requirements/    # requerimientos de mejora (IMPROV-REQ-*)
├── test-cases/      # casos de prueba (PDF/MD)
├── test-data/       # datos de prueba centralizados (JSON)
└── bugs/            # descripción y pasos de reproducción de cada bug
```

## Requerimientos de mejora

| ID | Documento | Export Word/PDF |
|----|-----------|-----------------|
| IMPROV-REQ-001 | [IMPROV-REQ-001-location-caba-api-combobox.md](requirements/IMPROV-REQ-001-location-caba-api-combobox.md) | [Requerimiento = Argentina-API.pdf](requirements/Requerimiento%20=%20Argentina-API.pdf) · regenerar: `pwsh tools/export-requerimiento-argentina-api.ps1` |
