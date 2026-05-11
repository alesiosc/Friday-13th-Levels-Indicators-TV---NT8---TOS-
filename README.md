# Friday 13th NQ+ES+YM SET 1-7+BKBrown - TradingView Pine Script

**Version:** v13.5 (Single Input Variant)

TradingView indicator that plots support/resistance levels, zones, and BKBrown liquidity labels for NQ, ES, and YM futures.

### Files
- **`Friday_13th_NQ_ES_SET_1-3_BKBrown_v13.5.pine`** — Original multi-input version (NQ+ES, 4 sets per symbol)
- **`Friday_13th_NQ_SET_1-7_BKBrown_Single_Input_v13.5.pine`** — Simplified single-input variant with:
  - One `All Levels Data` input field for all sets (supports NQ/ES/YM ticker prefixes)
  - Separate `Set 3` input for timestamp/levels format (`[TICKER] Time: ... | Mid/Lower/Upper`)
  - Per-set styling controls (label size, offset, colors, zone opacity, line width/style)
  - Sets 1, 2, 4 (BKBrown), 5 (Extremes), 6 (Scalping), 7 (Confluence) + LIS support

### Usage
Paste level data with headers like `NQ SET 1 (10):` or `ES SET 2` or `YM SET 5:` into the `All Levels Data` input. Set 3 uses the format `[ES] Time: 2026-05-11 11:29:00 | Mid: 7445, Lower: 7420, Upper: 7469`.
