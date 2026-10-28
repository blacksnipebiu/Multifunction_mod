using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using Multifunction_mod.GameModify;
using Multifunction_mod.Meta;
using Multifunction_mod.Models;
using Multifunction_mod.Patchs;
using Multifunction_mod.Patchs.Core;
using Multifunction_mod.Patchs.Dyson;
using Multifunction_mod.Patchs.UI;
using Multifunction_mod.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using static Multifunction_mod.Constant;
using static StorageComponent;
using Quaternion = UnityEngine.Quaternion;
using Random = System.Random;
using Vector3 = UnityEngine.Vector3;

namespace Multifunction_mod
{
    [BepInPlugin(ModInfo.GUID, ModInfo.NAME, ModInfo.VERSION)]
    public class Multifunction : BaseUnityPlugin
    {

        #region 临时变量


        public static bool IsEnglish;

        public int ReformType;
        public int ReformColor;
        public float OnesecondTime;
        public static GUIDraw guidraw;
        public static PropertyData propertyData;
        public static VeinControlProperty veinproperty;

        public static bool OneSecondTimeElapse;

        public static Player player;
        public static Queue<EnemyData> killEnemys = new Queue<EnemyData>();
        public static KeyboardShortcut tempShowWindow;
        private static int driftBuildingLevel;
        public static int DriftBuildingLevel
        {
            get => driftBuildingLevel;
            set
            {
                driftBuildingLevel = value;
                DriftBuildingHeight = 1 + DriftBuildingLevel * 0.0066f;
            }
        }
        public static float DriftBuildingHeight = 1;
        public static int incAbility = 4;

        private static bool LDBInitSave;
        public static bool refreshLDB;
        public static bool GameRunning;
        public static bool refreshPlayerData;
        public static bool changequapowerpertick;
        public static bool StationfullCount;
        public static bool ChangeQuickKey;
        public static bool ChangingQuickKey;
        public static bool blueprintPasteToolActive;
        public static bool DriftBuildings;
        public static bool Property9999999;
        public static bool alwaysemissiontemp;
        public static bool restorewater;
        public static bool quickEjector;
        public static bool quicksilo;
        public static bool ClickKillEnemyBaseChain;
        public static int temp;
        public static bool farconnectdistance;
        public static bool pasteanyway;
        public static bool PasteBuildAnyWay;
        public static bool closeallcollider;
        public static bool unlockpointtech;
        public static bool ClickGenerateDFRelay;
        public static bool ClickGenerateDFRelayLockClick;
        public string watertype = "";
        public static Dictionary<int, List<RecipeProto>> smeltRecipes = new Dictionary<int, List<RecipeProto>>();
        public static Dictionary<int, Dictionary<int, int>> Beltsignal = new Dictionary<int, Dictionary<int, int>>();
        public static Dictionary<int, Dictionary<int, int>> Beltsignalnumberoutput = new Dictionary<int, Dictionary<int, int>>();
        #endregion
        #region 配置菜单
        public static ConfigEntry<KeyboardShortcut> QuickKey;
        public static ConfigEntry<int> veinlines;
        public static ConfigEntry<int> scale;
        public static ConfigEntry<int> StackMultiple;
        public static ConfigEntry<int> StationStoExtra;
        public static ConfigEntry<int> Stationminenumber;
        public static ConfigEntry<int> Buildmaxlen;
        public static ConfigEntry<int> Quantumenergy;
        public static ConfigEntry<int> CuttingVeinNumbers;
        public static ConfigEntry<int> MaxOrbitRadiusConfig;
        public static ConfigEntry<int> Solarsailsabsorbeveryframe;
        public static ConfigEntry<int> StationMinerSmelterNum;
        public static ConfigEntry<int> ClickKillEnemyMode;
        public static ConfigEntry<Color> Textcolor;
        public static ConfigEntry<Color> mainWindowTextureColor_config;
        public static ConfigEntry<float> MainWindow_width;
        public static ConfigEntry<float> MainWindow_height;
        public static ConfigEntry<float> MainWindow_x_config;
        public static ConfigEntry<float> MainWindow_y_config;
        public static ConfigEntry<float> starquamaxpowerpertick;
        public static ConfigEntry<float> planetquamaxpowerpertick;
        public static ConfigEntry<bool> InfiniteSand;
        public static ConfigEntry<bool> isInstantItem;
        public static ConfigEntry<bool> WindturbinesUnlimitedEnergy;
        public static ConfigEntry<bool> Windturbinescovertheglobe;
        public static ConfigEntry<bool> NotTidyVein;
        public static ConfigEntry<bool> InspectDisNoLimit;
        public static ConfigEntry<bool> StationMaxproliferator;
        public static ConfigEntry<bool> Mechalogistics_bool;
        public static ConfigEntry<bool> MechalogBuildGotoPlayerStorageFirst_bool;
        public static ConfigEntry<bool> MechalogPlayerStorageFirst_bool;
        public static ConfigEntry<bool> MechalogStoragerecycle_bool;
        public static ConfigEntry<bool> MechalogStorageprovide_bool;
        public static ConfigEntry<bool> MechalogStationrecycle_bool;
        public static ConfigEntry<bool> MechalogStationprovide_bool;
        public static ConfigEntry<bool> Infinitestoragetank;
        public static ConfigEntry<bool> QuickHandcraft;
        public static ConfigEntry<bool> QuickPlayerMine;
        public static ConfigEntry<bool> QuickResearch;
        public static ConfigEntry<bool> TankMaxproliferator;
        public static ConfigEntry<bool> Quantumtransport_bool;
        public static ConfigEntry<bool> Quantumtransportbuild_bool;
        public static ConfigEntry<bool> Quantumtransportpdwarp_bool;
        public static ConfigEntry<bool> Quantumtransportstarwarp_bool;
        public static ConfigEntry<bool> needtrashsand;
        public static ConfigEntry<bool> InfineteStarPower;
        public static ConfigEntry<bool> PlanetPower_bool;
        public static ConfigEntry<bool> allhandcraft;
        public static ConfigEntry<bool> quickproduce;
        public static ConfigEntry<bool> blueprintpastenoneed_bool;
        public static ConfigEntry<bool> noneedwarp;
        public static ConfigEntry<bool> Infinitething;
        public static ConfigEntry<bool> Infiniteplayerpower;
        public static ConfigEntry<bool> EnableTp;
        public static ConfigEntry<bool> deleteveinbool;
        public static ConfigEntry<bool> MechalogisticsPlanet_bool;
        public static ConfigEntry<bool> StationMiner;
        public static ConfigEntry<bool> StationSprayer;
        public static ConfigEntry<bool> StationMinerSmelter;
        public static ConfigEntry<bool> dismantle_but_nobuild;
        public static ConfigEntry<bool> MiddleMouseLockGrid;
        public static ConfigEntry<bool> build_station_nocondition;
        public static ConfigEntry<bool> StationTrash;
        public static ConfigEntry<bool> DroneNoenergy_bool;
        public static ConfigEntry<bool> BuildNotime_bool;
        public static ConfigEntry<bool> Station_infiniteWarp_bool;
        public static ConfigEntry<bool> StationfullCount_bool;
        public static ConfigEntry<bool> Buildingnoconsume;
        public static ConfigEntry<bool> Stationfullenergy;
        public static ConfigEntry<bool> BeltSignalFunction;
        public static ConfigEntry<bool> Tankcontentall;
        public static ConfigEntry<bool> ArchitectMode;
        public static ConfigEntry<bool> quickabsorbsolar;
        public static ConfigEntry<bool> cancelsolarbullet;
        public static ConfigEntry<bool> QuickabortSwarm;
        public static ConfigEntry<bool> alwaysemission;
        public static ConfigEntry<bool> StationSpray;
        public static ConfigEntry<bool> Maxproliferator;
        public static ConfigEntry<bool> StationPowerGen;
        public static ConfigEntry<bool> ChangeDysonradius;
        public static ConfigEntry<bool> CloseUIAbnormalityTip;
        public static ConfigEntry<bool> IgnoreControlPanelLocalLimit;
        public static ConfigEntry<bool> QuantumtransportstationSupply;
        public static ConfigEntry<bool> QuantumtransportCollectorSupply;
        public static ConfigEntry<bool> QuantumtransportlabSupply;
        public static ConfigEntry<bool> QuantumtransportpowerSupply;
        public static ConfigEntry<bool> QuantumtransportassembleSupply;
        public static ConfigEntry<bool> QuantumtransportstationDemand;
        public static ConfigEntry<bool> LockPlayerHp;
        public static ConfigEntry<bool> LockBuildHp;
        public static ConfigEntry<bool> LockEnemyHp;
        public static ConfigEntry<bool> PlayerQuickAttack;
        public static ConfigEntry<bool> PlayerFakeDeath;
        public static ConfigEntry<bool> TurrentKeepSuperNoval;
        public static ConfigEntry<bool> TurrentKeepSuperNovalQuickKey;
        public static ConfigEntry<bool> ClickKillEnemy;
        public static ConfigEntry<bool> LocalAlwaysClearThreat;
        public static ConfigEntry<bool> SpaceAlwaysClearThreat;
        public static ConfigEntry<bool> LocalAlwaysMaxThreat;
        public static ConfigEntry<bool> SpaceAlwaysMaxThreat;
        public static ConfigEntry<bool> DarkFogBuilderQuickBuild;
        public static ConfigEntry<string> seedPlanetWater;
        public static ConfigEntry<string> AutoChangeStationName;
        public static ConfigEntry<bool> QuantumtransportminerDemand;
        public static ConfigEntry<bool> QuantumtransportsiloDemand;
        public static ConfigEntry<bool> QuantumtransportlabDemand;
        public static ConfigEntry<bool> QuantumtransportpowerDemand;
        public static ConfigEntry<bool> QuantumtransportassembleDemand;
        public static ConfigEntry<bool> ModifyDarkFogLevel;
        public static ConfigEntry<int> ModifyDarkFogLevelValue;
        #endregion
        void Start()
        {
            preparedraw();
            PlanetModify.Init();
            MultifunctionTranslate.regallTranslate();
            #region 初始化配置
            {
                Quantumtransport_bool = Config.Bind("量子传输站", "Quantumtransport_bool", false);
                Quantumtransportstarwarp_bool = Config.Bind("星系级翘曲全面供应", "Quantumtransportstarwarp_bool", false);
                Quantumtransportpdwarp_bool = Config.Bind("星球级翘曲全面供应", "Quantumtransportpdwarp_bool", false);
                Quantumtransportbuild_bool = Config.Bind("星球级材料供应", "Quantumtransportbuild_bool", true);
                QuantumtransportCollectorSupply = Config.Bind("采集器拿取", "QuantumtransportCollectorSupply", true);
                QuantumtransportstationSupply = Config.Bind("星球级物流站材料供应", "QuantumtransportstationSupply", true);
                QuantumtransportlabSupply = Config.Bind("星球级研究站材料供应", "QuantumtransportlabSupply", true);
                QuantumtransportpowerSupply = Config.Bind("星球级电力设备材料供应", "QuantumtransportpowerSupply", true);
                QuantumtransportassembleSupply = Config.Bind("星球级组装机材料供应", "QuantumtransportassembleSupply", true);
                QuantumtransportstationDemand = Config.Bind("星球级物流站材料拿取", "QuantumtransportstationDemand", true);
                QuantumtransportminerDemand = Config.Bind("星球级矿机材料拿取", "QuantumtransportminerDemand", true);
                QuantumtransportsiloDemand = Config.Bind("星球级发射井弹射器材料拿取", "QuantumtransportsiloDemand", true);
                QuantumtransportlabDemand = Config.Bind("星球级研究站材料拿取", "QuantumtransportlabDemand", true);
                QuantumtransportpowerDemand = Config.Bind("星球级电力设备材料拿取", "QuantumtransportpowerDemand", true);
                QuantumtransportassembleDemand = Config.Bind("星球级组装机材料拿取", "QuantumtransportassembleDemand", true);
                scale = Config.Bind("大小适配", "scale", 16);

                LockPlayerHp = Config.Bind("玩家锁血", "LockPlayerHp", false);
                LockBuildHp = Config.Bind("建筑锁血", "LockBuildHp", false);
                LockEnemyHp = Config.Bind("怪物锁血", "LockEnemyHp", false);
                PlayerQuickAttack = Config.Bind("玩家攻速Max", "PlayerQuickAttack", false);
                PlayerFakeDeath = Config.Bind("玩家假死", "PlayerFakeDeath", false);
                TurrentKeepSuperNoval = Config.Bind("炮塔永久超新星", "TurrentKeepSuperNoval", false);
                TurrentKeepSuperNovalQuickKey = Config.Bind("启用快捷键(ctrl+s)", "TurrentKeepSuperNovalQuickKey", false);
                LocalAlwaysClearThreat = Config.Bind("星球永远0威胁", "LocalAlwaysClearThreat", false);
                SpaceAlwaysClearThreat = Config.Bind("太空永远0威胁", "SpaceAlwaysClearThreat", false);
                LocalAlwaysMaxThreat = Config.Bind("玩家所在星球永远满威胁", "LocalAlwaysMaxThreat", false);
                SpaceAlwaysMaxThreat = Config.Bind("玩家所在星系永远满威胁", "SpaceAlwaysMaxThreat", false);
                DarkFogBuilderQuickBuild = Config.Bind("黑雾巢穴快速产怪", "DarkFogBuilderQuickBuild", false);
                ClickKillEnemy = Config.Bind("点击杀死敌人", "ClickKillEnemy", false);
                ClickKillEnemyMode = Config.Bind("点击杀死敌人模式", "ClickKillEnemyMode", 0);
                ModifyDarkFogLevel = Config.Bind("调整黑雾怪物等级", "ModifyDarkFogLevel", false);
                ModifyDarkFogLevelValue = Config.Bind("调整黑雾怪物等级数值", "ModifyDarkFogLevelValue", 15);

                ArchitectMode = Config.Bind("建筑师模式", "ArchitectMode", false);
                EnableTp = Config.Bind("启用传送", "EnableTp", false);
                Quantumenergy = Config.Bind("量子耗能", "Quantumenergy", 1000000);
                StationMinerSmelterNum = Config.Bind("星球熔炉矿机等价高级熔炉数量", "StationMinerSmelterNum", 120);
                StationfullCount_bool = Config.Bind("星球无限供货机", "StationfullCount_bool", false);
                StationMinerSmelter = Config.Bind("星球熔炉矿机", "StationMinerSmelter", false);
                StationSprayer = Config.Bind("喷涂加工厂", "StationSprayer", false);
                InfineteStarPower = Config.Bind("人造卫星无限能源", "InfineteStarPower", false);
                WindturbinesUnlimitedEnergy = Config.Bind("风力涡轮机无限能源", "WindturbinesUnlimitedEnergy", false);
                Windturbinescovertheglobe = Config.Bind("风力涡轮机覆盖全球", "Windturbinescovertheglobe", false);
                Infinitestoragetank = Config.Bind("无限储液站", "Infinitestoragetank", false);
                TankMaxproliferator = Config.Bind("储液站无限增产", "TankMaxproliferator", false);
                Tankcontentall = Config.Bind("储液站任意存", "Tankcontentall", false);
                allhandcraft = Config.Bind("全部手搓", "allhandcraft", false);
                quickproduce = Config.Bind("快速生产", "quickproduce", false);
                noneedwarp = Config.Bind("无翘曲器曲速", "noneedwarp", false);
                isInstantItem = Config.Bind("直接获取物品", "isInstantItem", false);
                QuickResearch = Config.Bind("鼠标中键锁定格子", "QuickResearch", false);
                MiddleMouseLockGrid = Config.Bind("鼠标中键锁定格子", "MiddleMouseLockGrid", false);

                CuttingVeinNumbers = Config.Bind("切割矿脉数量", "CuttingVeinNumbers", 3);
                veinlines = Config.Bind("矿物行数", "veinlines", 3);
                NotTidyVein = Config.Bind("矿堆不整理", "NotTidyVein", false);
                deleteveinbool = Config.Bind("删除矿物", "deleteveinbool", false);
                StationMaxproliferator = Config.Bind("物流站无限增产点数", "StationMaxproliferator", false);
                BeltSignalFunction = Config.Bind("传送带信号功能", "BeltSignalFunction", false);
                Mechalogistics_bool = Config.Bind("机甲物流", "Mechalogistics_bool", false);
                MechalogisticsPlanet_bool = Config.Bind("机甲物流专用星", "MechalogisticsPlanet_bool", false);
                MechalogBuildGotoPlayerStorageFirst_bool = Config.Bind("建筑优先进入玩家背包", "MechalogBuildGotoPlayerStorageFirst_bool", false);
                MechalogPlayerStorageFirst_bool = Config.Bind("所有物品优先进入玩家背包", "MechalogPlayerStorageFirst_bool", false);
                MechalogStationprovide_bool = Config.Bind("需求使用物流站", "MechalogStationprovide_bool", false);
                MechalogStationrecycle_bool = Config.Bind("回收使用物流站", "MechalogStationrecycle_bool", false);
                MechalogStorageprovide_bool = Config.Bind("需求使用储物仓", "MechalogStorageprovide_bool", false);
                MechalogStoragerecycle_bool = Config.Bind("回收使用储物仓", "MechalogStoragerecycle_bool", false);
                Infinitething = Config.Bind("无限物品", "Infinitething", false);
                InfiniteSand = Config.Bind("无限沙土", "InfiniteSand", false);
                Infiniteplayerpower = Config.Bind("无限机甲能量", "Infiniteplayerpower", false);
                StationMiner = Config.Bind("星球矿机", "stationmineropen", false);
                StationTrash = Config.Bind("星球垃圾箱", "stationtrashopen", false);
                Buildingnoconsume = Config.Bind("全设备不耗电", "Buildingnoconsume", false);
                Stationfullenergy = Config.Bind("物流站永久满电", "Stationfullenergy", false);
                StationSpray = Config.Bind("物流站喷涂", "StationSpray", false);
                StationPowerGen = Config.Bind("物流站内置发电", "StationPowerGen", false);
                build_station_nocondition = Config.Bind("建造无需条件", "build_station_nocondition", false);
                QuickHandcraft = Config.Bind("机甲制造MAX", "QuickHandcraft", false);
                QuickPlayerMine = Config.Bind("机甲采矿Max", "QuickPlayerMine", false);
                InspectDisNoLimit = Config.Bind("操作范围不受限制", "InspectDisNoLimit", false);
                needtrashsand = Config.Bind("需要垃圾沙土", "needtrashsand", false);
                dismantle_but_nobuild = Config.Bind("拆除不添加至背包", "dismantle_but_nobuild", false);
                DroneNoenergy_bool = Config.Bind("小飞机不耗能", "DroneNoenergy_bool", false);
                Station_infiniteWarp_bool = Config.Bind("星际运输站无限曲速", "Station_infiniteWarp_bool", false);
                BuildNotime_bool = Config.Bind("建筑秒完成", "BuildNotime_bool", false);
                PlanetPower_bool = Config.Bind("星球电网", "PlanetPower_bool", false);
                blueprintpastenoneed_bool = Config.Bind("蓝图建造无需材料", "blueprintpastenoneed_bool", false);
                quickabsorbsolar = Config.Bind("跳过太阳帆吸收阶段", "quickabsorbsolar", false);
                cancelsolarbullet = Config.Bind("跳过太阳帆子弹阶段", "cancelsolarbullet", false);
                alwaysemission = Config.Bind("全球打帆", "alwaysemission", false);
                QuickabortSwarm = Config.Bind("太阳帆秒吸收", "QuickabortSwarm", false);
                ChangeDysonradius = Config.Bind("改变戴森球半径上下限", "ChangeDysonradius", false);
                MaxOrbitRadiusConfig = Config.Bind("戴森球最大半径", "MaxOrbitRadiusConfig", 1000000);
                StationStoExtra = Config.Bind("运输站额外储量", "StationStoExtra", 0);
                StackMultiple = Config.Bind("堆叠倍数", "StackMultiple", 1);
                Stationminenumber = Config.Bind("星球矿机速率", "Stationminenumber", 1);
                Buildmaxlen = Config.Bind("建筑数量最大值", "Buildmaxlen", 15);
                Maxproliferator = Config.Bind("增产点数上限10", "Maxproliferator", false);
                starquamaxpowerpertick = Config.Bind("实时修改星球量子充电功率", "starquamaxpowerpertick", 60f);
                planetquamaxpowerpertick = Config.Bind("实时修改星系量子充电功率", "planetquamaxpowerpertick", 60f);
                CloseUIAbnormalityTip = Config.Bind("关闭异常提示", "CloseUIAbnormalityTip", false);
                Textcolor = Config.Bind("字体颜色", "Textcolor", Color.white);
                mainWindowTextureColor_config = Config.Bind("窗口材质颜色", "mainWindowTextureColor", Color.black);
                seedPlanetWater = Config.Bind("海洋类型修改", "seedPlanetWater", "");
                AutoChangeStationName = Config.Bind("物流站自动改名", "AutoChangeStationName", "");
                Solarsailsabsorbeveryframe = Config.Bind("每帧吸收个数", "Solarsailsabsorbeveryframe", 1);
                IgnoreControlPanelLocalLimit = Config.Bind("取消总控面板本地限制", "IgnoreControlPanelLocalLimit", false);


                MainWindow_x_config = Config.Bind("第一窗口x", "xl_SimpleUI_1_x_config", 448.0f);
                MainWindow_y_config = Config.Bind("第一窗口y", "xl_SimpleUI_1_y_config", 199.0f);
                MainWindow_width = Config.Bind("第一窗口宽度", "xl_SimpleUI_1_x_config", 1400.0f);
                MainWindow_height = Config.Bind("第一窗口高度", "xl_SimpleUI_1_y_config", 1000.0f);
                QuickKey = Config.Bind("打开窗口快捷键", "Key", new BepInEx.Configuration.KeyboardShortcut(KeyCode.LeftAlt, KeyCode.Alpha1));
            }
            #endregion
            if (QuickKey.Value.MainKey == KeyCode.Alpha1 && QuickKey.Value.Modifiers.Count() == 1 && QuickKey.Value.Modifiers.ElementAt(0) == KeyCode.LeftAlt)
            {
                QuickKey.Value = new KeyboardShortcut(KeyCode.LeftAlt, KeyCode.Alpha1);
            }
            AssetBundle assetBundle = AssetBundle.LoadFromStream(Assembly.GetExecutingAssembly().GetManifestResourceStream("Multifunction_mod.multifunctionpanel"));
            var MultiFunctionPanel = assetBundle.LoadAsset<GameObject>("MultiFunctionPanel");
            var temppanel = Instantiate<GameObject>(MultiFunctionPanel, UIRoot.instance.overlayCanvas.transform);
            temppanel.SetActive(false);
            guidraw = new GUIDraw(Math.Max(5, Math.Min(scale.Value, 35)), temppanel, this);
            veinproperty = new VeinControlProperty
            {
                VeinLines = veinlines.Value,
                CuttingVeinNumbers = CuttingVeinNumbers.Value,
                oillowerlimit = (int)(0.1f / VeinData.oilSpeedMultiplier),
                DeleteVein = deleteveinbool.Value,
                NotTidyVein = NotTidyVein.Value
            };
            incAbility = Maxproliferator.Value ? 10 : 4;
            tempShowWindow = QuickKey.Value;

            CollectorStation = new List<int>();
            PlanetModify.InitWaterTypesConfig();
            IgnoreControlPanelLocalLimit.SettingChanged += (_, _) =>
            {
                UIControlPanelPatch.Enable = IgnoreControlPanelLocalLimit.Value;
            };

            alwaysemission.SettingChanged += (_, _) =>
            {
                EjectAnywayPatch.Enable = alwaysemission.Value;
            };
            cancelsolarbullet.SettingChanged += (_, _) =>
            {
                SkipBulletPatch.Enable = cancelsolarbullet.Value;
            };
            SkipBulletPatch.Enable = cancelsolarbullet.Value;
            EjectAnywayPatch.Enable = alwaysemission.Value;
            UIControlPanelPatch.Enable = IgnoreControlPanelLocalLimit.Value;

            //用游戏生命周期事件代替每帧轮询 GameMain.instance.running
            GameLifecyclePatch.OnGameBegin += OnGameBeginEvent;
            GameLifecyclePatch.OnGameEnd += OnGameEndEvent;

            Multifunctionpatch.Patchallmethod();
        }
        int dockindex = -1;

        void Update()
        {
            if (GameRunning)
            {
                if (Time.time > OnesecondTime + 1)
                {
                    OnesecondTime = Time.time;
                    OneSecondTimeElapse = true;
                }
            }
            ChangeQuickKeyMethod();
            QuickKeyOpenWindow();
            AfterGameStart();

            if (Input.GetKeyDown(KeyCode.F9))
            {
                //UIRoot.instance.uiGame.controlPanelWindow.stationInspector.isLocal = true;
            }
        }

        void OnGUI()
        {
            IsEnglish = Localization.CurrentLanguage.glyph == 0;
            guidraw.Draw();
        }

        #region 量子传输站

        public static List<int> CollectorStation = new List<int>();
        public static List<int> SuperStation = new List<int>();
        public static List<int> StarSuperStation = new List<int>();
        public static Dictionary<int, int[]> StarSuperStationItemidstore = new Dictionary<int, int[]>();
        public static Dictionary<int, Dictionary<int, List<int[]>>> PlanetSuperStationItemidstore = new Dictionary<int, Dictionary<int, List<int[]>>>();
        public void RefreshSuperStation()
        {
            if (GameMain.data?.galacticTransport?.stationPool == null) return;
            StarSuperStation.Clear();
            SuperStation.Clear();
            StarSuperStationItemidstore.Clear();
            PlanetSuperStationItemidstore.Clear();
            for (int i = 0; i < GameMain.data.galacticTransport.stationPool.Length; i++)
            {
                StationComponent sc = GameMain.data.galacticTransport.stationPool[i];
                if (sc == null) continue;
                var pd = GameMain.galaxy.PlanetById(sc.planetId)?.factory;
                string stationComponentName = pd.ReadExtraInfoOnEntity(sc.entityId);
                if (string.IsNullOrEmpty(stationComponentName) || sc.storage == null || !sc.isStellar) continue;
                if (stationComponentName == "星系量子传输站")
                {
                    StarSuperStation.Add(i);
                    for (int j = 0; j < sc.storage.Length; j++)
                    {
                        ref StationStore store = ref sc.storage[j];
                        if (store.itemId > 0)
                        {
                            if (!StarSuperStationItemidstore.ContainsKey(store.itemId))
                            {
                                StarSuperStationItemidstore.Add(store.itemId, new int[] { StarSuperStation.Count - 1, j });
                            }
                            if (store.count < 0)
                                store.count = 0;
                            if (store.inc < 0)
                                store.inc = 0;
                            else if (store.inc > store.count * 4)
                            {
                                store.inc = store.count * 4;
                            }
                        }
                    }
                    if (changequapowerpertick)
                    {
                        try
                        {
                            GameMain.galaxy.PlanetById(sc.planetId).factory.powerSystem.consumerPool[sc.pcId].workEnergyPerTick = (long)starquamaxpowerpertick.Value * 16667;
                        }
                        catch { }
                    }
                }
                if (stationComponentName == "星球量子传输站")
                {
                    SuperStation.Add(i);
                    if (changequapowerpertick)
                    {
                        try
                        {
                            GameMain.galaxy.PlanetById(sc.planetId).factory.powerSystem.consumerPool[sc.pcId].workEnergyPerTick = (long)planetquamaxpowerpertick.Value * 16667;
                        }
                        catch { }
                    }

                }
            }
        }
        public void takeitemfromstarsuperstation()
        {
            if (GameMain.data?.galacticTransport?.stationPool == null) return;
            RefreshSuperStation();
            int SuperStationCount = SuperStation.Count;
            int StarSuperStationCount = StarSuperStation.Count;
            if (SuperStationCount == 0 && StarSuperStationCount == 0) return;
            changequapowerpertick = false;

            for (int j = 0; j < SuperStationCount; j++)
            {
                StationComponent sc = GameMain.data.galacticTransport.stationPool[SuperStation[j]];
                int pdID = sc.planetId;
                for (int i = sc.storage.Length - 1; i >= 0; i--)
                {
                    ref StationStore store = ref sc.storage[i];
                    int itemID = store.itemId;
                    if (itemID <= 0) continue;
                    if (!PlanetSuperStationItemidstore.ContainsKey(pdID))
                    {
                        PlanetSuperStationItemidstore.Add(pdID, new Dictionary<int, List<int[]>>());
                    }
                    var PlanetSuperStationstore = PlanetSuperStationItemidstore[pdID];
                    if (!PlanetSuperStationstore.ContainsKey(itemID))
                    {
                        PlanetSuperStationstore.Add(itemID, new List<int[]>() { new int[] { j, i } });
                    }
                    else
                    {
                        PlanetSuperStationstore[itemID].Add(new int[] { j, i });
                    }
                    if (store.remoteLogic == ELogisticStorage.Demand && store.count < store.max)
                    {
                        if (store.max > store.count)
                        {
                            int[] takeitem = doitemfromStarsuper(itemID, store.max - store.count);
                            store.count += takeitem[0];
                            store.inc += takeitem[1];
                        }
                        else if (store.max < store.count)
                        {
                            int[] takeitem = doitemfromStarsuper(itemID, store.count - store.max, store.inc, false);
                            store.count -= takeitem[0];
                            store.inc -= takeitem[1];
                        }
                    }
                    else if (store.remoteLogic == ELogisticStorage.Supply && store.count > 0)
                    {
                        int[] putitem = doitemfromStarsuper(itemID, store.count, store.inc, false);
                        store.count -= putitem[0];
                        store.inc -= putitem[1];
                    }
                    if (store.count < 0)
                        store.count = 0;
                    if (store.inc < 0)
                        store.inc = 0;
                }
                if (Quantumtransportpdwarp_bool.Value && sc.warperCount <= 5)
                    sc.warperCount += doitemfromStarsuper(1210, 50, 0)[0];
            }
            for (int j = 0; j < StarSuperStationCount; j++)
            {
                StationComponent sc = GameMain.data.galacticTransport.stationPool[StarSuperStation[j]];
                if (Quantumtransportstarwarp_bool.Value && sc.warperCount <= 5)
                    sc.warperCount += doitemfromStarsuper(1210, 50, 0)[0];

            }
            if (QuantumtransportCollectorSupply.Value)
                CollectorStationDemand();
            if (Quantumtransportbuild_bool.Value)
                SuperStationProvide();

        }
        public void StationSupplyDemand(FactorySystem fs, int pdid)
        {
            foreach (StationComponent sc in fs.factory.transport.stationPool)
            {
                if (sc?.storage == null) continue;
                string stationComponentName = fs.factory.ReadExtraInfoOnEntity(sc.entityId);
                if (stationComponentName == "星球量子传输站") continue;
                for (int i = 0; i < sc.storage.Length; i++)
                {
                    ref StationStore store = ref sc.storage[i];
                    if (store.itemId <= 0)
                        continue;
                    if (QuantumtransportstationDemand.Value)
                    {
                        if (store.localLogic == ELogisticStorage.Supply && store.count > 0 && stationComponentName != "星系量子传输站")
                        {
                            int[] takeitem = doitemfromPlanetSuper(store.itemId, store.count, pdid, store.inc, false);
                            store.count -= takeitem[0];
                            store.inc -= takeitem[1];
                        }
                    }

                    //物流站补货
                    if (QuantumtransportstationSupply.Value && store.localLogic == ELogisticStorage.Demand && store.count <= store.max)
                    {
                        int[] takeitem = doitemfromPlanetSuper(store.itemId, store.max - store.count, pdid);
                        store.count += takeitem[0];
                        store.inc += takeitem[1];
                    }
                }
            }
        }
        public void MinerSupplyDemand(FactorySystem fs, int pdid)
        {
            foreach (MinerComponent mc in fs.minerPool)
            {
                if (mc.id > 0 && mc.entityId > 0 && mc.productCount > 20 && mc.productId > 0)
                {
                    fs.minerPool[mc.id].productCount -= doitemfromPlanetSuper(mc.productId, mc.productCount, pdid, 0, false)[0];
                }
            }
        }
        public void SiloComponentSupplyDemand(FactorySystem fs, int pdid)
        {
            foreach (SiloComponent sc in fs.siloPool)
            {
                if (sc.id > 0 && sc.entityId > 0 && sc.bulletCount <= 1)
                {
                    int[] takeitem = doitemfromPlanetSuper(1503, 20, pdid);
                    fs.siloPool[sc.id].bulletCount += takeitem[0];
                    fs.siloPool[sc.id].bulletInc += takeitem[1];
                }
            }
            foreach (EjectorComponent ec in fs.ejectorPool)
            {
                if (ec.id > 0 && ec.entityId > 0 && ec.bulletCount <= 1)
                {
                    int[] takeitem = doitemfromPlanetSuper(1501, 20, pdid);
                    fs.ejectorPool[ec.id].bulletCount += takeitem[0];
                    fs.ejectorPool[ec.id].bulletInc += takeitem[1];
                }
            }
        }
        public void AssemblerSupplyDemand(FactorySystem fs, int pdid)
        {
            foreach (AssemblerComponent ac in fs.assemblerPool)
            {
                if (ac.id > 0 && ac.entityId > 0 && ac.recipeId > 0)
                {
                    if (QuantumtransportassembleDemand.Value)
                    {
                        int[] productCounts = ac.recipeExecuteData.productCounts;
                        int[] products = ac.recipeExecuteData.products;
                        for (int i = 0; i < productCounts.Length; i++)
                        {
                            int produced = ac.produced[i];
                            if (produced > 500) produced = 200;
                            if (produced > 0) produced -= doitemfromPlanetSuper(products[i], produced, pdid, 0, false)[0];
                            ac.produced[i] = produced;
                        }
                    }
                    if (QuantumtransportassembleSupply.Value)
                    {
                        int[] requireCounts = ac.recipeExecuteData.requireCounts;
                        int[] requires = ac.recipeExecuteData.requires;
                        for (int i = 0; i < ac.served.Length; i++)
                        {
                            if (ac.served[i] <= requireCounts[i] * 2)
                            {
                                int[] takeitem = doitemfromPlanetSuper(requires[i], 20, pdid);
                                ac.served[i] += takeitem[0];
                                ac.incServed[i] += takeitem[1];
                            }
                            if (ac.served[i] < 0) ac.served[i] = 0;
                        }
                    }
                }
            }
        }
        public void LabSupplyDemand(FactorySystem fs, int pdid)
        {
            foreach (LabComponent lc in fs.labPool)
            {
                if (lc.id > 0 && lc.entityId > 0)
                {
                    if (QuantumtransportlabSupply.Value && lc.researchMode)
                    {
                        for (int i = 0; i < lc.matrixServed.Length; i++)
                        {
                            if (lc.matrixServed[i] > 288000) continue;
                            int[] takeitem = doitemfromPlanetSuper(6001 + i, 20, pdid);
                            lc.matrixServed[i] += takeitem[0] * 3600;
                            lc.matrixIncServed[i] += takeitem[1] * 3600;
                        }
                    }
                    if (lc.matrixMode)
                    {
                        if (QuantumtransportlabDemand.Value)
                        {
                            for (int i = 0; i < lc.recipeExecuteData.productCounts.Length; i++)
                            {
                                if (lc.produced[i] > 5) lc.produced[i] -= doitemfromPlanetSuper(lc.recipeExecuteData.products[i], lc.produced[i], pdid, 0, false)[0];
                            }
                        }
                        if (QuantumtransportlabSupply.Value)
                        {
                            for (int i = 0; i < lc.served.Length; i++)
                            {
                                if (lc.served[i] > 20) continue;
                                int[] takeitem = doitemfromPlanetSuper(lc.recipeExecuteData.requires[i], 20, pdid);
                                lc.served[i] += takeitem[0];
                                lc.incServed[i] += takeitem[1];
                            }
                        }
                    }
                }
            }
        }
        public void PowerSupplyDemand(FactorySystem fs, int pdid)
        {
            foreach (PowerGeneratorComponent pgc in fs.factory.powerSystem.genPool)
            {
                if (pgc.id > 0)
                {
                    if (pgc.gamma)
                    {
                        if (QuantumtransportpowerDemand.Value && pgc.productCount >= 1) fs.factory.powerSystem.genPool[pgc.id].productCount -= doitemfromPlanetSuper(1208, (int)pgc.productCount, pdid, 0, false)[0];
                        if (QuantumtransportpowerSupply.Value && pgc.catalystPoint == 0)
                        {
                            int[] takeitem = doitemfromPlanetSuper(1209, 5, pdid);
                            fs.factory.powerSystem.genPool[pgc.id].catalystPoint += takeitem[0] * 3600;
                            fs.factory.powerSystem.genPool[pgc.id].catalystInc += (short)(takeitem[1] * 3600);
                        }
                    }
                    else if (QuantumtransportpowerSupply.Value)
                    {
                        int itemid = 0;
                        switch (pgc.fuelMask)
                        {
                            case 1: itemid = 1006; break;
                            case 2: itemid = 1802; break;
                            case 4: itemid = 1803; break;
                        }
                        if (itemid > 0 && pgc.fuelCount <= 2)
                        {
                            int[] takeitem = doitemfromPlanetSuper(itemid, 20, pdid);
                            short fuelcount = (short)takeitem[0];
                            if (fuelcount == 0)
                            {
                                return;
                            }
                            short inc = (short)takeitem[1];
                            if (pgc.fuelCount == 0)
                            {
                                fs.factory.powerSystem.genPool[pgc.id].SetNewFuel(itemid, fuelcount, inc);
                            }
                            else
                            {
                                fs.factory.powerSystem.genPool[pgc.id].fuelCount += fuelcount;
                                fs.factory.powerSystem.genPool[pgc.id].fuelInc += inc;
                            }
                        }
                    }
                }
            }
        }
        public void CollectorStationDemand()
        {
            foreach (var gid in CollectorStation)
            {
                StationComponent sc = GameMain.data.galacticTransport.stationPool[gid];
                ref var store1 = ref sc.storage[0];
                ref var store2 = ref sc.storage[1];
                store1.count -= doitemfromStarsuper(store1.itemId, store1.count, 0, false)[0];
                store2.count -= doitemfromStarsuper(store2.itemId, store2.count, 0, false)[0];
            }
        }
        public void SuperStationProvide()
        {
            foreach (int pdid in PlanetSuperStationItemidstore.Keys)
            {
                PlanetData pd = GameMain.galaxy.PlanetById(pdid);
                FactorySystem fs = pd.factory.factorySystem;
                if (QuantumtransportstationSupply.Value || QuantumtransportstationDemand.Value) StationSupplyDemand(fs, pdid);
                if (QuantumtransportminerDemand.Value) MinerSupplyDemand(fs, pdid);
                if (QuantumtransportsiloDemand.Value) SiloComponentSupplyDemand(fs, pdid);
                if (QuantumtransportlabSupply.Value || QuantumtransportlabDemand.Value) LabSupplyDemand(fs, pdid);
                if (QuantumtransportpowerSupply.Value || QuantumtransportpowerDemand.Value) PowerSupplyDemand(fs, pdid);
                if (QuantumtransportassembleSupply.Value || QuantumtransportassembleDemand.Value) AssemblerSupplyDemand(fs, pdid);
            }
        }
        public static int[] doitemfromStarsuper(int itemid, int itemcount, int inc = 0, bool take = true)
        {
            if (!StarSuperStationItemidstore.ContainsKey(itemid) || itemcount <= 0) return new int[2] { 0, 0 };
            StationComponent sc = GameMain.data.galacticTransport.stationPool[StarSuperStation[StarSuperStationItemidstore[itemid][0]]];
            int index = StarSuperStationItemidstore[itemid][0];
            int i = StarSuperStationItemidstore[itemid][1];
            long energycost = Stationfullenergy.Value ? 0 : Quantumenergy.Value;

            int count = itemcount;
            ref StationStore store = ref sc.storage[i];
            if (take)
            {
                count = Math.Min(count, store.count);
                if (sc.energy <= count * energycost && sc.energy > 0)
                {
                    count = Math.Min(count, (int)(sc.energy / energycost));
                    if (count == 0) return new int[2] { 0, 0 };
                }
                inc = Math.Min(count * 4, store.inc);
                sc.energy -= count * energycost;
                store.count -= count;
                store.inc -= inc;
            }
            else
            {
                if (sc.energy >= 0 && sc.energy < count * energycost)
                {
                    count = Math.Min(count, (int)(sc.energy / energycost));
                    if (count == 0) return new int[2] { 0, 0 };
                }
                if (store.count + count <= store.max)
                {
                    inc = Math.Min(count * incAbility, inc);
                    store.count += count;
                    store.inc += inc;
                }
                else if (store.localLogic == ELogisticStorage.None && store.remoteLogic == ELogisticStorage.None && store.max < int.MaxValue - 100)
                {
                    if (store.count + count <= int.MaxValue - 100)
                    {
                        inc = Math.Min(count * incAbility, inc);
                        store.max = store.count + count;
                        store.count = store.max;
                        store.inc += inc;
                    }
                    else
                    {
                        count = int.MaxValue - 100 - store.count;
                        store.max = store.count + count;
                        store.count = store.max;
                        store.inc += inc;
                    }
                }
                else
                {
                    count = store.max - store.count;
                    inc = Math.Min(count * incAbility, inc);
                    store.count = store.max;
                    store.inc += inc;
                }
                sc.energy -= count * energycost;
            }
            return new int[2] { count, StationMaxproliferator.Value ? incAbility * count : inc };
        }

        public static int[] doitemfromPlanetSuper(int itemid, int itemcount, int planetid, int inc = 0, bool take = true)
        {
            if (!PlanetSuperStationItemidstore.ContainsKey(planetid) || !PlanetSuperStationItemidstore[planetid].ContainsKey(itemid) || itemcount == 0) return new int[2] { 0, 0 };
            int PlanetSuperStationItemidstorecount = PlanetSuperStationItemidstore[planetid][itemid].Count;
            for (int i = 0; i < PlanetSuperStationItemidstorecount; i++)
            {
                int index = PlanetSuperStationItemidstore[planetid][itemid][i][0];
                int storageindex = PlanetSuperStationItemidstore[planetid][itemid][i][1];
                StationComponent sc = GameMain.data.galacticTransport.stationPool[SuperStation[index]];
                ref StationStore store = ref sc.storage[storageindex];
                if (sc.storage[storageindex].localLogic != ELogisticStorage.None)
                {
                    if (take && sc.storage[storageindex].localLogic != ELogisticStorage.Supply) continue;
                    if (!take && sc.storage[storageindex].localLogic != ELogisticStorage.Demand) continue;
                }
                long energycost = Stationfullenergy.Value ? 0 : Quantumenergy.Value;
                int count = itemcount;
                if (take)
                {
                    if (sc.energy < count * energycost)
                    {
                        count = Math.Min(count, (int)(sc.energy / energycost));
                    }
                    if (count == 0) return new int[2] { 0, 0 };
                    if (count > store.count)
                    {
                        int download = count - store.count;
                        int[] downloaded = doitemfromStarsuper(itemid, download);
                        count = store.count + downloaded[0];
                        inc = store.inc + downloaded[1];
                        store.count = 0;
                        store.inc = 0;
                    }
                    else
                    {
                        inc = Math.Min(count * incAbility, store.inc);
                        store.count -= count;
                        store.inc -= inc;
                    }
                }
                else
                {
                    if (sc.energy < count * energycost)
                    {
                        count = Math.Min(count, (int)(sc.energy / energycost));
                        if (count == 0) return new int[2] { 0, 0 };
                    }
                    inc = Math.Min(count * incAbility, inc);
                    if (store.count + count >= store.max)
                    {
                        int upload = store.count + count - store.max;
                        int[] uploaded = doitemfromStarsuper(itemid, upload, inc, false);
                        count -= upload - uploaded[0];
                        inc = Math.Min(count * incAbility, inc);
                        store.count = store.max;
                        store.inc += inc - uploaded[1];
                    }
                    else
                    {
                        store.count += count;
                        store.inc += inc;
                    }
                }
                return new int[2] { count, StationMaxproliferator.Value ? incAbility * count : inc };
            }
            return new int[2] { 0, 0 };
        }


        #endregion

        /// <summary>
        /// 进入游戏时初始化（GameMain.Begin 之后触发一次，此时存档已完整加载）
        /// </summary>
        private void OnGameBeginEvent()
        {
            GameRunning = true;
            PlanetModify.InitPlanetWaterType();
            if (BeltSignalFunction.Value)
            {
                BuildModify.InitBeltSignalDiction();
            }

            if (Quantumtransport_bool.Value)
            {
                BuildModify.InitQuantumTransport();
            }
            if (LDB.recipes != null && !LDBInitSave)
            {
                LDBInitSave = true;
                //保存配方原始属性
                RecipeModify.CaptureRecipeOriginals();
                //收集熔炉配方（原料 → 熔炼配方）
                RecipeModify.RebuildSmeltRecipeIndex(smeltRecipes);
            }
            if (LDB.recipes != null)
            {
                //每次进入存档都重新应用一次配方修改：LDB 若被重新加载，缓存会回到原值
                RecipeModify.ApplyRecipeModifications(allhandcraft.Value, quickproduce.Value);
            }
            alwaysemissiontemp = alwaysemission.Value;
            //更改物品相关
            RecipeModify.SetItemStackMultiple(StackMultiple.Value);
        }

        /// <summary>
        /// 退出游戏时清理本局状态（GameMain.End 之后触发）
        /// </summary>
        private void OnGameEndEvent()
        {
            GameRunning = false;
            closeallcollider = false;
            Beltsignal = new Dictionary<int, Dictionary<int, int>>();
            Beltsignalnumberoutput = new Dictionary<int, Dictionary<int, int>>();
            StarSuperStation = new List<int>();
            SuperStation = new List<int>();
            StarSuperStationItemidstore = new Dictionary<int, int[]>();
            PlanetSuperStationItemidstore = new Dictionary<int, Dictionary<int, List<int[]>>>();
            CollectorStation.Clear();
            alwaysemissiontemp = false;
            PlanetModify.ResetWaterTypes();
            GC.Collect();
        }

        /// <summary>
        /// 游戏启动后
        /// </summary>
        public void AfterGameStart()
        {
            if (!GameRunning)
                return;
            PlayerModify.PlayerDataInit();
            if (GameMain.localPlanet == null)
            {
                PlanetModify.CurrentPlanetWaterType = "";
            }
            else
            {
                BuildModify.BlueprintNoMaterial(blueprintpastenoneed_bool.Value);
                PlanetModify.RefreshCurrentPlanetWaterType();
            }
            if (!guidraw.DisplayingWindow || !guidraw.MouseInWindow)
            {
                veinproperty.ControlVein();
                CombatModify.CombatFunction();
            }
            if (refreshLDB)
            {
                refreshLDB = false;
                RecipeModify.ApplyRecipeModifications(allhandcraft.Value, quickproduce.Value);
            }
            OnsecondMethod();
            if (Infinitething.Value)
            {
                PlayerModify.InfinitePackage(player);
            }
            if (QuickResearch.Value)
            {
                TechnologyModify.AddResearchHash();
            }
            PlayerModify.SetDroneNoConsume(player, DroneNoenergy_bool.Value);
            BuildModify.DriftBuild(player);
        }

        /// <summary>
        /// 一秒间隔调用
        /// </summary>
        private void OnsecondMethod()
        {
            if (!OneSecondTimeElapse)
            {
                return;
            }
            //量子传输站
            if (Quantumtransport_bool.Value)
            {
                try
                {
                    takeitemfromstarsuperstation();
                }
                catch (Exception e) { Debug.Log("量子传输失败1" + e.Message); }
            }

            //机甲物流
            if (Mechalogistics_bool.Value)
            {
                PlayerModify.MechaLogisticsMethod(player);
            }

            //背包无限物品
            if (Infinitething.Value)
            {
                PlayerModify.InfinitePackage(player);
            }

            if (MiddleMouseLockGrid.Value)
            {
                PlayerModify.InfiniteFilteredPackage(player);
            }

            //消费掉本秒标记，保证 OnsecondMethod 每秒只执行一次
            OneSecondTimeElapse = false;
        }

        /// <summary>
        /// 取消所有选项
        /// </summary>
        public void CancelAllToggle()
        {
            InfiniteSand.Value = false;
            QuickHandcraft.Value = false;
            QuickPlayerMine.Value = false;
            QuickabortSwarm.Value = false;
            ChangeDysonradius.Value = false;
            NotTidyVein.Value = false;
            isInstantItem.Value = false;
            InspectDisNoLimit.Value = false;
            StationMaxproliferator.Value = false;
            Mechalogistics_bool.Value = false;
            MechalogStoragerecycle_bool.Value = false;
            MechalogStorageprovide_bool.Value = false;
            MechalogStationrecycle_bool.Value = false;
            MechalogStationprovide_bool.Value = false;
            Infinitestoragetank.Value = false;
            TankMaxproliferator.Value = false;
            Quantumtransport_bool.Value = false;
            Quantumtransportbuild_bool.Value = false;
            Quantumtransportpdwarp_bool.Value = false;
            Quantumtransportstarwarp_bool.Value = false;
            needtrashsand.Value = false;
            InfineteStarPower.Value = false;
            PlanetPower_bool.Value = false;
            allhandcraft.Value = false;
            quickproduce.Value = false;
            blueprintpastenoneed_bool.Value = false;
            noneedwarp.Value = false;
            Infinitething.Value = false;
            Infiniteplayerpower.Value = false;
            deleteveinbool.Value = false;
            MechalogisticsPlanet_bool.Value = false;
            StationMiner.Value = false;
            dismantle_but_nobuild.Value = false;
            build_station_nocondition.Value = false;
            StationTrash.Value = false;
            DroneNoenergy_bool.Value = false;
            BuildNotime_bool.Value = false;
            Station_infiniteWarp_bool.Value = false;
            StationfullCount_bool.Value = false;
            Buildingnoconsume.Value = false;
            Stationfullenergy.Value = false;
            BeltSignalFunction.Value = false;
            Tankcontentall.Value = false;
            ArchitectMode.Value = false;
            quickabsorbsolar.Value = false;
            cancelsolarbullet.Value = false;
            alwaysemission.Value = false;
            StationSpray.Value = false;
            Maxproliferator.Value = false;
            StationPowerGen.Value = false;
            CloseUIAbnormalityTip.Value = false;
            QuantumtransportstationSupply.Value = false;
            QuantumtransportlabSupply.Value = false;
            QuantumtransportpowerSupply.Value = false;
            QuantumtransportassembleSupply.Value = false;
            QuantumtransportstationDemand.Value = false;
            QuantumtransportminerDemand.Value = false;
            QuantumtransportsiloDemand.Value = false;
            QuantumtransportlabDemand.Value = false;
            QuantumtransportpowerDemand.Value = false;
            QuantumtransportassembleDemand.Value = false;
            Windturbinescovertheglobe.Value = false;
            WindturbinesUnlimitedEnergy.Value = false;
            AutoChangeStationName.Value = "";
            StationMinerSmelter.Value = false;
            StationSprayer.Value = false;
            //快速生产改的是配方缓存，取消全部勾选后必须重新应用一次配方，否则周期不会被还原
            refreshLDB = true;
        }

        /// <summary>
        /// 开启窗口
        /// </summary>
        public void QuickKeyOpenWindow()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                guidraw.CloseMainWindow();
            }
            if (!ChangingQuickKey)
            {
                bool isClick = false;
                if (QuickKey.Value.Modifiers.Count() == 0)
                {
                    if (Input.GetKeyDown(QuickKey.Value.MainKey))
                    {
                        isClick = true;
                    }
                }
                else
                {
                    if (Input.GetKey(QuickKey.Value.MainKey))
                    {
                        isClick = true;
                        foreach (var modify in QuickKey.Value.Modifiers)
                        {
                            if (!Input.GetKeyDown(modify))
                            {
                                isClick = false;
                            }
                        }
                    }
                }
                if (isClick)
                {
                    guidraw.MainWindowKeyInvoke();
                }
            }
            if (TurrentKeepSuperNovalQuickKey.Value)
            {
                if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.S))
                {
                    TurrentKeepSuperNoval.Value = !TurrentKeepSuperNoval.Value;
                    UIRealtimeTip.Popup("炮塔永久超新星" + (TurrentKeepSuperNoval.Value ? "开启" : "关闭"), true, 0);
                }
            }
        }

        /// <summary>
        /// 修改快捷键
        /// </summary>
        public void ChangeQuickKeyMethod()
        {
            if (ChangeQuickKey)
            {
                setQuickKey();
                ChangingQuickKey = true;
            }
            else if (!ChangeQuickKey && ChangingQuickKey)
            {
                QuickKey.Value = tempShowWindow;
                ChangingQuickKey = false;
            }
        }

        /// <summary>
        /// 设置地基
        /// </summary>
        /// <param name="type"></param>
        public void OnSetBase()
        {
            var localPlanet = GameMain.localPlanet;
            if (localPlanet == null || localPlanet.type == EPlanetType.Gas)
                return;
            var factory = localPlanet.factory;
            if (restorewater)
            {
                factory.PlanetReformRevert();
            }
            else
            {
                BuildTool_Reform buildTool_Reform = player.controller.actionBuild.reformTool;
                factory.PlanetReformAll(buildTool_Reform.brushType, buildTool_Reform.brushColor, buildTool_Reform.buryVeins);
            }
        }

        #region 物品修改
        #endregion

        /// <summary>
        /// 设置快捷键
        /// </summary>
        public void setQuickKey()
        {
            bool left = true;
            int[] result = new int[2];
            if (Input.GetKey(KeyCode.LeftShift) && left)
            {
                left = false;
                result[0] = 304;
            }
            if (Input.GetKey(KeyCode.LeftControl) && left)
            {
                left = false;
                result[0] = 306;
            }
            if (Input.GetKey(KeyCode.LeftAlt) && left)
            {
                left = false;
                result[0] = 308;
            }
            bool right = true;
            for (int i = (int)KeyCode.Alpha0; i <= (int)KeyCode.Alpha9 && right; i++)
            {
                if (Input.GetKey((KeyCode)i))
                {
                    result[1] = i;
                    right = false;
                    break;
                }
            }
            for (int i = (int)KeyCode.A; i <= (int)KeyCode.Z && right; i++)
            {
                if (Input.GetKey((KeyCode)i))
                {
                    result[1] = i;
                    right = false;
                    break;
                }
            }
            for (int i = (int)KeyCode.F1; i <= (int)KeyCode.F10 && right; i++)
            {
                if (Input.GetKey((KeyCode)i))
                {
                    result[1] = i;
                    right = false;
                    break;
                }
            }
            int num;
            if (left && right) num = 0;
            else if (!left && !right) num = 2;
            else num = 1;
            if (num == 2)
            {
                tempShowWindow = new KeyboardShortcut((KeyCode)result[1], (KeyCode)result[0]);
            }
            else if (num == 1)
            {
                int keynum = Math.Max(result[0], result[1]);
                tempShowWindow = new KeyboardShortcut((KeyCode)keynum);
            }
        }


    }

}