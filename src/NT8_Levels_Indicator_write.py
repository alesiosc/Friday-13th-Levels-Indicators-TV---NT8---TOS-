# Script to write the complete NT8_Levels_Indicator.cs file
import os

output_path = r'D:/MyPythonProjects_2/NINJATRADER_8_Levels_Indicator/src/NT8_Levels_Indicator.cs'

content = []
w = content.append

w('#region Using declarations')
w('using System;')
w('using System.Collections.Generic;')
w('using System.ComponentModel;')
w('using System.ComponentModel.DataAnnotations;')
w('using System.Linq;')
w('using System.Windows.Media;')
w('using System.Xml.Serialization;')
w('using NinjaTrader.Cbi;')
w('using NinjaTrader.Gui;')
w('using NinjaTrader.Gui.Chart;')
w('using NinjaTrader.Gui.Tools;')
w('using NinjaTrader.Data;')
w('using NinjaTrader.NinjaScript;')
w('using NinjaTrader.NinjaScript.DrawingTools;')
w('#endregion')
w('')
w('namespace NinjaTrader.NinjaScript.Indicators')
w('{')
w('    // Enum for label anchor position')
w('    public enum LabelAnchorPosition')
w('    {')
w('        Left,')
w('        Right')
w('    }')
w('')
w('    public class NT8_Levels_Indicator : Indicator')
w('    {')
w('        private List<string> drawingTags;')
w('')
print('Script file created, length:', len(content))
