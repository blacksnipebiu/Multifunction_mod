using HarmonyLib;
using static Multifunction_mod.Multifunction;

namespace Multifunction_mod.Patchs
{
    public class FactorySystemPatch
    {

        [HarmonyPostfix]
        [HarmonyPatch(typeof(FactorySystem), "NewEjectorComponent")]
        public static void NewEjectorComponentPatch(ref int __result, FactorySystem __instance)
        {
            if (!quickEjector)
            {
                return;
            }
            __instance.ejectorPool[__result].bulletCount = int.MaxValue;
            __instance.ejectorPool[__result].bulletId = 1501;
            __instance.ejectorPool[__result].coldSpend = 5;
            __instance.ejectorPool[__result].chargeSpend = 4;
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(FactorySystem), "NewSiloComponent")]
        public static void NewSiloComponentPatch(ref int __result, FactorySystem __instance)
        {
            if (!quicksilo)
            {
                return;
            }
            __instance.siloPool[__result].bulletCount = int.MaxValue;
            __instance.siloPool[__result].bulletId = 1503;
            __instance.siloPool[__result].coldSpend = 40;
            __instance.siloPool[__result].chargeSpend = 80;

        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(FactoryStorage), "NewTankComponent")]
        public static void NewTankComponentPatch(ref int fCount)
        {
            if (!Infinitestoragetank.Value)
                return;
            fCount = int.MaxValue - 200;
        }
    }
}
