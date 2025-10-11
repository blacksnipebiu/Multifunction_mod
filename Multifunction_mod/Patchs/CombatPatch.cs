using HarmonyLib;
using System;
using static Multifunction_mod.Multifunction;

namespace Multifunction_mod.Patchs
{
    public class CombatPatch
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(Mecha), "TakeDamage")]
        public static bool MechaTakeDamage(Mecha __instance)
        {
            if (LockPlayerHp.Value)
            {
                return false;
            }

            return true;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(Mecha), "UpdateCombatStats")]
        public static void MechaTakeUpdateCombatStats(Mecha __instance)
        {
            if (LockPlayerHp.Value)
            {
                __instance.energyShieldEnergy = __instance.energyShieldCapacity;
            }
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(ConstructionSystem), "GameTick")]
        public static void PlanetFactoryGameTick(ConstructionSystem __instance)
        {
            if (LockBuildHp.Value || LockEnemyHp.Value)
            {
                CombatStat[] buffer = __instance.factory.skillSystem.combatStats.buffer;
                foreach (CombatStat combatStat in buffer)
                {
                    if (LockBuildHp.Value && combatStat.objectType == 0)
                    {
                        buffer[combatStat.id].hp = combatStat.hpMax;
                    }
                    if (LockEnemyHp.Value && combatStat.objectType == 4)
                    {
                        buffer[combatStat.id].hp = combatStat.hpMax;
                    }
                }
            }
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(Mecha), "TickLaserFireCondition")]
        public static void MechaTickLaserFireCondition(Mecha __instance)
        {
            if (PlayerQuickAttack.Value)
            {
                __instance.laserFire = 0;
                __instance.laserEnergy = __instance.laserEnergyCapacity;
            }
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(Mecha), "TestLaserTurretEnergy")]
        public static bool MechaTestLaserTurretEnergy(ref bool __result)
        {
            if (PlayerQuickAttack.Value)
            {
                __result = true;
                return false;
            }
            return true;
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

        [HarmonyPrefix]
        [HarmonyPatch(typeof(EnemyDFHiveSystem), "GameTickLogic")]
        public static void EnemyDFHiveSystemPreGameTickLogic(EnemyDFHiveSystem __instance)
        {
            if (SpaceAlwaysClearThreat.Value)
            {
                __instance.evolve.threat = 0;
                __instance.evolve.threatshr = 0;
            }
            if (SpaceAlwaysMaxThreat.Value && __instance.evolve.threat < __instance.evolve.maxThreat && __instance.evolve.waveTicks == 0)
            {
                if (GameMain.localStar != null && __instance.starData.index == GameMain.localStar.index)
                {
                    __instance.evolve.threat = __instance.evolve.maxThreat * 2 + 1;
                    __instance.evolve.threatshr = __instance.evolve.maxThreat * 2 + 1;
                }
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(EnemyDFHiveSystem), "GameTickLogic")]
        public static void EnemyDFHiveSystemGameTickLogic(EnemyDFHiveSystem __instance)
        {
            if (SpaceAlwaysClearThreat.Value)
            {
                __instance.evolve.threat = 0;
                __instance.evolve.threatshr = 0;
            }
            if (SpaceAlwaysMaxThreat.Value && __instance.evolve.threat < __instance.evolve.maxThreat && __instance.evolve.waveTicks == 0)
            {
                if (GameMain.localStar != null && __instance.starData.index == GameMain.localStar.index)
                {
                    __instance.evolve.threat = __instance.evolve.maxThreat * 2 + 1;
                    __instance.evolve.threatshr = __instance.evolve.maxThreat * 2 + 1;
                }
            }

            if (PlayerFakeDeath.Value && __instance.isLocal)
            {
                __instance.local_player_exist_alive = false;
                __instance.local_player_in_range = false;
            }
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(DFGBaseComponent), "UpdateFactoryThreat")]
        public static void DFGBaseComponentPreLogicTick(DFGBaseComponent __instance, ref float power_threat_factor, ref float player_threat)
        {
            if (LocalAlwaysClearThreat.Value)
            {
                power_threat_factor = 0;
                player_threat = 0;
                __instance.evolve.threat = 0;
                __instance.evolve.threatshr = 0;
            }
            if (LocalAlwaysMaxThreat.Value && __instance.evolve.threat < __instance.evolve.maxThreat && __instance.evolve.waveTicks == 0)
            {
                if (GameMain.localPlanet != null && __instance.groundSystem.factory.planet.index == GameMain.localPlanet.index)
                {
                    __instance.evolve.threat = __instance.evolve.maxThreat * 2 + 1;
                    __instance.evolve.threatshr = __instance.evolve.maxThreat * 2 + 1;
                }
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(DFGBaseComponent), "UpdateFactoryThreat")]
        public static void DFGBaseComponentLogicTick(DFGBaseComponent __instance, ref float power_threat_factor, ref float player_threat)
        {
            if (LocalAlwaysClearThreat.Value)
            {
                power_threat_factor = 0;
                player_threat = 0;
                __instance.evolve.threat = 0;
                __instance.evolve.threatshr = 0;
            }
            if (LocalAlwaysMaxThreat.Value && __instance.evolve.threat < __instance.evolve.maxThreat && __instance.evolve.waveTicks == 0)
            {
                if (GameMain.localPlanet != null && __instance.groundSystem.factory.planet.index == GameMain.localPlanet.index)
                {
                    __instance.evolve.threat = __instance.evolve.maxThreat * 2 + 1;
                    __instance.evolve.threatshr = __instance.evolve.maxThreat * 2 + 1;
                }
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(EnemyBuilderComponent), "LogicTick")]
        public static void EnemyBuilderComponentBuildLogic_Ground(EnemyBuilderComponent __instance)
        {
            if (DarkFogBuilderQuickBuild.Value)
            {
                __instance.energy = __instance.maxEnergy;
                __instance.matter = __instance.maxMatter;
                __instance.sp = __instance.spMax;
                __instance.buildCDTime = 0;
            }
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(TurretComponent), "InternalUpdate")]
        public static void TurretComponentInternalUpdate(ref TurretComponent __instance)
        {
            if (TurrentKeepSuperNoval.Value)
            {
                __instance.supernovaTick = 901;
                __instance.supernovaStrength = 30f;
            }
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(HatredList), "HateTarget", new Type[] { typeof(ETargetType), typeof(int), typeof(int), typeof(int), typeof(EHatredOperation), })]
        public static bool HatredListHateTarget(ETargetType type)
        {
            if (PlayerFakeDeath.Value && type == ETargetType.Player)
            {
                return false;
            }
            return true;
        }
        [HarmonyPostfix]
        [HarmonyPatch(typeof(EnemyDFGroundSystem), "GameTickLogic_Prepare")]
        public static void EnemyDFGroundSystemGameTickLogic(EnemyDFGroundSystem __instance)
        {
            if (PlayerFakeDeath.Value && __instance.isLocalLoaded)
            {
                __instance.local_player_exist_alive = false;
            }
        }

    }
}
