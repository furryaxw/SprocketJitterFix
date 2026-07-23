using System;
using HarmonyLib;
using Il2CppSprocket;
using Il2CppSprocket.Vehicles.Weapons;
using MelonLoader;
using UnityEngine;

[assembly: MelonInfo(typeof(SprocketJitterFix.JitterFixMain), "LayingDrive Jitter Fix", "0.9.0", "furryAxw")]
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
        private const float AzimuthSpeedThreshold = 0.001f;
        private const float AzimuthRangeThreshold = 0.01f;

        // 预计算 0.99 的平方，避免运行时计算
        private const float SqrDotThreshold = 0.9801f;
        private const float RadToDeg = 57.29578f;

        static bool Prefix(LayingDriveBehaviour __instance, float deltaTime, ref float speedMultiplier, AimFlags flags)
        {
            // 0. 基础状态快筛
            if (speedMultiplier <= 0f || deltaTime <= 0f) return true;

            // 1. 横向机构过滤 (提至最前，如果横向机构在动，直接跳过后续所有逻辑)
            var azInfo = __instance.azimuthInfo;
            if (azInfo.MaxSpeed > AzimuthSpeedThreshold && (azInfo.Max - azInfo.Min) > AzimuthRangeThreshold) return true;

            // 2. 提前计算最大步长，拦截无须运算的帧 (避免后续的 C++ -> C# Transform Interop 开销)
            float elMaxSpeed = __instance.elevationInfo.MaxSpeed;
            float maxStepEl = elMaxSpeed * speedMultiplier * deltaTime;
            if (maxStepEl <= 0f) return true;

            // 获取目标方向并进行快速零值检查
            Vector3 targetDir = __instance.TargetDirection;
            if (targetDir.x == 0f && targetDir.y == 0f && targetDir.z == 0f) return true;

            // 获取引用 (此处发生 Il2Cpp 跨界调用，尽量延后)
            Transform trunnions = __instance.trunnions;
            Vector3 currentDir = trunnions.forward;
            Vector3 trRight = trunnions.right;

            // 3. 向量解算
            float rightDot = targetDir.x * trRight.x + targetDir.y * trRight.y + targetDir.z * trRight.z;
            Vector3 targetPitchVec = new Vector3(
                targetDir.x - trRight.x * rightDot,
                targetDir.y - trRight.y * rightDot,
                targetDir.z - trRight.z * rightDot
            );

            float targetPitchSqrMag = targetPitchVec.x * targetPitchVec.x +
                                      targetPitchVec.y * targetPitchVec.y +
                                      targetPitchVec.z * targetPitchVec.z;

            float rawDot = currentDir.x * targetPitchVec.x +
                           currentDir.y * targetPitchVec.y +
                           currentDir.z * targetPitchVec.z;

            // 如果夹角 > 90度，绝对不可能达到 0.99f (约8度) 的阈值，直接跳过
            if (rawDot <= 0f) return true;

            // 使用平方进行阈值比较: (rawDot / Sqrt(SqrMag)) >= 0.99  =>  rawDot^2 >= 0.9801 * SqrMag
            float rawDotSqr = rawDot * rawDot;
            if (rawDotSqr < SqrDotThreshold * targetPitchSqrMag) return true;

            // 目标方向几乎与横向轴重合，无法可靠计算俯仰角。
            if (targetPitchSqrMag < 0.00000001f) return true;

            // atan2(sin, cos) 保留零附近精度；Acos(float cosTheta) 会在约 0.02 度内
            // 因 cosTheta 舍入为 1 而错误地返回 0。
            float crossX = currentDir.y * targetPitchVec.z - currentDir.z * targetPitchVec.y;
            float crossY = currentDir.z * targetPitchVec.x - currentDir.x * targetPitchVec.z;
            float crossZ = currentDir.x * targetPitchVec.y - currentDir.y * targetPitchVec.x;
            float signedSine = crossX * trRight.x + crossY * trRight.y + crossZ * trRight.z;
            float verticalDiff = (float)Math.Atan2(Math.Abs(signedSine), rawDot) * RadToDeg;

            // 4. 动态阻尼计算
            if (verticalDiff <= maxStepEl)
            {
                speedMultiplier *= (verticalDiff / maxStepEl);
                if (speedMultiplier <= 0f)
                {
                    speedMultiplier = 0.00001f;
                }
            }

            return true;
        }
    }
}
