using HarmonyLib;
using static Multifunction_mod.Multifunction;

namespace Multifunction_mod.Patchs
{
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
}
