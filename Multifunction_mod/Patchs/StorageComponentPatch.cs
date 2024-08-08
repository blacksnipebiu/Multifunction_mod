using HarmonyLib;
using System;
using static Multifunction_mod.Multifunction;

namespace Multifunction_mod.Patchs
{
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
}
