# Native Hold Key

Native Hold Key is a cross-platform OpenTabletDriver binding plugin for macOS, Windows, and Linux. It keeps a key or key combination pressed for as long as a tablet button is held, emits repeat events, and safely releases held keys.

This is useful for spring-loaded tools in applications such as Adobe Photoshop and Illustrator, where a regular OpenTabletDriver key binding may be interpreted as a single tap instead of a physical key hold.

## Requirements

- macOS, Windows, or Linux
- OpenTabletDriver 0.6.7 or newer within the 0.6.x plugin API

## Installation

### Plugin Manager

Once the plugin is accepted into the official OpenTabletDriver plugin repository, install **Native Hold Key** from the Plugin Manager and restart OpenTabletDriver.

### Manual installation

1. Download `NativeHoldKey-1.1.0.0.zip` from the latest GitHub release.
2. Open the OpenTabletDriver Plugin Manager and use **Open Plugins Folder**.
3. Create a `Native Hold Key` folder there and extract `HoldKeyPlugin.dll` into it.
4. Restart OpenTabletDriver.

## Usage

1. Open the **Pen Settings** or **Auxiliary Settings** tab.
2. Open the advanced editor for the desired button.
3. Select **Native Hold Key**.
4. Configure:
   - **Keys:** `Z` or a combination such as `Application+Space`.
   - **Repeat interval:** `40` milliseconds is a good starting point.
   - **Safety release:** `15` seconds prevents a key from remaining stuck after a device or application failure.
5. Apply and save the profile.

`Application` represents Command on macOS, the Windows key on Windows, and Meta on Linux. Other supported modifier names include `Shift`, `Control`, and `Alt`. The macOS aliases `Command` and `Option` are also accepted.

## Platform behavior

- **macOS:** uses Quartz/CoreGraphics and marks repeated events as native autorepeat events.
- **Windows:** uses OpenTabletDriver's virtual keyboard, backed by the Windows `SendInput` API.
- **Linux:** uses OpenTabletDriver's virtual keyboard, backed by `evdev/uinput`, so it works independently of X11 or Wayland. The OpenTabletDriver installation must have permission to access `/dev/uinput`.

## Safety behavior

- Duplicate press reports are ignored.
- Keys are released in reverse order.
- Partial failures roll back keys that were already pressed.
- Disposal and the configurable safety timeout release held keys.
- The plugin is declared for macOS, Windows, and Linux and selects the correct input backend automatically.

## Building

Install the .NET 8 SDK, then run:

```bash
dotnet restore HoldKeyPlugin.sln
dotnet test HoldKeyPlugin.sln
dotnet build HoldKeyPlugin/HoldKeyPlugin.csproj --configuration Release
```

Create the release archive with:

```bash
./scripts/package.sh 1.1.0.0
```

The archive is written to `artifacts/` with `HoldKeyPlugin.dll` at its root, as required by the OpenTabletDriver Plugin Repository.

## License

[MIT](LICENSE)
