
        private void ClearPreviousDrawings()
        {
            foreach (var tag in drawingTags)
            {
                RemoveDrawObject(tag);
            }
            drawingTags.Clear();
        }

        private int GetLeftmostVisibleBarsAgo()
        {
            if (ChartBars != null)
            {
                int fromIndex = ChartBars.FromIndex;
                int barsAgo = CurrentBar - fromIndex;
                return Math.Max(0, Math.Min(barsAgo, CurrentBar));
            }
            return Math.Min(500, CurrentBar);
        }

        private int GetLabelBarsAgo(LabelAnchorPosition anchor, int xOffset)
        {
            if (anchor == LabelAnchorPosition.Right)
            {
                // Anchor to right: barsAgo = 0 (current bar) + offset moves left
                return Math.Max(0, Math.Min(xOffset, CurrentBar));
            }
            else
            {
                // Anchor to left: far left of chart - offset moves right
                return Math.Max(0, GetLeftmostVisibleBarsAgo() - xOffset);
            }
        }