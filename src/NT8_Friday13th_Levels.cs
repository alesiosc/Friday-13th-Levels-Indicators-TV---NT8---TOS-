#region Using declarations
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Windows.Media;
using System.Xml.Serialization;
using System.IO;
using NinjaTrader.Cbi;
using NinjaTrader.Gui;
using NinjaTrader.Gui.Chart;
using NinjaTrader.Gui.Tools;
using NinjaTrader.Data;
using NinjaTrader.NinjaScript;
using NinjaTrader.NinjaScript.DrawingTools;
using NinjaTrader.NinjaScript.Indicators;
#endregion

namespace NinjaTrader.NinjaScript.Indicators.Friday13th
{
    

    public class NT8_Friday13th_Levels : Indicator
    {
        // CACHE: Static file cache shared across all indicator instances
        private static readonly object cacheLock = new object();
        private static readonly Dictionary<string, CacheEntry> levelCache =
            new Dictionary<string, CacheEntry>(StringComparer.OrdinalIgnoreCase);

        private sealed class CacheEntry
        {
            public DateTime WriteTimeUtc { get; set; }
            public string FileContent { get; set; }
        }

        public enum LabelAnchorPosition
        {
            Left,
            Middle,
            Right
        }

        private List<string> drawingTags;
        private string activeRoot;

        // Data-change detection - skip re-parse when nothing changed
        private string lastAllDataHash;
        private string lastSet3DataHash;

        // Parsed data fields
        private string parsedS1, parsedS2, parsedS4, parsedS5, parsedS6, parsedS7;
        private string parsedZulu;
        private string parsedLIS;
        private string parsedTimestamp;
        private double? parsedMid, parsedLower, parsedUpper;

        // Reusable font to avoid per-zone allocation
        private SimpleFont cachedFont;

        private int cachedFontSize;

        private Brush MakeFrozenBrush(Color color)
        {
            var b = new SolidColorBrush(color);
            b.Freeze();
            return b;
        }

        private Brush MakeBrushWithOpacity(Brush baseBrush, int opacity)
        {
            SolidColorBrush scb = baseBrush as SolidColorBrush;
            if (scb != null)
            {
                Color c = scb.Color;
                int alpha = (int)Math.Round(255.0 * Math.Max(0, Math.Min(100, opacity)) / 100.0);
                SolidColorBrush b = new SolidColorBrush(Color.FromArgb((byte)alpha, c.R, c.G, c.B));
                b.Freeze();
                return b;
            }
            return baseBrush;
        }


        // =====================================================================
        // Resolve chart instrument to canonical root symbol
        // =====================================================================
        private string GetChartRootSymbol()
        {
            string raw    = Instrument?.MasterInstrument?.Name ?? string.Empty;
            string ticker = raw.Trim().ToUpperInvariant();
            if (ticker.StartsWith("MNQ")) return "NQ";
            if (ticker.StartsWith("MES")) return "ES";
            if (ticker.StartsWith("MYM")) return "YM";
            if (ticker.StartsWith("NQ"))  return "NQ";
            if (ticker.StartsWith("ES"))  return "ES";
            if (ticker.StartsWith("YM"))  return "YM";
            return null;
        }

        // =====================================================================
        // FONT HELPER - reuse font objects instead of per-zone allocation
        private SimpleFont GetFont(int size)
        {
            if (cachedFont == null || cachedFontSize != size)
            {
                cachedFont = new SimpleFont("Arial", size);
                cachedFontSize = size;
            }
            return cachedFont;
        }

        // DATA CHANGE DETECTION - avoid re-parsing if nothing changed
        private static string ComputeHash(string input)
        {
            if (string.IsNullOrEmpty(input)) return "";
            if (input.Length <= 64) return input;
            ulong sum = 0;
            for (int i = 0; i < input.Length; i++) sum += (ulong)input[i];
            return input.Length + ":" + input.Substring(0, 32) + ":" + input.Substring(input.Length - 32) + ":" + sum;
        }

        // State change
        // =====================================================================
        protected override void OnStateChange()
        {
            if (State == State.SetDefaults)
            {
                Description              = "NT8 - FRIDAY 13th Levels | Auto-loads from file, parses NQ/ES/YM SET 1-8 headers.";
                Name                     = "NT8 - FRIDAY 13th Levels";
                Calculate                = Calculate.OnBarClose;
                IsOverlay                = true;
                DisplayInDataBox         = false;
                DrawOnPricePanel         = true;
                ScaleJustification       = ScaleJustification.Right;
                IsSuspendedWhileInactive = true;
                ZOrder                   = -1;

                Set3Data         = "";
                FullLevelsFilePath = @"C:\Users\Cameron\Documents\new 1.txt";
                AutoLoadFromFile   = true;

                // Show/hide
                S1ShowZones = false; S1ShowLabels = false;
                S2ShowZones = true;  S2ShowLabels = true;
                S3ShowKeyLevels = true; S3ShowKeyLabels = true; S3ShowTimestamp = false;
                S4ShowZones = true;  S4ShowLabels = true;
                S5ShowZones = true;  S5ShowLabels = true;
                S6ShowZones = false; S6ShowLabels = false;
                S7ShowZones = true;  S7ShowLabels = true;
                // S1 defaults
                S1LabelOffset = -20; S1LabelSize = 8;
                S1LabelBG = MakeFrozenBrush(Color.FromArgb(75, 0x44, 0xa5, 0x48));
                S1LabelTextColor = Brushes.White;
                S1LineColor = MakeFrozenBrush(Color.FromArgb(255, 0x44, 0xa5, 0x48));
                S1ZoneColor = MakeFrozenBrush(Color.FromArgb(75, 0x44, 0xa5, 0x48));
                S1ZoneOpacity = 40; S1LineOpacity = 40; S1LineWidth = 2; S1LineStyle = DashStyleHelper.Solid; S1LabelAnchor = LabelAnchorPosition.Right;

                // S2 defaults
                S2LabelOffset = -32; S2LabelSize = 8;
                S2LabelBG = MakeFrozenBrush(Color.FromArgb(75, 0x63, 0x63, 0x63));
                S2LabelTextColor = Brushes.White;
                S2LineColor = MakeFrozenBrush(Color.FromArgb(255, 0x63, 0x63, 0x63));
                S2ZoneColor = MakeFrozenBrush(Color.FromArgb(75, 0x63, 0x63, 0x63)); S2ZoneOpacity = 70; S2LineOpacity = 100; S2LineWidth = 2; S2LineStyle = DashStyleHelper.Solid; S2LabelAnchor = LabelAnchorPosition.Right;

                // S3 defaults
                S3LabelOffset = 50; S3LabelSize = 8;
                S3LabelBG = MakeFrozenBrush(Color.FromArgb(75, 0xff, 0x98, 0x00));
                S3LabelTextColor = Brushes.White;
                S3MidColor   = Brushes.Orange;    S3LineOpacity = 100; S3MidWidth = 2;   S3MidStyle = DashStyleHelper.Dash;
                S3LowerColor = Brushes.Lime;     S3LowerWidth = 2; S3LowerStyle = DashStyleHelper.Dash;
                S3UpperColor = Brushes.Red;      S3UpperWidth = 2; S3UpperStyle = DashStyleHelper.Dash;
                S3TimestampPosition = TextPosition.BottomLeft; S3LabelAnchor = LabelAnchorPosition.Right;

                // S4 defaults
                S4LabelOffset = -42; S4LabelSize = 8;
                S4LabelBG = MakeFrozenBrush(Color.FromArgb(75, 0xdb, 0xdb, 0xdb));
                S4LabelTextColor = Brushes.White;
                S4LineColor = Brushes.White; S4LineOpacity = 100; S4LineWidth = 2; S4LineStyle = DashStyleHelper.Solid; S4LabelAnchor = LabelAnchorPosition.Right;

                // S5 defaults
                S5LabelOffset = -58; S5LabelSize = 8;
                S5LabelBG = MakeFrozenBrush(Color.FromArgb(75, 0x21, 0x96, 0xf3));
                S5LabelTextColor = Brushes.White;
                S5LineColor = MakeFrozenBrush(Color.FromArgb(255, 0x64, 0xb5, 0xf6));
                S5ZoneColor = MakeFrozenBrush(Color.FromArgb(75, 0x64, 0xb5, 0xf6));
                S5ZoneOpacity = 40; S5LineOpacity = 90; S5LineWidth = 2; S5LineStyle = DashStyleHelper.Solid; S5LabelAnchor = LabelAnchorPosition.Right;

                // S6 defaults
                S6LabelOffset = -74; S6LabelSize = 8;
                S6LabelBG = MakeFrozenBrush(Color.FromArgb(75, 0x9c, 0x27, 0xb0));
                S6LabelTextColor = Brushes.White;
                S6LineColor = MakeFrozenBrush(Color.FromArgb(255, 0x9c, 0x27, 0xb0));
                S6ZoneColor = MakeFrozenBrush(Color.FromArgb(75, 0x9c, 0x27, 0xb0));
                S6ZoneOpacity = 40; S6LineOpacity = 40; S6LineWidth = 2; S6LineStyle = DashStyleHelper.Solid; S6LabelAnchor = LabelAnchorPosition.Right;

                // S7 defaults
                S7LabelOffset = -88; S7LabelSize = 7;
                S7LabelBG = MakeFrozenBrush(Color.FromArgb(75, 0x79, 0x55, 0x48));
                S7LabelTextColor = Brushes.White;
                S7LineColor = MakeFrozenBrush(Color.FromArgb(255, 0xff, 0x98, 0x00));
                S7ZoneColor = MakeFrozenBrush(Color.FromArgb(75, 0xff, 0x98, 0x00));
                S7ZoneOpacity = 40; S7LineOpacity = 40; S7LineWidth = 1; S7LineStyle = DashStyleHelper.Solid; S7LabelAnchor = LabelAnchorPosition.Right;

                // Zulu defaults
                ZuluLabelOffset = 38; ZuluLabelSize = 8;
                ZuluLabelBG = MakeFrozenBrush(Color.FromArgb(75, 0x9c, 0x27, 0xb0));
                ZuluLabelTextColor = Brushes.White;
                ZuluVolColor = MakeFrozenBrush(Color.FromArgb(255, 0xce, 0x93, 0xd8));
                ZuluOiColor = MakeFrozenBrush(Color.FromArgb(255, 0x64, 0xb5, 0xf6));
                ZuluGammaColor = MakeFrozenBrush(Color.FromArgb(255, 0xef, 0x9a, 0x9a));
                ZuluLineOpacity = 100; ZuluLineWidth = 2; ZuluLineStyle = DashStyleHelper.Dash; ZuluLabelAnchor = LabelAnchorPosition.Right; ZuluShowLabels = true; ZuluShowLines = true;

                // LIS defaults
                LISLabelOffset = 32; LISLabelSize = 13;
                LISLabelBG = MakeFrozenBrush(Color.FromArgb(50, 0xff, 0x98, 0x00));
                LISLabelTextColor = Brushes.White;
                LISLineColor = MakeFrozenBrush(Color.FromArgb(30, 0xff, 0x98, 0x00)); LISZoneColor = MakeFrozenBrush(Color.FromArgb(50, 0xff, 0x98, 0x00)); LISZoneOpacity = 80;
                LISLineOpacity = 100; LISLineWidth = 3; LISLineStyle = DashStyleHelper.Solid; LISLabelAnchor = LabelAnchorPosition.Right; LISShowLines = true; LISShowLabels = true;
            }
            else if (State == State.DataLoaded)
            {
                drawingTags  = new List<string>();
            }
            else if (State == State.Terminated)
            {
                ClearPreviousDrawings();
            }
            else if (State == State.Historical)
            {
                SetZOrder(-1);
            }
        }
        // =====================================================================
        // OnBarUpdate
        // =====================================================================
        protected override void OnBarUpdate()
        {
            if (CurrentBar < 1) return;

            // Resolve activeRoot on first real bar (Instrument may not be ready in DataLoaded)
            if (string.IsNullOrEmpty(activeRoot))
            {
                activeRoot = GetChartRootSymbol();

                if (string.IsNullOrEmpty(activeRoot))
                    return;
            }
            if (IsFirstTickOfBar || CurrentBar >= Bars.Count - 2)
            {
                ClearPreviousDrawings();
                ProcessAndDrawLevels();
            }
        }

        private void ClearPreviousDrawings()
        {
            if (drawingTags == null) return;
            foreach (var tag in drawingTags) RemoveDrawObject(tag);
            drawingTags.Clear();
        }



        // =====================================================================
        // PARSER: processAllData - separate _LIS segments from normal
        // =====================================================================
        private void ProcessAllData(string rawData, out string normalData, out string lisData)
        {
            var normalParts = new List<string>();
            var lisParts    = new List<string>();

            if (!string.IsNullOrEmpty(rawData))
            {
                foreach (string seg in rawData.Split(';'))
                {
                    string s = seg.Trim();
                    if (string.IsNullOrEmpty(s)) continue;
                    if (s.Contains("_LIS"))
                        lisParts.Add(s.Replace("_LIS", ""));
                    else
                        normalParts.Add(s);
                }
            }

            normalData = string.Join(";", normalParts);
            lisData    = string.Join(";", lisParts);
        }

        private string AppendData(string existing, string newData)
        {
            if (string.IsNullOrEmpty(newData)) return existing;
            if (string.IsNullOrEmpty(existing)) return newData;
            return existing + ";" + newData;
        }
        // =====================================================================
        // PARSER: parseSectionsInstrumentAware
        // Handles "NQ SET 1:", "ES SET 2:", "YM SET 7:" headers properly
        // Only returns data matching the chartInstrument
        // =====================================================================
        private void ParseSectionsInstrumentAware(string rawData, string chartInstrument)
        {
            parsedS1 = parsedS2 = parsedS4 = parsedS5 = parsedS6 = parsedS7 = "";
            parsedZulu = "";
            parsedLIS = "";

            if (string.IsNullOrEmpty(rawData)) return;

            string cleaned = rawData.Replace("&amp;", ";");
            cleaned = cleaned.Replace("&lt;", "<").Replace("&gt;", ">").Replace("&quot;", "").Replace("&#39;", "'");
            string upper = cleaned.ToUpperInvariant();
            int pos = 0;

            while (pos < cleaned.Length)
            {
                int headerStart = -1;
                string foundInstrument = null;
                int bestPos = int.MaxValue;

                foreach (string instr in new[] { "MNQ", "NQ", "MES", "ES", "MYM", "YM" })
                {
                    int idx = upper.IndexOf(instr + " SET ", pos, StringComparison.Ordinal);
                    if (idx >= 0 && idx < bestPos)
                    {
                        // Validate header boundary: must be at line start or after separator
                        if (idx == 0 || cleaned[idx - 1] == ';' || cleaned[idx - 1] == '|' ||
                            cleaned[idx - 1] == '{' || char.IsWhiteSpace(cleaned[idx - 1]))
                        {
                            bestPos = idx;
                            foundInstrument = instr;
                        }
                    }
                }

                if (bestPos == int.MaxValue || foundInstrument == null)
                    break;
                headerStart = bestPos;

                string rootInstr = foundInstrument;
                if (rootInstr.StartsWith("MNQ")) rootInstr = "NQ";
                else if (rootInstr.StartsWith("MES")) rootInstr = "ES";
                else if (rootInstr.StartsWith("MYM")) rootInstr = "YM";

                int setKeywordEnd = headerStart + foundInstrument.Length + 4;
                if (setKeywordEnd >= cleaned.Length) break;

                int numStart = setKeywordEnd;
                while (numStart < cleaned.Length && cleaned[numStart] == ' ') numStart++;
                int numEndPos = numStart;
                while (numEndPos < cleaned.Length && char.IsDigit(cleaned[numEndPos])) numEndPos++;

                if (numEndPos == numStart) { pos = setKeywordEnd; continue; }
                int foundSetNum;
                if (!int.TryParse(cleaned.Substring(numStart, numEndPos - numStart), out foundSetNum)) { pos = setKeywordEnd; continue; }
                if (foundSetNum < 1 || foundSetNum > 8) { pos = setKeywordEnd; continue; }

                int dataScanStart = numEndPos;
                while (dataScanStart < cleaned.Length && cleaned[dataScanStart] == ' ') dataScanStart++;
                if (dataScanStart < cleaned.Length && cleaned[dataScanStart] == ':')
                    dataScanStart++;
                // Skip parenthetical content like (10), (Zulu), (EXTREMES), etc.
                while (dataScanStart < cleaned.Length && cleaned[dataScanStart] == '(')
                {
                    dataScanStart++;
                    while (dataScanStart < cleaned.Length && cleaned[dataScanStart] != ')')
                        dataScanStart++;
                    if (dataScanStart < cleaned.Length) dataScanStart++; // skip ')'
                    while (dataScanStart < cleaned.Length && cleaned[dataScanStart] == ' ') dataScanStart++;
                    if (dataScanStart < cleaned.Length && cleaned[dataScanStart] == ':')
                        dataScanStart++;
                    while (dataScanStart < cleaned.Length && cleaned[dataScanStart] == ' ') dataScanStart++;
                }

                int dataStart = -1;
                for (int j = dataScanStart; j < cleaned.Length; j++)
                {
                    char c = cleaned[j];
                    if (char.IsDigit(c) || (c == '-' && j + 1 < cleaned.Length && char.IsDigit(cleaned[j + 1])))
                    {
                        dataStart = j;
                        break;
                    }
                }

                if (dataStart < 0) { pos = setKeywordEnd; continue; }

                int dataEnd = cleaned.Length;
                foreach (string instr2 in new[] { "MNQ", "NQ", "MES", "ES", "MYM", "YM" })
                {
                    int nextHeader = upper.IndexOf(instr2 + " SET ", dataStart);
                    if (nextHeader > headerStart && nextHeader < dataEnd)
                        dataEnd = nextHeader;
                }

                string dataPart;
                if (foundSetNum == 8) // Zulu uses text prefixes, include everything from dataScanStart
                {
                    dataPart = cleaned.Substring(dataScanStart, dataEnd - dataScanStart).Trim();
                }
                else
                {
                    dataPart = cleaned.Substring(dataStart, dataEnd - dataStart).Trim();
                }
                while (dataPart.EndsWith(";"))
                    dataPart = dataPart.Substring(0, dataPart.Length - 1);

                if (rootInstr == chartInstrument)
                {
                    string sectionData, sectionLis;
                    ProcessAllData(dataPart, out sectionData, out sectionLis);

                    switch (foundSetNum)
                    {
                        case 1: parsedS1 = AppendData(parsedS1, sectionData); break;
                        case 2: parsedS2 = AppendData(parsedS2, sectionData); break;
                        case 3: Set3Data = AppendData(Set3Data, dataPart); break;
                        case 4: parsedS4 = AppendData(parsedS4, sectionData); break;
                        case 5: parsedS5 = AppendData(parsedS5, sectionData); break;
                        case 6: parsedS6 = AppendData(parsedS6, sectionData); break;
                        case 7: parsedS7 = AppendData(parsedS7, sectionData); break;
                        case 8: parsedZulu = AppendData(parsedZulu, dataPart); break;
                    }
                    parsedLIS = AppendData(parsedLIS, sectionLis);
                }

                pos = dataEnd;
            }
        }
        // =====================================================================
        // PARSER: parseSet3Data - ported from Pine
        // =====================================================================
        private void ParseSet3Data(string rawData, string chartInstrument)
        {
            parsedTimestamp = null;
            parsedMid = parsedLower = parsedUpper = null;

            if (string.IsNullOrEmpty(rawData)) return;

            string cleaned = rawData.Replace("&amp;", ";");
            cleaned = cleaned.Replace("&lt;", "<").Replace("&gt;", ">").Replace("&quot;", "").Replace("&#39;", "'");
            string upperCleaned = cleaned.ToUpperInvariant();
            bool matchesChart = false;

            if (upperCleaned.Contains("NQ") || upperCleaned.Contains("ES") || upperCleaned.Contains("YM") ||
                upperCleaned.Contains("MNQ") || upperCleaned.Contains("MES") || upperCleaned.Contains("MYM"))
            {
                string[] pipeCheck = cleaned.Split('|');
                if (pipeCheck.Length > 0)
                {
                    string timePart = pipeCheck[0].ToUpperInvariant();
                    if (chartInstrument == "NQ" && (timePart.Contains("MNQ") || timePart.Contains("NQ"))) matchesChart = true;
                    else if (chartInstrument == "ES" && (timePart.Contains("MES") || timePart.Contains("ES"))) matchesChart = true;
                    else if (chartInstrument == "YM" && (timePart.Contains("MYM") || timePart.Contains("YM"))) matchesChart = true;
                }
            }
            else
            {
                matchesChart = true;
            }

            if (!matchesChart) return;

            string[] parts = cleaned.Split('|');
            if (parts.Length > 0)
            {
                string timePart = parts[0];
                int timePos = timePart.IndexOf("Time:", StringComparison.OrdinalIgnoreCase);
                if (timePos >= 0)
                    parsedTimestamp = timePart.Substring(timePos + 5).Trim();
            }

            if (parts.Length > 1)
            {
                foreach (string item in parts[1].Split(','))
                {
                    string[] kv = item.Split(':');
                    if (kv.Length < 2) continue;
                    string key = kv[0].Trim();
                    string valStr = kv[1].Trim();
                    switch (key)
                    {
                        case "Mid":
                            double mv; if (double.TryParse(valStr, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out mv)) parsedMid = mv;
                            break;
                        case "Lower":
                            double lv; if (double.TryParse(valStr, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out lv)) parsedLower = lv;
                            break;
                        case "Upper":
                            double uv; if (double.TryParse(valStr, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out uv)) parsedUpper = uv;
                            break;
                    }
                }
            }
        }

        // =====================================================================
        // File loading
        // =====================================================================
        private string LoadFileContent()
        {
            if (string.IsNullOrEmpty(FullLevelsFilePath) || !File.Exists(FullLevelsFilePath))
                return "";

            try
            {
                string fullPath = Path.GetFullPath(FullLevelsFilePath);
                DateTime writeTime = File.GetLastWriteTimeUtc(fullPath);

                lock (cacheLock)
                {
                    CacheEntry entry;
                    if (levelCache.TryGetValue(fullPath, out entry) &&
                        entry != null &&
                        entry.WriteTimeUtc == writeTime &&
                        entry.FileContent != null)
                    {
                        return entry.FileContent;
                    }

                    string fileContent = File.ReadAllText(fullPath);
                    fileContent = fileContent.TrimStart(new char[] { '\ufeff' });
                    levelCache[fullPath] = new CacheEntry
                    {
                        WriteTimeUtc = writeTime,
                        FileContent = fileContent
                    };
                    return fileContent;
                }
            }
            catch (Exception ex)
            {
                Print("Error loading levels file: " + ex.Message);
                return "";
            }
        }
        // =====================================================================
        // Master draw dispatcher
        // =====================================================================
        private void ProcessAndDrawLevels()
        {
            string root = activeRoot;
            if (root == null)
                return;

            // Load from file
            string combined = "";
            if (AutoLoadFromFile)
            {
                combined = LoadFileContent();
            }

            // Data-change detection: only re-parse if input actually changed
            string combinedHash = ComputeHash(combined);
            string set3Hash     = ComputeHash(Set3Data ?? "");

            if (lastAllDataHash == null || combinedHash != lastAllDataHash)
            {
                ParseSectionsInstrumentAware(combined, root);
                lastAllDataHash = combinedHash;


            }

            // Also parse Set3 data that came from file (accumulated in Set3Data)
            if (lastSet3DataHash == null || set3Hash != lastSet3DataHash)
            {
                ParseSet3Data(Set3Data, root);
                lastSet3DataHash = set3Hash;

            }

            int lookback = Math.Min(500, CurrentBar);
            // Ensure startTime is valid
            if (CurrentBar < 1) return;
            DateTime startTime = Time[Math.Min(CurrentBar, lookback)];
            DateTime endTime   = Time[0].AddDays(10);

            if ((S1ShowZones || S1ShowLabels) && !string.IsNullOrWhiteSpace(parsedS1))
                DrawSet1(parsedS1, S1ShowZones, S1ShowLabels, root, startTime, endTime);
            if ((S2ShowZones || S2ShowLabels) && !string.IsNullOrWhiteSpace(parsedS2))
                DrawSet2(parsedS2, S2ShowZones, S2ShowLabels, root, startTime, endTime);
            if (parsedMid.HasValue || parsedLower.HasValue || parsedUpper.HasValue)
                DrawSet3(root, startTime, endTime);
            if ((S4ShowZones || S4ShowLabels) && !string.IsNullOrWhiteSpace(parsedS4))
                DrawSet4(parsedS4, S4ShowZones, S4ShowLabels, root, startTime, endTime);
            if ((S5ShowZones || S5ShowLabels) && !string.IsNullOrWhiteSpace(parsedS5))
                DrawSet5(parsedS5, S5ShowZones, S5ShowLabels, root, startTime, endTime);
            if ((S6ShowZones || S6ShowLabels) && !string.IsNullOrWhiteSpace(parsedS6))
                DrawSet6(parsedS6, S6ShowZones, S6ShowLabels, root, startTime, endTime);
            if ((S7ShowZones || S7ShowLabels) && !string.IsNullOrWhiteSpace(parsedS7))
                DrawSet7(parsedS7, S7ShowZones, S7ShowLabels, root, startTime, endTime);
            if (!string.IsNullOrWhiteSpace(parsedZulu))
                DrawZulu(parsedZulu, ZuluShowLines, ZuluShowLabels, startTime, endTime);
            if ((LISShowLines || LISShowLabels) && !string.IsNullOrWhiteSpace(parsedLIS))
                DrawLIS(parsedLIS, LISShowLines, LISShowLabels, startTime, endTime);
        }
        private System.Windows.TextAlignment GetTextAlignment(LabelAnchorPosition anchor)
        {
            switch (anchor)
            {
                case LabelAnchorPosition.Left:
                    return System.Windows.TextAlignment.Left;
                case LabelAnchorPosition.Middle:
                    return System.Windows.TextAlignment.Center;
                default:
                    return System.Windows.TextAlignment.Right;
            }
        }

        private int GetLabelBarsAgoForAnchor(LabelAnchorPosition anchor, int totalBars, int labelOffset)
        {
            int barsAgo;
            switch (anchor)
            {
                case LabelAnchorPosition.Left:
                    barsAgo = Math.Max(0, Math.Min(500, totalBars) - labelOffset);
                    break;
                case LabelAnchorPosition.Middle:
                    barsAgo = Math.Max(0, Math.Min(250, totalBars) - labelOffset);
                    break;
                default:
                    barsAgo = labelOffset;
                    break;
            }
            // Clamp to valid range (cannot exceed current bar count)
            return Math.Min(barsAgo, Math.Max(0, totalBars - 1));
        }

        // =====================================================================
        // DRAWING: Set 1 (Zones) - 6-part comma format
        // =====================================================================
        private void DrawSet1(string rawData, bool showZones, bool showLabels, string symbol,
            DateTime startTime, DateTime endTime)
        {
            if ((!showZones && !showLabels) || string.IsNullOrEmpty(rawData)) return;

            foreach (string z in rawData.Split(';'))
            {
                string trimmed = z.Trim();
                if (string.IsNullOrEmpty(trimmed)) continue;

                string[] p = trimmed.Split(',');
                if (p.Length < 6) continue;

                double pTop, pBottom;
                double t1 = 0, t2 = 0, o1 = 0, o2 = 0;
                if (!double.TryParse(p[0], System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out pTop)) continue;
                if (!double.TryParse(p[5], System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out pBottom)) continue;
                double.TryParse(p[1].Replace("T1=", "").Replace("T1", ""), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out t1);
                double.TryParse(p[2].Replace("T2=", "").Replace("T2", ""), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out t2);
                double.TryParse(p[3].Replace("O1=", "").Replace("O1", ""), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out o1);
                double.TryParse(p[4].Replace("O2=", "").Replace("O2", ""), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out o2);

                double hi  = Math.Max(pTop, pBottom);
                double lo  = Math.Min(pTop, pBottom);
                double mid = (hi + lo) / 2.0;

                if (showZones)
                {
                    if (Math.Abs(hi - lo) < 1)
                    {
                        string tag = "S1L_" + symbol + "_" + pTop.ToString("F2");
                        Draw.Line(this, tag, false, startTime, mid, endTime, mid, MakeBrushWithOpacity(S1LineColor, S1LineOpacity), S1LineStyle, S1LineWidth);
                        drawingTags.Add(tag);
                    }
                    else
                    {
                        string tag = "S1B_" + symbol + "_" + pTop.ToString("F2");
                        Draw.Rectangle(this, tag, false, startTime, hi, endTime, lo, S1ZoneColor, S1ZoneColor, S1ZoneOpacity);
                        drawingTags.Add(tag);
                    }
                }

                if (showLabels)
                {
                    double sum = t1 + t2 + o1 + o2;
                    string lineOne = "T1=" + t1 + ",T2=" + t2 + ",O1=" + o1 + ",O2=" + o2 + ", = " + sum;
                    string lineTwo = hi.ToString("F2") + " - " + lo.ToString("F2") + " = " + (int)Math.Round(hi - lo);
                    string fullText = lineOne + System.Environment.NewLine + lineTwo;
                    var font = GetFont(S1LabelSize);
                    Draw.Text(this, "S1LB_" + symbol + "_" + pTop.ToString("F2"), false, fullText, GetLabelBarsAgoForAnchor(S1LabelAnchor, CurrentBar, S1LabelOffset), mid, 0, S1LabelTextColor, font, GetTextAlignment(S1LabelAnchor), S1LabelBG, Brushes.Transparent, 0);
                    drawingTags.Add("S1LB_" + symbol + "_" + pTop.ToString("F2"));
                }
            }
        }
        // =====================================================================
        // DRAWING: Set 2 (Levels) - comma pairs or single values
        // =====================================================================
        private void DrawSet2(string rawData, bool showZones, bool showLabels, string symbol,
            DateTime startTime, DateTime endTime)
        {
            if ((!showZones && !showLabels) || string.IsNullOrEmpty(rawData)) return;

            foreach (string seg in rawData.Split(';'))
            {
                string trimmed = seg.Trim();
                if (string.IsNullOrEmpty(trimmed)) continue;

                IFormatProvider fp = System.Globalization.CultureInfo.InvariantCulture;

                if (trimmed.Contains(","))
                {
                    string[] p = trimmed.Split(',');
                    double top, bottom;
                    if (!double.TryParse(p[0], System.Globalization.NumberStyles.Any, fp, out top)) continue;
                    if (!double.TryParse(p[1], System.Globalization.NumberStyles.Any, fp, out bottom)) continue;

                    double hi = Math.Max(top, bottom);
                    double lo = Math.Min(top, bottom);
                    double midp = (hi + lo) / 2.0;

                    if (showZones)
                    {
                        string tag = "S2B_" + symbol + "_" + top.ToString("F2");
                        Draw.Rectangle(this, tag, false, startTime, hi, endTime, lo, S2ZoneColor, S2ZoneColor, S2ZoneOpacity);
                        drawingTags.Add(tag);
                    }
                    if (showLabels)
                    {
                        string lblStr = lo.ToString("F2") + " - " + hi.ToString("F2");
                        var font = GetFont(S2LabelSize);
                        Draw.Text(this, "S2LB_" + symbol + "_" + top.ToString("F2"), false, lblStr, GetLabelBarsAgoForAnchor(S2LabelAnchor, CurrentBar, S2LabelOffset), midp, -14, S2LabelTextColor, font, GetTextAlignment(S2LabelAnchor), S2LabelBG, Brushes.Transparent, 0);
                        drawingTags.Add("S2LB_" + symbol + "_" + top.ToString("F2"));
                    }
                }
                else
                {
                    double level;
                    if (!double.TryParse(trimmed, System.Globalization.NumberStyles.Any, fp, out level)) continue;

                    if (showZones)
                    {
                        string tag = "S2L_" + symbol + "_" + level.ToString("F2");
                        Draw.Line(this, tag, false, startTime, level, endTime, level, MakeBrushWithOpacity(S2LineColor, S2LineOpacity), S2LineStyle, S2LineWidth);
                        drawingTags.Add(tag);
                    }
                    if (showLabels)
                    {
                        string lblStr = level.ToString("F2");
                        var font = GetFont(S2LabelSize);
                        Draw.Text(this, "S2LB_" + symbol + "_" + level.ToString("F2"), false, lblStr, GetLabelBarsAgoForAnchor(S2LabelAnchor, CurrentBar, S2LabelOffset), level, -14, S2LabelTextColor, font, GetTextAlignment(S2LabelAnchor), S2LabelBG, Brushes.Transparent, 0);
                        drawingTags.Add("S2LB_" + symbol + "_" + level.ToString("F2"));
                    }
                }
            }
        }
        // =====================================================================
        // DRAWING: Set 3 (Timestamp) - pipe format + TextFixed corner display
        // =====================================================================
        private void DrawSet3(string symbol, DateTime startTime, DateTime endTime)
        {
            if (S3ShowTimestamp && !string.IsNullOrEmpty(parsedTimestamp))
            {
                string tsText = "Timestamp: " + parsedTimestamp;
                var tsFont = GetFont(S3LabelSize);
                Draw.TextFixed(this, "TS_" + symbol, tsText, S3TimestampPosition, S3LabelTextColor, tsFont, S3LabelBG, Brushes.Transparent, 50);
                drawingTags.Add("TS_" + symbol);
            }

            if (!S3ShowKeyLevels && !S3ShowKeyLabels) return;

            if (parsedMid.HasValue)
            {
                Draw.Line(this, "S3M_" + symbol, false, startTime, parsedMid.Value, endTime, parsedMid.Value, MakeBrushWithOpacity(S3MidColor, S3LineOpacity), S3MidStyle, S3MidWidth);
                drawingTags.Add("S3M_" + symbol);
                if (S3ShowKeyLabels)
                {
                    var font = GetFont(S3LabelSize);
                    Draw.Text(this, "S3ML_" + symbol, false, "Mid", GetLabelBarsAgoForAnchor(S3LabelAnchor, CurrentBar, S3LabelOffset), parsedMid.Value, -14, S3LabelTextColor, font, GetTextAlignment(S3LabelAnchor), S3LabelBG, Brushes.Transparent, 0);
                    drawingTags.Add("S3ML_" + symbol);
                }
            }
            if (parsedLower.HasValue)
            {
                Draw.Line(this, "S3L_" + symbol, false, startTime, parsedLower.Value, endTime, parsedLower.Value, MakeBrushWithOpacity(S3LowerColor, S3LineOpacity), S3LowerStyle, S3LowerWidth);
                drawingTags.Add("S3L_" + symbol);
                if (S3ShowKeyLabels)
                {
                    var font = GetFont(S3LabelSize);
                    Draw.Text(this, "S3LL_" + symbol, false, "Lower", GetLabelBarsAgoForAnchor(S3LabelAnchor, CurrentBar, S3LabelOffset), parsedLower.Value, -14, S3LabelTextColor, font, GetTextAlignment(S3LabelAnchor), S3LabelBG, Brushes.Transparent, 0);
                    drawingTags.Add("S3LL_" + symbol);
                }
            }
            if (parsedUpper.HasValue)
            {
                Draw.Line(this, "S3U_" + symbol, false, startTime, parsedUpper.Value, endTime, parsedUpper.Value, MakeBrushWithOpacity(S3UpperColor, S3LineOpacity), S3UpperStyle, S3UpperWidth);
                drawingTags.Add("S3U_" + symbol);
                if (S3ShowKeyLabels)
                {
                    var font = GetFont(S3LabelSize);
                    Draw.Text(this, "S3UL_" + symbol, false, "Upper", GetLabelBarsAgoForAnchor(S3LabelAnchor, CurrentBar, S3LabelOffset), parsedUpper.Value, -14, S3LabelTextColor, font, GetTextAlignment(S3LabelAnchor), S3LabelBG, Brushes.Transparent, 0);
                    drawingTags.Add("S3UL_" + symbol);
                }
            }
        }
        // =====================================================================
        // DRAWING: Set 4 (BKBrown) - dash-separated price-label
        // =====================================================================
        private void DrawSet4(string rawData, bool showZones, bool showLabels, string symbol,
            DateTime startTime, DateTime endTime)
        {
            if ((!showZones && !showLabels) || string.IsNullOrEmpty(rawData)) return;

            IFormatProvider fp = System.Globalization.CultureInfo.InvariantCulture;

            foreach (string seg in rawData.Split(';'))
            {
                string trimmed = seg.Trim();
                if (string.IsNullOrEmpty(trimmed)) continue;

                string prc = trimmed, lbl = trimmed;
                // Smart dash split: find first dash NOT part of a negative number
                // Fix: "-29300-HvolC" should parse as price=-29300, label=HvolC
                if (trimmed.Contains("-"))
                {
                    // Try each dash position: prefer the RIGHTMOST dash that gives a valid price
                    for (int di = trimmed.Length - 1; di >= 1; di--)
                    {
                        if (trimmed[di] == '-')
                        {
                            string testPrc = trimmed.Substring(0, di).TrimEnd();
                            double testVal;
                            if (double.TryParse(testPrc, System.Globalization.NumberStyles.Any,
                                System.Globalization.CultureInfo.InvariantCulture, out testVal))
                            {
                                // Only split if the label part is NOT a valid number (avoids "1-2" range split)
                                string testLbl = trimmed.Substring(di + 1).TrimStart();
                                double trash;
                                if (!double.TryParse(testLbl, System.Globalization.NumberStyles.Any,
                                    System.Globalization.CultureInfo.InvariantCulture, out trash))
                                {
                                    prc = testPrc;
                                    lbl = testLbl;
                                    break;
                                }
                            }
                        }
                    }
                }

                double level;
                if (!double.TryParse(prc, System.Globalization.NumberStyles.Any, fp, out level))
                    continue;

                // Insert spaces before capital letters in labels (CamelCase → Camel Case)
                string spacedLabel = "";
                for (int i = 0; i < lbl.Length; i++)
                {
                    if (i > 0 && char.IsUpper(lbl[i]))
                    {
                        if (char.IsLower(lbl[i - 1]))
                            spacedLabel += " ";
                        else if (i + 1 < lbl.Length && char.IsLower(lbl[i + 1]) && char.IsUpper(lbl[i - 1]))
                            spacedLabel += " ";
                    }
                    spacedLabel += lbl[i];
                }

                if (showZones)
                {
                    string tag = "S4L_" + symbol + "_" + level.ToString("F2");
                    Draw.Line(this, tag, false, startTime, level, endTime, level, MakeBrushWithOpacity(S4LineColor, S4LineOpacity), S4LineStyle, S4LineWidth);
                    drawingTags.Add(tag);
                }
                if (showLabels)
                {
                    var font = GetFont(S4LabelSize);
                    Draw.Text(this, "S4LB_" + symbol + "_" + level.ToString("F2"), false, spacedLabel, GetLabelBarsAgoForAnchor(S4LabelAnchor, CurrentBar, S4LabelOffset), level, -14, S4LabelTextColor, font, GetTextAlignment(S4LabelAnchor), S4LabelBG, Brushes.Transparent, 0);
                    drawingTags.Add("S4LB_" + symbol + "_" + level.ToString("F2"));
                }
            }
        }

        // =====================================================================
        // DRAWING: Set 5 (reuses Set 1 zone format with own styles)
        // =====================================================================
        private void DrawSet5(string rawData, bool showZones, bool showLabels, string symbol,
            DateTime startTime, DateTime endTime)
        {
            if ((!showZones && !showLabels) || string.IsNullOrEmpty(rawData)) return;
            IFormatProvider fp = System.Globalization.CultureInfo.InvariantCulture;

            foreach (string z in rawData.Split(';'))
            {
                string trimmed = z.Trim();
                if (string.IsNullOrEmpty(trimmed)) continue;
                string[] p = trimmed.Split(',');
                if (p.Length < 6) continue;
                double pTop, pBottom;
                if (!double.TryParse(p[0], System.Globalization.NumberStyles.Any, fp, out pTop)) continue;
                if (!double.TryParse(p[5], System.Globalization.NumberStyles.Any, fp, out pBottom)) continue;
                double t1 = 0, t2 = 0, o1 = 0, o2 = 0;
                double.TryParse(p[1].Replace("T1=", ""), System.Globalization.NumberStyles.Any, fp, out t1);
                double.TryParse(p[2].Replace("T2=", ""), System.Globalization.NumberStyles.Any, fp, out t2);
                double.TryParse(p[3].Replace("O1=", ""), System.Globalization.NumberStyles.Any, fp, out o1);
                double.TryParse(p[4].Replace("O2=", ""), System.Globalization.NumberStyles.Any, fp, out o2);
                double hi = Math.Max(pTop, pBottom);
                double lo = Math.Min(pTop, pBottom);
                double mid = (hi + lo) / 2.0;

                if (showZones)
                {
                    if (Math.Abs(hi - lo) < 1)
                    {
                        string tag = "S5L_" + symbol + "_" + pTop.ToString("F2");
                        Draw.Line(this, tag, false, startTime, mid, endTime, mid, MakeBrushWithOpacity(S5LineColor, S5LineOpacity), S5LineStyle, S5LineWidth);
                        drawingTags.Add(tag);
                    }
                    else
                    {
                        string tag = "S5B_" + symbol + "_" + pTop.ToString("F2");
                        Draw.Rectangle(this, tag, false, startTime, hi, endTime, lo, S5ZoneColor, S5ZoneColor, S5ZoneOpacity);
                        drawingTags.Add(tag);
                    }
                }
                if (showLabels)
                {
                    double sum = t1 + t2 + o1 + o2;
                    string lineOne = "T1=" + t1 + ",T2=" + t2 + ",O1=" + o1 + ",O2=" + o2 + ", = " + sum;
                    string lineTwo = hi.ToString("F2") + " - " + lo.ToString("F2") + " = " + (int)Math.Round(hi - lo);
                    string fullText = lineOne + System.Environment.NewLine + lineTwo;
                    var font = GetFont(S5LabelSize);
                    Draw.Text(this, "S5LB_" + symbol + "_" + pTop.ToString("F2"), false, fullText, GetLabelBarsAgoForAnchor(S5LabelAnchor, CurrentBar, S5LabelOffset), mid, 0, S5LabelTextColor, font, GetTextAlignment(S5LabelAnchor), S5LabelBG, Brushes.Transparent, 0);
                    drawingTags.Add("S5LB_" + symbol + "_" + pTop.ToString("F2"));
                }
            }
        }
        // =====================================================================
        // DRAWING: Set 6 (reuses Set 1 zone format with own styles)
        // =====================================================================
        private void DrawSet6(string rawData, bool showZones, bool showLabels, string symbol,
            DateTime startTime, DateTime endTime)
        {
            if ((!showZones && !showLabels) || string.IsNullOrEmpty(rawData)) return;
            IFormatProvider fp = System.Globalization.CultureInfo.InvariantCulture;

            foreach (string z in rawData.Split(';'))
            {
                string trimmed = z.Trim();
                if (string.IsNullOrEmpty(trimmed)) continue;
                string[] p = trimmed.Split(',');
                if (p.Length < 6) continue;
                double pTop, pBottom;
                if (!double.TryParse(p[0], System.Globalization.NumberStyles.Any, fp, out pTop)) continue;
                if (!double.TryParse(p[5], System.Globalization.NumberStyles.Any, fp, out pBottom)) continue;
                double t1 = 0, t2 = 0, o1 = 0, o2 = 0;
                double.TryParse(p[1].Replace("T1=", ""), System.Globalization.NumberStyles.Any, fp, out t1);
                double.TryParse(p[2].Replace("T2=", ""), System.Globalization.NumberStyles.Any, fp, out t2);
                double.TryParse(p[3].Replace("O1=", ""), System.Globalization.NumberStyles.Any, fp, out o1);
                double.TryParse(p[4].Replace("O2=", ""), System.Globalization.NumberStyles.Any, fp, out o2);
                double hi = Math.Max(pTop, pBottom);
                double lo = Math.Min(pTop, pBottom);
                double mid = (hi + lo) / 2.0;

                if (showZones)
                {
                    if (Math.Abs(hi - lo) < 1)
                    {
                        string tag = "S6L_" + symbol + "_" + pTop.ToString("F2");
                        Draw.Line(this, tag, false, startTime, mid, endTime, mid, MakeBrushWithOpacity(S6LineColor, S6LineOpacity), S6LineStyle, S6LineWidth);
                        drawingTags.Add(tag);
                    }
                    else
                    {
                        string tag = "S6B_" + symbol + "_" + pTop.ToString("F2");
                        Draw.Rectangle(this, tag, false, startTime, hi, endTime, lo, S6ZoneColor, S6ZoneColor, S6ZoneOpacity);
                        drawingTags.Add(tag);
                    }
                }
                if (showLabels)
                {
                    double sum = t1 + t2 + o1 + o2;
                    string lineOne = "T1=" + t1 + ",T2=" + t2 + ",O1=" + o1 + ",O2=" + o2 + ", = " + sum;
                    string lineTwo = hi.ToString("F2") + " - " + lo.ToString("F2") + " = " + (int)Math.Round(hi - lo);
                    string fullText = lineOne + System.Environment.NewLine + lineTwo;
                    var font = GetFont(S6LabelSize);
                    Draw.Text(this, "S6LB_" + symbol + "_" + pTop.ToString("F2"), false, fullText, GetLabelBarsAgoForAnchor(S6LabelAnchor, CurrentBar, S6LabelOffset), mid, 0, S6LabelTextColor, font, GetTextAlignment(S6LabelAnchor), S6LabelBG, Brushes.Transparent, 0);
                    drawingTags.Add("S6LB_" + symbol + "_" + pTop.ToString("F2"));
                }
            }
        }

        // =====================================================================
        // DRAWING: Set 7 (reuses Set 1 zone format with own styles)
        // =====================================================================
        private void DrawSet7(string rawData, bool showZones, bool showLabels, string symbol,
            DateTime startTime, DateTime endTime)
        {
            if ((!showZones && !showLabels) || string.IsNullOrEmpty(rawData)) return;
            IFormatProvider fp = System.Globalization.CultureInfo.InvariantCulture;

            foreach (string z in rawData.Split(';'))
            {
                string trimmed = z.Trim();
                if (string.IsNullOrEmpty(trimmed)) continue;
                string[] p = trimmed.Split(',');
                if (p.Length < 6) continue;
                double pTop, pBottom;
                if (!double.TryParse(p[0], System.Globalization.NumberStyles.Any, fp, out pTop)) continue;
                if (!double.TryParse(p[5], System.Globalization.NumberStyles.Any, fp, out pBottom)) continue;
                double t1 = 0, t2 = 0, o1 = 0, o2 = 0;
                double.TryParse(p[1].Replace("T1=", ""), System.Globalization.NumberStyles.Any, fp, out t1);
                double.TryParse(p[2].Replace("T2=", ""), System.Globalization.NumberStyles.Any, fp, out t2);
                double.TryParse(p[3].Replace("O1=", ""), System.Globalization.NumberStyles.Any, fp, out o1);
                double.TryParse(p[4].Replace("O2=", ""), System.Globalization.NumberStyles.Any, fp, out o2);
                double hi = Math.Max(pTop, pBottom);
                double lo = Math.Min(pTop, pBottom);
                double mid = (hi + lo) / 2.0;

                if (showZones)
                {
                    if (Math.Abs(hi - lo) < 1)
                    {
                        string tag = "S7L_" + symbol + "_" + pTop.ToString("F2");
                        Draw.Line(this, tag, false, startTime, mid, endTime, mid, MakeBrushWithOpacity(S7LineColor, S7LineOpacity), S7LineStyle, S7LineWidth);
                        drawingTags.Add(tag);
                    }
                    else
                    {
                        string tag = "S7B_" + symbol + "_" + pTop.ToString("F2");
                        Draw.Rectangle(this, tag, false, startTime, hi, endTime, lo, S7ZoneColor, S7ZoneColor, S7ZoneOpacity);
                        drawingTags.Add(tag);
                    }
                }
                if (showLabels)
                {
                    double sum = t1 + t2 + o1 + o2;
                    string lineOne = "T1=" + t1 + ",T2=" + t2 + ",O1=" + o1 + ",O2=" + o2 + ", = " + sum;
                    string lineTwo = hi.ToString("F2") + " - " + lo.ToString("F2") + " = " + (int)Math.Round(hi - lo);
                    string fullText = lineOne + System.Environment.NewLine + lineTwo;
                    var font = GetFont(S7LabelSize);
                    Draw.Text(this, "S7LB_" + symbol + "_" + pTop.ToString("F2"), false, fullText, GetLabelBarsAgoForAnchor(S7LabelAnchor, CurrentBar, S7LabelOffset), mid, 0, S7LabelTextColor, font, GetTextAlignment(S7LabelAnchor), S7LabelBG, Brushes.Transparent, 0);
                    drawingTags.Add("S7LB_" + symbol + "_" + pTop.ToString("F2"));
                }
            }
        }
        // =====================================================================
        // DRAWING: LIS - combined from all sets
        // =====================================================================
        private void DrawLIS(string rawData, bool showLines, bool showLabels, DateTime startTime, DateTime endTime)
        {
            if ((!showLines && !showLabels) || string.IsNullOrEmpty(rawData)) return;
            IFormatProvider fp = System.Globalization.CultureInfo.InvariantCulture;

            foreach (string seg in rawData.Split(';'))
            {
                string trimmed = seg.Trim();
                if (string.IsNullOrEmpty(trimmed)) continue;

                if (trimmed.Contains(","))
                {
                    string[] p = trimmed.Split(',');
                    double t, b;
                    if (p.Length < 2) continue;
                    if (!double.TryParse(p[0], System.Globalization.NumberStyles.Any, fp, out t)) continue;
                    if (!double.TryParse(p[1], System.Globalization.NumberStyles.Any, fp, out b)) continue;

                    double hi = Math.Max(t, b);
                    double lo = Math.Min(t, b);
                    double mid = (hi + lo) / 2.0;

                    if (showLines)
                    {
                        string tag = "LISB_" + t.ToString("F2");
                        Draw.Rectangle(this, tag, false, startTime, hi, endTime, lo, LISZoneColor, LISZoneColor, LISZoneOpacity);
                        drawingTags.Add(tag);
                    }
                    if (showLabels)
                    {
                        var font = GetFont(LISLabelSize);
                        Draw.Text(this, "LISLB_" + t.ToString("F2"), false, "LIS", GetLabelBarsAgoForAnchor(LISLabelAnchor, CurrentBar, LISLabelOffset), mid, -14, LISLabelTextColor, font, GetTextAlignment(LISLabelAnchor), LISLabelBG, Brushes.Transparent, 0);
                        drawingTags.Add("LISLB_" + t.ToString("F2"));
                    }
                }
                else
                {
                    double level;
                    if (!double.TryParse(trimmed, System.Globalization.NumberStyles.Any, fp, out level)) continue;

                    if (showLines)
                    {
                        string tag = "LISL_" + level.ToString("F2");
                        Draw.Line(this, tag, false, startTime, level, endTime, level, MakeBrushWithOpacity(LISLineColor, LISLineOpacity), LISLineStyle, LISLineWidth);
                        drawingTags.Add(tag);
                    }
                    if (showLabels)
                    {
                        var font = GetFont(LISLabelSize);
                        Draw.Text(this, "LISLB_" + level.ToString("F2"), false, "LIS", GetLabelBarsAgoForAnchor(LISLabelAnchor, CurrentBar, LISLabelOffset), level, -14, LISLabelTextColor, font, GetTextAlignment(LISLabelAnchor), LISLabelBG, Brushes.Transparent, 0);
                        drawingTags.Add("LISLB_" + level.ToString("F2"));
                    }
                }
            }
        }
        // =====================================================================
        // DRAWING: Zulu - Zulu VOL, OI, Gamma Flip levels
        // =====================================================================
        private void DrawZulu(string rawData, bool showLines, bool showLabels, DateTime startTime, DateTime endTime)
        {
            if ((!showLines && !showLabels) || string.IsNullOrEmpty(rawData)) return;
            IFormatProvider fp = System.Globalization.CultureInfo.InvariantCulture;

            foreach (string seg in rawData.Split(';'))
            {
                string trimmed = seg.Trim();
                if (string.IsNullOrEmpty(trimmed)) continue;

                if (trimmed.StartsWith("Zulu VOL-", StringComparison.OrdinalIgnoreCase))
                {
                    string vals = trimmed.Substring(9);
                    string[] parts = vals.Split(',');
                    if (parts.Length < 2) continue;
                    double upper, lower;
                    if (!double.TryParse(parts[0], System.Globalization.NumberStyles.Any, fp, out upper)) continue;
                    if (!double.TryParse(parts[1], System.Globalization.NumberStyles.Any, fp, out lower)) continue;

                    if (showLines)
                    {
                        string tagUp = "ZVOLU_" + upper.ToString("F2");
                        Draw.Line(this, tagUp, false, startTime, upper, endTime, upper, MakeBrushWithOpacity(ZuluVolColor, ZuluLineOpacity), ZuluLineStyle, ZuluLineWidth);
                        drawingTags.Add(tagUp);
                    }
                    if (showLabels)
                    {
                        var font = GetFont(ZuluLabelSize);
                        Draw.Text(this, "ZVOLUL_" + upper.ToString("F2"), false, "VOL Upper", GetLabelBarsAgoForAnchor(ZuluLabelAnchor, CurrentBar, ZuluLabelOffset), upper, -14, ZuluLabelTextColor, font, GetTextAlignment(ZuluLabelAnchor), ZuluLabelBG, Brushes.Transparent, 0);
                        drawingTags.Add("ZVOLUL_" + upper.ToString("F2"));
                    }

                    if (showLines)
                    {
                        string tagLo = "ZVOLL_" + lower.ToString("F2");
                        Draw.Line(this, tagLo, false, startTime, lower, endTime, lower, MakeBrushWithOpacity(ZuluVolColor, ZuluLineOpacity), ZuluLineStyle, ZuluLineWidth);
                        drawingTags.Add(tagLo);
                    }
                    if (showLabels)
                    {
                        var font = GetFont(ZuluLabelSize);
                        Draw.Text(this, "ZVOLLL_" + lower.ToString("F2"), false, "VOL Lower", GetLabelBarsAgoForAnchor(ZuluLabelAnchor, CurrentBar, ZuluLabelOffset), lower, -14, ZuluLabelTextColor, font, GetTextAlignment(ZuluLabelAnchor), ZuluLabelBG, Brushes.Transparent, 0);
                        drawingTags.Add("ZVOLLL_" + lower.ToString("F2"));
                    }
                }
                else if (trimmed.StartsWith("Zulu OI-", StringComparison.OrdinalIgnoreCase))
                {
                    string vals = trimmed.Substring(8);
                    string[] parts = vals.Split(',');
                    if (parts.Length < 2) continue;
                    double upper, lower;
                    if (!double.TryParse(parts[0], System.Globalization.NumberStyles.Any, fp, out upper)) continue;
                    if (!double.TryParse(parts[1], System.Globalization.NumberStyles.Any, fp, out lower)) continue;

                    if (showLines)
                    {
                        string tagUp = "ZOIU_" + upper.ToString("F2");
                        Draw.Line(this, tagUp, false, startTime, upper, endTime, upper, MakeBrushWithOpacity(ZuluOiColor, ZuluLineOpacity), ZuluLineStyle, ZuluLineWidth);
                        drawingTags.Add(tagUp);
                    }
                    if (showLabels)
                    {
                        var font = GetFont(ZuluLabelSize);
                        Draw.Text(this, "ZOIUL_" + upper.ToString("F2"), false, "OI Upper", GetLabelBarsAgoForAnchor(ZuluLabelAnchor, CurrentBar, ZuluLabelOffset), upper, -14, ZuluLabelTextColor, font, GetTextAlignment(ZuluLabelAnchor), ZuluLabelBG, Brushes.Transparent, 0);
                        drawingTags.Add("ZOIUL_" + upper.ToString("F2"));
                    }

                    if (showLines)
                    {
                        string tagLo = "ZOIL_" + lower.ToString("F2");
                        Draw.Line(this, tagLo, false, startTime, lower, endTime, lower, MakeBrushWithOpacity(ZuluOiColor, ZuluLineOpacity), ZuluLineStyle, ZuluLineWidth);
                        drawingTags.Add(tagLo);
                    }
                    if (showLabels)
                    {
                        var font = GetFont(ZuluLabelSize);
                        Draw.Text(this, "ZOILL_" + lower.ToString("F2"), false, "OI Lower", GetLabelBarsAgoForAnchor(ZuluLabelAnchor, CurrentBar, ZuluLabelOffset), lower, -14, ZuluLabelTextColor, font, GetTextAlignment(ZuluLabelAnchor), ZuluLabelBG, Brushes.Transparent, 0);
                        drawingTags.Add("ZOILL_" + lower.ToString("F2"));
                    }
                }
                else if (trimmed.StartsWith("Zulu Gamma Flip-", StringComparison.OrdinalIgnoreCase))
                {
                    string val = trimmed.Substring(16);
                    double level;
                    if (!double.TryParse(val, System.Globalization.NumberStyles.Any, fp, out level)) continue;

                    if (showLines)
                    {
                        string tag = "ZGF_" + level.ToString("F2");
                        Draw.Line(this, tag, false, startTime, level, endTime, level, MakeBrushWithOpacity(ZuluGammaColor, ZuluLineOpacity), ZuluLineStyle, ZuluLineWidth);
                        drawingTags.Add(tag);
                    }
                    if (showLabels)
                    {
                        var font = GetFont(ZuluLabelSize);
                        Draw.Text(this, "ZGFL_" + level.ToString("F2"), false, "Gamma Flip", GetLabelBarsAgoForAnchor(ZuluLabelAnchor, CurrentBar, ZuluLabelOffset), level, -14, ZuluLabelTextColor, font, GetTextAlignment(ZuluLabelAnchor), ZuluLabelBG, Brushes.Transparent, 0);
                        drawingTags.Add("ZGFL_" + level.ToString("F2"));
                    }
                }
            }
        }
        // =====================================================================
        // PROPERTIES
        // =====================================================================
        #region Properties

        // â”€â”€ Data Input â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€
        // Data Input
        [NinjaScriptProperty]
        [Display(Name = "Set 3 Data", Description = "Format: [TICKER] Time: ... | Mid: ..., Lower: ..., Upper: ...", GroupName = "Data Input", Order = 2)]
        public string Set3Data { get; set; }

        [Display(Name = "Auto-Load From File", GroupName = "File Loading", Order = 1)]
        public bool AutoLoadFromFile { get; set; }

        [Display(Name = "Full Levels File Path", GroupName = "File Loading", Order = 2)]
        public string FullLevelsFilePath { get; set; }

        [Display(Name = "Show Zones",  GroupName = "S1 Style", Order = 13)]  public bool S1ShowZones  { get; set; }
        [Display(Name = "Show Labels", GroupName = "S1 Style", Order = 14)]  public bool S1ShowLabels { get; set; }
        [Display(Name = "Show Zones",  GroupName = "S2 Style", Order = 13)]  public bool S2ShowZones  { get; set; }
        [Display(Name = "Show Labels", GroupName = "S2 Style", Order = 14)]  public bool S2ShowLabels { get; set; }
        [Display(Name = "Show Key Levels",  GroupName = "S3 Style", Order = 15)]  public bool S3ShowKeyLevels  { get; set; }
        [Display(Name = "Show Key Labels",  GroupName = "S3 Style", Order = 16)]  public bool S3ShowKeyLabels  { get; set; }
        [Display(Name = "Show Timestamp",    GroupName = "S3 Style", Order = 17)]  public bool S3ShowTimestamp    { get; set; }
        [Display(Name = "Show Lines",  GroupName = "S4 Style", Order = 8)]  public bool S4ShowZones  { get; set; }
        [Display(Name = "Show Labels", GroupName = "S4 Style", Order = 9)]  public bool S4ShowLabels { get; set; }
        [Display(Name = "Show Zones",  GroupName = "S5 Style", Order = 13)] public bool S5ShowZones  { get; set; }
        [Display(Name = "Show Labels", GroupName = "S5 Style", Order = 14)] public bool S5ShowLabels { get; set; }
        [Display(Name = "Show Zones",  GroupName = "S6 Style", Order = 13)] public bool S6ShowZones  { get; set; }
        [Display(Name = "Show Labels", GroupName = "S6 Style", Order = 14)] public bool S6ShowLabels { get; set; }
        [Display(Name = "Show Zones",  GroupName = "S7 Style", Order = 13)] public bool S7ShowZones  { get; set; }
        [Display(Name = "Show Labels", GroupName = "S7 Style", Order = 14)] public bool S7ShowLabels { get; set; }
        [Display(Name = "Show Labels", GroupName = "Zulu Style", Order = 10)] public bool ZuluShowLabels { get; set; }
        [Display(Name = "Show Lines",  GroupName = "Zulu Style", Order = 11)] public bool ZuluShowLines  { get; set; }
        // â”€â”€ S1 Style â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€
        [Display(Name = "Label Offset", GroupName = "S1 Style", Order = 1)]  public int S1LabelOffset { get; set; }
        [Display(Name = "Label Size",   GroupName = "S1 Style", Order = 2)]  public int S1LabelSize   { get; set; }
        [XmlIgnore][Display(Name = "Label BG",     GroupName = "S1 Style", Order = 3)]  public Brush S1LabelBG     { get; set; }
        [Browsable(false)] public string S1LabelBGSer     { get { return Serialize.BrushToString(S1LabelBG); }     set { S1LabelBG     = Serialize.StringToBrush(value); } }
        [XmlIgnore][Display(Name = "Label Text",   GroupName = "S1 Style", Order = 4)]  public Brush S1LabelTextColor { get; set; }
        [Browsable(false)] public string S1LabelTextColorSer { get { return Serialize.BrushToString(S1LabelTextColor); } set { S1LabelTextColor = Serialize.StringToBrush(value); } }
        [XmlIgnore][Display(Name = "Line Color",   GroupName = "S1 Style", Order = 5)]  public Brush S1LineColor   { get; set; }
        [Browsable(false)] public string S1LineColorSer   { get { return Serialize.BrushToString(S1LineColor); }   set { S1LineColor   = Serialize.StringToBrush(value); } }
        [XmlIgnore][Display(Name = "Zone Color",   GroupName = "S1 Style", Order = 6)]  public Brush S1ZoneColor   { get; set; }
        [Browsable(false)] public string S1ZoneColorSer   { get { return Serialize.BrushToString(S1ZoneColor); }   set { S1ZoneColor   = Serialize.StringToBrush(value); } }
        [Display(Name = "Zone Opacity", GroupName = "S1 Style", Order = 7)]  public int S1ZoneOpacity { get; set; }
        [Display(Name = "Line Opacity", GroupName = "S1 Style", Order = 10)]  public int S1LineOpacity { get; set; }
        [Display(Name = "Line Width",   GroupName = "S1 Style", Order = 8)]  public int S1LineWidth   { get; set; }
        [Display(Name = "Line Style",   GroupName = "S1 Style", Order = 9)]  public DashStyleHelper S1LineStyle { get; set; }
        [Display(Name = "Label Anchor", GroupName = "S1 Style", Order = 12)] public LabelAnchorPosition S1LabelAnchor { get; set; }

        // â”€â”€ S2 Style â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€
        [Display(Name = "Label Offset", GroupName = "S2 Style", Order = 1)]  public int S2LabelOffset { get; set; }
        [Display(Name = "Label Size",   GroupName = "S2 Style", Order = 2)]  public int S2LabelSize   { get; set; }
        [XmlIgnore][Display(Name = "Label BG",     GroupName = "S2 Style", Order = 3)]  public Brush S2LabelBG     { get; set; }
        [Browsable(false)] public string S2LabelBGSer     { get { return Serialize.BrushToString(S2LabelBG); }     set { S2LabelBG     = Serialize.StringToBrush(value); } }
        [XmlIgnore][Display(Name = "Label Text",   GroupName = "S2 Style", Order = 4)]  public Brush S2LabelTextColor { get; set; }
        [Browsable(false)] public string S2LabelTextColorSer { get { return Serialize.BrushToString(S2LabelTextColor); } set { S2LabelTextColor = Serialize.StringToBrush(value); } }
        [XmlIgnore][Display(Name = "Line Color",   GroupName = "S2 Style", Order = 5)]  public Brush S2LineColor   { get; set; }
        [Browsable(false)] public string S2LineColorSer   { get { return Serialize.BrushToString(S2LineColor); }   set { S2LineColor   = Serialize.StringToBrush(value); } }
        [XmlIgnore][Display(Name = "Zone Color",   GroupName = "S2 Style", Order = 6)]  public Brush S2ZoneColor   { get; set; }
        [Browsable(false)] public string S2ZoneColorSer   { get { return Serialize.BrushToString(S2ZoneColor); }   set { S2ZoneColor   = Serialize.StringToBrush(value); } }
        [Display(Name = "Zone Opacity", GroupName = "S2 Style", Order = 7)]  public int S2ZoneOpacity { get; set; }
        [Display(Name = "Line Opacity", GroupName = "S2 Style", Order = 10)]  public int S2LineOpacity { get; set; }
        [Display(Name = "Line Width",   GroupName = "S2 Style", Order = 8)]  public int S2LineWidth   { get; set; }
        [Display(Name = "Line Style",   GroupName = "S2 Style", Order = 9)]  public DashStyleHelper S2LineStyle { get; set; }
        [Display(Name = "Label Anchor", GroupName = "S2 Style", Order = 12)] public LabelAnchorPosition S2LabelAnchor { get; set; }

        // â”€â”€ S3 Style â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€
        [Display(Name = "Label Offset", GroupName = "S3 Style", Order = 1)]  public int S3LabelOffset { get; set; }
        [Display(Name = "Label Size",   GroupName = "S3 Style", Order = 2)]  public int S3LabelSize   { get; set; }
        [XmlIgnore][Display(Name = "Label BG",     GroupName = "S3 Style", Order = 3)]  public Brush S3LabelBG     { get; set; }
        [Browsable(false)] public string S3LabelBGSer     { get { return Serialize.BrushToString(S3LabelBG); }     set { S3LabelBG     = Serialize.StringToBrush(value); } }
        [XmlIgnore][Display(Name = "Label Text",   GroupName = "S3 Style", Order = 4)]  public Brush S3LabelTextColor { get; set; }
        [Browsable(false)] public string S3LabelTextColorSer { get { return Serialize.BrushToString(S3LabelTextColor); } set { S3LabelTextColor = Serialize.StringToBrush(value); } }
        [XmlIgnore][Display(Name = "Mid Color",    GroupName = "S3 Style", Order = 5)]  public Brush S3MidColor    { get; set; }
        [Browsable(false)] public string S3MidColorSer    { get { return Serialize.BrushToString(S3MidColor); }    set { S3MidColor    = Serialize.StringToBrush(value); } }
        [XmlIgnore][Display(Name = "Lower Color",  GroupName = "S3 Style", Order = 6)]  public Brush S3LowerColor  { get; set; }
        [Browsable(false)] public string S3LowerColorSer  { get { return Serialize.BrushToString(S3LowerColor); }  set { S3LowerColor  = Serialize.StringToBrush(value); } }
        [XmlIgnore][Display(Name = "Upper Color",  GroupName = "S3 Style", Order = 7)]  public Brush S3UpperColor  { get; set; }
        [Browsable(false)] public string S3UpperColorSer  { get { return Serialize.BrushToString(S3UpperColor); }  set { S3UpperColor  = Serialize.StringToBrush(value); } }
        [Display(Name = "Line Opacity", GroupName = "S3 Style", Order = 8)]  public int S3LineOpacity { get; set; }
        [Display(Name = "Mid Width",    GroupName = "S3 Style", Order = 9)]  public int S3MidWidth     { get; set; }
        [Display(Name = "Lower Width",  GroupName = "S3 Style", Order = 9)]  public int S3LowerWidth   { get; set; }
        [Display(Name = "Upper Width",  GroupName = "S3 Style", Order = 10)] public int S3UpperWidth   { get; set; }
        [Display(Name = "Mid Style",    GroupName = "S3 Style", Order = 11)] public DashStyleHelper S3MidStyle   { get; set; }
        [Display(Name = "Lower Style",  GroupName = "S3 Style", Order = 12)] public DashStyleHelper S3LowerStyle { get; set; }
        [Display(Name = "Upper Style",  GroupName = "S3 Style", Order = 13)] public DashStyleHelper S3UpperStyle { get; set; }
        [Display(Name = "Timestamp Position", GroupName = "S3 Style", Order = 14)] public TextPosition S3TimestampPosition { get; set; }
        [Display(Name = "Label Anchor", GroupName = "S3 Style", Order = 18)] public LabelAnchorPosition S3LabelAnchor { get; set; }
        // â”€â”€ S4 Style â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€
        [Display(Name = "Label Offset", GroupName = "S4 Style", Order = 1)]  public int S4LabelOffset { get; set; }
        [Display(Name = "Label Size",   GroupName = "S4 Style", Order = 2)]  public int S4LabelSize   { get; set; }
        [XmlIgnore][Display(Name = "Label BG",     GroupName = "S4 Style", Order = 3)]  public Brush S4LabelBG     { get; set; }
        [Browsable(false)] public string S4LabelBGSer     { get { return Serialize.BrushToString(S4LabelBG); }     set { S4LabelBG     = Serialize.StringToBrush(value); } }
        [XmlIgnore][Display(Name = "Label Text",   GroupName = "S4 Style", Order = 4)]  public Brush S4LabelTextColor { get; set; }
        [Browsable(false)] public string S4LabelTextColorSer { get { return Serialize.BrushToString(S4LabelTextColor); } set { S4LabelTextColor = Serialize.StringToBrush(value); } }
        [XmlIgnore][Display(Name = "Line Color",   GroupName = "S4 Style", Order = 5)]  public Brush S4LineColor   { get; set; }
        [Browsable(false)] public string S4LineColorSer   { get { return Serialize.BrushToString(S4LineColor); }   set { S4LineColor   = Serialize.StringToBrush(value); } }
        [Display(Name = "Line Opacity", GroupName = "S4 Style", Order = 6)]  public int S4LineOpacity  { get; set; }
        [Display(Name = "Line Width",   GroupName = "S4 Style", Order = 7)]  public int S4LineWidth   { get; set; }
        [Display(Name = "Line Style",   GroupName = "S4 Style", Order = 7)]  public DashStyleHelper S4LineStyle { get; set; }
        [Display(Name = "Label Anchor", GroupName = "S4 Style", Order = 10)] public LabelAnchorPosition S4LabelAnchor { get; set; }

        // â”€â”€ S5 Style â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€
        [Display(Name = "Label Offset", GroupName = "S5 Style", Order = 1)]  public int S5LabelOffset { get; set; }
        [Display(Name = "Label Size",   GroupName = "S5 Style", Order = 2)]  public int S5LabelSize   { get; set; }
        [XmlIgnore][Display(Name = "Label BG",     GroupName = "S5 Style", Order = 3)]  public Brush S5LabelBG     { get; set; }
        [Browsable(false)] public string S5LabelBGSer     { get { return Serialize.BrushToString(S5LabelBG); }     set { S5LabelBG     = Serialize.StringToBrush(value); } }
        [XmlIgnore][Display(Name = "Label Text",   GroupName = "S5 Style", Order = 4)]  public Brush S5LabelTextColor { get; set; }
        [Browsable(false)] public string S5LabelTextColorSer { get { return Serialize.BrushToString(S5LabelTextColor); } set { S5LabelTextColor = Serialize.StringToBrush(value); } }
        [XmlIgnore][Display(Name = "Line Color",   GroupName = "S5 Style", Order = 5)]  public Brush S5LineColor   { get; set; }
        [Browsable(false)] public string S5LineColorSer   { get { return Serialize.BrushToString(S5LineColor); }   set { S5LineColor   = Serialize.StringToBrush(value); } }
        [XmlIgnore][Display(Name = "Zone Color",   GroupName = "S5 Style", Order = 6)]  public Brush S5ZoneColor   { get; set; }
        [Browsable(false)] public string S5ZoneColorSer   { get { return Serialize.BrushToString(S5ZoneColor); }   set { S5ZoneColor   = Serialize.StringToBrush(value); } }
        [Display(Name = "Zone Opacity", GroupName = "S5 Style", Order = 7)]  public int S5ZoneOpacity { get; set; }
        [Display(Name = "Line Opacity", GroupName = "S5 Style", Order = 10)]  public int S5LineOpacity { get; set; }
        [Display(Name = "Line Width",   GroupName = "S5 Style", Order = 8)]  public int S5LineWidth   { get; set; }
        [Display(Name = "Line Style",   GroupName = "S5 Style", Order = 9)]  public DashStyleHelper S5LineStyle { get; set; }
        [Display(Name = "Label Anchor", GroupName = "S5 Style", Order = 12)] public LabelAnchorPosition S5LabelAnchor { get; set; }

        // â”€â”€ S6 Style â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€
        [Display(Name = "Label Offset", GroupName = "S6 Style", Order = 1)]  public int S6LabelOffset { get; set; }
        [Display(Name = "Label Size",   GroupName = "S6 Style", Order = 2)]  public int S6LabelSize   { get; set; }
        [XmlIgnore][Display(Name = "Label BG",     GroupName = "S6 Style", Order = 3)]  public Brush S6LabelBG     { get; set; }
        [Browsable(false)] public string S6LabelBGSer     { get { return Serialize.BrushToString(S6LabelBG); }     set { S6LabelBG     = Serialize.StringToBrush(value); } }
        [XmlIgnore][Display(Name = "Label Text",   GroupName = "S6 Style", Order = 4)]  public Brush S6LabelTextColor { get; set; }
        [Browsable(false)] public string S6LabelTextColorSer { get { return Serialize.BrushToString(S6LabelTextColor); } set { S6LabelTextColor = Serialize.StringToBrush(value); } }
        [XmlIgnore][Display(Name = "Line Color",   GroupName = "S6 Style", Order = 5)]  public Brush S6LineColor   { get; set; }
        [Browsable(false)] public string S6LineColorSer   { get { return Serialize.BrushToString(S6LineColor); }   set { S6LineColor   = Serialize.StringToBrush(value); } }
        [XmlIgnore][Display(Name = "Zone Color",   GroupName = "S6 Style", Order = 6)]  public Brush S6ZoneColor   { get; set; }
        [Browsable(false)] public string S6ZoneColorSer   { get { return Serialize.BrushToString(S6ZoneColor); }   set { S6ZoneColor   = Serialize.StringToBrush(value); } }
        [Display(Name = "Zone Opacity", GroupName = "S6 Style", Order = 7)]  public int S6ZoneOpacity { get; set; }
        [Display(Name = "Line Opacity", GroupName = "S6 Style", Order = 10)]  public int S6LineOpacity { get; set; }
        [Display(Name = "Line Width",   GroupName = "S6 Style", Order = 8)]  public int S6LineWidth   { get; set; }
        [Display(Name = "Line Style",   GroupName = "S6 Style", Order = 9)]  public DashStyleHelper S6LineStyle { get; set; }
        [Display(Name = "Label Anchor", GroupName = "S6 Style", Order = 12)] public LabelAnchorPosition S6LabelAnchor { get; set; }

        // â”€â”€ S7 Style â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€
        [Display(Name = "Label Offset", GroupName = "S7 Style", Order = 1)]  public int S7LabelOffset { get; set; }
        [Display(Name = "Label Size",   GroupName = "S7 Style", Order = 2)]  public int S7LabelSize   { get; set; }
        [XmlIgnore][Display(Name = "Label BG",     GroupName = "S7 Style", Order = 3)]  public Brush S7LabelBG     { get; set; }
        [Browsable(false)] public string S7LabelBGSer     { get { return Serialize.BrushToString(S7LabelBG); }     set { S7LabelBG     = Serialize.StringToBrush(value); } }
        [XmlIgnore][Display(Name = "Label Text",   GroupName = "S7 Style", Order = 4)]  public Brush S7LabelTextColor { get; set; }
        [Browsable(false)] public string S7LabelTextColorSer { get { return Serialize.BrushToString(S7LabelTextColor); } set { S7LabelTextColor = Serialize.StringToBrush(value); } }
        [XmlIgnore][Display(Name = "Line Color",   GroupName = "S7 Style", Order = 5)]  public Brush S7LineColor   { get; set; }
        [Browsable(false)] public string S7LineColorSer   { get { return Serialize.BrushToString(S7LineColor); }   set { S7LineColor   = Serialize.StringToBrush(value); } }
        [XmlIgnore][Display(Name = "Zone Color",   GroupName = "S7 Style", Order = 6)]  public Brush S7ZoneColor   { get; set; }
        [Browsable(false)] public string S7ZoneColorSer   { get { return Serialize.BrushToString(S7ZoneColor); }   set { S7ZoneColor   = Serialize.StringToBrush(value); } }
        [Display(Name = "Zone Opacity", GroupName = "S7 Style", Order = 7)]  public int S7ZoneOpacity { get; set; }
        [Display(Name = "Line Opacity", GroupName = "S7 Style", Order = 10)]  public int S7LineOpacity { get; set; }
        [Display(Name = "Line Width",   GroupName = "S7 Style", Order = 8)]  public int S7LineWidth   { get; set; }
        [Display(Name = "Line Style",   GroupName = "S7 Style", Order = 9)]  public DashStyleHelper S7LineStyle { get; set; }
        [Display(Name = "Label Anchor", GroupName = "S7 Style", Order = 12)] public LabelAnchorPosition S7LabelAnchor { get; set; }

        // â”€â”€ Zulu Style â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€
        [Display(Name = "Label Offset", GroupName = "Zulu Style", Order = 1)]  public int ZuluLabelOffset { get; set; }
        [Display(Name = "Label Size",   GroupName = "Zulu Style", Order = 2)]  public int ZuluLabelSize   { get; set; }
        [XmlIgnore][Display(Name = "Label BG",     GroupName = "Zulu Style", Order = 3)]  public Brush ZuluLabelBG     { get; set; }
        [Browsable(false)] public string ZuluLabelBGSer     { get { return Serialize.BrushToString(ZuluLabelBG); }     set { ZuluLabelBG     = Serialize.StringToBrush(value); } }
        [XmlIgnore][Display(Name = "Label Text",   GroupName = "Zulu Style", Order = 4)]  public Brush ZuluLabelTextColor { get; set; }
        [Browsable(false)] public string ZuluLabelTextColorSer { get { return Serialize.BrushToString(ZuluLabelTextColor); } set { ZuluLabelTextColor = Serialize.StringToBrush(value); } }
        [XmlIgnore][Display(Name = "VOL Color",    GroupName = "Zulu Style", Order = 5)]  public Brush ZuluVolColor   { get; set; }
        [Browsable(false)] public string ZuluVolColorSer   { get { return Serialize.BrushToString(ZuluVolColor); }   set { ZuluVolColor   = Serialize.StringToBrush(value); } }
        [XmlIgnore][Display(Name = "OI Color",     GroupName = "Zulu Style", Order = 6)]  public Brush ZuluOiColor    { get; set; }
        [Browsable(false)] public string ZuluOiColorSer    { get { return Serialize.BrushToString(ZuluOiColor); }    set { ZuluOiColor    = Serialize.StringToBrush(value); } }
        [XmlIgnore][Display(Name = "Gamma Color",  GroupName = "Zulu Style", Order = 7)]  public Brush ZuluGammaColor { get; set; }
        [Browsable(false)] public string ZuluGammaColorSer { get { return Serialize.BrushToString(ZuluGammaColor); } set { ZuluGammaColor = Serialize.StringToBrush(value); } }
        [Display(Name = "Line Opacity", GroupName = "Zulu Style", Order = 8)]  public int ZuluLineOpacity { get; set; }
        [Display(Name = "Line Width",   GroupName = "Zulu Style", Order = 9)]  public int ZuluLineWidth   { get; set; }
        [Display(Name = "Line Style",   GroupName = "Zulu Style", Order = 10)] public DashStyleHelper ZuluLineStyle { get; set; }
        [Display(Name = "Label Anchor", GroupName = "Zulu Style", Order = 12)] public LabelAnchorPosition ZuluLabelAnchor { get; set; }

        // â”€â”€ LIS (within S2 and LIS Style) â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€
        [Display(Name = "LIS Label Offset", GroupName = "S2 and LIS Style", Order = 21)]  public int LISLabelOffset { get; set; }
        [Display(Name = "LIS Label Size",   GroupName = "S2 and LIS Style", Order = 22)]  public int LISLabelSize   { get; set; }
        [XmlIgnore][Display(Name = "LIS Label BG",     GroupName = "S2 and LIS Style", Order = 23)]  public Brush LISLabelBG     { get; set; }
        [Browsable(false)] public string LISLabelBGSer     { get { return Serialize.BrushToString(LISLabelBG); }     set { LISLabelBG     = Serialize.StringToBrush(value); } }
        [XmlIgnore][Display(Name = "LIS Label Text",   GroupName = "S2 and LIS Style", Order = 24)]  public Brush LISLabelTextColor { get; set; }
        [Browsable(false)] public string LISLabelTextColorSer { get { return Serialize.BrushToString(LISLabelTextColor); } set { LISLabelTextColor = Serialize.StringToBrush(value); } }
        [XmlIgnore][Display(Name = "LIS Line Color",   GroupName = "S2 and LIS Style", Order = 25)]  public Brush LISLineColor   { get; set; }
        [XmlIgnore][Display(Name = "LIS Zone Color",   GroupName = "S2 and LIS Style", Order = 26)]  public Brush LISZoneColor   { get; set; }
        [Browsable(false)] public string LISZoneColorSer   { get { return Serialize.BrushToString(LISZoneColor); }   set { LISZoneColor   = Serialize.StringToBrush(value); } }
        [Browsable(false)] public string LISLineColorSer   { get { return Serialize.BrushToString(LISLineColor); }   set { LISLineColor   = Serialize.StringToBrush(value); } }
        [Display(Name = "LIS Line Opacity", GroupName = "S2 and LIS Style", Order = 27)]  public int LISLineOpacity  { get; set; }
        [Display(Name = "LIS Line Width",   GroupName = "S2 and LIS Style", Order = 28)]  public int LISLineWidth   { get; set; }
        [Display(Name = "LIS Line Style",   GroupName = "S2 and LIS Style", Order = 28)]  public DashStyleHelper LISLineStyle { get; set; }
        [Display(Name = "LIS Label Anchor", GroupName = "S2 and LIS Style", Order = 29)] public LabelAnchorPosition LISLabelAnchor { get; set; }
        [Display(Name = "LIS Zone Opacity", GroupName = "S2 and LIS Style", Order = 26)]  public int LISZoneOpacity { get; set; }
        [Display(Name = "LIS Show Lines",  GroupName = "S2 and LIS Style", Order = 31)]  public bool LISShowLines  { get; set; }
        [Display(Name = "LIS Show Labels", GroupName = "S2 and LIS Style", Order = 32)]  public bool LISShowLabels { get; set; }

        #endregion
    }
}
