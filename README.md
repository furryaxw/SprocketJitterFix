# LayingDrive Jitter Fix（SprocketJitterFix）

[![Game](https://img.shields.io/badge/Game-Sprocket-blue)](https://store.steampowered.com/app/1674170/Sprocket/)
[![Mod Loader](https://img.shields.io/badge/Loader-MelonLoader-green)](https://melonwiki.xyz/)
[![Version](https://img.shields.io/badge/version-0.9.1-brightgreen)](https://github.com/furryaxw/SprocketJitterFix/releases)

> **Not Vanilla, but stabilized.**

面向《Sprocket》高灵敏度火控设计的稳定化模组，用于减轻高低机接近目标时的过冲和反复震荡。

## 功能

- **近目标线性阻尼**：根据剩余俯仰误差按比例缩小 `MoveToTarget` 的速度倍率，避免以全速跨越目标。
- **小角度精确计算**：使用 `atan2(sin, cos)` 计算俯仰误差，避免 `Acos` 在极小角度下因浮点舍入而把非零误差误判为零。
- **微小保持倍率**：在误差精确为零时保留极小的非零倍率，避免断电式行为导致的下垂或突变。
- **低开销路径**：横向机构工作、无目标或误差较大时直接交还游戏原逻辑；只在接近目标时执行精细角度计算。

## 工作范围

- 仅补丁 `LayingDriveBehaviour.MoveToTarget` 的俯仰接近阶段。
- 横向机构存在有效运动范围时不介入，保持游戏原有的横向控制行为。

## 安装

1. 安装与游戏版本匹配的 [MelonLoader](https://melonwiki.xyz/)。
2. 从 [Releases](https://github.com/furryaxw/SprocketJitterFix/releases) 下载 `SprocketJitterFix.dll`。
3. 将 DLL 放入游戏根目录的 `Mods` 文件夹。
4. 启动游戏。

## 鸣谢

- Author: furryAxw
- Tools: Harmony, MelonLoader, Visual Studio

## License

[GPL-3.0](LICENSE.txt)
