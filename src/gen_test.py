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