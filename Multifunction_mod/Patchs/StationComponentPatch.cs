using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Reflection;
using UnityEngine;
using static Multifunction_mod.Multifunction;

namespace Multifunction_mod.Patchs
{
    public class StationComponentPatch
    {
        private static HashSet<int> tempids=new HashSet<int>();
        [HarmonyPrefix]
        [HarmonyPatch(typeof(StationComponent), "Init")]
        public static void StationComponentInit(ref int _extraStorage, StationComponent __instance)
        {
            if (StationStoExtra.Value <= 0)
            {
                return;
            }
            int basemax = __instance.isStellar ? 10000 : 5000;
            _extraStorage = (StationStoExtra.Value + 1) * basemax;
        }

        /// <summary>
        /// 获取目标星球目标矿脉数量
        /// </summary>
        /// <param name="itemid" ></param>
        /// <param name="pdid"></param>
        /// <returns></returns>
        public static int GetNumberOfVein(int itemid, PlanetData pd)
        {
            EVeinType evt = LDB.veins.GetVeinTypeByItemId(itemid);
            if (evt == EVeinType.None)
            {
                return 0;
            }
            if (evt == EVeinType.Oil)
            {
                long[] veinAmounts = new long[64];
                pd.SummarizeVeinAmountsByFilter(ref veinAmounts, tempids, UIRoot.instance.uiGame.veinAmountDisplayFilter);
                int collectspeed = (int)(veinAmounts[7] * VeinData.oilSpeedMultiplier + 0.5);
                if (collectspeed > 1) return collectspeed;
            }
            return pd.factory.veinPool.Count(x => x.type == evt);
        }

        /// <summary>
        /// 物流站采矿
        /// </summary>
        /// <param name="itemid"></param>
        /// <param name="minenumber"></param>
        /// <param name="pdid"></param>
        /// <returns></returns>
        public static int MineVein(int itemid, int minenumber, PlanetData pd)
        {
            if (pd.waterItemId == itemid)
            {
                return (int)(30 * GameMain.history.miningSpeedScale * Stationminenumber.Value);
            }
            if (minenumber <= 0) return 0;
            int getmine = 0;
            int maxmineNumber = itemid != 1007 ? (int)(minenumber * GameMain.history.miningSpeedScale / 2) : (int)(minenumber * GameMain.history.miningSpeedScale);
            int neednumber = itemid != 1007 ? (int)(maxmineNumber * GameMain.history.miningCostRate / 2) : (int)(maxmineNumber * GameMain.history.miningCostRate);
            if (GameMain.data.gameDesc.isInfiniteResource)
                return maxmineNumber;
            if (LDB.veins.GetVeinTypeByItemId(itemid) == EVeinType.None || pd == null)
            {
                return 0;
            }
            if (neednumber == 0)
            {
                return maxmineNumber;
            }
            foreach (VeinData i in pd.factory.veinPool)
            {
                if (i.type != LDB.veins.GetVeinTypeByItemId(itemid))
                    continue;
                if (i.amount == 1)
                {
                    continue;
                }
                if (i.amount > neednumber - getmine)
                {
                    if (itemid == 1007 && i.amount * VeinData.oilSpeedMultiplier <= 0.1)
                    {
                        int dis = veinproperty.oillowerlimit - i.amount;
                        pd.factory.veinPool[i.id].amount += dis;
                        pd.factory.veinGroups[i.groupIndex].amount += dis;
                        getmine += (int)(0.1 * GameMain.history.miningSpeedScale * Stationminenumber.Value);
                    }
                    else
                    {
                        pd.factory.veinPool[i.id].amount -= neednumber;
                        pd.factory.veinGroups[i.groupIndex].amount -= neednumber;
                        getmine = maxmineNumber;
                    }
                }
                else
                {
                    getmine += i.amount - 1;
                    pd.factory.veinPool[i.id].amount = 1;
                    pd.factory.veinGroups[i.groupIndex].amount -= i.amount - 1;
                }
                if (getmine >= maxmineNumber) break;
            }
            return getmine;
        }

        /// <summary>
        /// 物流站星球采矿
        /// </summary>
        /// <param name="sc"></param>
        /// <param name="pd"></param>
        public static void StationMine(StationComponent sc, PlanetData pd, int[] productRegister)
        {
            for (int i = 0; i < sc.storage.Length && sc.energy > 0; i++)
            {
                ref StationStore store = ref sc.storage[i];
                int itemID = store.itemId;
                if (itemID <= 0 || store.count >= store.max)
                    continue;
                int veinNumbers = GetNumberOfVein(itemID, pd);
                int pointminenum = veinNumbers * Stationminenumber.Value;
                if (veinNumbers > 0 && pointminenum == 0) pointminenum = 1;
                else if (veinNumbers == 0 && itemID != pd.waterItemId) continue;

                if (!Stationfullenergy.Value && sc.energy <= pointminenum * GameMain.history.miningSpeedScale * 5000)
                {
                    pointminenum = (int)(sc.energy*0.9f / (GameMain.history.miningSpeedScale * 5000));
                }
                int minenum = MineVein(itemID, pointminenum, pd);
                if (minenum <= 0)
                {
                    continue;
                }
                lock (productRegister)
                {
                    productRegister[itemID] += minenum;
                    sc.storage[i].count += minenum;
                    if (!Stationfullenergy.Value)
                        sc.energy -= minenum * 5000;
                }
            }
        }

        /// <summary>
        /// 物流站内置发电
        /// </summary>
        /// <param name="sc"></param>
        /// <param name="planetID"></param>
        public static void StationPowerGeneration(StationComponent sc, int[] consumeRegister)
        {
            if (sc == null || sc.storage == null || sc.energy >= sc.energyMax - 1000000) return;
            ref StationStore store = ref sc.storage[sc.storage.Length - 1];
            if (store.itemId <= 0 || store.count <= 0)
                return;
            ItemProto ip = LDB.items.Select(store.itemId);
            if (ip.HeatValue > 0 && sc.energy + ip.HeatValue < sc.energyMax)
            {
                int num = Math.Min(store.count, (int)((sc.energyMax - sc.energy) / ip.HeatValue));
                sc.energy += num * ip.HeatValue;
                store.count -= num;
                if (num > 0)
                {
                    lock (consumeRegister)
                    {
                        consumeRegister[ip.ID] = num;
                    }
                }
            }
        }

        /// <summary>
        /// 物流站内置喷涂
        /// </summary>
        /// <param name="sc"></param>
        public static void StationSprayInc(StationComponent sc, int[] consumeRegister)
        {
            int incIndex = -1;
            for (int i = 0; i < sc.storage.Length; i++)
            {
                if (sc.storage[i].itemId == 1143)
                {
                    incIndex = i;
                    if (sc.storage[i].count == 0) return;
                    break;
                }
            }
            if (incIndex == -1)
                return;
            for (int i = 0; i < sc.storage.Length; i++)
            {
                ref StationStore store = ref sc.storage[i];
                ref StationStore incstore = ref sc.storage[incIndex];
                if (store.itemId == 1143 || store.itemId <= 0 || store.count <= 0) continue;

                int needinc = store.count * incAbility - store.inc;
                int needNumber = Math.Min((int)Math.Ceiling(needinc / 296.0), incstore.count);
                if (needNumber == 0) continue;
                lock (consumeRegister)
                {
                    consumeRegister[1143] = needNumber;
                    store.inc += (incstore.count >= needNumber ? needNumber : incstore.count) * 296;
                    incstore.count -= needNumber;
                }
                if (store.count == 0)
                {
                    store.inc = 0;
                }
            }
        }

        /// <summary>
        /// 物流站满货物
        /// </summary>
        /// <param name="sc"></param>
        public static void StationFullItemCount(StationComponent sc)
        {
            for (int i = 0; i < sc.storage.Length; i++)
            {
                if (sc.storage[i].itemId <= 0)
                    continue;
                sc.storage[i].count = sc.storage[i].max;
            }
        }

        /// <summary>
        /// 物流站垃圾站
        /// </summary>
        /// <param name="sc"></param>
        public static void StationTrashMethod(StationComponent sc, int[] consumeRegister)
        {
            long addSand = 0;
            for (int i = 0; i < sc.storage.Length && sc.energy > 0; i++)
            {
                ref StationStore store = ref sc.storage[i];
                int itemID = store.itemId;
                if (itemID <= 0) continue;
                int trashnum = store.count;
                if (sc.energy > trashnum * 10000)
                {
                    lock (consumeRegister)
                    {
                        consumeRegister[itemID] = trashnum;
                        sc.storage[i].count -= trashnum;
                        if (!Stationfullenergy.Value)
                            sc.energy -= trashnum * 10000;
                    }
                    if (needtrashsand.Value)
                    {
                        addSand += trashnum * 100;
                    }
                }
            }
            if (addSand > 0)
            {
                player.sandCountChanged -= UIRoot.instance.uiGame.OnSandCountChanged;
                player.SetSandCount(player.sandCount + addSand);
                player.sandCountChanged += UIRoot.instance.uiGame.OnSandCountChanged;
            }
        }

        /// <summary>
        /// 星球熔炉矿机
        /// </summary>
        /// <param name="sc"></param>
        /// <param name="pdId"></param>
        public static void StationFurnaceMiner(StationComponent sc, long time, int[] consumeRegister, int[] productRegister)
        {
            List<int> storageItems = sc.storage.Select(x => x.itemId).ToList();
            storageItems.ForEach(itemId =>
            {
                if (itemId <= 0 || !smeltRecipes.ContainsKey(itemId)) return;
                smeltRecipes[itemId].ForEach(rp =>
                {
                    int spendtime = rp.TimeSpend / 2;
                    if (spendtime < 2) { }
                    else if (time % spendtime != 0) return;
                    if (!rp.Results.All(storageItems.Contains) || !rp.Items.All(storageItems.Contains))
                        return;
                    for (int i = 0; i < rp.ResultCounts.Length; i++)
                    {
                        int index = storageItems.IndexOf(rp.Results[i]);
                        if (sc.storage[index].count > sc.storage[index].max)
                        {
                            return;
                        }
                    }
                    int smelters = StationMinerSmelterNum.Value;
                    int costenergypertime = 1440000;
                    int doublecostenergypertime = 2880000;
                    int smeltTime = smelters;
                    int incsmeltTime = smeltTime;
                    int len = rp.Items.Length;
                    for (int i = 0; i < len; i++)
                    {
                        if (rp.ItemCounts[i] == 0) continue;
                        int index = storageItems.IndexOf(rp.Items[i]);
                        int tempcount = sc.storage[index].count / rp.ItemCounts[i];
                        smeltTime = Math.Min(smeltTime, tempcount);
                        incsmeltTime = Math.Min(Math.Min(incsmeltTime, tempcount), sc.storage[index].inc / incAbility);
                    }
                    if (Stationfullenergy.Value)
                    {
                        smeltTime = smeltTime - incsmeltTime;
                    }
                    else
                    {
                        incsmeltTime = (int)Math.Min(incsmeltTime, sc.energy / doublecostenergypertime);
                        sc.energy -= incsmeltTime * doublecostenergypertime;
                        smeltTime = (int)Math.Min(smeltTime - incsmeltTime, sc.energy / costenergypertime);
                    }
                    if (smeltTime + incsmeltTime == 0) return;
                    if (!Stationfullenergy.Value)
                    {
                        sc.energy -= smeltTime * costenergypertime;
                    }
                    for (int i = 0; i < len; i++)
                    {
                        int consumeCount = rp.ItemCounts[i] * (smeltTime + incsmeltTime);
                        lock (consumeRegister)
                        {
                            int index = storageItems.IndexOf(rp.Items[i]);
                            sc.storage[index].count -= consumeCount;
                            sc.storage[index].inc -= incsmeltTime * rp.ItemCounts[i] * incAbility;
                            sc.storage[index].inc = Math.Max(0, Math.Min(sc.storage[index].count * incAbility, sc.storage[index].inc));
                            consumeRegister[rp.Items[i]] = consumeCount;
                        }
                    }
                    for (int i = 0; i < rp.ResultCounts.Length; i++)
                    {
                        lock (productRegister)
                        {
                            int addcount = (int)(rp.ResultCounts[i] * (smeltTime + incsmeltTime * (1 + Cargo.incTableMilli[incAbility])));
                            int index = storageItems.IndexOf(rp.Results[i]);
                            sc.storage[index].count += addcount;
                            productRegister[rp.Results[i]] = addcount;
                        }
                    }
                });
            });
        }

    }
}
