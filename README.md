# Native Hold Key

Native Hold Key is an OpenTabletDriver binding plugin for macOS. It keeps a key or key combination pressed for as long as a tablet button is held, emits Quartz autorepeat events, and safely releases held keys.

This is useful for spring-loaded tools in applications such as Adobe Photoshop and Illustrator, where a regular OpenTabletDriver key binding may be interpreted as a single tap instead of a physical key hold.

## Requirements

- macOS
- OpenTabletDriver 0.6.7 or newer within the 0.6.x plugin API

## Installation

### Plugin Manager

Once the plugin is accepted into the official OpenTabletDriver plugin repository, install **Native Hold Key** from the Plugin Manager and restart OpenTabletDriver.

### Manual installation

1. Download `NativeHoldKey-1.0.0.0.zip` from the latest GitHub release.
2. Extract `HoldKeyPlugin.dll` into a dedicated folder under:

   `~/Library/Application Support/OpenTabletDriver/Plugins/Native Hold Key/`

3. Restart OpenTabletDriver.

## Usage

1. Open the **Pen Settings** or **Auxiliary Settings** tab.
2. Open the advanced editor for the desired button.
3. Select **Native Hold Key**.
4. Configure:
   - **Keys:** `Z` or a combination such as `Application+Space`.
   - **Repeat interval:** `40` milliseconds is a good starting point.
   - **Safety release:** `15` seconds prevents a key from remaining stuck after a device or application failure.
5. Apply and save the profile.

`Application` represents the Command key on macOS. Other supported modifier names include `Shift`, `Control`, `Alt`, and `Option`.

## Safety behavior

- Duplicate press reports are ignored.
- Keys are released in reverse order.
- Partial failures roll back keys that were already pressed.
- Disposal and the configurable safety timeout release held keys.
- The plugin is declared macOS-only and uses Quartz/CoreGraphics events.

## Building

Install the .NET 8 SDK, then run:

```bash
dotnet restore HoldKeyPlugin.sln
dotnet test HoldKeyPlugin.sln
dotnet build HoldKeyPlugin/HoldKeyPlugin.csproj --configuration Release
```

Create the release archive with:

```bash
./scripts/package.sh 1.0.0.0
```

The archive is written to `artifacts/` with `HoldKeyPlugin.dll` at its root, as required by the OpenTabletDriver Plugin Repository.

## License

[MIT](LICENSE)
