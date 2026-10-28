using HarmonyLib;
using static Multifunction_mod.Multifunction;

namespace Multifunction_mod.Patchs.Misc
{
    /// <summary>
    /// 物流站永久满电：每个 tick 把能量补到上限。
    /// </summary>
    public class DispenserFullEnergyPatch
    {
        public static bool Enable
        {
            get { return PatchToggle<DispenserFullEnergyPatch>.Enable; }
            set { PatchToggle<DispenserFullEnergyPatch>.Enable = value; }
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(DispenserComponent), "InternalTick")]
        public static void DispenserComponentInternalTickPrefix(ref DispenserComponent __instance)
        {
            __instance.energy = __instance.energyMax;
        }
    }
}
