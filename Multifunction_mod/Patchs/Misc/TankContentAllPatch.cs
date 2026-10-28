using HarmonyLib;
using static Multifunction_mod.Multifunction;

namespace Multifunction_mod.Patchs.Misc
{
    /// <summary>
    /// 储液站任意存：让所有物品都被判定为流体。
    /// </summary>
    public class TankContentAllPatch
    {
        public static bool Enable
        {
            get { return PatchToggle<TankContentAllPatch>.Enable; }
            set { PatchToggle<TankContentAllPatch>.Enable = value; }
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(ItemProto), "isFluid")]
        public static void ItemProtoisFluidPatch(ref bool __result)
        {
            __result = true;
        }
    }
}
