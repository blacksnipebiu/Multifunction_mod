namespace Multifunction_mod.Models
{
    public class PropertyData
    {
        private float walkSpeed;
        private float techSpeed;
        private float droneSpeed;
        private float droneMovement;
        private float droneCount;
        private float maxSailSpeed;
        private float maxWarpSpeed;
        private float buildArea;
        private float reactorPowerGen;
        private float logisticCourierSpeed;
        private float logisticDroneSpeedScale;
        private float logisticShipSpeedScale;
        private float logisticCourierCarries;
        private float logisticDroneCarries;
        private float logisticShipCarries;
        private float miningCostRate;
        private float miningSpeedScale;
        private float labLevel;
        private float storageLevel;
        private float stackSizeMultiplier;

        /// <summary>
        /// 走路速度
        /// </summary>
        public float WalkSpeed
        {
            get
            {
                return walkSpeed;
            }

            set
            {
                if (value == 0 || value == walkSpeed) return;
                walkSpeed = value;
                if (!CheckCondition()) return;
                GameMain.mainPlayer.mecha.walkSpeed = value;
            }
        }

        /// <summary>
        /// 研发速度
        /// </summary>
        public float TechSpeed
        {
            get
            {
                return techSpeed;
            }

            set
            {
                if (value == 0 || value == techSpeed) return;
                techSpeed = value;
                if (!CheckCondition()) return;
                GameMain.history.techSpeed = (int)value;
            }
        }

        /// <summary>
        /// 小飞机速度
        /// </summary>
        public float DroneSpeed
        {
            get
            {
                return droneSpeed;
            }

            set
            {
                if (value == 0 || value == droneSpeed) return;
                droneSpeed = value;
                if (!CheckCondition()) return;
                GameMain.history.constructionDroneSpeed = value;
            }
        }

        /// <summary>
        /// 小飞机数量
        /// </summary>
        public float DroneCount
        {
            get
            {
                return droneCount;
            }

            set
            {
                if (value == 0 || value == droneCount) return;
                droneCount = value;
                if (!CheckCondition()) return;
                int changedvalue = (int)(value - GameMain.mainPlayer.mecha.constructionModule.droneCount);
                GameMain.mainPlayer.mecha.constructionModule.droneCount += changedvalue;
                GameMain.mainPlayer.mecha.constructionModule.droneAliveCount += changedvalue;
                GameMain.mainPlayer.mecha.constructionModule.droneIdleCount += changedvalue;
            }
        }

        /// <summary>
        /// 最大航行速度
        /// </summary>
        public float MaxSailSpeed
        {
            get
            {
                return maxSailSpeed;
            }

            set
            {
                if (value == 0 || value == maxSailSpeed) return;
                maxSailSpeed = value;
                if (!CheckCondition()) return;
                GameMain.mainPlayer.mecha.maxSailSpeed = (int)value * 1000;
            }
        }

        /// <summary>
        /// 最大曲速
        /// </summary>
        public float MaxWarpSpeed
        {
            get
            {
                return maxWarpSpeed;
            }

            set
            {
                if (value == 0 || value == maxWarpSpeed) return;
                maxWarpSpeed = value;
                if (!CheckCondition()) return;
                GameMain.mainPlayer.mecha.maxWarpSpeed = (int)value * 1000 * 480;
            }
        }

        /// <summary>
        /// 建造范围
        /// </summary>
        public float BuildArea
        {
            get
            {
                return buildArea;
            }

            set
            {
                if (value == 0 || value == buildArea) return;
                buildArea = value;
                if (!CheckCondition()) return;
                GameMain.mainPlayer.mecha.buildArea = value;
            }
        }

        /// <summary>
        /// 核心功率
        /// </summary>
        public float ReactorPowerGen
        {
            get
            {
                return reactorPowerGen;
            }

            set
            {
                if (value == 0 || value == reactorPowerGen) return;
                reactorPowerGen = value;
                if (!CheckCondition()) return;
                GameMain.mainPlayer.mecha.reactorPowerGen = value;
            }
        }

        /// <summary>
        /// 配送机速度
        /// </summary>
        public float LogisticCourierSpeed
        {
            get
            {
                return logisticCourierSpeed;
            }

            set
            {
                if (value == 0 || value == logisticCourierSpeed) return;
                logisticCourierSpeed = value;
                if (!CheckCondition()) return;
                GameMain.history.logisticCourierSpeed = (int)value;
            }
        }

        /// <summary>
        /// 运输机速度
        /// </summary>
        public float LogisticDroneSpeedScale
        {
            get
            {
                return logisticDroneSpeedScale;
            }

            set
            {
                if (value == 0 || value == logisticDroneSpeedScale) return;
                logisticDroneSpeedScale = value;
                if (!CheckCondition()) return;
                GameMain.history.logisticDroneSpeedScale = value;
            }
        }

        /// <summary>
        /// 运输船速度
        /// </summary>
        public float LogisticShipSpeedScale
        {
            get
            {
                return logisticShipSpeedScale;
            }

            set
            {
                if (value == 0 || value == logisticShipSpeedScale) return;
                logisticShipSpeedScale = value;
                if (!CheckCondition()) return;
                GameMain.history.logisticShipSpeedScale = value;
            }
        }

        /// <summary>
        /// 配送机载量
        /// </summary>
        public float LogisticCourierCarries
        {
            get
            {
                return logisticCourierCarries;
            }

            set
            {
                if (value == 0 || value == logisticCourierCarries) return;
                logisticCourierCarries = value;
                if (!CheckCondition()) return;
                GameMain.history.logisticCourierCarries = (int)value;
            }
        }

        /// <summary>
        /// 运输机载量
        /// </summary>
        public float LogisticDroneCarries
        {
            get
            {
                return logisticDroneCarries;
            }

            set
            {
                if (value == 0 || value == logisticDroneCarries) return;
                logisticDroneCarries = (int)(value / 50) * 50;
                if (!CheckCondition()) return;
                GameMain.history.logisticDroneCarries = (int)value;
            }
        }

        /// <summary>
        /// 运输船载量
        /// </summary>
        public float LogisticShipCarries
        {
            get
            {
                return logisticShipCarries;
            }

            set
            {
                if (value == 0 || value == logisticShipCarries) return;
                logisticShipCarries = (int)(value / 500) * 500;
                if (!CheckCondition()) return;
                GameMain.history.logisticShipCarries = (int)value;
            }
        }

        public float MiningCostRate
        {
            get
            {
                return miningCostRate;
            }

            set
            {
                if (value == 0 || value == miningCostRate) return;
                miningCostRate = value;
                if (!CheckCondition()) return;
                GameMain.history.miningCostRate = value;
            }
        }

        /// <summary>
        /// 采矿机速度倍率
        /// </summary>
        public float MiningSpeedScale
        {
            get
            {
                return miningSpeedScale;
            }

            set
            {
                if (value == 0 || value == miningSpeedScale) return;
                miningSpeedScale = value;
                if (!CheckCondition()) return;
                GameMain.history.miningSpeedScale = value;
            }
        }

        /// <summary>
        /// 建筑堆叠高度
        /// </summary>
        public float LabLevel
        {
            get
            {
                return labLevel;
            }

            set
            {
                if (value == 0 || value == labLevel) return;
                labLevel = value;
                if (!CheckCondition()) return;
                GameMain.history.storageLevel = (int)value;
                GameMain.history.labLevel = (int)value;
            }
        }

        /// <summary>
        /// 货物集装数量
        /// </summary>
        public float StorageLevel
        {
            get
            {
                return storageLevel;
            }

            set
            {
                if (value == 0 || value == storageLevel) return;
                storageLevel = value;
                if (!CheckCondition()) return;
                GameMain.history.stationPilerLevel = (int)value;
            }
        }

        /// <summary>
        /// 物流背包堆叠倍率
        /// </summary>
        public float StackSizeMultiplier
        {
            get
            {
                return stackSizeMultiplier;
            }

            set
            {
                if (value == 0 || value == stackSizeMultiplier) return;
                stackSizeMultiplier = value;
                if (!CheckCondition()) return;
                GameMain.mainPlayer.deliveryPackage.stackSizeMultiplier = (int)value;
            }
        }

        private float kineticDamageScale;
        /// <summary>
        /// 动能伤害系数
        /// </summary>
        public float KineticDamageScale
        {
            get { return kineticDamageScale; }
            set
            {
                if (value == 0 || value == kineticDamageScale) return;
                kineticDamageScale = value;
                if (!CheckCondition()) return;
                GameMain.history.kineticDamageScale = value;
            }
        }
        /// <summary>
        /// 能量伤害系数
        /// </summary>
        private float energyDamageScale;
        public float EnergyDamageScale
        {
            get { return energyDamageScale; }
            set
            {
                if (value == 0 || value == energyDamageScale) return;
                energyDamageScale = value;
                if (!CheckCondition()) return;
                GameMain.history.energyDamageScale = value;
            }
        }
        /// <summary>
        /// 爆破伤害系数
        /// </summary>
        private float blastDamageScale;
        public float BlastDamageScale
        {
            get { return blastDamageScale; }
            set
            {
                if (value == 0 || value == blastDamageScale) return;
                blastDamageScale = value;
                if (!CheckCondition()) return;
                GameMain.history.blastDamageScale = value;
            }
        }

        /// <summary>
        /// 电磁伤害系数
        /// </summary>
        private float magneticDamageScale;

        public float MagneticDamageScale
        {
            get { return magneticDamageScale; }
            set
            {
                if (value == 0 || value == magneticDamageScale) return;
                magneticDamageScale = value;
                if (!CheckCondition()) return;
                GameMain.history.magneticDamageScale = value;
            }
        }

        private float globalHpEnhancement;

        public float GlobalHpEnhancement
        {
            get => globalHpEnhancement;
            set
            {
                if (value == 0 || value == globalHpEnhancement) return;
                globalHpEnhancement = value;
                if (!CheckCondition()) return;
                GameMain.history.globalHpEnhancement = value;
            }
        }


        private float energyShieldRadius;


        /// <summary>
        /// 玩家护盾半径
        /// </summary>
        public float EnergyShieldRadius
        {
            get { return energyShieldRadius; }
            set
            {
                if (value == 0 || value == energyShieldRadius) return;
                energyShieldRadius = value;
                if (!CheckCondition()) return;
                GameMain.mainPlayer.mecha.energyShieldRadius = value;
            }
        }

        private long energyShieldCapacity;
        /// <summary>
        /// 玩家护盾能量上限
        /// </summary>
        public long EnergyShieldCapacity
        {
            get { return energyShieldCapacity; }
            set
            {
                if (value == 0 || value == energyShieldCapacity) return;
                if (value > 990_000_000_000) value = 1_000_000_000_000;
                if (value < 1000) value = 0;
                energyShieldCapacity = value;
                if (!CheckCondition()) return;
                GameMain.mainPlayer.mecha.energyShieldCapacity = (long)value;
            }
        }

        private float combatDroneDamageRatio;

        /// <summary>
        /// 战斗飞机伤害倍率
        /// </summary>
        public float CombatDroneDamageRatio
        {
            get { return combatDroneDamageRatio; }
            set
            {
                if (value == 0 || value == combatDroneDamageRatio) return;
                combatDroneDamageRatio = value;
                if (!CheckCondition()) return;
                GameMain.history.combatDroneDamageRatio = value;
            }
        }

        private float combatDroneROFRatio;

        /// <summary>
        /// 战斗飞机攻击速度倍率
        /// </summary>
        public float CombatDroneROFRatio
        {
            get { return combatDroneROFRatio; }
            set
            {
                if (value == 0 || value == combatDroneROFRatio) return;
                combatDroneROFRatio = value;
                if (!CheckCondition()) return;
                GameMain.history.combatDroneROFRatio = value;
            }
        }

        private float combatDroneDurabilityRatio;

        /// <summary>
        /// 战斗飞机耐久度倍率
        /// </summary>
        public float CombatDroneDurabilityRatio
        {
            get { return combatDroneDurabilityRatio; }
            set
            {
                if (value == 0 || value == combatDroneDurabilityRatio) return;
                combatDroneDurabilityRatio = value;
                if (!CheckCondition()) return;
                GameMain.history.combatDroneDurabilityRatio = value;
            }
        }

        private float enemyDropScale;

        /// <summary>
        /// 残骸产出倍率
        /// </summary>
        public float EnemyDropScale
        {
            get { return enemyDropScale; }
            set
            {
                if (value == 0 || value == enemyDropScale) return;
                enemyDropScale = value;
                if (!CheckCondition()) return;
                GameMain.history.enemyDropScale = value;
            }
        }


        private float combatDroneSpeedRatio;

        /// <summary>
        /// 战斗飞机速度倍率
        /// </summary>
        public float CombatDroneSpeedRatio
        {
            get { return combatDroneSpeedRatio; }
            set
            {
                if (value == 0 || value == combatDroneSpeedRatio) return;
                combatDroneSpeedRatio = value;
                if (!CheckCondition()) return;
                GameMain.history.combatDroneSpeedRatio = value;
            }
        }


        private float combatShipSpeedRatio;


        /// <summary>
        /// 战斗飞船速度倍率
        /// </summary>
        public float CombatShipSpeedRatio
        {
            get { return combatShipSpeedRatio; }
            set
            {
                if (value == 0 || value == combatShipSpeedRatio) return;
                combatShipSpeedRatio = value;
                if (!CheckCondition()) return;
                GameMain.history.combatShipSpeedRatio = value;
            }
        }


        private float combatShipDamageRatio;

        /// <summary>
        /// 战斗飞船伤害倍率
        /// </summary>
        public float CombatShipDamageRatio
        {
            get { return combatShipDamageRatio; }
            set
            {
                if (value == 0 || value == combatShipDamageRatio) return;
                combatShipDamageRatio = value;
                if (!CheckCondition()) return;
                GameMain.history.combatShipDamageRatio = value;
            }
        }


        private float combatShipROFRatio;

        /// <summary>
        /// 战斗飞船攻击速度倍率
        /// </summary>
        public float CombatShipROFRatio
        {
            get { return combatShipROFRatio; }
            set
            {
                if (value == 0 || value == combatShipROFRatio) return;
                combatShipROFRatio = value;
                if (!CheckCondition()) return;
                GameMain.history.combatShipROFRatio = value;
            }
        }


        private float combatShipDurabilityRatio;

        /// <summary>
        /// 战斗飞船耐久度倍率
        /// </summary>
        public float CombatShipDurabilityRatio
        {
            get { return combatShipDurabilityRatio; }
            set
            {
                if (value == 0 || value == combatShipDurabilityRatio) return;
                combatShipDurabilityRatio = value;
                if (!CheckCondition()) return;
                GameMain.history.combatShipDurabilityRatio = value;
            }
        }


        private float planetaryATFieldEnergyRate;

        /// <summary>
        /// 星球防御盾能量倍率
        /// </summary>
        public float PlanetaryATFieldEnergyRate
        {
            get { return planetaryATFieldEnergyRate; }
            set
            {
                if (value == 0 || value == planetaryATFieldEnergyRate) return;
                planetaryATFieldEnergyRate = value;
                if (!CheckCondition()) return;
                GameMain.history.planetaryATFieldEnergyRate = (long)value * Configs.freeMode.planetaryATFieldEnergyRate;
            }
        }


        private int hpMaxUpgrade;

        /// <summary>
        /// 玩家额外血量
        /// </summary>
        public int HpMaxUpgrade
        {
            get { return hpMaxUpgrade; }
            set
            {
                if (value == 0 || value == hpMaxUpgrade) return;
                if (value < 100) value = 0;
                hpMaxUpgrade = value;
                if (!CheckCondition()) return;
                GameMain.mainPlayer.mecha.hpMaxUpgrade = (int)value * 100;
            }
        }

        private float laserAttackLocalRange;

        /// <summary>
        /// 机甲激光攻击范围
        /// </summary>
        public float LaserAttackLocalRange
        {
            get { return laserAttackLocalRange; }
            set
            {
                if (value == 0 || value == laserAttackLocalRange) return;
                laserAttackLocalRange = value;
                if (!CheckCondition()) return;
                GameMain.mainPlayer.mecha.laserLocalAttackRange = (int)value;
            }
        }

        private float laserAttackSpaceRange;

        /// <summary>
        /// 机甲激光太空攻击范围
        /// </summary>
        public float LaserAttackSpaceRange
        {
            get { return laserAttackSpaceRange; }
            set
            {
                if (value == 0 || value == laserAttackSpaceRange) return;
                laserAttackSpaceRange = value / 10 * 10;
                if (!CheckCondition()) return;
                GameMain.mainPlayer.mecha.laserSpaceAttackRange = (int)value;
            }
        }

        private float laserAttackDamage;
        /// <summary>
        /// 机甲激光伤害
        /// </summary>
        public float LaserAttackDamage
        {
            get { return laserAttackDamage; }
            set
            {
                if (value == 0 || value == laserAttackDamage) return;
                if (value < 5) value = 0;
                laserAttackDamage = value;
                if (!CheckCondition()) return;
                GameMain.mainPlayer.mecha.laserLocalDamage = (int)value * 100;
                GameMain.mainPlayer.mecha.laserSpaceDamage = (int)value * 100;
            }
        }

        private float laserEnergyCapacity;

        /// <summary>
        /// 机甲激光能量存储上限
        /// </summary>
        public float LaserEnergyCapacity
        {
            get { return laserEnergyCapacity; }
            set
            {
                if (value == 0 || value == laserEnergyCapacity) return;
                if (value < 1000) value = 0;
                laserEnergyCapacity = value;
                if (!CheckCondition()) return;
                GameMain.mainPlayer.mecha.laserEnergyCapacity = (int)value;
            }
        }

        private float laserEnergyCost;

        /// <summary>
        /// 机甲激光消耗能量
        /// </summary>
        public float LaserEnergyCost
        {
            get { return laserEnergyCost; }
            set
            {
                if (value == 0 || value == laserEnergyCost) return;
                if (value < 10) value = 0;
                laserEnergyCost = value;
                if (!CheckCondition()) return;
                GameMain.mainPlayer.mecha.laserLocalEnergyCost = (int)value;
                GameMain.mainPlayer.mecha.laserSpaceEnergyCost = (int)value;
            }
        }

        public void SetContent(Mecha mecha, GameHistoryData historyData, int stackSizeMultiplier)
        {
            walkSpeed = mecha.walkSpeed;
            techSpeed = historyData.techSpeed;
            droneSpeed = historyData.constructionDroneSpeed;
            droneMovement = historyData.constructionDroneMovement;
            droneCount = mecha.constructionModule.droneCount;
            maxSailSpeed = mecha.maxSailSpeed / 1000;
            maxWarpSpeed = mecha.maxWarpSpeed / 480000;
            buildArea = mecha.buildArea;
            reactorPowerGen = (float)mecha.reactorPowerGen;
            logisticCourierSpeed = historyData.logisticCourierSpeed > 0 ? historyData.logisticCourierSpeed : 6;
            logisticDroneSpeedScale = historyData.logisticDroneSpeedScale > 0 ? historyData.logisticDroneSpeedScale : 3;
            logisticShipSpeedScale = historyData.logisticShipSpeedScale > 0 ? historyData.logisticShipSpeedScale : 5;
            logisticCourierCarries = historyData.logisticCourierCarries > 0 ? historyData.logisticDroneCarries : 3;
            logisticDroneCarries = historyData.logisticDroneCarries > 0 ? historyData.logisticDroneCarries : 100;
            logisticShipCarries = historyData.logisticShipCarries > 0 ? historyData.logisticShipCarries : 100;
            miningCostRate = historyData.miningCostRate;
            miningSpeedScale = historyData.miningSpeedScale;
            labLevel = historyData.labLevel;
            storageLevel = historyData.storageLevel;
            globalHpEnhancement = historyData.globalHpEnhancement;
            this.stackSizeMultiplier = stackSizeMultiplier;


            kineticDamageScale = historyData.kineticDamageScale;
            energyDamageScale = historyData.energyDamageScale;
            blastDamageScale = historyData.blastDamageScale;
            magneticDamageScale = historyData.magneticDamageScale;

            energyShieldRadius = mecha.energyShieldRadius;
            energyShieldCapacity = mecha.energyShieldCapacity;
            hpMaxUpgrade = mecha.hpMaxUpgrade / 100;
            laserAttackLocalRange = mecha.laserLocalAttackRange;
            laserAttackSpaceRange = mecha.laserSpaceAttackRange;
            laserAttackDamage = mecha.laserLocalDamage / 100;
            laserEnergyCapacity = mecha.laserEnergyCapacity;
            laserEnergyCost = mecha.laserLocalEnergyCost;

            combatDroneDamageRatio = historyData.combatDroneDamageRatio;
            combatDroneROFRatio = historyData.combatDroneROFRatio;
            combatDroneDurabilityRatio = historyData.combatDroneDurabilityRatio;
            combatDroneSpeedRatio = historyData.combatDroneSpeedRatio;
            enemyDropScale = historyData.enemyDropScale;

            combatShipDamageRatio = historyData.combatShipDamageRatio;
            combatShipROFRatio = historyData.combatShipROFRatio;
            combatShipDurabilityRatio = historyData.combatShipDurabilityRatio;
            combatShipSpeedRatio = historyData.combatShipSpeedRatio;

            planetaryATFieldEnergyRate = historyData.planetaryATFieldEnergyRate / Configs.freeMode.planetaryATFieldEnergyRate;
        }

        bool CheckCondition()
        {
            return GameMain.mainPlayer != null && GameMain.mainPlayer.mecha != null && GameMain.history != null;
        }
    }
}
