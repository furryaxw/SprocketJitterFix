# LayingDrive Jitter Fix (SprocketJitterFix)

[中文](README.zh.md) | **English**

[![Game](https://img.shields.io/badge/Game-Sprocket-blue)](https://store.steampowered.com/app/1674170/Sprocket/)
[![Mod Loader](https://img.shields.io/badge/Loader-BepInEx%206-blue)](https://github.com/BepInEx/BepInEx)
[![Version](https://img.shields.io/badge/version-1.0.0-brightgreen)](https://github.com/furryaxw/SprocketJitterFix/releases)

> **Not Vanilla, but stabilized.**

A stabilization mod for high-sensitivity fire control in Sprocket, built to reduce the
overshoot and repeated oscillation of the elevation drive as it closes on the target.

## Features

- **Linear damping near the target**: scales down the `MoveToTarget` speed multiplier in
  proportion to the remaining elevation error, so the drive never crosses the target at
  full speed.
- **Accurate small-angle computation**: computes the elevation error with
  `atan2(sin, cos)` instead of `Acos`, which at very small angles can mistake a small
  non-zero error for zero because of floating-point rounding.
- **Tiny hold multiplier**: keeps a very small non-zero multiplier when the error is
  exactly zero, avoiding the sag or sudden jump that power-cut-like behaviour would cause.
- **Low-overhead path**: hands control straight back to the game's original logic while
  the traverse mechanism is active, when there is no target, or when the error is large;
  the precise angle computation only runs as the drive approaches the target.

## Scope

- Patches only the elevation approach stage of `LayingDriveBehaviour.MoveToTarget`.
- Does not intervene while the traverse mechanism has a valid range of motion, preserving
  the game's original traverse control behaviour.

## Requirements

- Sprocket `0.2.55.5`, BepInEx `6.0.0-be.788` (IL2CPP / net6)
- Windows x64

## Installation

1. Install BepInEx 6 (IL2CPP) for Sprocket.
2. Download `SprocketJitterFix.dll` from [Releases](https://github.com/furryaxw/SprocketJitterFix/releases).
3. Place the DLL into `BepInEx\plugins`.
4. Launch the game.

## Credits

- Author: furryAxw
- Tools: Harmony, BepInEx, Visual Studio

## License

[GPL-3.0](LICENSE.txt)
