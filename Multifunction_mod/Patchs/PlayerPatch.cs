using HarmonyLib;
using UnityEngine;
using static Multifunction_mod.Multifunction;

namespace Multifunction_mod.Patchs
{
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
}
