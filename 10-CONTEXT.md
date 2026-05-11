//@version=5
indicator('💖Friday 13th💖 NQ+ES-SET 1-3+BKBrown - v13.5 (Color Coded-2)', overlay = true)
// =====================================================================
// 1. INPUT SETTINGS
// =====================================================================
// --- NQ Data Inputs ---
string rawData1_NQ = input.string('', 'Set 1 Data', group = 'NQ Data Inputs', inline = 'nq1')
bool showZones1_NQ = input.bool(true, 'Zones', group = 'NQ Data Inputs', inline = 'nq1')
bool showLabels1_NQ = input.bool(true, 'Labels', group = 'NQ Data Inputs', inline = 'nq1')
string rawData2_NQ = input.string('24150;24003.75', 'Set 2 Data', group = 'NQ Data Inputs', inline = 'nq2')
bool showZones2_NQ = input.bool(true, 'Zones', group = 'NQ Data Inputs', inline = 'nq2')
bool showLabels2_NQ = input.bool(true, 'Labels', group = 'NQ Data Inputs', inline = 'nq2')
string rawData3_NQ = input.string('[NQ] Time: 2025-09-16 12:13:38 | Mid: 24615, Lower: 24532, Upper: 24697', 'Set 3 Data', group = 'NQ Data Inputs', inline = 'nq3')
bool showKeyLevels3_NQ = input.bool(true, 'Levels', group = 'NQ Data Inputs', inline = 'nq3')
bool showKeyLabels3_NQ = input.bool(true, 'Labels', group = 'NQ Data Inputs', inline = 'nq3')
bool showTimestamp3_NQ = input.bool(true, 'Timestamp', group = 'NQ Data Inputs', inline = 'nq3')
string rawData4_NQ = input.string('24150-HvolC;24150-UpperPDVR', 'Set 4 (BKBrown)', group = 'NQ Data Inputs', inline = 'nq4')
bool showZones4_NQ = input.bool(true, 'Zones', group = 'NQ Data Inputs', inline = 'nq4')
bool showLabels4_NQ = input.bool(true, 'Labels', group = 'NQ Data Inputs', inline = 'nq4')
// --- ES Data Inputs ---
string rawData1_ES = input.string('', 'Set 1 Data', group = 'ES Data Inputs', inline = 'es1')
bool showZones1_ES = input.bool(true, 'Zones', group = 'ES Data Inputs', inline = 'es1')
bool showLabels1_ES = input.bool(true, 'Labels', group = 'ES Data Inputs', inline = 'es1')
string rawData2_ES = input.string('', 'Set 2 Data', group = 'ES Data Inputs', inline = 'es2')
bool showZones2_ES = input.bool(true, 'Zones', group = 'ES Data Inputs', inline = 'es2')
bool showLabels2_ES = input.bool(true, 'Labels', group = 'ES Data Inputs', inline = 'es2')
string rawData3_ES = input.string('', 'Set 3 Data', group = 'ES Data Inputs', inline = 'es3')
bool showKeyLevels3_ES = input.bool(true, 'Levels', group = 'ES Data Inputs', inline = 'es3')
bool showKeyLabels3_ES = input.bool(true, 'Labels', group = 'ES Data Inputs', inline = 'es3')
bool showTimestamp3_ES = input.bool(true, 'Timestamp', group = 'ES Data Inputs', inline = 'es3')
string rawData4_ES = input.string('', 'Set 4 (BKBrown)', group = 'ES Data Inputs', inline = 'es4')
bool showZones4_ES = input.bool(true, 'Zones', group = 'ES Data Inputs', inline = 'es4')
bool showLabels4_ES = input.bool(true, 'Labels', group = 'ES Data Inputs', inline = 'es4')

// --- Set 1 Styling ---
int set1LabelOffset = input.int(50, 'Label Offset', group = 'Set 1 Styling')
float avgCharWidthInBars = input.float(1.4, "Average Char Width (Bars)", step=0.1, group="Set 1 Styling", tooltip="Controls the spacing for side-by-side labels. Reduce if labels are too far apart, increase if they overlap.")
string set1LabelSize = input.string('small', 'Label Size', options = ['tiny', 'small', 'normal', 'large', 'huge'], group = 'Set 1 Styling')
color set1LabelBGColor = input.color(color.new(color.rgb(68, 165, 72), 75), 'Label BG Color', group = 'Set 1 Styling')
color set1LabelTextColor = input.color(color.black, 'Label Text Color', group = 'Set 1 Styling')
color set1ZoneColor = input.color(color.rgb(68, 165, 72), 'Zone Color', group = 'Set 1 Styling')
int set1ZoneOpacity = input.int(80, 'Zone Opacity', minval = 0, maxval = 100, group = 'Set 1 Styling')

// --- Set 2 Styling ---
int set2LabelOffset = input.int(25, 'Label Offset', group = 'Set 2 Styling')
string set2LabelSize = input.string('small', 'Label Size', options = ['tiny', 'small', 'normal', 'large', 'huge'], group = 'Set 2 Styling')
color set2LabelBGColor = input.color(color.new(color.rgb(99, 99, 99), 75), 'Label BG Color', group = 'Set 2 Styling')
color set2LabelTextColor = input.color(color.black, 'Label Text Color', group = 'Set 2 Styling')
color set2LineColor = input.color(color.rgb(99, 99, 99), 'Line/Zone Color', group = 'Set 2 Styling')
int set2ZoneOpacity = input.int(80, 'Zone Opacity', minval = 0, maxval = 100, group = 'Set 2 Styling')
int set2LineWidth = input.int(3, 'Line Width', minval = 1, maxval = 10, group = 'Set 2 Styling')
string set2LineStyle = input.string('solid', 'Line Style', options = ['solid', 'dashed', 'dotted'], group = 'Set 2 Styling')

// --- Set 3 (Timestamp) Styling ---
string timestampPosition = input.string("Bottom Left", "Timestamp Position", options = ["Top Left", "Top Right", "Bottom Left", "Bottom Right"], group = "Set 3 (Timestamp) Styling")
int set3LabelOffset = input.int(-24, 'Label Offset', group = 'Set 3 (Timestamp) Styling')
string set3LabelSize = input.string('small', 'Label Size', options = ['tiny', 'small', 'normal', 'large', 'huge'], group = 'Set 3 (Timestamp) Styling')
color set3LabelBGColor = input.color(color.new(color.orange, 75), 'Label BG Color', group = 'Set 3 (Timestamp) Styling')
color set3LabelTextColor = input.color(color.black, 'Label Text Color', group = 'Set 3 (Timestamp) Styling')
color midLineColor = input.color(color.new(color.orange, 0), 'Mid Line Color', group = 'Set 3 (Timestamp) Styling', inline = 'mid')
int midLineWidth = input.int(2, 'Width', minval = 1, maxval = 10, group = 'Set 3 (Timestamp) Styling', inline = 'mid')
string midLineStyle = input.string('dashed', 'Style', options = ['solid', 'dashed', 'dotted'], group = 'Set 3 (Timestamp) Styling', inline = 'mid')
color lowerLineColor = input.color(color.new(color.red, 0), 'Lower Line Color', group = 'Set 3 (Timestamp) Styling', inline = 'lower')
int lowerLineWidth = input.int(2, 'Width', minval = 1, maxval = 10, group = 'Set 3 (Timestamp) Styling', inline = 'lower')
string lowerLineStyle = input.string('dashed', 'Style', options = ['solid', 'dashed', 'dotted'], group = 'Set 3 (Timestamp) Styling', inline = 'lower')
color upperLineColor = input.color(color.new(color.green, 0), 'Upper Line Color', group = 'Set 3 (Timestamp) Styling', inline = 'upper')
int upperLineWidth = input.int(2, 'Width', minval = 1, maxval = 10, group = 'Set 3 (Timestamp) Styling', inline = 'upper')
string upperLineStyle = input.string('dashed', 'Style', options = ['solid', 'dashed', 'dotted'], group = 'Set 3 (Timestamp) Styling', inline = 'upper')

// --- Set 4 (BKBrowns) Styling ---
int set4LabelOffset = input.int(-24, 'Label Offset', group = 'Set 4 (BKBrowns) Styling')
string set4LabelSize = input.string('small', 'Label Size', options = ['tiny', 'small', 'normal', 'large', 'huge'], group = 'Set 4 (BKBrowns) Styling')
color set4LabelBGColor = input.color(color.new(color.white, 75), 'Label BG Color', group = 'Set 4 (BKBrowns) Styling')
color set4LabelTextColor = input.color(color.black, 'Label Text Color', group = 'Set 4 (BKBrowns) Styling')
color set4LineColor = input.color(color.new(color.white, 40), 'Line Color', group = 'Set 4 (BKBrowns) Styling')
int set4LineWidth = input.int(3, 'Line Width', minval = 1, maxval = 10, group = 'Set 4 (BKBrowns) Styling')
string set4LineStyle = input.string('solid', 'Line Style', options = ['solid', 'dashed', 'dotted'], group = 'Set 4 (BKBrowns) Styling')

// --- LIS Styling ---
int lisLabelOffset = input.int(-27, 'Label Offset', group = 'LIS Styling')
string lisLabelSize = input.string('small', 'Label Size', options = ['tiny', 'small', 'normal', 'large', 'huge'], group = 'LIS Styling')
color lisLabelBGColor = input.color(color.new(#ff9800, 50), 'Label BG Color', group = 'LIS Styling')
color lisLabelTextColor = input.color(color.white, 'Label Text Color', group = 'LIS Styling')
color lisLineColor = input.color(color.new(#ff9800, 30), 'Line/Zone Color', group = 'LIS Styling')

// =====================================================================
// 2. TRACK DRAWING OBJECTS
// =====================================================================
var array<line> lines1_NQ_arr = array.new_line()
var array<box> boxes1_NQ_arr = array.new_box()
var array<label> labels1_NQ_arr = array.new_label()
var array<line> lines2_NQ_arr = array.new_line()
var array<box> boxes2_NQ_arr = array.new_box()
var array<label> labels2_NQ_arr = array.new_label()
var array<line> lines3_NQ_arr = array.new_line()
var array<label> labels3_NQ_arr = array.new_label()
var array<line> lines4_NQ_arr = array.new_line()
var array<label> labels4_NQ_arr = array.new_label()
var array<line> lines1_ES_arr = array.new_line()
var array<box> boxes1_ES_arr = array.new_box()
var array<label> labels1_ES_arr = array.new_label()
var array<line> lines2_ES_arr = array.new_line()
var array<box> boxes2_ES_arr = array.new_box()
var array<label> labels2_ES_arr = array.new_label()
var array<line> lines3_ES_arr = array.new_line()
var array<label> labels3_ES_arr = array.new_label()
var array<line> lines4_ES_arr = array.new_line()
var array<label> labels4_ES_arr = array.new_label()
var array<line> lisLines_arr = array.new_line()
var array<box> lisBoxes_arr = array.new_box()
var array<label> lisLabels_arr = array.new_label()

f_stringToPosition(pos) =>
    pos == "Top Left" ? position.top_left :
     pos == "Top Right" ? position.top_right :
     pos == "Bottom Left" ? position.bottom_left :
     position.bottom_right

var table timestamp_display_table = na
// =====================================================================
// 3. GENERIC DRAWING FUNCTIONS
// =====================================================================
str_join(string_array, separator) =>
    string result = ''
    if array.size(string_array) > 0
        result := array.get(string_array, 0)
        if array.size(string_array) > 1
            for i = 1 to array.size(string_array) - 1 by 1
                result := result + separator + array.get(string_array, i)
    result

clear_objects(linesArray, boxesArray, labelsArray) =>
    for id in linesArray
        line.delete(id)
    array.clear(linesArray)
    for id in boxesArray
        box.delete(id)
    array.clear(boxesArray)
    for id in labelsArray
        label.delete(id)
    array.clear(labelsArray)

processAllData(rawData) =>
    array<string> normalSegments = array.new_string()
    array<string> lisSegments = array.new_string()
    if str.length(rawData) > 0
        allSegments = str.split(rawData, ';')
        for seg in allSegments
            if str.contains(seg, '_LIS')
                array.push(lisSegments, str.replace_all(seg, '_LIS', ''))
            else
                array.push(normalSegments, seg)
    [str_join(normalSegments, ';'), str_join(lisSegments, ';')]

parseSet3Data(rawData) =>
    string timestampStr = na
    float midVal = na
    float lowerVal = na
    float upperVal = na
    if str.length(rawData) > 0
        parts = str.split(rawData, '|')
        if array.size(parts) > 0
            timePart = array.get(parts, 0)
            timePos = str.pos(timePart, "Time:")
            if timePos > -1
                timestampStr := str.trim(str.substring(timePart, timePos + 5, str.length(timePart)))
        if array.size(parts) > 1
            levelsPart = array.get(parts, 1)
            levelItems = str.split(levelsPart, ',')
            for item in levelItems
                keyValue = str.split(item, ':')
                if array.size(keyValue) > 1
                    key = str.trim(array.get(keyValue, 0))
                    valueStr = str.trim(array.get(keyValue, 1))
                    if key == 'Mid'
                        midVal := str.tonumber(valueStr)
                    else if key == 'Lower'
                        lowerVal := str.tonumber(valueStr)
                    else if key == 'Upper'
                        upperVal := str.tonumber(valueStr)
    [timestampStr, midVal, lowerVal, upperVal]

// --- MODIFIED DRAWING FUNCTIONS TO ACCEPT LABEL COUNTER ---
drawSet1(rawData, showZones, showLabels, linesArray, boxesArray, labelsArray, labelOffsets) =>
    if (showZones or showLabels) and str.length(rawData) > 0
        zones = str.split(rawData, ';')
        for zoneRaw in zones
            z = str.trim(zoneRaw)
            if str.length(z) > 0
                parts = str.split(z, ',')
                if array.size(parts) >= 6
                    pTop = str.tonumber(array.get(parts, 0))
                    t1v = str.tonumber(str.replace_all(array.get(parts, 1), 'T1=', ''))
                    t2v = str.tonumber(str.replace_all(array.get(parts, 2), 'T2=', ''))
                    o1v = str.tonumber(str.replace_all(array.get(parts, 3), 'O1=', ''))
                    o2v = str.tonumber(str.replace_all(array.get(parts, 4), 'O2=', ''))
                    pBottom = str.tonumber(array.get(parts, 5))
                    if not na(pTop) and not na(pBottom)
                        hi = math.max(pTop, pBottom)
                        lo = math.min(pTop, pBottom)
                        midPoint = (hi + lo) / 2
                        if showZones
                            if hi - lo < 1
                                array.push(linesArray, line.new(bar_index - 500, midPoint, bar_index + 500, midPoint, xloc = xloc.bar_index, color = set1ZoneColor, style = line.style_solid, width = 2))
                            else
                                array.push(boxesArray, box.new(bar_index - 500, hi, bar_index + 500, lo, bgcolor = color.new(set1ZoneColor, set1ZoneOpacity), border_color = na, xloc = xloc.bar_index))
                        if showLabels
                            string priceKey = str.tostring(midPoint)
                            currentOffset = labelOffsets.get(priceKey)
                            if na(currentOffset)
                                currentOffset := set1LabelOffset
                            sumVal = t1v + t2v + o1v + o2v
                            lineOne = 'T1=' + str.tostring(t1v) + ',T2=' + str.tostring(t2v) + ',O1=' + str.tostring(o1v) + ',O2=' + str.tostring(o2v) + ', = ' + str.tostring(sumVal)
                            diffInt = math.round(hi - lo)
                            lineTwo = str.tostring(hi, '#.##') + ' – ' + str.tostring(lo, '#.##') + ' = ' + str.tostring(diffInt)
                            fullText = lineOne + '\n' + lineTwo
                            array.push(labelsArray, label.new(bar_index + currentOffset, midPoint, fullText, style = label.style_label_right, size = set1LabelSize, color = set1LabelBGColor, textcolor = set1LabelTextColor, xloc = xloc.bar_index))
                            labelWidthInBars = math.round(str.length(lineOne) * avgCharWidthInBars) + 2
                            labelOffsets.put(priceKey, currentOffset + labelWidthInBars)

drawSet2(rawData, showZones, showLabels, linesArray, boxesArray, labelsArray, labelOffsets) =>
    if (showZones or showLabels) and str.length(rawData) > 0
        lineStyleConst = set2LineStyle == 'dashed' ? line.style_dashed : set2LineStyle == 'dotted' ? line.style_dotted : line.style_solid
        segments = str.split(rawData, ';')
        for seg in segments
            trimmed = str.trim(seg)
            if str.length(trimmed) > 0
                if str.contains(trimmed, ',')
                    parts = str.split(trimmed, ',')
                    top = str.tonumber(array.get(parts, 0))
                    bottom = str.tonumber(array.get(parts, 1))
                    if not na(top) and not na(bottom)
                        if showZones
                            array.push(boxesArray, box.new(bar_index - 500, top, bar_index + 500, bottom, bgcolor = color.new(set2LineColor, set2ZoneOpacity), border_color = na, xloc = xloc.bar_index))
                        if showLabels
                            string priceKey = str.tostring((top + bottom) / 2)
                            currentOffset = labelOffsets.get(priceKey)
                            if na(currentOffset)
                                currentOffset := set2LabelOffset
                            labelText = str.tostring(bottom, '#.##') + ' - ' + str.tostring(top, '#.##')
                            array.push(labelsArray, label.new(bar_index + currentOffset, (top + bottom) / 2, labelText, style = label.style_label_up, size = set2LabelSize, color = set2LabelBGColor, textcolor = set2LabelTextColor, xloc = xloc.bar_index))
                            labelWidthInBars = math.round(str.length(labelText) * avgCharWidthInBars) + 2
                            labelOffsets.put(priceKey, currentOffset + labelWidthInBars)
                else
                    level = str.tonumber(trimmed)
                    if not na(level)
                        if showZones
                            array.push(linesArray, line.new(bar_index - 500, level, bar_index + 500, level, xloc = xloc.bar_index, color = set2LineColor, style = lineStyleConst, width = set2LineWidth))
                        if showLabels
                            string priceKey = str.tostring(level)
                            currentOffset = labelOffsets.get(priceKey)
                            if na(currentOffset)
                                currentOffset := set2LabelOffset
                            labelText = str.tostring(level, '#.##')
                            array.push(labelsArray, label.new(bar_index + currentOffset, level, labelText, style = label.style_label_up, size = set2LabelSize, color = set2LabelBGColor, textcolor = set2LabelTextColor, xloc = xloc.bar_index))
                            labelWidthInBars = math.round(str.length(labelText) * avgCharWidthInBars) + 2
                            labelOffsets.put(priceKey, currentOffset + labelWidthInBars)

drawSet4(rawData, showZones, showLabels, linesArray, labelsArray, labelOffsets) =>
    if (showZones or showLabels) and str.length(rawData) > 0
        lineStyleConst = set4LineStyle == 'dashed' ? line.style_dashed : set4LineStyle == 'dotted' ? line.style_dotted : line.style_solid
        segments = str.split(rawData, ';')
        for seg in segments
            trimmed = str.trim(seg)
            if str.length(trimmed) > 0
                string priceStr = trimmed
                string labelStr = trimmed

                parts = str.split(trimmed, '-')
                if array.size(parts) >= 2
                    priceStr := array.get(parts, 0)
                    labelStr := str_join(array.slice(parts, 1), "-")
                
                float level = str.tonumber(priceStr)

                if not na(level)
                    if showZones
                        array.push(linesArray, line.new(bar_index - 500, level, bar_index + 500, level, xloc = xloc.bar_index, color = set4LineColor, style = lineStyleConst, width = set4LineWidth))
                    if showLabels
                        string priceKey = str.tostring(level)
                        currentOffset = labelOffsets.get(priceKey)
                        if na(currentOffset)
                            currentOffset := set4LabelOffset
                        array.push(labelsArray, label.new(bar_index + currentOffset, level, labelStr, style = label.style_label_up, size = set4LabelSize, color = set4LabelBGColor, textcolor = set4LabelTextColor, xloc = xloc.bar_index))
                        labelWidthInBars = math.round(str.length(labelStr) * avgCharWidthInBars) + 2
                        labelOffsets.put(priceKey, currentOffset + labelWidthInBars)

drawLIS(rawData, linesArray, boxesArray, labelsArray, labelOffsets) =>
    if str.length(rawData) > 0
        segments = str.split(rawData, ';')
        for seg in segments
            trimmed = str.trim(seg)
            if str.length(trimmed) > 0
                if str.contains(trimmed, ',')
                    parts = str.split(trimmed, ',')
                    top = str.tonumber(array.get(parts, 0))
                    bottom = str.tonumber(array.get(parts, 1))
                    if not na(top) and not na(bottom)
                        midPoint = (top + bottom) / 2
                        array.push(boxesArray, box.new(bar_index - 500, top, bar_index + 500, bottom, bgcolor = color.new(lisLineColor, 80), border_color = na, xloc = xloc.bar_index))
                        string priceKey = str.tostring(midPoint)
                        currentOffset = labelOffsets.get(priceKey)
                        if na(currentOffset)
                            currentOffset := lisLabelOffset
                        labelText = 'LIS'
                        array.push(labelsArray, label.new(bar_index + currentOffset, midPoint, labelText, style = label.style_label_up, size = lisLabelSize, color = lisLabelBGColor, textcolor = lisLabelTextColor, xloc = xloc.bar_index))
                        labelWidthInBars = math.round(str.length(labelText) * avgCharWidthInBars) + 2
                        labelOffsets.put(priceKey, currentOffset + labelWidthInBars)
                else
                    level = str.tonumber(trimmed)
                    if not na(level)
                        array.push(linesArray, line.new(bar_index - 500, level, bar_index + 500, level, color = lisLineColor, width = 3, xloc = xloc.bar_index))
                        string priceKey = str.tostring(level)
                        currentOffset = labelOffsets.get(priceKey)
                        if na(currentOffset)
                            currentOffset := lisLabelOffset
                        labelText = 'LIS'
                        array.push(labelsArray, label.new(bar_index + currentOffset, level, labelText, style = label.style_label_up, size = lisLabelSize, color = lisLabelBGColor, textcolor = lisLabelTextColor, xloc = xloc.bar_index))
                        labelWidthInBars = math.round(str.length(labelText) * avgCharWidthInBars) + 2
                        labelOffsets.put(priceKey, currentOffset + labelWidthInBars)

// =====================================================================
// 4. EXECUTION
// =====================================================================
if barstate.islast
    // --- MAP TO PREVENT LABEL OVERLAPS ---
    var map<string, int> labelOffsets = map.new<string, int>()
    map.clear(labelOffsets)

    clear_objects(lines1_NQ_arr, boxes1_NQ_arr, labels1_NQ_arr)
    clear_objects(lines2_NQ_arr, boxes2_NQ_arr, labels2_NQ_arr)
    clear_objects(lines3_NQ_arr, array.new_box(), labels3_NQ_arr)
    clear_objects(lines4_NQ_arr, array.new_box(), labels4_NQ_arr)
    clear_objects(lines1_ES_arr, boxes1_ES_arr, labels1_ES_arr)
    clear_objects(lines2_ES_arr, boxes2_ES_arr, labels2_ES_arr)
    clear_objects(lines3_ES_arr, array.new_box(), labels3_ES_arr)
    clear_objects(lines4_ES_arr, array.new_box(), labels4_ES_arr)
    clear_objects(lisLines_arr, lisBoxes_arr, lisLabels_arr)
    
    if na(timestamp_display_table)
        timestamp_display_table := table.new(f_stringToPosition(timestampPosition), 1, 2)
    else
        table.set_position(timestamp_display_table, f_stringToPosition(timestampPosition))
    table.clear(timestamp_display_table, 0, 0, 0, 1)

    [normalData1_NQ, lisData1_NQ] = processAllData(rawData1_NQ)
    [normalData2_NQ, lisData2_NQ] = processAllData(rawData2_NQ)
    [normalData3_NQ, lisData3_NQ] = processAllData(rawData3_NQ)
    [normalData4_NQ, lisData4_NQ] = processAllData(rawData4_NQ)
    [normalData1_ES, lisData1_ES] = processAllData(rawData1_ES)
    [normalData2_ES, lisData2_ES] = processAllData(rawData2_ES)
    [normalData3_ES, lisData3_ES] = processAllData(rawData3_ES)
    [normalData4_ES, lisData4_ES] = processAllData(rawData4_ES)
    
    array<string> allLisData = array.new_string()
    if str.length(lisData1_NQ) > 0
        array.push(allLisData, lisData1_NQ)
    if str.length(lisData2_NQ) > 0
        array.push(allLisData, lisData2_NQ)
    if str.length(lisData3_NQ) > 0
        array.push(allLisData, lisData3_NQ)
    if str.length(lisData4_NQ) > 0
        array.push(allLisData, lisData4_NQ)
    if str.length(lisData1_ES) > 0
        array.push(allLisData, lisData1_ES)
    if str.length(lisData2_ES) > 0
        array.push(allLisData, lisData2_ES)
    if str.length(lisData3_ES) > 0
        array.push(allLisData, lisData3_ES)
    if str.length(lisData4_ES) > 0
        array.push(allLisData, lisData4_ES)
    string combinedLisData = str_join(allLisData, ';')
    
    // NQ Drawings
    drawSet1(rawData1_NQ, showZones1_NQ, showLabels1_NQ, lines1_NQ_arr, boxes1_NQ_arr, labels1_NQ_arr, labelOffsets)
    drawSet2(rawData2_NQ, showZones2_NQ, showLabels2_NQ, lines2_NQ_arr, boxes2_NQ_arr, labels2_NQ_arr, labelOffsets)
    drawSet4(rawData4_NQ, showZones4_NQ, showLabels4_NQ, lines4_NQ_arr, labels4_NQ_arr, labelOffsets)
    
    [ts_nq, mid_nq, low_nq, up_nq] = parseSet3Data(rawData3_NQ)
    if showTimestamp3_NQ and not na(ts_nq)
        table.cell(timestamp_display_table, 0, 0, 'NQ Timestamp: ' + ts_nq, text_color=set3LabelTextColor, text_size=set3LabelSize)
    if showKeyLevels3_NQ
        if not na(mid_nq)
            array.push(lines3_NQ_arr, line.new(bar_index - 500, mid_nq, bar_index + 500, mid_nq, xloc = xloc.bar_index, color = midLineColor, style = midLineStyle == 'dashed' ? line.style_dashed : midLineStyle == 'dotted' ? line.style_dotted : line.style_solid, width = midLineWidth))
            if showKeyLabels3_NQ
                array.push(labels3_NQ_arr, label.new(bar_index + set3LabelOffset, mid_nq, 'Mid', style = label.style_label_right, size = set3LabelSize, color = set3LabelBGColor, textcolor = set3LabelTextColor, xloc = xloc.bar_index))
        if not na(low_nq)
            // MODIFIED: Hardcoded color to #4caf50 (Green)
            array.push(lines3_NQ_arr, line.new(bar_index - 500, low_nq, bar_index + 500, low_nq, xloc = xloc.bar_index, color = #4caf50, style = lowerLineStyle == 'dashed' ? line.style_dashed : lowerLineStyle == 'dotted' ? line.style_dotted : line.style_solid, width = lowerLineWidth))
            if showKeyLabels3_NQ
                array.push(labels3_NQ_arr, label.new(bar_index + set3LabelOffset, low_nq, 'Lower', style = label.style_label_right, size = set3LabelSize, color = set3LabelBGColor, textcolor = set3LabelTextColor, xloc = xloc.bar_index))
        if not na(up_nq)
            // MODIFIED: Hardcoded color to #ff5252 (Red)
            array.push(lines3_NQ_arr, line.new(bar_index - 500, up_nq, bar_index + 500, up_nq, xloc = xloc.bar_index, color = #ff5252, style = upperLineStyle == 'dashed' ? line.style_dashed : upperLineStyle == 'dotted' ? line.style_dotted : line.style_solid, width = upperLineWidth))
            if showKeyLabels3_NQ
                array.push(labels3_NQ_arr, label.new(bar_index + set3LabelOffset, up_nq, 'Upper', style = label.style_label_right, size = set3LabelSize, color = set3LabelBGColor, textcolor = set3LabelTextColor, xloc = xloc.bar_index))
    
    // ES Drawings
    drawSet1(rawData1_ES, showZones1_ES, showLabels1_ES, lines1_ES_arr, boxes1_ES_arr, labels1_ES_arr, labelOffsets)
    drawSet2(rawData2_ES, showZones2_ES, showLabels2_ES, lines2_ES_arr, boxes2_ES_arr, labels2_ES_arr, labelOffsets)
    drawSet4(rawData4_ES, showZones4_ES, showLabels4_ES, lines4_ES_arr, labels4_ES_arr, labelOffsets)
    
    [ts_es, mid_es, low_es, up_es] = parseSet3Data(rawData3_ES)
    if showTimestamp3_ES and not na(ts_es)
        table.cell(timestamp_display_table, 0, 1, 'ES Timestamp: ' + ts_es, text_color=set3LabelTextColor, text_size=set3LabelSize)
    if showKeyLevels3_ES
        if not na(mid_es)
            array.push(lines3_ES_arr, line.new(bar_index - 500, mid_es, bar_index + 500, mid_es, xloc = xloc.bar_index, color = midLineColor, style = midLineStyle == 'dashed' ? line.style_dashed : midLineStyle == 'dotted' ? line.style_dotted : line.style_solid, width = midLineWidth))
            if showKeyLabels3_ES
                array.push(labels3_ES_arr, label.new(bar_index + set3LabelOffset, mid_es, 'Mid', style = label.style_label_right, size = set3LabelSize, color = set3LabelBGColor, textcolor = set3LabelTextColor, xloc = xloc.bar_index))
        if not na(low_es)
            // MODIFIED: Hardcoded color to #4caf50 (Green)
            array.push(lines3_ES_arr, line.new(bar_index - 500, low_es, bar_index + 500, low_es, xloc = xloc.bar_index, color = #4caf50, style = lowerLineStyle == 'dashed' ? line.style_dashed : lowerLineStyle == 'dotted' ? line.style_dotted : line.style_solid, width = lowerLineWidth))
            if showKeyLabels3_ES
                array.push(labels3_ES_arr, label.new(bar_index + set3LabelOffset, low_es, 'Lower', style = label.style_label_right, size = set3LabelSize, color = set3LabelBGColor, textcolor = set3LabelTextColor, xloc = xloc.bar_index))
        if not na(up_es)
            // MODIFIED: Hardcoded color to #ff5252 (Red)
            array.push(lines3_ES_arr, line.new(bar_index - 500, up_es, bar_index + 500, up_es, xloc = xloc.bar_index, color = #ff5252, style = upperLineStyle == 'dashed' ? line.style_dashed : upperLineStyle == 'dotted' ? line.style_dotted : line.style_solid, width = upperLineWidth))
            if showKeyLabels3_ES
                array.push(labels3_ES_arr, label.new(bar_index + set3LabelOffset, up_es, 'Upper', style = label.style_label_right, size = set3LabelSize, color = set3LabelBGColor, textcolor = set3LabelTextColor, xloc = xloc.bar_index))
    
    // LIS Drawings
    drawLIS(combinedLisData, lisLines_arr, lisBoxes_arr, lisLabels_arr, labelOffsets)