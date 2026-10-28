using System.Collections.Generic;

namespace Multifunction_mod.GameModify
{
    /// <summary>
    /// 科技/研究相关的游戏修改：解锁全部科技、回退无穷科技、研究秒完成。
    /// </summary>
    public static class TechnologyModify
    {
        /// <summary>研究秒完成时补充的科技哈希</summary>
        public const long QuickResearchHash = 600_000_000_000_000L;

        /// <summary>
        /// 解锁全部科技（可以反复升级的无穷科技不处理）
        /// </summary>
        /// <param name="player">玩家，为空时直接返回</param>
        /// <param name="addStat">是否把消耗补进统计面板</param>
        public static void UnlockAllTech(Player player, bool addStat = false)
        {
            if (player == null)
            {
                return;
            }

            bool end = true;
            while (end)
            {
                end = false;
                foreach (TechProto techProto in new List<TechProto>(LDB.techs.dataArray))
                {
                    if (GameMain.data.history.TechUnlocked(techProto.ID) || techProto.IsObsolete) continue;
                    if (!GameMain.data.history.CanEnqueueTech(techProto.ID)) continue;
                    if (techProto.MaxLevel > 15) continue;
                    end = true;
                    if (techProto.Level < techProto.MaxLevel)
                    {
                        for (int level = techProto.Level; level < techProto.MaxLevel; ++level)
                        {
                            if (addStat)
                            {
                                for (int i = 0; i < techProto.itemArray.Length; i++)
                                {
                                    AddConsumeItemToTotal(techProto.Items[i], (int)(techProto.ItemPoints[i] * techProto.GetHashNeeded(techProto.Level) / 3600));
                                }
                            }
                            for (int index = 0; index < techProto.UnlockFunctions.Length; ++index)
                            {
                                GameMain.history.UnlockTechFunction(techProto.UnlockFunctions[index], techProto.UnlockValues[index], level);
                            }
                        }
                    }
                    if (!GameMain.history.techStates[techProto.ID].unlocked)
                    {
                        if (addStat)
                        {
                            for (int i = 0; i < techProto.itemArray.Length; i++)
                            {
                                AddConsumeItemToTotal(techProto.Items[i], (int)(techProto.ItemPoints[i] * techProto.GetHashNeeded(techProto.Level) / 3600));
                            }
                        }
                        GameMain.history.UnlockTech(techProto.ID);
                    }
                }
            }
        }

        /// <summary>
        /// 回退无穷科技
        /// </summary>
        public static void LockInfiniteTech()
        {
            TechProto[] dataArray = LDB.techs.dataArray;
            for (int index = 0; index < dataArray.Length; index++)
            {
                TechProto tp = dataArray[index];
                if (tp.MaxLevel > 30 && tp.Level != tp.MaxLevel)
                {
                    TechState techState = GameMain.history.techStates[tp.ID];
                    techState.unlocked = false;
                    techState.curLevel = tp.Level;
                    techState.hashUploaded = 0L;
                    techState.hashNeeded = tp.GetHashNeeded(techState.curLevel);
                    GameMain.history.techStates[tp.ID] = techState;
                }
            }
        }

        /// <summary>
        /// 研究秒完成：给科技哈希池塞入足量哈希
        /// </summary>
        public static void AddResearchHash()
        {
            if (GameMain.history == null)
            {
                return;
            }

            GameMain.history.AddTechHash(QuickResearchHash);
        }

        /// <summary>
        /// 把解锁科技消耗的物品补进统计面板的产量/消耗统计
        /// </summary>
        public static void AddConsumeItemToTotal(int itemid, int count)
        {
            List<int> readytocompute = new List<int>();
            List<long> countList = new List<long>();
            readytocompute.Add(itemid);
            countList.Add(count);
            int index = 0;
            while (index < readytocompute.Count)
            {
                ItemProto item = LDB.items.Select(readytocompute[index]);
                if (item.recipes == null || item.recipes.Count == 0 || readytocompute[index] == 1003)
                {
                    index++;
                    continue;
                }
                for (int i = 0; i < item.recipes[0].Items.Length; i++)
                {
                    readytocompute.Add(item.recipes[0].Items[i]);
                    countList.Add(item.recipes[0].ItemCounts[i] * countList[index]);
                }
                index++;
            }
            for (int i = readytocompute.Count - 1; i >= 0; i--)
            {
                try
                {
                    if (itemid == 1503 && readytocompute[i] == 1501) countList[i] *= 12;
                    if (itemid == 1503 && readytocompute[i] == 1404) countList[i] *= 12;
                    while (countList[i] > 0)
                    {
                        int num = countList[i] > int.MaxValue ? int.MaxValue : (int)countList[i];
                        GameMain.data.statistics.production.factoryStatPool[GameMain.localPlanet.factory.index].AddProductionToTotalArray(readytocompute[i], num);
                        GameMain.data.statistics.production.factoryStatPool[GameMain.localPlanet.factory.index].AddConsumptionToTotalArray(readytocompute[i], num);
                        countList[i] -= num;
                    }
                }
                catch
                {
                }
            }
        }
    }
}
