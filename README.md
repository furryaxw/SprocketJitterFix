# LayingDrive Jitter Fix (SprocketJitterFix)

[![Game](https://img.shields.io/badge/Game-Sprocket-blue)](https://store.steampowered.com/app/1674170/Sprocket/)
[![Mod Loader](https://img.shields.io/badge/Loader-MelonLoader-green)](https://melonwiki.xyz/)

> **"Not Vanilla, but stabilized."**

这是一个为《Sprocket》开发的增强模组，专注于解决原版游戏在特定设计下（如高灵敏度火控）出现的**高低机抖动**问题。

## 🛠️ 功能特性

- **动态阻尼**：在炮管接近目标仰角时，自动介入线性阻尼，防止过度修正导致的反复震荡。
- **性能优化**：
  - 采用 **点积快路径** 逻辑，仅在误差极小时才进入复杂计算。

## 🎥 效果对比

- **Vanilla**: 炮管在目标点上下高频震颤，影响行进间射击体验。
- **Stabilized**: 丝滑锁定，接近准星时平稳减速并锚定。

## 📥 安装方法

1. 确保你已安装最新版本的 [MelonLoader](https://melonwiki.xyz/)。
2. 下载本项目的最新 [Releases](https://github.com/furryaxw/SprocketJitterFix/releases) 中的 `SprocketJitterFix.dll`。
3. 将 `.dll` 文件放入游戏根目录的 `Mods` 文件夹中。
4. 启动游戏，尽情享受稳定的火控。

## 💻 技术实现

本模组通过 Harmony 补丁拦截了 `LayingDriveBehaviour.MoveToTarget` 方法。
主要数学优化逻辑：
- 排除所有具备横向转动能力的驱动器。
- 使用 `Vector3.ProjectOnPlane` 进行垂直向量正交分解。
- 当 `cos(θ) > 0.99` 时触发阻尼乘区 `speedMultiplier *= (verticalDiff / maxStepEl)`。

## 🤝 鸣谢

- **Author**: furryAxw
- **Tools**: Harmony, MelonLoader, Visual Studio 2026

## 📄 License

本项目采用 [GPL-3.0 License](LICENSE) 开源许可。
