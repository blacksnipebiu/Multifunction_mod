using HarmonyLib;

namespace Multifunction_mod.Patchs.Misc
{
    /// <summary>
    /// 无限元数据：物品总属性直接返回 int.MaxValue。
    /// </summary>
    public class UnlimitedMetadataPatch
    {
        public static bool Enable
        {
            get { return PatchToggle<UnlimitedMetadataPatch>.Enable; }
            set { PatchToggle<UnlimitedMetadataPatch>.Enable = value; }
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(PropertySystem), "GetItemTotalProperty")]
        public static void GetItemTotalPropertyPatch(ref long __result)
        {
            __result = int.MaxValue;
        }
    }
}
