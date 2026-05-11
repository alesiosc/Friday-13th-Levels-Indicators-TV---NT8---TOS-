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

namespace NinjaTrader.NinjaScript.Indicators
{
    // Enum for label anchor position
    public enum LabelAnchorPosition
    {
        Left,
        Right
    }

    public class NT8_Levels_Indicator : Indicator
    {
        private List<string> drawingTags;

        protected override void OnStateChange()
        {
            if (State == State.SetDefaults)
            {
                Description = "NQ+ES+YM Levels Indicator - Set 1-7 + LIS";
                Name = "NT8_Levels_Indicator";
                Calculate = Calculate.OnBarClose;
                IsOverlay = true;
                DisplayInDataBox = false;
                DrawOnPricePanel = true;
                ScaleJustification = ScaleJustification.Right;
                IsSuspendedWhileInactive = true;
                
                // Draw zones behind candles
                ZOrder = -1;

                // File Loading Defaults
                FullLevelsFilePath = @"C:\levels.txt";
                AutoLoadFromFile = false;

                // Initialize default data values
                NqSet1Data = ""; NqSet2Data = ""; NqSet3Data = ""; NqSet4Data = ""; NqSet5Data = ""; NqSet6Data = ""; NqSet7Data = "";
                EsSet1Data = ""; EsSet2Data = ""; EsSet3Data = ""; EsSet4Data = ""; EsSet5Data = ""; EsSet6Data = ""; EsSet7Data = "";
                YmSet1Data = ""; YmSet2Data = ""; YmSet3Data = ""; YmSet4Data = ""; YmSet5Data = ""; YmSet6Data = ""; YmSet7Data = "";

                // Default visibility
                NqShowZones1 = true; NqShowLabels1 = true; NqShowZones2 = true; NqShowLabels2 = true; NqShowKeyLevels3 = true; NqShowKeyLabels3 = true; NqShowTimestamp3 = true; NqShowZones4 = true; NqShowLabels4 = true; NqShowZones5 = true; NqShowLabels5 = true; NqShowZones6 = true; NqShowLabels6 = true; NqShowZones7 = true; NqShowLabels7 = true;
                EsShowZones1 = true; EsShowLabels1 = true; EsShowZones2 = true; EsShowLabels2 = true; EsShowKeyLevels3 = true; EsShowKeyLabels3 = true; EsShowTimestamp3 = true; EsShowZones4 = true; EsShowLabels4 = true; EsShowZones5 = true; EsShowLabels5 = true; EsShowZones6 = true; EsShowLabels6 = true; EsShowZones7 = true; EsShowLabels7 = true;
                YmShowZones1 = true; YmShowLabels1 = true; YmShowZones2 = true; YmShowLabels2 = true; YmShowKeyLevels3 = true; YmShowKeyLabels3 = true; YmShowTimestamp3 = true; YmShowZones4 = true; YmShowLabels4 = true; YmShowZones5 = true; YmShowLabels5 = true; YmShowZones6 = true; YmShowLabels6 = true; YmShowZones7 = true; YmShowLabels7 = true;

                // Initial Styling - NQ
                SetNqDefaults();
                // Initial Styling - ES
                SetEsDefaults();
                // Initial Styling - YM
                SetYmDefaults();
                // Initial Styling - LIS
                SetLisDefaults();
            }
            else if (State == State.DataLoaded)
            {
                drawingTags = new List<string>();
            }
            else if (State == State.Historical)
            {
                SetZOrder(-1);
            }
        }

        private void SetNqDefaults()
        {
            NqSet1LabelOffset = 3; NqSet1LabelSize = 10; NqSet1LabelBGColor = Brushes.Green; NqSet1LabelTextColor = Brushes.White; NqSet1LineColor = Brushes.Green; NqSet1ZoneColor = Brushes.Green; NqSet1ZoneOpacity = 80; NqSet1LineWidth = 2; NqSet1LineStyle = DashStyleHelper.Solid; NqSet1LabelAnchor = LabelAnchorPosition.Left;
            NqSet2LabelOffset = 3; NqSet2LabelSize = 10; NqSet2LabelBGColor = Brushes.Gray; NqSet2LabelTextColor = Brushes.Yellow; NqSet2LineColor = Brushes.DodgerBlue; NqSet2ZoneOpacity = 80; NqSet2LineWidth = 2; NqSet2LineStyle = DashStyleHelper.Solid; NqSet2LabelAnchor = LabelAnchorPosition.Left;
            NqSet3LabelOffset = 3; NqSet3LabelSize = 10; NqSet3LabelTextColor = Brushes.White; NqMidLineColor = Brushes.Orange; NqLowerLineColor = Brushes.Lime; NqUpperLineColor = Brushes.Red; NqMidLineWidth = 2; NqMidLineStyle = DashStyleHelper.Dash; NqLowerLineWidth = 2; NqLowerLineStyle = DashStyleHelper.Dash; NqUpperLineWidth = 2; NqUpperLineStyle = DashStyleHelper.Dash; NqSet3LabelAnchor = LabelAnchorPosition.Left;
            NqSet4LabelOffset = -5; NqSet4LabelSize = 11; NqSet4LabelTextColor = Brushes.Magenta; NqSet4LineColor = Brushes.White; NqSet4LineWidth = 2; NqSet4LineStyle = DashStyleHelper.Solid; NqSet4LabelAnchor = LabelAnchorPosition.Right; NqSet4LabelXOffset = 100;
            NqSet5LabelOffset = 3; NqSet5LabelSize = 10; NqSet5LabelBGColor = Brushes.Green; NqSet5LabelTextColor = Brushes.White; NqSet5LineColor = Brushes.Green; NqSet5ZoneColor = Brushes.Green; NqSet5ZoneOpacity = 80; NqSet5LineWidth = 2; NqSet5LineStyle = DashStyleHelper.Solid; NqSet5LabelAnchor = LabelAnchorPosition.Left;
            NqSet6LabelOffset = 3; NqSet6LabelSize = 10; NqSet6LabelBGColor = Brushes.Cyan; NqSet6LabelTextColor = Brushes.White; NqSet6LineColor = Brushes.Cyan; NqSet6ZoneColor = Brushes.Cyan; NqSet6ZoneOpacity = 80; NqSet6LineWidth = 2; NqSet6LineStyle = DashStyleHelper.Solid; NqSet6LabelAnchor = LabelAnchorPosition.Left;
            NqSet7LabelOffset = 3; NqSet7LabelSize = 10; NqSet7LabelBGColor = Brushes.Yellow; NqSet7LabelTextColor = Brushes.Black; NqSet7LineColor = Brushes.Yellow; NqSet7ZoneColor = Brushes.Yellow; NqSet7ZoneOpacity = 80; NqSet7LineWidth = 2; NqSet7LineStyle = DashStyleHelper.Solid; NqSet7LabelAnchor = LabelAnchorPosition.Left;
        }

        private void SetEsDefaults()
        {
            EsSet1LabelOffset = 3; EsSet1LabelSize = 10; EsSet1LabelBGColor = Brushes.Green; EsSet1LabelTextColor = Brushes.White; EsSet1LineColor = Brushes.Green; EsSet1ZoneColor = Brushes.Green; EsSet1ZoneOpacity = 80; EsSet1LineWidth = 2; EsSet1LineStyle = DashStyleHelper.Solid; EsSet1LabelAnchor = LabelAnchorPosition.Left;
            EsSet2LabelOffset = 3; EsSet2LabelSize = 10; EsSet2LabelBGColor = Brushes.Gray; EsSet2LabelTextColor = Brushes.Yellow; EsSet2LineColor = Brushes.DodgerBlue; EsSet2ZoneOpacity = 80; EsSet2LineWidth = 2; EsSet2LineStyle = DashStyleHelper.Solid; EsSet2LabelAnchor = LabelAnchorPosition.Left;
            EsSet3LabelOffset = 3; EsSet3LabelSize = 10; EsSet3LabelTextColor = Brushes.White; EsMidLineColor = Brushes.Orange; EsLowerLineColor = Brushes.Lime; EsUpperLineColor = Brushes.Red; EsMidLineWidth = 2; EsMidLineStyle = DashStyleHelper.Dash; EsLowerLineWidth = 2; EsLowerLineStyle = DashStyleHelper.Dash; EsUpperLineWidth = 2; EsUpperLineStyle = DashStyleHelper.Dash; EsSet3LabelAnchor = LabelAnchorPosition.Left;
            EsSet4LabelOffset = -5; EsSet4LabelSize = 11; EsSet4LabelTextColor = Brushes.Magenta; EsSet4LineColor = Brushes.White; EsSet4LineWidth = 2; EsSet4LineStyle = DashStyleHelper.Solid; EsSet4LabelAnchor = LabelAnchorPosition.Right; EsSet4LabelXOffset = 100;
            EsSet5LabelOffset = 3; EsSet5LabelSize = 10; EsSet5LabelBGColor = Brushes.Green; EsSet5LabelTextColor = Brushes.White; EsSet5LineColor = Brushes.Green; EsSet5ZoneColor = Brushes.Green; EsSet5ZoneOpacity = 80; EsSet5LineWidth = 2; EsSet5LineStyle = DashStyleHelper.Solid; EsSet5LabelAnchor = LabelAnchorPosition.Left;
            EsSet6LabelOffset = 3; EsSet6LabelSize = 10; EsSet6LabelBGColor = Brushes.Cyan; EsSet6LabelTextColor = Brushes.White; EsSet6LineColor = Brushes.Cyan; EsSet6ZoneColor = Brushes.Cyan; EsSet6ZoneOpacity = 80; EsSet6LineWidth = 2; EsSet6LineStyle = DashStyleHelper.Solid; EsSet6LabelAnchor = LabelAnchorPosition.Left;
            EsSet7LabelOffset = 3; EsSet7LabelSize = 10; EsSet7LabelBGColor = Brushes.Yellow; EsSet7LabelTextColor = Brushes.Black; EsSet7LineColor = Brushes.Yellow; EsSet7ZoneColor = Brushes.Yellow; EsSet7ZoneOpacity = 80; EsSet7LineWidth = 2; EsSet7LineStyle = DashStyleHelper.Solid; EsSet7LabelAnchor = LabelAnchorPosition.Left;
        }

        private void SetYmDefaults()
        {
            YmSet1LabelOffset = 3; YmSet1LabelSize = 10; YmSet1LabelBGColor = Brushes.Green; YmSet1LabelTextColor = Brushes.White; YmSet1LineColor = Brushes.Green; YmSet1ZoneColor = Brushes.Green; YmSet1ZoneOpacity = 80; YmSet1LineWidth = 2; YmSet1LineStyle = DashStyleHelper.Solid; YmSet1LabelAnchor = LabelAnchorPosition.Left;
            YmSet2LabelOffset = 3; YmSet2LabelSize = 10; YmSet2LabelBGColor = Brushes.Gray; YmSet2LabelTextColor = Brushes.Yellow; YmSet2LineColor = Brushes.DodgerBlue; YmSet2ZoneOpacity = 80; YmSet2LineWidth = 2; YmSet2LineStyle = DashStyleHelper.Solid; YmSet2LabelAnchor = LabelAnchorPosition.Left;
            YmSet3LabelOffset = 3; YmSet3LabelSize = 10; YmSet3LabelTextColor = Brushes.White; YmMidLineColor = Brushes.Orange; YmLowerLineColor = Brushes.Lime; YmUpperLineColor = Brushes.Red; YmMidLineWidth = 2; YmMidLineStyle = DashStyleHelper.Dash; YmLowerLineWidth = 2; YmLowerLineStyle = DashStyleHelper.Dash; YmUpperLineWidth = 2; YmUpperLineStyle = DashStyleHelper.Dash; YmSet3LabelAnchor = LabelAnchorPosition.Left;
            YmSet4LabelOffset = -5; YmSet4LabelSize = 11; YmSet4LabelTextColor = Brushes.Magenta; YmSet4LineColor = Brushes.White; YmSet4LineWidth = 2; YmSet4LineStyle = DashStyleHelper.Solid; YmSet4LabelAnchor = LabelAnchorPosition.Right; YmSet4LabelXOffset = 100;
            YmSet5LabelOffset = 3; YmSet5LabelSize = 10; YmSet5LabelBGColor = Brushes.Green; YmSet5LabelTextColor = Brushes.White; YmSet5LineColor = Brushes.Green; YmSet5ZoneColor = Brushes.Green; YmSet5ZoneOpacity = 80; YmSet5LineWidth = 2; YmSet5LineStyle = DashStyleHelper.Solid; YmSet5LabelAnchor = LabelAnchorPosition.Left;
            YmSet6LabelOffset = 3; YmSet6LabelSize = 10; YmSet6LabelBGColor = Brushes.Cyan; YmSet6LabelTextColor = Brushes.White; YmSet6LineColor = Brushes.Cyan; YmSet6ZoneColor = Brushes.Cyan; YmSet6ZoneOpacity = 80; YmSet6LineWidth = 2; YmSet6LineStyle = DashStyleHelper.Solid; YmSet6LabelAnchor = LabelAnchorPosition.Left;
            YmSet7LabelOffset = 3; YmSet7LabelSize = 10; YmSet7LabelBGColor = Brushes.Yellow; YmSet7LabelTextColor = Brushes.Black; YmSet7LineColor = Brushes.Yellow; YmSet7ZoneColor = Brushes.Yellow; YmSet7ZoneOpacity = 80; YmSet7LineWidth = 2; YmSet7LineStyle = DashStyleHelper.Solid; YmSet7LabelAnchor = LabelAnchorPosition.Left;
        }

        private void SetLisDefaults()
        {
            LisLabelOffset = 3; LisLabelSize = 10; LisLabelBGColor = Brushes.Orange; LisLabelTextColor = Brushes.White; LisLineColor = Brushes.Orange; LisLineWidth = 2; LisLineStyle = DashStyleHelper.Solid; LisLabelAnchor = LabelAnchorPosition.Left;
        }

        protected override void OnBarUpdate()
        {
            if (CurrentBar < 1) return;
            if (IsFirstTickOfBar || CurrentBar >= Bars.Count - 2)
            {
                ClearPreviousDrawings();
                ProcessAndDrawLevels();
            }
        }

        private void ClearPreviousDrawings()
        {
            foreach (var tag in drawingTags) RemoveDrawObject(tag);
            drawingTags.Clear();
        }

        private int GetLeftmostVisibleBarsAgo()
        {
            if (ChartBars != null) return Math.Max(0, Math.Min(CurrentBar - ChartBars.FromIndex, CurrentBar));
            return Math.Min(500, CurrentBar);
        }

        private int GetLabelBarsAgo(LabelAnchorPosition anchor, int xOffset)
        {
            if (anchor == LabelAnchorPosition.Right) return Math.Max(0, Math.Min(xOffset, CurrentBar));
            return Math.Max(0, GetLeftmostVisibleBarsAgo() - xOffset);
        }

        private void LoadLevelsFromFile()
        {
            if (string.IsNullOrEmpty(FullLevelsFilePath) || !File.Exists(FullLevelsFilePath)) return;
            try
            {
                string content = File.ReadAllText(FullLevelsFilePath);
                string[] lines = content.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
                string currentKey = null;
                System.Text.StringBuilder currentData = new System.Text.StringBuilder();
                foreach (string line in lines)
                {
                    string trimmed = line.Trim();
                    if (string.IsNullOrEmpty(trimmed)) continue;
                    string upperLine = trimmed.ToUpper();
                    if (upperLine.Contains(" SET "))
                    {
                        string instrument = null;
                        if (upperLine.StartsWith("NQ")) instrument = "NQ";
                        else if (upperLine.StartsWith("ES")) instrument = "ES";
                        else if (upperLine.StartsWith("YM")) instrument = "YM";
                        if (instrument != null)
                        {
                            for (int i = 1; i <= 7; i++)
                            {
                                if (upperLine.Contains("SET " + i))
                                {
                                    if (currentKey != null) AssignParsedData(currentKey, currentData.ToString().Trim(';'));
                                    currentKey = instrument + "SET" + i;
                                    currentData.Clear();
                                    break;
                                }
                            }
                        }
                    }
                    else if (currentKey != null)
                    {
                        if (currentData.Length > 0 && !currentData.ToString().EndsWith(";")) currentData.Append(";");
                        currentData.Append(trimmed);
                    }
                }
                if (currentKey != null) AssignParsedData(currentKey, currentData.ToString().Trim(';'));
            }
            catch (Exception ex) { Print("Error loading levels: " + ex.Message); }
        }

        private void AssignParsedData(string key, string data)
        {
            switch (key)
            {
                case "NQSET1": NqSet1Data = data; break; case "NQSET2": NqSet2Data = data; break; case "NQSET3": NqSet3Data = data; break; case "NQSET4": NqSet4Data = data; break; case "NQSET5": NqSet5Data = data; break; case "NQSET6": NqSet6Data = data; break; case "NQSET7": NqSet7Data = data; break;
                case "ESSET1": EsSet1Data = data; break; case "ESSET2": EsSet2Data = data; break; case "ESSET3": EsSet3Data = data; break; case "ESSET4": EsSet4Data = data; break; case "ESSET5": EsSet5Data = data; break; case "ESSET6": EsSet6Data = data; break; case "ESSET7": EsSet7Data = data; break;
                case "YMSET1": YmSet1Data = data; break; case "YMSET2": YmSet2Data = data; break; case "YMSET3": YmSet3Data = data; break; case "YMSET4": YmSet4Data = data; break; case "YMSET5": YmSet5Data = data; break; case "YMSET6": YmSet6Data = data; break; case "YMSET7": YmSet7Data = data; break;
            }
        }

        private void ProcessAndDrawLevels()
        {
            if (AutoLoadFromFile) LoadLevelsFromFile();
            DateTime startTime = Time[Math.Min(CurrentBar, 500)];
            DateTime endTime = Time[0].AddDays(10);

            // NQ
            DrawSet1(NqSet1Data, NqShowZones1, NqShowLabels1, "NQ", startTime, endTime, NqSet1LineColor, NqSet1LineStyle, NqSet1LineWidth, NqSet1LabelBGColor, NqSet1ZoneColor, NqSet1ZoneOpacity, NqSet1LabelTextColor, NqSet1LabelSize, NqSet1LabelOffset, NqSet1LabelBold, NqSet1LabelItalic, NqSet1LabelAnchor, NqSet1LabelXOffset);
            DrawSet2(NqSet2Data, NqShowZones2, NqShowLabels2, "NQ", startTime, endTime, NqSet2LineColor, NqSet2LineStyle, NqSet2LineWidth, NqSet2LabelBGColor, NqSet2ZoneOpacity, NqSet2LabelTextColor, NqSet2LabelSize, NqSet2LabelOffset, NqSet2LabelBold, NqSet2LabelItalic, NqSet2LabelAnchor, NqSet2LabelXOffset);
            DrawSet3(NqSet3Data, NqShowKeyLevels3, NqShowKeyLabels3, NqShowTimestamp3, "NQ", startTime, endTime, NqMidLineColor, NqMidLineStyle, NqMidLineWidth, NqLowerLineColor, NqLowerLineStyle, NqLowerLineWidth, NqUpperLineColor, NqUpperLineStyle, NqUpperLineWidth, NqSet3LabelTextColor, NqSet3LabelSize, NqSet3LabelOffset, NqSet3LabelBold, NqSet3LabelItalic, NqSet3LabelAnchor, NqSet3LabelXOffset);
            DrawSet4(NqSet4Data, NqShowZones4, NqShowLabels4, "NQ", startTime, endTime, NqSet4LineColor, NqSet4LineStyle, NqSet4LineWidth, NqSet4LabelTextColor, NqSet4LabelSize, NqSet4LabelOffset, NqSet4LabelBold, NqSet4LabelItalic, NqSet4LabelAnchor, NqSet4LabelXOffset);
            DrawSet5(NqSet5Data, NqShowZones5, NqShowLabels5, "NQ", startTime, endTime, NqSet5LineColor, NqSet5LineStyle, NqSet5LineWidth, NqSet5LabelBGColor, NqSet5ZoneColor, NqSet5ZoneOpacity, NqSet5LabelTextColor, NqSet5LabelSize, NqSet5LabelOffset, NqSet5LabelBold, NqSet5LabelItalic, NqSet5LabelAnchor, NqSet5LabelXOffset);
            DrawSet6(NqSet6Data, NqShowZones6, NqShowLabels6, "NQ", startTime, endTime, NqSet6LineColor, NqSet6LineStyle, NqSet6LineWidth, NqSet6LabelBGColor, NqSet6ZoneColor, NqSet6ZoneOpacity, NqSet6LabelTextColor, NqSet6LabelSize, NqSet6LabelOffset, NqSet6LabelBold, NqSet6LabelItalic, NqSet6LabelAnchor, NqSet6LabelXOffset);
            DrawSet7(NqSet7Data, NqShowZones7, NqShowLabels7, "NQ", startTime, endTime, NqSet7LineColor, NqSet7LineStyle, NqSet7LineWidth, NqSet7LabelBGColor, NqSet7ZoneColor, NqSet7ZoneOpacity, NqSet7LabelTextColor, NqSet7LabelSize, NqSet7LabelOffset, NqSet7LabelBold, NqSet7LabelItalic, NqSet7LabelAnchor, NqSet7LabelXOffset);

            // ES
            DrawSet1(EsSet1Data, EsShowZones1, EsShowLabels1, "ES", startTime, endTime, EsSet1LineColor, EsSet1LineStyle, EsSet1LineWidth, EsSet1LabelBGColor, EsSet1ZoneColor, EsSet1ZoneOpacity, EsSet1LabelTextColor, EsSet1LabelSize, EsSet1LabelOffset, EsSet1LabelBold, EsSet1LabelItalic, EsSet1LabelAnchor, EsSet1LabelXOffset);
            DrawSet2(EsSet2Data, EsShowZones2, EsShowLabels2, "ES", startTime, endTime, EsSet2LineColor, EsSet2LineStyle, EsSet2LineWidth, EsSet2LabelBGColor, EsSet2ZoneOpacity, EsSet2LabelTextColor, EsSet2LabelSize, EsSet2LabelOffset, EsSet2LabelBold, EsSet2LabelItalic, EsSet2LabelAnchor, EsSet2LabelXOffset);
            DrawSet3(EsSet3Data, EsShowKeyLevels3, EsShowKeyLabels3, EsShowTimestamp3, "ES", startTime, endTime, EsMidLineColor, EsMidLineStyle, EsMidLineWidth, EsLowerLineColor, EsLowerLineStyle, EsLowerLineWidth, EsUpperLineColor, EsUpperLineStyle, EsUpperLineWidth, EsSet3LabelTextColor, EsSet3LabelSize, EsSet3LabelOffset, EsSet3LabelBold, EsSet3LabelItalic, EsSet3LabelAnchor, EsSet3LabelXOffset);
            DrawSet4(EsSet4Data, EsShowZones4, EsShowLabels4, "ES", startTime, endTime, EsSet4LineColor, EsSet4LineStyle, EsSet4LineWidth, EsSet4LabelTextColor, EsSet4LabelSize, EsSet4LabelOffset, EsSet4LabelBold, EsSet4LabelItalic, EsSet4LabelAnchor, EsSet4LabelXOffset);
            DrawSet5(EsSet5Data, EsShowZones5, EsShowLabels5, "ES", startTime, endTime, EsSet5LineColor, EsSet5LineStyle, EsSet5LineWidth, EsSet5LabelBGColor, EsSet5ZoneColor, EsSet5ZoneOpacity, EsSet5LabelTextColor, EsSet5LabelSize, EsSet5LabelOffset, EsSet5LabelBold, EsSet5LabelItalic, EsSet5LabelAnchor, EsSet5LabelXOffset);
            DrawSet6(EsSet6Data, EsShowZones6, EsShowLabels6, "ES", startTime, endTime, EsSet6LineColor, EsSet6LineStyle, EsSet6LineWidth, EsSet6LabelBGColor, EsSet6ZoneColor, EsSet6ZoneOpacity, EsSet6LabelTextColor, EsSet6LabelSize, EsSet6LabelOffset, EsSet6LabelBold, EsSet6LabelItalic, EsSet6LabelAnchor, EsSet6LabelXOffset);
            DrawSet7(EsSet7Data, EsShowZones7, EsShowLabels7, "ES", startTime, endTime, EsSet7LineColor, EsSet7LineStyle, EsSet7LineWidth, EsSet7LabelBGColor, EsSet7ZoneColor, EsSet7ZoneOpacity, EsSet7LabelTextColor, EsSet7LabelSize, EsSet7LabelOffset, EsSet7LabelBold, EsSet7LabelItalic, EsSet7LabelAnchor, EsSet7LabelXOffset);

            // YM
            DrawSet1(YmSet1Data, YmShowZones1, YmShowLabels1, "YM", startTime, endTime, YmSet1LineColor, YmSet1LineStyle, YmSet1LineWidth, YmSet1LabelBGColor, YmSet1ZoneColor, YmSet1ZoneOpacity, YmSet1LabelTextColor, YmSet1LabelSize, YmSet1LabelOffset, YmSet1LabelBold, YmSet1LabelItalic, YmSet1LabelAnchor, YmSet1LabelXOffset);
            DrawSet2(YmSet2Data, YmShowZones2, YmShowLabels2, "YM", startTime, endTime, YmSet2LineColor, YmSet2LineStyle, YmSet2LineWidth, YmSet2LabelBGColor, YmSet2ZoneOpacity, YmSet2LabelTextColor, YmSet2LabelSize, YmSet2LabelOffset, YmSet2LabelBold, YmSet2LabelItalic, YmSet2LabelAnchor, YmSet2LabelXOffset);
            DrawSet3(YmSet3Data, YmShowKeyLevels3, YmShowKeyLabels3, YmShowTimestamp3, "YM", startTime, endTime, YmMidLineColor, YmMidLineStyle, YmMidLineWidth, YmLowerLineColor, YmLowerLineStyle, YmLowerLineWidth, YmUpperLineColor, YmUpperLineStyle, YmUpperLineWidth, YmSet3LabelTextColor, YmSet3LabelSize, YmSet3LabelOffset, YmSet3LabelBold, YmSet3LabelItalic, YmSet3LabelAnchor, YmSet3LabelXOffset);
            DrawSet4(YmSet4Data, YmShowZones4, YmShowLabels4, "YM", startTime, endTime, YmSet4LineColor, YmSet4LineStyle, YmSet4LineWidth, YmSet4LabelTextColor, YmSet4LabelSize, YmSet4LabelOffset, YmSet4LabelBold, YmSet4LabelItalic, YmSet4LabelAnchor, YmSet4LabelXOffset);
            DrawSet5(YmSet5Data, YmShowZones5, YmShowLabels5, "YM", startTime, endTime, YmSet5LineColor, YmSet5LineStyle, YmSet5LineWidth, YmSet5LabelBGColor, YmSet5ZoneColor, YmSet5ZoneOpacity, YmSet5LabelTextColor, YmSet5LabelSize, YmSet5LabelOffset, YmSet5LabelBold, YmSet5LabelItalic, YmSet5LabelAnchor, YmSet5LabelXOffset);
            DrawSet6(YmSet6Data, YmShowZones6, YmShowLabels6, "YM", startTime, endTime, YmSet6LineColor, YmSet6LineStyle, YmSet6LineWidth, YmSet6LabelBGColor, YmSet6ZoneColor, YmSet6ZoneOpacity, YmSet6LabelTextColor, YmSet6LabelSize, YmSet6LabelOffset, YmSet6LabelBold, YmSet6LabelItalic, YmSet6LabelAnchor, YmSet6LabelXOffset);
            DrawSet7(YmSet7Data, YmShowZones7, YmShowLabels7, "YM", startTime, endTime, YmSet7LineColor, YmSet7LineStyle, YmSet7LineWidth, YmSet7LabelBGColor, YmSet7ZoneColor, YmSet7ZoneOpacity, YmSet7LabelTextColor, YmSet7LabelSize, YmSet7LabelOffset, YmSet7LabelBold, YmSet7LabelItalic, YmSet7LabelAnchor, YmSet7LabelXOffset);

            ProcessAndDrawLIS(startTime, endTime);
        }

        private void DrawTextWithOffset(string tag, string text, int barsAgo, double price, Brush textColor, int fontSize, int yPixelOffset, bool bold, bool italic)
        {
            var font = new SimpleFont("Arial", fontSize) { Bold = bold, Italic = italic };
            Draw.Text(this, tag, false, text, barsAgo, price, -yPixelOffset, textColor, font, System.Windows.TextAlignment.Left, Brushes.Transparent, Brushes.Transparent, 0);
            drawingTags.Add(tag);
        }

        private void DrawSet1(string rawData, bool showZones, bool showLabels, string symbol, DateTime startTime, DateTime endTime, Brush lineColor, DashStyleHelper lineStyle, int lineWidth, Brush labelBGColor, Brush zoneColor, int zoneOpacity, Brush labelTextColor, int labelFontSize, int labelYOffset, bool labelBold, bool labelItalic, LabelAnchorPosition labelAnchor, int labelXOffset)
        {
            if (!showZones && !showLabels || string.IsNullOrEmpty(rawData)) return;
            foreach (string z in rawData.Split(';')) {
                string trimmed = z.Trim(); if (string.IsNullOrEmpty(trimmed) || trimmed.Contains("_LIS")) continue;
                string[] p = trimmed.Split(',');
                if (p.Length >= 6 && double.TryParse(p[0], out double pTop) && double.TryParse(p[5], out double pBottom)) {
                    double t1=0, t2=0, o1=0, o2=0; double.TryParse(p[1].Replace("T1=",""), out t1); double.TryParse(p[2].Replace("T2=",""), out t2); double.TryParse(p[3].Replace("O1=",""), out o1); double.TryParse(p[4].Replace("O2=",""), out o2);
                    double hi = Math.Max(pTop, pBottom), lo = Math.Min(pTop, pBottom), mid = (hi + lo) / 2;
                    if (showZones) {
                        if (Math.Abs(hi - lo) < 1) { Draw.Line(this, "S1L_"+symbol+"_"+pTop, false, startTime, mid, endTime, mid, lineColor, lineStyle, lineWidth); drawingTags.Add("S1L_"+symbol+"_"+pTop); }
                        else { Draw.Rectangle(this, "S1B_"+symbol+"_"+pTop, false, startTime, hi, endTime, lo, labelBGColor, zoneColor, zoneOpacity); drawingTags.Add("S1B_"+symbol+"_"+pTop); }
                    }
                    if (showLabels) {
                        string txt = "T1="+t1+",T2="+t2+",O1="+o1+",O2="+o2+", = "+(t1+t2+o1+o2)+"\n"+hi.ToString("F2")+" - "+lo.ToString("F2")+" = "+(int)Math.Round(hi-lo);
                        DrawTextWithOffset("S1LB_"+symbol+"_"+pTop, txt, GetLabelBarsAgo(labelAnchor, labelXOffset), hi, labelTextColor, labelFontSize, labelYOffset, labelBold, labelItalic);
                    }
                }
            }
        }

        private void DrawSet2(string rawData, bool showZones, bool showLabels, string symbol, DateTime startTime, DateTime endTime, Brush lineColor, DashStyleHelper lineStyle, int lineWidth, Brush labelBGColor, int zoneOpacity, Brush labelTextColor, int labelFontSize, int labelYOffset, bool labelBold, bool labelItalic, LabelAnchorPosition labelAnchor, int labelXOffset)
        {
            if (!showZones && !showLabels || string.IsNullOrEmpty(rawData)) return;
            foreach (string seg in rawData.Split(';')) {
                string trimmed = seg.Trim(); if (string.IsNullOrEmpty(trimmed) || trimmed.Contains("_LIS")) continue;
                if (trimmed.Contains(",")) {
                    string[] p = trimmed.Split(',');
                    if (double.TryParse(p[0], out double top) && double.TryParse(p[1], out double bottom)) {
                        if (showZones) { Draw.Rectangle(this, "S2B_"+symbol+"_"+top, false, startTime, top, endTime, bottom, labelBGColor, lineColor, zoneOpacity); drawingTags.Add("S2B_"+symbol+"_"+top); }
                        if (showLabels) DrawTextWithOffset("S2LB_"+symbol+"_"+top, Math.Min(top, bottom).ToString("F2")+" - "+Math.Max(top, bottom).ToString("F2"), GetLabelBarsAgo(labelAnchor, labelXOffset), Math.Max(top, bottom), labelTextColor, labelFontSize, labelYOffset, labelBold, labelItalic);
                    }
                } else if (double.TryParse(trimmed, out double level)) {
                    if (showZones) { Draw.Line(this, "S2L_"+symbol+"_"+level, false, startTime, level, endTime, level, lineColor, lineStyle, lineWidth); drawingTags.Add("S2L_"+symbol+"_"+level); }
                    if (showLabels) DrawTextWithOffset("S2LB_"+symbol+"_"+level, level.ToString("F2"), GetLabelBarsAgo(labelAnchor, labelXOffset), level, labelTextColor, labelFontSize, labelYOffset, labelBold, labelItalic);
                }
            }
        }

        private void DrawSet3(string rawData, bool showKeyLevels, bool showKeyLabels, bool showTimestamp, string symbol, DateTime startTime, DateTime endTime, Brush midLineColor, DashStyleHelper midLineStyle, int midLineWidth, Brush lowerLineColor, DashStyleHelper lowerLineStyle, int lowerLineWidth, Brush upperLineColor, DashStyleHelper upperLineStyle, int upperLineWidth, Brush labelTextColor, int labelFontSize, int labelYOffset, bool labelBold, bool labelItalic, LabelAnchorPosition labelAnchor, int labelXOffset)
        {
            if (!showKeyLevels && !showTimestamp || string.IsNullOrEmpty(rawData)) return;
            string ts = null; double? m=null, l=null, u=null;
            string[] parts = rawData.Split('|');
            if (parts.Length > 0 && parts[0].Contains("Time:")) ts = parts[0].Substring(parts[0].IndexOf("Time:")+5).Trim();
            if (parts.Length > 1) {
                foreach (string item in parts[1].Split(',')) {
                    string[] kv = item.Split(':'); if (kv.Length < 2) continue;
                    if (kv[0].Trim() == "Mid" && double.TryParse(kv[1], out double mv)) m=mv;
                    else if (kv[0].Trim() == "Lower" && double.TryParse(kv[1], out double lv)) l=lv;
                    else if (kv[0].Trim() == "Upper" && double.TryParse(kv[1], out double uv)) u=uv;
                }
            }
            if (showTimestamp && !string.IsNullOrEmpty(ts)) DrawTextWithOffset("TS_"+symbol, symbol+" Timestamp: "+ts, GetLabelBarsAgo(labelAnchor, labelXOffset), High[HighestBar(High, Math.Max(1, Math.Min(50, CurrentBar)))], labelTextColor, labelFontSize, labelYOffset, labelBold, labelItalic);
            if (showKeyLevels) {
                if (m.HasValue) { Draw.Line(this, "S3M_"+symbol, false, startTime, m.Value, endTime, m.Value, midLineColor, midLineStyle, midLineWidth); drawingTags.Add("S3M_"+symbol); if (showKeyLabels) DrawTextWithOffset("S3ML_"+symbol, "Mid", GetLabelBarsAgo(labelAnchor, labelXOffset), m.Value, labelTextColor, labelFontSize, labelYOffset, labelBold, labelItalic); }
                if (l.HasValue) { Draw.Line(this, "S3L_"+symbol, false, startTime, l.Value, endTime, l.Value, lowerLineColor, lowerLineStyle, lowerLineWidth); drawingTags.Add("S3L_"+symbol); if (showKeyLabels) DrawTextWithOffset("S3LL_"+symbol, "Lower", GetLabelBarsAgo(labelAnchor, labelXOffset), l.Value, labelTextColor, labelFontSize, labelYOffset, labelBold, labelItalic); }
                if (u.HasValue) { Draw.Line(this, "S3U_"+symbol, false, startTime, u.Value, endTime, u.Value, upperLineColor, upperLineStyle, upperLineWidth); drawingTags.Add("S3U_"+symbol); if (showKeyLabels) DrawTextWithOffset("S3UL_"+symbol, "Upper", GetLabelBarsAgo(labelAnchor, labelXOffset), u.Value, labelTextColor, labelFontSize, labelYOffset, labelBold, labelItalic); }
            }
        }

        private void DrawSet4(string rawData, bool showZones, bool showLabels, string symbol, DateTime startTime, DateTime endTime, Brush lineColor, DashStyleHelper lineStyle, int lineWidth, Brush labelTextColor, int labelFontSize, int labelYOffset, bool labelBold, bool labelItalic, LabelAnchorPosition labelAnchor, int labelXOffset)
        {
            if (!showZones && !showLabels || string.IsNullOrEmpty(rawData)) return;
            foreach (string seg in rawData.Split(';')) {
                string trimmed = seg.Trim(); if (string.IsNullOrEmpty(trimmed) || trimmed.Contains("_LIS")) continue;
                string prc = trimmed, lbl = trimmed;
                if (trimmed.Contains("-")) { string[] p = trimmed.Split('-'); prc = p[0]; lbl = string.Join("-", p.Skip(1)); }
                if (double.TryParse(prc, out double level)) {
                    if (showZones) { Draw.Line(this, "S4L_"+symbol+"_"+level, false, startTime, level, endTime, level, lineColor, lineStyle, lineWidth); drawingTags.Add("S4L_"+symbol+"_"+level); }
                    if (showLabels) DrawTextWithOffset("S4LB_"+symbol+"_"+level, lbl, GetLabelBarsAgo(labelAnchor, labelXOffset), level, labelTextColor, labelFontSize, labelYOffset, labelBold, labelItalic);
                }
            }
        }

        private void DrawSet5(string rawData, bool showZones, bool showLabels, string symbol, DateTime startTime, DateTime endTime, Brush lineColor, DashStyleHelper lineStyle, int lineWidth, Brush labelBGColor, Brush zoneColor, int zoneOpacity, Brush labelTextColor, int labelFontSize, int labelYOffset, bool labelBold, bool labelItalic, LabelAnchorPosition labelAnchor, int labelXOffset) { DrawSet1(rawData, showZones, showLabels, symbol+"_S5", startTime, endTime, lineColor, lineStyle, lineWidth, labelBGColor, zoneColor, zoneOpacity, labelTextColor, labelFontSize, labelYOffset, labelBold, labelItalic, labelAnchor, labelXOffset); }
        private void DrawSet6(string rawData, bool showZones, bool showLabels, string symbol, DateTime startTime, DateTime endTime, Brush lineColor, DashStyleHelper lineStyle, int lineWidth, Brush labelBGColor, Brush zoneColor, int zoneOpacity, Brush labelTextColor, int labelFontSize, int labelYOffset, bool labelBold, bool labelItalic, LabelAnchorPosition labelAnchor, int labelXOffset) { DrawSet1(rawData, showZones, showLabels, symbol+"_S6", startTime, endTime, lineColor, lineStyle, lineWidth, labelBGColor, zoneColor, zoneOpacity, labelTextColor, labelFontSize, labelYOffset, labelBold, labelItalic, labelAnchor, labelXOffset); }
        private void DrawSet7(string rawData, bool showZones, bool showLabels, string symbol, DateTime startTime, DateTime endTime, Brush lineColor, DashStyleHelper lineStyle, int lineWidth, Brush labelBGColor, Brush zoneColor, int zoneOpacity, Brush labelTextColor, int labelFontSize, int labelYOffset, bool labelBold, bool labelItalic, LabelAnchorPosition labelAnchor, int labelXOffset) { DrawSet1(rawData, showZones, showLabels, symbol+"_S7", startTime, endTime, lineColor, lineStyle, lineWidth, labelBGColor, zoneColor, zoneOpacity, labelTextColor, labelFontSize, labelYOffset, labelBold, labelItalic, labelAnchor, labelXOffset); }

        private void ProcessAndDrawLIS(DateTime startTime, DateTime endTime)
        {
            List<string> lisParts = new List<string>();
            string[] datas = { NqSet1Data, NqSet2Data, NqSet3Data, NqSet4Data, NqSet5Data, NqSet6Data, NqSet7Data, EsSet1Data, EsSet2Data, EsSet3Data, EsSet4Data, EsSet5Data, EsSet6Data, EsSet7Data, YmSet1Data, YmSet2Data, YmSet3Data, YmSet4Data, YmSet5Data, YmSet6Data, YmSet7Data };
            foreach (var d in datas) ExtractLIS(d, lisParts);
            if (lisParts.Count > 0) DrawLIS(string.Join(";", lisParts), startTime, endTime, LisLineColor, LisLineStyle, LisLineWidth, LisLabelBGColor, LisLabelTextColor, LisLabelSize, LisLabelOffset, LisLabelBold, LisLabelItalic, LisLabelAnchor, LisLabelXOffset);
        }

        private void ExtractLIS(string rawData, List<string> lisParts) { if (string.IsNullOrEmpty(rawData)) return; foreach (string s in rawData.Split(';')) if (s.Contains("_LIS")) lisParts.Add(s.Replace("_LIS", "")); }

        private void DrawLIS(string rawData, DateTime startTime, DateTime endTime, Brush lineColor, DashStyleHelper lineStyle, int lineWidth, Brush labelBGColor, Brush labelTextColor, int labelFontSize, int labelYOffset, bool labelBold, bool labelItalic, LabelAnchorPosition labelAnchor, int labelXOffset)
        {
            if (string.IsNullOrEmpty(rawData)) return;
            foreach (string seg in rawData.Split(';')) {
                string trimmed = seg.Trim(); if (string.IsNullOrEmpty(trimmed)) continue;
                if (trimmed.Contains(",")) {
                    string[] p = trimmed.Split(','); if (p.Length >= 2 && double.TryParse(p[0], out double t) && double.TryParse(p[1], out double b)) {
                        Draw.Rectangle(this, "LISB_"+t, false, startTime, t, endTime, b, labelBGColor, lineColor, 80); drawingTags.Add("LISB_"+t);
                        DrawTextWithOffset("LISLB_"+t, "LIS", GetLabelBarsAgo(labelAnchor, labelXOffset), Math.Max(t, b), labelTextColor, labelFontSize, labelYOffset, labelBold, labelItalic);
                    }
                } else if (double.TryParse(trimmed, out double level)) {
                    Draw.Line(this, "LISL_"+level, false, startTime, level, endTime, level, lineColor, lineStyle, lineWidth); drawingTags.Add("LISL_"+level);
                    DrawTextWithOffset("LISLB_"+level, "LIS", GetLabelBarsAgo(labelAnchor, labelXOffset), level, labelTextColor, labelFontSize, labelYOffset, labelBold, labelItalic);
                }
            }
        }

        #region Properties
        // [NQ] Data
        [NinjaScriptProperty] [Display(Name="NQ Set 1", GroupName="[NQ] Data", Order=1)] public string NqSet1Data { get; set; }
        [NinjaScriptProperty] [Display(Name="NQ Show Zones 1", GroupName="[NQ] Data", Order=2)] public bool NqShowZones1 { get; set; }
        [NinjaScriptProperty] [Display(Name="NQ Show Labels 1", GroupName="[NQ] Data", Order=3)] public bool NqShowLabels1 { get; set; }
        [NinjaScriptProperty] [Display(Name="NQ Set 2", GroupName="[NQ] Data", Order=4)] public string NqSet2Data { get; set; }
        [NinjaScriptProperty] [Display(Name="NQ Show Zones 2", GroupName="[NQ] Data", Order=5)] public bool NqShowZones2 { get; set; }
        [NinjaScriptProperty] [Display(Name="NQ Show Labels 2", GroupName="[NQ] Data", Order=6)] public bool NqShowLabels2 { get; set; }
        [NinjaScriptProperty] [Display(Name="NQ Set 3", GroupName="[NQ] Data", Order=7)] public string NqSet3Data { get; set; }
        [NinjaScriptProperty] [Display(Name="NQ Show Key Levels 3", GroupName="[NQ] Data", Order=8)] public bool NqShowKeyLevels3 { get; set; }
        [NinjaScriptProperty] [Display(Name="NQ Show Key Labels 3", GroupName="[NQ] Data", Order=9)] public bool NqShowKeyLabels3 { get; set; }
        [NinjaScriptProperty] [Display(Name="NQ Show Timestamp 3", GroupName="[NQ] Data", Order=10)] public bool NqShowTimestamp3 { get; set; }
        [NinjaScriptProperty] [Display(Name="NQ Set 4", GroupName="[NQ] Data", Order=11)] public string NqSet4Data { get; set; }
        [NinjaScriptProperty] [Display(Name="NQ Show Zones 4", GroupName="[NQ] Data", Order=12)] public bool NqShowZones4 { get; set; }
        [NinjaScriptProperty] [Display(Name="NQ Show Labels 4", GroupName="[NQ] Data", Order=13)] public bool NqShowLabels4 { get; set; }
        [NinjaScriptProperty] [Display(Name="NQ Set 5", GroupName="[NQ] Data", Order=14)] public string NqSet5Data { get; set; }
        [NinjaScriptProperty] [Display(Name="NQ Show Zones 5", GroupName="[NQ] Data", Order=15)] public bool NqShowZones5 { get; set; }
        [NinjaScriptProperty] [Display(Name="NQ Show Labels 5", GroupName="[NQ] Data", Order=16)] public bool NqShowLabels5 { get; set; }
        [NinjaScriptProperty] [Display(Name="NQ Set 6", GroupName="[NQ] Data", Order=17)] public string NqSet6Data { get; set; }
        [NinjaScriptProperty] [Display(Name="NQ Show Zones 6", GroupName="[NQ] Data", Order=18)] public bool NqShowZones6 { get; set; }
        [NinjaScriptProperty] [Display(Name="NQ Show Labels 6", GroupName="[NQ] Data", Order=19)] public bool NqShowLabels6 { get; set; }
        [NinjaScriptProperty] [Display(Name="NQ Set 7", GroupName="[NQ] Data", Order=20)] public string NqSet7Data { get; set; }
        [NinjaScriptProperty] [Display(Name="NQ Show Zones 7", GroupName="[NQ] Data", Order=21)] public bool NqShowZones7 { get; set; }
        [NinjaScriptProperty] [Display(Name="NQ Show Labels 7", GroupName="[NQ] Data", Order=22)] public bool NqShowLabels7 { get; set; }

        // [ES] Data
        [NinjaScriptProperty] [Display(Name="ES Set 1", GroupName="[ES] Data", Order=1)] public string EsSet1Data { get; set; }
        [NinjaScriptProperty] [Display(Name="ES Show Zones 1", GroupName="[ES] Data", Order=2)] public bool EsShowZones1 { get; set; }
        [NinjaScriptProperty] [Display(Name="ES Show Labels 1", GroupName="[ES] Data", Order=3)] public bool EsShowLabels1 { get; set; }
        [NinjaScriptProperty] [Display(Name="ES Set 2", GroupName="[ES] Data", Order=4)] public string EsSet2Data { get; set; }
        [NinjaScriptProperty] [Display(Name="ES Show Zones 2", GroupName="[ES] Data", Order=5)] public bool EsShowZones2 { get; set; }
        [NinjaScriptProperty] [Display(Name="ES Show Labels 2", GroupName="[ES] Data", Order=6)] public bool EsShowLabels2 { get; set; }
        [NinjaScriptProperty] [Display(Name="ES Set 3", GroupName="[ES] Data", Order=7)] public string EsSet3Data { get; set; }
        [NinjaScriptProperty] [Display(Name="ES Show Key Levels 3", GroupName="[ES] Data", Order=8)] public bool EsShowKeyLevels3 { get; set; }
        [NinjaScriptProperty] [Display(Name="ES Show Key Labels 3", GroupName="[ES] Data", Order=9)] public bool EsShowKeyLabels3 { get; set; }
        [NinjaScriptProperty] [Display(Name="ES Show Timestamp 3", GroupName="[ES] Data", Order=10)] public bool EsShowTimestamp3 { get; set; }
        [NinjaScriptProperty] [Display(Name="ES Set 4", GroupName="[ES] Data", Order=11)] public string EsSet4Data { get; set; }
        [NinjaScriptProperty] [Display(Name="ES Show Zones 4", GroupName="[ES] Data", Order=12)] public bool EsShowZones4 { get; set; }
        [NinjaScriptProperty] [Display(Name="ES Show Labels 4", GroupName="[ES] Data", Order=13)] public bool EsShowLabels4 { get; set; }
        [NinjaScriptProperty] [Display(Name="ES Set 5", GroupName="[ES] Data", Order=14)] public string EsSet5Data { get; set; }
        [NinjaScriptProperty] [Display(Name="ES Show Zones 5", GroupName="[ES] Data", Order=15)] public bool EsShowZones5 { get; set; }
        [NinjaScriptProperty] [Display(Name="ES Show Labels 5", GroupName="[ES] Data", Order=16)] public bool EsShowLabels5 { get; set; }
        [NinjaScriptProperty] [Display(Name="ES Set 6", GroupName="[ES] Data", Order=17)] public string EsSet6Data { get; set; }
        [NinjaScriptProperty] [Display(Name="ES Show Zones 6", GroupName="[ES] Data", Order=18)] public bool EsShowZones6 { get; set; }
        [NinjaScriptProperty] [Display(Name="ES Show Labels 6", GroupName="[ES] Data", Order=19)] public bool EsShowLabels6 { get; set; }
        [NinjaScriptProperty] [Display(Name="ES Set 7", GroupName="[ES] Data", Order=20)] public string EsSet7Data { get; set; }
        [NinjaScriptProperty] [Display(Name="ES Show Zones 7", GroupName="[ES] Data", Order=21)] public bool EsShowZones7 { get; set; }
        [NinjaScriptProperty] [Display(Name="ES Show Labels 7", GroupName="[ES] Data", Order=22)] public bool EsShowLabels7 { get; set; }

        // [YM] Data
        [NinjaScriptProperty] [Display(Name="YM Set 1", GroupName="[YM] Data", Order=1)] public string YmSet1Data { get; set; }
        [NinjaScriptProperty] [Display(Name="YM Show Zones 1", GroupName="[YM] Data", Order=2)] public bool YmShowZones1 { get; set; }
        [NinjaScriptProperty] [Display(Name="YM Show Labels 1", GroupName="[YM] Data", Order=3)] public bool YmShowLabels1 { get; set; }
        [NinjaScriptProperty] [Display(Name="YM Set 2", GroupName="[YM] Data", Order=4)] public string YmSet2Data { get; set; }
        [NinjaScriptProperty] [Display(Name="YM Show Zones 2", GroupName="[YM] Data", Order=5)] public bool YmShowZones2 { get; set; }
        [NinjaScriptProperty] [Display(Name="YM Show Labels 2", GroupName="[YM] Data", Order=6)] public bool YmShowLabels2 { get; set; }
        [NinjaScriptProperty] [Display(Name="YM Set 3", GroupName="[YM] Data", Order=7)] public string YmSet3Data { get; set; }
        [NinjaScriptProperty] [Display(Name="YM Show Key Levels 3", GroupName="[YM] Data", Order=8)] public bool YmShowKeyLevels3 { get; set; }
        [NinjaScriptProperty] [Display(Name="YM Show Key Labels 3", GroupName="[YM] Data", Order=9)] public bool YmShowKeyLabels3 { get; set; }
        [NinjaScriptProperty] [Display(Name="YM Show Timestamp 3", GroupName="[YM] Data", Order=10)] public bool YmShowTimestamp3 { get; set; }
        [NinjaScriptProperty] [Display(Name="YM Set 4", GroupName="[YM] Data", Order=11)] public string YmSet4Data { get; set; }
        [NinjaScriptProperty] [Display(Name="YM Show Zones 4", GroupName="[YM] Data", Order=12)] public bool YmShowZones4 { get; set; }
        [NinjaScriptProperty] [Display(Name="YM Show Labels 4", GroupName="[YM] Data", Order=13)] public bool YmShowLabels4 { get; set; }
        [NinjaScriptProperty] [Display(Name="YM Set 5", GroupName="[YM] Data", Order=14)] public string YmSet5Data { get; set; }
        [NinjaScriptProperty] [Display(Name="YM Show Zones 5", GroupName="[YM] Data", Order=15)] public bool YmShowZones5 { get; set; }
        [NinjaScriptProperty] [Display(Name="YM Show Labels 5", GroupName="[YM] Data", Order=16)] public bool YmShowLabels5 { get; set; }
        [NinjaScriptProperty] [Display(Name="YM Set 6", GroupName="[YM] Data", Order=17)] public string YmSet6Data { get; set; }
        [NinjaScriptProperty] [Display(Name="YM Show Zones 6", GroupName="[YM] Data", Order=18)] public bool YmShowZones6 { get; set; }
        [NinjaScriptProperty] [Display(Name="YM Show Labels 6", GroupName="[YM] Data", Order=19)] public bool YmShowLabels6 { get; set; }
        [NinjaScriptProperty] [Display(Name="YM Set 7", GroupName="[YM] Data", Order=20)] public string YmSet7Data { get; set; }
        [NinjaScriptProperty] [Display(Name="YM Show Zones 7", GroupName="[YM] Data", Order=21)] public bool YmShowZones7 { get; set; }
        [NinjaScriptProperty] [Display(Name="YM Show Labels 7", GroupName="[YM] Data", Order=22)] public bool YmShowLabels7 { get; set; }

        // File Loading
        [Display(Name="File Path", GroupName="File Loading", Order=1)] public string FullLevelsFilePath { get; set; }
        [Display(Name="Auto-Load", GroupName="File Loading", Order=2)] public bool AutoLoadFromFile { get; set; }

        // [NQ] Styling (Compressed for size)
        #region NQ Styling
        [Display(Name="S1 YOff", GroupName="[NQ] S1 Style")] public int NqSet1LabelOffset { get; set; }
        [Display(Name="S1 Size", GroupName="[NQ] S1 Style")] public int NqSet1LabelSize { get; set; }
        [XmlIgnore] [Display(Name="S1 BG", GroupName="[NQ] S1 Style")] public Brush NqSet1LabelBGColor { get; set; }
        [Browsable(false)] public string NqSet1LabelBGColorSer { get { return Serialize.BrushToString(NqSet1LabelBGColor); } set { NqSet1LabelBGColor = Serialize.StringToBrush(value); } }
        [XmlIgnore] [Display(Name="S1 TXT", GroupName="[NQ] S1 Style")] public Brush NqSet1LabelTextColor { get; set; }
        [Browsable(false)] public string NqSet1LabelTextColorSer { get { return Serialize.BrushToString(NqSet1LabelTextColor); } set { NqSet1LabelTextColor = Serialize.StringToBrush(value); } }
        [XmlIgnore] [Display(Name="S1 LNE", GroupName="[NQ] S1 Style")] public Brush NqSet1LineColor { get; set; }
        [Browsable(false)] public string NqSet1LineColorSer { get { return Serialize.BrushToString(NqSet1LineColor); } set { NqSet1LineColor = Serialize.StringToBrush(value); } }
        [XmlIgnore] [Display(Name="S1 ZNE", GroupName="[NQ] S1 Style")] public Brush NqSet1ZoneColor { get; set; }
        [Browsable(false)] public string NqSet1ZoneColorSer { get { return Serialize.BrushToString(NqSet1ZoneColor); } set { NqSet1ZoneColor = Serialize.StringToBrush(value); } }
        [Display(Name="S1 Opac", GroupName="[NQ] S1 Style")] public int NqSet1ZoneOpacity { get; set; }
        [Display(Name="S1 Width", GroupName="[NQ] S1 Style")] public int NqSet1LineWidth { get; set; }
        [Display(Name="S1 Dash", GroupName="[NQ] S1 Style")] public DashStyleHelper NqSet1LineStyle { get; set; }
        [Display(Name="S1 XOff", GroupName="[NQ] S1 Style")] public int NqSet1LabelXOffset { get; set; }
        [Display(Name="S1 Anch", GroupName="[NQ] S1 Style")] public LabelAnchorPosition NqSet1LabelAnchor { get; set; }
        [Display(Name="S1 Bold", GroupName="[NQ] S1 Style")] public bool NqSet1LabelBold { get; set; }
        [Display(Name="S1 Ital", GroupName="[NQ] S1 Style")] public bool NqSet1LabelItalic { get; set; }

        [Display(Name="S2 YOff", GroupName="[NQ] S2 Style")] public int NqSet2LabelOffset { get; set; }
        [Display(Name="S2 Size", GroupName="[NQ] S2 Style")] public int NqSet2LabelSize { get; set; }
        [XmlIgnore] [Display(Name="S2 BG", GroupName="[NQ] S2 Style")] public Brush NqSet2LabelBGColor { get; set; }
        [Browsable(false)] public string NqSet2LabelBGColorSer { get { return Serialize.BrushToString(NqSet2LabelBGColor); } set { NqSet2LabelBGColor = Serialize.StringToBrush(value); } }
        [XmlIgnore] [Display(Name="S2 TXT", GroupName="[NQ] S2 Style")] public Brush NqSet2LabelTextColor { get; set; }
        [Browsable(false)] public string NqSet2LabelTextColorSer { get { return Serialize.BrushToString(NqSet2LabelTextColor); } set { NqSet2LabelTextColor = Serialize.StringToBrush(value); } }
        [XmlIgnore] [Display(Name="S2 LNE", GroupName="[NQ] S2 Style")] public Brush NqSet2LineColor { get; set; }
        [Browsable(false)] public string NqSet2LineColorSer { get { return Serialize.BrushToString(NqSet2LineColor); } set { NqSet2LineColor = Serialize.StringToBrush(value); } }
        [Display(Name="S2 Opac", GroupName="[NQ] S2 Style")] public int NqSet2ZoneOpacity { get; set; }
        [Display(Name="S2 Width", GroupName="[NQ] S2 Style")] public int NqSet2LineWidth { get; set; }
        [Display(Name="S2 Dash", GroupName="[NQ] S2 Style")] public DashStyleHelper NqSet2LineStyle { get; set; }
        [Display(Name="S2 XOff", GroupName="[NQ] S2 Style")] public int NqSet2LabelXOffset { get; set; }
        [Display(Name="S2 Anch", GroupName="[NQ] S2 Style")] public LabelAnchorPosition NqSet2LabelAnchor { get; set; }
        [Display(Name="S2 Bold", GroupName="[NQ] S2 Style")] public bool NqSet2LabelBold { get; set; }
        [Display(Name="S2 Ital", GroupName="[NQ] S2 Style")] public bool NqSet2LabelItalic { get; set; }

        [Display(Name="S3 YOff", GroupName="[NQ] S3 Style")] public int NqSet3LabelOffset { get; set; }
        [Display(Name="S3 Size", GroupName="[NQ] S3 Style")] public int NqSet3LabelSize { get; set; }
        [XmlIgnore] [Display(Name="S3 TXT", GroupName="[NQ] S3 Style")] public Brush NqSet3LabelTextColor { get; set; }
        [Browsable(false)] public string NqSet3LabelTextColorSer { get { return Serialize.BrushToString(NqSet3LabelTextColor); } set { NqSet3LabelTextColor = Serialize.StringToBrush(value); } }
        [XmlIgnore] [Display(Name="S3 Mid", GroupName="[NQ] S3 Style")] public Brush NqMidLineColor { get; set; }
        [Browsable(false)] public string NqMidLineColorSer { get { return Serialize.BrushToString(NqMidLineColor); } set { NqMidLineColor = Serialize.StringToBrush(value); } }
        [XmlIgnore] [Display(Name="S3 Low", GroupName="[NQ] S3 Style")] public Brush NqLowerLineColor { get; set; }
        [Browsable(false)] public string NqLowerLineColorSer { get { return Serialize.BrushToString(NqLowerLineColor); } set { NqLowerLineColor = Serialize.StringToBrush(value); } }
        [XmlIgnore] [Display(Name="S3 Upp", GroupName="[NQ] S3 Style")] public Brush NqUpperLineColor { get; set; }
        [Browsable(false)] public string NqUpperLineColorSer { get { return Serialize.BrushToString(NqUpperLineColor); } set { NqUpperLineColor = Serialize.StringToBrush(value); } }
        [Display(Name="S3 MidW", GroupName="[NQ] S3 Style")] public int NqMidLineWidth { get; set; }
        [Display(Name="S3 LowW", GroupName="[NQ] S3 Style")] public int NqLowerLineWidth { get; set; }
        [Display(Name="S3 UppW", GroupName="[NQ] S3 Style")] public int NqUpperLineWidth { get; set; }
        [Display(Name="S3 MidS", GroupName="[NQ] S3 Style")] public DashStyleHelper NqMidLineStyle { get; set; }
        [Display(Name="S3 LowS", GroupName="[NQ] S3 Style")] public DashStyleHelper NqLowerLineStyle { get; set; }
        [Display(Name="S3 UppS", GroupName="[NQ] S3 Style")] public DashStyleHelper NqUpperLineStyle { get; set; }
        [Display(Name="S3 XOff", GroupName="[NQ] S3 Style")] public int NqSet3LabelXOffset { get; set; }
        [Display(Name="S3 Anch", GroupName="[NQ] S3 Style")] public LabelAnchorPosition NqSet3LabelAnchor { get; set; }
        [Display(Name="S3 Bold", GroupName="[NQ] S3 Style")] public bool NqSet3LabelBold { get; set; }
        [Display(Name="S3 Ital", GroupName="[NQ] S3 Style")] public bool NqSet3LabelItalic { get; set; }

        [Display(Name="S4 YOff", GroupName="[NQ] S4 Style")] public int NqSet4LabelOffset { get; set; }
        [Display(Name="S4 Size", GroupName="[NQ] S4 Style")] public int NqSet4LabelSize { get; set; }
        [XmlIgnore] [Display(Name="S4 TXT", GroupName="[NQ] S4 Style")] public Brush NqSet4LabelTextColor { get; set; }
        [Browsable(false)] public string NqSet4LabelTextColorSer { get { return Serialize.BrushToString(NqSet4LabelTextColor); } set { NqSet4LabelTextColor = Serialize.StringToBrush(value); } }
        [XmlIgnore] [Display(Name="S4 LNE", GroupName="[NQ] S4 Style")] public Brush NqSet4LineColor { get; set; }
        [Browsable(false)] public string NqSet4LineColorSer { get { return Serialize.BrushToString(NqSet4LineColor); } set { NqSet4LineColor = Serialize.StringToBrush(value); } }
        [Display(Name="S4 Width", GroupName="[NQ] S4 Style")] public int NqSet4LineWidth { get; set; }
        [Display(Name="S4 Dash", GroupName="[NQ] S4 Style")] public DashStyleHelper NqSet4LineStyle { get; set; }
        [Display(Name="S4 XOff", GroupName="[NQ] S4 Style")] public int NqSet4LabelXOffset { get; set; }
        [Display(Name="S4 Anch", GroupName="[NQ] S4 Style")] public LabelAnchorPosition NqSet4LabelAnchor { get; set; }
        [Display(Name="S4 Bold", GroupName="[NQ] S4 Style")] public bool NqSet4LabelBold { get; set; }
        [Display(Name="S4 Ital", GroupName="[NQ] S4 Style")] public bool NqSet4LabelItalic { get; set; }

        [Display(Name="S5 YOff", GroupName="[NQ] S5 Style")] public int NqSet5LabelOffset { get; set; }
        [Display(Name="S5 Size", GroupName="[NQ] S5 Style")] public int NqSet5LabelSize { get; set; }
        [XmlIgnore] [Display(Name="S5 BG", GroupName="[NQ] S5 Style")] public Brush NqSet5LabelBGColor { get; set; }
        [Browsable(false)] public string NqSet5LabelBGColorSer { get { return Serialize.BrushToString(NqSet5LabelBGColor); } set { NqSet5LabelBGColor = Serialize.StringToBrush(value); } }
        [XmlIgnore] [Display(Name="S5 TXT", GroupName="[NQ] S5 Style")] public Brush NqSet5LabelTextColor { get; set; }
        [Browsable(false)] public string NqSet5LabelTextColorSer { get { return Serialize.BrushToString(NqSet5LabelTextColor); } set { NqSet5LabelTextColor = Serialize.StringToBrush(value); } }
        [XmlIgnore] [Display(Name="S5 LNE", GroupName="[NQ] S5 Style")] public Brush NqSet5LineColor { get; set; }
        [Browsable(false)] public string NqSet5LineColorSer { get { return Serialize.BrushToString(NqSet5LineColor); } set { NqSet5LineColor = Serialize.StringToBrush(value); } }
        [XmlIgnore] [Display(Name="S5 ZNE", GroupName="[NQ] S5 Style")] public Brush NqSet5ZoneColor { get; set; }
        [Browsable(false)] public string NqSet5ZoneColorSer { get { return Serialize.BrushToString(NqSet5ZoneColor); } set { NqSet5ZoneColor = Serialize.StringToBrush(value); } }
        [Display(Name="S5 Opac", GroupName="[NQ] S5 Style")] public int NqSet5ZoneOpacity { get; set; }
        [Display(Name="S5 Width", GroupName="[NQ] S5 Style")] public int NqSet5LineWidth { get; set; }
        [Display(Name="S5 Dash", GroupName="[NQ] S5 Style")] public DashStyleHelper NqSet5LineStyle { get; set; }
        [Display(Name="S5 XOff", GroupName="[NQ] S5 Style")] public int NqSet5LabelXOffset { get; set; }
        [Display(Name="S5 Anch", GroupName="[NQ] S5 Style")] public LabelAnchorPosition NqSet5LabelAnchor { get; set; }
        [Display(Name="S5 Bold", GroupName="[NQ] S5 Style")] public bool NqSet5LabelBold { get; set; }
        [Display(Name="S5 Ital", GroupName="[NQ] S5 Style")] public bool NqSet5LabelItalic { get; set; }

        [Display(Name="S6 YOff", GroupName="[NQ] S6 Style")] public int NqSet6LabelOffset { get; set; }
        [Display(Name="S6 Size", GroupName="[NQ] S6 Style")] public int NqSet6LabelSize { get; set; }
        [XmlIgnore] [Display(Name="S6 BG", GroupName="[NQ] S6 Style")] public Brush NqSet6LabelBGColor { get; set; }
        [Browsable(false)] public string NqSet6LabelBGColorSer { get { return Serialize.BrushToString(NqSet6LabelBGColor); } set { NqSet6LabelBGColor = Serialize.StringToBrush(value); } }
        [XmlIgnore] [Display(Name="S6 TXT", GroupName="[NQ] S6 Style")] public Brush NqSet6LabelTextColor { get; set; }
        [Browsable(false)] public string NqSet6LabelTextColorSer { get { return Serialize.BrushToString(NqSet6LabelTextColor); } set { NqSet6LabelTextColor = Serialize.StringToBrush(value); } }
        [XmlIgnore] [Display(Name="S6 LNE", GroupName="[NQ] S6 Style")] public Brush NqSet6LineColor { get; set; }
        [Browsable(false)] public string NqSet6LineColorSer { get { return Serialize.BrushToString(NqSet6LineColor); } set { NqSet6LineColor = Serialize.StringToBrush(value); } }
        [XmlIgnore] [Display(Name="S6 ZNE", GroupName="[NQ] S6 Style")] public Brush NqSet6ZoneColor { get; set; }
        [Browsable(false)] public string NqSet6ZoneColorSer { get { return Serialize.BrushToString(NqSet6ZoneColor); } set { NqSet6ZoneColor = Serialize.StringToBrush(value); } }
        [Display(Name="S6 Opac", GroupName="[NQ] S6 Style")] public int NqSet6ZoneOpacity { get; set; }
        [Display(Name="S6 Width", GroupName="[NQ] S6 Style")] public int NqSet6LineWidth { get; set; }
        [Display(Name="S6 Dash", GroupName="[NQ] S6 Style")] public DashStyleHelper NqSet6LineStyle { get; set; }
        [Display(Name="S6 XOff", GroupName="[NQ] S6 Style")] public int NqSet6LabelXOffset { get; set; }
        [Display(Name="S6 Anch", GroupName="[NQ] S6 Style")] public LabelAnchorPosition NqSet6LabelAnchor { get; set; }
        [Display(Name="S6 Bold", GroupName="[NQ] S6 Style")] public bool NqSet6LabelBold { get; set; }
        [Display(Name="S6 Ital", GroupName="[NQ] S6 Style")] public bool NqSet6LabelItalic { get; set; }

        [Display(Name="S7 YOff", GroupName="[NQ] S7 Style")] public int NqSet7LabelOffset { get; set; }
        [Display(Name="S7 Size", GroupName="[NQ] S7 Style")] public int NqSet7LabelSize { get; set; }
        [XmlIgnore] [Display(Name="S7 BG", GroupName="[NQ] S7 Style")] public Brush NqSet7LabelBGColor { get; set; }
        [Browsable(false)] public string NqSet7LabelBGColorSer { get { return Serialize.BrushToString(NqSet7LabelBGColor); } set { NqSet7LabelBGColor = Serialize.StringToBrush(value); } }
        [XmlIgnore] [Display(Name="S7 TXT", GroupName="[NQ] S7 Style")] public Brush NqSet7LabelTextColor { get; set; }
        [Browsable(false)] public string NqSet7LabelTextColorSer { get { return Serialize.BrushToString(NqSet7LabelTextColor); } set { NqSet7LabelTextColor = Serialize.StringToBrush(value); } }
        [XmlIgnore] [Display(Name="S7 LNE", GroupName="[NQ] S7 Style")] public Brush NqSet7LineColor { get; set; }
        [Browsable(false)] public string NqSet7LineColorSer { get { return Serialize.BrushToString(NqSet7LineColor); } set { NqSet7LineColor = Serialize.StringToBrush(value); } }
        [XmlIgnore] [Display(Name="S7 ZNE", GroupName="[NQ] S7 Style")] public Brush NqSet7ZoneColor { get; set; }
        [Browsable(false)] public string NqSet7ZoneColorSer { get { return Serialize.BrushToString(NqSet7ZoneColor); } set { NqSet7ZoneColor = Serialize.StringToBrush(value); } }
        [Display(Name="S7 Opac", GroupName="[NQ] S7 Style")] public int NqSet7ZoneOpacity { get; set; }
        [Display(Name="S7 Width", GroupName="[NQ] S7 Style")] public int NqSet7LineWidth { get; set; }
        [Display(Name="S7 Dash", GroupName="[NQ] S7 Style")] public DashStyleHelper NqSet7LineStyle { get; set; }
        [Display(Name="S7 XOff", GroupName="[NQ] S7 Style")] public int NqSet7LabelXOffset { get; set; }
        [Display(Name="S7 Anch", GroupName="[NQ] S7 Style")] public LabelAnchorPosition NqSet7LabelAnchor { get; set; }
        [Display(Name="S7 Bold", GroupName="[NQ] S7 Style")] public bool NqSet7LabelBold { get; set; }
        [Display(Name="S7 Ital", GroupName="[NQ] S7 Style")] public bool NqSet7LabelItalic { get; set; }
        #endregion

        // [ES] Styling
        #region ES Styling
        [Display(Name="S1 YOff", GroupName="[ES] S1 Style")] public int EsSet1LabelOffset { get; set; }
        [Display(Name="S1 Size", GroupName="[ES] S1 Style")] public int EsSet1LabelSize { get; set; }
        [XmlIgnore] [Display(Name="S1 BG", GroupName="[ES] S1 Style")] public Brush EsSet1LabelBGColor { get; set; }
        [Browsable(false)] public string EsSet1LabelBGColorSer { get { return Serialize.BrushToString(EsSet1LabelBGColor); } set { EsSet1LabelBGColor = Serialize.StringToBrush(value); } }
        [XmlIgnore] [Display(Name="S1 TXT", GroupName="[ES] S1 Style")] public Brush EsSet1LabelTextColor { get; set; }
        [Browsable(false)] public string EsSet1LabelTextColorSer { get { return Serialize.BrushToString(EsSet1LabelTextColor); } set { EsSet1LabelTextColor = Serialize.StringToBrush(value); } }
        [XmlIgnore] [Display(Name="S1 LNE", GroupName="[ES] S1 Style")] public Brush EsSet1LineColor { get; set; }
        [Browsable(false)] public string EsSet1LineColorSer { get { return Serialize.BrushToString(EsSet1LineColor); } set { EsSet1LineColor = Serialize.StringToBrush(value); } }
        [XmlIgnore] [Display(Name="S1 ZNE", GroupName="[ES] S1 Style")] public Brush EsSet1ZoneColor { get; set; }
        [Browsable(false)] public string EsSet1ZoneColorSer { get { return Serialize.BrushToString(EsSet1ZoneColor); } set { EsSet1ZoneColor = Serialize.StringToBrush(value); } }
        [Display(Name="S1 Opac", GroupName="[ES] S1 Style")] public int EsSet1ZoneOpacity { get; set; }
        [Display(Name="S1 Width", GroupName="[ES] S1 Style")] public int EsSet1LineWidth { get; set; }
        [Display(Name="S1 Dash", GroupName="[ES] S1 Style")] public DashStyleHelper EsSet1LineStyle { get; set; }
        [Display(Name="S1 XOff", GroupName="[ES] S1 Style")] public int EsSet1LabelXOffset { get; set; }
        [Display(Name="S1 Anch", GroupName="[ES] S1 Style")] public LabelAnchorPosition EsSet1LabelAnchor { get; set; }
        [Display(Name="S1 Bold", GroupName="[ES] S1 Style")] public bool EsSet1LabelBold { get; set; }
        [Display(Name="S1 Ital", GroupName="[ES] S1 Style")] public bool EsSet1LabelItalic { get; set; }

        [Display(Name="S2 YOff", GroupName="[ES] S2 Style")] public int EsSet2LabelOffset { get; set; }
        [Display(Name="S2 Size", GroupName="[ES] S2 Style")] public int EsSet2LabelSize { get; set; }
        [XmlIgnore] [Display(Name="S2 BG", GroupName="[ES] S2 Style")] public Brush EsSet2LabelBGColor { get; set; }
        [Browsable(false)] public string EsSet2LabelBGColorSer { get { return Serialize.BrushToString(EsSet2LabelBGColor); } set { EsSet2LabelBGColor = Serialize.StringToBrush(value); } }
        [XmlIgnore] [Display(Name="S2 TXT", GroupName="[ES] S2 Style")] public Brush EsSet2LabelTextColor { get; set; }
        [Browsable(false)] public string EsSet2LabelTextColorSer { get { return Serialize.BrushToString(EsSet2LabelTextColor); } set { EsSet2LabelTextColor = Serialize.StringToBrush(value); } }
        [XmlIgnore] [Display(Name="S2 LNE", GroupName="[ES] S2 Style")] public Brush EsSet2LineColor { get; set; }
        [Browsable(false)] public string EsSet2LineColorSer { get { return Serialize.BrushToString(EsSet2LineColor); } set { EsSet2LineColor = Serialize.StringToBrush(value); } }
        [Display(Name="S2 Opac", GroupName="[ES] S2 Style")] public int EsSet2ZoneOpacity { get; set; }
        [Display(Name="S2 Width", GroupName="[ES] S2 Style")] public int EsSet2LineWidth { get; set; }
        [Display(Name="S2 Dash", GroupName="[ES] S2 Style")] public DashStyleHelper EsSet2LineStyle { get; set; }
        [Display(Name="S2 XOff", GroupName="[ES] S2 Style")] public int EsSet2LabelXOffset { get; set; }
        [Display(Name="S2 Anch", GroupName="[ES] S2 Style")] public LabelAnchorPosition EsSet2LabelAnchor { get; set; }
        [Display(Name="S2 Bold", GroupName="[ES] S2 Style")] public bool EsSet2LabelBold { get; set; }
        [Display(Name="S2 Ital", GroupName="[ES] S2 Style")] public bool EsSet2LabelItalic { get; set; }

        [Display(Name="S3 YOff", GroupName="[ES] S3 Style")] public int EsSet3LabelOffset { get; set; }
        [Display(Name="S3 Size", GroupName="[ES] S3 Style")] public int EsSet3LabelSize { get; set; }
        [XmlIgnore] [Display(Name="S3 TXT", GroupName="[ES] S3 Style")] public Brush EsSet3LabelTextColor { get; set; }
        [Browsable(false)] public string EsSet3LabelTextColorSer { get { return Serialize.BrushToString(EsSet3LabelTextColor); } set { EsSet3LabelTextColor = Serialize.StringToBrush(value); } }
        [XmlIgnore] [Display(Name="S3 Mid", GroupName="[ES] S3 Style")] public Brush EsMidLineColor { get; set; }
        [Browsable(false)] public string EsMidLineColorSer { get { return Serialize.BrushToString(EsMidLineColor); } set { EsMidLineColor = Serialize.StringToBrush(value); } }
        [XmlIgnore] [Display(Name="S3 Low", GroupName="[ES] S3 Style")] public Brush EsLowerLineColor { get; set; }
        [Browsable(false)] public string EsLowerLineColorSer { get { return Serialize.BrushToString(EsLowerLineColor); } set { EsLowerLineColor = Serialize.StringToBrush(value); } }
        [XmlIgnore] [Display(Name="S3 Upp", GroupName="[ES] S3 Style")] public Brush EsUpperLineColor { get; set; }
        [Browsable(false)] public string EsUpperLineColorSer { get { return Serialize.BrushToString(EsUpperLineColor); } set { EsUpperLineColor = Serialize.StringToBrush(value); } }
        [Display(Name="S3 MidW", GroupName="[ES] S3 Style")] public int EsMidLineWidth { get; set; }
        [Display(Name="S3 LowW", GroupName="[ES] S3 Style")] public int EsLowerLineWidth { get; set; }
        [Display(Name="S3 UppW", GroupName="[ES] S3 Style")] public int EsUpperLineWidth { get; set; }
        [Display(Name="S3 MidS", GroupName="[ES] S3 Style")] public DashStyleHelper EsMidLineStyle { get; set; }
        [Display(Name="S3 LowS", GroupName="[ES] S3 Style")] public DashStyleHelper EsLowerLineStyle { get; set; }
        [Display(Name="S3 UppS", GroupName="[ES] S3 Style")] public DashStyleHelper EsUpperLineStyle { get; set; }
        [Display(Name="S3 XOff", GroupName="[ES] S3 Style")] public int EsSet3LabelXOffset { get; set; }
        [Display(Name="S3 Anch", GroupName="[ES] S3 Style")] public LabelAnchorPosition EsSet3LabelAnchor { get; set; }
        [Display(Name="S3 Bold", GroupName="[ES] S3 Style")] public bool EsSet3LabelBold { get; set; }
        [Display(Name="S3 Ital", GroupName="[ES] S3 Style")] public bool EsSet3LabelItalic { get; set; }

        [Display(Name="S4 YOff", GroupName="[ES] S4 Style")] public int EsSet4LabelOffset { get; set; }
        [Display(Name="S4 Size", GroupName="[ES] S4 Style")] public int EsSet4LabelSize { get; set; }
        [XmlIgnore] [Display(Name="S4 TXT", GroupName="[ES] S4 Style")] public Brush EsSet4LabelTextColor { get; set; }
        [Browsable(false)] public string EsSet4LabelTextColorSer { get { return Serialize.BrushToString(EsSet4LabelTextColor); } set { EsSet4LabelTextColor = Serialize.StringToBrush(value); } }
        [XmlIgnore] [Display(Name="S4 LNE", GroupName="[ES] S4 Style")] public Brush EsSet4LineColor { get; set; }
        [Browsable(false)] public string EsSet4LineColorSer { get { return Serialize.BrushToString(EsSet4LineColor); } set { EsSet4LineColor = Serialize.StringToBrush(value); } }
        [Display(Name="S4 Width", GroupName="[ES] S4 Style")] public int EsSet4LineWidth { get; set; }
        [Display(Name="S4 Dash", GroupName="[ES] S4 Style")] public DashStyleHelper EsSet4LineStyle { get; set; }
        [Display(Name="S4 XOff", GroupName="[ES] S4 Style")] public int EsSet4LabelXOffset { get; set; }
        [Display(Name="S4 Anch", GroupName="[ES] S4 Style")] public LabelAnchorPosition EsSet4LabelAnchor { get; set; }
        [Display(Name="S4 Bold", GroupName="[ES] S4 Style")] public bool EsSet4LabelBold { get; set; }
        [Display(Name="S4 Ital", GroupName="[ES] S4 Style")] public bool EsSet4LabelItalic { get; set; }

        [Display(Name="S5 YOff", GroupName="[ES] S5 Style")] public int EsSet5LabelOffset { get; set; }
        [Display(Name="S5 Size", GroupName="[ES] S5 Style")] public int EsSet5LabelSize { get; set; }
        [XmlIgnore] [Display(Name="S5 BG", GroupName="[ES] S5 Style")] public Brush EsSet5LabelBGColor { get; set; }
        [Browsable(false)] public string EsSet5LabelBGColorSer { get { return Serialize.BrushToString(EsSet5LabelBGColor); } set { EsSet5LabelBGColor = Serialize.StringToBrush(value); } }
        [XmlIgnore] [Display(Name="S5 TXT", GroupName="[ES] S5 Style")] public Brush EsSet5LabelTextColor { get; set; }
        [Browsable(false)] public string EsSet5LabelTextColorSer { get { return Serialize.BrushToString(EsSet5LabelTextColor); } set { EsSet5LabelTextColor = Serialize.StringToBrush(value); } }
        [XmlIgnore] [Display(Name="S5 LNE", GroupName="[ES] S5 Style")] public Brush EsSet5LineColor { get; set; }
        [Browsable(false)] public string EsSet5LineColorSer { get { return Serialize.BrushToString(EsSet5LineColor); } set { EsSet5LineColor = Serialize.StringToBrush(value); } }
        [XmlIgnore] [Display(Name="S5 ZNE", GroupName="[ES] S5 Style")] public Brush EsSet5ZoneColor { get; set; }
        [Browsable(false)] public string EsSet5ZoneColorSer { get { return Serialize.BrushToString(EsSet5ZoneColor); } set { EsSet5ZoneColor = Serialize.StringToBrush(value); } }
        [Display(Name="S5 Opac", GroupName="[ES] S5 Style")] public int EsSet5ZoneOpacity { get; set; }
        [Display(Name="S5 Width", GroupName="[ES] S5 Style")] public int EsSet5LineWidth { get; set; }
        [Display(Name="S5 Dash", GroupName="[ES] S5 Style")] public DashStyleHelper EsSet5LineStyle { get; set; }
        [Display(Name="S5 XOff", GroupName="[ES] S5 Style")] public int EsSet5LabelXOffset { get; set; }
        [Display(Name="S5 Anch", GroupName="[ES] S5 Style")] public LabelAnchorPosition EsSet5LabelAnchor { get; set; }
        [Display(Name="S5 Bold", GroupName="[ES] S5 Style")] public bool EsSet5LabelBold { get; set; }
        [Display(Name="S5 Ital", GroupName="[ES] S5 Style")] public bool EsSet5LabelItalic { get; set; }

        [Display(Name="S6 YOff", GroupName="[ES] S6 Style")] public int EsSet6LabelOffset { get; set; }
        [Display(Name="S6 Size", GroupName="[ES] S6 Style")] public int EsSet6LabelSize { get; set; }
        [XmlIgnore] [Display(Name="S6 BG", GroupName="[ES] S6 Style")] public Brush EsSet6LabelBGColor { get; set; }
        [Browsable(false)] public string EsSet6LabelBGColorSer { get { return Serialize.BrushToString(EsSet6LabelBGColor); } set { EsSet6LabelBGColor = Serialize.StringToBrush(value); } }
        [XmlIgnore] [Display(Name="S6 TXT", GroupName="[ES] S6 Style")] public Brush EsSet6LabelTextColor { get; set; }
        [Browsable(false)] public string EsSet6LabelTextColorSer { get { return Serialize.BrushToString(EsSet6LabelTextColor); } set { EsSet6LabelTextColor = Serialize.StringToBrush(value); } }
        [XmlIgnore] [Display(Name="S6 LNE", GroupName="[ES] S6 Style")] public Brush EsSet6LineColor { get; set; }
        [Browsable(false)] public string EsSet6LineColorSer { get { return Serialize.BrushToString(EsSet6LineColor); } set { EsSet6LineColor = Serialize.StringToBrush(value); } }
        [XmlIgnore] [Display(Name="S6 ZNE", GroupName="[ES] S6 Style")] public Brush EsSet6ZoneColor { get; set; }
        [Browsable(false)] public string EsSet6ZoneColorSer { get { return Serialize.BrushToString(EsSet6ZoneColor); } set { EsSet6ZoneColor = Serialize.StringToBrush(value); } }
        [Display(Name="S6 Opac", GroupName="[ES] S6 Style")] public int EsSet6ZoneOpacity { get; set; }
        [Display(Name="S6 Width", GroupName="[ES] S6 Style")] public int EsSet6LineWidth { get; set; }
        [Display(Name="S6 Dash", GroupName="[ES] S6 Style")] public DashStyleHelper EsSet6LineStyle { get; set; }
        [Display(Name="S6 XOff", GroupName="[ES] S6 Style")] public int EsSet6LabelXOffset { get; set; }
        [Display(Name="S6 Anch", GroupName="[ES] S6 Style")] public LabelAnchorPosition EsSet6LabelAnchor { get; set; }
        [Display(Name="S6 Bold", GroupName="[ES] S6 Style")] public bool EsSet6LabelBold { get; set; }
        [Display(Name="S6 Ital", GroupName="[ES] S6 Style")] public bool EsSet6LabelItalic { get; set; }

        [Display(Name="S7 YOff", GroupName="[ES] S7 Style")] public int EsSet7LabelOffset { get; set; }
        [Display(Name="S7 Size", GroupName="[ES] S7 Style")] public int EsSet7LabelSize { get; set; }
        [XmlIgnore] [Display(Name="S7 BG", GroupName="[ES] S7 Style")] public Brush EsSet7LabelBGColor { get; set; }
        [Browsable(false)] public string EsSet7LabelBGColorSer { get { return Serialize.BrushToString(EsSet7LabelBGColor); } set { EsSet7LabelBGColor = Serialize.StringToBrush(value); } }
        [XmlIgnore] [Display(Name="S7 TXT", GroupName="[ES] S7 Style")] public Brush EsSet7LabelTextColor { get; set; }
        [Browsable(false)] public string EsSet7LabelTextColorSer { get { return Serialize.BrushToString(EsSet7LabelTextColor); } set { EsSet7LabelTextColor = Serialize.StringToBrush(value); } }
        [XmlIgnore] [Display(Name="S7 LNE", GroupName="[ES] S7 Style")] public Brush EsSet7LineColor { get; set; }
        [Browsable(false)] public string EsSet7LineColorSer { get { return Serialize.BrushToString(EsSet7LineColor); } set { EsSet7LineColor = Serialize.StringToBrush(value); } }
        [XmlIgnore] [Display(Name="S7 ZNE", GroupName="[ES] S7 Style")] public Brush EsSet7ZoneColor { get; set; }
        [Browsable(false)] public string EsSet7ZoneColorSer { get { return Serialize.BrushToString(EsSet7ZoneColor); } set { EsSet7ZoneColor = Serialize.StringToBrush(value); } }
        [Display(Name="S7 Opac", GroupName="[ES] S7 Style")] public int EsSet7ZoneOpacity { get; set; }
        [Display(Name="S7 Width", GroupName="[ES] S7 Style")] public int EsSet7LineWidth { get; set; }
        [Display(Name="S7 Dash", GroupName="[ES] S7 Style")] public DashStyleHelper EsSet7LineStyle { get; set; }
        [Display(Name="S7 XOff", GroupName="[ES] S7 Style")] public int EsSet7LabelXOffset { get; set; }
        [Display(Name="S7 Anch", GroupName="[ES] S7 Style")] public LabelAnchorPosition EsSet7LabelAnchor { get; set; }
        [Display(Name="S7 Bold", GroupName="[ES] S7 Style")] public bool EsSet7LabelBold { get; set; }
        [Display(Name="S7 Ital", GroupName="[ES] S7 Style")] public bool EsSet7LabelItalic { get; set; }
        #endregion

        // [YM] Styling
        #region YM Styling
        [Display(Name="S1 YOff", GroupName="[YM] S1 Style")] public int YmSet1LabelOffset { get; set; }
        [Display(Name="S1 Size", GroupName="[YM] S1 Style")] public int YmSet1LabelSize { get; set; }
        [XmlIgnore] [Display(Name="S1 BG", GroupName="[YM] S1 Style")] public Brush YmSet1LabelBGColor { get; set; }
        [Browsable(false)] public string YmSet1LabelBGColorSer { get { return Serialize.BrushToString(YmSet1LabelBGColor); } set { YmSet1LabelBGColor = Serialize.StringToBrush(value); } }
        [XmlIgnore] [Display(Name="S1 TXT", GroupName="[YM] S1 Style")] public Brush YmSet1LabelTextColor { get; set; }
        [Browsable(false)] public string YmSet1LabelTextColorSer { get { return Serialize.BrushToString(YmSet1LabelTextColor); } set { YmSet1LabelTextColor = Serialize.StringToBrush(value); } }
        [XmlIgnore] [Display(Name="S1 LNE", GroupName="[YM] S1 Style")] public Brush YmSet1LineColor { get; set; }
        [Browsable(false)] public string YmSet1LineColorSer { get { return Serialize.BrushToString(YmSet1LineColor); } set { YmSet1LineColor = Serialize.StringToBrush(value); } }
        [XmlIgnore] [Display(Name="S1 ZNE", GroupName="[YM] S1 Style")] public Brush YmSet1ZoneColor { get; set; }
        [Browsable(false)] public string YmSet1ZoneColorSer { get { return Serialize.BrushToString(YmSet1ZoneColor); } set { YmSet1ZoneColor = Serialize.StringToBrush(value); } }
        [Display(Name="S1 Opac", GroupName="[YM] S1 Style")] public int YmSet1ZoneOpacity { get; set; }
        [Display(Name="S1 Width", GroupName="[YM] S1 Style")] public int YmSet1LineWidth { get; set; }
        [Display(Name="S1 Dash", GroupName="[YM] S1 Style")] public DashStyleHelper YmSet1LineStyle { get; set; }
        [Display(Name="S1 XOff", GroupName="[YM] S1 Style")] public int YmSet1LabelXOffset { get; set; }
        [Display(Name="S1 Anch", GroupName="[YM] S1 Style")] public LabelAnchorPosition YmSet1LabelAnchor { get; set; }
        [Display(Name="S1 Bold", GroupName="[YM] S1 Style")] public bool YmSet1LabelBold { get; set; }
        [Display(Name="S1 Ital", GroupName="[YM] S1 Style")] public bool YmSet1LabelItalic { get; set; }

        [Display(Name="S2 YOff", GroupName="[YM] S2 Style")] public int YmSet2LabelOffset { get; set; }
        [Display(Name="S2 Size", GroupName="[YM] S2 Style")] public int YmSet2LabelSize { get; set; }
        [XmlIgnore] [Display(Name="S2 BG", GroupName="[YM] S2 Style")] public Brush YmSet2LabelBGColor { get; set; }
        [Browsable(false)] public string YmSet2LabelBGColorSer { get { return Serialize.BrushToString(YmSet2LabelBGColor); } set { YmSet2LabelBGColor = Serialize.StringToBrush(value); } }
        [XmlIgnore] [Display(Name="S2 TXT", GroupName="[YM] S2 Style")] public Brush YmSet2LabelTextColor { get; set; }
        [Browsable(false)] public string YmSet2LabelTextColorSer { get { return Serialize.BrushToString(YmSet2LabelTextColor); } set { YmSet2LabelTextColor = Serialize.StringToBrush(value); } }
        [XmlIgnore] [Display(Name="S2 LNE", GroupName="[YM] S2 Style")] public Brush YmSet2LineColor { get; set; }
        [Browsable(false)] public string YmSet2LineColorSer { get { return Serialize.BrushToString(YmSet2LineColor); } set { YmSet2LineColor = Serialize.StringToBrush(value); } }
        [Display(Name="S2 Opac", GroupName="[YM] S2 Style")] public int YmSet2ZoneOpacity { get; set; }
        [Display(Name="S2 Width", GroupName="[YM] S2 Style")] public int YmSet2LineWidth { get; set; }
        [Display(Name="S2 Dash", GroupName="[YM] S2 Style")] public DashStyleHelper YmSet2LineStyle { get; set; }
        [Display(Name="S2 XOff", GroupName="[YM] S2 Style")] public int YmSet2LabelXOffset { get; set; }
        [Display(Name="S2 Anch", GroupName="[YM] S2 Style")] public LabelAnchorPosition YmSet2LabelAnchor { get; set; }
        [Display(Name="S2 Bold", GroupName="[YM] S2 Style")] public bool YmSet2LabelBold { get; set; }
        [Display(Name="S2 Ital", GroupName="[YM] S2 Style")] public bool YmSet2LabelItalic { get; set; }

        [Display(Name="S3 YOff", GroupName="[YM] S3 Style")] public int YmSet3LabelOffset { get; set; }
        [Display(Name="S3 Size", GroupName="[YM] S3 Style")] public int YmSet3LabelSize { get; set; }
        [XmlIgnore] [Display(Name="S3 TXT", GroupName="[YM] S3 Style")] public Brush YmSet3LabelTextColor { get; set; }
        [Browsable(false)] public string YmSet3LabelTextColorSer { get { return Serialize.BrushToString(YmSet3LabelTextColor); } set { YmSet3LabelTextColor = Serialize.StringToBrush(value); } }
        [XmlIgnore] [Display(Name="S3 Mid", GroupName="[YM] S3 Style")] public Brush YmMidLineColor { get; set; }
        [Browsable(false)] public string YmMidLineColorSer { get { return Serialize.BrushToString(YmMidLineColor); } set { YmMidLineColor = Serialize.StringToBrush(value); } }
        [XmlIgnore] [Display(Name="S3 Low", GroupName="[YM] S3 Style")] public Brush YmLowerLineColor { get; set; }
        [Browsable(false)] public string YmLowerLineColorSer { get { return Serialize.BrushToString(YmLowerLineColor); } set { YmLowerLineColor = Serialize.StringToBrush(value); } }
        [XmlIgnore] [Display(Name="S3 Upp", GroupName="[YM] S3 Style")] public Brush YmUpperLineColor { get; set; }
        [Browsable(false)] public string YmUpperLineColorSer { get { return Serialize.BrushToString(YmUpperLineColor); } set { YmUpperLineColor = Serialize.StringToBrush(value); } }
        [Display(Name="S3 MidW", GroupName="[YM] S3 Style")] public int YmMidLineWidth { get; set; }
        [Display(Name="S3 LowW", GroupName="[YM] S3 Style")] public int YmLowerLineWidth { get; set; }
        [Display(Name="S3 UppW", GroupName="[YM] S3 Style")] public int YmUpperLineWidth { get; set; }
        [Display(Name="S3 MidS", GroupName="[YM] S3 Style")] public DashStyleHelper YmMidLineStyle { get; set; }
        [Display(Name="S3 LowS", GroupName="[YM] S3 Style")] public DashStyleHelper YmLowerLineStyle { get; set; }
        [Display(Name="S3 UppS", GroupName="[YM] S3 Style")] public DashStyleHelper YmUpperLineStyle { get; set; }
        [Display(Name="S3 XOff", GroupName="[YM] S3 Style")] public int YmSet3LabelXOffset { get; set; }
        [Display(Name="S3 Anch", GroupName="[YM] S3 Style")] public LabelAnchorPosition YmSet3LabelAnchor { get; set; }
        [Display(Name="S3 Bold", GroupName="[YM] S3 Style")] public bool YmSet3LabelBold { get; set; }
        [Display(Name="S3 Ital", GroupName="[YM] S3 Style")] public bool YmSet3LabelItalic { get; set; }

        [Display(Name="S4 YOff", GroupName="[YM] S4 Style")] public int YmSet4LabelOffset { get; set; }
        [Display(Name="S4 Size", GroupName="[YM] S4 Style")] public int YmSet4LabelSize { get; set; }
        [XmlIgnore] [Display(Name="S4 TXT", GroupName="[YM] S4 Style")] public Brush YmSet4LabelTextColor { get; set; }
        [Browsable(false)] public string YmSet4LabelTextColorSer { get { return Serialize.BrushToString(YmSet4LabelTextColor); } set { YmSet4LabelTextColor = Serialize.StringToBrush(value); } }
        [XmlIgnore] [Display(Name="S4 LNE", GroupName="[YM] S4 Style")] public Brush YmSet4LineColor { get; set; }
        [Browsable(false)] public string YmSet4LineColorSer { get { return Serialize.BrushToString(YmSet4LineColor); } set { YmSet4LineColor = Serialize.StringToBrush(value); } }
        [Display(Name="S4 Width", GroupName="[YM] S4 Style")] public int YmSet4LineWidth { get; set; }
        [Display(Name="S4 Dash", GroupName="[YM] S4 Style")] public DashStyleHelper YmSet4LineStyle { get; set; }
        [Display(Name="S4 XOff", GroupName="[YM] S4 Style")] public int YmSet4LabelXOffset { get; set; }
        [Display(Name="S4 Anch", GroupName="[YM] S4 Style")] public LabelAnchorPosition YmSet4LabelAnchor { get; set; }
        [Display(Name="S4 Bold", GroupName="[YM] S4 Style")] public bool YmSet4LabelBold { get; set; }
        [Display(Name="S4 Ital", GroupName="[YM] S4 Style")] public bool YmSet4LabelItalic { get; set; }

        [Display(Name="S5 YOff", GroupName="[YM] S5 Style")] public int YmSet5LabelOffset { get; set; }
        [Display(Name="S5 Size", GroupName="[YM] S5 Style")] public int YmSet5LabelSize { get; set; }
        [XmlIgnore] [Display(Name="S5 BG", GroupName="[YM] S5 Style")] public Brush YmSet5LabelBGColor { get; set; }
        [Browsable(false)] public string YmSet5LabelBGColorSer { get { return Serialize.BrushToString(YmSet5LabelBGColor); } set { YmSet5LabelBGColor = Serialize.StringToBrush(value); } }
        [XmlIgnore] [Display(Name="S5 TXT", GroupName="[YM] S5 Style")] public Brush YmSet5LabelTextColor { get; set; }
        [Browsable(false)] public string YmSet5LabelTextColorSer { get { return Serialize.BrushToString(YmSet5LabelTextColor); } set { YmSet5LabelTextColor = Serialize.StringToBrush(value); } }
        [XmlIgnore] [Display(Name="S5 LNE", GroupName="[YM] S5 Style")] public Brush YmSet5LineColor { get; set; }
        [Browsable(false)] public string YmSet5LineColorSer { get { return Serialize.BrushToString(YmSet5LineColor); } set { YmSet5LineColor = Serialize.StringToBrush(value); } }
        [XmlIgnore] [Display(Name="S5 ZNE", GroupName="[YM] S5 Style")] public Brush YmSet5ZoneColor { get; set; }
        [Browsable(false)] public string YmSet5ZoneColorSer { get { return Serialize.BrushToString(YmSet5ZoneColor); } set { YmSet5ZoneColor = Serialize.StringToBrush(value); } }
        [Display(Name="S5 Opac", GroupName="[YM] S5 Style")] public int YmSet5ZoneOpacity { get; set; }
        [Display(Name="S5 Width", GroupName="[YM] S5 Style")] public int YmSet5LineWidth { get; set; }
        [Display(Name="S5 Dash", GroupName="[YM] S5 Style")] public DashStyleHelper YmSet5LineStyle { get; set; }
        [Display(Name="S5 XOff", GroupName="[YM] S5 Style")] public int YmSet5LabelXOffset { get; set; }
        [Display(Name="S5 Anch", GroupName="[YM] S5 Style")] public LabelAnchorPosition YmSet5LabelAnchor { get; set; }
        [Display(Name="S5 Bold", GroupName="[YM] S5 Style")] public bool YmSet5LabelBold { get; set; }
        [Display(Name="S5 Ital", GroupName="[YM] S5 Style")] public bool YmSet5LabelItalic { get; set; }

        [Display(Name="S6 YOff", GroupName="[YM] S6 Style")] public int YmSet6LabelOffset { get; set; }
        [Display(Name="S6 Size", GroupName="[YM] S6 Style")] public int YmSet6LabelSize { get; set; }
        [XmlIgnore] [Display(Name="S6 BG", GroupName="[YM] S6 Style")] public Brush YmSet6LabelBGColor { get; set; }
        [Browsable(false)] public string YmSet6LabelBGColorSer { get { return Serialize.BrushToString(YmSet6LabelBGColor); } set { YmSet6LabelBGColor = Serialize.StringToBrush(value); } }
        [XmlIgnore] [Display(Name="S6 TXT", GroupName="[YM] S6 Style")] public Brush YmSet6LabelTextColor { get; set; }
        [Browsable(false)] public string YmSet6LabelTextColorSer { get { return Serialize.BrushToString(YmSet6LabelTextColor); } set { YmSet6LabelTextColor = Serialize.StringToBrush(value); } }
        [XmlIgnore] [Display(Name="S6 LNE", GroupName="[YM] S6 Style")] public Brush YmSet6LineColor { get; set; }
        [Browsable(false)] public string YmSet6LineColorSer { get { return Serialize.BrushToString(YmSet6LineColor); } set { YmSet6LineColor = Serialize.StringToBrush(value); } }
        [XmlIgnore] [Display(Name="S6 ZNE", GroupName="[YM] S6 Style")] public Brush YmSet6ZoneColor { get; set; }
        [Browsable(false)] public string YmSet6ZoneColorSer { get { return Serialize.BrushToString(YmSet6ZoneColor); } set { YmSet6ZoneColor = Serialize.StringToBrush(value); } }
        [Display(Name="S6 Opac", GroupName="[YM] S6 Style")] public int YmSet6ZoneOpacity { get; set; }
        [Display(Name="S6 Width", GroupName="[YM] S6 Style")] public int YmSet6LineWidth { get; set; }
        [Display(Name="S6 Dash", GroupName="[YM] S6 Style")] public DashStyleHelper YmSet6LineStyle { get; set; }
        [Display(Name="S6 XOff", GroupName="[YM] S6 Style")] public int YmSet6LabelXOffset { get; set; }
        [Display(Name="S6 Anch", GroupName="[YM] S6 Style")] public LabelAnchorPosition YmSet6LabelAnchor { get; set; }
        [Display(Name="S6 Bold", GroupName="[YM] S6 Style")] public bool YmSet6LabelBold { get; set; }
        [Display(Name="S6 Ital", GroupName="[YM] S6 Style")] public bool YmSet6LabelItalic { get; set; }

        [Display(Name="S7 YOff", GroupName="[YM] S7 Style")] public int YmSet7LabelOffset { get; set; }
        [Display(Name="S7 Size", GroupName="[YM] S7 Style")] public int YmSet7LabelSize { get; set; }
        [XmlIgnore] [Display(Name="S7 BG", GroupName="[YM] S7 Style")] public Brush YmSet7LabelBGColor { get; set; }
        [Browsable(false)] public string YmSet7LabelBGColorSer { get { return Serialize.BrushToString(YmSet7LabelBGColor); } set { YmSet7LabelBGColor = Serialize.StringToBrush(value); } }
        [XmlIgnore] [Display(Name="S7 TXT", GroupName="[YM] S7 Style")] public Brush YmSet7LabelTextColor { get; set; }
        [Browsable(false)] public string YmSet7LabelTextColorSer { get { return Serialize.BrushToString(YmSet7LabelTextColor); } set { YmSet7LabelTextColor = Serialize.StringToBrush(value); } }
        [XmlIgnore] [Display(Name="S7 LNE", GroupName="[YM] S7 Style")] public Brush YmSet7LineColor { get; set; }
        [Browsable(false)] public string YmSet7LineColorSer { get { return Serialize.BrushToString(YmSet7LineColor); } set { YmSet7LineColor = Serialize.StringToBrush(value); } }
        [XmlIgnore] [Display(Name="S7 ZNE", GroupName="[YM] S7 Style")] public Brush YmSet7ZoneColor { get; set; }
        [Browsable(false)] public string YmSet7ZoneColorSer { get { return Serialize.BrushToString(YmSet7ZoneColor); } set { YmSet7ZoneColor = Serialize.StringToBrush(value); } }
        [Display(Name="S7 Opac", GroupName="[YM] S7 Style")] public int YmSet7ZoneOpacity { get; set; }
        [Display(Name="S7 Width", GroupName="[YM] S7 Style")] public int YmSet7LineWidth { get; set; }
        [Display(Name="S7 Dash", GroupName="[YM] S7 Style")] public DashStyleHelper YmSet7LineStyle { get; set; }
        [Display(Name="S7 XOff", GroupName="[YM] S7 Style")] public int YmSet7LabelXOffset { get; set; }
        [Display(Name="S7 Anch", GroupName="[YM] S7 Style")] public LabelAnchorPosition YmSet7LabelAnchor { get; set; }
        [Display(Name="S7 Bold", GroupName="[YM] S7 Style")] public bool YmSet7LabelBold { get; set; }
        [Display(Name="S7 Ital", GroupName="[YM] S7 Style")] public bool YmSet7LabelItalic { get; set; }
        #endregion

        // LIS Styling
        #region LIS Styling
        [Display(Name="YOff", GroupName="[LIS] Style")] public int LisLabelOffset { get; set; }
        [Display(Name="Size", GroupName="[LIS] Style")] public int LisLabelSize { get; set; }
        [XmlIgnore] [Display(Name="BG", GroupName="[LIS] Style")] public Brush LisLabelBGColor { get; set; }
        [Browsable(false)] public string LisLabelBGColorSer { get { return Serialize.BrushToString(LisLabelBGColor); } set { LisLabelBGColor = Serialize.StringToBrush(value); } }
        [XmlIgnore] [Display(Name="TXT", GroupName="[LIS] Style")] public Brush LisLabelTextColor { get; set; }
        [Browsable(false)] public string LisLabelTextColorSer { get { return Serialize.BrushToString(LisLabelTextColor); } set { LisLabelTextColor = Serialize.StringToBrush(value); } }
        [XmlIgnore] [Display(Name="LNE", GroupName="[LIS] Style")] public Brush LisLineColor { get; set; }
        [Browsable(false)] public string LisLineColorSer { get { return Serialize.BrushToString(LisLineColor); } set { LisLineColor = Serialize.StringToBrush(value); } }
        [Display(Name="Width", GroupName="[LIS] Style")] public int LisLineWidth { get; set; }
        [Display(Name="Dash", GroupName="[LIS] Style")] public DashStyleHelper LisLineStyle { get; set; }
        [Display(Name="XOff", GroupName="[LIS] Style")] public int LisLabelXOffset { get; set; }
        [Display(Name="Anch", GroupName="[LIS] Style")] public LabelAnchorPosition LisLabelAnchor { get; set; }
        [Display(Name="Bold", GroupName="[LIS] Style")] public bool LisLabelBold { get; set; }
        [Display(Name="Ital", GroupName="[LIS] Style")] public bool LisLabelItalic { get; set; }
        #endregion
        #endregion
    }
}
#region NinjaScript generated code. Neither change nor remove.
namespace NinjaTrader.NinjaScript.Indicators
{
	public partial class Indicator : NinjaTrader.Gui.NinjaScript.IndicatorRenderBase
	{
		private NT8_Levels_Indicator[] cacheNT8_Levels_Indicator;
		public NT8_Levels_Indicator NT8_Levels_Indicator(string nqSet1Data, bool nqShowZones1, bool nqShowLabels1)
		{
			return NT8_Levels_Indicator(Input, nqSet1Data, nqShowZones1, nqShowLabels1);
		}

		public NT8_Levels_Indicator NT8_Levels_Indicator(ISeries<double> input, string nqSet1Data, bool nqShowZones1, bool nqShowLabels1)
		{
			if (cacheNT8_Levels_Indicator != null)
				for (int idx = 0; idx < cacheNT8_Levels_Indicator.Length; idx++)
					if (cacheNT8_Levels_Indicator[idx] != null && cacheNT8_Levels_Indicator[idx].NqSet1Data == nqSet1Data && cacheNT8_Levels_Indicator[idx].NqShowZones1 == nqShowZones1 && cacheNT8_Levels_Indicator[idx].NqShowLabels1 == nqShowLabels1 && cacheNT8_Levels_Indicator[idx].EqualsInput(input))
						return cacheNT8_Levels_Indicator[idx];
			return CacheIndicator<NT8_Levels_Indicator>(new NT8_Levels_Indicator(){ NqSet1Data = nqSet1Data, NqShowZones1 = nqShowZones1, NqShowLabels1 = nqShowLabels1 }, input, ref cacheNT8_Levels_Indicator);
		}
	}
}

namespace NinjaTrader.NinjaScript.MarketAnalyzerColumns
{
	public partial class MarketAnalyzerColumn : MarketAnalyzerColumnBase
	{
		public Indicators.NT8_Levels_Indicator NT8_Levels_Indicator(string nqSet1Data, bool nqShowZones1, bool nqShowLabels1)
		{
			return indicator.NT8_Levels_Indicator(Input, nqSet1Data, nqShowZones1, nqShowLabels1);
		}

		public Indicators.NT8_Levels_Indicator NT8_Levels_Indicator(ISeries<double> input , string nqSet1Data, bool nqShowZones1, bool nqShowLabels1)
		{
			return indicator.NT8_Levels_Indicator(input, nqSet1Data, nqShowZones1, nqShowLabels1);
		}
	}
}

namespace NinjaTrader.NinjaScript.Strategies
{
	public partial class Strategy : NinjaTrader.Gui.NinjaScript.StrategyRenderBase
	{
		public Indicators.NT8_Levels_Indicator NT8_Levels_Indicator(string nqSet1Data, bool nqShowZones1, bool nqShowLabels1)
		{
			return indicator.NT8_Levels_Indicator(Input, nqSet1Data, nqShowZones1, nqShowLabels1);
		}

		public Indicators.NT8_Levels_Indicator NT8_Levels_Indicator(ISeries<double> input , string nqSet1Data, bool nqShowZones1, bool nqShowLabels1)
		{
			return indicator.NT8_Levels_Indicator(input, nqSet1Data, nqShowZones1, nqShowLabels1);
		}
	}
}
#endregion
