using HarmonyLib;
using static Multifunction_mod.Multifunction;

namespace Multifunction_mod.Patchs
{
    public class SpraycoaterComponentPatch
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(SpraycoaterComponent), "InternalUpdate")]
        public static void SpraycoaterComponentInternalUpdatePrefix(ref SpraycoaterComponent __instance)
        {
            if (!Maxproliferator.Value)
            {
                return;
            }
            Multifunctionpatch.ability = __instance.incAbility;
            __instance.incAbility = 10;
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(SpraycoaterComponent), "InternalUpdate")]
        public static void SpraycoaterComponentInternalUpdatePostfix(ref SpraycoaterComponent __instance)
        {
            if (Maxproliferator.Value || __instance.incAbility <= 4)
            {
                return;
            }
            __instance.incAbility = Multifunctionpatch.ability;
        }
    }
}
