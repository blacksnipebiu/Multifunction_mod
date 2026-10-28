using HarmonyLib;
using static Multifunction_mod.Multifunction;

namespace Multifunction_mod.Patchs.Power
{
    /// <summary>无限能源：恒星发电机燃料/发电量拉满</summary>
    public class InfiniteStarPowerPatch
    {
        public static bool Enable
        {
            get { return PatchToggle<InfiniteStarPowerPatch>.Enable; }
            set { PatchToggle<InfiniteStarPowerPatch>.Enable = value; }
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(PowerSystem), "NewGeneratorComponent")]
        public static void PowerSystemNewGeneratorComponent(ref int __result, PowerSystem __instance)
        {
            if (__instance.genPool[__result].fuelMask == 4)
            {
                __instance.genPool[__result].fuelId = 1803;
                __instance.genPool[__result].fuelCount = 100;
                __instance.genPool[__result].fuelEnergy = long.MaxValue;
                __instance.genPool[__result].genEnergyPerTick = 1000000000000;
            }
        }
    }
}
