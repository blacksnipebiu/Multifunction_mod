using HarmonyLib;
using static Multifunction_mod.Multifunction;

namespace Multifunction_mod.Patchs.Power
{
    /// <summary>风力涡轮机无限能源</summary>
    public class WindTurbineUnlimitedEnergyPatch
    {
        public static bool Enable
        {
            get { return PatchToggle<WindTurbineUnlimitedEnergyPatch>.Enable; }
            set { PatchToggle<WindTurbineUnlimitedEnergyPatch>.Enable = value; }
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(PowerSystem), "NewGeneratorComponent")]
        public static void PowerSystemNewGeneratorComponent(ref int __result, PowerSystem __instance)
        {
            if (__instance.genPool[__result].wind)
            {
                __instance.genPool[__result].genEnergyPerTick = 100_000_000_0000;
            }
        }
    }
}
