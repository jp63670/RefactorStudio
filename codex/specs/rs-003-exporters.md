# rs-003: Exporters (HTML + DOCX)

## Goal
Add Exporter service to produce self-contained HTML (inline CSS) and a DOCX stub from model output.

## Acceptance
- New services: IExporter, HtmlExporter, DocxExporter (stub using Open XML SDK)
- Given text (Markdown-ish or HTML), HtmlExporter produces outputs/*.html with inline CSS
- DocxExporter creates outputs/*.docx (basic headings/paragraphs OK)
- UI: "Export" button appears after Run Recipe, saving last result as HTML/DOCX
- Unit tests for HtmlExporter (contains <style> and expected headings)

## Steps
1) Add DocumentFormat.OpenXml to Core or App project hosting exporter.
2) Implement HtmlExporter (simple CSS + write file).
3) Implement DocxExporter (very basic).
4) Wire DI and add button to UI.
