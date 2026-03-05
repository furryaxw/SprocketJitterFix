using HarmonyLib;
using Il2CppSprocket;
using Il2CppSprocket.Vehicles.Weapons;
using MelonLoader;
using UnityEngine;

[assembly: MelonInfo(typeof(SprocketJitterFix.JitterFixMain), "LayingDrive Jitter Fix", "0.8.3", "furryAxw")]
[assembly: MelonGame("HD", "Sprocket")]

namespace SprocketJitterFix
{
    public class JitterFixMain : MelonMod
    {
        public override void OnInitializeMelon()
        {
            LoggerInstance.Msg("=== 动态阻尼已启动 ===");
        }
    }

    [HarmonyPatch(typeof(LayingDriveBehaviour), nameof(LayingDriveBehaviour.MoveToTarget))]
    public class LayingDrive_MoveToTarget_FixPatch
    {
        // 预定义常量
        private const float MinAngleThreshold = 0.001f;
        private const float AzimuthSpeedThreshold = 0.001f;
        private const float AzimuthRangeThreshold = 0.01f;

        static bool Prefix(LayingDriveBehaviour __instance, float deltaTime, ref float speedMultiplier, AimFlags flags)
        {
            // 0. 基础状态快筛：如果 multiplier 已经是 0 或者 deltaTime 异常，直接跳过
            if (speedMultiplier <= 0f || deltaTime <= 0f) return true;

            Vector3 targetDir = __instance.TargetDirection;
            if (targetDir == Vector3.zero) return true;

            // 1. 横向机构过滤 (缓存属性访问)
            var azInfo = __instance.azimuthInfo;
            if (azInfo.MaxSpeed > AzimuthSpeedThreshold && (azInfo.Max - azInfo.Min) > AzimuthRangeThreshold)
            {
                return true;
            }

            // 获取引用
            Transform trunnions = __instance.trunnions;
            Vector3 currentDir = trunnions.forward;
            Vector3 trRight = trunnions.right;

            // 2. 垂直误差快速解算
            // 替代 ProjectOnPlane: 投射到垂直平面相当于去除 trRight 方向的分量并归一化
            Vector3 targetPitchVec = targetDir - Vector3.Project(targetDir, trRight);
            Vector3 currentPitchVec = currentDir - Vector3.Project(currentDir, trRight);

            // 使用点积计算余弦值 (注意：Project 后的向量需要量化归一化或处理)
            // 为保持高性能且逻辑简单，这里依然计算角度，但增加一个“快路径”判断
            float elMaxSpeed = __instance.elevationInfo.MaxSpeed;
            float maxStepEl = elMaxSpeed * speedMultiplier * deltaTime;

            if (maxStepEl <= 0) return true;

            // --- 使用平方量级的快速检查 ---
            // 只有当两个向量非常接近时，才进行昂贵的 Angle 计算
            float dot = Vector3.Dot(currentPitchVec.normalized, targetPitchVec.normalized);

            // 只要 dot > 0.99 (约 8度以内)，才进精细计算，否则直接返回
            // 这里的 0.99 是一个保守值，可以覆盖所有需要阻尼的情况
            if (dot < 0.99f) return true;

            float verticalDiff = Vector3.Angle(currentPitchVec, targetPitchVec);

            // 3. 动态阻尼计算
            if (verticalDiff <= maxStepEl)
            {
                if (verticalDiff < MinAngleThreshold)
                {
                    speedMultiplier = 0f;
                }
                else
                {
                    // 线性阻尼
                    speedMultiplier *= (verticalDiff / maxStepEl);
                }
            }

            return true;
        }
    }
}