# Changelog

## 1.1.0.0 — 2026-09-19

- Add Windows support through OpenTabletDriver's `SendInput` virtual keyboard.
- Add Linux support through OpenTabletDriver's `evdev/uinput` virtual keyboard.
- Select the native input backend automatically at runtime.
- Preserve Quartz autorepeat behavior on macOS.
- Add a macOS, Windows, and Linux CI test matrix.

## 1.0.0.0 — 2026-09-19

- Add configurable native macOS hold-key binding.
- Add Quartz autorepeat for spring-loaded application shortcuts.
- Add configurable safety release timeout.
- Add rollback for partial key emission failures.
- Add OpenTabletDriver 0.6.7 packaging and macOS-only declaration.
