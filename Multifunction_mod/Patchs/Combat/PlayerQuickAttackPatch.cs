using HarmonyLib;
using static Multifunction_mod.Multifunction;

namespace Multifunction_mod.Patchs.Combat
{
    /// <summary>
    /// 玩家攻速Max：机甲激光无冷却，炮塔能量检测始终通过。
    /// </summary>
    public class PlayerQuickAttackPatch
    {
        public static bool Enable
        {
            get { return PatchToggle<PlayerQuickAttackPatch>.Enable; }
            set { PatchToggle<PlayerQuickAttackPatch>.Enable = value; }
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(Mecha), "TickLaserFireCondition")]
        public static void MechaTickLaserFireCondition(Mecha __instance)
        {
            __instance.laserFire = 0;
            __instance.laserEnergy = __instance.laserEnergyCapacity;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(Mecha), "TestLaserTurretEnergy")]
        public static bool MechaTestLaserTurretEnergy(ref bool __result)
        {
            __result = true;
            return false;
        }
    }
}
