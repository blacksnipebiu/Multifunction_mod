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
        private float miningSpeedScale;
        private float inserterStackCount;
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
                GameMain.mainPlayer.mecha.droneSpeed = value;
            }
        }

        /// <summary>
        /// 小飞机任务点数
        /// </summary>
        public float DroneMovement
        {
            get
            {
                return droneMovement;
            }

            set
            {
                if (value == 0 || value == droneMovement) return;
                droneMovement = value;
                if (!CheckCondition()) return;
                GameMain.mainPlayer.mecha.droneMovement = (int)value;
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
                GameMain.mainPlayer.mecha.droneCount = (int)value;
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
        /// 极速分拣器数量
        /// </summary>
        public float InserterStackCount
        {
            get
            {
                return inserterStackCount;
            }

            set
            {
                if (value == 0 || value == inserterStackCount) return;
                inserterStackCount = value;
                if (!CheckCondition()) return;
                GameMain.history.inserterStackCount = (int)value;
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

        public void SetContent(Mecha mecha, GameHistoryData historyData, int stackSizeMultiplier)
        {
            walkSpeed = mecha.walkSpeed;
            techSpeed = historyData.techSpeed;
            droneSpeed = mecha.droneSpeed;
            droneMovement = mecha.droneMovement;
            droneCount = mecha.droneCount;
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
            miningSpeedScale = historyData.miningSpeedScale;
            inserterStackCount = historyData.inserterStackCount;
            labLevel = historyData.labLevel;
            storageLevel = historyData.storageLevel;
            this.stackSizeMultiplier = stackSizeMultiplier;
        }

        bool CheckCondition()
        {
            return GameMain.mainPlayer != null && GameMain.mainPlayer.mecha != null && GameMain.history != null;
        }
    }
}
