#region Using declarations
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Windows.Media;
using System.Xml.Serialization;
using NinjaTrader.Cbi;
using NinjaTrader.Gui;
using NinjaTrader.Gui.Chart;
using NinjaTrader.Gui.Tools;
using NinjaTrader.Data;
using NinjaTrader.NinjaScript;
using NinjaTrader.NinjaScript.DrawingTools;
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
                Description = "NQ+ES Levels Indicator - Set 1-7 + LIS";
                Name = "NT8_Levels_Indicator";
                Calculate = Calculate.OnBarClose;
                IsOverlay = true;
                DisplayInDataBox = false;
                DrawOnPricePanel = true;
                ScaleJustification = ScaleJustification.Right;
                IsSuspendedWhileInactive = true;

                // Draw zones behind candles
                ZOrder = -1;

                // Initialize default data values

                NqSet1Data = "27785.86,T1=1,T2=0,O1=0,O2=1,27785.86;27877.59,T1=1,T2=0,O1=0,O2=1,27877.31;28088.00,T1=1,T2=0,O1=1,O2=0,28084.27;28121.50,T1=1,T2=1,O1=0,O2=1,28114.15;28400.00,T1=1,T2=2,O1=0,O2=0,28394.69;28486.14,T1=1,T2=0,O1=0,O2=0,28486.14;28616.64,T1=1,T2=0,O1=0,O2=0,28616.64;28642.00,T1=1,T2=0,O1=1,O2=1,28639.15;28869.00,T1=1,T2=0,O1=0,O2=0,28869.00";
                NqSet2Data = "28766.24,28607.84;28586.09;28378.75;28178.17_LIS;27986.94;27857.25,27836.50;27426.85,27404.35";
                NqSet3Data = "";
                NqSet4Data = "28200-HvolC;28100-HvolP;28200-UpperPDVR;27800-LowerPDVR;28150-UpperCDVR;27930-LowerCDVR;28100-LastPBlock";

                NqSet5Data = "27785.86,T1=1,T2=0,O1=0,O2=1,27785.86;27877.59,T1=1,T2=0,O1=0,O2=1,27877.31;28088.00,T1=1,T2=0,O1=1,O2=0,28084.27;28121.50,T1=1,T2=1,O1=0,O2=1,28114.15;28400.00,T1=1,T2=2,O1=0,O2=0,28394.69;28486.14,T1=1,T2=0,O1=0,O2=0,28486.14;28616.64,T1=1,T2=0,O1=0,O2=0,28616.64;28642.00,T1=1,T2=0,O1=1,O2=1,28639.15;28869.00,T1=1,T2=0,O1=0,O2=0,28869.00";
                NqSet6Data = "27937.70,T1=0,T2=1,O1=0,O2=2,27937.70;27950.00,T1=1,T2=1,O1=0,O2=0,27950.00;28030.00,T1=1,T2=1,O1=0,O2=2,28030.00;28050.09,T1=1,T2=1,O1=0,O2=0,28050.09;28100.05,T1=0,T2=1,O1=0,O2=1,28099.20;28121.50,T1=1,T2=1,O1=0,O2=1,28114.15;28133.06,T1=0,T2=1,O1=1,O2=1,28125.00;28187.16,T1=1,T2=2,O1=0,O2=2,28185.23;28213.98,T1=0,T2=2,O1=0,O2=2,28213.98;28331.54,T1=0,T2=2,O1=1,O2=2,28331.54;28350.09,T1=0,T2=2,O1=0,O2=1,28350.09;28400.00,T1=1,T2=2,O1=0,O2=0,28394.69;28523.80,T1=1,T2=2,O1=0,O2=2,28523.80";
                NqSet7Data = "27785.86,T1=1,T2=0,O1=0,O2=1,27785.86;27822.12,T1=1,T2=0,O1=0,O2=1,27822.12;27877.59,T1=1,T2=0,O1=0,O2=1,27877.31;27937.70,T1=0,T2=1,O1=0,O2=2,27937.70;27979.74,T1=1,T2=0,O1=0,O2=1,27970.00;27992.18,T1=0,T2=0,O1=1,O2=1,27992.18;28030.00,T1=1,T2=1,O1=0,O2=2,28030.00;28050.09,T1=1,T2=1,O1=0,O2=1,28050.09;28100.05,T1=0,T2=1,O1=0,O2=1,28099.20;28125.13,T1=1,T2=2,O1=0,O2=2,28114.15;28140.60,T1=0,T2=0,O1=2,O2=1,28133.06;28187.16,T1=1,T2=2,O1=0,O2=2,28185.23;28209.28,T1=0,T2=0,O1=1,O2=2,28200.00;28224.65,T1=1,T2=2,O1=0,O2=1,28224.65;28261.44,T1=0,T2=0,O1=1,O2=1,28250.00;28267.16,T1=0,T2=0,O1=1,O2=2,28266.46;28292.35,T1=0,T2=0,O1=2,O2=1,28281.00;28331.54,T1=0,T2=2,O1=1,O2=2,28331.54;28350.09,T1=0,T2=2,O1=0,O2=1,28350.09;28394.69,T1=1,T2=0,O1=0,O2=1,28394.69;28426.12,T1=0,T2=0,O1=1,O2=2,28426.12;28523.80,T1=1,T2=2,O1=0,O2=2,28523.80;28616.64,T1=1,T2=0,O1=1,O2=2,28616.64;28642.00,T1=1,T2=0,O1=1,O2=1,28639.15";

                EsSet1Data = "7196.54,T1=1,T2=0,O1=0,O2=1,7196.54;7220.32,T1=1,T2=0,O1=0,O2=1,7220.25;7275.00,T1=1,T2=0,O1=1,O2=0,7273.85;7281.57,T1=1,T2=1,O1=0,O2=0,7281.57;7286.49,T1=1,T2=0,O1=0,O2=0,7286.49;7355.60,T1=1,T2=2,O1=0,O2=0,7354.25;7377.91,T1=1,T2=0,O1=0,O2=0,7377.91;7411.72,T1=1,T2=0,O1=0,O2=2,7411.72;7417.73,T1=1,T2=0,O1=0,O2=1,7417.56";
                EsSet2Data = "7354.64,7347.64;7337.10;7317.67;7290.92_LIS;7261.93,7252.93;7197.31,7186.31;7089.19,7080.69";
                EsSet3Data = "";
                EsSet4Data = "7300-HvolC;7250-HvolP;7300-UpperPDVR;7250-LowerPDVR;7290-UpperCDVR;7270-LowerCDVR;7300-LastCBlock;6955-LastPBlock";

                EsSet5Data = "7196.54,T1=1,T2=0,O1=0,O2=1,7196.54;7220.32,T1=1,T2=0,O1=0,O2=1,7220.25;7275.00,T1=1,T2=0,O1=1,O2=0,7273.85;7281.57,T1=1,T2=1,O1=0,O2=0,7281.57;7286.49,T1=1,T2=0,O1=0,O2=0,7286.49;7355.60,T1=1,T2=2,O1=0,O2=0,7354.25;7377.91,T1=1,T2=0,O1=0,O2=0,7377.91;7411.72,T1=1,T2=0,O1=0,O2=2,7411.72;7417.73,T1=1,T2=0,O1=0,O2=1,7417.56";
                EsSet6Data = "7235.87,T1=0,T2=1,O1=0,O2=1,7235.87;7239.05,T1=1,T2=1,O1=0,O2=0,7239.05;7260.00,T1=1,T2=1,O1=0,O2=0,7260.00;7265.00,T1=1,T2=1,O1=0,O2=0,7265.00;7281.57,T1=1,T2=1,O1=0,O2=0,7281.57;7300.50,T1=1,T2=2,O1=0,O2=1,7300.00;7337.89,T1=0,T2=2,O1=0,O2=1,7337.89;7342.70,T1=0,T2=2,O1=0,O2=1,7342.70;7355.60,T1=1,T2=2,O1=0,O2=0,7354.25";
                EsSet7Data = "7199.62,T1=1,T2=0,O1=0,O2=2,7196.54;7205.95,T1=1,T2=0,O1=0,O2=1,7205.95;7220.32,T1=1,T2=0,O1=0,O2=1,7220.25;7239.05,T1=1,T2=2,O1=0,O2=1,7239.05;7265.00,T1=2,T2=2,O1=0,O2=1,7260.00;7303.80,T1=1,T2=2,O1=1,O2=1,7300.00;7310.21,T1=1,T2=0,O1=0,O2=2,7310.21;7321.20,T1=0,T2=0,O1=1,O2=3,7321.04;7340.75,T1=0,T2=2,O1=0,O2=2,7337.89;7354.25,T1=1,T2=0,O1=1,O2=1,7354.25;7360.00,T1=1,T2=2,O1=0,O2=2,7358.19;7411.72,T1=1,T2=0,O1=0,O2=2,7411.72;7417.73,T1=1,T2=0,O1=0,O2=1,7417.56";

                // Default visibility - Sets 1-4
                NqShowZones1 = true; NqShowLabels1 = true;
                NqShowZones2 = true; NqShowLabels2 = true;
                NqShowKeyLevels3 = true; NqShowKeyLabels3 = true; NqShowTimestamp3 = true;
                NqShowZones4 = true; NqShowLabels4 = true;
                EsShowZones1 = true; EsShowLabels1 = true;
                EsShowZones2 = true; EsShowLabels2 = true;
                EsShowKeyLevels3 = true; EsShowKeyLabels3 = true; EsShowTimestamp3 = true;
                EsShowZones4 = true; EsShowLabels4 = true;

                // Default visibility - Sets 5, 6, 7
                NqShowZones5 = true; NqShowLabels5 = true;
                NqShowZones6 = true; NqShowLabels6 = true;
                NqShowZones7 = true; NqShowLabels7 = true;
                EsShowZones5 = true; EsShowLabels5 = true;
                EsShowZones6 = true; EsShowLabels6 = true;
                EsShowZones7 = true; EsShowLabels7 = true;

                // Per-set label anchors (default all to Left)
                Set1LabelAnchor = LabelAnchorPosition.Left;
                Set2LabelAnchor = LabelAnchorPosition.Left;
                Set3LabelAnchor = LabelAnchorPosition.Left;
                Set4LabelAnchor = LabelAnchorPosition.Left;
                Set5LabelAnchor = LabelAnchorPosition.Left;
                Set6LabelAnchor = LabelAnchorPosition.Left;
                Set7LabelAnchor = LabelAnchorPosition.Left;
                LisLabelAnchor = LabelAnchorPosition.Left;

                // Set 1 Styling
                Set1LabelOffset = 3;
                Set1LabelSize = 10;
                Set1LabelBold = false;
                Set1LabelItalic = false;
                Set1LabelXOffset = 0;
                Set1LabelBGColor = Brushes.Green;
                Set1LabelTextColor = Brushes.White;
                Set1LineColor = Brushes.Green;
                Set1ZoneColor = Brushes.Green;
                Set1ZoneOpacity = 80;
                Set1LineWidth = 2;
                Set1LineStyle = DashStyleHelper.Solid;

                // Set 2 Styling
                Set2LabelOffset = 3;
                Set2LabelSize = 10;
                Set2LabelBold = false;
                Set2LabelItalic = false;
                Set2LabelXOffset = 0;
                Set2LabelBGColor = Brushes.Gray;
                Set2LabelTextColor = Brushes.Yellow;
                Set2LineColor = Brushes.DodgerBlue;
                Set2ZoneOpacity = 80;
                Set2LineWidth = 2;
                Set2LineStyle = DashStyleHelper.Solid;

                // Set 3 Styling
                Set3LabelOffset = 3;
                Set3LabelSize = 10;
                Set3LabelBold = false;
                Set3LabelItalic = false;
                Set3LabelXOffset = 0;
                Set3LabelBGColor = Brushes.Orange;
                Set3LabelTextColor = Brushes.Black;
                MidLineColor = Brushes.Orange;
                MidLineWidth = 2;
                MidLineStyle = DashStyleHelper.Dash;
                LowerLineColor = Brushes.Lime;
                LowerLineWidth = 2;
                LowerLineStyle = DashStyleHelper.Dash;
                UpperLineColor = Brushes.Red;
                UpperLineWidth = 2;
                UpperLineStyle = DashStyleHelper.Dash;

                // Set 4 Styling
                Set4LabelOffset = -5;
                Set4LabelSize = 11;
                Set4LabelBold = false;
                Set4LabelItalic = false;
                Set4LabelXOffset = 100;
                Set4LabelBGColor = Brushes.White;
                Set4LabelTextColor = Brushes.Magenta;
                Set4LineColor = Brushes.White;
                Set4LineWidth = 2;
                Set4LineStyle = DashStyleHelper.Solid;

                // Set 5 Styling (Extremes)
                Set5LabelOffset = 3;
                Set5LabelSize = 10;
                Set5LabelBold = false;
                Set5LabelItalic = false;
                Set5LabelXOffset = 0;
                Set5LabelBGColor = Brushes.Green;
                Set5LabelTextColor = Brushes.White;
                Set5LineColor = Brushes.Green;
                Set5ZoneColor = Brushes.Green;
                Set5ZoneOpacity = 80;
                Set5LineWidth = 2;
                Set5LineStyle = DashStyleHelper.Solid;

                // Set 6 Styling (Scalping)
                Set6LabelOffset = 3;
                Set6LabelSize = 10;
                Set6LabelBold = false;
                Set6LabelItalic = false;
                Set6LabelXOffset = 0;
                Set6LabelBGColor = Brushes.Cyan;
                Set6LabelTextColor = Brushes.Black;
                Set6LineColor = Brushes.Cyan;
                Set6ZoneColor = Brushes.Cyan;
                Set6ZoneOpacity = 80;
                Set6LineWidth = 2;
                Set6LineStyle = DashStyleHelper.Solid;

                // Set 7 Styling (Confluence)
                Set7LabelOffset = 3;
                Set7LabelSize = 10;
                Set7LabelBold = false;
                Set7LabelItalic = false;
                Set7LabelXOffset = 0;
                Set7LabelBGColor = Brushes.Yellow;
                Set7LabelTextColor = Brushes.Black;
                Set7LineColor = Brushes.Yellow;
                Set7ZoneColor = Brushes.Yellow;
                Set7ZoneOpacity = 80;
                Set7LineWidth = 2;
                Set7LineStyle = DashStyleHelper.Solid;

                // LIS Styling
                LisLabelOffset = 3;
                LisLabelSize = 10;
                LisLabelBold = false;
                LisLabelItalic = false;
                LisLabelXOffset = 0;
                LisLabelBGColor = Brushes.Orange;
                LisLabelTextColor = Brushes.White;
                LisLineColor = Brushes.Orange;
                LisLineWidth = 2;
                LisLineStyle = DashStyleHelper.Solid;
            }
            else if (State == State.DataLoaded)
            {
                drawingTags = new List<string>();
            }
            else if (State == State.Historical)
            {
                // Set ZOrder to draw indicator behind price bars/candles
                SetZOrder(-1);
            }
        }

        protected override void OnBarUpdate()
        {
            if (CurrentBar < 1) return;

            // Always redraw on the last bar to ensure property changes update the display
            if (IsFirstTickOfBar || CurrentBar >= Bars.Count - 2)
            {
                ClearPreviousDrawings();
                ProcessAndDrawLevels();
            }
        }