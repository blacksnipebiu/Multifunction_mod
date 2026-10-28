using HarmonyLib;
using static Multifunction_mod.Multifunction;

namespace Multifunction_mod.Patchs.Storage
{
    /// <summary>
    /// 储液站无限增产：按 incAbility 补满储液站的增产点数增量。
    /// </summary>
    public class TankMaxProliferatorPatch
    {
        public static bool Enable
        {
            get { return PatchToggle<TankMaxProliferatorPatch>.Enable; }
            set { PatchToggle<TankMaxProliferatorPatch>.Enable = value; }
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(TankComponent), "GameTick")]
        public static void TankComponentGameTickPatchPrefix(ref TankComponent __instance)
        {
            __instance.fluidInc = __instance.fluidCount * incAbility;
        }
    }
}
