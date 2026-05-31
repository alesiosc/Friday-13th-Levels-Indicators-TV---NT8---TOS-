# NT8 Friday 13th Levels Indicator - Handoff

## Project Location
- **Source file**: D:\MyPythonProjects_2\NINJATRADER_8_Levels_Indicator\Friday 13th\NT8_Friday13th_Levels.cs
- **NT8 destination**: C:\Users\Cameron\Documents\NinjaTrader 8\bin\Custom\Indicators\Friday 13th\NT8_Friday13th_Levels.cs
- **Data file**: C:\Users\Cameron\Documents\NinjaTrader 8\bin\Custom\Indicators\Friday 13th\levels.txt
- **Project root**: D:\MyPythonProjects_2\NINJATRADER_8_Levels_Indicator
- **NT8 version**: 8.1.7.0 64-bit

## Current Features
- Parses `NQ SET X:`, `ES SET X:`, `YM SET X:` headers from data blob
- Supports SET 1-8 + LIS (Liquidity Imbalance Separation)
- **SET 8 (Zulu)** newly added: parses `Zulu VOL-upper,lower`, `Zulu OI-upper,lower`, `Zulu Gamma Flip-value` as dashed lines with labels
- Auto-detects chart instrument (NQ/ES/YM including micros)
- File loading via `AutoLoadFromFile=true` with `FullLevelsFilePath`
- 7 drawing sets + LIS + Zulu levels, each with independent styling

## UNRESOLVED ISSUE: Friday 13th Folder in Indicator Picker

**Problem**: NT8 8.1.7.0 does NOT have a `Folder` property on the `Indicator` base class. The following all fail to compile:
- `Folder = "Friday 13th";` → CS0103
- `this.Folder = "Friday 13th";` → CS1061
- `public override string Folder => "Friday 13th";` → CS0115
- `[Category("Friday 13th")]` on class → compiles but has no effect

**What other NT8 vendors do**: They ship compiled DLL assemblies. When a DLL is in `bin/Custom/`, NT8 reads metadata and creates folders automatically. For .cs NinjaScript files, NT8 8.1.7.0 doesn't support custom folders in the chart indicator picker via code.

**What works**: The file lives in a `Friday 13th\` subfolder under `Custom\Indicators\`, which creates a folder in the NinjaScript Editor tree. The indicator appears in the chart picker alphabetically under "NT8 - FRIDAY 13th Levels".

## Other Notes
- Zulu labels use `yPixelOffset = -14` (above the line) like SET 3
- Zulu has NO zone/rectangle support (lines only + optional labels)
- No "Master toggle" checkbox exists yet (HANDOFF v1 mentioned as desired)
- `levels.txt` contains ALL sets for NQ, ES, YM with the original disclaimer text

## To Rerun
- Compile within NT8 NinjaScript Editor (F5)
- Add to chart via Indicators dialog (alphabetical under "N")
- Leave All Levels Data empty — uses file loading by default
