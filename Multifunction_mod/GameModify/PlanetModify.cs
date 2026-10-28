using System;
using System.Collections.Generic;
using System.Linq;
using Multifunction_mod.Models;
using static Multifunction_mod.Multifunction;

namespace Multifunction_mod.GameModify
{
    /// <summary>
    /// 星球相关的游戏修改，目前是海洋类型：查看/修改/还原各星球的海洋。
    /// 相关状态放在这里，Multifunction 只负责在合适的时机调用。
    /// </summary>
    public static class PlanetModify
    {
        /// <summary>面板上当前显示的星球海洋类型</summary>
        public static string CurrentPlanetWaterType = "";

        private static SeedPlanetWater originWaterTypes;
        private static SeedPlanetWater currentWaterTypes;
        private static Dictionary<long, SeedPlanetWater> seedPlanetWaterTypes;

        /// <summary>
        /// 初始化海洋类型容器（插件启动时调用一次）
        /// </summary>
        public static void Init()
        {
            originWaterTypes = new SeedPlanetWater();
            seedPlanetWaterTypes = new Dictionary<long, SeedPlanetWater>();
        }

        /// <summary>
        /// 退出存档时重置原始海洋记录
        /// </summary>
        public static void ResetWaterTypes()
        {
            originWaterTypes = new SeedPlanetWater();
        }

        /// <summary>
        /// 从配置导入海洋类型数据
        /// </summary>
        public static void InitWaterTypesConfig()
        {
            string[] seedPlanetWaters = seedPlanetWater.Value.Split('|');
            foreach (var seedPlanetWaterTypestr in seedPlanetWaters)
            {
                if (string.IsNullOrEmpty(seedPlanetWaterTypestr)) continue;
                var seedPlanetWater = new SeedPlanetWater();
                string[] values = seedPlanetWaterTypestr.Split(',');
                if (values[0] == "0") continue;
                if (values[1].Length > 0)
                {
                    seedPlanetWater.seedKey64 = Convert.ToInt64(values[0]);
                    string[] planetwatertypes = values[1].Split('-');
                    foreach (var planetwatertypestr in planetwatertypes)
                    {
                        if (string.IsNullOrEmpty(planetwatertypestr)) continue;
                        string[] watervalues = planetwatertypestr.Split(':');
                        seedPlanetWater.waterTypes.Add(Convert.ToInt32(watervalues[0]), Convert.ToInt32(watervalues[1]));
                    }
                }
                if (seedPlanetWater.seedKey64 != 0)
                {
                    seedPlanetWaterTypes.Add(seedPlanetWater.seedKey64, seedPlanetWater);
                }
            }
        }

        /// <summary>
        /// 初始化存档海洋类型：记录原始海洋，并恢复本存档之前改过的海洋
        /// </summary>
        public static void InitPlanetWaterType()
        {
            if (GameMain.galaxy?.stars == null || GameMain.data?.gameDesc == null)
            {
                return;
            }
            long seed = GameMain.data.gameDesc.seedKey64;
            originWaterTypes.seedKey64 = seed;
            foreach (var star in GameMain.galaxy.stars)
            {
                if (star == null) continue;
                foreach (var planet in star.planets)
                {
                    if (planet == null || planet.gasTotalHeat > 0) continue;
                    originWaterTypes.waterTypes.Add(planet.id, planet.waterItemId);
                }
            }
            if (seedPlanetWaterTypes.ContainsKey(seed))
            {
                currentWaterTypes = seedPlanetWaterTypes[seed];
                foreach (var watertypes in currentWaterTypes.waterTypes)
                {
                    var pd = GameMain.galaxy.PlanetById(watertypes.Key);
                    if (pd != null)
                    {
                        pd.waterItemId = watertypes.Value;
                    }
                }
            }
            else
            {
                currentWaterTypes = new SeedPlanetWater(seed);
                seedPlanetWaterTypes.Add(seed, currentWaterTypes);
            }
        }

        /// <summary>
        /// 刷新面板上显示的当前星球海洋类型
        /// </summary>
        public static void RefreshCurrentPlanetWaterType()
        {
            if (GameMain.localPlanet == null)
            {
                CurrentPlanetWaterType = "";
                return;
            }
            if (string.IsNullOrEmpty(CurrentPlanetWaterType))
            {
                CurrentPlanetWaterType = LDB.items.Select(GameMain.localPlanet.waterItemId)?.name ?? "无海洋".getTranslate();
            }
        }

        /// <summary>
        /// 设置当前星球的海洋类型
        /// </summary>
        public static void SetWaterType()
        {
            if (GameMain.localPlanet == null) return;
            var item = LDB.items.dataArray.ToList().Find(x => x.name == CurrentPlanetWaterType);
            if (item == null)
            {
                UIMessageBox.Show("", "设置海洋类型失败".getTranslate(), "确定".Translate(), 3);
                return;
            }
            if (GameMain.localPlanet.waterItemId == item.ID) return;
            GameMain.localPlanet.waterItemId = item.ID;
            int planetId = GameMain.localPlanet.id;
            int originwaterId = originWaterTypes.waterTypes[planetId];
            if (originwaterId == item.ID && currentWaterTypes.waterTypes.ContainsKey(planetId))
            {
                currentWaterTypes.waterTypes.Remove(planetId);
            }
            else if (originwaterId != item.ID)
            {
                if (!currentWaterTypes.waterTypes.ContainsKey(planetId))
                    currentWaterTypes.waterTypes.Add(planetId, item.ID);
                else
                    currentWaterTypes.waterTypes[planetId] = item.ID;
            }

            SaveWaterTypesConfig();
        }

        /// <summary>
        /// 还原所有被修改过的海洋类型
        /// </summary>
        public static void RestoreWaterType()
        {
            foreach (var keyvalue in currentWaterTypes.waterTypes)
            {
                if (originWaterTypes.waterTypes.ContainsKey(keyvalue.Key))
                {
                    GameMain.galaxy.PlanetById(keyvalue.Key).waterItemId = originWaterTypes.waterTypes[keyvalue.Key];
                    if (GameMain.localPlanet.id == keyvalue.Key)
                    {
                        CurrentPlanetWaterType = LDB.items.Select(originWaterTypes.waterTypes[keyvalue.Key])?.name ?? "无海洋".getTranslate();
                    }
                }
            }
            currentWaterTypes.waterTypes.Clear();
            SaveWaterTypesConfig();
        }

        /// <summary>
        /// 把当前的海洋修改写回配置
        /// </summary>
        private static void SaveWaterTypesConfig()
        {
            string result = "";
            foreach (var seedPlanetWaterType in seedPlanetWaterTypes)
            {
                if (seedPlanetWaterType.Value.seedKey64 == 0 || seedPlanetWaterType.Value.waterTypes.Count == 0) continue;
                result += seedPlanetWaterType.Value.ToStr() + '|';
            }
            seedPlanetWater.Value = result;
        }
    }
}
