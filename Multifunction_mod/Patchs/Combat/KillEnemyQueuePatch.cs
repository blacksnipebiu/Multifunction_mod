using HarmonyLib;
using static Multifunction_mod.Multifunction;

namespace Multifunction_mod.Patchs.Combat
{
    /// <summary>
    /// 点击杀死敌人：消费 killEnemys 队列，真正击杀队列中的敌人。
    /// </summary>
    public class KillEnemyQueuePatch
    {
        public static bool Enable
        {
            get { return PatchToggle<KillEnemyQueuePatch>.Enable; }
            set { PatchToggle<KillEnemyQueuePatch>.Enable = value; }
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(SkillSystem), "GameTick")]
        public static void SkillSystemGameTick(SkillSystem __instance)
        {
            if (killEnemys.Count > 0)
            {
                var enemydata = killEnemys.Dequeue();

                if (enemydata.id > 0)
                {
                    if (enemydata.combatStatId > 0 && __instance.combatStats.buffer[enemydata.combatStatId].id == enemydata.combatStatId)
                    {
                        __instance.combatStats.buffer[enemydata.combatStatId].hp = 0;
                    }
                    else if (enemydata.originAstroId > 1000000)
                    {
                        ref EnemyData reference = ref __instance.sector.enemyPool[enemydata.id];
                        __instance.sector.KillEnemyFinal(reference.id, ref CombatStat.empty);
                    }
                    else if (enemydata.originAstroId > 100 && enemydata.originAstroId <= 204899 && enemydata.originAstroId % 100 > 0)
                    {
                        PlanetFactory planetFactory = __instance.astroFactories[enemydata.originAstroId];
                        ref EnemyData reference = ref planetFactory.enemyPool[enemydata.id];
                        planetFactory.KillEnemyFinally(reference.id, ref CombatStat.empty);
                    }
                }
            }
        }
    }
}
