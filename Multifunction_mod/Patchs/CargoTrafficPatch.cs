using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using static Multifunction_mod.Multifunction;

namespace Multifunction_mod.Patchs
{
    public class CargoTrafficPatch
    {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(CargoTraffic), "SetBeltSignalIcon")]
        public static void SetBeltSignalIconPatch(int signalId, int entityId, CargoTraffic __instance)
        {
            if (!BeltSignalFunction.Value) return;
            int factoryIndex = __instance.factory.index;
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
                int factoryIndex = __instance.factory.index;
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
            if (!Tankcontentall.Value || needs == null)
            {
                return;
            }
            if (ItemProto.fluids == needs)
                needs = null;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(CargoTraffic), "RemoveBeltComponent")]
        public static void Prefix(CargoTraffic __instance, int id)
        {
            if (!BeltSignalFunction.Value) return;
            int factoryIndex = __instance.factory.index;
            if (Beltsignal.ContainsKey(factoryIndex) && Beltsignal[factoryIndex].ContainsKey(id))
                Beltsignal[factoryIndex].Remove(id);
            if (Beltsignalnumberoutput.ContainsKey(factoryIndex) && Beltsignalnumberoutput[factoryIndex].ContainsKey(id))
                Beltsignalnumberoutput[factoryIndex].Remove(id);
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(CargoTraffic), "SplitterGameTick")]
        public static void CargoTrafficSplitterGameTickPatch(CargoTraffic __instance)
        {
            CargoSignalFunction(__instance);
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(CargoTraffic), "GameTick")]
        public static void CargoTrafficGameTickPatch(CargoTraffic __instance)
        {
            CargoSignalFunction(__instance);
        }

        public static void CargoSignalFunction(CargoTraffic __instance)
        {
            if (!BeltSignalFunction.Value || !FinallyInit) return;
            int factoryIndex = __instance.factory.index;
            if (!Beltsignal.ContainsKey(factoryIndex)) return;
            FactorySystem fs = __instance.factory.factorySystem;
            foreach (KeyValuePair<int, int> wap2 in Beltsignal[factoryIndex])
            {
                int signalID = wap2.Value;
                if (signalID == 404)
                {
                    __instance.TryPickItem(wap2.Key, 0, 0, out _, out _);
                }
                else if (1000 <= signalID && signalID < 20000)
                {
                    int outputbeltID = wap2.Key;
                    var belt = fs.traffic.beltPool[outputbeltID];
                    int index = belt.segIndex + belt.segPivotOffset;
                    int beltnumber = (int)fs.factory.entitySignPool[fs.traffic.beltPool[outputbeltID].entityId].count0;
                    if (beltnumber == 999999)
                    {
                        __instance.TryInsertItem(outputbeltID, 0, signalID, 1, 0);
                    }
                    else if (beltnumber > 999900 && beltnumber % 10 != 0)
                    {
                        int t = beltnumber % 100;
                        int stack1 = t % 10;
                        int inc1 = ((t / 10 >= 3 ? 4 : t / 10)) * stack1;
                        __instance.TryInsertItem(outputbeltID, 0, signalID, (byte)stack1, (byte)inc1);
                    }
                    else if (beltnumber >= 11 && beltnumber <= 14)
                    {
                        bool breakfor = false;
                        int stackNum = beltnumber % 10;
                        CargoPath cargoPath = __instance.GetCargoPath(belt.segPathId);
                        cargoPath.GetCargoAtIndex(index, out Cargo cargo, out _, out _);
                        if (cargo.item > 0)
                        {
                            continue;
                        }
                        if (fs.factory.transport?.stationPool != null)
                        {
                            foreach (StationComponent sc in fs.factory.transport.stationPool)
                            {
                                if (breakfor) break;
                                if (sc == null || sc.storage == null)
                                {
                                    continue;
                                }
                                for (int i = 0; i < sc.storage.Length; i++)
                                {
                                    if (sc.storage[i].itemId != signalID)
                                    {
                                        continue;
                                    }
                                    if (sc.storage[i].count < stackNum)
                                        break;
                                    int inc1 = Math.Min(sc.storage[i].inc, 4 * stackNum);
                                    if (cargoPath.TryInsertItem(index, signalID, (byte)stackNum, (byte)inc1))
                                    {
                                        sc.storage[i].count -= stackNum;
                                        sc.storage[i].inc -= inc1;
                                    }
                                    breakfor = true;
                                }
                            }
                        }
                        if (breakfor) continue;
                        if (fs.factory.factoryStorage != null)
                        {
                            var storagePool = fs.factory.factoryStorage.storagePool;
                            if (storagePool != null)
                            {
                                foreach (StorageComponent sc in storagePool)
                                {
                                    if (breakfor) break;
                                    if (sc == null || sc.isEmpty || sc.GetItemCount(signalID) < stackNum)
                                    {
                                        continue;
                                    }
                                    sc.TakeItem(signalID, stackNum, out int inc1);
                                    if (!cargoPath.TryInsertItem(index, signalID, (byte)stackNum, (byte)inc1))
                                    {
                                        sc.AddItem(signalID, stackNum, inc1, out _);
                                    }
                                    breakfor = true;
                                    break;
                                }
                            }
                            if (breakfor) continue;
                            var tankPool = fs.factory.factoryStorage.tankPool;
                            if (tankPool != null)
                            {
                                foreach (TankComponent tc in tankPool)
                                {
                                    if (breakfor) break;
                                    if (tc.fluidId != signalID || tc.id <= 0 || tc.fluidCount < stackNum)
                                    {
                                        continue;
                                    }
                                    int inc1 = Math.Min(tc.fluidInc, stackNum * 4);
                                    if (cargoPath.TryInsertItem(index, signalID, (byte)stackNum, (byte)inc1))
                                    {
                                        fs.factory.factoryStorage.tankPool[tc.id].fluidInc -= inc1;
                                        fs.factory.factoryStorage.tankPool[tc.id].fluidCount -= stackNum;
                                    }
                                    breakfor = true;
                                    break;
                                }
                            }
                        }
                    }
                    else if (beltnumber >= 21 && beltnumber <= 24 && fs.minerPool != null)
                    {
                        int statckNum = beltnumber % 20;
                        var cargoPath = __instance.GetCargoPath(belt.segPathId);
                        cargoPath.GetCargoAtIndex(index, out Cargo cargo, out _, out _);
                        if (cargo.item > 0)
                        {
                            continue;
                        }
                        foreach (MinerComponent mc in fs.minerPool)
                        {
                            if (mc.id > 0 && mc.entityId > 0 && mc.productId == signalID && mc.productCount >= statckNum)
                            {
                                fs.minerPool[mc.id].productCount -= cargoPath.TryInsertItem(index, signalID, (byte)statckNum, 0) ? statckNum : 0;
                                break;
                            }
                        }
                    }

                }
                else if (signalID == 405)
                {
                    BeltComponent belt = fs.traffic.beltPool[wap2.Key];
                    CargoPath cargoPath = __instance.GetCargoPath(belt.segPathId);
                    byte stack;
                    byte inc;
                    int num1 = belt.segIndex + belt.segPivotOffset;
                    cargoPath.GetCargoAtIndex(num1, out Cargo cargo, out _, out _);
                    int itemid = cargo.item;
                    if (itemid < 1000) continue;
                    bool breakfor;
                    switch (fs.factory.entitySignPool[belt.entityId].count0)
                    {
                        case 1:
                            if (itemid != 1006 && itemid != 1007 && itemid != 1011 && itemid != 1109 && itemid != 1114 && itemid != 1120 && itemid != 1801 && itemid != 1802) continue;
                            var genPool = fs.factory.powerSystem?.genPool;
                            if (genPool == null) continue;
                            foreach (PowerGeneratorComponent pgc in genPool)
                            {
                                if (pgc.id <= 0 || pgc.fuelCount > 2)
                                {
                                    continue;
                                }
                                if (pgc.fuelMask == 1 && itemid != 1802)
                                {
                                    if (itemid == genPool[pgc.id].fuelId)
                                    {
                                        cargoPath.TryPickItem(num1 - 5, 12, out stack, out inc);
                                        genPool[pgc.id].SetNewFuel(itemid, (short)(genPool[pgc.id].fuelCount + stack), inc);
                                        break;
                                    }
                                    else if (pgc.fuelCount == 0)
                                    {
                                        cargoPath.TryPickItem(num1 - 5, 12, out stack, out inc);
                                        genPool[pgc.id].SetNewFuel(itemid, (short)(genPool[pgc.id].fuelCount + stack), inc);
                                        break;
                                    }
                                }
                                if (pgc.fuelMask == 2 && itemid == 1802)
                                {
                                    cargoPath.TryPickItem(num1 - 5, 12, out stack, out inc);
                                    genPool[pgc.id].SetNewFuel(1802, (short)(genPool[pgc.id].fuelCount + stack), inc);
                                    break;
                                }
                            }
                            break;
                        case 5:
                            if (fs.assemblerPool == null) continue;
                            breakfor = false;
                            foreach (AssemblerComponent ac in fs.assemblerPool)
                            {
                                if (breakfor) break;
                                if (ac.id <= 0 || ac.entityId <= 0 || ac.recipeId <= 0)
                                {
                                    continue;
                                }
                                for (int i = 0; i < ac.served.Length; i++)
                                {
                                    if (itemid != ac.requires[i]) continue;
                                    if (ac.served[i] < 0)
                                    {
                                        ac.served[i] = 0;
                                        continue;
                                    }
                                    if (ac.served[i] <= ac.requireCounts[i] * 2)
                                    {
                                        var itemId = cargoPath.TryPickItem(num1 - 5, 12, out stack, out inc);
                                        if (itemId == 0) return;
                                        ac.served[i] += stack;
                                        ac.incServed[i] += inc;
                                        breakfor = true;
                                        break;
                                    }
                                }
                            }
                            break;
                        case 6:
                            if (fs.factory.transport == null || fs.factory.transport.stationPool == null) continue;
                            breakfor = false;
                            foreach (StationComponent sc in fs.factory.transport.stationPool)
                            {
                                if (sc == null || sc.storage == null)
                                {
                                    continue;
                                }
                                for (int i = 0; i < sc.storage.Length; i++)
                                {
                                    if (sc.storage[i].itemId != itemid || sc.storage[i].count >= sc.storage[i].max) continue;
                                    cargoPath.TryPickItem(num1 - 5, 12, out stack, out inc);
                                    sc.AddItem(itemid, stack, inc);
                                    breakfor = true;
                                    break;
                                }
                                if (breakfor) break;
                            }
                            break;
                        case 8:
                            if (fs.labPool == null) continue;
                            breakfor = false;
                            foreach (LabComponent lc in fs.labPool)
                            {
                                if (breakfor) break;
                                if (lc.id <= 0 || lc.entityId <= 0)
                                {
                                    continue;
                                }
                                if (lc.researchMode)
                                {
                                    if (itemid < 6001 || itemid > 6006) continue;
                                    for (int i = 0; i < lc.matrixPoints.Length; i++)
                                    {
                                        if (itemid != 6001 + i) continue;
                                        if (lc.matrixServed[i] <= 36000)
                                        {
                                            int itemId = cargoPath.TryPickItem(num1 - 5, 12, out stack, out inc);
                                            if (itemId == 0) return;
                                            lc.matrixServed[i] += 3600 * stack;
                                            lc.matrixIncServed[i] += inc;
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
                                            int itemId = cargoPath.TryPickItem(num1 - 5, 12, out stack, out inc);
                                            if (itemId == 0) return;
                                            lc.served[i] += stack;
                                            lc.incServed[i] += inc;
                                            break;
                                        }
                                    }
                                }
                            }

                            if (itemid != 1503 && itemid != 1501 && itemid != 1209 && itemid != 1803 && itemid != 1804) continue;
                            if (itemid == 1209 || itemid == 1803 || itemid == 1804)
                            {
                                if (fs.factory.powerSystem == null || fs.factory.powerSystem.genPool == null) continue;
                                foreach (PowerGeneratorComponent pgc in fs.factory.powerSystem.genPool)
                                {
                                    if (pgc.id <= 0)
                                    {
                                        continue;
                                    }
                                    if (pgc.gamma)
                                    {
                                        if (itemid != 1209) continue;
                                        if (pgc.catalystPoint == 0)
                                        {
                                            int itemId = cargoPath.TryPickItem(num1 - 5, 12, out stack, out inc);
                                            if (itemId == 0) return;
                                            fs.factory.powerSystem.genPool[pgc.id].catalystPoint += 3600 * stack;
                                            fs.factory.powerSystem.genPool[pgc.id].catalystIncPoint += 3600 * inc;
                                            breakfor = true;
                                            break;
                                        }

                                    }
                                    else if (pgc.fuelMask == 4 && (itemid == 1803 || itemid == 1804) && pgc.fuelCount <= 2)
                                    {
                                        if (itemid != 1803 && itemid != 1804) continue;
                                        int itemId = cargoPath.TryPickItem(num1 - 5, 12, out stack, out inc);
                                        if (itemId == 0) return;
                                        fs.factory.powerSystem.genPool[pgc.id].SetNewFuel(itemid, (short)(fs.factory.powerSystem.genPool[pgc.id].fuelCount + stack), inc);
                                        breakfor = true;
                                        break;
                                    }
                                }
                            }
                            else if (itemid == 1503)
                            {
                                if (fs.siloPool == null) continue;
                                foreach (SiloComponent sc in fs.siloPool)
                                {
                                    if (sc.id <= 0 || sc.entityId <= 0 || sc.bulletCount > 1)
                                    {
                                        continue;
                                    }
                                    cargoPath.TryPickItem(num1 - 5, 12, out stack, out inc);
                                    fs.siloPool[sc.id].bulletCount += stack;
                                    fs.siloPool[sc.id].bulletInc += inc;
                                    break;
                                }
                            }
                            else if (itemid == 1501)
                            {
                                if (fs.ejectorPool == null) continue;
                                foreach (EjectorComponent ec in fs.ejectorPool)
                                {
                                    if (ec.id <= 0 || ec.entityId <= 0 || ec.bulletCount > 1)
                                    {
                                        continue;
                                    }
                                    cargoPath.TryPickItem(num1 - 5, 12, out stack, out inc);
                                    fs.ejectorPool[ec.id].bulletCount += stack;
                                    fs.ejectorPool[ec.id].bulletInc += inc;
                                    break;
                                }
                            }
                            break;
                        case 9:
                            break;
                    }
                }
                else if (signalID == 600 && Beltsignalnumberoutput.ContainsKey(factoryIndex))
                {
                    int index = (int)fs.factory.entitySignPool[fs.traffic.beltPool[wap2.Key].entityId].count0;
                    var outputbeltID = Beltsignalnumberoutput[factoryIndex].FirstOrDefault(x => x.Value == index).Key;
                    if (index <= 0 || outputbeltID <= 0)
                    {
                        continue;
                    }
                    int itemId = __instance.TryPickItem(wap2.Key, 0, 0, out byte stack, out byte inc);
                    if (itemId < 1000) continue;
                    if (!__instance.TryInsertItem(outputbeltID, 0, itemId, stack, inc))
                    {
                        __instance.TryInsertItem(wap2.Key, 0, itemId, stack, inc);
                    }
                }
            }
        }
    }
}
