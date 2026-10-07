# TSL HMI Clone v0.3.1 — .NET/WPF

Editable Visual Studio project for reconstruction of the TSL VB6 HMI.

## Open it
1. Install Visual Studio Community with **.NET desktop development**.
2. Open `TSL.HMI.csproj` (or `TSL.HMI.sln`).
3. Select **x86**. This is intentional because the original TSL/VB6/COM stack is 32-bit.
4. Press F5.

The app defaults to `C:\ITS`. For offline testing pass the extracted ITS folder as the first command-line argument.

## Current safety state
This build is **write locked**. `OfflineControllerService.WriteAsync()` always throws. Do not remove that lock until the ItsCtrl COM signatures and tag semantics have been validated against the original HMI.

## UI direction
The original HMI's functional concepts and machine-state visuals are retained, while the shell is redesigned for clearer hierarchy and diagnostics. Original bitmap assets are reference material; the target is scalable/vector machine visualization with explicit state/fault indication.

## Current implemented areas
- Machine/config discovery
- VariableTags.xml parsing
- diag.xml parsing
- Overview dashboard
- Former shell
- Safety circuit browser
- Tag/diagnostics browser
- Heat/Trim navigation placeholders
- x86 controller-service boundary

## Next
1. Validate XML parser against the live 1384 directory.
2. Inspect/register `ItsCtrl.Controller` and confirm read signatures.
3. Implement read-only polling.
4. Bind Former/Trim/Safety states.
5. Reconstruct original forms into modern WPF views.
6. Add recipes and alarm help.
7. Only after read parity: introduce a separately gated write service.


## v0.3.1 build fix
- Added explicit `System.IO` imports for filesystem access.
- Replaced the compressed INI/XML loader expressions with explicit parsing methods.
- Added ITS root validation and clearer loader exceptions.
- No controller write capability is enabled.
