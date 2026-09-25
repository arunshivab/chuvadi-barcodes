# Decoder test corpus

Real-world images used to measure and tune decoding (milestones M5 and M6).

## Layout (from M4 onwards)

```
corpus/
  <symbology>/          e.g. qr, datamatrix, code128
    <image>.<ext>       the photo or scan
    <image>.expected    expected decode results (one per line: format, text)
```

## Rules

1. **No real patient or personal data.** Every image must be synthetic, or scrubbed
   before it is committed. This repository is public.
2. Record the source of each image (who took it, device, conditions) in the matching
   `.expected` file header.
3. Keep individual files small (prefer ≤ 2 MB); downscale phone photos if needed.
