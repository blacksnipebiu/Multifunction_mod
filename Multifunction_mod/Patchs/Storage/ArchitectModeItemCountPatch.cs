using HarmonyLib;
using System;
using static Multifunction_mod.Multifunction;

namespace Multifunction_mod.Patchs.Storage
{
    /// <summary>
    /// 建筑师模式：背包里可建造物品数量为 0 时按 100 显示。
    /// </summary>
    public class ArchitectModeItemCountPatch
    {
        public static bool Enable
        {
            get { return PatchToggle<ArchitectModeItemCountPatch>.Enable; }
            set { PatchToggle<ArchitectModeItemCountPatch>.Enable = value; }
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(StorageComponent), "GetItemCount", new Type[] { typeof(int) })]
        public static void GetItemCountPatch(StorageComponent __instance, int itemId, ref int __result)
        {
            if (itemId <= 0 || itemId >= 6007 || __instance == null || __instance.id != GameMain.mainPlayer.package.id) return;
            if (LDB.items.Select(itemId).CanBuild && __result == 0) __result = 100;
        }
    }
}
