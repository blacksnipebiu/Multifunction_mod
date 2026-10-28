using HarmonyLib;
using static Multifunction_mod.Multifunction;

namespace Multifunction_mod.Patchs.Combat
{
    /// <summary>
    /// 怪物锁血：敌方战斗单位的血量始终保持满值。
    /// </summary>
    public class LockEnemyHpPatch
    {
        public static bool Enable
        {
            get { return PatchToggle<LockEnemyHpPatch>.Enable; }
            set { PatchToggle<LockEnemyHpPatch>.Enable = value; }
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(ConstructionSystem), "GameTick")]
        public static void PlanetFactoryGameTick(ConstructionSystem __instance)
        {
            CombatStat[] buffer = __instance.factory.skillSystem.combatStats.buffer;
            foreach (CombatStat combatStat in buffer)
            {
                if (combatStat.objectType == 4)
                {
                    buffer[combatStat.id].hp = combatStat.hpMax;
                }
            }
        }
    }
}
