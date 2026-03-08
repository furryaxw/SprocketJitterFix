# LayingDrive Jitter Fix (SprocketJitterFix)

[![Game](https://img.shields.io/badge/Game-Sprocket-blue)](https://store.steampowered.com/app/1674170/Sprocket/)
[![Mod Loader](https://img.shields.io/badge/Loader-MelonLoader-green)](https://melonwiki.xyz/)

> **"Not Vanilla, but stabilized."**

这是一个为《Sprocket》开发的增强模组，专注于解决原版游戏在特定设计下（如高灵敏度火控）出现的**高低机抖动**问题。

## 🛠️ 功能特性

- **线性阻尼**：在炮管接近目标仰角时，自动介入线性阻尼，平滑减速，防止过度修正导致的反复震荡。
- **马达锁死**：修复了上个版本断电 Bug，在误差极小时保持液压马达微通电抱死，彻底解决重力下垂。
- **性能优化**：
  - **向量正交**：利用 Transform `forward` 与 `right` 天然正交的数学特性，完全消除了多余的自身投影计算。
  - **消除开方**：摒弃了昂贵的 `Vector3.normalized` 和隐式的 `sqrt` 计算，全程使用向量长度平方进行不等式阈值比较。
  - **消除跨界开销**：手工展开点乘、投影和反三角函数，替代 Unity 原生的 `Vector3.Project` 和 `Vector3.Angle`，将 Il2Cpp 的 C# <-> C++ 跨界调用开销降至最低。

## 🎥 效果对比

- **Vanilla**: 炮管在目标点上下高频震颤，影响行进间射击体验。
- **Stabilized**: 丝滑锁定，接近准星时平稳减速并锚定。

## 📥 安装方法

1. 确保你已安装最新版本的 [MelonLoader](https://melonwiki.xyz/)。
2. 下载本项目的最新 [Releases](https://github.com/furryaxw/SprocketJitterFix/releases) 中的 `SprocketJitterFix.dll`。
3. 将 `.dll` 文件放入游戏根目录的 `Mods` 文件夹中。
4. 启动游戏，尽情享受稳定的火控。

## 🤝 鸣谢

- **Author**: furryAxw
- **Tools**: Harmony, MelonLoader, Visual Studio 2026

## 📄 License

本项目采用 [GPL-3.0 License](LICENSE.txt) 开源许可。
