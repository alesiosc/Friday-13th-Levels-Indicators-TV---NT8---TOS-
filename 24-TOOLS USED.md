# Tools Used - NinjaTrader 8 Levels Indicator

## **Date Last Updated:** 2026-01-13 18:59:00

---

## **Development Tools**

| Tool | Purpose |
|------|---------|
| Google Gemini CLI | Pine Script to C# conversion, code generation |
| Google Antigravity | Project status updates, documentation, feature implementation |
| Qwen CLI | Supplementary code assistance |

---

## **Languages & Frameworks**

| Technology | Version | Purpose |
|------------|---------|---------|
| C# | .NET Framework | NinjaTrader 8 indicator development |
| Pine Script | v5 | Original TradingView indicator source |
| NinjaTrader 8 | Latest | Target trading platform |

---

## **Project Libraries**

| Library | Description |
|---------|-------------|
| NinjaTrader.Custom | NT8 custom indicator base classes |
| NinjaTrader.Gui | NT8 drawing tools (lines, boxes, labels) |
| NinjaTrader.Gui.Tools | SimpleFont for text styling (bold, italic) |
| NinjaTrader.Gui.Chart | ChartBars for visible bar range detection |
| System.Collections.Generic | Data structures (arrays, maps) |
| System.Xml.Serialization | XmlIgnore attribute for Brush properties |
| System.Linq | LINQ extensions for array manipulation |

---

## **Original Pine Script Features Converted**

- Set 1-4 level drawing (zones, lines, labels)
- LIS (Institutional Support) levels
- NQ and ES data input parsing
- Label overlap prevention mechanism
- Timestamp display table
- Multiple line styles (solid, dashed, dotted)
- Configurable colors and opacity settings

---

## **NT8-Specific Features Added (2026-01-13)**

| Feature | Description |
|---------|-------------|
| LabelAnchorPosition Enum | Custom enum (Left, Right) for label anchor selection |
| GlobalLabelAnchor Property | Anchors labels to left or right edge of visible chart |
| GetLabelBarsAgo() Helper | Centralized label positioning logic respecting anchor |
| Set1LineWidth/Style | Line width and style properties for Set 1 |
| All Set X Offsets | Horizontal label offset for all sets (Set1-4, LIS) |
