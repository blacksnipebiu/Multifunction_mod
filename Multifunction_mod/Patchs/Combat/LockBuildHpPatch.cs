using HarmonyLib;
using static Multifunction_mod.Multifunction;

namespace Multifunction_mod.Patchs.Combat
{
    /// <summary>
    /// 建筑锁血：建筑类战斗单位的血量始终保持满值。
    /// </summary>
    public class LockBuildHpPatch
    {
        public static bool Enable
        {
            get { return PatchToggle<LockBuildHpPatch>.Enable; }
            set { PatchToggle<LockBuildHpPatch>.Enable = value; }
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(ConstructionSystem), "GameTick")]
        public static void PlanetFactoryGameTick(ConstructionSystem __instance)
        {
            CombatStat[] buffer = __instance.factory.skillSystem.combatStats.buffer;
            foreach (CombatStat combatStat in buffer)
            {
                if (combatStat.objectType == 0)
                {
                    buffer[combatStat.id].hp = combatStat.hpMax;
                }
            }
        }
    }
}
