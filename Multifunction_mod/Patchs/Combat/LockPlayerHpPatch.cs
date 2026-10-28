using HarmonyLib;
using static Multifunction_mod.Multifunction;

namespace Multifunction_mod.Patchs.Combat
{
    /// <summary>
    /// 玩家锁血：机甲不受伤，护盾始终保持满值。
    /// </summary>
    public class LockPlayerHpPatch
    {
        public static bool Enable
        {
            get { return PatchToggle<LockPlayerHpPatch>.Enable; }
            set { PatchToggle<LockPlayerHpPatch>.Enable = value; }
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(Mecha), "TakeDamage")]
        public static bool MechaTakeDamage(Mecha __instance)
        {
            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(Mecha), "UpdateCombatStats")]
        public static void MechaTakeUpdateCombatStats(Mecha __instance)
        {
            __instance.energyShieldEnergy = __instance.energyShieldCapacity;
        }
    }
}
