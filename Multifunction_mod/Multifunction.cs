using BepInEx;
using BepInEx.Configuration;
using Multifunction_mod.Models;
using Multifunction_mod.Patchs;
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
    [BepInPlugin(GUID, NAME, VERSION)]
    public class Multifunction : BaseUnityPlugin
    {
        public const string GUID = "cn.blacksnipe.dsp.Multfuntion_mod";
        public const string NAME = "Multfuntion_mod";
        public const string VERSION = "3.4.7";

        #region 临时变量


        public static bool IsEnglish;

        public int ReformType;
        public int ReformColor;
        public float OnesecondTime;
        public float HalfsecondTime;
        public string currentPlanetWaterType = "";
        private SeedPlanetWater originWaterTypes;
        private SeedPlanetWater currentWaterTypes;
        public Dictionary<long, SeedPlanetWater> seedPlanetWaterTypes;
        public static GUIDraw guidraw;
        public static PropertyData propertyData;
        public static VeinControlProperty veinproperty;

        public static bool OneSecondTimeElapse;
        public static bool HalfSecondTimeElapse;

        public RecipeProto[] originRecipeProtos;
        public static Player player;
        public static List<Tempsail> tempsails = new List<Tempsail>();
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
        public static bool playcancelsolarbullet;
        public static bool alwaysemissiontemp;
        public static bool FinallyInit;
        public static bool restorewater;
        public static bool entityitemnoneed;
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
        public static ConfigEntry<string> Mechalogneed;
        public static ConfigEntry<bool> InfiniteSand;
        public static ConfigEntry<bool> isInstantItem;
        public static ConfigEntry<bool> WindturbinesUnlimitedEnergy;
        public static ConfigEntry<bool> Windturbinescovertheglobe;
        public static ConfigEntry<bool> NotTidyVein;
        public static ConfigEntry<bool> InspectDisNoLimit;
        public static ConfigEntry<bool> StationMaxproliferator;
        public static ConfigEntry<bool> Mechalogistics_bool;
        public static ConfigEntry<bool> MechalogStoragerecycle_bool;
        public static ConfigEntry<bool> MechalogStorageprovide_bool;
        public static ConfigEntry<bool> MechalogStationrecycle_bool;
        public static ConfigEntry<bool> MechalogStationprovide_bool;
        public static ConfigEntry<bool> Infinitestoragetank;
        public static ConfigEntry<bool> QuickHandcraft;
        public static ConfigEntry<bool> QuickPlayerMine;
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
            originWaterTypes = new SeedPlanetWater();
            seedPlanetWaterTypes = new Dictionary<long, SeedPlanetWater>();
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
                MiddleMouseLockGrid = Config.Bind("鼠标中间锁定格子", "MiddleMouseLockGrid", false);

                Mechalogneed = Config.Bind("机甲物流需求情况", "Mechalogneed", "");
                CuttingVeinNumbers = Config.Bind("切割矿脉数量", "CuttingVeinNumbers", 3);
                veinlines = Config.Bind("矿物行数", "veinlines", 3);
                NotTidyVein = Config.Bind("矿堆不整理", "NotTidyVein", false);
                deleteveinbool = Config.Bind("删除矿物", "deleteveinbool", false);
                StationMaxproliferator = Config.Bind("物流站无限增产点数", "StationMaxproliferator", false);
                BeltSignalFunction = Config.Bind("传送带信号功能", "BeltSignalFunction", false);
                Mechalogistics_bool = Config.Bind("机甲物流", "Mechalogistics_bool", false);
                MechalogisticsPlanet_bool = Config.Bind("机甲物流专用星", "MechalogisticsPlanet_bool", false);
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
                oillowerlimit = (int)(1 / VeinData.oilSpeedMultiplier),
                DeleteVein = deleteveinbool.Value,
                NotTidyVein = NotTidyVein.Value
            };
            incAbility = Maxproliferator.Value ? 10 : 4;
            tempShowWindow = QuickKey.Value;

            CollectorStation = new List<int>();
            InitWaterTypes();
            IgnoreControlPanelLocalLimit.SettingChanged += (sender, args) =>
            {
                UIControlPanelPatch.Enable = IgnoreControlPanelLocalLimit.Value;
            };
            UIControlPanelPatch.Enable = IgnoreControlPanelLocalLimit.Value;
            Multifunctionpatch.Patchallmethod();
        }
        int dockindex = -1;

        void Update()
        {
            if (FinallyInit)
            {
                if (Time.time > OnesecondTime + 1)
                {
                    OnesecondTime = Time.time;
                    OneSecondTimeElapse = true;
                }
                if (Time.time > HalfsecondTime + 0.5f)
                {
                    HalfsecondTime = Time.time;
                    HalfSecondTimeElapse = true;
                }
            }
            ChangeQuickKeyMethod();
            QuickKeyOpenWindow();
            FirstStartGame();
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
                        for (int i = 0; i < ac.productCounts.Length; i++)
                        {
                            int produced = ac.produced[i];
                            if (produced > 500) produced = 200;
                            if (produced > 0) produced -= doitemfromPlanetSuper(ac.products[i], produced, pdid, 0, false)[0];
                            ac.produced[i] = produced;
                        }
                    }
                    if (QuantumtransportassembleSupply.Value)
                    {
                        for (int i = 0; i < ac.served.Length; i++)
                        {
                            if (ac.served[i] <= ac.requireCounts[i] * 2)
                            {
                                int[] takeitem = doitemfromPlanetSuper(ac.requires[i], 20, pdid);
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
                        for (int i = 0; i < lc.matrixPoints.Length; i++)
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
                            for (int i = 0; i < lc.productCounts.Length; i++)
                            {
                                if (lc.produced[i] > 5) lc.produced[i] -= doitemfromPlanetSuper(lc.products[i], lc.produced[i], pdid, 0, false)[0];
                            }
                        }
                        if (QuantumtransportlabSupply.Value)
                        {
                            for (int i = 0; i < lc.served.Length; i++)
                            {
                                if (lc.served[i] > 20) continue;
                                int[] takeitem = doitemfromPlanetSuper(lc.requires[i], 20, pdid);
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
                            fs.factory.powerSystem.genPool[pgc.id].catalystIncPoint += takeitem[1] * 3600;
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
        /// 加载存档时进行初始化操作
        /// </summary>
        public void FirstStartGame()
        {
            if (GameMain.instance == null)
            {
                return;
            }
            if (!GameMain.instance.running && FinallyInit)
            {
                GameRunning = false;
                FinallyInit = false;
                closeallcollider = false;
                tempsails = new List<Tempsail>();
                Beltsignal = new Dictionary<int, Dictionary<int, int>>();
                Beltsignalnumberoutput = new Dictionary<int, Dictionary<int, int>>();
                StarSuperStation = new List<int>();
                SuperStation = new List<int>();
                StarSuperStationItemidstore = new Dictionary<int, int[]>();
                PlanetSuperStationItemidstore = new Dictionary<int, Dictionary<int, List<int[]>>>();
                CollectorStation.Clear();
                SuperStation = new List<int>();
                StarSuperStation = new List<int>();
                playcancelsolarbullet = false;
                alwaysemissiontemp = false;
                originWaterTypes = new SeedPlanetWater();
                GC.Collect();
            }
            if (GameMain.instance.running && !FinallyInit)
            {
                FinallyInit = true;
                InitPlanetWaterType();
                if (BeltSignalFunction.Value)
                {
                    InitBeltSignalDiction();
                }

                if (Quantumtransport_bool.Value)
                {
                    InitQuantumTransport();
                }
                if (LDB.recipes != null && !LDBInitSave)
                {
                    LDBInitSave = true;
                    //保存配方原始属性
                    originRecipeProtos = new RecipeProto[LDB.recipes.dataArray.Length];
                    for (int i = 0; i < originRecipeProtos.Length; i++)
                    {
                        var recipe = LDB.recipes.dataArray[i];
                        originRecipeProtos[i] = new RecipeProto()
                        {
                            TimeSpend = recipe.TimeSpend,
                            Handcraft = recipe.Handcraft
                        };
                        //收集熔炉配方
                        if (recipe.Type != ERecipeType.Smelt) continue;
                        foreach (var item in recipe.Items)
                        {
                            if (smeltRecipes.ContainsKey(item))
                            {
                                smeltRecipes[item].Add(recipe);
                            }
                            else
                            {
                                smeltRecipes.Add(item, new List<RecipeProto>() { recipe });
                            }
                        }
                    }
                    //更改配方相关
                    ChangeRecipe();
                }
                playcancelsolarbullet = cancelsolarbullet.Value;
                alwaysemissiontemp = alwaysemission.Value;
                //更改物品相关
                SetmultipleItemStatck(StackMultiple.Value);
                ChangeItemstack();
            }

        }

        /// <summary>
        /// 游戏启动后
        /// </summary>
        public void AfterGameStart()
        {
            if (GameMain.history == null || !FinallyInit)
                return;
            GameRunning = true;
            PlayerDataInit();
            if (GameMain.localPlanet == null)
            {
                currentPlanetWaterType = "";
            }
            else
            {
                BluePrintpasteNoneed();
                if (string.IsNullOrEmpty(currentPlanetWaterType))
                {
                    currentPlanetWaterType = LDB.items.Select(GameMain.localPlanet.waterItemId)?.name ?? "无海洋".getTranslate();
                }
            }
            if (!guidraw.DisplayingWindow || !guidraw.MouseInWindow)
            {
                veinproperty.ControlVein();
                CombatFunction();
            }
            if (refreshLDB)
            {
                refreshLDB = false;
                ChangeRecipe();
            }
            OnsecondMethod();
            if (Infinitething.Value)
            {
                InfiniteAllThingInPackage();
            }
            SetDronenocomsume();
            DriftBuild();
        }

        private void CombatFunction()
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

        /// <summary>
        /// 初始化存档海洋类型
        /// </summary>
        private void InitPlanetWaterType()
        {
            if (GameMain.galaxy?.stars == null || GameMain.data?.gameDesc == null)
            {
                return;
            }
            long seed = GameMain.data.gameDesc.seedKey64;
            originWaterTypes.seedKey64 = seed;
            foreach (var star in GameMain.galaxy.stars)
            {
                if (star == null) continue;
                foreach (var planet in star.planets)
                {
                    if (planet == null || planet.gasTotalHeat > 0) continue;
                    originWaterTypes.waterTypes.Add(planet.id, planet.waterItemId);
                }
            }
            if (seedPlanetWaterTypes.ContainsKey(seed))
            {
                currentWaterTypes = seedPlanetWaterTypes[seed];
                foreach (var watertypes in currentWaterTypes.waterTypes)
                {
                    var pd = GameMain.galaxy.PlanetById(watertypes.Key);
                    if (pd != null)
                    {
                        pd.waterItemId = watertypes.Value;
                    }
                }
            }
            else
            {
                currentWaterTypes = new SeedPlanetWater(seed);
                seedPlanetWaterTypes.Add(seed, currentWaterTypes);
            }
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
                MechaLogisticsMethod();
            }

            //背包无限物品
            if (Infinitething.Value)
            {
                InfiniteAllThingInPackage();
            }

            if (MiddleMouseLockGrid.Value)
            {
                InfiniteFilterThingInPackage();
            }
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
        /// 控制建筑的高度
        /// </summary>
        public void DriftBuild()
        {
            if (!DriftBuildings)
                return;
            var actionBuild = player?.controller?.actionBuild;
            if (actionBuild != null && player.controller.cmd.type == ECommand.Build)
            {
                if (blueprintPasteToolActive != actionBuild.blueprintPasteTool.active)
                {
                    blueprintPasteToolActive = !blueprintPasteToolActive;
                    DriftBuildingLevel = 0;
                }
                if (Input.GetKeyDown(KeyCode.UpArrow))
                {
                    DriftBuildingLevel++;
                    if (blueprintPasteToolActive)
                    {
                        actionBuild.blueprintPasteTool.cursorValid = true;
                        actionBuild.blueprintPasteTool.DeterminePreviewsPrestage();
                    }
                }
                if (Input.GetKeyDown(KeyCode.DownArrow))
                {
                    DriftBuildingLevel--;
                    if (blueprintPasteToolActive)
                    {
                        actionBuild.blueprintPasteTool.cursorValid = true;
                        actionBuild.blueprintPasteTool.DeterminePreviewsPrestage();
                    }
                }
            }
            else if (DriftBuildingLevel != 0)
            {
                DriftBuildingLevel = 0;
            }
        }

        /// <summary>
        /// 超密铺采集器
        /// </summary>
        public void SetMaxGasStation()
        {
            if (GameMain.localPlanet == null || GameMain.localPlanet.type != EPlanetType.Gas)
                return;
            for (int i = 0; i < nodepos.Count; i++)
            {
                Vector3 pos;
                pos = nodepos[i] * GameMain.localPlanet.realRadius;
                Vector3 vector3_3 = 0.025f * pos.normalized * player.planetData.realRadius;
                Quaternion quaternion3 = Maths.SphericalRotation(pos, player.controller.actionBuild.clickTool.yaw);
                pos = pos + vector3_3;
                PrebuildData prebuild = new PrebuildData();
                prebuild.protoId = 2105;
                prebuild.modelIndex = 117;
                prebuild.pos = pos + quaternion3 * Vector3.zero;
                prebuild.pos2 = pos + quaternion3 * Vector3.zero;
                prebuild.rot = quaternion3 * Quaternion.identity;
                prebuild.rot2 = quaternion3 * Quaternion.identity;
                prebuild.pickOffset = 0;
                prebuild.insertOffset = 0;
                prebuild.recipeId = 0;
                prebuild.filterId = 0;
                prebuild.paramCount = 0;
                prebuild.InitParametersArray(0);
                player.controller.actionBuild.clickTool.factory.AddPrebuildDataWithComponents(prebuild);
            }
        }

        /// <summary>
        /// 初始化玩家数据
        /// </summary>
        public void InitPlayer()
        {
            if (player == null || player.mecha == null || player.mecha.constructionModule.droneIdleCount < player.mecha.constructionModule.droneAliveCount) return;
            refreshPlayerData = true;
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
        /// 物流背包逻辑
        /// </summary>
        public void MechaLogisticsMethod()
        {
            StorageComponent package = player.package;

            for (int i = 0; i < player.deliveryPackage.grids.Length; i++)
            {
                var item = player.deliveryPackage.grids[i];
                int itemId = item.itemId;
                int count = item.count;
                int packagecount = player.packageUtility.GetPackageItemCount(itemId);
                int exsitcount = packagecount + count;
                if (item.requireCount > exsitcount)
                {
                    int remain = item.stackSizeModified - count;
                    for (int index = 0; index < package.size; ++index)
                    {
                        if (package.grids[index].itemId == itemId)
                            remain += LDB.items.Select(itemId).StackSize - package.grids[index].count;
                        else if (package.grids[index].itemId == 0)
                            remain += LDB.items.Select(itemId).StackSize;
                    }
                    int need = Math.Min(item.requireCount - exsitcount, remain);
                    int getcount = 0;
                    int inc = 0;
                    foreach (StarData sd in GameMain.galaxy.stars)
                    {
                        foreach (PlanetData pd in sd.planets)
                        {
                            if (pd == null || pd.factory == null) continue;
                            if (MechalogisticsPlanet_bool.Value && !pd.displayName.Equals("机甲物流")) continue;
                            if (pd.factory.transport != null && pd.factory.transport.stationPool != null && MechalogStationprovide_bool.Value)
                            {
                                foreach (StationComponent sc in pd.factory.transport.stationPool)
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
                                if (pd.factory.factoryStorage != null && pd.factory.factoryStorage.tankPool != null && getcount < need)
                                {
                                    foreach (TankComponent tc in pd.factory.factoryStorage.tankPool)
                                    {
                                        if (tc.id > 0 && tc.fluidId > 0 && tc.fluidCount > 0 && itemId == tc.fluidId)
                                        {
                                            int temp = tc.fluidCount > need ? need : tc.fluidCount;

                                            int num = (int)(tc.fluidInc / tc.fluidCount + 0.5) * temp;
                                            pd.factory.factoryStorage.tankPool[tc.id].fluidCount -= temp;
                                            pd.factory.factoryStorage.tankPool[tc.id].fluidInc -= num;

                                            inc += num;
                                            getcount += temp;
                                            need -= temp;
                                            if (getcount >= need) break;
                                        }
                                    }
                                }
                                if (getcount >= need) break;
                            }
                            if (pd.factory.factoryStorage != null && pd.factory.factoryStorage.storagePool != null && MechalogStorageprovide_bool.Value && getcount < need)
                            {
                                foreach (StorageComponent sc in pd.factory.factoryStorage.storagePool)
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
                        if (getcount >= need) break;
                    }
                    player.packageUtility.AddItemToAllPackages(itemId, getcount, i, inc, out _);
                }
                if (item.recycleCount < exsitcount)
                {
                    int recyclenum = exsitcount - item.recycleCount;
                    player.packageUtility.TakeItemFromAllPackages(i, ref itemId, ref recyclenum, out int inc1);
                    if (itemId == 0 || recyclenum == 0) continue;
                    foreach (StarData sd in GameMain.galaxy.stars)
                    {
                        foreach (PlanetData pd in sd.planets)
                        {
                            if (pd == null || pd.factory == null) continue;
                            if (MechalogisticsPlanet_bool.Value && !pd.displayName.Equals("机甲物流")) continue;
                            if (pd.factory.transport != null && pd.factory.transport.stationPool != null && MechalogStationrecycle_bool.Value)
                            {
                                foreach (StationComponent sc in pd.factory.transport.stationPool)
                                {
                                    if (sc == null || sc.storage == null) continue;
                                    recyclenum -= sc.AddItem(itemId, recyclenum, inc1);
                                    if (recyclenum == 0) break;
                                }

                                if (pd.factory.factoryStorage != null && pd.factory.factoryStorage.tankPool != null && recyclenum > 0)
                                {
                                    foreach (TankComponent tc in pd.factory.factoryStorage.tankPool)
                                    {
                                        if (recyclenum == 0) break;
                                        if (tc.id > 0 && tc.fluidId > 0 && tc.fluidCount > 0 && itemId == tc.fluidId)
                                        {
                                            if (tc.fluidCount > 10000)
                                            {
                                                pd.factory.factoryStorage.tankPool[tc.id].fluidCount += recyclenum;
                                                pd.factory.factoryStorage.tankPool[tc.id].fluidInc += inc1;
                                                recyclenum = 0;
                                            }
                                            else
                                            {
                                                int temp = tc.fluidCapacity - tc.fluidCount > recyclenum ? recyclenum : tc.fluidCapacity - tc.fluidCount;
                                                recyclenum -= temp;
                                                pd.factory.factoryStorage.tankPool[tc.id].fluidCount += temp;
                                            }
                                        }
                                    }
                                }
                                if (recyclenum == 0) break;
                            }
                            if (pd.factory.factoryStorage != null && pd.factory.factoryStorage.storagePool != null && MechalogStoragerecycle_bool.Value && recyclenum > 0)
                            {
                                foreach (StorageComponent sc in pd.factory.factoryStorage.storagePool)
                                {
                                    if (sc == null) continue;
                                    recyclenum -= sc.AddItem(itemId, recyclenum, inc1, out int inc);
                                    if (recyclenum == 0) break;
                                }
                                if (recyclenum == 0) break;
                            }
                        }
                        if (recyclenum == 0) break;
                    }
                }
            }
        }

        /// <summary>
        /// 初始化信号传送带功能
        /// </summary>
        public void InitBeltSignalDiction()
        {
            if (GameMain.galaxy == null || GameMain.galaxy.stars == null) return;
            foreach (StarData sd in GameMain.galaxy.stars)
            {
                foreach (PlanetData pd in sd.planets)
                {
                    if (pd == null || pd.factory == null || pd.factory.entitySignPool == null) continue;
                    foreach (EntityData ed in pd.factory.entityPool)
                    {
                        if (ed.id <= 0 || ed.beltId == 0 || pd.factory.entitySignPool[ed.id].iconId0 == 0) continue;
                        int signalId = (int)pd.factory.entitySignPool[ed.id].iconId0;
                        int beltid = ed.beltId;
                        if (!Beltsignal.ContainsKey(pd.factoryIndex))
                            Beltsignal.Add(pd.factoryIndex, new Dictionary<int, int>());
                        if (!Beltsignal[pd.factoryIndex].ContainsKey(beltid))
                            Beltsignal[pd.factoryIndex].Add(beltid, signalId);
                        else
                            Beltsignal[pd.factoryIndex][beltid] = signalId;
                        if (!Beltsignalnumberoutput.ContainsKey(pd.factoryIndex))
                            Beltsignalnumberoutput.Add(pd.factoryIndex, new Dictionary<int, int>());
                        if (!Beltsignalnumberoutput[pd.factoryIndex].ContainsKey(beltid) && pd.factory.entitySignPool[ed.id].iconId0 == 601 && pd.factory.entitySignPool[ed.id].count0 > 0)
                            Beltsignalnumberoutput[pd.factoryIndex].Add(beltid, (int)pd.factory.entitySignPool[ed.id].count0);
                    }
                }
            }
        }

        /// <summary>
        /// 量子站初始化
        /// </summary>
        private void InitQuantumTransport()
        {
            foreach (StationComponent sc in GameMain.data.galacticTransport.stationPool)
            {
                if (sc == null || !sc.isCollector) continue;
                CollectorStation.Add(sc.gid);
            }
        }

        /// <summary>
        /// 添加物品数量消耗
        /// </summary>
        /// <param name="itemid"></param>
        /// <param name="count"></param>
        public static void AddComsumeItemtoTotal(int itemid, int count)
        {
            List<int> readytocompute = new List<int>();
            List<long> countList = new List<long>();
            readytocompute.Add(itemid);
            countList.Add(count);
            int index = 0;
            while (index < readytocompute.Count)
            {
                ItemProto item = LDB.items.Select(readytocompute[index]);
                if (item.recipes == null || item.recipes.Count == 0 || readytocompute[index] == 1003)
                {
                    index++;
                    continue;
                }
                for (int i = 0; i < item.recipes[0].Items.Length; i++)
                {
                    readytocompute.Add(item.recipes[0].Items[i]);
                    countList.Add(item.recipes[0].ItemCounts[i] * countList[index]);
                }
                index++;

            }
            for (int i = readytocompute.Count - 1; i >= 0; i--)
            {
                try
                {
                    if (itemid == 1503 && readytocompute[i] == 1501) countList[i] *= 12;
                    if (itemid == 1503 && readytocompute[i] == 1404) countList[i] *= 12;
                    while (countList[i] > 0)
                    {
                        int num = countList[i] > int.MaxValue ? int.MaxValue : (int)countList[i];
                        GameMain.data.statistics.production.factoryStatPool[GameMain.localPlanet.factory.index].AddProductionToTotalArray(readytocompute[i], num);
                        GameMain.data.statistics.production.factoryStatPool[GameMain.localPlanet.factory.index].AddConsumptionToTotalArray(readytocompute[i], num);
                        countList[i] -= num;
                    }
                }
                catch
                {

                }
            }

        }

        /// <summary>
        /// 初始化玩家数据
        /// </summary>
        public void PlayerDataInit()
        {
            if (refreshPlayerData || player == null || GameMain.mainPlayer != player)
            {
                refreshPlayerData = false;
                player = GameMain.mainPlayer;
                propertyData = new PropertyData();
                propertyData.SetContent(player.mecha, GameMain.history, player.deliveryPackage.stackSizeMultiplier);
                if (Mechalogistics_bool.Value && !player.deliveryPackage.unlocked)
                {
                    player.deliveryPackage.unlocked = true;
                    player.deliveryPackage.colCount = 1;
                    player.deliveryPackage.NotifySizeChange();
                }
            }

        }

        /// <summary>
        /// 蓝图建筑无需资源
        /// </summary>
        public void BluePrintpasteNoneed()
        {
            if (!blueprintpastenoneed_bool.Value) return;

            var factory = GameMain.localPlanet?.factory;
            if (factory==null) return;
            for (int i = 1; i < factory.prebuildCursor; i++)
            {
                if (factory.prebuildPool[i].protoId > 0)
                {
                    factory.prebuildPool[i].itemRequired = 0;
                }
            }
        }

        /// <summary>
        /// 瞬间完成戴森球
        /// </summary>
        public void FinishDysonShell()
        {
            foreach (DysonSphere ds in GameMain.data.dysonSpheres)
            {
                if (ds == null || ds.starData != GameMain.localStar) continue;
                int sum1 = 0;
                int sum2 = 0;
                foreach (DysonSphereLayer w in ds.layersIdBased)
                {
                    if (w == null) continue;
                    foreach (DysonNode t in w.nodePool)
                    {
                        if (t == null) continue;
                        if (t.totalSp < t.totalSpMax)
                        {
                            sum1 += t.totalSpMax - t.totalSp;
                            t.sp = 30;
                            for (int index = 0; index < t.frames.Count; ++index)
                            {
                                if (t.frames[index].nodeA == t)
                                    t.frames[index].spA = t.frames[index].spMax / 2;
                                else if (t.frames[index].nodeB == t)
                                    t.frames[index].spB = t.frames[index].spMax / 2;
                            }
                        }
                        if (t.totalCp < t.totalCpMax)
                        {
                            sum2 = t.totalCpMax - t.totalCp;
                            for (int index = 0; index < t.shells.Count; index++)
                            {
                                DysonShell shell = t.shells[index];
                                int nodeIndex = shell.nodeIndexMap[t.id];
                                shell.nodecps[nodeIndex] += (shell.vertsqOffset[nodeIndex + 1] - shell.vertsqOffset[nodeIndex]) * shell.cpPerVertex;
                                shell.nodecps[shell.nodecps.Length - 1] += (shell.vertsqOffset[nodeIndex + 1] - shell.vertsqOffset[nodeIndex]) * shell.cpPerVertex;
                            }
                        }
                    }
                }
                ds.modelRenderer.RebuildModels();
            }
        }

        /// <summary>
        /// 清除全部建筑
        /// </summary>
        /// <param name="type">0正常清除，1不掉落清除，2初始化带还原地形，3初始化不还原地形</param>
        public void RemoveAllBuild(int type)
        {
            if (GameMain.localPlanet != null && GameMain.localPlanet.factory != null)
            {
                if (type == 2 || type == 3)
                {
                    PlanetData planet = GameMain.localPlanet;
                    //planet.data = new PlanetRawData(planet.precision);
                    //planet.data.CalcVerts();
                    foreach (EntityData ed in planet.factory.entityPool)
                    {
                        if (ed.id != 0)
                        {
                            if (ed.colliderId != 0)
                            {
                                planet.physics.RemoveLinkedColliderData(ed.colliderId);
                                planet.physics.NotifyObjectRemove(EObjectType.Entity, ed.id);
                            }
                            if (ed.modelId != 0)
                            {
                                GameMain.gpuiManager.RemoveModel(ed.modelIndex, ed.modelId, true);
                            }
                            if (ed.mmblockId != 0)
                            {
                                planet.factory.blockContainer.RemoveMiniBlock(ed.mmblockId);
                            }
                            if (ed.audioId != 0)
                            {
                                if (planet.audio != null)
                                {
                                    planet.audio.RemoveAudioData(ed.audioId);
                                }
                            }
                        }
                    }
                    if (planet.factory.enemyPool != null)
                    {
                        for (int i = 1; i < planet.factory.enemyCursor; i++)
                        {
                            ref EnemyData ptr13 = ref planet.factory.enemyPool[i];
                            if (ptr13.id == i)
                            {
                                int combatStatId = ptr13.combatStatId;
                                planet.factory.skillSystem.OnRemovingSkillTarget(combatStatId, planet.factory.skillSystem.combatStats.buffer[combatStatId].originAstroId, ETargetType.CombatStat);
                                planet.factory.skillSystem.combatStats.Remove(combatStatId);
                                planet.factory.KillEnemyFinally(player, i, ref CombatStat.empty);
                            }
                        }
                        planet.factory.enemySystem.Free();
                        UIRoot.instance.uiGame.dfAssaultTip.ClearAllSpots();
                    }

                    if (planet.factory.transport != null && planet.factory.transport.stationPool != null)
                    {
                        foreach (StationComponent sc in planet.factory.transport.stationPool)
                        {
                            if (sc != null && sc.id > 0)
                            {
                                sc.storage = new StationStore[sc.storage.Length];
                                sc.needs = new int[sc.needs.Length];
                                int protoId = planet.factory.entityPool[sc.entityId].protoId;
                                planet.factory.DismantleFinally(player, sc.entityId, ref protoId);
                            }
                        }
                    }
                    if (GameMain.gameScenario != null)
                    {
                        if (planet.factory.powerSystem != null && planet.factory.powerSystem.genPool != null)
                        {
                            foreach (PowerGeneratorComponent pgc in planet.factory.powerSystem.genPool)
                            {
                                if (pgc.id > 0)
                                {
                                    int protoId = planet.factory.entityPool[pgc.entityId].protoId;
                                    GameMain.gameScenario.achievementLogic.NotifyBeforeDismantleEntity(planet.id, protoId, pgc.entityId);
                                    GameMain.gameScenario.NotifyOnDismantleEntity(planet.id, protoId, pgc.entityId);
                                }
                            }
                        }
                    }

                    #region 初始化坑
                    if (type == 2)
                    {
                        byte[] modData = planet.modData;
                        bool temprestorewater = restorewater;
                        restorewater = true;
                        for (int i = 0; i < modData.Length * 2; i++)
                            planet.AddHeightMapModLevel(i, 3);
                        if (planet.UpdateDirtyMeshes())
                            planet.factory.RenderLocalPlanetHeightmap();
                        if (planet.factory.platformSystem.reformData == null)
                            planet.factory.platformSystem.InitReformData();
                        restorewater = temprestorewater;
                    }
                    #endregion

                    planet.UnloadFactory();
                    int index = planet.factory.index;

                    for (int i = 1; i < GameMain.data.warningSystem.warningCursor; i++)
                    {
                        if (GameMain.data.warningSystem.warningPool[i].factoryId == index)
                        {
                            GameMain.data.warningSystem.RemoveWarningData(GameMain.data.warningSystem.warningPool[i].id);
                        }
                    }
                    lock (Beltsignal)
                    {
                        Beltsignal.Remove(index);
                    }
                    PlanetFactory planetFactory = new PlanetFactory();
                    PlanetAlgorithm planetAlgorithm = PlanetModelingManager.Algorithm(planet);
                    planetAlgorithm.GenerateVeins();
                    //PlanetModelingManager.Algorithm(planet).GenerateVegetables();
                    planetAlgorithm.GenerateTerrain(planet.mod_x, planet.mod_y);
                    planetAlgorithm.CalcWaterPercent();
                    planet.data.vegeCursor = 1;
                    planet.CalculateVeinGroups();
                    planetFactory.Init(GameMain.data, planet, index);
                    GameMain.data.factories[index] = planetFactory;
                    planet.factoryIndex = index;
                    planet.factory.platformSystem.EnsureReformData();
                    //GameMain.data.statistics.production.CreateFactoryStat(index);
                    planet.LoadFactory();
                    return;
                }
                if (type == 1) entityitemnoneed = true;
                foreach (EntityData etd in GameMain.localPlanet.factory.entityPool)
                {
                    int stationId = etd.stationId;
                    if (stationId > 0)
                    {
                        StationComponent sc = GameMain.localPlanet.factory.transport.stationPool[stationId];
                        for (int i = 0; i < sc.storage.Length && type == 0; i++)
                        {
                            int package = player.TryAddItemToPackage(sc.storage[i].itemId, sc.storage[i].count, 0, true, etd.id);
                            UIItemup.Up(sc.storage[i].itemId, package);
                        }
                        sc.storage = new StationStore[sc.storage.Length];
                        sc.needs = new int[sc.needs.Length];
                    }
                    if (type == 1)
                    {
                        GameMain.localPlanet.factory.RemoveEntityWithComponents(etd.id, false);
                    }
                    else
                    {
                        player.controller.actionBuild.DoDismantleObject(etd.id);
                    }
                }
                entityitemnoneed = false;
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

        /// <summary>
        /// 清空背包
        /// </summary>
        public void Clearpackage()
        {
            if (player == null) return;
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
        /// 控制背包大小大小
        /// </summary>
        /// <param name="rownum"></param>
        /// <param name="colnum"></param>
        /// <param name="add"></param>
        public void Operatepackagesize(int rownum, int colnum, bool add = true)
        {
            if (player == null) return;
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
        /// 解锁全部科技
        /// </summary>
        /// <param name="addstat">是否补充统计信息</param>
        public void UnlockallTech(bool addstat = false)
        {
            if (player == null) return;
            bool end = true;
            while (end)
            {
                end = false;
                foreach (TechProto techProto in new List<TechProto>(LDB.techs.dataArray))
                {
                    if (GameMain.data.history.TechUnlocked(techProto.ID) || techProto.IsObsolete) continue;
                    if (!GameMain.data.history.CanEnqueueTech(techProto.ID)) continue;
                    if (techProto.MaxLevel > 15) continue;
                    end = true;
                    if (techProto.Level < techProto.MaxLevel)
                    {
                        for (int level = techProto.Level; level < techProto.MaxLevel; ++level)
                        {
                            if (addstat)
                            {
                                for (int i = 0; i < techProto.itemArray.Length; i++)
                                {
                                    AddComsumeItemtoTotal(techProto.Items[i], (int)(techProto.ItemPoints[i] * techProto.GetHashNeeded(techProto.Level) / 3600));
                                }
                            }
                            for (int index = 0; index < techProto.UnlockFunctions.Length; ++index)
                            {
                                if (techProto.UnlockFunctions[index] == 7)
                                {
                                    //Debug.Log(techProto.Name);
                                }
                                GameMain.history.UnlockTechFunction(techProto.UnlockFunctions[index], techProto.UnlockValues[index], level);
                            }
                        }
                    }
                    if (!GameMain.history.techStates[techProto.ID].unlocked)
                    {
                        if (addstat)
                        {
                            for (int i = 0; i < techProto.itemArray.Length; i++)
                            {
                                AddComsumeItemtoTotal(techProto.Items[i], (int)(techProto.ItemPoints[i] * techProto.GetHashNeeded(techProto.Level) / 3600));
                            }
                        }
                        for (int index = 0; index < techProto.UnlockFunctions.Length; ++index)
                        {
                            if (techProto.UnlockFunctions[index] == 7)
                            {
                                //Debug.Log(techProto.Name);
                            }
                        }
                        GameMain.history.UnlockTech(techProto.ID);
                    }

                }
            }
        }

        /// <summary>
        /// 回退无穷科技
        /// </summary>
        public void lockTech()
        {
            TechProto[] dataArray = LDB.techs.dataArray;
            for (int index = 0; index < dataArray.Length; index++)
            {
                TechProto tp = dataArray[index];
                if (tp.MaxLevel > 30 && tp.Level != tp.MaxLevel)
                {
                    TechState techState = GameMain.history.techStates[tp.ID];
                    techState.unlocked = false;
                    techState.curLevel = tp.Level;
                    techState.hashUploaded = 0L;
                    techState.hashNeeded = tp.GetHashNeeded(techState.curLevel);
                    GameMain.history.techStates[tp.ID] = techState;
                }
            }
        }

        /// <summary>
        /// 鼠标中间锁定玩家背包
        /// </summary>
        public void InfiniteFilterThingInPackage()
        {
            if (GameMain.mainPlayer == null) return;
            StorageComponent.GRID[] grids = GameMain.mainPlayer.package.grids;
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
                GameMain.mainPlayer.package.NotifyStorageChange();
            }
        }

        /// <summary>
        /// 背包无限物品
        /// </summary>
        public void InfiniteAllThingInPackage()
        {
            if (GameMain.mainPlayer == null) return;
            StorageComponent.GRID[] grids = GameMain.mainPlayer.package.grids;
            if (grids.Length < LDB.items.dataArray.Length)
            {
                GameMain.mainPlayer.package.SetSize((LDB.items.dataArray.Length / player.packageColCount + 1) * player.packageColCount);
            }
            int i = 0;
            bool changed = false;
            foreach (ItemProto ip in LDB.items.dataArray)
            {
                if (ip.ID == 1099)
                {
                    continue;
                }
                if (grids[i].itemId != ip.ID || grids[i].count != StorageComponent.itemStackCount[ip.ID] || grids[i].stackSize != StorageComponent.itemStackCount[ip.ID])
                {
                    changed = true;
                    grids[i].itemId = ip.ID;
                    grids[i].stackSize = StorageComponent.itemStackCount[ip.ID];
                    grids[i].count = StorageComponent.itemStackCount[ip.ID];
                }
                i++;
                if (i == grids.Length) break;
            }
            if (changed)
            {
                GameMain.mainPlayer.package.NotifyStorageChange();
            }
        }

        /// <summary>
        /// 设置无人机消耗
        /// </summary>
        public void SetDronenocomsume()
        {
            if (player.mecha != null)
            {
                if (DroneNoenergy_bool.Value && player.mecha.droneEjectEnergy != 0)
                {
                    player.mecha.droneEjectEnergy = 0;
                    player.mecha.droneEnergyPerMeter = 0;
                }
                if (!DroneNoenergy_bool.Value && player.mecha.droneEjectEnergy == 0)
                {
                    player.mecha.droneEjectEnergy = 300000;
                    player.mecha.droneEnergyPerMeter = 60000;
                }
            }
        }

        #region 配方修改

        /// <summary>
        /// 配方相关修改
        /// </summary>
        public void ChangeRecipe()
        {
            //新配方设置
            //{
            //    RecipeProto rp = new RecipeProto();
            //    rp.ID = 999;
            //    rp.name = "GreenTech";
            //    rp.Type = ERecipeType.Assemble;
            //    rp.Handcraft = true;
            //    rp.Explicit = true;
            //    rp.TimeSpend = 60;
            //    rp.Items = new int[1] { 6005 };
            //    rp.ItemCounts = new int[1] { 2 };
            //    rp.Results = new int[2] { 1209, 1305 };
            //    rp.ResultCounts = new int[2] { 10, 1 };
            //    rp.GridIndex = 1609;
            //    list.Add(rp);

            //    temp.AddRange(list);
            //    LDB._recipes.dataArray = temp.ToArray();
            //    LDB._recipes.OnAfterDeserialize();
            //}



            for (int i = 0; i < LDB.recipes.dataArray.Length && i < originRecipeProtos.Length; i++)
            {
                var recipe = LDB.recipes.dataArray[i];
                recipe.Handcraft = allhandcraft.Value || originRecipeProtos[i].Handcraft;
                recipe.TimeSpend = quickproduce.Value ? 10 : originRecipeProtos[i].TimeSpend;
            }
            //配方信息查询
            foreach (RecipeProto rp in LDB.recipes.dataArray)
            {
                //获取配方数据
                {
                    //string itemstr = "";
                    //string itemcountstr = "";
                    //string resultstr = "";
                    //string resultcount = "";
                    //foreach (int i in rp.Items)
                    //{
                    //    itemstr = itemstr + i + " ";
                    //}
                    //foreach (int i in rp.ItemCounts)
                    //{

                    //    itemcountstr = itemcountstr + i + " ";
                    //}
                    //foreach (int i in rp.Results)
                    //{
                    //    resultstr = resultstr + i + " ";
                    //}
                    //foreach (int i in rp.ResultCounts)
                    //{
                    //    resultcount = resultcount + i + " ";
                    //}
                    //Logger.LogInfo(rp.ID + "," + rp.name + "," + rp.Type + "," + rp.Name + "," + rp.Handcraft + "," + rp.Explicit + ","
                    //    + rp.TimeSpend + ";" + itemstr + ";" + itemcountstr + ";" + resultstr + ";" + resultcount + ";" + rp.GridIndex);
                }

            }
        }
        #endregion

        #region 物品修改
        /// <summary>
        /// 设置物品堆叠倍数
        /// </summary>
        /// <param name="multiple"></param>
        public void SetmultipleItemStatck(int multiple)
        {
            int itemsLength = LDB.items.dataArray.Length;
            for (int i = 0; i < itemsLength; i++)
            {
                var item = LDB.items.dataArray[i];
                StorageComponent.itemStackCount[item.ID] = item.StackSize * multiple;
            }
            StackMultiple.Value = multiple;
        }
        /// <summary>
        /// 物品相关修改
        /// </summary>
        public void ChangeItemstack()
        {
            //foreach (ItemProto ip in itemProtos)
            //{
            //    Debug.Log(ip.ID + " " + ip.Name);
            //    ip.StackSize *= StackMultiple.Value;
            //    if (ip.prefabDesc != null)
            //    {
            //        Debug.Log(ip.name + " " + ip.prefabDesc.isInserter);
            //    }
            //    if (ip.CanBuild)
            //    {
            //        ip.BuildInGas = true;
            //        if (ip.prefabDesc != null)
            //        {
            //            ip.prefabDesc.workEnergyPerTick = 0;
            //            ip.prefabDesc.idleEnergyPerTick = 0;
            //        }
            //    }
            //    string t = ip.Description + ";" + ip.ID + ";" + ip.GridIndex + ";" + ip.HeatValue + ";" + ip.Potential + ";" + ip.StackSize;

            //    Logger.LogInfo(t);
            //}
        }
        #endregion

        #region 海洋类型设置
        /// <summary>
        /// 导入海洋类型数据
        /// </summary>
        private void InitWaterTypes()
        {
            string[] seedPlanetWaters = seedPlanetWater.Value.Split('|');
            foreach (var seedPlanetWaterTypestr in seedPlanetWaters)
            {
                if (string.IsNullOrEmpty(seedPlanetWaterTypestr)) continue;
                var seedPlanetWater = new SeedPlanetWater();
                string[] values = seedPlanetWaterTypestr.Split(',');
                if (values[0] == "0") continue;
                if (values[1].Length > 0)
                {
                    seedPlanetWater.seedKey64 = Convert.ToInt64(values[0]);
                    string[] planetwatertypes = values[1].Split('-');
                    foreach (var planetwatertypestr in planetwatertypes)
                    {
                        if (string.IsNullOrEmpty(planetwatertypestr)) continue;
                        string[] watervalues = planetwatertypestr.Split(':');
                        seedPlanetWater.waterTypes.Add(Convert.ToInt32(watervalues[0]), Convert.ToInt32(watervalues[1]));
                    }
                }
                if (seedPlanetWater.seedKey64 != 0)
                {
                    seedPlanetWaterTypes.Add(seedPlanetWater.seedKey64, seedPlanetWater);
                }
            }
        }

        /// <summary>
        /// 设置海洋类型
        /// </summary>
        public void SetWaterType()
        {
            if (GameMain.localPlanet == null) return;
            var item = LDB.items.dataArray.ToList().Find(x => x.name == currentPlanetWaterType);
            if (item == null)
            {
                UIMessageBox.Show("", "设置海洋类型失败".getTranslate(), "确定".Translate(), 3);
                return;
            }
            if (GameMain.localPlanet.waterItemId == item.ID) return;
            GameMain.localPlanet.waterItemId = item.ID;
            int planetId = GameMain.localPlanet.id;
            int originwaterId = originWaterTypes.waterTypes[planetId];
            if (originwaterId == item.ID && currentWaterTypes.waterTypes.ContainsKey(planetId))
            {
                currentWaterTypes.waterTypes.Remove(planetId);
            }
            else if (originwaterId != item.ID)
            {
                if (!currentWaterTypes.waterTypes.ContainsKey(planetId))
                    currentWaterTypes.waterTypes.Add(planetId, item.ID);
                else
                    currentWaterTypes.waterTypes[planetId] = item.ID;
            }

            string result = "";
            foreach (var seedPlanetWaterType in seedPlanetWaterTypes)
            {
                if (seedPlanetWaterType.Value.seedKey64 == 0 || seedPlanetWaterType.Value.waterTypes.Count == 0) continue;
                result += seedPlanetWaterType.Value.ToStr() + '|';
            }
            seedPlanetWater.Value = result;
        }

        /// <summary>
        /// 还原海洋类型
        /// </summary>
        public void RestoreWaterType()
        {
            string result = "";
            foreach (var keyvalue in currentWaterTypes.waterTypes)
            {
                if (originWaterTypes.waterTypes.ContainsKey(keyvalue.Key))
                {
                    GameMain.galaxy.PlanetById(keyvalue.Key).waterItemId = originWaterTypes.waterTypes[keyvalue.Key];
                    if (GameMain.localPlanet.id == keyvalue.Key)
                    {
                        currentPlanetWaterType = LDB.items.Select(originWaterTypes.waterTypes[keyvalue.Key])?.name ?? "无海洋".getTranslate();
                    }
                }
            }
            currentWaterTypes.waterTypes.Clear();
            foreach (var seedPlanetWaterType in seedPlanetWaterTypes)
            {
                if (seedPlanetWaterType.Value.seedKey64 == 0 || seedPlanetWaterType.Value.waterTypes.Count == 0) continue;
                result += seedPlanetWaterType.Value.ToStr() + '|';
            }
            seedPlanetWater.Value = result;
        }
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