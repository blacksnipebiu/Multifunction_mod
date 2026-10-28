using HarmonyLib;
using static Multifunction_mod.Multifunction;

namespace Multifunction_mod.Patchs.Combat
{
    /// <summary>
    /// 炮塔永久超新星：炮塔的超新星进度与强度始终保持满值。
    /// </summary>
    public class TurretSupernovaPatch
    {
        public static bool Enable
        {
            get { return PatchToggle<TurretSupernovaPatch>.Enable; }
            set { PatchToggle<TurretSupernovaPatch>.Enable = value; }
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(TurretComponent), "InternalUpdate")]
        public static void TurretComponentInternalUpdate(ref TurretComponent __instance)
        {
            __instance.supernovaTick = 901;
            __instance.supernovaStrength = 30f;
        }
    }
}
