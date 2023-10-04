using HarmonyLib;
using System;
using System.Collections.Generic;
using UnityEngine;
using static Multfunction_mod.Multifunction;

namespace Multfunction_mod
{
    public class Tempsail
    {
        public DysonSail ss;
        public int orbitid;
        public int index;
        public long time;

        public Tempsail(DysonSail ss, int orbitId, long time, int index)
        {
            this.ss = ss;
            orbitid = orbitId;
            this.time = time;
            this.index = index;
        }
    }
    public class Multifunctionpatch
    {
        public static int ability = 4;
        public static int ejectorNum;
        public static int siloNum;
        public static float time;
        public static void InitPatch()
        {
            ejectorNum = 0;
            siloNum = 0;
        }

        public static void Patchallmethod()
        {
            Harmony harmony = new Harmony(GUID);
            var m = typeof(StorageComponent).GetMethods();
            foreach (var i in m)
            {
                if (i.Name == "TakeTailItems" && i.ReturnType == typeof(void))
                {
                    var prefix = typeof(Multifunctionpatch).GetMethod("TakeTailItemsPatch");
                    harmony.Patch(i, new HarmonyMethod(prefix));
                    break;
                }
            }
            m = typeof(FactorySystem).GetMethods();
            harmony.PatchAll(typeof(Multifunctionpatch));
        }

        public static bool TakeTailItemsPatch(StorageComponent __instance, ref int itemId)
        {
            if (ArchitectMode.Value)
            {
                if (__instance == null || __instance.id != GameMain.mainPlayer.package.id) return true;
                if (itemId <= 0 || itemId >= 6007) return true;
                if (LDB.items.Select(itemId).CanBuild) return false;
            }
            return true;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(DysonSwarm), "AbsorbSail")]
        public static bool DysonSwarmPatch1(ref DysonSwarm __instance, ref bool __result, DysonNode node)
        {
            if (quickabsorbsolar.Value)
            {
                if (__instance.expiryCursor == __instance.expiryEnding)
                {
                    __result = false;
                    return false;
                }
                __instance.expiryEnding--;
                if (__instance.expiryEnding < 0)
                {
                    __instance.expiryEnding += __instance.sailCapacity;
                }
                int num = __instance.expiryEnding;
                int index = __instance.expiryOrder[num].index;
                if (__instance.expiryOrder[num].time == 0L)
                {
                    Assert.CannotBeReached();
                    __result = false;
                    return false;
                }
                __instance.expiryOrder[num].time = 0L;
                __instance.expiryOrder[num].index = 0;
                if (node != null && node.ConstructCp() != null)
                {
                    __instance.dysonSphere.productRegister[11903]++;
                }
                __instance.RemoveSolarSail(index);
                return false;
            }
            return true;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(DysonSwarm), "GameTick")]
        public static void DysonSwarmPatch2(ref DysonSwarm __instance, long time)
        {
            if (playcancelsolarbullet && __instance.bulletCursor > 1)
            {
                int tempnum = 1;
                for (int i = 1; i < __instance.bulletCursor; i++)
                {
                    if (__instance.bulletPool[i].id == i)
                    {
                        tempnum++;
                    }
                    if (tempnum > 1) break;
                }
                if (tempnum == 1)
                {
                    __instance.bulletCursor = 1;
                    __instance.bulletCapacity = 128;
                    __instance.bulletRecycleCursor = 1;
                    __instance.bulletRecycle = new int[128];
                    __instance.bulletPool = new SailBullet[128];
                }
            }
            for (int i = tempsails.Count - 1; i >= 0; i--)
            {
                Tempsail tempsail = tempsails[i];
                if (tempsail != null)
                {
                    if (__instance.starData.index != tempsail.index) continue;
                    __instance.AddSolarSail(tempsail.ss, tempsail.orbitid, tempsail.time + time);
                }
                tempsails.RemoveAt(i);
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(DysonSphere), "Init")]
        public static void DysonSphereInit(ref DysonSphere __instance)
        {
            if (ChangeDysonradius.Value)
            {
                __instance.minOrbitRadius = 100;
                __instance.maxOrbitRadius = MaxOrbitRadiusConfig.Value;
            }
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(DysonSphereLayer), "GameTick")]
        public static void DysonSphereLayerGameTick(ref DysonSphereLayer __instance, long gameTick)
        {
            if (QuickabortSwarm.Value)
            {
                DysonSwarm swarm = __instance.dysonSphere.swarm;
                for (int i = 1; i < __instance.nodeCursor; i++)
                {
                    DysonNode dysonNode = __instance.nodePool[i];
                    if (dysonNode != null && dysonNode.id == i && dysonNode.sp == dysonNode.spMax)
                    {
                        dysonNode.OrderConstructCp(gameTick, swarm);
                    }
                }
            }
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(SiloComponent), "InternalUpdate")]
        public static void SiloComponentInternalUpdatePatch(ref SiloComponent __instance)
        {
            if (TempSiloRandomEmission && SiloNumber != 0 && __instance.chargeSpend != 0)
            {
                if (siloNum > 0 && siloNum % SiloNumber == 0)
                {
                    TempSiloRandomEmission = false;
                }
                else
                {
                    siloNum++;
                    __instance.time = siloNum * 500000 % __instance.chargeSpend;
                }
            }
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(EjectorComponent), "InternalUpdate")]
        public static bool EjectorComponentPatch(ref EjectorComponent __instance, AstroData[] astroPoses, AnimData[] animPool, ref DysonSwarm swarm, float power, int[] consumeRegister, ref uint __result)
        {
            if (TempEjectorRandomEmission && EjectorNumber > 0 && __instance.chargeSpend > 0)
            {
                if (ejectorNum > 0 && ejectorNum % EjectorNumber == 0)
                {
                    TempEjectorRandomEmission = false;
                }
                else
                {
                    ejectorNum++;
                    __instance.time = ejectorNum * 10000 % __instance.chargeSpend;
                }
            }
            if (playcancelsolarbullet || alwaysemissiontemp)
            {
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
                        __instance.localDir.x = __instance.localDir.x * 0.9f;
                        __instance.localDir.y = __instance.localDir.y * 0.9f;
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
                                if (i != __instance.planetId)
                                {
                                    double num6 = astroPoses[i].uRadius;
                                    if (num6 > 1.0)
                                    {
                                        VectorLF3 vectorLF4 = astroPoses[i].uPos - vectorLF;
                                        double num7 = vectorLF4.x * vectorLF4.x + vectorLF4.y * vectorLF4.y + vectorLF4.z * vectorLF4.z;
                                        double num8 = vectorLF4.x * vectorLF3.x + vectorLF4.y * vectorLF3.y + vectorLF4.z * vectorLF3.z;
                                        if (num8 > 0.0)
                                        {
                                            double num9 = num7 - num8 * num8;
                                            num6 += 120.0;
                                            if (num9 < num6 * num6)
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
                                DysonSail ss = default(DysonSail);
                                VectorLF3 vectorLF1 = vectorLF2 - swarm.starData.uPosition;
                                ss.px = (float)vectorLF1.x;
                                ss.py = (float)vectorLF1.y;
                                ss.pz = (float)vectorLF1.z;
                                vectorLF1 = uEndVel;
                                vectorLF1 += RandomTable.SphericNormal(ref swarm.randSeed, 0.5);
                                ss.vx = (float)vectorLF1.x;
                                ss.vy = (float)vectorLF1.y;
                                ss.vz = (float)vectorLF1.z;
                                ss.gs = 1f;
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
            return true;
        }

        //无翘曲器曲速
        [HarmonyPostfix]
        [HarmonyPatch(typeof(Mecha), "UseWarper")]
        public static void EnoughWarperPatch(ref bool __result)
        {
            if (noneedwarp.Value)
            {
                __result = true;
            }
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(Mecha), "UseEnergy")]
        public static bool InfiniteplayerpowerPatch(Mecha __instance, ref float __result)
        {
            if (Infiniteplayerpower.Value)
            {
                __result = 1;
                __instance.coreEnergy = __instance.coreEnergyCap;
                return false;
            }
            return true;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(MechaForge), "GameTick")]
        public static void Prefix(ref MechaForge __instance)
        {
            if (QuickHandcraft.Value && __instance.tasks.Count > 0)
            {
                __instance.tasks[0].tick = __instance.tasks[0].tickSpend;
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(PlayerAction_Mine), "GameTick")]
        public static void PlayerAction_MineGameTickPatch(ref PlayerAction_Mine __instance)
        {
            var factory = __instance.player.factory;
            if (QuickPlayerMine.Value && __instance.miningId > 0 && factory != null && GameMain.localPlanet != null)
            {
                if (__instance.miningType == EObjectType.Vegetable)
                {
                    VegeData vegeData = factory.GetVegeData(__instance.miningId);
                    VegeProto vegeProto = LDB.veges.Select(vegeData.protoId);
                    if (vegeProto != null)
                    {
                        __instance.miningTick = LDB.veges.Select(factory.GetVegeData(__instance.miningId).protoId).MiningTime * 10000;
                    }
                }
                else if (__instance.miningType == EObjectType.Vein)
                {
                    VeinData veinData = factory.GetVeinData(__instance.miningId);
                    VeinProto veinProto = LDB.veins.Select((int)veinData.type);
                    if (veinProto != null)
                    {
                        __instance.miningTick = veinProto.MiningTime * 10000;
                    }
                }
            }
        }

        //关闭窗口快捷键
        [HarmonyPrefix]
        [HarmonyPatch(typeof(UIGame), "On_E_Switch")]
        public static bool CloseWindowPatch(ref UIGame __instance)
        {
            if (guidraw.DisplayingWindow)
            {
                guidraw.CloseMainWindow();
                return false;
            }
            return true;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(Player), "TryAddItemToPackage")]
        public static bool PlayerTryAddItemToPackage()
        {
            if (dismantle_but_nobuild.Value || entityitemnoneed || Itemdelete_bool)
            {
                Itemdelete_bool = false;
                return false;
            }
            return true;
        }

        //自动改名
        [HarmonyPostfix]
        [HarmonyPatch(typeof(PlanetTransport), "NewStationComponent")]
        public static void PlanetTransportNewStationComponent(ref StationComponent __result)
        {
            if (__result.isCollector)
            {
                CollectorStation.Add(__result.gid);
                return;
            }
            else if (__result.isVeinCollector)
            {
                return;
            }
            if (Quantumtransport_bool.Value)
            {
                if (autochangeQuantumstationname)
                {
                    __result.name = "星球量子传输站";
                }
                if (autochangeQuantumStarstationname)
                {
                    __result.name = "星系量子传输站";
                }
            }
            else if (autochangestationname.Value)
            {
                __result.name = Localization.language != Language.zhCN ? "Station_miner" : "星球矿机";
            }
            if (Buildingnoconsume.Value)
            {
                GameMain.localPlanet.factory.powerSystem.consumerPool[__result.pcId].idleEnergyPerTick = 1000;
            }
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(StationComponent), "Init")]
        public static void StationComponentInit(ref int _extraStorage, PrefabDesc _desc, StationComponent __instance)
        {
            int basemax = __instance.isStellar ? 10000 : 5000;
            _extraStorage = (StationStoExtra.Value + 1) * basemax;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(PlanetTransport), "RemoveStationComponent")]
        public static void PlanetTransportRemoveStationComponent(PlanetTransport __instance, int id)
        {
            if (__instance.stationPool[id] != null && __instance.stationPool[id].id != 0 && __instance.stationPool[id].isCollector)
            {
                int gid = __instance.stationPool[id].gid;
                if (CollectorStation.Contains(gid))
                {
                    CollectorStation.Remove(gid);
                }
            }
        }


        [HarmonyPostfix]
        [HarmonyPatch(typeof(PlayerAction_Inspect), "GetObjectSelectDistance")]
        public static void PlayerAction_InspectPatch(ref float __result)
        {
            if (InspectDisNoLimit.Value)
            {
                __result = 400;
            }
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(UIAbnormalityTip), "Determine")]
        public static bool UIAbnormalityTipDeterminePatch()
        {
            return !CloseUIAbnormalityTip.Value;
        }


        [HarmonyPostfix]
        [HarmonyPatch(typeof(PowerSystem), "NewGeneratorComponent")]
        public static void PowerSystemNewGeneratorComponent(ref int __result, PowerSystem __instance)
        {
            if (InfineteStarPower.Value && __instance.genPool[__result].fuelMask == 4)
            {
                __instance.genPool[__result].fuelId = 1803;
                __instance.genPool[__result].fuelCount = 100;
                __instance.genPool[__result].fuelEnergy = long.MaxValue;
                __instance.genPool[__result].genEnergyPerTick = 1000000000000;
            }
            if (WindturbinesUnlimitedEnergy.Value && __instance.genPool[__result].wind)
            {
                __instance.genPool[__result].genEnergyPerTick = 100_000_000_0000;
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(PowerSystem), "NewConsumerComponent")]
        public static void PowerSystemNewConsumerComponent(ref int __result, PowerSystem __instance)
        {
            if (Buildingnoconsume.Value)
            {
                __instance.consumerPool[__result].requiredEnergy = 0;
                __instance.consumerPool[__result].idleEnergyPerTick = 0;
                __instance.consumerPool[__result].servedEnergy = 0;
                __instance.consumerPool[__result].workEnergyPerTick = 0;
            }
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(PowerSystem), "NewNodeComponent")]
        public static void NewNodeComponentPatchPrefix(PowerSystem __instance, ref int entityId, ref float conn, ref float cover)
        {
            if (!Windturbinescovertheglobe.Value && !PlanetPower_bool.Value) return;
            var itemID = LDB.items.Select(__instance.factory.entityPool[entityId].protoId).ID;
            if (PlanetPower_bool.Value && itemID == 2210)
            {
                cover = GameMain.localPlanet.realRadius * 4;
                if (farconnectdistance)
                {
                    conn = GameMain.localPlanet.realRadius * 1.5f;
                    farconnectdistance = false;
                }
            }
            if (Windturbinescovertheglobe.Value && itemID == 2203)
            {
                cover = GameMain.localPlanet.realRadius * 4;
            }
        }
        [HarmonyPostfix]
        [HarmonyPatch(typeof(PowerSystem), "NewNodeComponent")]
        public static void NewNodeComponentPatchPostfix(ref int __result, PowerSystem __instance)
        {
            if (Buildingnoconsume.Value && GameMain.localPlanet.factory.entityPool[__instance.nodePool[__result].entityId].stationId <= 0)
            {
                __instance.nodePool[__result].requiredEnergy = 0;
                __instance.nodePool[__result].idleEnergyPerTick = 0;
            }
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(BuildTool_Inserter), "CheckBuildConditions")]
        public static bool BuildTool_InserterCheckBuildConditionsPatch(ref bool __result)
        {
            if (PasteBuildAnyWay)
            {
                __result = true;
                return false;
            }
            return true;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(BuildTool_Addon), "CheckBuildConditions")]
        public static bool BuildTool_AddonCheckBuildConditionsPatch(ref bool __result)
        {
            if (PasteBuildAnyWay)
            {
                __result = true;
                return false;
            }
            return true;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(BuildTool_Path), "CheckBuildConditions")]
        public static bool BuildTool_PathCheckBuildConditionsPatch(ref bool __result)
        {
            if (PasteBuildAnyWay)
            {
                __result = true;
                return false;
            }
            return true;
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(FactorySystem), "NewEjectorComponent")]
        public static void NewEjectorComponentPatch(ref int __result, FactorySystem __instance)
        {
            if (quickEjector)
            {
                __instance.ejectorPool[__result].bulletCount = int.MaxValue;
                __instance.ejectorPool[__result].bulletId = 1501;
                __instance.ejectorPool[__result].coldSpend = 5;
                __instance.ejectorPool[__result].chargeSpend = 4;
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(PropertySystem), "GetItemTotalProperty")]
        public static void GetItemTotalPropertyPatch(ref int __result)
        {
            if (Property9999999)
                __result = int.MaxValue;
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(FactorySystem), "NewSiloComponent")]
        public static void NewSiloComponentPatch(ref int __result, FactorySystem __instance)
        {
            if (quicksilo)
            {
                __instance.siloPool[__result].bulletCount = int.MaxValue;
                __instance.siloPool[__result].bulletId = 1503;
                __instance.siloPool[__result].coldSpend = 40;
                __instance.siloPool[__result].chargeSpend = 80;
            }

        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(FactoryStorage), "NewTankComponent")]
        public static void NewTankComponentPatch(ref int fCount)
        {
            if (Infinitestoragetank.Value)
                fCount = int.MaxValue - 200;
        }



        [HarmonyPrefix]
        [HarmonyPatch(typeof(StarSimulator), "LateUpdate")]
        public static bool StarSimulatorLateUpdate(ref StarSimulator __instance, Material ___bodyMaterial, Material ___haloMaterial)
        {
            __instance.sunLight.enabled = GameMain.localStar == __instance.starData && !FactoryModel.whiteMode0;
            if (GameMain.localStar == __instance.starData)
            {
                if (!FactoryModel.whiteMode0)
                {
                    Vector3 vector3 = sunlight_bool.Value ? GameMain.mainPlayer.transform.up : __instance.transform.forward;
                    Shader.SetGlobalVector("_Global_SunDir", new Vector4(vector3.x, vector3.y, vector3.z, 0.0f));
                    Shader.SetGlobalColor("_Global_SunsetColor0", Color.Lerp(Color.white, __instance.sunsetColor0, __instance.useSunsetColor));
                    Shader.SetGlobalColor("_Global_SunsetColor1", Color.Lerp(Color.white, __instance.sunsetColor1, __instance.useSunsetColor));
                    Shader.SetGlobalColor("_Global_SunsetColor2", Color.Lerp(Color.white, __instance.sunsetColor2, __instance.useSunsetColor));
                }
                else
                {
                    Transform transform = GameCamera.instance.camLight.transform;
                    transform.rotation = Quaternion.LookRotation((GameMain.mainPlayer.position * 0.75f - transform.position).normalized, transform.position.normalized);
                    Vector3 vector = -GameCamera.instance.camLight.transform.forward;
                    Shader.SetGlobalVector("_Global_SunDir", new Vector4(vector.x, vector.y, vector.z, 0f));
                    Shader.SetGlobalColor("_Global_SunsetColor0", Color.white);
                    Shader.SetGlobalColor("_Global_SunsetColor1", Color.white);
                    Shader.SetGlobalColor("_Global_SunsetColor2", Color.white);
                }
            }
            ___bodyMaterial.renderQueue = GameMain.localStar == __instance.starData ? 2981 : 2979;
            ___haloMaterial.renderQueue = GameMain.localStar == __instance.starData ? 2981 : 2979;
            __instance.blackRenderer.enabled = GameMain.localStar == __instance.starData && __instance.starData.type != EStarType.BlackHole;
            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(PlanetSimulator), "LateUpdate")]
        public static bool PlanetSimulatorLateUpdatePatch(PlanetSimulator __instance)
        {
            PlanetData localPlanet = GameMain.localPlanet;
            if (localPlanet == __instance.planetData && localPlanet != null)
            {
                Vector3 vector3 = Quaternion.Inverse(localPlanet.runtimeRotation) * (__instance.planetData.star.uPosition - __instance.planetData.uPosition).normalized;
                vector3 = sunlight_bool.Value ? GameMain.mainPlayer.transform.up : vector3;
                if (FactoryModel.whiteMode0)
                    vector3 = -GameCamera.instance.camLight.transform.forward;
                if (__instance.surfaceRenderer != null && __instance.surfaceRenderer.Length != 0)
                    __instance.surfaceRenderer[0].sharedMaterial.SetVector("_SunDir", (Vector4)vector3);
                if (__instance.reformMat0 != null)
                    __instance.reformMat0.SetVector("_SunDir", (Vector4)vector3);
                if (__instance.reformMat1 != null)
                    __instance.reformMat1.SetVector("_SunDir", (Vector4)vector3);
                if (__instance.atmoMat != null)
                    __instance.atmoMat.SetVector("_SunDir", (Vector4)vector3);
            }
            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(ProductionStatistics), "GameTick")]
        public static bool Prefix(ProductionStatistics __instance)
        {
            if (!GameMain.instance.running || !FinallyInit)
            {
                addStatDic = new Dictionary<int, Dictionary<int, long>>();
                consumeStatDic = new Dictionary<int, Dictionary<int, long>>();
                return true;
            }
            if (GameMain.galaxy != null && (Time.time - time) > 0.5f)
            {
                time = Time.time;
                if (addStatDic != null)
                {
                    var key1s = new List<int>(addStatDic.Keys);
                    foreach (var key1 in key1s)
                    {
                        var key2s = new List<int>(addStatDic[key1].Keys);
                        int factoryIndex = GameMain.galaxy.PlanetById(key1).factoryIndex;
                        foreach (var key2 in key2s)
                        {
                            if (addStatDic[key1][key2] == 0) continue;
                            int itemNum = addStatDic[key1][key2] > int.MaxValue ? int.MaxValue : (int)addStatDic[key1][key2];
                            __instance.factoryStatPool[factoryIndex].productRegister[key2] += itemNum;
                            addStatDic[key1][key2] -= itemNum;
                        }
                    }
                }
                if (consumeStatDic != null)
                {
                    List<int> key1s = new List<int>(consumeStatDic.Keys);
                    foreach (var key1 in key1s)
                    {
                        int factoryIndex = GameMain.galaxy.PlanetById(key1).factoryIndex;
                        List<int> key2s = new List<int>(consumeStatDic[key1].Keys);
                        foreach (var key2 in key2s)
                        {
                            if (consumeStatDic[key1][key2] == 0) continue;
                            int itemNum = consumeStatDic[key1][key2] > int.MaxValue ? int.MaxValue : (int)consumeStatDic[key1][key2];
                            __instance.factoryStatPool[factoryIndex].consumeRegister[key2] += itemNum;
                            consumeStatDic[key1][key2] -= itemNum;
                        }
                    }
                }
            }

            return true;
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(PlanetFactory), "ComputeFlattenTerrainReform")]
        public static void PlanetFactoryNoComsumeSand(ref int __result)
        {
            if (InfiniteSand.Value)
            {
                __result = 0;
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(PlanetFactory), "UpgradeEntityWithComponents")]
        public static void UpgradeEntityWithComponentsPatch(int entityId, PlanetFactory __instance)
        {
            if (entityId == 0 || __instance.entityPool[entityId].id == 0)
                return;
            if (Buildingnoconsume.Value && GameMain.localPlanet.factory.entityPool[entityId].stationId <= 0)
            {
                int powerConId = __instance.entityPool[entityId].powerConId;
                if (powerConId > 0)
                {
                    __instance.powerSystem.consumerPool[powerConId].idleEnergyPerTick = 0;
                    __instance.powerSystem.consumerPool[powerConId].workEnergyPerTick = 0;
                }
            }
        }


        [HarmonyPrefix]
        [HarmonyPatch(typeof(PlanetFactory), "TakeBackItemsInEntity")]
        public static bool TakeBackItemsInEntityPatch()
        {
            return !entityitemnoneed;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(PlanetFactory), "ComputeFlattenTerrainReform")]
        public static void ComputeFlattenTerrainReformPatch(PlanetFactory __instance, Vector3[] points, Vector3 center, float radius, int pointsCount, float fade0 = 3f)
        {
            if (!restorewater) return;
            PlanetRawData data = __instance.planet.data;
            if (tmp_levelChanges == null)
                tmp_levelChanges = new Dictionary<int, int>();
            tmp_levelChanges.Clear();
            Quaternion quaternion = Maths.SphericalRotation(center, 22.5f);
            float realRadius = __instance.planet.realRadius;
            Vector3[] vertices = data.vertices;
            ushort[] heightData = data.heightData;
            float num1 = Mathf.Min(9f, Mathf.Abs((float)((heightData[data.QueryIndex(center)] - (double)__instance.planet.realRadius * 100.0 + 20.0) * 0.00999999977648258 * 2.0)));
            fade0 += num1;
            float num2 = radius + fade0;
            float num3 = num2 * num2;
            float num4 = (float)((double)realRadius * 3.14159274101257 / (__instance.planet.precision * 2.0));
            int num5 = Mathf.CeilToInt((float)((double)num2 * 1.41400003433228 / (double)num4 * 1.5 + 0.5));
            Vector3[] vector3Array = new Vector3[9]
            {
                    center,
                    center + quaternion * (new Vector3(0.0f, 0.0f, 1.414f) * num2),
                    center + quaternion * (new Vector3(0.0f, 0.0f, -1.414f) * num2),
                    center + quaternion * (new Vector3(1.414f, 0.0f, 0.0f) * num2),
                    center + quaternion * (new Vector3(-1.414f, 0.0f, 0.0f) * num2),
                    center + quaternion * (new Vector3(1f, 0.0f, 1f) * num2),
                    center + quaternion * (new Vector3(-1f, 0.0f, -1f) * num2),
                    center + quaternion * (new Vector3(1f, 0.0f, -1f) * num2),
                    center + quaternion * (new Vector3(-1f, 0.0f, 1f) * num2)
            };
            int stride = data.stride;
            int dataLength = data.dataLength;
            float num6 = 8f;
            foreach (Vector3 vpos in vector3Array)
            {
                int num8 = data.QueryIndex(vpos);
                for (int index1 = -num5; index1 <= num5; ++index1)
                {
                    int num9 = num8 + index1 * stride;
                    if (num9 >= 0 && num9 < dataLength)
                    {
                        for (int index2 = -num5; index2 <= num5; ++index2)
                        {
                            int index3 = num9 + index2;
                            if ((uint)index3 < dataLength)
                            {
                                Vector3 vector3_1;
                                vector3_1.x = vertices[index3].x * realRadius;
                                vector3_1.y = vertices[index3].y * realRadius;
                                vector3_1.z = vertices[index3].z * realRadius;
                                Vector3 vector3_2;
                                vector3_2.x = vector3_1.x - center.x;
                                vector3_2.y = vector3_1.y - center.y;
                                vector3_2.z = vector3_1.z - center.z;
                                if (!tmp_levelChanges.ContainsKey(index3))
                                {
                                    float num10 = float.PositiveInfinity;
                                    for (int index4 = 0; index4 < pointsCount; ++index4)
                                    {
                                        double num11 = points[index4].x - vector3_1.x;
                                        float num12 = points[index4].y - vector3_1.y;
                                        float num13 = points[index4].z - vector3_1.z;
                                        float num14 = (float)(num11 * num11 + (double)num12 * (double)num12 + (double)num13 * (double)num13);
                                        num10 = (double)num10 < (double)num14 ? num10 : num14;
                                    }
                                    int num15;
                                    if ((double)num10 <= (double)num6)
                                    {
                                        num15 = 3;
                                    }
                                    else
                                    {
                                        float num11 = num10 - num6;
                                        if ((double)num11 <= (double)fade0 * (double)fade0)
                                        {
                                            float num12 = num11 / (fade0 * fade0);
                                            if ((double)num12 <= 0.111111097037792)
                                                num15 = 2;
                                            else if ((double)num12 <= 0.444444388151169)
                                                num15 = 1;
                                            else if ((double)num12 < 1.0)
                                                num15 = 0;
                                            else
                                                continue;
                                        }
                                        else
                                            continue;
                                    }
                                    tmp_levelChanges[index3] = num15;
                                }
                            }
                        }
                    }
                }
            }


        }


        [HarmonyPostfix]
        [HarmonyPatch(typeof(StorageComponent), "Import")]
        public static void StorageComponentImportPatch(StorageComponent __instance)
        {
            if (StackMultiple.Value > 1)
            {
                for (int i = 0; i < __instance.size; i++)
                {
                    if (__instance.entityId > 0 && __instance.grids[i].itemId > 0)
                    {
                        ItemProto itemProto = LDB.items.Select(__instance.grids[i].itemId);
                        if (itemProto != null)
                        {
                            __instance.grids[i].stackSize = itemProto.StackSize * StackMultiple.Value;
                        }
                    }
                }
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(StorageComponent), "LoadStatic")]
        public static void Postfix()
        {
            if (StackMultiple.Value > 1)
            {
                ItemProto[] dataArray = LDB.items.dataArray;
                for (int j = 0; j < dataArray.Length; j++)
                {
                    StorageComponent.itemStackCount[dataArray[j].ID] = dataArray[j].StackSize * StackMultiple.Value;
                }
            }

        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(StorageComponent), "TakeItemFromGrid")]
        public static void DetermineMoreChainTargetsPatch(StorageComponent __instance, int gridIndex, ref int itemId, ref int count)
        {
            if (lockpackage_bool.Value && GameMain.mainPlayer != null && __instance == GameMain.mainPlayer.package)
            {
                __instance.grids[gridIndex].itemId = itemId;
                __instance.grids[gridIndex].count = count;
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(StorageComponent), "GetItemCount", new Type[] { typeof(int) })]
        public static void GetItemCountPatch(StorageComponent __instance, int itemId, ref int __result)
        {
            if (ArchitectMode.Value)
            {
                if (__instance == null || __instance.id != GameMain.mainPlayer.package.id) return;
                if (itemId <= 0 || itemId >= 6007) return;
                if (LDB.items.Select(itemId).CanBuild && __result == 0) __result = 100;
            }
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(PlanetRawData), "AddModLevel")]
        public static bool Prefix(int index, ref int level, PlanetRawData __instance)
        {
            if (!restorewater) return true;
            int num1 = __instance.modData[index >> 1] >> ((index & 1) << 2) & 3;
            level = -4 - num1;
            return true;

        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(PlatformSystem), "SetReformType")]
        public static bool Prefix(ref int type)
        {
            if (restorewater) type = 0;

            return true;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(UITechTree), "_OnUpdate")]
        public static void UITechTreeSelectTechPatch(UITechTree __instance)
        {
            if (unlockpointtech && __instance.selected != null)
            {
                TechProto techProto = __instance.selected.techProto;
                if (GameMain.history.techStates[techProto.ID].unlocked) return;
                if (techProto.MaxLevel <= 10)
                {
                    if (techProto.Level < techProto.MaxLevel)
                    {
                        for (int level = 1; level < techProto.MaxLevel; ++level)
                        {
                            for (int i = 0; i < techProto.itemArray.Length; i++)
                            {
                                AddComsumeItemtoTotal(techProto.Items[i], (int)(techProto.ItemPoints[i] * techProto.GetHashNeeded(techProto.Level) / 3600));
                            }
                            GameMain.history.UnlockTechFunction(techProto.UnlockFunctions[0], techProto.UnlockValues[0], level);
                        }
                        GameMain.history.UnlockTech(techProto.ID);
                    }
                    if (!GameMain.history.techStates[techProto.ID].unlocked)
                    {
                        for (int i = 0; i < techProto.itemArray.Length; i++)
                        {
                            AddComsumeItemtoTotal(techProto.Items[i], (int)(techProto.ItemPoints[i] * techProto.GetHashNeeded(techProto.Level) / 3600));
                        }
                        GameMain.history.UnlockTech(techProto.ID);
                    }
                }
            }
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(UITechTree), "UpdateScale")]
        public static bool UITechTreeUpdateScalePatch(UITechTree __instance)
        {
            if (unlockpointtech && (__instance.selected != null || __instance.centerViewNode != null)) return false;
            return true;
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(ItemProto), "isFluid")]
        public static void ItemProtoisFluidPatch(ref bool __result)
        {
            if (Tankcontentall.Value)
            {
                __result = true;
            }
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(BuildTool_BlueprintPaste), "CheckBuildConditions")]
        public static bool Prefix(BuildTool_BlueprintPaste __instance, ref bool __result)
        {
            if (pasteanyway && __instance.bpPool != null)
            {
                for (int i = 0; i < __instance.bpPool.Length; i++)
                {
                    if (__instance.bpPool[i] != null && __instance.bpPool[i].item != null && __instance.bpPool[i].item.prefabDesc != null)
                    {
                        if (__instance.bpPool[i].item.prefabDesc.veinMiner) return true;
                    }
                }
                __result = true;
                return false;
            }
            return true;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(BuildTool_BlueprintPaste), "CreatePrebuilds")]
        public static void BuildTool_BlueprintPasteCreatePrebuildsPatch(ref BuildTool_BlueprintPaste __instance)
        {
            if (DriftBuildings)
            {
                for (int i = 0; i < __instance.bpCursor; i++)
                {
                    if (__instance.bpPool[i].desc.isBelt)
                    {
                        __instance.bpPool[i].lpos *= 1 + DriftBuildingLevel * 0.0066f;
                        __instance.bpPool[i].lpos2 *= 1 + DriftBuildingLevel * 0.0066f;
                    }
                }
            }
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(BuildTool_BlueprintPaste), "UpdatePreviewModels")]
        public static void BuildTool_BlueprintPastePrefix(ref BuildTool_BlueprintPaste __instance)
        {
            if (DriftBuildings)
            {
                for (int i = 0; i < __instance.bpCursor; i++)
                {
                    if (__instance.bpPool[i].desc.isBelt)
                    {
                        __instance.bpPool[i].lpos *= 1 + DriftBuildingLevel * 0.0066f;
                        __instance.bpPool[i].lpos2 *= 1 + DriftBuildingLevel * 0.0066f;
                    }
                }
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(BuildTool_BlueprintPaste), "UpdatePreviewModels")]
        public static void BuildTool_BlueprintPastePostfix(ref BuildTool_BlueprintPaste __instance)
        {
            if (DriftBuildings)
            {
                for (int i = 0; i < __instance.bpCursor; i++)
                {
                    if (__instance.bpPool[i].desc.isBelt)
                    {
                        __instance.bpPool[i].lpos /= 1 + DriftBuildingLevel * 0.0066f;
                        __instance.bpPool[i].lpos2 /= 1 + DriftBuildingLevel * 0.0066f;
                    }
                }
            }
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(BuildTool_BlueprintPaste), "AlterBPGPUIModel")]
        public static void BuildTool_BlueprintPasteAlterBPGPUIModelPatch(ref BuildPreview _bp)
        {
            if (DriftBuildings)
            {
                _bp.lpos *= 1 + DriftBuildingLevel * 0.0066f;
                _bp.lpos2 *= 1 + DriftBuildingLevel * 0.0066f;
            }
        }


        [HarmonyPrefix]
        [HarmonyPatch(typeof(DispenserComponent), "InternalTick")]
        public static void DispenserComponentInternalTickPrefix(ref DispenserComponent __instance)
        {
            if (Stationfullenergy.Value)
            {
                __instance.energy = __instance.energyMax;
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(DispenserComponent), "InternalTick")]
        public static void DispenserComponentInternalTickPostfix(ref DispenserComponent __instance)
        {
            if (Stationfullenergy.Value)
            {
                __instance.energy = __instance.energyMax;
            }
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(StationComponent), "InternalTickLocal")]
        public static void InternalTickLocalPatchPrefix(ref StationComponent __instance)
        {
            if (Stationfullenergy.Value)
            {
                __instance.energy = __instance.energyMax;
            }
            if (StationMaxproliferator.Value)
            {
                for (int i = 0; i < __instance.storage.Length; i++)
                    if (__instance.storage[i].itemId > 0)
                        __instance.storage[i].inc = __instance.storage[i].count * incAbility + 100;
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(StationComponent), "InternalTickLocal")]
        public static void InternalTickLocalPatchPostfix(ref StationComponent __instance)
        {
            if (Stationfullenergy.Value)
            {
                __instance.energy = __instance.energyMax;
            }
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(TankComponent), "GameTick")]
        public static void TankComponentGameTickPatchPrefix(ref TankComponent __instance)
        {
            if (TankMaxproliferator.Value)
            {
                __instance.fluidInc = __instance.fluidCount * incAbility;
                if (__instance.fluidCount > 100) __instance.fluidInc += 100;
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(TankComponent), "GameTick")]
        public static void TankComponentGameTickPatchPostfix(ref TankComponent __instance)
        {
            if (TankMaxproliferator.Value)
            {
                __instance.fluidInc = __instance.fluidCount * incAbility;
            }
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(SpraycoaterComponent), "InternalUpdate")]
        public static void SpraycoaterComponentInternalUpdatePrefix(ref SpraycoaterComponent __instance)
        {
            if (Maxproliferator.Value)
            {
                ability = __instance.incAbility;
                __instance.incAbility = 10;
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(SpraycoaterComponent), "InternalUpdate")]
        public static void SpraycoaterComponentInternalUpdatePostfix(ref SpraycoaterComponent __instance)
        {
            if (!Maxproliferator.Value && __instance.incAbility > 4)
            {
                __instance.incAbility = ability;
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(BuildTool_Click), "_OnInit")]
        public static void DeterminePreviewsPatch(BuildTool_Click __instance)
        {
            __instance.dotsSnapped = new Vector3[Buildmaxlen.Value];
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(BuildTool_Click), "CheckBuildConditions")]
        public static bool CheckBuildConditionsPatchPrefix(BuildTool_Click __instance, ref bool __result)
        {
            if (PasteBuildAnyWay)
            {
                for (int i = 0; i < __instance.buildPreviews.Count; i++)
                {
                    if (__instance.buildPreviews[i] != null && __instance.buildPreviews[i].item != null && __instance.buildPreviews[i].item.prefabDesc != null)
                    {
                        if (__instance.buildPreviews[i].item.prefabDesc.veinMiner) return true;
                    }
                }
                __result = true;
                return false;
            }
            return true;
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(BuildTool_Click), "CheckBuildConditions")]
        public static void CheckBuildConditionsPatchPostfix(BuildTool_Click __instance, ref bool __result)
        {
            if (!build_gascol_noequator.Value && !build_tooclose_bool.Value)
                return;
            for (int index = 0; index < __instance.buildPreviews.Count; ++index)
            {
                BuildPreview buildPreview = __instance.buildPreviews[index];
                if (build_gascol_noequator.Value && buildPreview.item.ID == 2105)
                {
                    buildPreview.condition = EBuildCondition.Ok;
                    __result = true;
                }
                if (build_tooclose_bool.Value && (buildPreview.condition == EBuildCondition.TowerTooClose || buildPreview.condition == EBuildCondition.MK2MinerTooClose))
                {
                    buildPreview.condition = EBuildCondition.Ok;
                    __result = true;
                }
            }
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(BuildTool_Click), "UpdatePreviewModels")]
        public static void Prefix(ref BuildTool_Click __instance)
        {
            if (DriftBuildings)
            {
                for (int i = 0; i < __instance.buildPreviews.Count; i++)
                {
                    __instance.buildPreviews[i].lpos *= 1 + DriftBuildingLevel * 0.0066f;
                    __instance.buildPreviews[i].lpos2 *= 1 + DriftBuildingLevel * 0.0066f;
                }
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(CargoTraffic), "SetBeltSignalIcon")]
        public static void SetBeltSignalIconPatch(int signalId, int entityId, CargoTraffic __instance)
        {
            if (!BeltSignalFunction.Value) return;
            int factoryIndex = GameMain.localPlanet.factoryIndex;
            int beltid = __instance.factory.entityPool[entityId].beltId;
            if (!Beltsignal.ContainsKey(factoryIndex))
                Beltsignal.Add(factoryIndex, new Dictionary<int, int>());
            if (!Beltsignal[factoryIndex].ContainsKey(beltid))
                Beltsignal[factoryIndex].Add(beltid, signalId);
            else
                Beltsignal[factoryIndex][beltid] = signalId;
            if (Beltsignalnumberoutput.ContainsKey(factoryIndex) && Beltsignalnumberoutput[factoryIndex].ContainsKey(beltid) && signalId != 601)
            {
                Beltsignalnumberoutput[factoryIndex].Remove(beltid);
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(CargoTraffic), "SetBeltSignalNumber")]
        public static void SetBeltSignalNumberPatch(float number, int entityId, CargoTraffic __instance)
        {
            if (!BeltSignalFunction.Value) return;
            if (__instance.factory.entitySignPool[entityId].iconType != 0U && __instance.factory.entitySignPool[entityId].iconId0 != 0U)
            {
                int factoryIndex = GameMain.localPlanet.factoryIndex;
                int beltid = __instance.factory.entityPool[entityId].beltId;
                if (__instance.factory.entitySignPool[entityId].iconId0 == 600) { }
                else if (__instance.factory.entitySignPool[entityId].iconId0 == 601)
                {
                    if (!Beltsignalnumberoutput.ContainsKey(factoryIndex))
                    {
                        Beltsignalnumberoutput.Add(factoryIndex, new Dictionary<int, int>());
                    }
                    if (!Beltsignalnumberoutput[factoryIndex].ContainsKey(beltid))
                        Beltsignalnumberoutput[factoryIndex].Add(beltid, (int)number);
                    else
                        Beltsignalnumberoutput[factoryIndex][beltid] = (int)number;
                }
                else
                {
                    if (Beltsignalnumberoutput.ContainsKey(factoryIndex) && Beltsignalnumberoutput[factoryIndex].ContainsKey(beltid))
                    {
                        Beltsignalnumberoutput[factoryIndex][beltid] = (int)number;
                    }
                }
            }
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(CargoTraffic), "TryPickItemAtRear")]
        public static void Prefix(ref int[] needs)
        {
            if (Tankcontentall.Value && needs != null)
            {
                if (ItemProto.fluids == needs)
                    needs = null;
            }
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(CargoTraffic), "RemoveBeltComponent")]
        public static void Prefix(int id)
        {
            if (!BeltSignalFunction.Value) return;
            int factoryIndex = GameMain.localPlanet.factoryIndex;
            if (Beltsignal.ContainsKey(factoryIndex) && Beltsignal[factoryIndex].ContainsKey(id))
                Beltsignal[factoryIndex].Remove(id);
            if (Beltsignalnumberoutput.ContainsKey(factoryIndex) && Beltsignalnumberoutput[factoryIndex].ContainsKey(id))
                Beltsignalnumberoutput[factoryIndex].Remove(id);
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(GameData), "Import")]
        public static void GameDataImport()
        {

        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(GameData), "GameTick")]
        public static void GameDataGameTickPatch()
        {
            if (!BeltSignalFunction.Value || GameMain.instance == null || !GameMain.instance.running || GameMain.galaxy == null || !FinallyInit) return;
            foreach (KeyValuePair<int, Dictionary<int, int>> wap1 in Beltsignal)
            {
                FactorySystem fs = GameMain.data.factories[wap1.Key].factorySystem;
                foreach (KeyValuePair<int, int> wap2 in wap1.Value)
                {
                    int signalId = wap2.Value;
                    byte stack = 1;
                    byte inc = 0;
                    if (signalId == 404)
                    {
                        BeltComponent belt = fs.traffic.beltPool[wap2.Key];
                        CargoPath cargoPath = fs.factory.cargoTraffic.GetCargoPath(belt.segPathId);
                        cargoPath.TryPickItem(belt.segIndex + belt.segPivotOffset - 5, 12, out stack, out inc);
                    }
                    else if (1000 <= signalId && signalId < 20000)
                    {
                        BeltComponent belt = fs.traffic.beltPool[wap2.Key];
                        int index = belt.segIndex + belt.segPivotOffset;
                        int beltnumber = (int)fs.factory.entitySignPool[belt.entityId].count0;
                        if (beltnumber == 999999)
                            fs.factory.cargoTraffic.GetCargoPath(belt.segPathId).TryInsertItem(belt.segIndex + belt.segPivotOffset, signalId, 1, 0);
                        else if (beltnumber > 999900 && beltnumber % 10 != 0)
                        {
                            int t = beltnumber % 100;
                            int stack1 = t % 10;
                            int inc1 = ((t / 10 >= 3 ? 4 : t / 10)) * stack1;
                            fs.factory.cargoTraffic.GetCargoPath(belt.segPathId).TryInsertItem(belt.segIndex + belt.segPivotOffset, signalId, (byte)stack1, (byte)inc1);
                        }
                        else if (beltnumber == 1 || (beltnumber >= 11 && beltnumber <= 14))
                        {
                            bool breakfor = false;
                            CargoPath cargoPath = fs.factory.cargoTraffic.GetCargoPath(belt.segPathId);
                            int cargoId = -1;
                            int offset = -1;
                            Cargo cargo;
                            cargoPath.GetCargoAtIndex(index, out cargo, out cargoId, out offset);
                            if (cargo.item <= 0)
                            {
                                int outputstack = 0;
                                int outputinc = 0;
                                if (fs.factory.transport != null && fs.factory.transport.stationPool != null)
                                {
                                    foreach (StationComponent sc in fs.factory.transport.stationPool)
                                    {
                                        if (breakfor) break;
                                        if (sc != null && sc.storage != null)
                                        {
                                            for (int i = 0; i < sc.storage.Length; i++)
                                            {
                                                if (sc.storage[i].itemId == signalId && sc.storage[i].count > 0)
                                                {
                                                    int stack1 = beltnumber % 10 > sc.storage[i].count ? sc.storage[i].count : beltnumber % 10;
                                                    int inc1 = sc.storage[i].inc > 3 * stack1 ? 3 * stack1 : sc.storage[i].inc;
                                                    sc.TakeItem(ref signalId, ref stack1, out inc1);
                                                    outputstack += stack1;
                                                    outputinc += inc1;
                                                    if (outputstack >= beltnumber % 10 && !cargoPath.TryInsertItem(index, signalId, (byte)outputstack, (byte)outputinc))
                                                    {
                                                        sc.AddItem(signalId, stack1, inc1);
                                                        breakfor = true;
                                                        break;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                                if (breakfor) continue;
                                if (fs.factory.factoryStorage != null && fs.factory.factoryStorage.storagePool != null)
                                {
                                    foreach (StorageComponent sc in fs.factory.factoryStorage.storagePool)
                                    {
                                        if (breakfor) break;
                                        int itemnum = 0;
                                        if (sc != null && !sc.isEmpty && (itemnum = sc.GetItemCount(signalId)) > 0)
                                        {
                                            int inc1 = 0;
                                            int stack1 = (beltnumber % 10 - outputstack) > itemnum ? itemnum : beltnumber % 10;
                                            sc.TakeItem(signalId, stack1, out inc1);
                                            outputstack += stack1;
                                            outputinc += inc1;
                                            int remaininc;
                                            if (outputstack >= beltnumber % 10 && !cargoPath.TryInsertItem(index, signalId, (byte)outputstack, (byte)outputinc))
                                                sc.AddItem(signalId, stack1, inc1, out remaininc);
                                            breakfor = true;
                                            break;
                                        }
                                    }
                                }
                                if (breakfor) continue;
                                if (fs.factory.factoryStorage != null && fs.factory.factoryStorage.tankPool != null)
                                {
                                    foreach (TankComponent tc in fs.factory.factoryStorage.tankPool)
                                    {
                                        if (breakfor) break;
                                        if (tc.fluidId == signalId && tc.id > 0 && tc.fluidCount > 0)
                                        {
                                            int stack1 = (beltnumber % 10 - outputstack) > tc.fluidCount ? tc.fluidCount : beltnumber % 10;
                                            int inc1 = tc.fluidInc >= 4 * stack1 ? 4 * stack1 : tc.fluidInc;
                                            outputstack += stack1;
                                            outputinc += inc1;
                                            if (outputstack >= beltnumber % 10 && cargoPath.TryInsertItem(index, signalId, (byte)outputstack, (byte)outputinc))
                                            {
                                                fs.factory.factoryStorage.tankPool[tc.id].fluidInc -= inc1;
                                                fs.factory.factoryStorage.tankPool[tc.id].fluidCount -= stack1;
                                                breakfor = true;
                                                break;
                                            }
                                        }
                                    }
                                }
                                if (breakfor || outputstack == 0) continue;
                                else
                                {
                                    cargoPath.TryInsertItem(index, signalId, (byte)outputstack, (byte)outputinc);
                                }
                            }
                        }
                        else if (beltnumber == 2 || (beltnumber >= 21 && beltnumber <= 24))
                        {
                            if (fs.minerPool != null)
                            {
                                foreach (MinerComponent mc in fs.minerPool)
                                {
                                    if (mc.id > 0 && mc.entityId > 0 && mc.productId == signalId && mc.productCount > 0)
                                    {
                                        int count = fs.factory.cargoTraffic.GetCargoPath(belt.segPathId).TryInsertItem(index, signalId, (byte)(beltnumber > 2 ? beltnumber % 20 : 2), 0) ? 1 : 0;
                                        fs.minerPool[mc.id].productCount -= count--;
                                        break;
                                    }
                                }
                            }
                        }

                    }
                    else if (signalId == 405)
                    {
                        BeltComponent belt = fs.traffic.beltPool[wap2.Key];
                        CargoPath cargoPath = fs.factory.cargoTraffic.GetCargoPath(belt.segPathId);
                        int itemid = 0;
                        int cargoId = -1;
                        int offset = -1;
                        Cargo cargo;
                        int num1 = belt.segIndex + belt.segPivotOffset;
                        if (cargoPath.GetCargoAtIndex(num1, out cargo, out cargoId, out offset))
                        {
                            itemid = LDB.items.Select(cargo.item).ID;
                        }
                        if (itemid < 1000) continue;
                        bool breakfor = false;
                        switch (fs.factory.entitySignPool[belt.entityId].count0)
                        {
                            case 1:
                                if (itemid != 1006 && itemid != 1007 && itemid != 1011 && itemid != 1109 && itemid != 1114 && itemid != 1120 && itemid != 1801 && itemid != 1802) continue;
                                if (fs.factory.powerSystem == null || fs.factory.powerSystem.genPool == null) continue;
                                foreach (PowerGeneratorComponent pgc in fs.factory.powerSystem.genPool)
                                {
                                    if (pgc.id > 0 && pgc.fuelCount <= 2)
                                    {
                                        if (pgc.fuelMask == 1 && itemid != 1802)
                                        {
                                            if (itemid == fs.factory.powerSystem.genPool[pgc.id].fuelId)
                                            {
                                                cargoPath.TryPickItem(num1 - 5, 12, out stack, out inc);
                                                fs.factory.powerSystem.genPool[pgc.id].SetNewFuel(itemid, (short)(fs.factory.powerSystem.genPool[pgc.id].fuelCount + stack), inc);
                                                break;
                                            }
                                            else if (pgc.fuelCount == 0)
                                            {
                                                cargoPath.TryPickItem(num1 - 5, 12, out stack, out inc);
                                                fs.factory.powerSystem.genPool[pgc.id].SetNewFuel(itemid, (short)(fs.factory.powerSystem.genPool[pgc.id].fuelCount + stack), inc);
                                                break;
                                            }
                                        }
                                        if (pgc.fuelMask == 2 && itemid == 1802)
                                        {
                                            cargoPath.TryPickItem(num1 - 5, 12, out stack, out inc);
                                            fs.factory.powerSystem.genPool[pgc.id].SetNewFuel(1802, (short)(fs.factory.powerSystem.genPool[pgc.id].fuelCount + stack), inc);
                                            break;
                                        }
                                    }
                                }
                                break;
                            case 5:
                                if (fs.assemblerPool == null) continue;
                                breakfor = false;
                                foreach (AssemblerComponent ac in fs.assemblerPool)
                                {
                                    if (breakfor) break;
                                    if (ac.id > 0 && ac.entityId > 0 && ac.recipeId > 0)
                                    {
                                        for (int i = 0; i < ac.served.Length; i++)
                                        {
                                            if (itemid != ac.requires[i]) continue;
                                            if (ac.served[i] <= ac.requireCounts[i] * 2)
                                            {
                                                cargoPath.TryPickItem(num1 - 5, 12, out stack, out inc);
                                                ac.served[i] += stack;
                                                ac.incServed[i] += inc;
                                                breakfor = true;
                                                break;
                                            }
                                            if (ac.served[i] < 0) ac.served[i] = 0;
                                        }
                                    }
                                }
                                break;
                            case 6:
                                if (fs.factory.transport == null || fs.factory.transport.stationPool == null) continue;
                                breakfor = false;
                                foreach (StationComponent sc in fs.factory.transport.stationPool)
                                {
                                    if (breakfor) break;
                                    if (sc != null && sc.storage != null)
                                    {
                                        for (int i = 0; i < sc.storage.Length; i++)
                                        {
                                            if (sc.storage[i].itemId != itemid || sc.storage[i].count >= sc.storage[i].max) continue;
                                            cargoPath.TryPickItem(num1 - 5, 12, out stack, out inc);
                                            sc.AddItem(itemid, stack, inc);
                                            breakfor = true;
                                            break;
                                        }
                                    }
                                }
                                break;
                            case 7:
                                if (fs.labPool == null) continue;
                                breakfor = false;
                                foreach (LabComponent lc in fs.labPool)
                                {
                                    if (breakfor) break;
                                    if (lc.id > 0 && lc.entityId > 0)
                                    {
                                        if (lc.researchMode)
                                        {
                                            if (itemid < 6001 || itemid > 6006) continue;
                                            for (int i = 0; i < lc.matrixPoints.Length; i++)
                                            {
                                                if (itemid != 6001 + i) continue;
                                                if (lc.matrixServed[i] <= 36000)
                                                {
                                                    cargoPath.TryPickItem(num1 - 5, 12, out stack, out inc);
                                                    lc.matrixServed[i] += 3600 * stack;
                                                    lc.matrixIncServed[i] += 3600 * stack;
                                                    breakfor = true;
                                                    break;
                                                }
                                            }
                                        }
                                        else if (lc.matrixMode)
                                        {
                                            for (int i = 0; i < lc.served.Length; i++)
                                            {
                                                if (itemid != lc.requires[i]) continue;
                                                if (lc.served[i] <= lc.requireCounts[i] * 2)
                                                {
                                                    cargoPath.TryPickItem(num1 - 5, 12, out stack, out inc);
                                                    lc.served[i] += stack;
                                                    lc.incServed[i] += inc;
                                                    breakfor = true;
                                                    break;
                                                }
                                            }
                                        }
                                    }
                                }
                                break;
                            case 8:
                                if (itemid != 1503 && itemid != 1501 && itemid != 1209 && itemid != 1803) continue;
                                if (itemid == 1209 || itemid == 1803)
                                {
                                    if (fs.factory.powerSystem == null || fs.factory.powerSystem.genPool == null) continue;
                                    foreach (PowerGeneratorComponent pgc in fs.factory.powerSystem.genPool)
                                    {
                                        if (pgc.id > 0)
                                        {
                                            if (pgc.gamma)
                                            {
                                                if (itemid != 1209) continue;
                                                if (pgc.catalystPoint == 0)
                                                {
                                                    cargoPath.TryPickItem(num1 - 5, 12, out stack, out inc);
                                                    fs.factory.powerSystem.genPool[pgc.id].catalystPoint += 3600 * stack;
                                                    fs.factory.powerSystem.genPool[pgc.id].catalystIncPoint += 3600 * inc;
                                                    break;
                                                }

                                            }
                                            else if (pgc.fuelMask == 4 && itemid == 1803 && pgc.fuelCount <= 2)
                                            {
                                                if (itemid != 1803) continue;
                                                cargoPath.TryPickItem(num1 - 5, 12, out stack, out inc);
                                                fs.factory.powerSystem.genPool[pgc.id].SetNewFuel(1803, (short)(fs.factory.powerSystem.genPool[pgc.id].fuelCount + stack), inc);
                                                break;
                                            }
                                        }
                                    }
                                }
                                else if (itemid == 1503)
                                {
                                    if (fs.siloPool == null) continue;
                                    foreach (SiloComponent sc in fs.siloPool)
                                    {
                                        if (sc.id > 0 && sc.entityId > 0 && sc.bulletCount <= 1)
                                        {
                                            cargoPath.TryPickItem(num1 - 5, 12, out stack, out inc);
                                            fs.siloPool[sc.id].bulletCount += stack;
                                            fs.siloPool[sc.id].bulletInc += inc;
                                            break;
                                        }
                                    }
                                }
                                else if (itemid == 1501)
                                {
                                    if (fs.ejectorPool == null) continue;
                                    foreach (EjectorComponent ec in fs.ejectorPool)
                                    {
                                        if (ec.id > 0 && ec.entityId > 0 && ec.bulletCount <= 1)
                                        {
                                            cargoPath.TryPickItem(num1 - 5, 12, out stack, out inc);
                                            fs.ejectorPool[ec.id].bulletCount += stack;
                                            fs.ejectorPool[ec.id].bulletInc += inc;
                                            break;
                                        }
                                    }
                                }
                                break;
                        }
                    }
                    else if (signalId == 600)
                    {
                        BeltComponent belt = fs.traffic.beltPool[wap2.Key];
                        int index = (int)fs.factory.entitySignPool[belt.entityId].count0;
                        if (index > 0)
                        {
                            CargoPath cargoPath = fs.factory.cargoTraffic.GetCargoPath(belt.segPathId);
                            int itemid = 0;
                            int cargoId = -1;
                            int offset = -1;
                            Cargo cargo;
                            int num1 = belt.segIndex + belt.segPivotOffset;
                            if (cargoPath.GetCargoAtIndex(num1, out cargo, out cargoId, out offset))
                            {
                                itemid = cargo.item;
                                inc = cargoPath.cargoContainer.cargoPool[cargoId].inc;
                                stack = cargoPath.cargoContainer.cargoPool[cargoId].stack;
                            }
                            if (itemid < 1000) continue;
                            if (Beltsignalnumberoutput.ContainsKey(wap1.Key))
                            {
                                foreach (KeyValuePair<int, int> wap3 in Beltsignalnumberoutput[wap1.Key])
                                {
                                    if (wap3.Value == index)
                                    {
                                        BeltComponent belt1 = fs.traffic.beltPool[wap3.Key];
                                        if (fs.factory.cargoTraffic.GetCargoPath(belt1.segPathId).TryInsertItem(belt1.segIndex + belt1.segPivotOffset, itemid, stack, inc))
                                        {
                                            cargoPath.TryPickItem(num1 - 5, 12, out byte stack1, out byte inc1);
                                            if ((inc1 != inc || stack1 != stack))
                                                cargoPath.TryPickItem(num1 - 5, 12, out _, out _);
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(UIStationStorage), "GetAdditionStorage")]
        public static void UIStationWindow_OnOpen(UIStationStorage __instance, ref int __result)
        {
            if (StationStoExtra.Value == 0 || __instance.station.isCollector)
                return;
            int basemax = __instance.station.isStellar ? 10000 : 5000;
            __result += StationStoExtra.Value * basemax;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(PlanetTransport), "SetStationStorage")]
        public static bool UIStationWindow_SetStationStorage(PlanetTransport __instance, int stationId, int storageIdx, int itemId, int itemCountMax, ELogisticStorage localLogic, ELogisticStorage remoteLogic, Player player)
        {
            if (StationStoExtra.Value <= 0)
            {
                return true;
            }
            if (itemId != 0 && LDB.items.Select(itemId) == null)
            {
                itemId = 0;
            }
            StationComponent stationComponent = __instance.GetStationComponent(stationId);
            bool flag = false;
            bool flag2 = false;
            if (stationComponent != null)
            {
                if (!stationComponent.isStellar)
                {
                    remoteLogic = ELogisticStorage.None;
                }
                if (itemId <= 0)
                {
                    itemId = 0;
                    itemCountMax = 0;
                    localLogic = ELogisticStorage.None;
                    remoteLogic = ELogisticStorage.None;
                }
                int modelIndex = __instance.factory.entityPool[stationComponent.entityId].modelIndex;
                ModelProto modelProto = LDB.models.Select(modelIndex);
                int num = 0;
                if (modelProto != null)
                {
                    num = modelProto.prefabDesc.stationMaxItemCount;
                }

                int basemax = stationComponent.isStellar ? 10000 : 5000;
                int maxvalue = num + (StationStoExtra.Value + 1) * basemax;
                if (itemCountMax > maxvalue)
                {
                    itemCountMax = maxvalue;
                }
                if (storageIdx >= 0 && storageIdx < stationComponent.storage.Length)
                {
                    StationStore stationStore = stationComponent.storage[storageIdx];
                    if (stationStore.localLogic != localLogic)
                    {
                        flag = true;
                    }
                    if (stationStore.remoteLogic != remoteLogic)
                    {
                        flag2 = true;
                    }
                    if (stationStore.itemId == itemId)
                    {
                        stationComponent.storage[storageIdx].max = itemCountMax;
                        stationComponent.storage[storageIdx].localLogic = localLogic;
                        stationComponent.storage[storageIdx].remoteLogic = remoteLogic;
                    }
                    else
                    {
                        if (stationStore.localLogic != ELogisticStorage.None || localLogic != ELogisticStorage.None)
                        {
                            flag = true;
                        }
                        if (stationStore.remoteLogic != ELogisticStorage.None || remoteLogic != ELogisticStorage.None)
                        {
                            flag2 = true;
                        }
                        if (stationStore.count > 0 && stationStore.itemId > 0 && player != null)
                        {
                            int num3 = player.TryAddItemToPackage(stationStore.itemId, stationStore.count, stationStore.inc, true, 0);
                            UIItemup.Up(stationStore.itemId, num3);
                            if (num3 < stationStore.count)
                            {
                                UIRealtimeTip.Popup("无法收回仓储物品".Translate(), true, 0);
                            }
                        }
                        stationComponent.storage[storageIdx].itemId = itemId;
                        stationComponent.storage[storageIdx].count = 0;
                        stationComponent.storage[storageIdx].inc = 0;
                        stationComponent.storage[storageIdx].localOrder = 0;
                        stationComponent.storage[storageIdx].remoteOrder = 0;
                        stationComponent.storage[storageIdx].max = itemCountMax;
                        stationComponent.storage[storageIdx].localLogic = localLogic;
                        stationComponent.storage[storageIdx].remoteLogic = remoteLogic;
                    }
                    if (itemId == 0)
                    {
                        stationComponent.storage[storageIdx] = default(StationStore);
                        for (int i = 0; i < stationComponent.slots.Length; i++)
                        {
                            if (stationComponent.slots[i].dir == IODir.Output && stationComponent.slots[i].storageIdx - 1 == storageIdx)
                            {
                                stationComponent.slots[i].counter = 0;
                                stationComponent.slots[i].storageIdx = 0;
                                stationComponent.slots[i].dir = IODir.Output;
                            }
                        }
                    }
                }
                if (!stationComponent.isStellar)
                {
                    flag2 = false;
                }
            }
            if (flag)
            {
                __instance.RefreshStationTraffic(stationId);
            }
            if (flag2)
            {
                __instance.gameData.galacticTransport.RefreshTraffic(stationComponent.gid);
            }
            return false;
        }
    }
}
