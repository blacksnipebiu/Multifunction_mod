using HarmonyLib;
using System;
using UnityEngine;
using static Multifunction_mod.Multifunction;

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
        public static bool EjectorComponentPatch(ref EjectorComponent __instance, AstroData[] astroPoses, AnimData[] animPool, ref DysonSwarm swarm, float power, int[] consumeRegister, ref uint __result)
        {
            if (!playcancelsolarbullet && !alwaysemissiontemp)
            {
                return true;
            }
            if (__instance.needs == null)
            {
                __instance.needs = new int[6];
            }
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
            if (__instance.orbitId < 0 || __instance.orbitId >= swarm.orbitCursor || swarm.orbits[__instance.orbitId].id != __instance.orbitId || !swarm.orbits[__instance.orbitId].enabled)
            {
                __instance.orbitId = 0;
            }
            float num2 = (float)Cargo.accTableMilli[__instance.incLevel];
            int num3 = (int)(power * 10000f * (1f + num2) + 0.1f);
            if (__instance.orbitId == 0)
            {
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
                    __instance.localDir.x *= 0.9f;
                    __instance.localDir.y *= 0.9f;
                    __instance.localDir.z = __instance.localDir.z * 0.9f + 0.1f;
                    __result = 1U;
                    return false;
                }
                __result = 0U;
            }
            else
            {
                if (power < 0.1f)
                {
                    if (__instance.direction == 1)
                    {
                        __instance.time = __instance.time * __instance.coldSpend / __instance.chargeSpend;
                        __instance.direction = -1;
                    }
                    __result = 0U;
                    return false;
                }
                __instance.targetState = EjectorComponent.ETargetState.OK;
                bool flag = true;
                int num4 = __instance.planetId / 100 * 100;
                float num5 = __instance.localAlt + __instance.pivotY + (__instance.muzzleY - __instance.pivotY) / Mathf.Max(0.1f, Mathf.Sqrt(1f - __instance.localDir.y * __instance.localDir.y));

                Vector3 vector = new Vector3(__instance.localPosN.x * num5, __instance.localPosN.y * num5, __instance.localPosN.z * num5);
                VectorLF3 uPos = astroPoses[num4].uPos;
                VectorLF3 vectorLF = astroPoses[__instance.planetId].uPos + Maths.QRotateLF(astroPoses[__instance.planetId].uRot, vector);
                VectorLF3 b = uPos - vectorLF;
                VectorLF3 vectorLF2 = uPos + VectorLF3.Cross(swarm.orbits[__instance.orbitId].up, b).normalized * swarm.orbits[__instance.orbitId].radius;
                if (!alwaysemissiontemp)
                {
                    Quaternion q = astroPoses[__instance.planetId].uRot * __instance.localRot;
                    VectorLF3 vectorLF3 = vectorLF2 - vectorLF;
                    __instance.targetDist = vectorLF3.magnitude;
                    vectorLF3.x /= __instance.targetDist;
                    vectorLF3.y /= __instance.targetDist;
                    vectorLF3.z /= __instance.targetDist;
                    Vector3 vector2 = Maths.QInvRotate(q, vectorLF3);
                    __instance.localDir.x = __instance.localDir.x * 0.9f + vector2.x * 0.1f;
                    __instance.localDir.y = __instance.localDir.y * 0.9f + vector2.y * 0.1f;
                    __instance.localDir.z = __instance.localDir.z * 0.9f + vector2.z * 0.1f;
                    if (vector2.y < 0.08715574 || vector2.y > 0.8660254f)
                    {
                        __instance.targetState = EjectorComponent.ETargetState.AngleLimit;
                        flag = false;
                    }
                    if (__instance.bulletCount > 0 && flag)
                    {
                        for (int i = num4 + 1; i <= __instance.planetId + 2; i++)
                        {
                            if (i == __instance.planetId)
                            {
                                continue;
                            }
                            double uradius = astroPoses[i].uRadius;
                            if (uradius <= 1.0)
                            {
                                continue;
                            }
                            VectorLF3 vectorLF4 = astroPoses[i].uPos - vectorLF;
                            double num7 = vectorLF4.x * vectorLF4.x + vectorLF4.y * vectorLF4.y + vectorLF4.z * vectorLF4.z;
                            double num8 = vectorLF4.x * vectorLF3.x + vectorLF4.y * vectorLF3.y + vectorLF4.z * vectorLF3.z;
                            if (num8 <= 0.0)
                            {
                                continue;
                            }
                            double num9 = num7 - num8 * num8;
                            uradius += 120.0;
                            if (num9 >= uradius * uradius)
                            {
                                continue;
                            }
                            flag = false;
                            __instance.targetState = EjectorComponent.ETargetState.Blocked;
                            break;
                        }
                    }
                }

                bool flag2 = __instance.bulletCount > 0;
                bool flag3 = flag && flag2;
                if (__instance.direction == 1)
                {
                    if (!flag3)
                    {
                        __instance.time = __instance.time * __instance.coldSpend / __instance.chargeSpend;
                        __instance.direction = -1;
                    }
                }
                else if (__instance.direction == 0 && flag3)
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
                        VectorLF3 uEndVel = VectorLF3.Cross(vectorLF2 - uPos, swarm.orbits[__instance.orbitId].up).normalized * Math.Sqrt((double)(swarm.dysonSphere.gravity / swarm.orbits[__instance.orbitId].radius));
                        if (playcancelsolarbullet)
                        {
                            VectorLF3 vectorLF1 = vectorLF2 - swarm.starData.uPosition;
                            DysonSail ss = new DysonSail
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
                                uEndVel = uEndVel,
                                uBegin = vectorLF,
                                uEnd = vectorLF2
                            }, __instance.orbitId);

                        }
                        __instance.bulletInc -= __instance.bulletInc / __instance.bulletCount;
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
                        __instance.direction = (flag3 ? 1 : 0);
                    }
                }
                else
                {
                    __instance.time = 0;
                }
                __result = (flag2 ? (flag ? 4U : 3U) : 2U);
            }
            return false;
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
