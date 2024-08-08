using HarmonyLib;
using System;
using static Multifunction_mod.Multifunction;

namespace Multifunction_mod.Patchs
{
    public class PlanetTransportPatch
    {
        //自动改名
        [HarmonyPostfix]
        [HarmonyPatch(typeof(PlanetTransport), "NewStationComponent")]
        public static void PlanetTransportNewStationComponent(ref StationComponent __result, PlanetTransport __instance, int _entityId)
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
            if (!string.IsNullOrEmpty(AutoChangeStationName.Value))
            {
                Console.WriteLine(AutoChangeStationName.Value.getTranslate() + " " + _entityId + " " + __instance.factory.entityPool[_entityId].id);
                __instance.factory.WriteExtraInfoOnEntity(_entityId, AutoChangeStationName.Value.getTranslate());
                Console.WriteLine(__instance.factory.ReadExtraInfoOnEntity(_entityId));
            }
            if (Buildingnoconsume.Value)
            {
                GameMain.localPlanet.factory.powerSystem.consumerPool[__result.pcId].idleEnergyPerTick = 1000;
            }
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

        [HarmonyPrefix]
        [HarmonyPatch(typeof(PlanetTransport), "GameTick")]
        public static void PlanetTransportGameTick(long time, PlanetTransport __instance)
        {
            int num = (int)(time % 20L);
            if (num < 0)
            {
                num += 20;
            }
            int num2 = (int)(time % 60L);
            if (num2 < 0)
            {
                num2 += 60;
            }
            bool localTick = num == 0;
            bool remoteTick = num2 == 0;
            FactoryProductionStat factoryProductionStat = GameMain.statistics.production.factoryStatPool[__instance.factory.index];
            int[] productRegister = factoryProductionStat.productRegister;
            int[] consumeRegister = factoryProductionStat.consumeRegister;
            for (int i = __instance.stationPool.Length - 1; i > 0; i--)
            {
                StationComponent sc = __instance.stationPool[i];
                if (sc == null || sc.id != i)
                {
                    continue;
                }
                if (localTick)
                {
                    if (Stationfullenergy.Value)
                    {
                        sc.energy = sc.energyMax;
                    }
                    if (StationMaxproliferator.Value)
                    {
                        for (int j = 0; j < sc.storage.Length; j++)
                            if (sc.storage[j].itemId > 0)
                                sc.storage[j].inc = sc.storage[j].count * incAbility;
                    }
                    if (StationfullCount)
                    {
                        StationComponentPatch.StationFullItemCount(sc);
                    }
                }
                if (sc.isCollector || sc.isVeinCollector) continue;
                if (StationSpray.Value)
                {
                    StationComponentPatch.StationSprayInc(sc, consumeRegister);
                }
                if (StationPowerGen.Value)
                {
                    StationComponentPatch.StationPowerGeneration(sc, consumeRegister);
                }
                if (localTick)
                {
                    if (Station_infiniteWarp_bool.Value && sc.isStellar)
                        sc.warperCount = 50;
                }
                string stationComponentName = __instance.factory.ReadExtraInfoOnEntity(sc.entityId);
                if (!string.IsNullOrEmpty(stationComponentName))
                {
                    switch (stationComponentName)
                    {
                        case "星球无限供货机":
                            if (!StationfullCount_bool.Value && remoteTick || StationfullCount)
                                continue;
                            StationComponentPatch.StationFullItemCount(sc);
                            break;
                        case "垃圾站":
                        case "Station_trash":
                            if (!StationTrash.Value && remoteTick)
                                continue;
                            StationComponentPatch.StationTrashMethod(sc, consumeRegister);
                            break;
                        case "喷涂加工厂":
                            if (!StationSprayer.Value || StationSpray.Value)
                                continue;
                            StationComponentPatch.StationSprayInc(sc, consumeRegister);
                            break;
                        case "星球熔炉矿机":
                            if (!StationMinerSmelter.Value)
                                continue;
                            if (remoteTick)
                            {
                                StationComponentPatch.StationMine(sc, __instance.planet, productRegister);
                            }
                            StationComponentPatch.StationFurnaceMiner(sc, time, consumeRegister, productRegister);
                            break;
                        case "星球矿机":
                        case "Station_miner":
                            if (!StationMiner.Value)
                                continue;
                            if (remoteTick)
                            {
                                StationComponentPatch.StationMine(sc, __instance.planet, productRegister);
                            }
                            break;
                        case "星球量子传输站":
                        case "星系量子传输站":
                            break;
                    }
                }
            }
        }

    }
}
