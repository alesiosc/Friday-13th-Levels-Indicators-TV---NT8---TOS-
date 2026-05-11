`GENERAL`
Add extensive debugging logs
Fallback Logic
Use the Devekoper Tools
Flaresolver for Cloudfare

`SCRAPING`
Try API Backend First.

`BROWSER AUTOMATION`
Show Me The Browser So I Can See Whats Going On

---

## OCR Ticker Detection: Purple Header Issue (SPY/QQQ/SPX Misclassification)

### Problem Description
SPY Blind Spots levels were being incorrectly classified as NQ (10 SPY levels → NQ instead of SPY).

**Symptoms:**
- SPY images showed `NQ (10)` instead of `SPY (10)` in the UI
- The `(SPY)` ticker in the purple header was being detected by standalone OCR test but NOT by the live processing

### Root Cause
The image processing pipeline in `image_service.py` applies these steps:
1. Convert to grayscale
2. Invert if dark background (mean < 128)
3. Scale up 2x
4. Apply binary threshold (Otsu's method)
5. Apply dilation
6. **Run OCR for ticker detection ← PROBLEM WAS HERE**
7. Detect rows and OCR each cell

**The issue:** Ticker OCR was running on the **thresholded image** (`thresh`), which destroyed the purple header containing `(SPY)`. The binary threshold converts the purple color to white, making the text invisible.

### How We Diagnosed It
1. Created `test_ocr.py` to test OCR directly on the raw image → `(SPY)` WAS detected at y=36
2. Created `test_full_flow.py` to test the full processing pipeline → `(SPY)` was NOT in the OCR output
3. Compared the two: The difference was using `gray` (grayscale) vs `thresh` (thresholded) image
4. The combined line text showed `'Key Levels Calcultated From Value'` - no `(SPY)` because it was destroyed by thresholding

### The Fix
**File:** `packages/analysis-service/services/image_service.py`

**Changed line ~163:**
```python
# BEFORE (broken):
ocr_data = pytesseract.image_to_data(thresh, output_type=pytesseract.Output.DATAFRAME)

# AFTER (fixed):
ocr_data = pytesseract.image_to_data(gray, output_type=pytesseract.Output.DATAFRAME)
```

**Why this works:** The grayscale image preserves the purple header text, while the thresholded image converts it to white (invisible).

### Key Insight
- **Row OCR** should use `thresh` (thresholded) for clean number extraction
- **Ticker OCR** should use `gray` (grayscale) to preserve colored headers

### Additional Fixes Applied (May Not Have Been Necessary)
1. Made ticker pattern regex case-insensitive: `re.IGNORECASE`
2. Added header-based ticker detection: If combined text contains header keywords (`KEY`, `LEVELS`, `FROM`, `VALUE`) and ticker keywords (`SPY`, `QQQ`, `SPX`), use that ticker even without brackets
3. Added duplicate leading digit OCR artifact fix (e.g., `66886` → `6886`)

### Test Commands
```bash
# Quick OCR test on a single image
my_python_3_11_env\Scripts\python.exe test_ocr.py

# Full processing flow test
my_python_3_11_env\Scripts\python.exe test_full_flow.py
```

### Lesson Learned
When OCR works in isolation but not in the full pipeline, check:
1. **Image preprocessing differences** - threshold, inversion, scaling
2. **Coordinate space differences** - scaled vs original coordinates
3. **Color destruction** - colored text may be lost during binary thresholding