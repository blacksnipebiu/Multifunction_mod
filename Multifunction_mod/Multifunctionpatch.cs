using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Multifunction_mod.Multifunction;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

namespace Multifunction_mod
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
        public static Harmony harmony;
        public static int ability = 4;

        public static void Patchallmethod()
        {
            harmony = new Harmony(GUID);
            var m = typeof(StorageComponent).GetMethods();
            foreach (var i in m)
            {
                if (i.Name == "TakeTailItems" && i.ReturnType == typeof(void))
                {
                    var prefix = typeof(Multifunctionpatch).GetMethod("TakeTailItemsPatch");
                    harmony.Patch(i, new HarmonyMethod(prefix));
                }
            }

            harmony.PatchAll(typeof(DysonPatch));
            harmony.PatchAll(typeof(PlayerPatch));
            harmony.PatchAll(typeof(PowerSystemPatch));
            harmony.PatchAll(typeof(PlanetFactoryPatch));
            harmony.PatchAll(typeof(StorageComponentPatch));
            harmony.PatchAll(typeof(StationComponentPatch));
            harmony.PatchAll(typeof(FactorySystemPatch));
            harmony.PatchAll(typeof(CargoTrafficPatch));
            harmony.PatchAll(typeof(SomePatch));
            harmony.PatchAll(typeof(UIPatch));
            harmony.PatchAll(typeof(PlanetTransportPatch));
            harmony.PatchAll(typeof(TankComponentPatch));
            harmony.PatchAll(typeof(SpraycoaterComponentPatch));
            harmony.PatchAll(typeof(CombatPatch));
        }

        public static bool TakeTailItemsPatch(StorageComponent __instance, ref int itemId)
        {
            if (ArchitectMode.Value)
            {
                if (itemId <= 0 || itemId >= 6007 || __instance == null || __instance.id != GameMain.mainPlayer.package.id) return true;
                if (LDB.items.Select(itemId).CanBuild) return false;
            }
            return true;
        }

        public class DysonPatch
        {
            [HarmonyPrefix]
            [HarmonyPatch(typeof(DysonSwarm), "AbsorbSail")]
            public static bool DysonSwarmPatch1(ref DysonSwarm __instance, ref bool __result, DysonNode node)
            {
                if (!quickabsorbsolar.Value)
                {
                    return true;
                }
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
                if (__instance.expiryOrder[num].time == 0)
                {
                    Assert.CannotBeReached();
                    __result = false;
                    return false;
                }
                __instance.expiryOrder[num].time = 0;
                __instance.expiryOrder[num].index = 0;
                if (node != null && node.ConstructCp() != null)
                {
                    __instance.dysonSphere.productRegister[11903]++;
                }
                __instance.RemoveSolarSail(index);
                return false;
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
                if (!QuickabortSwarm.Value)
                    return;
                int num = (int)(gameTick % 120L);
                for (int i = 1; i < __instance.nodeCursor; i++)
                {
                    DysonNode dysonNode = __instance.nodePool[i];
                    if (dysonNode?.id == i && dysonNode.id % 120 == num && dysonNode.sp == dysonNode.spMax)
                    {
                        for (int j = 1; j <= Solarsailsabsorbeveryframe.Value; j++)
                        {
                            dysonNode.OrderConstructCp(gameTick, __instance.dysonSphere.swarm);
                        }
                    }
                }
            }
        }

        public class PlayerPatch
        {
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
                if (QuickPlayerMine.Value && __instance.miningId > 0 && GameMain.localPlanet != null)
                {
                    var factory = __instance.player.factory;
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

            [HarmonyPostfix]
            [HarmonyPatch(typeof(PlayerAction_Inspect), "GetObjectSelectDistance")]
            public static void PlayerAction_InspectPatch(ref float __result)
            {
                if (InspectDisNoLimit.Value)
                {
                    __result = 400;
                }
            }

            [HarmonyPostfix]
            [HarmonyPatch(typeof(BuildTool_Click), "_OnInit")]
            public static void DeterminePreviewsPatch(BuildTool_Click __instance)
            {
                __instance.dotsSnapped = new Vector3[Buildmaxlen.Value];
            }

            [HarmonyPrefix]
            [HarmonyPatch(typeof(BuildTool_BlueprintPaste), "CheckBuildConditions")]
            public static bool Prefix(BuildTool_BlueprintPaste __instance, ref bool __result)
            {
                if (pasteanyway && __instance.bpPool != null)
                {
                    for (int i = 0; i < __instance.bpPool.Length; i++)
                    {
                        var prefab = __instance.bpPool[i]?.item?.prefabDesc;
                        if (prefab != null && prefab.veinMiner)
                        {
                            return true;
                        }
                    }
                    __result = true;
                    return false;
                }
                return true;
            }

            [HarmonyPrefix]
            [HarmonyPatch(typeof(Player), "TryAddItemToPackage")]
            public static bool PlayerTryAddItemToPackage(int itemId)
            {
                if (!dismantle_but_nobuild.Value || itemId == 1099)
                {
                    return true;
                }
                return false;
            }

            [HarmonyPrefix]
            [HarmonyPatch(typeof(Player), "SendItemToPlayer")]
            public static bool PlayerSendItemToPlayer(ref int itemId, ref int itemCount, ref int itemInc, bool toPackage, ItemBundle sentList)
            {
                if (!dismantle_but_nobuild.Value || itemId == 1099)
                {
                    return true;
                }

                if (itemId > 0 && itemCount > 0 && toPackage)
                {
                    sentList?.Alter(itemId, itemCount);
                    itemId = 0;
                    itemCount = 0;
                    itemInc = 0;
                    return false;
                }
                return true;
            }
        }

        public class PasteAnywayPatch
        {
            public static bool IsPatched { get; internal set; }

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
        }

        public class DriftBuildingsPatch
        {
            public static bool IsPatched;
            [HarmonyPrefix]
            [HarmonyPatch(typeof(BuildTool_Click), "UpdatePreviewModels")]
            public static void Prefix(ref BuildTool_Click __instance)
            {
                if (!DriftBuildings)
                    return;
                for (int i = 0; i < __instance.buildPreviews.Count; i++)
                {
                    var bp = __instance.buildPreviews[i];
                    bp.lpos *= DriftBuildingHeight;
                    bp.lpos2 *= DriftBuildingHeight;
                }
            }

            [HarmonyPrefix]
            [HarmonyPatch(typeof(BuildTool_BlueprintPaste), "CreatePrebuilds")]
            public static void BuildTool_BlueprintPasteCreatePrebuildsPatch(ref BuildTool_BlueprintPaste __instance)
            {
                if (!DriftBuildings)
                    return;
                for (int i = 0; i < __instance.bpCursor; i++)
                {
                    var bp = __instance.bpPool[i];
                    if (bp.desc.isBelt)
                    {
                        bp.lpos *= DriftBuildingHeight;
                        bp.lpos2 *= DriftBuildingHeight;
                    }
                }
            }

            [HarmonyPrefix]
            [HarmonyPatch(typeof(BuildTool_BlueprintPaste), "UpdatePreviewModels")]
            public static void BuildTool_BlueprintPastePrefix(ref BuildTool_BlueprintPaste __instance)
            {
                if (!DriftBuildings)
                    return;
                for (int i = 0; i < __instance.bpCursor; i++)
                {
                    var bp = __instance.bpPool[i];
                    if (bp.desc.isBelt)
                    {
                        bp.lpos *= DriftBuildingHeight;
                        bp.lpos2 *= DriftBuildingHeight;
                    }
                }
            }

            [HarmonyPostfix]
            [HarmonyPatch(typeof(BuildTool_BlueprintPaste), "UpdatePreviewModels")]
            public static void BuildTool_BlueprintPastePostfix(ref BuildTool_BlueprintPaste __instance)
            {
                if (!DriftBuildings)
                    return;
                for (int i = 0; i < __instance.bpCursor; i++)
                {
                    var bp = __instance.bpPool[i];
                    if (bp.desc.isBelt)
                    {
                        bp.lpos /= DriftBuildingHeight;
                        bp.lpos2 /= DriftBuildingHeight;
                    }

                }
            }

            [HarmonyPrefix]
            [HarmonyPatch(typeof(BuildTool_BlueprintPaste), "AlterBPGPUIModel")]
            public static void BuildTool_BlueprintPasteAlterBPGPUIModelPatch(ref BuildPreview _bp)
            {
                if (!DriftBuildings)
                    return;
                _bp.lpos *= DriftBuildingHeight;
                _bp.lpos2 *= DriftBuildingHeight;
            }
        }

        public class UIPatch
        {
            public static bool tempisInstantItem;
            //关闭窗口快捷键
            [HarmonyPrefix]
            [HarmonyPatch(typeof(UIGame), "On_E_Switch")]
            public static bool CloseWindowPatch()
            {
                if (guidraw.DisplayingWindow)
                {
                    guidraw.CloseMainWindow();
                    return false;
                }
                return true;
            }

            [HarmonyPostfix]
            [HarmonyPatch(typeof(UIReplicatorWindow), "SetSelectedRecipeIndex")]
            public static void UIReplicatorWindowSetSelectedRecipe(ref UIReplicatorWindow __instance)
            {
                if (!isInstantItem.Value || !tempisInstantItem || !(Traverse.Create(__instance).Field("selectedRecipe").GetValue() is RecipeProto recipe))
                {
                    return;
                }
                Player mainPlayer = GameMain.mainPlayer;
                RecipeProto recipeProto = recipe;
                if (recipe == null) return;
                int num = 1;
                if (__instance.multipliers.ContainsKey(recipe.ID))
                {
                    num = __instance.multipliers[recipe.ID];
                }
                if (num < 1)
                {
                    num = 1;
                }
                else if (num > 10)
                {
                    num = 10;
                }
                for (int i = 0; i < recipeProto.Results.Length; i++)
                {
                    int num2 = recipeProto.Results[i];
                    int stackSize = LDB.items.Select(num2).StackSize;
                    int num3 = __instance.isBatch ? (num * stackSize) : num;
                    int num4 = mainPlayer.TryAddItemToPackage(num2, num3, 0, true, 0);
                    int num5 = num3 - num4;
                    if (num5 > 0)
                    {
                        ItemProto itemProto = LDB.items.Select(num2);
                        if (itemProto != null)
                        {
                            UIRealtimeTip.Popup(string.Format("背包已满未添加".Translate(), num5, itemProto.name), true, 0);
                        }
                    }
                    if (num4 > 0)
                    {
                        UIItemup.Up(num2, num4);
                    }
                    mainPlayer.mecha.AddProductionStat(num2, num3, mainPlayer.nearestFactory);
                }
            }

            [HarmonyPostfix]
            [HarmonyPatch(typeof(UIReplicatorWindow), "_OnOpen")]
            public static void UIReplicatorWindow_OnOpen(ref UIReplicatorWindow __instance)
            {
                if (!isInstantItem.Value)
                {
                    return;
                }
                __instance.isInstantItem = tempisInstantItem;
                __instance.instantItemSwitch.gameObject.SetActive(true);
                __instance.instantItemSwitch.SetToggleNoEvent(tempisInstantItem);
                if (tempisInstantItem)
                {
                    Traverse.Create(__instance).Method("RefreshRecipeIcons").GetValue();
                }
                __instance.instantItemSwitch.SetToggleNoEvent(tempisInstantItem);
                __instance.batchSwitch.gameObject.SetActive(tempisInstantItem);
                __instance.batchSwitch.SetToggleNoEvent(true);
                __instance.sandboxAddUsefulItemButton.gameObject.SetActive(tempisInstantItem);
                __instance.sandboxClearPackageButton.gameObject.SetActive(tempisInstantItem);
            }

            [HarmonyPostfix]
            [HarmonyPatch(typeof(UIReplicatorWindow), "OnInstantSwitchClick")]
            public static void UIReplicatorWindowOnInstantSwitchClick(ref UIReplicatorWindow __instance)
            {
                if (!isInstantItem.Value)
                {
                    return;
                }
                tempisInstantItem = __instance.isInstantItem;
                __instance.sandboxAddUsefulItemButton.gameObject.SetActive(tempisInstantItem);
                __instance.sandboxClearPackageButton.gameObject.SetActive(tempisInstantItem);
            }

            [HarmonyPrefix]
            [HarmonyPatch(typeof(UIReplicatorWindow), "OnOkButtonClick")]
            public static bool UIReplicatorWindowOnOkButtonClick(ref UIReplicatorWindow __instance)
            {
                if (!isInstantItem.Value || !tempisInstantItem)
                {
                    return true;
                }
                var selectedRecipe = Traverse.Create(__instance).Field("selectedRecipe").GetValue() as RecipeProto;
                if (selectedRecipe == null)
                {
                    return false;
                }
                int id = selectedRecipe.ID;
                int num = 1;
                if (__instance.multipliers.ContainsKey(id))
                {
                    num = __instance.multipliers[id];
                }
                if (num < 1)
                {
                    num = 1;
                }
                else if (num > 10)
                {
                    num = 10;
                }
                Player mainPlayer = GameMain.mainPlayer;
                RecipeProto recipeProto = LDB.recipes.Select(id);
                for (int i = 0; i < recipeProto.Results.Length; i++)
                {
                    int num2 = recipeProto.Results[i];
                    int stackSize = LDB.items.Select(num2).StackSize;
                    int num3 = __instance.isBatch ? (num * stackSize) : num;
                    int num4 = mainPlayer.TryAddItemToPackage(num2, num3, 0, true, 0);
                    int num5 = num3 - num4;
                    if (num5 > 0)
                    {
                        ItemProto itemProto = LDB.items.Select(num2);
                        if (itemProto != null)
                        {
                            UIRealtimeTip.Popup(string.Format("背包已满未添加".Translate(), num5, itemProto.name), true, 0);
                        }
                    }
                    if (num4 > 0)
                    {
                        UIItemup.Up(num2, num4);
                    }
                    mainPlayer.mecha.AddProductionStat(num2, num3, mainPlayer.nearestFactory);
                }
                return false;
            }

            [HarmonyPrefix]
            [HarmonyPatch(typeof(UITechTree), "_OnUpdate")]
            public static void UITechTreeSelectTechPatch(UITechTree __instance)
            {
                if (!unlockpointtech || __instance.selected == null)
                {
                    return;
                }
                TechProto techProto = __instance.selected.techProto;
                if (GameMain.history.techStates[techProto.ID].unlocked || techProto.MaxLevel > 10) return;
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

            [HarmonyPrefix]
            [HarmonyPatch(typeof(UITechTree), "UpdateScale")]
            public static bool UITechTreeUpdateScalePatch(UITechTree __instance)
            {
                if (unlockpointtech && (__instance.selected != null || __instance.centerViewNode != null)) return false;
                return true;
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

            [HarmonyPostfix]
            [HarmonyPatch(typeof(UIAbnormalityTip), "_OnOpen")]
            public static void UIAbnormalityTipDeterminePatch(UIAbnormalityTip __instance)
            {
                if (CloseUIAbnormalityTip.Value)
                {
                    __instance._Close();
                }
            }

            [HarmonyPostfix]
            [HarmonyPatch(typeof(UIStarmap), "UpdateCursorView")]
            public static void UIStarmapUpdateCursorView(UIStarmap __instance)
            {
                if (EnableTp.Value)
                {
                    if (__instance.focusPlanet != null)
                    {
                        Player mainPlayer = GameMain.mainPlayer;
                        __instance.fastTravelButton.gameObject.SetActive(mainPlayer.planetId != __instance.focusPlanet.planet.id && !mainPlayer.warping);
                    }
                }
            }

            [HarmonyPostfix]
            [HarmonyPatch(typeof(UIStarmap), "OnFastTravelButtonClick")]
            public static void UIStarmapOnFastTravelButtonClick(UIStarmap __instance)
            {
                if (EnableTp.Value)
                {
                    if (__instance.focusPlanet != null)
                    {
                        GameMain.mainPlayer.controller.actionSail.StartFastTravelToPlanet(__instance.focusPlanet.planet);
                    }
                }
            }
        }

        public class PowerSystemPatch
        {
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
                if (!Buildingnoconsume.Value)
                {
                    return;
                }
                int entityId = __instance.consumerPool[__result].entityId;
                int modelIndex = __instance.factory.entityPool[entityId].modelIndex;

                if (modelIndex > 0)
                {
                    ModelProto modelProto = LDB.models.modelArray[modelIndex];
                    if (modelProto?.prefabDesc != null && modelProto.prefabDesc.isFieldGenerator)
                    {
                        return;
                    }
                }
                __instance.consumerPool[__result].requiredEnergy = 0;
                __instance.consumerPool[__result].idleEnergyPerTick = 0;
                __instance.consumerPool[__result].workEnergyPerTick = 0;
            }

            [HarmonyPrefix]
            [HarmonyPatch(typeof(PowerSystem), "NewNodeComponent")]
            public static void NewNodeComponentPatchPrefix(PowerSystem __instance, ref int entityId, ref float conn, ref float cover)
            {
                var itemID = __instance.factory.entityPool[entityId].protoId;
                if (PlanetPower_bool.Value && itemID == 2210)
                {
                    cover = GameMain.localPlanet.realRadius * 4;
                    if (farconnectdistance)
                    {
                        conn = GameMain.localPlanet.realRadius * 1.5f;
                        farconnectdistance = false;
                    }
                }
                else if (Windturbinescovertheglobe.Value && itemID == 2203)
                {
                    cover = GameMain.localPlanet.realRadius * 4;
                }
            }

            [HarmonyPostfix]
            [HarmonyPatch(typeof(PowerSystem), "NewNodeComponent")]
            public static void NewNodeComponentPatchPostfix(ref int __result, PowerSystem __instance)
            {
                if (!Buildingnoconsume.Value || GameMain.localPlanet.factory.entityPool[__instance.nodePool[__result].entityId].stationId > 0)
                {
                    return;
                }
                __instance.nodePool[__result].requiredEnergy = 0;
                __instance.nodePool[__result].idleEnergyPerTick = 0;
            }

        }

        public class PlanetFactoryPatch
        {
            [HarmonyPostfix]
            [HarmonyPatch(typeof(PlanetFactory), "ComputeFlattenTerrainReform")]
            public static void PlanetFactoryNoComsumeSand(ref int __result)
            {
                if (!InfiniteSand.Value)
                {
                    return;
                }
                if (GameMain.mainPlayer != null && GameMain.mainPlayer.sandCount < int.MaxValue)
                {
                    GameMain.mainPlayer.SetSandCount(int.MaxValue);
                }
                __result = 0;
            }

            [HarmonyPostfix]
            [HarmonyPatch(typeof(PlanetFactory), "UpgradeEntityWithComponents")]
            public static void UpgradeEntityWithComponentsPatch(int entityId, PlanetFactory __instance)
            {
                if (!Buildingnoconsume.Value || entityId == 0 || __instance.entityPool[entityId].id == 0)
                    return;
                if (GameMain.localPlanet.factory.entityPool[entityId].stationId > 0)
                {
                    return;
                }
                int powerConId = __instance.entityPool[entityId].powerConId;
                if (powerConId <= 0)
                {
                    return;
                }
                __instance.powerSystem.consumerPool[powerConId].idleEnergyPerTick = 0;
                __instance.powerSystem.consumerPool[powerConId].workEnergyPerTick = 0;
            }

            [HarmonyPrefix]
            [HarmonyPatch(typeof(PlanetFactory), "TakeBackItemsInEntity")]
            public static bool TakeBackItemsInEntityPatch()
            {
                return !entityitemnoneed;
            }

            [HarmonyPrefix]
            [HarmonyPatch(typeof(PlanetFactory), "CreateEntityLogicComponents")]
            public static void CreateEntityLogicComponentsPatch(PlanetFactory __instance, int entityId, PrefabDesc desc, int prebuildId)
            {
                if (desc.isStation && !string.IsNullOrEmpty(AutoChangeStationName.Value))
                {
                    __instance.WriteExtraInfoOnPrebuild(prebuildId, AutoChangeStationName.Value.getTranslate());
                }
            }

            [HarmonyPrefix]
            [HarmonyPatch(typeof(ConstructionSystem), "GameTick")]
            public static void ConstructionSystemGameTick(ConstructionSystem __instance)
            {
                if (BuildNotime_bool.Value)
                {
                    PlanetFactory planetFactory = player?.factory;
                    if (GameMain.localPlanet == null || planetFactory == null)
                    {
                        return;
                    }

                    PrebuildData[] prebuildPool = planetFactory.prebuildPool;
                    if (planetFactory.prebuildCount <= 0)
                    {
                        return;
                    }

                    int num = 0;
                    PlanetFactory.batchBuild = true;
                    HighStopwatch highStopwatch = new HighStopwatch();
                    highStopwatch.Begin();
                    planetFactory.BeginFlattenTerrain();
                    for (int i = planetFactory.prebuildCursor - 1; i > 0; i--)
                    {
                        if (prebuildPool[i].itemRequired > 0 && !Infinitething.Value && !ArchitectMode.Value) continue;
                        if (prebuildPool[i].id == i && !prebuildPool[i].isDestroyed)
                        {
                            planetFactory.BuildFinally(GameMain.mainPlayer, prebuildPool[i].id, false);
                            num++;
                        }
                    }

                    planetFactory.EndFlattenTerrain();

                    PlanetFactory.batchBuild = false;
                    if (num > 0)
                    {
                        GameMain.localPlanet.physics?.raycastLogic?.NotifyBatchObjectRemove();
                        GameMain.localPlanet.audio?.SetPlanetAudioDirty();
                    }
                }
            }


        }

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
                                if (!StationPowerGen.Value)
                                {
                                    StationComponentPatch.StationPowerGeneration(sc, consumeRegister);
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
                                if (!StationPowerGen.Value)
                                {
                                    StationComponentPatch.StationPowerGeneration(sc, consumeRegister);
                                }
                                break;
                            case "星球量子传输站":
                            case "星系量子传输站":
                                if (remoteTick)
                                {
                                    StationComponentPatch.StationPowerGeneration(sc, consumeRegister);
                                }
                                break;
                        }
                    }
                }
            }

        }

        public class StorageComponentPatch
        {
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
            [HarmonyPatch(typeof(StorageComponent), "GetItemCount", new Type[] { typeof(int) })]
            public static void GetItemCountPatch(StorageComponent __instance, int itemId, ref int __result)
            {
                if (ArchitectMode.Value)
                {
                    if (itemId <= 0 || itemId >= 6007 || __instance == null || __instance.id != GameMain.mainPlayer.package.id) return;
                    if (LDB.items.Select(itemId).CanBuild && __result == 0) __result = 100;
                }
            }

        }

        public class StationComponentPatch
        {
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
                    pd.CalcVeinAmounts(ref veinAmounts, new HashSet<int>(), UIRoot.instance.uiGame.veinAmountDisplayFilter);
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
                int getmine = 0;
                if (pd.waterItemId == itemid)
                {
                    return (int)(30 * GameMain.history.miningSpeedScale * Stationminenumber.Value);
                }
                int neednumber = itemid != 1007 ? (int)(minenumber * GameMain.history.miningCostRate / 2) : (int)(minenumber * GameMain.history.miningCostRate);
                int maxmineNumber = itemid != 1007 ? (int)(minenumber * GameMain.history.miningSpeedScale / 2) : (int)(minenumber * GameMain.history.miningSpeedScale); ;
                if (GameMain.data.gameDesc.isInfiniteResource)
                    return maxmineNumber;
                if (LDB.veins.GetVeinTypeByItemId(itemid) == EVeinType.None || pd == null)
                {
                    return 0;
                }
                if (minenumber > 0 && neednumber == 0)
                {
                    return itemid != 1007 ? (int)(minenumber * GameMain.history.miningSpeedScale / 2) : (int)(minenumber * GameMain.history.miningSpeedScale);
                }
                foreach (VeinData i in pd.factory.veinPool)
                {
                    if (i.type != LDB.veins.GetVeinTypeByItemId(itemid))
                        continue;
                    if (i.amount > neednumber - getmine)
                    {
                        if (itemid == 1007 && i.amount * VeinData.oilSpeedMultiplier <= 0.1)
                        {
                            int dis = veinproperty.oillowerlimit - pd.factory.veinPool[i.id].amount;
                            pd.factory.veinPool[i.id].amount += dis;
                            pd.factory.veinGroups[i.groupIndex].amount += dis;
                            getmine += (int)(0.1 * GameMain.history.miningSpeedScale * Stationminenumber.Value);
                        }
                        else
                        {
                            pd.factory.veinPool[i.id].amount -= neednumber;
                            pd.factory.veinGroups[i.groupIndex].amount -= neednumber;
                            getmine = minenumber;
                        }
                    }
                    else
                    {
                        if (itemid != 1007)
                        {
                            pd.factory.veinGroups[i.groupIndex].count--;
                            pd.factory.veinGroups[i.groupIndex].amount -= i.amount;
                            pd.factory.RemoveVeinWithComponents(i.id);
                            getmine += i.amount;
                        }
                    }
                    if (pd.factory.veinGroups[i.groupIndex].count == 0)
                    {
                        pd.factory.veinGroups[i.groupIndex].type = 0;
                        pd.factory.veinGroups[i.groupIndex].amount = 0;
                        pd.factory.veinGroups[i.groupIndex].pos = Vector3.zero;
                    }
                    if (getmine == minenumber) break;
                }
                return itemid != 1007 ? (int)(getmine * GameMain.history.miningSpeedScale / 2) : (int)(getmine * GameMain.history.miningSpeedScale);
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
                        continue;
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
                            if (needtrashsand.Value)
                                player.TryAddItemToPackage(1099, trashnum * 100, 0, false);
                        }
                    }
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

        public class TankComponentPatch
        {
            [HarmonyPrefix]
            [HarmonyPatch(typeof(TankComponent), "GameTick")]
            public static void TankComponentGameTickPatchPrefix(ref TankComponent __instance)
            {
                if (!TankMaxproliferator.Value)
                {
                    return;
                }
                __instance.fluidInc = __instance.fluidCount * incAbility;
            }

        }

        public class FactorySystemPatch
        {

            [HarmonyPostfix]
            [HarmonyPatch(typeof(FactorySystem), "NewEjectorComponent")]
            public static void NewEjectorComponentPatch(ref int __result, FactorySystem __instance)
            {
                if (!quickEjector)
                {
                    return;
                }
                __instance.ejectorPool[__result].bulletCount = int.MaxValue;
                __instance.ejectorPool[__result].bulletId = 1501;
                __instance.ejectorPool[__result].coldSpend = 5;
                __instance.ejectorPool[__result].chargeSpend = 4;
            }

            [HarmonyPostfix]
            [HarmonyPatch(typeof(FactorySystem), "NewSiloComponent")]
            public static void NewSiloComponentPatch(ref int __result, FactorySystem __instance)
            {
                if (!quicksilo)
                {
                    return;
                }
                __instance.siloPool[__result].bulletCount = int.MaxValue;
                __instance.siloPool[__result].bulletId = 1503;
                __instance.siloPool[__result].coldSpend = 40;
                __instance.siloPool[__result].chargeSpend = 80;

            }

            [HarmonyPrefix]
            [HarmonyPatch(typeof(FactoryStorage), "NewTankComponent")]
            public static void NewTankComponentPatch(ref int fCount)
            {
                if (!Infinitestoragetank.Value)
                    return;
                fCount = int.MaxValue - 200;
            }
        }

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

            [HarmonyPrefix]
            [HarmonyPatch(typeof(BuildTool_Click), "CheckBuildConditions")]
            public static bool CheckBuildConditionsPatchPrefix(BuildTool_Click __instance, ref bool __result)
            {
                if (!PasteBuildAnyWay && !build_station_nocondition.Value)
                {
                    return true;
                }
                for (int i = 0; i < __instance.buildPreviews.Count; i++)
                {
                    var prefab = __instance.buildPreviews[i]?.item?.prefabDesc;
                    if (prefab != null && prefab.veinMiner)
                    {
                        return true;
                    }
                    if (build_station_nocondition.Value && !PasteBuildAnyWay && !(prefab.isStation || prefab.isStation || prefab.isCollectStation))
                    {
                        return true;
                    }
                }
                __result = true;
                return false;
            }
        }

        public class SpraycoaterComponentPatch
        {
            [HarmonyPrefix]
            [HarmonyPatch(typeof(SpraycoaterComponent), "InternalUpdate")]
            public static void SpraycoaterComponentInternalUpdatePrefix(ref SpraycoaterComponent __instance)
            {
                if (!Maxproliferator.Value)
                {
                    return;
                }
                ability = __instance.incAbility;
                __instance.incAbility = 10;
            }

            [HarmonyPostfix]
            [HarmonyPatch(typeof(SpraycoaterComponent), "InternalUpdate")]
            public static void SpraycoaterComponentInternalUpdatePostfix(ref SpraycoaterComponent __instance)
            {
                if (Maxproliferator.Value || __instance.incAbility <= 4)
                {
                    return;
                }
                __instance.incAbility = ability;
            }
        }

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

        public class CombatPatch
        {
            [HarmonyPrefix]
            [HarmonyPatch(typeof(Mecha), "TakeDamage")]
            public static bool MechaTakeDamage(Mecha __instance)
            {
                if (LockPlayerHp.Value)
                {
                    return false;
                }

                return true;
            }

            [HarmonyPrefix]
            [HarmonyPatch(typeof(Mecha), "UpdateCombatStats")]
            public static void MechaTakeUpdateCombatStats(Mecha __instance)
            {
                if (LockPlayerHp.Value)
                {
                    __instance.energyShieldEnergy = __instance.energyShieldCapacity;
                }
            }

            [HarmonyPrefix]
            [HarmonyPatch(typeof(ConstructionSystem), "GameTick")]
            public static void PlanetFactoryGameTick(ConstructionSystem __instance)
            {
                if (LockBuildHp.Value || LockEnemyHp.Value)
                {
                    CombatStat[] buffer = __instance.factory.skillSystem.combatStats.buffer;
                    foreach (CombatStat combatStat in buffer)
                    {
                        if (LockBuildHp.Value && combatStat.objectType == 0)
                        {
                            buffer[combatStat.id].hp = combatStat.hpMax;
                        }
                        if (LockEnemyHp.Value && combatStat.objectType == 4)
                        {
                            buffer[combatStat.id].hp = combatStat.hpMax;
                        }
                    }
                }
            }

            [HarmonyPrefix]
            [HarmonyPatch(typeof(Mecha), "TickLaserFireCondition")]
            public static void MechaTickLaserFireCondition(Mecha __instance)
            {
                if (PlayerQuickAttack.Value)
                {
                    __instance.laserFire = 0;
                    __instance.laserEnergy = __instance.laserEnergyCapacity;
                }
            }

            [HarmonyPrefix]
            [HarmonyPatch(typeof(Mecha), "TestLaserTurretEnergy")]
            public static bool MechaTestLaserTurretEnergy(ref bool __result)
            {
                if (PlayerQuickAttack.Value)
                {
                    __result = true;
                    return false;
                }
                return true;
            }

            [HarmonyPrefix]
            [HarmonyPatch(typeof(SkillSystem), "GameTick")]
            public static void SkillSystemGameTick(SkillSystem __instance)
            {
                if (killEnemys.Count > 0)
                {
                    var enemydata = killEnemys.Dequeue();

                    if (enemydata.id > 0)
                    {
                        if (enemydata.combatStatId > 0 && __instance.combatStats.buffer[enemydata.combatStatId].id == enemydata.combatStatId)
                        {
                            __instance.combatStats.buffer[enemydata.combatStatId].hp = 0;
                        }
                        else if (enemydata.originAstroId > 1000000)
                        {
                            ref EnemyData reference = ref __instance.sector.enemyPool[enemydata.id];
                            __instance.sector.KillEnemyFinal(reference.id, ref CombatStat.empty);
                        }
                        else if (enemydata.originAstroId > 100 && enemydata.originAstroId <= 204899 && enemydata.originAstroId % 100 > 0)
                        {
                            PlanetFactory planetFactory = __instance.astroFactories[enemydata.originAstroId];
                            ref EnemyData reference = ref planetFactory.enemyPool[enemydata.id];
                            planetFactory.KillEnemyFinally(GameMain.data.mainPlayer, reference.id, ref CombatStat.empty);
                        }
                    }
                }
            }

            [HarmonyPrefix]
            [HarmonyPatch(typeof(EnemyDFHiveSystem), "GameTickLogic")]
            public static void EnemyDFHiveSystemPreGameTickLogic(EnemyDFHiveSystem __instance)
            {
                if (SpaceAlwaysClearThreat.Value)
                {
                    __instance.evolve.threat = 0;
                    __instance.evolve.threatshr = 0;
                }
                if (SpaceAlwaysMaxThreat.Value && __instance.evolve.threat < __instance.evolve.maxThreat && __instance.evolve.waveTicks == 0)
                {
                    if (GameMain.localStar != null && __instance.starData.index == GameMain.localStar.index)
                    {
                        __instance.evolve.threat = __instance.evolve.maxThreat * 2 + 1;
                        __instance.evolve.threatshr = __instance.evolve.maxThreat * 2 + 1;
                    }
                }
            }

            [HarmonyPostfix]
            [HarmonyPatch(typeof(EnemyDFHiveSystem), "GameTickLogic")]
            public static void EnemyDFHiveSystemGameTickLogic(EnemyDFHiveSystem __instance)
            {
                if (SpaceAlwaysClearThreat.Value)
                {
                    __instance.evolve.threat = 0;
                    __instance.evolve.threatshr = 0;
                }
                if (SpaceAlwaysMaxThreat.Value && __instance.evolve.threat < __instance.evolve.maxThreat && __instance.evolve.waveTicks == 0)
                {
                    if (GameMain.localStar != null && __instance.starData.index == GameMain.localStar.index)
                    {
                        __instance.evolve.threat = __instance.evolve.maxThreat * 2 + 1;
                        __instance.evolve.threatshr = __instance.evolve.maxThreat * 2 + 1;
                    }
                }

                if (PlayerFakeDeath.Value && __instance.isLocal)
                {
                    __instance.local_player_exist_alive = false;
                    __instance.local_player_in_range = false;
                }
            }

            [HarmonyPrefix]
            [HarmonyPatch(typeof(DFGBaseComponent), "UpdateFactoryThreat")]
            public static void DFGBaseComponentPreLogicTick(DFGBaseComponent __instance, ref float power_threat_factor, ref float player_threat)
            {
                if (LocalAlwaysClearThreat.Value)
                {
                    power_threat_factor = 0;
                    player_threat = 0;
                    __instance.evolve.threat = 0;
                    __instance.evolve.threatshr = 0;
                }
                if (LocalAlwaysMaxThreat.Value && __instance.evolve.threat < __instance.evolve.maxThreat && __instance.evolve.waveTicks == 0)
                {
                    if (GameMain.localPlanet != null && __instance.groundSystem.factory.planet.index == GameMain.localPlanet.index)
                    {
                        __instance.evolve.threat = __instance.evolve.maxThreat * 2 + 1;
                        __instance.evolve.threatshr = __instance.evolve.maxThreat * 2 + 1;
                    }
                }
            }

            [HarmonyPostfix]
            [HarmonyPatch(typeof(DFGBaseComponent), "UpdateFactoryThreat")]
            public static void DFGBaseComponentLogicTick(DFGBaseComponent __instance, ref float power_threat_factor, ref float player_threat)
            {
                if (LocalAlwaysClearThreat.Value)
                {
                    power_threat_factor = 0;
                    player_threat = 0;
                    __instance.evolve.threat = 0;
                    __instance.evolve.threatshr = 0;
                }
                if (LocalAlwaysMaxThreat.Value && __instance.evolve.threat < __instance.evolve.maxThreat && __instance.evolve.waveTicks == 0)
                {
                    if (GameMain.localPlanet != null && __instance.groundSystem.factory.planet.index == GameMain.localPlanet.index)
                    {
                        __instance.evolve.threat = __instance.evolve.maxThreat * 2 + 1;
                        __instance.evolve.threatshr = __instance.evolve.maxThreat * 2 + 1;
                    }
                }
            }

            [HarmonyPostfix]
            [HarmonyPatch(typeof(EnemyBuilderComponent), "LogicTick")]
            public static void EnemyBuilderComponentBuildLogic_Ground(EnemyBuilderComponent __instance)
            {
                if (DarkFogBuilderQuickBuild.Value)
                {
                    __instance.energy = __instance.maxEnergy;
                    __instance.matter = __instance.maxMatter;
                    __instance.sp = __instance.spMax;
                    __instance.buildCDTime = 0;
                }
            }

            [HarmonyPrefix]
            [HarmonyPatch(typeof(TurretComponent), "InternalUpdate")]
            public static void TurretComponentInternalUpdate(ref TurretComponent __instance)
            {
                if (TurrentKeepSuperNoval.Value)
                {
                    __instance.supernovaTick = 901;
                    __instance.supernovaStrength = 30f;
                }
            }

            [HarmonyPrefix]
            [HarmonyPatch(typeof(HatredList), "HateTarget", new Type[] { typeof(ETargetType), typeof(int), typeof(int), typeof(int), typeof(EHatredOperation), })]
            public static bool HatredListHateTarget(ETargetType type)
            {
                if (PlayerFakeDeath.Value && type == ETargetType.Player)
                {
                    return false;
                }
                return true;
            }
            [HarmonyPostfix]
            [HarmonyPatch(typeof(EnemyDFGroundSystem), "GameTickLogic")]
            public static void EnemyDFGroundSystemGameTickLogic(EnemyDFGroundSystem __instance)
            {
                if (PlayerFakeDeath.Value && __instance.isLocalLoaded)
                {
                    __instance.local_player_exist_alive = false;
                }
            }

        }

    }

}
