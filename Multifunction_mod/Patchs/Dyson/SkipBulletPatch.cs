using HarmonyLib;
using Multifunction_mod.Meta;
using Multifunction_mod.Patchs.Core;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using UnityEngine;
using static Multifunction_mod.Multifunction;

namespace Multifunction_mod.Patchs.Dyson
{
    /// <summary>
    /// 跳过太阳帆子弹阶段：弹射器发射时不再生成飞行中的太阳帆，而是把太阳帆直接塞进缓存，
    /// 由 DysonSwarm.GameTick 一次性加入轨道，省去打帆子弹的飞行过程。
    /// 注意：类不能声明为 static —— PatchToggle&lt;T&gt; 的类型实参不能是静态类型（CS0718）。
    /// </summary>
    public class SkipBulletPatch
    {
        private static long _sailLifeTime;
        public static string Patchid => ModInfo.GUID + "." + nameof(SkipBulletPatch);
        private static Tempsail[][] _sailsCache;
        private static int[] _sailsCacheLen, _sailsCacheCapacity;

        public static bool Enable
        {
            get { return PatchToggle<SkipBulletPatch>.Enable; }
            set
            {
                if (PatchToggle<SkipBulletPatch>.Enable == value) return;
                Init();
                PatchToggle<SkipBulletPatch>.Enable = value;
                if (value)
                {
                    GameLifecyclePatch.OnGameBegin += GameMain_Begin_Postfix;
                    UpdateSailLifeTime();
                }
                else
                {
                    GameLifecyclePatch.OnGameBegin -= GameMain_Begin_Postfix;
                }
            }
        }

        public static bool CheckPatched()
        {
            var patches = Harmony.GetPatchInfo(typeof(EjectorComponent).GetMethod(nameof(EjectorComponent.InternalUpdate)));
            return patches.Transpilers.Count > 0;
        }

        private static void GameMain_Begin_Postfix()
        {
            Init();
        }

        private static void UpdateSailLifeTime()
        {
            if (GameMain.history == null) return;
            _sailLifeTime = (long)(GameMain.history.solarSailLife * 60f + 0.1f);
        }

        public static void Init()
        {
            var galaxy = GameMain.data?.galaxy;
            if (galaxy == null) return;
            var starCount = GameMain.data.galaxy.starCount;
            _sailsCache = new Tempsail[starCount][];
            _sailsCacheLen = new int[starCount];
            _sailsCacheCapacity = new int[starCount];
            Array.Clear(_sailsCacheLen, 0, starCount);
            Array.Clear(_sailsCacheCapacity, 0, starCount);
            UpdateSailLifeTime();
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(GameHistoryData), nameof(GameHistoryData.UnlockTechFunction))]
        private static void GameHistoryData_SetForNewGame_Postfix(int func)
        {
            if (func == 12)
            {
                UpdateSailLifeTime();
            }
        }

        [HarmonyTranspiler]
        [HarmonyPatch(typeof(EjectorComponent), nameof(EjectorComponent.InternalUpdate))]
        private static IEnumerable<CodeInstruction> EjectorComponent_InternalUpdate_Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            var matcher = new CodeMatcher(instructions, generator);
            var endLabel = generator.DefineLabel(); // 定义跳转目标标签
            matcher
                .MatchForward(
                    true,
                    new CodeMatch(OpCodes.Ldc_R4, 10f),
                          new CodeMatch(OpCodes.Stfld))
                .Advance(1)
                .Insert(
                    new CodeInstruction(OpCodes.Ldarg_0),
                        new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(EjectorComponent), nameof(EjectorComponent.fired))),
                        new CodeInstruction(OpCodes.Brtrue, endLabel))
                .MatchForward(
                    true,
                    new CodeMatch(OpCodes.Pop)
                ).Insert(
                    new CodeInstruction(OpCodes.Nop).WithLabels(endLabel),
                    new CodeInstruction(OpCodes.Ldarg_3),
                    new CodeInstruction(OpCodes.Ldarg_3),
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(EjectorComponent), nameof(EjectorComponent.orbitId))),
                    new CodeInstruction(OpCodes.Ldloc_S, 9),
                    new CodeInstruction(OpCodes.Ldloc_S, 11),
                    new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(SkipBulletPatch), nameof(AddDysonSail))));
            //matcher.Start().Advance(start).RemoveInstructions(end - start).Insert(
            //    new CodeInstruction(OpCodes.Ldarg_3),
            //    new CodeInstruction(OpCodes.Ldarg_0),
            //    new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(EjectorComponent), nameof(EjectorComponent.orbitId))),
            //    new CodeInstruction(OpCodes.Ldloc_S, 9),
            //    new CodeInstruction(OpCodes.Ldloc_S, 11),
            //    new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(SkipBulletPatch), nameof(AddDysonSail)))
            //);
            return matcher.InstructionEnumeration();
        }

        private static void AddDysonSail(DysonSwarm swarm, int orbitId, VectorLF3 uPos, VectorLF3 endVec)
        {
            var index = swarm.starData.index;
            var delta1 = endVec - swarm.starData.uPosition;
            var delta2 = VectorLF3.Cross(endVec - uPos, swarm.orbits[orbitId].up).normalized * Math.Sqrt(swarm.dysonSphere.gravity / swarm.orbits[orbitId].radius);
            lock (swarm)
            {
                var cache = _sailsCache[index];
                var len = _sailsCacheLen[index];
                if (cache == null)
                {
                    SetSailsCacheCapacity(index, 256);
                    cache = _sailsCache[index];
                }
                else
                {
                    var capacity = _sailsCacheCapacity[index];
                    if (len >= capacity)
                    {
                        SetSailsCacheCapacity(index, capacity * 2);
                        cache = _sailsCache[index];
                    }
                }
                _sailsCacheLen[index] = len + 1;
                cache[len].time = _sailLifeTime;
                cache[len].FromData(delta1, delta2, orbitId);
            }
        }

        private static void SetSailsCacheCapacity(int index, int capacity)
        {
            var newCache = new Tempsail[capacity];
            var len = _sailsCacheLen[index];
            if (len > 0)
            {
                Array.Copy(_sailsCache[index], newCache, len);
            }
            _sailsCache[index] = newCache;
            _sailsCacheCapacity[index] = capacity;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(DysonSwarm), "GameTick")]
        public static void DysonSwarmPatch(ref DysonSwarm __instance, long time)
        {
            if (__instance.starData == null || _sailsCacheLen == null) return;
            var sdIndex = __instance.starData.index;
            if (_sailsCacheLen[sdIndex] == 0) return;
            for (int i = _sailsCacheLen[sdIndex] - 1; i >= 0; i--)
            {
                Tempsail tempsail = _sailsCache[sdIndex][i];
                __instance.AddSolarSail(tempsail.ss, tempsail.orbitid, tempsail.time + time);
            }
            _sailsCacheLen[sdIndex] = 0;
        }
    }

    /// <summary>太阳帆缓存条目：暂存一颗待加入轨道的太阳帆及其轨道、寿命。</summary>
    public struct Tempsail
    {
        public DysonSail ss;
        public int orbitid;
        public long time;
        public void FromData(in VectorLF3 delta1, in VectorLF3 delta2, int orbitId)
        {
            ss.px = (float)delta1.x;
            ss.py = (float)delta1.y;
            ss.pz = (float)delta1.z;
            ss.vx = (float)delta2.x;
            ss.vy = (float)delta2.y;
            ss.vz = (float)delta2.z;
            ss.gs = 1f;
            orbitid = orbitId;
            time = (long)(GameMain.history.solarSailLife * 60f + 0.1f);
        }
    }
}
