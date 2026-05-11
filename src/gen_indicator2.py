import os

output_path = r'D:/MyPythonProjects_2/NINJATRADER_8_Levels_Indicator/src/NT8_Levels_Indicator.cs'

NL = chr(10)
lines = []

def w(s):
    lines.append(s)

def blank():
    lines.append("")

w("")
w("        private void ClearPreviousDrawings()")
w("        {")
w("            foreach (var tag in drawingTags)")
w("            {")
w("                RemoveDrawObject(tag);")
w("            }")
w("            drawingTags.Clear();")
w("        }")
blank()
w("        private int GetLeftmostVisibleBarsAgo()")
w("        {")
w("            if (ChartBars != null)")
w("            {")
w("                int fromIndex = ChartBars.FromIndex;")
w("                int barsAgo = CurrentBar - fromIndex;")
w("                return Math.Max(0, Math.Min(barsAgo, CurrentBar));")
w("            }")
w("            return Math.Min(500, CurrentBar);")
w("        }")
blank()
w("        private int GetLabelBarsAgo(LabelAnchorPosition anchor, int xOffset)")
w("        {")
w("            if (anchor == LabelAnchorPosition.Right)")
w("            {")
w("                // Anchor to right: barsAgo = 0 (current bar) + offset moves left")
w("                return Math.Max(0, Math.Min(xOffset, CurrentBar));")
w("            }")
w("            else")
w("            {")
w("                // Anchor to left: far left of chart - offset moves right")
w("                return Math.Max(0, GetLeftmostVisibleBarsAgo() - xOffset);")
w("            }")
w("        }")

with open(r'D:/MyPythonProjects_2/NINJATRADER_8_Levels_Indicator/src/gen_part2.py', 'w', encoding='utf-8') as f:
    f.write(NL.join(lines))
print(f"Part 2: {len(lines)} lines")
