using System.Collections.Generic;
using UnityEngine;
using Multifunction_mod.Patchs.Build;
using static Multifunction_mod.Constant;
using static Multifunction_mod.Multifunction;

namespace Multifunction_mod.GameModify
{
    /// <summary>
    /// 建筑相关的游戏修改：蓝图建筑免材料、清除全部建筑、建筑抬升、超密铺采集器、传送带信号、量子站初始化。
    /// </summary>
    public static class BuildModify
    {
        /// <summary>
        /// 蓝图预建造的建筑不再需要材料（蓝图建造无需材料）
        /// </summary>
        public static void BlueprintNoMaterial(bool enabled)
        {
            if (!enabled)
            {
                return;
            }

            var factory = GameMain.localPlanet?.factory;
            if (factory == null)
            {
                return;
            }

            for (int i = 1; i < factory.prebuildCursor; i++)
            {
                if (factory.prebuildPool[i].protoId > 0)
                {
                    factory.prebuildPool[i].itemRequired = 0;
                }
            }
        }

        /// <summary>
        /// 清除当前星球全部建筑
        /// </summary>
        /// <param name="player">玩家</param>
        /// <param name="type">0 正常清除（进背包），1 不掉落清除，2 初始化带还原地形，3 初始化不还原地形</param>
        public static void RemoveAllBuild(Player player, int type)
        {
            if (player == null)
            {
                return;
            }

            if (GameMain.localPlanet != null && GameMain.localPlanet.factory != null)
            {
                bool noItem = type == 1;
                if (noItem)
                {
                    DismantleNoItemPatch.Enable = true;
                }
                try
                {
                    foreach (EntityData etd in GameMain.localPlanet.factory.entityPool)
                    {
                        int stationId = etd.stationId;
                        if (stationId > 0)
                        {
                            StationComponent sc = GameMain.localPlanet.factory.transport.stationPool[stationId];
                            for (int i = 0; i < sc.storage.Length && type == 0; i++)
                            {
                                int package = player.TryAddItemToPackage(sc.storage[i].itemId, sc.storage[i].count, 0, true, etd.id);
                                UIItemup.Up(sc.storage[i].itemId, package);
                            }
                            sc.storage = new StationStore[sc.storage.Length];
                            sc.needs = new int[sc.needs.Length];
                        }
                        if (type == 1)
                        {
                            GameMain.localPlanet.factory.RemoveEntityWithComponents(etd.id, false);
                        }
                        else
                        {
                            player.controller.actionBuild.DoDismantleObject(etd.id);
                        }
                    }
                }
                finally
                {
                    if (noItem)
                    {
                        DismantleNoItemPatch.Enable = false;
                    }
                }
            }
        }

        /// <summary>
        /// 控制建筑的高度（建筑抬升，上下方向键调整层数）
        /// </summary>
        public static void DriftBuild(Player player)
        {
            if (!DriftBuildings)
                return;
            var actionBuild = player?.controller?.actionBuild;
            if (actionBuild != null && player.controller.cmd.type == ECommand.Build)
            {
                if (blueprintPasteToolActive != actionBuild.blueprintPasteTool.active)
                {
                    blueprintPasteToolActive = !blueprintPasteToolActive;
                    DriftBuildingLevel = 0;
                }
                if (Input.GetKeyDown(KeyCode.UpArrow))
                {
                    DriftBuildingLevel++;
                    if (blueprintPasteToolActive)
                    {
                        actionBuild.blueprintPasteTool.cursorValid = true;
                        actionBuild.blueprintPasteTool.DeterminePreviewsPrestage();
                    }
                }
                if (Input.GetKeyDown(KeyCode.DownArrow))
                {
                    DriftBuildingLevel--;
                    if (blueprintPasteToolActive)
                    {
                        actionBuild.blueprintPasteTool.cursorValid = true;
                        actionBuild.blueprintPasteTool.DeterminePreviewsPrestage();
                    }
                }
            }
            else if (DriftBuildingLevel != 0)
            {
                DriftBuildingLevel = 0;
            }
        }

        /// <summary>
        /// 超密铺采集器
        /// </summary>
        public static void SetMaxGasStation(Player player)
        {
            if (player == null)
            {
                return;
            }

            if (GameMain.localPlanet == null || GameMain.localPlanet.type != EPlanetType.Gas)
                return;
            for (int i = 0; i < nodepos.Count; i++)
            {
                Vector3 pos;
                pos = nodepos[i] * GameMain.localPlanet.realRadius;
                Vector3 vector3_3 = 0.025f * pos.normalized * player.planetData.realRadius;
                Quaternion quaternion3 = Maths.SphericalRotation(pos, player.controller.actionBuild.clickTool.yaw);
                pos = pos + vector3_3;
                PrebuildData prebuild = new PrebuildData();
                prebuild.protoId = 2105;
                prebuild.modelIndex = 117;
                prebuild.pos = pos + quaternion3 * Vector3.zero;
                prebuild.pos2 = pos + quaternion3 * Vector3.zero;
                prebuild.rot = quaternion3 * Quaternion.identity;
                prebuild.rot2 = quaternion3 * Quaternion.identity;
                prebuild.pickOffset = 0;
                prebuild.insertOffset = 0;
                prebuild.recipeId = 0;
                prebuild.filterId = 0;
                prebuild.paramCount = 0;
                prebuild.InitParametersArray(0);
                player.controller.actionBuild.clickTool.factory.AddPrebuildDataWithComponents(prebuild);
            }
        }

        /// <summary>
        /// 初始化信号传送带功能：扫描全部星球上的信号标记，建立传送带信号索引
        /// </summary>
        public static void InitBeltSignalDiction()
        {
            if (GameMain.galaxy == null || GameMain.galaxy.stars == null) return;
            foreach (StarData sd in GameMain.galaxy.stars)
            {
                foreach (PlanetData pd in sd.planets)
                {
                    if (pd == null || pd.factory == null || pd.factory.entitySignPool == null) continue;
                    foreach (EntityData ed in pd.factory.entityPool)
                    {
                        if (ed.id <= 0 || ed.beltId == 0 || pd.factory.entitySignPool[ed.id].iconId0 == 0) continue;
                        int signalId = (int)pd.factory.entitySignPool[ed.id].iconId0;
                        int beltid = ed.beltId;
                        if (!Beltsignal.ContainsKey(pd.factoryIndex))
                            Beltsignal.Add(pd.factoryIndex, new Dictionary<int, int>());
                        if (!Beltsignal[pd.factoryIndex].ContainsKey(beltid))
                            Beltsignal[pd.factoryIndex].Add(beltid, signalId);
                        else
                            Beltsignal[pd.factoryIndex][beltid] = signalId;
                        if (!Beltsignalnumberoutput.ContainsKey(pd.factoryIndex))
                            Beltsignalnumberoutput.Add(pd.factoryIndex, new Dictionary<int, int>());
                        if (!Beltsignalnumberoutput[pd.factoryIndex].ContainsKey(beltid) && pd.factory.entitySignPool[ed.id].iconId0 == 601 && pd.factory.entitySignPool[ed.id].count0 > 0)
                            Beltsignalnumberoutput[pd.factoryIndex].Add(beltid, (int)pd.factory.entitySignPool[ed.id].count0);
                    }
                }
            }
        }

        /// <summary>
        /// 量子站初始化：收集当前星系里的采集站
        /// </summary>
        public static void InitQuantumTransport()
        {
            if (GameMain.data == null || GameMain.data.galacticTransport == null || GameMain.data.galacticTransport.stationPool == null)
            {
                return;
            }

            foreach (StationComponent sc in GameMain.data.galacticTransport.stationPool)
            {
                if (sc == null || !sc.isCollector) continue;
                CollectorStation.Add(sc.gid);
            }
        }
    }
}
