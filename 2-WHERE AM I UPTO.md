### **Date:** 2026-05-31

## **Project Status:**

- Indicator compiles clean in NT8 8.1.7.0
- All 8 sets + LIS drawing working
- SET 8 (Zulu) added with custom VOL/OI/Gamma Flip parsing
- Labels offset above lines (-14 yPixelOffset)
- File auto-loading enabled by default
- levels.txt has full NQ+ES+YM data

## **Unresolved:**

1. **Friday 13th folder in chart picker** — NT8 8.1.7.0 lacks `Folder` property on `Indicator` base. Only appears alphabetically. DLL-based approach or NT8 update needed.
2. **Master toggle checkbox** — user wanted a single checkbox to enable/disable all zones/labels (not implemented)
