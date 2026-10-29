using System;
using System.Collections.Generic;
using Multifunction_mod.Models;
using Multifunction_mod.Utils;
using static Multifunction_mod.Multifunction;
using static StorageComponent;

namespace Multifunction_mod.GameModify
{
    /// <summary>
    /// 玩家/机甲相关的游戏修改：机甲速度、无人机消耗、背包、背包大小、机甲物流、玩家数据初始化。
    /// </summary>
    public static class PlayerModify
    {
        /// <summary>机甲速度类开关统一的上限值</summary>
        public const int MaxMechaSpeed = 100;

        /// <summary>无人机不耗能是否已修改（只有改过才恢复，未勾选时不写任何值）</summary>
        private static bool droneNoConsumeApplied;
        private static Mecha droneNoConsumeMecha;
        private static double droneNoConsumeSavedEjectEnergy;
        private static double droneNoConsumeSavedEnergyPerMeter;

        /// <summary>
        /// 设置机甲手搓速度（机甲制造MAX）
        /// </summary>
        public static void SetMechaReplicateSpeed(Player player, bool max)
        {
            Mecha mecha = player != null ? player.mecha : null;
            if (mecha == null)
            {
                return;
            }

            mecha.replicateSpeed = max ? MaxMechaSpeed : Configs.freeMode.mechaReplicateSpeed;
        }

        /// <summary>
        /// 设置机甲采矿速度（机甲采矿MAX）
        /// </summary>
        public static void SetMechaMiningSpeed(Player player, bool max)
        {
            Mecha mecha = player != null ? player.mecha : null;
            if (mecha == null)
            {
                return;
            }

            mecha.miningSpeed = max ? MaxMechaSpeed : Configs.freeMode.mechaMiningSpeed;
        }

        /// <summary>
        /// 设置无人机构建机是否消耗能量（小飞机不耗能）
        /// </summary>
        public static void SetDroneNoConsume(Player player, bool noEnergy)
        {
            Mecha mecha = player != null ? player.mecha : null;
            if (mecha == null)
            {
                return;
            }

            //换机甲/换存档后旧的"已修改"记录作废，避免把上一局的值写到新机甲上
            if (droneNoConsumeApplied && droneNoConsumeMecha != mecha)
            {
                droneNoConsumeApplied = false;
            }

            if (noEnergy)
            {
                if (!droneNoConsumeApplied)
                {
                    droneNoConsumeApplied = true;
                    droneNoConsumeMecha = mecha;
                    droneNoConsumeSavedEjectEnergy = mecha.droneEjectEnergy;
                    droneNoConsumeSavedEnergyPerMeter = mecha.droneEnergyPerMeter;
                }
                mecha.droneEjectEnergy = 0;
                mecha.droneEnergyPerMeter = 0;
            }
            else if (droneNoConsumeApplied)
            {
                //只有确实修改过才恢复，未勾选时不做任何写入
                droneNoConsumeApplied = false;
                mecha.droneEjectEnergy = droneNoConsumeSavedEjectEnergy;
                mecha.droneEnergyPerMeter = droneNoConsumeSavedEnergyPerMeter;
            }
        }

        /// <summary>
        /// 背包无限物品：把背包扩到能放下所有物品，并把每种物品补满
        /// </summary>
        public static void InfinitePackage(Player player)
        {
            if (player == null || player.package == null || LDB.items == null || player.packageColCount <= 0)
            {
                return;
            }

            if (player.package.grids.Length < LDB.items.dataArray.Length)
            {
                player.package.SetSize((LDB.items.dataArray.Length / player.packageColCount + 1) * player.packageColCount);
            }

            //SetSize 会换掉 grids 数组，必须重新取一次
            StorageComponent.GRID[] grids = player.package.grids;
            int i = 0;
            bool changed = false;
            foreach (ItemProto item in LDB.items.dataArray)
            {
                //1099 是沙土之类的特殊物品，保持原样
                if (item.ID == 1099)
                {
                    continue;
                }

                int stackCount = StorageComponent.itemStackCount[item.ID];
                if (grids[i].itemId != item.ID || grids[i].count != stackCount || grids[i].stackSize != stackCount)
                {
                    changed = true;
                    grids[i].itemId = item.ID;
                    grids[i].stackSize = stackCount;
                    grids[i].count = stackCount;
                }

                i++;
                if (i == grids.Length)
                {
                    break;
                }
            }

            if (changed)
            {
                player.package.NotifyStorageChange();
            }
        }

        /// <summary>
        /// 背包中键锁定：把带过滤器的格子补满
        /// </summary>
        public static void InfiniteFilteredPackage(Player player)
        {
            if (player == null || player.package == null)
            {
                return;
            }

            StorageComponent.GRID[] grids = player.package.grids;
            bool changed = false;
            for (int i = 0; i < grids.Length; i++)
            {
                if (grids[i].itemId > 0 && grids[i].filter > 0 && grids[i].count != grids[i].stackSize)
                {
                    changed = true;
                    grids[i].count = grids[i].stackSize;
                }
            }

            if (changed)
            {
                player.package.NotifyStorageChange();
            }
        }

        /// <summary>
        /// 清空背包
        /// </summary>
        public static void ClearPackage(Player player)
        {
            if (player == null)
            {
                return;
            }

            for (int i = 0; i < player.package.grids.Length; i++)
            {
                player.package.grids[i].count = 0;
                if (player.package.grids[i].filter == 0)
                {
                    player.package.grids[i].itemId = 0;
                    player.package.grids[i].stackSize = 0;
                }
            }
            player.package.NotifyStorageChange();
        }

        /// <summary>
        /// 控制背包大小（加/减行、加/减列）
        /// </summary>
        /// <param name="player">玩家</param>
        /// <param name="rownum">行数变化</param>
        /// <param name="colnum">列数变化</param>
        /// <param name="add">true 增加，false 减少</param>
        public static void OperatePackageSize(Player player, int rownum, int colnum, bool add = true)
        {
            if (player == null)
            {
                return;
            }

            int packageColCount = player.packageColCount;
            int packagerowNum = (player.package.size - 1) / packageColCount + 1;
            if (add)
            {
                if (rownum > 0)
                {
                    player.package.SetSize(player.package.size + rownum * packageColCount);
                }
                if (colnum > 0 && player.packageColCount < 30)
                {
                    player.packageColCount += colnum;
                    player.package.SetSize(packagerowNum * player.packageColCount, packageColCount, player.packageColCount);
                }
            }
            else
            {
                if (rownum > 0)
                {
                    player.package.SetSize(player.package.size - rownum * packageColCount);
                }
                if (colnum > 0 && player.packageColCount > 10)
                {
                    player.packageColCount -= colnum;
                    int newsize = player.package.size - packagerowNum * colnum;
                    GRID[] array = new GRID[newsize];
                    for (int i = 0; i < player.package.grids.Length; i++)
                    {
                        int num = i / packageColCount;
                        int num2 = i % packageColCount;
                        int num3 = num * player.packageColCount + num2;
                        if (num3 == newsize)
                        {
                            break;
                        }
                        array[num3] = player.package.grids[i];
                    }

                    player.package.grids = null;
                    player.package.grids = array;
                    player.package.size = newsize;
                    player.package.searchStart = 0;
                    player.package.lastFullItem = -1;
                    player.package.lastEmptyItem = -1;
                    player.package.NotifyStorageChange();
                    player.package.NotifyStorageSizeChange();
                }
            }
        }

        /// <summary>
        /// 初始化玩家数据：同步全局 player，并在需要时重建属性面板数据
        /// </summary>
        public static void PlayerDataInit()
        {
            if (Multifunction.refreshPlayerData || Multifunction.player == null || GameMain.mainPlayer != Multifunction.player)
            {
                Multifunction.refreshPlayerData = false;
                Multifunction.player = GameMain.mainPlayer;
                if (Multifunction.player == null)
                {
                    return;
                }
                Multifunction.propertyData = new PropertyData();
                Multifunction.propertyData.SetContent(Multifunction.player.mecha, GameMain.history, Multifunction.player.deliveryPackage.stackSizeMultiplier);
                if (Mechalogistics_bool.Value && !Multifunction.player.deliveryPackage.unlocked)
                {
                    Multifunction.player.deliveryPackage.unlocked = true;
                    Multifunction.player.deliveryPackage.colCount = 1;
                    Multifunction.player.deliveryPackage.NotifySizeChange();
                }
            }
        }

        /// <summary>
        /// 初始化玩家：把机甲/背包/物流/战斗等数值恢复成 freeMode 配置
        /// </summary>
        public static void InitPlayer(Player player)
        {
            if (player == null || player.mecha == null || player.mecha.constructionModule.droneIdleCount < player.mecha.constructionModule.droneAliveCount)
            {
                return;
            }

            Multifunction.refreshPlayerData = true;
            ModeConfig freeMode = Configs.freeMode;
            GameHistoryData historyData = GameMain.history;
            Mecha mecha = player.mecha;
            mecha.coreEnergyCap = freeMode.mechaCoreEnergyCap;
            mecha.coreLevel = freeMode.mechaCoreLevel;
            mecha.corePowerGen = freeMode.mechaCorePowerGen;
            mecha.miningSpeed = freeMode.mechaMiningSpeed;

            mecha.walkSpeed = freeMode.mechaWalkSpeed;
            mecha.jumpSpeed = freeMode.mechaJumpSpeed;
            mecha.walkPower = freeMode.mechaWalkPower;
            mecha.thrusterLevel = freeMode.mechaThrusterLevel;
            mecha.jumpEnergy = freeMode.mechaJumpEnergy;
            mecha.constructionModule.Setup(freeMode);
            mecha.maxSailSpeed = freeMode.mechaSailSpeedMax;
            mecha.maxWarpSpeed = freeMode.mechaWarpSpeedMax;
            mecha.buildArea = freeMode.mechaBuildArea;
            mecha.replicateSpeed = freeMode.mechaReplicateSpeed;
            mecha.reactorPowerGen = freeMode.mechaReactorPowerGen;

            mecha.thrustPowerPerAcc = freeMode.mechaThrustPowerPerAcc;
            mecha.warpKeepingPowerPerSpeed = freeMode.mechaWarpKeepingPowerPerSpeed;
            mecha.warpStartPowerPerSpeed = freeMode.mechaWarpStartPowerPerSpeed;
            mecha.miningPower = freeMode.mechaMiningPower;
            mecha.hpMaxUpgrade = 0;
            mecha.laserLocalAttackRange = freeMode.mechaLocalLaserAttackRange;
            mecha.laserSpaceAttackRange = freeMode.mechaSpaceLaserAttackRange;
            mecha.laserLocalDamage = freeMode.mechaLocalLaserDamage;
            mecha.laserEnergyCapacity = freeMode.mechaLaserEnergyCapacity;
            mecha.laserLocalEnergyCost = freeMode.mechaLocalLaserEnergyCost;
            mecha.laserSpaceEnergyCost = freeMode.mechaSpaceLaserEnergyCost;
            mecha.energyShieldRadius = freeMode.energyShieldRadius;
            mecha.energyShieldCapacity = freeMode.energyShieldCapacity;
            mecha.groundCombatModule.fleetCount = 0;
            mecha.spaceCombatModule.fleetCount = 0;

            player.deliveryPackage.colCount = 0;
            player.packageColCount = 10;
            player.deliveryPackage.stackSizeMultiplier = Configs.freeMode.deliveryPackageStackSizeMultiplier;
            int packagesize = freeMode.playerPackageSize;
            player.deliveryPackage.rowCount = (packagesize - 1) / 10 + 1;

            historyData.constructionDroneSpeed = freeMode.droneSpeed;
            historyData.globalHpEnhancement = 0;
            historyData.constructionDroneMovement = freeMode.droneMovement;
            historyData.stationPilerLevel = 1;
            historyData.logisticDroneCarries = freeMode.logisticDroneCarries;
            historyData.logisticDroneSpeed = freeMode.logisticDroneSpeed;
            historyData.logisticShipCarries = freeMode.logisticShipCarries;
            historyData.logisticShipSailSpeed = freeMode.logisticShipSailSpeed;
            historyData.logisticShipWarpSpeed = freeMode.logisticShipWarpSpeed;
            historyData.logisticCourierSpeed = freeMode.logisticCourierSpeed;
            historyData.logisticCourierCarries = freeMode.logisticCourierCarries;
            historyData.dispenserDeliveryMaxAngle = freeMode.dispenserDeliveryMaxAngle;
            historyData.logisticShipSpeedScale = 1;
            historyData.logisticDroneSpeedScale = 1;
            historyData.logisticCourierSpeedScale = 1;
            historyData.inserterStackInput = 2;
            historyData.logisticShipWarpDrive = freeMode.logisticShipWarpDrive;
            historyData.miningCostRate = freeMode.miningCostRate;
            historyData.miningSpeedScale = freeMode.miningSpeedScale;
            historyData.storageLevel = 2;
            historyData.labLevel = 3;
            historyData.enemyDropScale = 1;
            historyData.techSpeed = freeMode.techSpeed;
            historyData.dysonNodeLatitude = 0;
            historyData.solarEnergyLossRate = 0.7f;
            historyData.solarSailLife = freeMode.solarSailLife;
            historyData.localStationExtraStorage = 0;
            historyData.remoteStationExtraStorage = 0;
            historyData.autoReconstructSpeed = 0;

            historyData.kineticDamageScale = 1;
            historyData.energyDamageScale = 1;
            historyData.blastDamageScale = 1;
            historyData.magneticDamageScale = 1;
            historyData.combatDroneDamageRatio = 1;
            historyData.combatDroneROFRatio = 1;
            historyData.combatDroneDurabilityRatio = 1;
            historyData.combatDroneSpeedRatio = 1;
            historyData.combatShipSpeedRatio = 1;
            historyData.combatShipDamageRatio = 1;
            historyData.combatShipROFRatio = 1;
            historyData.combatShipDurabilityRatio = 1;
            historyData.planetaryATFieldEnergyRate = freeMode.planetaryATFieldEnergyRate;
            historyData.groundFleetPortCount = 0;
            historyData.spaceFleetPortCount = 0;

            GRID[] array = new GRID[player.package.size];
            Array.Copy(player.package.grids, array, player.package.size);

            player.package.SetSize(packagesize);
            foreach (TechProto tp in new List<TechProto>(LDB.techs.dataArray))
            {
                if (tp.Level < tp.MaxLevel)
                {
                    for (int i = tp.Level; i < GameMain.history.techStates[tp.ID].curLevel + (GameMain.history.techStates[tp.ID].curLevel >= tp.MaxLevel ? 1 : 0); i++)
                        for (int j = 0; j < tp.UnlockFunctions.Length; j++)
                        {
                            if (tp.UnlockFunctions[j] == 30) historyData.localStationExtraStorage += (int)tp.UnlockValues[j];
                            else if (tp.UnlockFunctions[j] == 31) historyData.remoteStationExtraStorage += (int)tp.UnlockValues[j];
                            else GameMain.history.UnlockTechFunction(tp.UnlockFunctions[j], tp.UnlockValues[j], i);
                        }
                }
                else
                {
                    if (!GameMain.history.techStates[tp.ID].unlocked)
                        continue;
                    for (int index = 0; index < tp.UnlockFunctions.Length; ++index)
                    {
                        GameMain.history.UnlockTechFunction(tp.UnlockFunctions[index], tp.UnlockValues[index], GameMain.history.techStates[tp.ID].maxLevel);
                    }
                }
            }

            int num = 0;
            for (int i = 0; i < array.Length; i++)
            {
                if (num == player.package.size)
                {
                    break;
                }
                if (array[i].count == 0 && array[i].filter == 0 && array[i].itemId == 0)
                {
                    continue;
                }
                player.package.grids[num++] = array[i];
            }
        }

        /// <summary>
        /// 机甲物流：按需求/回收数量在背包、物流站、储物仓之间搬运物品
        /// </summary>
        public static void MechaLogisticsMethod(Player player)
        {
            if (player == null || player.package == null)
            {
                return;
            }

            StorageComponent package = player.package;
            if (player.inhandItemId != 0)
            {
                return;
            }
            int priority = MechalogPlayerStorageFirst_bool.Value ? 1 : 0;
            for (int i = 0; i < player.deliveryPackage.grids.Length; i++)
            {
                var item = player.deliveryPackage.grids[i];
                int itemId = item.itemId;
                if (itemId == 0) continue;
                if (priority == 0 && MechalogBuildGotoPlayerStorageFirst_bool.Value && LDB.items.Select(itemId).CanBuild)
                {
                    priority = 1;
                }
                int count = item.count;
                int packagecount = player.packageUtility.GetPackageItemCount(itemId);
                int exsitcount = packagecount + count;
                if (item.requireCount > exsitcount)
                {
                    int remain = player.packageUtility.GetPackageItemCapacity(itemId);
                    int need = Math.Min(item.requireCount - exsitcount, remain);
                    int getcount = 0;
                    int inc = 0;
                    for (int j = 0; j < GameMain.data.factoryCount; j++)
                    {
                        var factory = GameMain.data.factories[j];
                        if (MechalogisticsPlanet_bool.Value && !factory.planet.displayName.Equals("机甲物流")) continue;
                        if (MechalogStationprovide_bool.Value && factory.transport?.stationPool != null)
                        {
                            foreach (StationComponent sc in factory.transport.stationPool)
                            {
                                if (sc == null || sc.storage == null) continue;
                                int temp = need;
                                int tempitemid = itemId;
                                sc.TakeItem(ref tempitemid, ref temp, out int tempinc);
                                getcount += temp;
                                need -= temp;
                                inc += tempinc;
                                if (getcount >= need) break;
                            }
                        }
                        if (MechalogStorageprovide_bool.Value && factory.factoryStorage != null)
                        {
                            if (getcount < need && factory.factoryStorage.tankPool != null)
                            {
                                foreach (TankComponent tc in factory.factoryStorage.tankPool)
                                {
                                    if (tc.id > 0 && tc.fluidId > 0 && tc.fluidCount > 0 && itemId == tc.fluidId)
                                    {
                                        int temp = tc.fluidCount > need ? need : tc.fluidCount;

                                        int num = (int)(tc.fluidInc / tc.fluidCount + 0.5) * temp;
                                        factory.factoryStorage.tankPool[tc.id].fluidCount -= temp;
                                        factory.factoryStorage.tankPool[tc.id].fluidInc -= num;

                                        inc += num;
                                        getcount += temp;
                                        need -= temp;
                                        if (getcount >= need) break;
                                    }
                                }
                            }
                            if (getcount < need && factory.factoryStorage.storagePool != null)
                            {
                                foreach (StorageComponent sc in factory.factoryStorage.storagePool)
                                {
                                    if (sc == null) continue;
                                    int temp = sc.TakeItem(itemId, need, out int tempinc);
                                    getcount += temp;
                                    inc += tempinc;
                                    need -= temp;
                                    if (getcount >= need) break;
                                }
                                if (getcount >= need) break;
                            }
                        }
                    }
                    player.packageUtility.AddItemToAllPackages(itemId, getcount, i, inc, out _, priority);
                }
                if (item.recycleCount < exsitcount)
                {
                    int recyclenum = exsitcount - item.recycleCount;
                    player.packageUtility.TakeItemFromAllPackages(i, ref itemId, ref recyclenum, out int remaininc, true);
                    if (itemId == 0 || recyclenum == 0) continue;

                    for (int j = 0; j < GameMain.data.factoryCount; j++)
                    {
                        var factory = GameMain.data.factories[j];
                        if (MechalogisticsPlanet_bool.Value && !factory.planet.displayName.Equals("机甲物流")) continue;
                        if (MechalogStationrecycle_bool.Value && factory.transport?.stationPool != null)
                        {
                            foreach (StationComponent sc in factory.transport.stationPool)
                            {
                                if (sc == null || sc.storage == null) continue;

                                for (int k = 0; k < sc.storage.Length; k++)
                                {
                                    if (sc.storage[k].itemId != itemId) continue;
                                    int remainStorageCount = sc.storage[k].max - sc.storage[k].count;
                                    int splitinc = SplitIncUtil.split_inc(ref recyclenum, ref remaininc, remainStorageCount);
                                    sc.storage[k].count = sc.storage[k].count + remainStorageCount;
                                    sc.storage[k].inc = sc.storage[k].inc + splitinc;
                                }
                                if (recyclenum == 0) break;
                            }
                        }
                        if (factory.transport != null && factory.transport.stationPool != null && MechalogStationrecycle_bool.Value)
                        {
                            if (recyclenum == 0) break;
                        }
                        if (MechalogStoragerecycle_bool.Value && factory.factoryStorage != null)
                        {
                            if (recyclenum > 0 && factory.factoryStorage.tankPool != null)
                            {
                                foreach (TankComponent tc in factory.factoryStorage.tankPool)
                                {
                                    if (tc.id <= 0 || tc.fluidId <= 0 || tc.fluidCount <= 0 || itemId != tc.fluidId)
                                    {
                                        continue;
                                    }
                                    if (tc.fluidCount > 10000)
                                    {
                                        factory.factoryStorage.tankPool[tc.id].fluidCount += recyclenum;
                                        factory.factoryStorage.tankPool[tc.id].fluidInc += remaininc;
                                        recyclenum = 0;
                                    }
                                    else
                                    {
                                        int temp = tc.fluidCapacity - tc.fluidCount > recyclenum ? recyclenum : tc.fluidCapacity - tc.fluidCount;
                                        int splitinc = SplitIncUtil.split_inc(ref recyclenum, ref remaininc, temp);
                                        factory.factoryStorage.tankPool[tc.id].fluidCount += temp;
                                        factory.factoryStorage.tankPool[tc.id].fluidInc += splitinc;
                                    }
                                    if (recyclenum == 0) break;
                                }
                            }
                            if (recyclenum > 0 && factory.factoryStorage.storagePool != null)
                            {
                                foreach (StorageComponent sc in factory.factoryStorage.storagePool)
                                {
                                    if (sc == null) continue;
                                    recyclenum -= sc.AddItem(itemId, recyclenum, remaininc, out int inc);
                                    remaininc -= inc;
                                    if (recyclenum == 0) break;
                                }
                            }
                        }
                    }
                    player.packageUtility.AddItemToAllPackages(itemId, recyclenum, i, remaininc, out _);
                }
            }
        }
    }
}
