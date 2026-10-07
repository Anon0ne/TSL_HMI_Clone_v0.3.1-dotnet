# UI redesign notes

The original `tslhmi32.exe` is a VB6 HMI with bitmap-based machine-state imagery and functional areas including Former, Heat, Trim, Recipes, Setup, Diagnostics and maintenance/security functions.

The replacement keeps operator terminology and workflow recognizable while improving:
- persistent machine/mode status
- fault/safety visibility
- consistent navigation
- larger touch targets
- searchable engineering diagnostics
- separation of operator controls from engineering controls
- scalable machine graphics instead of fixed-resolution bitmap chrome
- explicit read-only/write-lock state during commissioning

The original `Former Guard Doors.bmp` and related door/E-stop/lock-pin assets establish the geometry/state vocabulary. They should be redrawn as WPF vector layers so each physical safety device can be bound directly to its `diag.xml` point.
