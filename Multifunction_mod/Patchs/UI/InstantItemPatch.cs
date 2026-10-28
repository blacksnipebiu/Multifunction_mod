using HarmonyLib;
using static Multifunction_mod.Multifunction;

namespace Multifunction_mod.Patchs.UI
{
    /// <summary>
    /// 直接获取物品：复制窗口开启“直接获取”后，选中配方即直接获得产物，不消耗材料。
    /// </summary>
    public class InstantItemPatch
    {
        public static bool Enable
        {
            get { return PatchToggle<InstantItemPatch>.Enable; }
            set { PatchToggle<InstantItemPatch>.Enable = value; }
        }

        //复制窗口“直接获取物品”开关的临时状态，供打开窗口/切换开关时复用
        public static bool tempisInstantItem;

        [HarmonyPostfix]
        [HarmonyPatch(typeof(UIReplicatorWindow), "SetSelectedRecipeIndex")]
        public static void UIReplicatorWindowSetSelectedRecipe(ref UIReplicatorWindow __instance)
        {
            if (!tempisInstantItem || !(Traverse.Create(__instance).Field("selectedRecipe").GetValue() is RecipeProto recipe))
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
            tempisInstantItem = __instance.isInstantItem;
            __instance.sandboxAddUsefulItemButton.gameObject.SetActive(tempisInstantItem);
            __instance.sandboxClearPackageButton.gameObject.SetActive(tempisInstantItem);
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(UIReplicatorWindow), "OnOkButtonClick")]
        public static bool UIReplicatorWindowOnOkButtonClick(ref UIReplicatorWindow __instance)
        {
            if (!tempisInstantItem)
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
    }
}
