using HarmonyLib;
using System;
using System.Numerics;
using UnityEngine;
using static Multifunction_mod.Multifunction;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

namespace Multifunction_mod.Patchs
{
    public class SomePatch
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(DispenserComponent), "InternalTick")]
        public static void DispenserComponentInternalTickPrefix(ref DispenserComponent __instance)
        {
            if (!Stationfullenergy.Value)
            {
                return;
            }
            __instance.energy = __instance.energyMax;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(EjectorComponent), "InternalUpdate")]
        public static bool EjectorComponentPatch(ref EjectorComponent __instance,long tick, AstroData[] astroPoses, AnimData[] animPool, ref DysonSwarm swarm, float power, int[] consumeRegister, ref uint __result)
        {
            if (!playcancelsolarbullet && !alwaysemissiontemp)
            {
                return true;
            }
            if (__instance.needs == null)
            {
                __instance.needs = new int[6];
            }
            var originneedFindNextOrbit = (bool)Traverse.Create(__instance).Field("needFindNextOrbit").GetValue();
            __instance.needs[0] = ((__instance.bulletCount >= 20) ? 0 : __instance.bulletId);
            animPool[__instance.entityId].prepare_length = __instance.localDir.x;
            animPool[__instance.entityId].working_length = __instance.localDir.y;
            animPool[__instance.entityId].power = __instance.localDir.z;
            __instance.targetState = EjectorComponent.ETargetState.None;
            if (__instance.fired)
            {
                int num = __instance.entityId;
                animPool[num].time = animPool[num].time + 0.016666668f;
                if (animPool[__instance.entityId].time >= 11f)
                {
                    __instance.fired = false;
                    animPool[__instance.entityId].time = 0f;
                }
            }
            else if (__instance.direction > 0)
            {
                animPool[__instance.entityId].time = (float)__instance.time / __instance.chargeSpend;
            }
            else if (__instance.direction < 0)
            {
                animPool[__instance.entityId].time = -(float)__instance.time / __instance.coldSpend;
            }
            else
            {
                animPool[__instance.entityId].time = 0f;
            }
            if (!__instance.autoOrbit)
            {
                __instance.runtimeOrbitId = __instance.orbitId;
            }
            if (__instance.orbitId < 0 || __instance.orbitId >= swarm.orbitCursor || swarm.orbits[__instance.orbitId].id != __instance.orbitId || !swarm.orbits[__instance.orbitId].enabled)
            {
                __instance.orbitId = 0;
            }
            if (swarm.orbits[__instance.runtimeOrbitId].id != __instance.runtimeOrbitId || !swarm.orbits[__instance.runtimeOrbitId].enabled)
            {
                __instance.runtimeOrbitId = __instance.orbitId;
            }
            if (swarm.orbits[__instance.findingOrbitId].id != __instance.findingOrbitId || !swarm.orbits[__instance.findingOrbitId].enabled)
            {
                __instance.findingOrbitId = __instance.orbitId;
            }
            float num2 = (float)Cargo.accTableMilli[__instance.incLevel];
            int num3 = (int)(power * 10000f * (1f + num2) + 0.1f);
            if (__instance.boost)
            {
                num3 *= 10;
            }
            if (__instance.runtimeOrbitId == 0 && !originneedFindNextOrbit)
            {
                if (__instance.autoOrbit)
                {
                    originneedFindNextOrbit = true;
                    Traverse.Create(__instance).Field("needFindNextOrbit").SetValue(originneedFindNextOrbit);
                }
                if (__instance.direction == 1)
                {
                    __instance.time = (int)((long)__instance.time * __instance.coldSpend / __instance.chargeSpend);
                    __instance.direction = -1;
                }
                if (__instance.direction == -1)
                {
                    __instance.time -= num3;
                    if (__instance.time <= 0)
                    {
                        __instance.time = 0;
                        __instance.direction = 0;
                    }
                }
                if (power >= 0.1f)
                {
                    __instance.localDir.x = __instance.localDir.x * 0.9f;
                    __instance.localDir.y = __instance.localDir.y * 0.9f;
                    __instance.localDir.z = __instance.localDir.z * 0.9f + 0.1f;
                    __result= 1U;
                    return false;
                }
                __result = 0U;
                return false;
            }
            else
            {
                if (power < 0.1f)
                {
                    if (__instance.direction == 1)
                    {
                        __instance.time = (int)((long)__instance.time * __instance.coldSpend / __instance.chargeSpend);
                        __instance.direction = -1;
                    }
                    __result = 0;
                    return false;
                }
                uint result = 0U;
                __instance.targetState = EjectorComponent.ETargetState.OK;
                bool flag = true;
                int num4 = __instance.planetId / 100 * 100;
                float num5 = __instance.localAlt + __instance.pivotY + (__instance.muzzleY - __instance.pivotY) / Mathf.Max(0.1f, Mathf.Sqrt(1f - __instance.localDir.y * __instance.localDir.y));
                Vector3 vector = new Vector3(__instance.localPosN.x * num5, __instance.localPosN.y * num5, __instance.localPosN.z * num5);
                VectorLF3 vectorLF = astroPoses[__instance.planetId].uPos + Maths.QRotateLF(astroPoses[__instance.planetId].uRot, vector);
                Quaternion q = astroPoses[__instance.planetId].uRot * __instance.localRot;
                VectorLF3 uPos = astroPoses[num4].uPos;
                VectorLF3 b = uPos - vectorLF;
                if (originneedFindNextOrbit)
                {
                    int num6 = 0;
                    long num7 = tick % 30L;
                    long num8 = (long)(__instance.id % 30);
                    if (num7 == num8 && __instance.orbitId != 0)
                    {
                        num6 = __instance.orbitId;
                    }
                    else if ((num7 + 15L) % 30L == num8)
                    {
                        int num9 = __instance.findingOrbitId + 1;
                        if (num9 >= swarm.orbitCursor)
                        {
                            num9 = 1;
                        }
                        while (swarm.orbits[num9].id != num9 || !swarm.orbits[num9].enabled)
                        {
                            num9++;
                            if (num9 >= swarm.orbitCursor)
                            {
                                num9 = 1;
                            }
                            if (num9 == __instance.runtimeOrbitId)
                            {
                                break;
                            }
                        }
                        num6 = num9;
                        __instance.findingOrbitId = num9;
                    }
                    if (num6 != 0)
                    {
                        VectorLF3 vectorLF2 = uPos + VectorLF3.Cross(swarm.orbits[num6].up, b).normalized * (double)swarm.orbits[num6].radius - vectorLF;
                        __instance.targetDist = vectorLF2.magnitude;
                        vectorLF2.x /= __instance.targetDist;
                        vectorLF2.y /= __instance.targetDist;
                        vectorLF2.z /= __instance.targetDist;
                        Vector3 vector2 = Maths.QInvRotate(q, vectorLF2);
                        if ((double)vector2.y >= 0.08715574 && vector2.y <= 0.8660254f)
                        {
                            bool flag2 = false;
                            for (int i = num4 + 1; i <= __instance.planetId + 2; i++)
                            {
                                if (i != __instance.planetId)
                                {
                                    double num10 = (double)astroPoses[i].uRadius;
                                    if (num10 > 1.0)
                                    {
                                        VectorLF3 vectorLF3 = astroPoses[i].uPos - vectorLF;
                                        double num11 = vectorLF3.x * vectorLF3.x + vectorLF3.y * vectorLF3.y + vectorLF3.z * vectorLF3.z;
                                        double num12 = vectorLF3.x * vectorLF2.x + vectorLF3.y * vectorLF2.y + vectorLF3.z * vectorLF2.z;
                                        if (num12 > 0.0)
                                        {
                                            double num13 = num11 - num12 * num12;
                                            num10 += 120.0;
                                            if (num13 < num10 * num10)
                                            {
                                                flag2 = true;
                                                break;
                                            }
                                        }
                                    }
                                }
                            }
                            if (!flag2)
                            {
                                __instance.runtimeOrbitId = num6;
                            }
                        }
                    }
                }
                bool flag3 = __instance.bulletCount > 0;
                VectorLF3 vectorLF4 = uPos + VectorLF3.Cross(swarm.orbits[__instance.runtimeOrbitId].up, b).normalized * (double)swarm.orbits[__instance.runtimeOrbitId].radius;
                VectorLF3 vectorLF5 = vectorLF4 - vectorLF;
                __instance.targetDist = vectorLF5.magnitude;
                vectorLF5.x /= __instance.targetDist;
                vectorLF5.y /= __instance.targetDist;
                vectorLF5.z /= __instance.targetDist;
                Vector3 vector3 = Maths.QInvRotate(q, vectorLF5);
                if (!alwaysemissiontemp)
                {
                    if (vector3.y < 0.08715574 || vector3.y > 0.8660254f)
                    {
                        __instance.targetState = EjectorComponent.ETargetState.AngleLimit;
                        flag = false;
                    }
                    if (flag3 && flag)
                    {
                        for (int j = num4 + 1; j <= __instance.planetId + 2; j++)
                        {
                            if (j != __instance.planetId)
                            {
                                double num14 = astroPoses[j].uRadius;
                                if (num14 > 1.0)
                                {
                                    VectorLF3 vectorLF6 = astroPoses[j].uPos - vectorLF;
                                    double num15 = vectorLF6.x * vectorLF6.x + vectorLF6.y * vectorLF6.y + vectorLF6.z * vectorLF6.z;
                                    double num16 = vectorLF6.x * vectorLF5.x + vectorLF6.y * vectorLF5.y + vectorLF6.z * vectorLF5.z;
                                    if (num16 > 0.0)
                                    {
                                        double num17 = num15 - num16 * num16;
                                        num14 += 120.0;
                                        if (num17 < num14 * num14)
                                        {
                                            flag = false;
                                            __instance.targetState = EjectorComponent.ETargetState.Blocked;
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                if (!__instance.autoOrbit || (flag && __instance.runtimeOrbitId != 0))
                {
                    originneedFindNextOrbit = false;
                    Traverse.Create(__instance).Field("needFindNextOrbit").SetValue(originneedFindNextOrbit);
                    __instance.localDir.x = __instance.localDir.x * 0.9f + vector3.x * 0.1f;
                    __instance.localDir.y = __instance.localDir.y * 0.9f + vector3.y * 0.1f;
                    __instance.localDir.z = __instance.localDir.z * 0.9f + vector3.z * 0.1f;
                    bool flag4 = flag && flag3;
                    result = (flag3 ? (flag ? 4U : 3U) : 2U);
                    if (__instance.direction == 1)
                    {
                        if (!flag4)
                        {
                            __instance.time = __instance.time * __instance.coldSpend / __instance.chargeSpend;
                            __instance.direction = -1;
                        }
                    }
                    else if (__instance.direction == 0 && flag4)
                    {
                        __instance.direction = 1;
                    }
                    if (__instance.direction == 1)
                    {
                        __instance.time += num3;
                        if (__instance.time >= __instance.chargeSpend)
                        {
                            __instance.fired = true;
                            animPool[__instance.entityId].time = 10f; 
                            if (playcancelsolarbullet)
                            {
                                VectorLF3 vectorLF1 = vectorLF4 - swarm.starData.uPosition;
                                var uEndVel = VectorLF3.Cross(vectorLF4 - uPos, swarm.orbits[__instance.runtimeOrbitId].up).normalized * Math.Sqrt((double)(swarm.dysonSphere.gravity / swarm.orbits[__instance.runtimeOrbitId].radius));
                                var ss = new DysonSail()
                                {
                                    px = (float)vectorLF1.x,
                                    py = (float)vectorLF1.y,
                                    pz = (float)vectorLF1.z,
                                    vx = (float)uEndVel.x,
                                    vy = (float)uEndVel.y,
                                    vz = (float)uEndVel.z,
                                    gs = 1f
                                };
                                lock (tempsails)
                                {
                                    tempsails.Add(new Tempsail(ss, __instance.orbitId, (long)(GameMain.history.solarSailLife * 60f + 0.1f), swarm.starData.index));
                                }
                            }
                            else
                            {
                                swarm.AddBullet(new SailBullet
                                {
                                    maxt = (float)(__instance.targetDist / 5000.0),
                                    lBegin = vector,
                                    uEndVel = VectorLF3.Cross(vectorLF4 - uPos, swarm.orbits[__instance.runtimeOrbitId].up).normalized * Math.Sqrt((double)(swarm.dysonSphere.gravity / swarm.orbits[__instance.runtimeOrbitId].radius)),
                                    uBegin = vectorLF,
                                    uEnd = vectorLF4
                                }, __instance.runtimeOrbitId);

                            }
                            int num18 = __instance.bulletInc / __instance.bulletCount;
                            if (!__instance.incUsed)
                            {
                                __instance.incUsed = (num18 > 0);
                            }
                            __instance.bulletInc -= num18;
                            __instance.bulletCount--;
                            if (__instance.bulletCount == 0)
                            {
                                __instance.bulletInc = 0;
                            }
                            lock (consumeRegister)
                            {
                                consumeRegister[__instance.bulletId]++;
                            }
                            __instance.time = __instance.coldSpend;
                            __instance.direction = -1;
                        }
                    }
                    else if (__instance.direction == -1)
                    {
                        __instance.time -= num3;
                        if (__instance.time <= 0)
                        {
                            __instance.time = 0;
                            __instance.direction = (flag4 ? 1 : 0);
                        }
                    }
                    else
                    {
                        __instance.time = 0;
                    }
                    return false;
                }
                originneedFindNextOrbit = true;
                Traverse.Create(__instance).Field("needFindNextOrbit").SetValue(originneedFindNextOrbit);
                __instance.runtimeOrbitId = 0;
                if (__instance.direction == 1)
                {
                    __instance.time = (int)((long)__instance.time * (long)__instance.coldSpend / (long)__instance.chargeSpend);
                    __instance.direction = -1;
                }
                if (__instance.direction == -1)
                {
                    __instance.time -= num3;
                    if (__instance.time <= 0)
                    {
                        __instance.time = 0;
                        __instance.direction = 0;
                    }
                }
                if (power >= 0.1f)
                {
                    __instance.localDir.x = __instance.localDir.x * 0.9f;
                    __instance.localDir.y = __instance.localDir.y * 0.9f;
                    __instance.localDir.z = __instance.localDir.z * 0.9f + 0.1f;
                    __result = 1;
                    return false;
                }
                __result = 0;
                return false;
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(PropertySystem), "GetItemTotalProperty")]
        public static void GetItemTotalPropertyPatch(ref int __result)
        {
            if (!Property9999999)
                return;
            __result = int.MaxValue;
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(ItemProto), "isFluid")]
        public static void ItemProtoisFluidPatch(ref bool __result)
        {
            if (!Tankcontentall.Value)
            {
                return;
            }
            __result = true;
        }

    }
}
