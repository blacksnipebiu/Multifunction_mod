using HarmonyLib;
using static Multifunction_mod.Multifunction;

namespace Multifunction_mod.Patchs.Storage
{
    /// <summary>
    /// 堆叠倍数：按 StackMultiple 配置放大存储堆叠上限（导入存档与静态数据加载）。
    /// </summary>
    public class StackMultiplePatch
    {
        public static bool Enable
        {
            get { return PatchToggle<StackMultiplePatch>.Enable; }
            set { PatchToggle<StackMultiplePatch>.Enable = value; }
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(StorageComponent), "Import")]
        public static void StorageComponentImportPatch(StorageComponent __instance)
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

        [HarmonyPostfix]
        [HarmonyPatch(typeof(StorageComponent), "LoadStatic")]
        public static void Postfix()
        {
            ItemProto[] dataArray = LDB.items.dataArray;
            for (int j = 0; j < dataArray.Length; j++)
            {
                StorageComponent.itemStackCount[dataArray[j].ID] = dataArray[j].StackSize * StackMultiple.Value;
            }
        }
    }
}
