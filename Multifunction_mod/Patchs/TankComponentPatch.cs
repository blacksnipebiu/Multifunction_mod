using HarmonyLib;
using static Multifunction_mod.Multifunction;

namespace Multifunction_mod.Patchs
{
    public class TankComponentPatch
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(TankComponent), "GameTick")]
        public static void TankComponentGameTickPatchPrefix(ref TankComponent __instance)
        {
            if (!TankMaxproliferator.Value)
            {
                return;
            }
            __instance.fluidInc = __instance.fluidCount * incAbility;
        }

    }
}
