using System;
using UnityEngine;
using static Multifunction_mod.Multifunction;
using Random = System.Random;

namespace Multifunction_mod.GameModify
{
    /// <summary>
    /// 战斗相关的游戏修改：点击生成黑雾中继站、点击击杀敌人（含基地核心连锁）。
    /// </summary>
    public static class CombatModify
    {
        /// <summary>
        /// 处理面板打开时也会响应的鼠标战斗操作（左右键点击世界）
        /// </summary>
        public static void CombatFunction()
        {
            if (Input.GetMouseButtonUp(0) || (ClickGenerateDFRelayLockClick && Input.GetMouseButtonDown(0)))
            {
                ClickGenerateDFRelayLockClick = false;
            }
            if (ClickGenerateDFRelay && Input.GetMouseButtonDown(0) && Physics.Raycast(GameCamera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit raycastHit, 800f, 8720, QueryTriggerInteraction.Collide))
            {
                ClickGenerateDFRelay = false;
                ClickGenerateDFRelayLockClick = true;
                VectorLF3 vectorLF2 = raycastHit.point.normalized * (GameMain.localPlanet.realRadius + 70f);
                var hive = GameMain.data.spaceSector.dfHives[GameMain.localStar.index];
                if (hive == null)
                {
                    return;
                }
                int minCount = hive.relayCount;
                for (EnemyDFHiveSystem enemyDFHiveSystem = GameMain.data.spaceSector.dfHives[GameMain.localStar.index]; enemyDFHiveSystem != null; enemyDFHiveSystem = enemyDFHiveSystem.nextSibling)
                {
                    if (enemyDFHiveSystem.isAlive && minCount > enemyDFHiveSystem.relayCount)
                    {
                        hive = enemyDFHiveSystem;
                    }
                }
                Random random = new Random(hive.seed);
                double num11 = random.NextDouble();
                double num12 = random.NextDouble();
                int enemyId = GameMain.data.spaceSector.CreateEnemyFinal(hive, 8116, GameMain.localPlanet.astroId, vectorLF2, Maths.SphericalRotation(vectorLF2, (float)num12 * 360f));

                int dfRelayId = hive.sector.enemyPool[enemyId].dfRelayId;

                DFRelayComponent dfrelayComponent = hive.relays.buffer[dfRelayId];
                var b = -(GameMain.localPlanet.veinBiasVector * 0.97f).normalized;

                if (dfrelayComponent != null)
                {
                    dfrelayComponent.SetDockIndex(hive.relayCount);
                    dfrelayComponent.hiveAstroId = hive.hiveAstroId;
                    dfrelayComponent.targetAstroId = GameMain.localPlanet.astroId;
                    dfrelayComponent.targetLPos = vectorLF2;
                    dfrelayComponent.targetYaw = (float)num12 * 360f;
                    dfrelayComponent.baseState = 1;
                    dfrelayComponent.baseId = 0;
                    double num20 = VectorLF3.Dot(vectorLF2.normalized, b);
                    num20 = Maths.Clamp01((num20 + 1.0) * 0.5);
                    num20 = Math.Pow(num20, 0.5);
                    dfrelayComponent.baseTicks = (int)(6400f * (float)(Math.Pow(num11, 2.0) * num20) + 200.5f);
                    dfrelayComponent.baseEvolve = hive.evolve;
                    dfrelayComponent.baseEvolve.threat = 0;
                    dfrelayComponent.baseEvolve.waves = 1;
                    dfrelayComponent.direction = 0;
                    dfrelayComponent.stage = 2;
                    int num21 = random.Next(180001) * 100;
                    int builderId = dfrelayComponent.builderId;
                    hive.builders.buffer[builderId].energy = hive.builders.buffer[builderId].maxEnergy + num21;
                    hive.sector.enemyAnimPool[enemyId].time = 1f;
                    hive.sector.enemyAnimPool[enemyId].state = 1U;
                    hive.sector.enemyAnimPool[enemyId].power = -1f;
                }

                hive.idleRelayCount = 0;
                for (int k = 0; k < hive.relayDocks.Length; k++)
                {
                    ref DFDock ptr = ref hive.relayDocks[k % hive.relayDocks.Length];
                    int num22 = hive.sector.CreateEnemyFinal(hive, 8116, hive.hiveAstroId, ptr.pos, ptr.rot);
                    int dfRelayId2 = hive.sector.enemyPool[num22].dfRelayId;
                    DFRelayComponent dfrelayComponent2 = hive.relays.buffer[dfRelayId2];
                    Assert.True(dfrelayComponent2 != null && dfRelayId2 > 0 && dfRelayId2 == dfrelayComponent2.id);
                    if (dfrelayComponent2 != null)
                    {
                        dfrelayComponent2.SetDockIndex(k);
                        hive.idleRelayIds[hive.idleRelayCount++] = dfRelayId2;
                    }
                }

            }
            else if (!ClickGenerateDFRelayLockClick && ClickKillEnemy.Value && ((Input.GetMouseButton(0) && ClickKillEnemyMode.Value == 1) || (Input.GetMouseButtonDown(0) && ClickKillEnemyMode.Value == 0)))
            {
                GameData data = GameMain.data;
                PlanetData localPlanet = data.localPlanet;
                PlanetFactory planetFactory = (localPlanet != null && localPlanet.factoryLoaded) ? localPlanet.factory : null;
                PlanetPhysics planetPhysics = (planetFactory != null) ? planetFactory.planet.physics : null;
                SpaceSector spaceSector = data.spaceSector;
                SectorPhysics physics = spaceSector.physics;

                EnemyData enemyData = default(EnemyData);
                EnemyDFGroundSystem enemyDFGroundSystem = null;
                EnemyDFHiveSystem enemyDFHiveSystem = null;
                if (planetPhysics != null)
                {
                    enemyData = planetPhysics.raycastLogic.castEnemy;
                    if (enemyData.id > 0)
                    {
                        enemyDFGroundSystem = planetFactory.enemySystem;
                    }
                }
                if (enemyDFGroundSystem == null && physics != null && physics.spaceColliderLogic.cursorCastAllCount > 0)
                {
                    RaycastData raycastData = physics.spaceColliderLogic.cursorCastAll[0];
                    if (raycastData.objType == EObjectType.Enemy && raycastData.objId > 0)
                    {
                        enemyData = spaceSector.enemyPool[raycastData.objId];
                        enemyDFGroundSystem = null;
                        enemyDFHiveSystem = spaceSector.GetHiveByAstroId(enemyData.originAstroId);
                    }
                }
                if (enemyData.id > 0 && enemyData.modelIndex > 0)
                {
                    if (enemyDFHiveSystem != null)
                    {
                        int dfSCoreId = enemyData.dfSCoreId;
                        if (dfSCoreId > 0 && ClickKillEnemyBaseChain)
                        {
                            var enemyPool = GameMain.data.spaceSector.enemyPool;
                            foreach (var enemy in enemyPool)
                            {
                                if (enemy.originAstroId == enemyDFHiveSystem.hiveAstroId)
                                {
                                    killEnemys.Enqueue(enemy);
                                }
                            }
                        }
                        killEnemys.Enqueue(enemyData);
                    }
                    else if (enemyDFGroundSystem != null)
                    {
                        var enemyPool = planetFactory.enemyPool;
                        var dfGBaseId = enemyData.dfGBaseId;
                        if (dfGBaseId > 0 && ClickKillEnemyBaseChain)
                        {
                            foreach (var enemy in enemyPool)
                            {
                                if (enemy.owner == dfGBaseId)
                                {
                                    killEnemys.Enqueue(enemy);
                                }
                            }
                        }
                        killEnemys.Enqueue(enemyData);
                    }
                }

            }
        }
    }
}
