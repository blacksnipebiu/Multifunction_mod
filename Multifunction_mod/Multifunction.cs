using BepInEx;
using BepInEx.Configuration;
using Multifunction_mod.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using static Multfunction_mod.Constant;
using static Multfunction_mod.Multifunctionpatch;

namespace Multfunction_mod
{
    [BepInPlugin(GUID, NAME, VERSION)]
    public class Multifunction : BaseUnityPlugin
    {
        public const string GUID = "cn.blacksnipe.dsp.Multfuntion_mod";
        public const string NAME = "Multfuntion_mod";
        public const string VERSION = "2.8.3";

        #region 临时变量

        public Light SunLight;
        private bool coolDown;
        public bool CoolDown
        {
            get => coolDown;
            set
            {
                coolDown = value;
                if (value)
                {
                    Task.Run(() =>
                    {
                        Thread.Sleep(1000);
                        coolDown = false;
                    });
                }
            }
        }
        public static GUIDraw guidraw;
        public static PropertyData propertyData;
        public Texture2D mytexture;
        public ItemProto[] itemProtos => LDB.items.dataArray;

        public static bool Itemdelete_bool;

        public ItemProto[] originItemProtos;
        public RecipeProto[] originRecipeProtos;
        public static Player player;
        public List<int[]> packageitemlist = new List<int[]>();
        public static List<Tempsail> tempsails = new List<Tempsail>();
        public static Dictionary<int, int> tmp_levelChanges;
        public VeinData pointveindata;
        public static KeyboardShortcut tempShowWindow;
        public int oillowerlimit;
        public static int veintype = 1;
        public static int DriftBuildingLevel;
        public static int incAbility = 4;

        public float tempx1;
        public float tempy1;
        public static float buildheight = 1;
        public static float[] warpstationqua;
        public static float[] warpsuperstationqua;
        private static bool LDBInitSave;
        public static bool refreshLDB;
        public static bool GameRunning;
        public static bool firstrestorewater = true;
        public static bool loadbutton;
        public static bool refreshPlayerData;
        public static bool RefreshStationStorage;
        public static bool Window_moving;
        public static bool changequapowerpertick;
        public static bool StationfullCount;
        public static bool ChangeQuickKey;
        public static bool ChangingQuickKey;
        public static bool leftscaling;
        public static bool rightscaling;
        public static bool bottomscaling;
        public static bool dropdownbutton;
        public static bool addveinbool;
        public static bool buildnotimecolddown;
        public static bool SandBoxMode;
        public static bool blueprintPasteToolActive;
        public static bool changexveinspos;
        public static bool changeveinposbool;
        public static bool changeveingroupposbool;
        public static bool getallVein_bool;
        public static int EjectorNumber;
        public static int SiloNumber;
        public static bool DriftBuildings;
        public static bool Property9999999;
        public static bool playcancelsolarbullet;
        public static bool alwaysemissiontemp;
        public static bool FinallyInit;
        public static bool lighton;
        public static bool DisplayingWindow;
        public static bool ItemDisplayingWindow;
        public static bool restorewater;
        public static bool entityitemnoneed;
        public static bool quickEjector;
        public static bool quicksilo;
        public static bool ready;
        public static bool temp;
        public static bool farconnectdistance;
        public static bool pasteanyway;
        public static bool PasteBuildAnyWay;
        public static bool closeallcollider;
        public static bool unlockpointtech;
        public static bool autochangeQuantumstationname;
        public static bool autochangeQuantumStarstationname;
        public static bool TempEjectorRandomEmission;
        public static bool TempSiloRandomEmission;
        public string watertype = "";
        public static Dictionary<int, Dictionary<int, long>> addStatDic = new Dictionary<int, Dictionary<int, long>>();
        public static Dictionary<int, Dictionary<int, long>> consumeStatDic = new Dictionary<int, Dictionary<int, long>>();
        public static Dictionary<int, Dictionary<int, int>> Beltsignal = new Dictionary<int, Dictionary<int, int>>();
        public static Dictionary<int, Dictionary<int, int>> Beltsignalnumberoutput = new Dictionary<int, Dictionary<int, int>>();
        #endregion
        #region 配置菜单
        public static ConfigEntry<KeyboardShortcut> WindowQuickKey;
        public static ConfigEntry<int> veinlines;
        public static ConfigEntry<int> scale;
        public static ConfigEntry<int> MULTIPELSMELT;
        public static ConfigEntry<int> StackMultiple;
        public static ConfigEntry<int> StationStoExtra;
        public static ConfigEntry<int> Stationminenumber;
        public static ConfigEntry<int> Buildmaxlen;
        public static ConfigEntry<int> Quantumenergy;
        public static ConfigEntry<int> changeveinsposx;
        public static ConfigEntry<int> MaxOrbitRadiusConfig;
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
        public static ConfigEntry<bool> WindturbinesUnlimitedEnergy;
        public static ConfigEntry<bool> Windturbinescovertheglobe;
        public static ConfigEntry<bool> NotTidyVein;
        public static ConfigEntry<bool> InspectDisNoLimit;
        public static ConfigEntry<bool> RandomEmission;
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
        public static ConfigEntry<bool> noneedtrashsand;
        public static ConfigEntry<bool> InfineteStarPower;
        public static ConfigEntry<bool> PlanetPower_bool;
        public static ConfigEntry<bool> allhandcraft;
        public static ConfigEntry<bool> quickproduce;
        public static ConfigEntry<bool> lockpackage_bool;
        public static ConfigEntry<bool> blueprintpastenoneed_bool;
        public static ConfigEntry<bool> noneedwarp;
        public static ConfigEntry<bool> Infinitething;
        public static ConfigEntry<bool> Infiniteplayerpower;
        public static ConfigEntry<bool> deleteveinbool;
        public static ConfigEntry<bool> MechalogisticsPlanet_bool;
        public static ConfigEntry<bool> StationMiner;
        public static ConfigEntry<bool> autochangestationname;
        public static ConfigEntry<bool> dismantle_but_nobuild;
        public static ConfigEntry<bool> build_gascol_noequator;
        public static ConfigEntry<bool> StationTrash;
        public static ConfigEntry<bool> build_tooclose_bool;
        public static ConfigEntry<bool> sunlight_bool;
        public static ConfigEntry<bool> DroneNoenergy_bool;
        public static ConfigEntry<bool> BuildNotime_bool;
        public static ConfigEntry<bool> Station_infiniteWarp_bool;
        public static ConfigEntry<bool> StationfullCount_bool;
        public static ConfigEntry<bool> ItemList_bool;
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
        public static ConfigEntry<bool> QuantumtransportstationSupply;
        public static ConfigEntry<bool> QuantumtransportCollectorSupply;
        public static ConfigEntry<bool> QuantumtransportlabSupply;
        public static ConfigEntry<bool> QuantumtransportpowerSupply;
        public static ConfigEntry<bool> QuantumtransportassembleSupply;
        public static ConfigEntry<bool> QuantumtransportstationDemand;
        public static ConfigEntry<bool> QuantumtransportminerDemand;
        public static ConfigEntry<bool> QuantumtransportsiloDemand;
        public static ConfigEntry<bool> QuantumtransportlabDemand;
        public static ConfigEntry<bool> QuantumtransportpowerDemand;
        public static ConfigEntry<bool> QuantumtransportassembleDemand;
        #endregion

        void Start()
        {
            preparedraw();
            Patchallmethod();
            MultifunctionTranslate.regallTranslate();
            AssetBundle assetBundle = AssetBundle.LoadFromStream(Assembly.GetExecutingAssembly().GetManifestResourceStream("Multifunction_mod.multifunctionpanel"));
            var MultiFunctionPanel = assetBundle.LoadAsset<GameObject>("MultiFunctionPanel");
            var temppanel = UnityEngine.Object.Instantiate<GameObject>(MultiFunctionPanel, UIRoot.instance.overlayCanvas.transform);
            temppanel.SetActive(false);
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


                ArchitectMode = Config.Bind("建筑师模式", "ArchitectMode", false);
                Quantumenergy = Config.Bind("量子耗能", "Quantumenergy", 1000000);
                StationfullCount_bool = Config.Bind("星球无限供货机", "StationfullCount_bool", false);
                InfineteStarPower = Config.Bind("人造卫星无限能源", "InfineteStarPower", false);
                WindturbinesUnlimitedEnergy = Config.Bind("风力涡轮机无限能源", "WindturbinesUnlimitedEnergy", false);
                Windturbinescovertheglobe = Config.Bind("风力涡轮机覆盖全球", "Windturbinescovertheglobe", false);
                Infinitestoragetank = Config.Bind("无限储液站", "Infinitestoragetank", false);
                TankMaxproliferator = Config.Bind("储液站无限增产", "TankMaxproliferator", false);
                Tankcontentall = Config.Bind("储液站任意存", "Tankcontentall", false);
                allhandcraft = Config.Bind("全部手搓", "allhandcraft", false);
                quickproduce = Config.Bind("快速生产", "quickproduce", false);
                noneedwarp = Config.Bind("无翘曲器曲速", "noneedwarp", false);

                Mechalogneed = Config.Bind("机甲物流需求情况", "Mechalogneed", "");
                changeveinsposx = Config.Bind("切割矿脉数量", "changeveinsposx", 3);
                veinlines = Config.Bind("矿物行数", "veinlines", 3);
                NotTidyVein = Config.Bind("矿堆不整理", "NotTidyVein", false);
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
                deleteveinbool = Config.Bind("删除矿物", "deleteveinbool", false);
                StationMiner = Config.Bind("星球矿机", "stationmineropen", false);
                StationTrash = Config.Bind("星球垃圾箱", "stationtrashopen", false);
                autochangestationname = Config.Bind("自动改名", "autochangestationname", false);
                Buildingnoconsume = Config.Bind("全设备不耗电", "Buildingnoconsume", false);
                Stationfullenergy = Config.Bind("物流站永久满电", "Stationfullenergy", false);
                StationSpray = Config.Bind("物流站喷涂", "StationSpray", false);
                StationPowerGen = Config.Bind("物流站内置发电", "StationPowerGen", false);
                build_gascol_noequator = Config.Bind("采集器无视赤道", "build_gascol_noequator", false);
                lockpackage_bool = Config.Bind("锁定背包", "lockpackage_bool", false);
                QuickHandcraft = Config.Bind("机甲制造MAX", "QuickHandcraft", false);
                QuickPlayerMine = Config.Bind("机甲采矿Max", "QuickPlayerMine", false);
                InspectDisNoLimit = Config.Bind("操作范围不受限制", "InspectDisNoLimit", false);
                noneedtrashsand = Config.Bind("不需要垃圾沙土", "noneedtrashsand", false);
                dismantle_but_nobuild = Config.Bind("拆除不添加至背包", "dismantle_but_nobuild", false);
                ItemList_bool = Config.Bind("物品列表", "ItemList_bool", false);
                sunlight_bool = Config.Bind("日光灯", "sunlight_bool", false);
                DroneNoenergy_bool = Config.Bind("小飞机不耗能", "DroneNoenergy_bool", false);
                Station_infiniteWarp_bool = Config.Bind("星际运输站无限曲速", "Station_infiniteWarp_bool", false);
                BuildNotime_bool = Config.Bind("建筑秒完成", "BuildNotime_bool", false);
                PlanetPower_bool = Config.Bind("星球电网", "PlanetPower_bool", false);
                build_tooclose_bool = Config.Bind("强行近距离建造物流站", "build_tooclose_bool", false);
                blueprintpastenoneed_bool = Config.Bind("蓝图建造无需材料", "blueprintpastenoneed_bool", false);
                quickabsorbsolar = Config.Bind("跳过太阳帆吸收阶段", "quickabsorbsolar", false);
                cancelsolarbullet = Config.Bind("跳过太阳帆子弹阶段", "cancelsolarbullet", false);
                alwaysemission = Config.Bind("全球打帆", "alwaysemission", false);
                RandomEmission = Config.Bind("随机发射", "RandomEmission", false);
                QuickabortSwarm = Config.Bind("太阳帆秒吸收", "QuickabortSwarm", false);
                ChangeDysonradius = Config.Bind("改变戴森球半径上下限", "ChangeDysonradius", false);
                MaxOrbitRadiusConfig = Config.Bind("戴森球最大半径", "MaxOrbitRadiusConfig", 1000000);
                StationStoExtra = Config.Bind("运输站额外储量", "StationStoExtra", 0);
                StackMultiple = Config.Bind("堆叠倍数", "StackMultiple", 1);
                Stationminenumber = Config.Bind("星球矿机速率", "Stationminenumber", 1);
                MULTIPELSMELT = Config.Bind("冶炼倍数", "mutiplesmelt", 1);
                Buildmaxlen = Config.Bind("建筑数量最大值", "Buildmaxlen", 15);
                Maxproliferator = Config.Bind("增产点数上限10", "Maxproliferator", false);
                starquamaxpowerpertick = Config.Bind("实时修改星球量子充电功率", "starquamaxpowerpertick", 60f);
                planetquamaxpowerpertick = Config.Bind("实时修改星系量子充电功率", "planetquamaxpowerpertick", 60f);
                CloseUIAbnormalityTip = Config.Bind("关闭异常提示", "CloseUIAbnormalityTip", false);
                Textcolor = Config.Bind("字体颜色", "Textcolor", Color.white);
                mainWindowTextureColor_config = Config.Bind("窗口材质颜色", "mainWindowTextureColor", Color.black);

                MainWindow_x_config = Config.Bind("第一窗口x", "xl_SimpleUI_1_x_config", 448.0f);
                MainWindow_y_config = Config.Bind("第一窗口y", "xl_SimpleUI_1_y_config", 199.0f);
                MainWindow_width = Config.Bind("第一窗口宽度", "xl_SimpleUI_1_x_config", 1400.0f);
                MainWindow_height = Config.Bind("第一窗口高度", "xl_SimpleUI_1_y_config", 1000.0f);
                WindowQuickKey = Config.Bind("打开窗口快捷键", "Key", new BepInEx.Configuration.KeyboardShortcut(KeyCode.Alpha1, KeyCode.LeftAlt));
            }
            #endregion

            guidraw = new GUIDraw(Math.Max(5, Math.Min(scale.Value, 35)), temppanel, this);
            incAbility = Maxproliferator.Value ? 10 : 4;
            oillowerlimit = (int)(0.1 / VeinData.oilSpeedMultiplier);
            tempShowWindow = WindowQuickKey.Value;

            CollectorStation = new List<int>();
            StartCoroutine(Timer());
        }

        void Update()
        {
            ChangeQuickKeyMethod();
            RestoreWaterMethod();
            FirstStartGame();
            QuickKeyOpenWindow();
            AfterGameStart();
        }

        public void OnGUI()
        {
            guidraw.Draw();
            //如果物品列表开着，按ctrl或shift能够删除选中物品
            if (guidraw.TabDisplayingWindow && (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.LeftShift)))
            {
                Itemdelete_bool = true;
                player.SetHandItems(0, 0);
            }
            else
            {
                Itemdelete_bool = false;
            }
        }


        #region 量子传输站

        public static List<int> CollectorStation = new List<int>();
        public static List<int> SuperStation = new List<int>();
        public static List<int> StarSuperStation = new List<int>();
        public static Dictionary<int, int[]> StarSuperStationItemidstore = new Dictionary<int, int[]>();
        public static Dictionary<int, Dictionary<int, List<int[]>>> PlanetSuperStationItemidstore = new Dictionary<int, Dictionary<int, List<int[]>>>();
        public void takeitemfromstarsuperstation()
        {
            if (GameMain.data?.galacticTransport?.stationPool == null) return;
            StarSuperStation = new List<int>();
            SuperStation = new List<int>();
            StarSuperStationItemidstore = new Dictionary<int, int[]>();
            PlanetSuperStationItemidstore = new Dictionary<int, Dictionary<int, List<int[]>>>();
            for (int i = 0; i < GameMain.data.galacticTransport.stationPool.Length; i++)
            {
                StationComponent sc = GameMain.data.galacticTransport.stationPool[i];
                if (sc?.name == null || sc.storage == null || !sc.isStellar) continue;
                if (sc.name == "星系量子传输站")
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
                if (sc.name == "星球量子传输站")
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
            int SuperStationCount = SuperStation.Count;
            int StarSuperStationCount = StarSuperStation.Count;
            if (SuperStationCount == 0 && StarSuperStationCount == 0) return;
            changequapowerpertick = false;
            if (Math.Max(StarSuperStation.Count, SuperStation.Count) > warpstationqua.Length - 50)
                SetSuperStationCapacity(Math.Max(StarSuperStation.Count, SuperStation.Count) + 50);
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
                if (sc?.storage == null || sc?.name == "星球量子传输站") continue;
                for (int i = 0; i < sc.storage.Length; i++)
                {
                    ref StationStore store = ref sc.storage[i];
                    if (store.itemId <= 0)
                        continue;
                    if (QuantumtransportstationDemand.Value)
                    {
                        if (store.localLogic == ELogisticStorage.Supply && store.count > 0 && sc.name != "星系量子传输站")
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
                            if (ac.produced[i] > 0) ac.produced[i] -= doitemfromPlanetSuper(ac.products[i], ac.produced[i], pdid, 0, false)[0];
                            if (ac.produced[i] > 500) ac.produced[i] = 200;
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
                            fs.factory.powerSystem.genPool[pgc.id].SetNewFuel(itemid, (short)takeitem[0], (short)takeitem[1]);
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
            if (energycost != 0)
            {
                if (warpsuperstationqua[index] >= Time.time - 5)
                {
                    energycost = Quantumenergy.Value / 100;
                }
                else if (sc.warperCount > 0 && warpsuperstationqua[index] + 5 < Time.time)
                {
                    sc.warperCount--;
                    warpsuperstationqua[index] = Time.time;
                    energycost = Quantumenergy.Value / 100;
                    AddStatInfo(sc.planetId, 1210, 1, false);
                }
            }

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
            StationPowerGeneration(sc);
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
                if (energycost != 0)
                {
                    if (warpstationqua[index] >= Time.time - 5)
                        energycost = Quantumenergy.Value / 100;
                    else if (sc.warperCount > 0 && warpstationqua[index] + 5 < Time.time)
                    {
                        sc.warperCount--;
                        warpstationqua[index] = Time.time;
                        energycost = Quantumenergy.Value / 100;
                        AddStatInfo(sc.planetId, 1210, 1);
                    }
                }
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
                StationPowerGeneration(sc);
                return new int[2] { count, StationMaxproliferator.Value ? incAbility * count : inc };
            }
            return new int[2] { 0, 0 };
        }

        public void SetSuperStationCapacity(int newCapacity)
        {
            float[] warpstationquacurtime = warpstationqua;
            float[] warpsuperstationquacurtime = warpsuperstationqua;
            int len1 = warpstationquacurtime.Length;
            int len2 = warpsuperstationquacurtime.Length;
            warpstationqua = new float[newCapacity];
            warpsuperstationqua = new float[newCapacity];
            Array.Copy(warpstationquacurtime, warpstationqua, len1);
            Array.Copy(warpsuperstationquacurtime, warpsuperstationqua, len2);
        }

        #endregion

        #region 存档初始化
        /// <summary>
        /// 加载存档时进行初始化操作
        /// </summary>
        public void FirstStartGame()
        {
            if (GameMain.instance != null)
            {
                if (!GameMain.instance.running && FinallyInit)
                {
                    GameRunning = false;
                    FinallyInit = false;
                    closeallcollider = false;
                    tempsails = new List<Tempsail>();
                    addStatDic = new Dictionary<int, Dictionary<int, long>>();
                    Beltsignal = new Dictionary<int, Dictionary<int, int>>();
                    consumeStatDic = new Dictionary<int, Dictionary<int, long>>();
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
                }
                if (GameMain.instance.running && !FinallyInit)
                {
                    FinallyInit = true;
                    RandomEmissionEjectorSilo();
                    warpstationqua = new float[200];
                    warpsuperstationqua = new float[200];
                    for (int i = 0; i < warpstationqua.Length; i++)
                        warpstationqua[i] = Time.time;
                    for (int i = 0; i < warpsuperstationqua.Length; i++)
                        warpsuperstationqua[i] = Time.time;
                    if (BeltSignalFunction.Value)
                    {
                        InitBeltSignalDiction();
                    }

                    if (Quantumtransport_bool.Value)
                    {
                        InitQuantumTransport();
                    }
                    if (!LDBInitSave && LDB.items != null && LDB.recipes != null)
                    {
                        LDBInitSave = true;
                        originItemProtos = new ItemProto[LDB.items.dataArray.Length];
                        originRecipeProtos = new RecipeProto[LDB.recipes.dataArray.Length];
                        Array.Copy(LDB.items.dataArray, originItemProtos, originItemProtos.Length);
                        Array.Copy(LDB.recipes.dataArray, originRecipeProtos, originRecipeProtos.Length);
                    }
                    playcancelsolarbullet = cancelsolarbullet.Value;
                    alwaysemissiontemp = alwaysemission.Value;
                    //更改配方相关
                    ChangeRecipe();
                    //更改物品相关
                    SetmultipleItemStatck(StackMultiple.Value);
                    ChangeItemstack();
                }
            }

        }
        #endregion

        #region 游戏启动后
        public void AfterGameStart()
        {
            if (GameMain.history != null && FinallyInit)
            {
                StationComponentSet();
                PlayerDataInit();
                LockPlayerPackage();
                BluePrintpasteNoneed();
                if (player != null)
                {
                    GameRunning = true;
                    ControlVein();
                    Sunlightset();
                    SetDronenocomsume();
                    DriftBuild();
                    if (refreshLDB)
                    {
                        refreshLDB = false;
                        ChangeRecipe();
                    }
                    if (BuildNotime_bool.Value && GameMain.localPlanet?.factory != null)
                    {
                        if (GameMain.localPlanet.factory.prebuildCount > 0)
                        {
                            GameMain.localPlanet.factory.BeginFlattenTerrain();
                            for (int i = 1; i < GameMain.localPlanet.factory.prebuildCursor; i++)
                            {
                                if (GameMain.localPlanet.factory.prebuildPool[i].itemRequired > 0 && !Infinitething.Value && !ArchitectMode.Value) continue;
                                int preid = GameMain.localPlanet.factory.prebuildPool[i].id;
                                if (preid == i)
                                {
                                    GameMain.localPlanet.factory.BuildFinally(GameMain.mainPlayer, preid, false);
                                }
                            }
                            GameMain.localPlanet.factory.EndFlattenTerrain();
                        }
                    }
                }
            }
        }
        #endregion

        /// <summary>
        /// 定时任务
        /// </summary>
        private IEnumerator Timer()
        {
            while (true)
            {
                if (FinallyInit)
                {
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

                    //星球矿机
                    if (StationMiner.Value)
                    {
                        StationMine();
                    }
                }
                // 等待指定的时间
                yield return new WaitForSeconds(1);
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
            InspectDisNoLimit.Value = false;
            RandomEmission.Value = false;
            changexveinspos = false;
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
            noneedtrashsand.Value = false;
            InfineteStarPower.Value = false;
            PlanetPower_bool.Value = false;
            allhandcraft.Value = false;
            quickproduce.Value = false;
            lockpackage_bool.Value = false;
            blueprintpastenoneed_bool.Value = false;
            noneedwarp.Value = false;
            Infinitething.Value = false;
            Infiniteplayerpower.Value = false;
            deleteveinbool.Value = false;
            changeveinposbool = false;
            MechalogisticsPlanet_bool.Value = false;
            StationMiner.Value = false;
            autochangestationname.Value = false;
            changeveingroupposbool = false;
            dismantle_but_nobuild.Value = false;
            build_gascol_noequator.Value = false;
            StationTrash.Value = false;
            build_tooclose_bool.Value = false;
            sunlight_bool.Value = false;
            DroneNoenergy_bool.Value = false;
            BuildNotime_bool.Value = false;
            Station_infiniteWarp_bool.Value = false;
            StationfullCount_bool.Value = false;
            ItemList_bool.Value = false;
            getallVein_bool = false;
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
        }

        #region 开启窗口
        public void QuickKeyOpenWindow()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                guidraw.CloseMainWindow();
            }
            if (WindowQuickKey.Value.IsDown() && !ChangingQuickKey)
            {
                guidraw.MainWindowKeyInvoke();
            }

            if (player != null && player.controller != null && !player.controller.actionBuild.active)
            {
                if (ItemList_bool.Value && Input.GetKeyDown(KeyCode.Tab))
                {
                    guidraw.TabWindowKeyInvoke();
                }
                if (GameMain.localPlanet == null) ItemDisplayingWindow = false;
            }
            else
            {
                ItemDisplayingWindow = false;
            }
        }
        #endregion

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
                WindowQuickKey.Value = tempShowWindow;
                ChangingQuickKey = false;
            }
        }

        /// <summary>
        /// 控制建筑的高度
        /// </summary>
        public void DriftBuild()
        {
            if (player?.controller != null && player.controller.cmd.type == ECommand.Build && player.controller.actionBuild != null)
            {
                if (blueprintPasteToolActive != player.controller.actionBuild.blueprintPasteTool.active)
                {
                    blueprintPasteToolActive = !blueprintPasteToolActive;
                    DriftBuildingLevel = 0;
                }
                if (Input.GetKeyDown(KeyCode.UpArrow))
                {
                    DriftBuildingLevel++;
                    if (blueprintPasteToolActive)
                    {
                        player.controller.actionBuild.blueprintPasteTool.cursorValid = true;
                        player.controller.actionBuild.blueprintPasteTool.DeterminePreviewsPrestage();
                    }
                }
                if (Input.GetKeyDown(KeyCode.DownArrow))
                {
                    DriftBuildingLevel--;
                    if (blueprintPasteToolActive)
                    {
                        player.controller.actionBuild.blueprintPasteTool.cursorValid = true;
                        player.controller.actionBuild.blueprintPasteTool.DeterminePreviewsPrestage();
                    }
                }
            }
            else if (DriftBuildingLevel != 0)
            {
                DriftBuildingLevel = 0;
            }
        }

        /// <summary>
        /// 还原全部海洋类型
        /// </summary>
        public void RestoreWaterMethod()
        {
            if (restorewater && tmp_levelChanges != null && tmp_levelChanges.Count > 0 && Input.GetMouseButton(0))
            {
                foreach (KeyValuePair<int, int> tmpLevelChange in tmp_levelChanges)
                {
                    GameMain.localPlanet.AddHeightMapModLevel(tmpLevelChange.Key, tmpLevelChange.Value);
                }
            }
            if (restorewater && firstrestorewater)
            {
                firstrestorewater = false;
            }
            else if (!restorewater && !firstrestorewater)
            {
                firstrestorewater = true;
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
            if (player == null || player.mecha == null || player.mecha.idleDroneCount < player.mecha.droneCount) return;
            refreshPlayerData = true;
            ModeConfig freeMode = Configs.freeMode;
            GameHistoryData historyData = GameMain.history;
            Mecha mecha = player.mecha;
            mecha.coreEnergyCap = freeMode.mechaCoreEnergyCap;
            mecha.coreLevel = freeMode.mechaCoreLevel;
            mecha.miningSpeed = freeMode.mechaMiningSpeed;
            mecha.walkSpeed = freeMode.mechaWalkSpeed;
            mecha.jumpSpeed = freeMode.mechaJumpSpeed;
            mecha.droneCount = 0;
            mecha.droneCount = freeMode.mechaDroneCount;
            mecha.droneSpeed = freeMode.mechaDroneSpeed;
            mecha.droneMovement = freeMode.mechaDroneMovement;
            mecha.maxSailSpeed = freeMode.mechaSailSpeedMax;
            mecha.maxWarpSpeed = freeMode.mechaWarpSpeedMax;
            mecha.buildArea = freeMode.mechaBuildArea;
            mecha.replicateSpeed = freeMode.mechaReplicateSpeed;
            mecha.reactorPowerGen = freeMode.mechaReactorPowerGen;
            player.deliveryPackage.colCount = 0;
            player.deliveryPackage.stackSizeMultiplier = Configs.freeMode.deliveryPackageStackSizeMultiplier;
            int packagesize = freeMode.playerPackageSize;
            player.deliveryPackage.rowCount = (packagesize - 1) / 10 + 1;
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
            historyData.inserterStackCount = freeMode.inserterStackCount;
            historyData.logisticShipWarpDrive = freeMode.logisticShipWarpDrive;
            historyData.miningCostRate = freeMode.miningCostRate;
            historyData.miningSpeedScale = freeMode.miningSpeedScale;
            historyData.techSpeed = freeMode.techSpeed;
            historyData.storageLevel = 2;
            historyData.labLevel = 3;
            historyData.dysonNodeLatitude = 0;
            historyData.solarEnergyLossRate = 0.7f;
            historyData.solarSailLife = freeMode.solarSailLife;
            historyData.localStationExtraStorage = 0;
            historyData.remoteStationExtraStorage = 0;
            player.package.SetSize(packagesize);
            foreach (TechProto tp in new List<TechProto>(LDB.techs.dataArray))
            {
                if (tp.Level < tp.MaxLevel)
                {
                    for (int i = tp.Level; i < GameMain.history.techStates[tp.ID].curLevel + (GameMain.history.techStates[tp.ID].curLevel >= tp.MaxLevel ? 1 : 0); i++)
                        for (int j = 0; j < tp.UnlockFunctions.Length; j++)
                        {
                            //t +=tp.Name+" "+ tp.UnlockFunctions[j] + " " + tp.UnlockValues[j].ToString() + " ";
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
                        //t += tp.Name + " " + tp.UnlockFunctions[index] + " " + tp.UnlockValues[index].ToString() + " ";
                        GameMain.history.UnlockTechFunction(tp.UnlockFunctions[index], tp.UnlockValues[index], GameMain.history.techStates[tp.ID].maxLevel);
                    }
                }
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
                GameHistoryData historyData = GameMain.history;
                refreshPlayerData = false;
                player = GameMain.mainPlayer;
                propertyData = new PropertyData();
                propertyData.SetContent(player.mecha, historyData, player.deliveryPackage.stackSizeMultiplier);
                packageitemlist = new List<int[]>();
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
            if (!blueprintpastenoneed_bool.Value || GameMain.localPlanet?.factory == null) return;

            for (int i = 1; i < GameMain.localPlanet.factory.prebuildCursor; i++)
            {
                if (GameMain.localPlanet.factory.prebuildPool[i].protoId > 0)
                {
                    GameMain.localPlanet.factory.prebuildPool[i].itemRequired = 0;
                }
            }
        }

        /// <summary>
        /// 获取目标星球目标矿脉数量
        /// </summary>
        /// <param name="itemid" ></param>
        /// <param name="pdid"></param>
        /// <returns></returns>
        public int GetNumberOfVein(int itemid, PlanetData pd)
        {
            EVeinType evt = LDB.veins.GetVeinTypeByItemId(itemid);
            if (evt == EVeinType.None)
            {
                return 0;
            }
            if (evt == EVeinType.Oil)
            {
                long[] veinAmounts = new long[64];
                pd.CalcVeinAmounts(ref veinAmounts, new HashSet<int>(), UIRoot.instance.uiGame.veinAmountDisplayFilter);
                int collectspeed = (int)(veinAmounts[7] * VeinData.oilSpeedMultiplier + 0.5);
                if (collectspeed > 1) return collectspeed;
            }
            return pd.factory.veinPool.Count(x => x.type == evt);
        }

        /// <summary>
        /// 添加统计数据
        /// </summary>
        /// <param name="planetId"></param>
        /// <param name="itemId"></param>
        /// <param name="itemNum"></param>
        /// <param name="produce"></param>
        public static void AddStatInfo(int planetId, int itemId, int itemNum, bool produce = true)
        {
            if (addStatDic == null || consumeStatDic == null) return;
            if (produce)
            {
                if (planetId == 0) return;
                if (!addStatDic.ContainsKey(planetId))
                {
                    addStatDic.Add(planetId, new Dictionary<int, long>());
                }
                if (!addStatDic[planetId].ContainsKey(itemId))
                {
                    addStatDic[planetId].Add(itemId, itemNum);
                }
                else
                {
                    addStatDic[planetId][itemId] += itemNum;
                }
            }
            else
            {
                if (!consumeStatDic.ContainsKey(planetId))
                {
                    consumeStatDic.Add(planetId, new Dictionary<int, long>());
                }
                if (!consumeStatDic[planetId].ContainsKey(itemId))
                {
                    consumeStatDic[planetId].Add(itemId, itemNum);
                }
                else
                {
                    consumeStatDic[planetId][itemId] += itemNum;
                }
            }

        }

        /// <summary>
        /// 让弹射器和发射井不在同一时刻发射
        /// </summary>
        public void RandomEmissionEjectorSilo()
        {
            EjectorNumber = 0;
            SiloNumber = 0;
            if (!RandomEmission.Value) return;

            foreach (var sd in GameMain.galaxy.stars)
            {
                foreach (var pd in sd.planets)
                {
                    if (pd != null && pd.factory != null && pd.factory.factorySystem != null)
                    {
                        for (int l = 1; l < pd.factory.factorySystem.ejectorCursor; l++)
                        {
                            if (pd.factory.factorySystem.ejectorPool[l].id == l)
                            {
                                EjectorNumber++;
                            }
                        }
                        for (int l = 1; l < pd.factory.factorySystem.siloCursor; l++)
                        {
                            if (pd.factory.factorySystem.siloPool[l].id == l)
                            {
                                SiloNumber++;
                            }
                        }
                    }
                }
            }
            InitPatch();
            TempEjectorRandomEmission = true;
            TempSiloRandomEmission = true;
        }

        #region 矿脉管理
        public void ControlVein()
        {
            if (Input.GetMouseButton(0))
            {
                if (deleteveinbool.Value && GameMain.mainPlayer.controller.actionBuild.dismantleTool.active)
                {
                    removevein();
                }
                else if (addveinbool)
                {
                    addvein(veintype, 1000000000, new Vector3());
                }
            }
            if (Input.GetMouseButton(1))
            {
                addveinbool = false;
                changeveingroupposbool = false;
                changexveinspos = false;
                changeveinposbool = false;
                getallVein_bool = false;
                //changeveinposbool = false;
            }
            if (Input.GetMouseButton(0) && !player.controller.actionBuild.dismantleTool.active)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    pointveindata = getveinbymouse();
                }
                if (pointveindata.amount != 0)
                {
                    if (getallVein_bool)
                    {
                        getallVein(pointveindata);
                    }
                    else if (changeveingroupposbool)
                    {
                        changeveingrouppos(pointveindata);
                    }
                    else if (changexveinspos)
                    {
                        changexveins(pointveindata);
                    }
                    else if (changeveinposbool)
                    {
                        RaycastHit raycastHit1;
                        if (!Physics.Raycast(player.controller.mainCamera.ScreenPointToRay(Input.mousePosition), out raycastHit1, 800f, 8720, (QueryTriggerInteraction)2))
                            return;
                        Vector3 raycastpos = raycastHit1.point;
                        changeveinpos(pointveindata, raycastpos);
                    }
                }
            }
        }

        /// <summary>
        /// 添加矿脉
        /// </summary>
        /// <param name="veintype"></param>
        /// <param name="number"></param>
        /// <param name="pos"></param>
        public void addvein(int veintype, int number, Vector3 pos)
        {
            if (number == 0) return;
            PlanetData pd = GameMain.localPlanet;
            if (pd == null || pd.type == EPlanetType.Gas) return;
            RaycastHit raycastHit1;
            if (pd == null || !Physics.Raycast(player.controller.mainCamera.ScreenPointToRay(Input.mousePosition), out raycastHit1, 800f, 8720, (QueryTriggerInteraction)2))
                return;
            Vector3 raycastpos = raycastHit1.point;
            if (pos.magnitude == 0)
            {
                foreach (VeinData i in pd.factory.veinPool)
                {
                    if (i.type == EVeinType.None) continue;
                    if ((raycastpos - i.pos).magnitude < 1)
                    {
                        if (i.type != EVeinType.Oil)
                        {
                            if (pd.factory.veinPool[i.id].amount + number < 1000000000)
                            {
                                pd.factory.veinPool[i.id].amount += number;
                                pd.factory.veinGroups[i.groupIndex].amount += number;
                            }
                            else
                            {
                                pd.factory.veinGroups[i.groupIndex].amount += 1000000000 - pd.factory.veinPool[i.id].amount;
                                pd.factory.veinPool[i.id].amount = 1000000000;
                            }
                        }
                        else
                        {
                            pd.factory.veinPool[i.id].amount += (int)(1 / VeinData.oilSpeedMultiplier);
                            pd.factory.veinGroups[i.groupIndex].amount += (int)(1 / VeinData.oilSpeedMultiplier);
                        }
                        return;
                    }
                }
                pos = raycastpos;
            }
            pos = raycastpos;
            VeinData vein = new VeinData()
            {
                amount = veintype == 7 ? (int)(1 / VeinData.oilSpeedMultiplier) : number,
                type = (EVeinType)veintype,
                pos = pos,
                productId = LDB.veins.Select(veintype).MiningItem,
                modelIndex = (short)LDB.veins.Select(veintype).ModelIndex
            };
            vein.id = pd.factory.AddVeinData(vein);
            vein.colliderId = pd.physics.AddColliderData(LDB.veins.Select(veintype).prefabDesc.colliders[0].BindToObject(vein.id, 0, EObjectType.Vein, vein.pos, Quaternion.FromToRotation(Vector3.up, vein.pos.normalized)));
            vein.modelId = pd.factoryModel.gpuiManager.AddModel(vein.modelIndex, vein.id, vein.pos, Maths.SphericalRotation(vein.pos, UnityEngine.Random.value * 360f));
            vein.minerCount = 0;
            pd.factory.AssignGroupIndexForNewVein(ref vein);
            pd.factory.veinPool[vein.id] = vein;
            pd.factory.RefreshVeinMiningDisplay(vein.id, 0, 0);
            pd.factory.RecalculateVeinGroup(pd.factory.veinPool[vein.id].groupIndex);
        }

        public void changexveins(VeinData vd)
        {
            PlanetData pd = GameMain.localPlanet;
            if (pd == null || pd.type == EPlanetType.Gas) return;
            RaycastHit raycastHit1;
            if (pd == null || !Physics.Raycast(player.controller.mainCamera.ScreenPointToRay(Input.mousePosition), out raycastHit1, 800f, 8720, (QueryTriggerInteraction)2))
                return;
            if (pd.factory.veinGroups[pd.factory.veinPool[vd.id].groupIndex].count <= changeveinsposx.Value)
            {
                changeveingrouppos(vd);
                return;
            }

            Vector3 raycastpos = raycastHit1.point;
            VeinData[] veinPool = pd.factory.veinPool;
            int colliderId;
            Vector3 begin = veinPool[vd.id].pos;
            bool find = false;
            List<int> veinids = new List<int>();
            foreach (VeinData vd1 in veinPool)
            {
                if (vd1.pos == null || vd1.id <= 0) continue;
                if (vd1.groupIndex == vd.groupIndex)
                {
                    int VeinId = vd1.id;
                    if (vd.id == VeinId) find = true;
                    if (!find && veinids.Count == changeveinsposx.Value - 1) continue;
                    veinids.Add(vd1.id);
                    if (veinids.Count == changeveinsposx.Value) break;
                }
            }
            if (veinids.Count != changeveinsposx.Value) return;
            int index = 0;
            foreach (int VeinId in veinids)
            {
                veinPool[VeinId].pos = NotTidyVein.Value ? raycastpos : PostionCompute(begin, raycastpos, veinPool[VeinId].pos, index++);
                if (float.IsNaN(veinPool[VeinId].pos.x) || float.IsNaN(veinPool[VeinId].pos.y) || float.IsNaN(veinPool[VeinId].pos.z))
                {
                    continue;
                }
                colliderId = veinPool[VeinId].colliderId;
                pd.physics.RemoveColliderData(colliderId);
                veinPool[VeinId].colliderId = pd.physics.AddColliderData(LDB.veins.Select((int)veinPool[VeinId].type).prefabDesc.colliders[0].BindToObject(VeinId, 0, EObjectType.Vein, veinPool[VeinId].pos, Quaternion.FromToRotation(Vector3.up, veinPool[VeinId].pos.normalized)));

                pd.factoryModel.gpuiManager.AlterModel(veinPool[VeinId].modelIndex, veinPool[VeinId].modelId, VeinId, veinPool[VeinId].pos, Maths.SphericalRotation(veinPool[VeinId].pos, 90f));

            }
            bool leave = true;
            foreach (VeinData vd1 in veinPool)
            {
                if (veinids.Contains(vd1.id) || vd1.type != vd.type)
                {
                    continue;
                }
                else if ((pd.factory.veinPool[vd.id].pos - vd1.pos).magnitude < 5)
                {
                    leave = false;
                    break;
                }
            }
            if (leave)
            {
                int origingroup = pd.factory.veinPool[vd.id].groupIndex;
                pd.factory.veinPool[vd.id].groupIndex = (short)pd.factory.AddVeinGroup(vd.type, vd.pos.normalized);
                foreach (int veinid in veinids)
                {
                    if (veinid == vd.id) continue;
                    else
                    {
                        pd.factory.veinPool[veinid].groupIndex = pd.factory.veinPool[vd.id].groupIndex;
                    }
                }
                pd.factory.RecalculateVeinGroup(pd.factory.veinPool[vd.id].groupIndex);
                pd.factory.RecalculateVeinGroup(origingroup);
                pd.factory.ArrangeVeinGroups();
            }

        }

        public void changeveingrouppos(VeinData vd)
        {
            if (CoolDown) return;
            PlanetData pd = GameMain.localPlanet;
            if (pd == null || pd.type == EPlanetType.Gas) return;
            RaycastHit raycastHit1;
            if (pd == null || !Physics.Raycast(player.controller.mainCamera.ScreenPointToRay(Input.mousePosition), out raycastHit1, 800f, 8720, (QueryTriggerInteraction)2))
                return;
            Vector3 raycastpos = raycastHit1.point;
            VeinData[] veinPool = pd.factory.veinPool;
            int colliderId;
            Vector3 begin = veinPool[vd.id].pos;
            int index = 0;
            foreach (VeinData vd1 in veinPool)
            {
                if (vd1.pos == null || vd1.id <= 0) continue;
                int VeinId = vd1.id;
                if (vd1.groupIndex == veinPool[vd.id].groupIndex)
                {
                    if (NotTidyVein.Value)
                    {
                        veinPool[VeinId].pos = raycastpos;
                    }
                    else
                    {
                        Vector3 temp = PostionCompute(begin, raycastpos, vd1.pos, index++, vd.type == EVeinType.Oil);
                        if (CoolDown) return;
                        if (Vector3.Distance(temp, vd1.pos) < 0.01) continue;
                        veinPool[VeinId].pos = temp;
                        if (float.IsNaN(veinPool[VeinId].pos.x) || float.IsNaN(veinPool[VeinId].pos.y) || float.IsNaN(veinPool[VeinId].pos.z))
                        {
                            continue;
                        }
                    }
                    colliderId = veinPool[VeinId].colliderId;
                    pd.physics.RemoveColliderData(colliderId);
                    veinPool[VeinId].colliderId = pd.physics.AddColliderData(LDB.veins.Select((int)veinPool[VeinId].type).prefabDesc.colliders[0].BindToObject(VeinId, 0, EObjectType.Vein, veinPool[VeinId].pos, Quaternion.FromToRotation(Vector3.up, veinPool[VeinId].pos.normalized)));

                    pd.factoryModel.gpuiManager.AlterModel((int)veinPool[VeinId].modelIndex, veinPool[VeinId].modelId, VeinId, veinPool[VeinId].pos, Maths.SphericalRotation(veinPool[VeinId].pos, 90f));

                }
            }

            pd.factory.veinGroups[veinPool[vd.id].groupIndex].pos = veinPool[vd.id].pos / (pd.realRadius + 2.5f);
        }

        public void changeveinpos(VeinData vd, Vector3 pos)
        {
            PlanetData planet = GameMain.localPlanet;
            int VeinId = vd.id;
            if (planet == null || planet.type == EPlanetType.Gas) return;
            VeinData[] veinPool = planet.factory.veinPool;
            veinPool[VeinId].pos = pos;
            int colliderId = veinPool[VeinId].colliderId;
            planet.physics.RemoveColliderData(colliderId);
            veinPool[VeinId].colliderId = planet.physics.AddColliderData(LDB.veins.Select((int)veinPool[VeinId].type).prefabDesc.colliders[0].BindToObject(VeinId, 0, EObjectType.Vein, veinPool[VeinId].pos, Quaternion.FromToRotation(Vector3.up, veinPool[VeinId].pos.normalized)));
            planet.factoryModel.gpuiManager.AlterModel(veinPool[VeinId].modelIndex, veinPool[VeinId].modelId, VeinId, veinPool[VeinId].pos, Maths.SphericalRotation(veinPool[VeinId].pos, 90f));
            bool leave = false;
            int origingroup = -1;
            if (planet.factory.veinGroups[veinPool[VeinId].groupIndex].count > 1)
            {
                Vector3 vector3 = pos - planet.factory.veinGroups[veinPool[VeinId].groupIndex].pos * (planet.realRadius + 2.5f);
                if (vector3.magnitude > 10.0)
                {
                    leave = true;
                    origingroup = veinPool[VeinId].groupIndex;
                    veinPool[VeinId].groupIndex = -1;
                }
            }
            else
            {
                planet.factory.veinGroups[veinPool[VeinId].groupIndex].pos = veinPool[VeinId].pos / (planet.realRadius + 2.5f);
                foreach (VeinData veindata in planet.factory.veinPool)
                {
                    if (veindata.type == veinPool[VeinId].type && veindata.groupIndex != origingroup && (veindata.pos - veinPool[VeinId].pos).magnitude < 10)
                    {
                        origingroup = veinPool[VeinId].groupIndex;
                        veinPool[VeinId].groupIndex = veindata.groupIndex;
                        planet.factory.RecalculateVeinGroup(origingroup);
                    }
                }
            }
            if (leave)
            {
                planet.factory.RecalculateVeinGroup(origingroup);
                foreach (VeinData veindata in planet.factory.veinPool)
                {
                    if (veindata.type == veinPool[VeinId].type && veindata.groupIndex != origingroup && (veindata.pos - veinPool[VeinId].pos).magnitude < 10)
                    {
                        veinPool[VeinId].groupIndex = veindata.groupIndex;
                    }
                }
                if (veinPool[VeinId].groupIndex == -1)
                {
                    veinPool[VeinId].groupIndex = (short)planet.factory.AddVeinGroup(veinPool[VeinId].type, veinPool[VeinId].pos.normalized);
                }
            }
            planet.factory.RecalculateVeinGroup(veinPool[VeinId].groupIndex);
            planet.factory.ArrangeVeinGroups();
        }

        public void getallVein(VeinData vd)
        {
            PlanetData pd = GameMain.localPlanet;
            if (pd == null || pd.type == EPlanetType.Gas || !Physics.Raycast(player.controller.mainCamera.ScreenPointToRay(Input.mousePosition), out RaycastHit raycastHit1, 800f, 8720, (QueryTriggerInteraction)2))
                return;
            Vector3 raycastpos = raycastHit1.point;
            VeinData[] veinPool = pd.factory.veinPool;
            int colliderId;
            Vector3 begin = veinPool[vd.id].pos;
            int index = 0;
            foreach (VeinData vd1 in veinPool)
            {
                if (vd1.pos == null || vd1.id <= 0 || vd1.type != vd.type) continue;
                int VeinId = vd1.id;
                veinPool[VeinId].pos = NotTidyVein.Value ? raycastpos : PostionCompute(begin, raycastpos, vd1.pos, index++, vd.type == EVeinType.Oil);
                if (CoolDown) return;
                if (float.IsNaN(veinPool[VeinId].pos.x) || float.IsNaN(veinPool[VeinId].pos.y) || float.IsNaN(veinPool[VeinId].pos.z))
                {
                    continue;
                }
                if (vd1.groupIndex != vd.groupIndex)
                {
                    int origingroup = veinPool[vd1.id].groupIndex;
                    veinPool[vd1.id].groupIndex = vd.groupIndex;
                    pd.factory.RecalculateVeinGroup(origingroup);
                    pd.factory.RecalculateVeinGroup(vd.groupIndex);
                    pd.factory.ArrangeVeinGroups();
                }
                colliderId = veinPool[VeinId].colliderId;
                pd.physics.RemoveColliderData(colliderId);
                veinPool[VeinId].colliderId = pd.physics.AddColliderData(LDB.veins.Select((int)veinPool[VeinId].type).prefabDesc.colliders[0].BindToObject(VeinId, 0, EObjectType.Vein, veinPool[VeinId].pos, Quaternion.FromToRotation(Vector3.up, veinPool[VeinId].pos.normalized)));

                pd.factoryModel.gpuiManager.AlterModel(veinPool[VeinId].modelIndex, veinPool[VeinId].modelId, VeinId, veinPool[VeinId].pos, Maths.SphericalRotation(veinPool[VeinId].pos, 90f));
            }
            pd.factory.RecalculateVeinGroup(vd.groupIndex);
            pd.factory.ArrangeVeinGroups();
        }

        public void removevein()
        {
            PlanetData pd = GameMain.localPlanet;
            if (pd == null || pd.type == EPlanetType.Gas || !Physics.Raycast(player.controller.mainCamera.ScreenPointToRay(Input.mousePosition), out RaycastHit raycastHit1, 800f, 8720, (QueryTriggerInteraction)2))
                return;
            Vector3 raycastpos = raycastHit1.point;
            foreach (VeinData i in pd.factory.veinPool)
            {
                if ((raycastpos - i.pos).magnitude < 1 && i.type != EVeinType.None)
                {
                    pd.factory.veinGroups[i.groupIndex].count--;
                    pd.factory.veinGroups[i.groupIndex].amount -= i.amount;
                    pd.factory.RemoveVeinWithComponents(i.id);
                    if (pd.factory.veinGroups[i.groupIndex].count == 0)
                    {
                        pd.factory.veinGroups[i.groupIndex].type = 0;
                        pd.factory.veinGroups[i.groupIndex].amount = 0;
                        pd.factory.veinGroups[i.groupIndex].pos = Vector3.zero;
                    }
                    return;
                }
            }
        }

        public VeinData getveinbymouse()
        {
            PlanetData pd = GameMain.localPlanet;
            if (pd != null && pd.type != EPlanetType.Gas && Physics.Raycast(player.controller.mainCamera.ScreenPointToRay(Input.mousePosition), out RaycastHit raycastHit1, 800f, 8720, (QueryTriggerInteraction)2))
            {
                Vector3 raycastpos = raycastHit1.point;
                float min = 100;
                VeinData vein = new VeinData();
                foreach (VeinData vd in pd.factory.veinPool)
                {
                    if (vd.id == 0) continue;
                    if ((raycastpos - vd.pos).magnitude < min && vd.type != EVeinType.None)
                    {
                        min = (raycastpos - vd.pos).magnitude;
                        vein = vd;
                    }
                }
                if (min > 4)
                {
                    return new VeinData();
                }
                else
                {
                    return vein;
                }
            }

            return new VeinData();
        }

        public Vector3 PostionCompute(Vector3 begin, Vector3 end, Vector3 pointpos, int index, bool oil = false)
        {
            if (end.y > 193 || end.y < -193)
            {
                UIMessageBox.Show("移动矿堆失败".Translate(), "当前纬度过高，为避免出错，无法移动矿堆".Translate(), "确定".Translate(), 3);
                CoolDown = true;
                return pointpos;
            }
            Vector3 pos1 = begin;
            Vector3 pos2 = end;
            Vector3 pos3;
            float radius = GameMain.localPlanet.realRadius;
            Quaternion quaternion2 = Maths.SphericalRotation(pos1, 0);
            float areaRadius = oil ? 15 : 1.5f;
            if (!oil)
            {
                pos2.x = (int)pos2.x;
                pos2.z = (int)pos2.z;
                pos2.y = (int)pos2.y;
                pos3 = pos1 + quaternion2 * (new Vector3(index / veinlines.Value, 0, index % veinlines.Value) * areaRadius);
            }
            else
                pos3 = pos1 - quaternion2 * (new Vector3((index / veinlines.Value) * 8, 0, index % veinlines.Value * areaRadius));
            double del1 = Math.Atan(pos1.z / pos1.x) - Math.Atan(pos2.z / pos2.x);
            double del2 = Math.Acos(pos1.y / radius) - Math.Acos(pos2.y / radius);
            double del3_1 = -Math.Atan(pos3.z / pos3.x) + del1;
            double del3_2 = Math.Acos(pos3.y / radius) - del2;
            if (del1 == double.NaN || del2 == double.NaN || del3_1 == double.NaN || del3_2 == double.NaN)
            {
                return pointpos;
            }
            pos3.x = (float)(end.x < 0 ? -Math.Abs(Math.Sin(del3_2) * Math.Cos(del3_1)) : Math.Abs(Math.Sin(del3_2) * Math.Cos(del3_1)));
            pos3.y = (float)(end.y < 0 ? -Math.Abs(Math.Cos(del3_2)) : Math.Abs(Math.Cos(del3_2)));
            pos3.z = (float)(end.z < 0 ? -Math.Abs(Math.Sin(del3_2) * Math.Sin(del3_1)) : Math.Abs(Math.Sin(del3_2) * Math.Sin(del3_1)));
            pos3.x *= radius;
            pos3.y *= radius;
            pos3.z *= radius;

            if (pos3.x == float.NaN || pos3.y == float.NaN || pos3.z == float.NaN || pos3.y > 190 || pos3.y < -190)
            {
                return pointpos;
            }
            return pos3;
        }

        public void BuryAllvein()
        {
            PlanetData pd = GameMain.localPlanet;
            if (pd == null || pd.type == EPlanetType.Gas) return;
            float num9 = pd.realRadius - 50f;
            foreach (VeinData i in pd.factory.veinPool)
            {
                PlanetPhysics physics = pd.physics;
                int id = i.id;
                int colliderId = i.colliderId;
                ColliderData colliderData = physics.GetColliderData(colliderId);
                Vector3 vector3_2 = colliderData.pos.normalized * (num9 + 0.4f);
                physics.colChunks[colliderId >> 20].colliderPool[colliderId & 1048575].pos = vector3_2;
                pd.factory.veinPool[id].pos = i.pos.normalized * num9;
            }
            foreach (VeinData i in pd.factory.veinPool)
            {
                GameMain.gpuiManager.AlterModel(i.modelIndex, i.modelId, i.id, i.pos, false);
            }
            GameMain.gpuiManager.SyncAllGPUBuffer();
        }

        public void RemoveAllvein()
        {
            PlanetData pd = GameMain.localPlanet;
            if (pd == null || pd.type == EPlanetType.Gas) return;
            foreach (VeinData i in pd.factory.veinPool)
            {
                if (i.type != EVeinType.None)
                {
                    pd.factory.veinGroups[i.groupIndex].count--;
                    pd.factory.veinGroups[i.groupIndex].amount -= i.amount;
                    pd.factory.RemoveVeinWithComponents(i.id);
                    if (pd.factory.veinGroups[i.groupIndex].count == 0)
                    {
                        pd.factory.veinGroups[i.groupIndex].type = 0;
                        pd.factory.veinGroups[i.groupIndex].amount = 0;
                        pd.factory.veinGroups[i.groupIndex].pos = Vector3.zero;
                    }
                }
            }
            pd.factory.ArrangeVeinGroups();
        }

        #endregion

        /// <summary>
        /// 瞬间完成戴森球
        /// </summary>
        public void FinishDysonShell()
        {
            foreach (DysonSphere ds in GameMain.data.dysonSpheres)
            {
                if (ds != null && ds.starData == GameMain.localStar)
                {
                    int sum1 = 0;
                    int sum2 = 0;
                    foreach (DysonSphereLayer w in ds.layersIdBased)
                    {
                        if (w == null) continue;
                        foreach (DysonNode t in w.nodePool)
                        {
                            if (t != null)
                            {
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
                    }
                    AddComsumeItemtoTotal(1501, sum2);
                    AddComsumeItemtoTotal(1503, sum1);
                    ds.modelRenderer.RebuildModels();
                }
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
                    PlanetModelingManager.Algorithm(planet).GenerateVeins();
                    //PlanetModelingManager.Algorithm(planet).GenerateVegetables();
                    PlanetModelingManager.Algorithm(planet).GenerateTerrain(planet.mod_x, planet.mod_y);
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
                        GameMain.localPlanet.factory.RemoveEntityWithComponents(etd.id);
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
        public void OnSetBase(int type)
        {
            if (restorewater && type != 0) return;
            int ReformType = 2;
            int ReformColor = 16;
            if (GameMain.localPlanet == null || GameMain.localPlanet.type == EPlanetType.Gas)
                return;
            for (int id = 0; id < GameMain.localPlanet.factory.vegePool.Length; ++id)
                GameMain.localPlanet.factory.RemoveVegeWithComponents(id);
            byte[] modData = GameMain.localPlanet.modData;
            for (int index = 0; index < modData.Length * 2; ++index)
                GameMain.localPlanet.AddHeightMapModLevel(index, 3);
            if (GameMain.localPlanet.UpdateDirtyMeshes())
                GameMain.localPlanet.factory.RenderLocalPlanetHeightmap();
            if (GameMain.localPlanet.factory.platformSystem.reformData == null)
                GameMain.localPlanet.factory.platformSystem.InitReformData();
            if (type == 0)
                ReformType = 0;
            if (type == 1)
                ReformColor = 0;

            byte[] reformData = GameMain.localPlanet.factory.platformSystem.reformData;
            for (int index = 0; index < reformData.Length; ++index)
            {
                GameMain.localPlanet.factory.platformSystem.SetReformType(index, ReformType);
                GameMain.localPlanet.factory.platformSystem.SetReformColor(index, ReformColor);
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
            }
            player.package.NotifyStorageChange();
        }

        /// <summary>
        /// 锁定背包
        /// </summary>
        public void LockPlayerPackage()
        {
            if (player == null || player.package == null) return;
            StorageComponent.GRID[] grids = player.package.grids;
            if (lockpackage_bool.Value && (packageitemlist == null || packageitemlist.Count == 0 || packageitemlist.Count < grids.Length))
            {
                packageitemlist = new List<int[]>();
                for (int i = 0; i < grids.Length; i++)
                {
                    int[] temparray = new int[3] { grids[i].count, grids[i].itemId, grids[i].inc };
                    packageitemlist.Add(temparray);
                }
            }
            else if (!lockpackage_bool.Value && packageitemlist != null && packageitemlist.Count > 0)
            {
                packageitemlist.Clear();
                return;
            }
            else if (!lockpackage_bool.Value && (packageitemlist == null || packageitemlist.Count == 0))
            {
                return;
            }
            for (int i = 0; i < grids.Length; i++)
            {
                grids[i].count = packageitemlist[i][0];
                grids[i].itemId = packageitemlist[i][1];
                grids[i].inc = packageitemlist[i][2];
            }
        }

        /// <summary>
        /// 控制背包大小和物流背包大小
        /// </summary>
        /// <param name="rownum"></param>
        /// <param name="colnum"></param>
        /// <param name="add"></param>
        public void Operatepackagesize(int rownum, int colnum, bool add = true)
        {
            if (player == null) return;
            if (add)
            {
                if (rownum > 0)
                {
                    player.package.SetSize(player.package.size / 10 * 10 + 10 * rownum);
                }
                if (colnum > 0)
                {
                    player.deliveryPackage.unlocked = true;
                    player.deliveryPackage.colCount += colnum;
                    player.deliveryPackage.NotifySizeChange();
                }
            }
            else
            {
                if (rownum > 0)
                {
                    player.package.SetSize(player.package.size / 10 * 10 - 10 * rownum);
                }
                if (colnum > 0 && player.deliveryPackage.colCount > colnum)
                {
                    player.deliveryPackage.unlocked = true;
                    player.deliveryPackage.colCount -= colnum;
                    player.deliveryPackage.NotifySizeChange();
                }
            }
        }

        /// <summary>
        /// 解锁全部科技
        /// </summary>
        public void UnlockallTech()
        {
            if (player == null) return;
            bool end = true;
            while (end)
            {
                end = false;
                foreach (TechProto techProto in new List<TechProto>(LDB.techs.dataArray))
                {
                    if (GameMain.data.history.TechUnlocked(techProto.ID)) continue;
                    if (!GameMain.data.history.CanEnqueueTech(techProto.ID)) continue;
                    if (techProto.MaxLevel > 15) continue;
                    end = true;
                    if (techProto.Level < techProto.MaxLevel)
                    {
                        for (int level = techProto.Level; level < techProto.MaxLevel; ++level)
                        {
                            for (int i = 0; i < techProto.itemArray.Length; i++)
                            {
                                AddComsumeItemtoTotal(techProto.Items[i], (int)(techProto.ItemPoints[i] * techProto.GetHashNeeded(techProto.Level) / 3600));
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
                        for (int i = 0; i < techProto.itemArray.Length; i++)
                        {
                            AddComsumeItemtoTotal(techProto.Items[i], (int)(techProto.ItemPoints[i] * techProto.GetHashNeeded(techProto.Level) / 3600));
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

        #region 配方修改
        public void Setmultiplesmelt(int multiple)
        {
            for (int i = 0; i < LDB.recipes.dataArray.Length; i++)
            {
                if (LDB.recipes.dataArray[i].Type == ERecipeType.Smelt)
                {
                    for (int j = 0; j < LDB.recipes.dataArray[i].ItemCounts.Length; ++j)
                        LDB.recipes.dataArray[i].ItemCounts[j] /= MULTIPELSMELT.Value;
                    for (int j = 0; j < LDB.recipes.dataArray[i].ResultCounts.Length; ++j)
                        LDB.recipes.dataArray[i].ResultCounts[j] /= MULTIPELSMELT.Value;
                }
            }
            bool needMulti = LDB.recipes.dataArray[0].ItemCounts[0] == 1;
            for (int i = 0; i < LDB.recipes.dataArray.Length; i++)
            {
                if (MULTIPELSMELT.Value != 1 && needMulti && LDB.recipes.dataArray[i].Type == ERecipeType.Smelt)
                {
                    for (int j = 0; j < LDB.recipes.dataArray[i].ItemCounts.Length; ++j)
                        LDB.recipes.dataArray[i].ItemCounts[j] *= MULTIPELSMELT.Value;
                    for (int j = 0; j < LDB.recipes.dataArray[i].ResultCounts.Length; ++j)
                        LDB.recipes.dataArray[i].ResultCounts[j] *= MULTIPELSMELT.Value;
                }
            }
            MULTIPELSMELT.Value = multiple;
        }

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
                if (allhandcraft.Value)
                {
                    LDB.recipes.dataArray[i].Handcraft = true;
                }
                else
                {
                    LDB.recipes.dataArray[i].Handcraft = originRecipeProtos[i].Handcraft;
                }

                if (quickproduce.Value)
                {
                    LDB.recipes.dataArray[i].TimeSpend = 1;
                }
                else
                {
                    LDB.recipes.dataArray[i].TimeSpend = originRecipeProtos[i].TimeSpend;
                }
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

        /// <summary>
        /// 背包无限物品
        /// </summary>
        public void InfiniteAllThingInPackage()
        {
            if (GameMain.mainPlayer == null) return;
            StorageComponent.GRID[] grids = GameMain.mainPlayer.package.grids;
            if (grids.Length < itemProtos.Length)
            {
                GameMain.mainPlayer.package.SetSize((itemProtos.Length / 10 + 1) * 10);
            }
            int i = 0;
            foreach (ItemProto ip in itemProtos)
            {
                grids[i].itemId = ip.ID;
                grids[i].count = StorageComponent.itemStackCount[ip.ID];
                grids[i].stackSize = StorageComponent.itemStackCount[ip.ID];
                i++;
                if (i == grids.Length) break;
            }
            GameMain.mainPlayer.package.NotifyStorageChange();
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

        public void Sunlightset()
        {
            if (GameMain.universeSimulator == null || GameMain.universeSimulator.LocalStarSimulator() == null || GameMain.universeSimulator.LocalStarSimulator().sunLight == null)
            {
                SunLight = null;
                lighton = false;
                return;
            }
            if (sunlight_bool.Value)
            {
                if (SunLight == null)
                    SunLight = GameMain.universeSimulator.LocalStarSimulator().sunLight;
                if (SunLight != null)
                    SunLight.transform.rotation = Quaternion.LookRotation(-player.transform.up);
                lighton = true;
            }
            else if (!sunlight_bool.Value && SunLight != null && lighton)
            {
                SunLight = null;
                lighton = false;
            }
        }

        #region 星际物流站设置及星球矿机
        public void StationComponentSet()
        {
            if (GameMain.data.galacticTransport == null || GameMain.data.galacticTransport.stationPool == null) return;
            foreach (StarData sd in GameMain.galaxy.stars)
            {
                foreach (PlanetData pd in sd.planets)
                {
                    if (pd.factory == null || pd.factory.transport == null) continue;
                    int pdId = pd.id;
                    foreach (StationComponent sc in pd.factory.transport.stationPool)
                    {
                        if (sc == null || sc.storage == null) continue;
                        if (StationfullCount)
                        {
                            StationFullItemCount(sc);
                        }
                        if (sc.isCollector || sc.isVeinCollector) continue;
                        if (StationSpray.Value)
                        {
                            StationSprayInc(sc);
                        }
                        if (StationPowerGen.Value || sc?.name == "星球矿机")
                        {
                            StationPowerGeneration(sc, pdId);
                        }
                        if (Station_infiniteWarp_bool.Value && sc.isStellar)
                            sc.warperCount = 50;
                        switch (sc.name)
                        {
                            case "星球无限供货机":
                                if (!StationfullCount_bool.Value)
                                    continue;
                                StationFullItemCount(sc);
                                break;
                            case "垃圾站":
                            case "Station_trash":
                                if (!StationTrash.Value)
                                    continue;
                                StationTrashMethod(sc, pdId);
                                break;
                        }
                    }
                }
            }
        }

        private void StationMine()
        {
            foreach (StarData sd in GameMain.galaxy.stars)
            {
                foreach (PlanetData pd in sd.planets)
                {
                    if (pd?.factory?.transport == null) continue;
                    int pdId = pd.id;
                    foreach (StationComponent sc in pd.factory.transport.stationPool)
                    {
                        if (sc?.storage == null) continue;
                        if (sc.isCollector || sc.isVeinCollector) continue;
                        if (sc.name != "星球矿机" && sc.name != "Station_miner") continue;
                        for (int i = 0; i < sc.storage.Length && sc.energy > 0; i++)
                        {
                            int itemID = sc.storage[i].itemId;
                            if (itemID <= 0 || sc.storage[i].count >= sc.storage[i].max)
                                continue;

                            int veinNumbers = GetNumberOfVein(itemID, pd);
                            int pointminenum = veinNumbers * Stationminenumber.Value;
                            if (veinNumbers > 0 && pointminenum == 0) pointminenum = 1;
                            else if (veinNumbers == 0 && itemID != pd.waterItemId) continue;

                            if (Stationfullenergy.Value || sc.energy > pointminenum * GameMain.history.miningSpeedScale * 5000)
                            {
                                int minenum = MineVein(itemID, pointminenum, pd.id);
                                if (minenum > 0)
                                {
                                    AddStatInfo(pd.id, itemID, minenum);
                                    sc.AddItem(itemID, minenum, 0);
                                    if (!Stationfullenergy.Value)
                                        sc.energy -= minenum * 5000;
                                }
                            }
                        }
                    }
                }
            }

        }

        /// <summary>
        /// 物流站采矿
        /// </summary>
        /// <param name="itemid"></param>
        /// <param name="minenumber"></param>
        /// <param name="pdid"></param>
        /// <returns></returns>
        public int MineVein(int itemid, int minenumber, int pdid)
        {
            int getmine = 0;
            PlanetData pd = GameMain.galaxy.PlanetById(pdid);
            if (pd.waterItemId == itemid)
            {
                return (int)(30 * GameMain.history.miningSpeedScale * Stationminenumber.Value);
            }
            if (GameMain.data.gameDesc.isInfiniteResource)
                return itemid != 1007 ? (int)(minenumber * GameMain.history.miningSpeedScale / 2) : (int)(minenumber * GameMain.history.miningSpeedScale);
            if (LDB.veins.GetVeinTypeByItemId(itemid) == EVeinType.None || pd == null)
            {
                return 0;
            }
            int neednumber = itemid != 1007 ? (int)(minenumber * GameMain.history.miningCostRate / 2) : (int)(minenumber * GameMain.history.miningCostRate);
            if (minenumber > 0 && neednumber == 0) neednumber = 1;
            foreach (VeinData i in pd.factory.veinPool)
            {
                if (i.type != LDB.veins.GetVeinTypeByItemId(itemid))
                    continue;
                if (i.amount > neednumber - getmine)
                {
                    if (itemid == 1007 && i.amount * VeinData.oilSpeedMultiplier <= 0.1)
                    {
                        int dis = oillowerlimit - pd.factory.veinPool[i.id].amount;
                        pd.factory.veinPool[i.id].amount += dis;
                        pd.factory.veinGroups[i.groupIndex].amount += dis;
                        getmine += (int)(0.1 * GameMain.history.miningSpeedScale * Stationminenumber.Value);
                    }
                    else
                    {
                        pd.factory.veinPool[i.id].amount -= neednumber;
                        pd.factory.veinGroups[i.groupIndex].amount -= neednumber;
                        getmine = minenumber;
                    }
                }
                else
                {
                    if (itemid != 1007)
                    {
                        pd.factory.veinGroups[i.groupIndex].count--;
                        pd.factory.veinGroups[i.groupIndex].amount -= i.amount;
                        pd.factory.RemoveVeinWithComponents(i.id);
                        getmine += i.amount;
                    }
                }
                if (pd.factory.veinGroups[i.groupIndex].count == 0)
                {
                    pd.factory.veinGroups[i.groupIndex].type = 0;
                    pd.factory.veinGroups[i.groupIndex].amount = 0;
                    pd.factory.veinGroups[i.groupIndex].pos = Vector3.zero;
                }
                if (getmine == minenumber) break;
            }
            return itemid != 1007 ? (int)(getmine * GameMain.history.miningSpeedScale / 2) : (int)(getmine * GameMain.history.miningSpeedScale);
        }

        /// <summary>
        /// 物流站内置发电
        /// </summary>
        /// <param name="sc"></param>
        /// <param name="planetID"></param>
        public static void StationPowerGeneration(StationComponent sc, int planetID = 0)
        {
            if (sc == null || sc.storage == null || sc.energy >= sc.energyMax - 1000000) return;
            planetID = sc.planetId > 0 ? sc.planetId : planetID;
            if (planetID == 0) return;
            ref StationStore store = ref sc.storage[sc.storage.Length - 1];
            if (store.itemId <= 0 || store.count <= 0)
                return;
            ItemProto ip = LDB.items.Select(store.itemId);
            if (ip.HeatValue > 0 && sc.energy + ip.HeatValue < sc.energyMax)
            {
                int num = Math.Min(store.count, (int)((sc.energyMax - sc.energy) / ip.HeatValue));
                sc.energy += num * ip.HeatValue;
                store.count -= num;
                if (num > 0)
                {
                    AddStatInfo(planetID, ip.ID, num);
                }
            }
        }

        /// <summary>
        /// 物流站内置喷涂
        /// </summary>
        /// <param name="sc"></param>
        public static void StationSprayInc(StationComponent sc)
        {
            var incstore = sc.storage.FirstOrDefault(x => x.itemId == 1143 && x.count > 0 && x.localLogic != ELogisticStorage.None);
            if (incstore.itemId <= 0)
                return;
            for (int i = 0; i < sc.storage.Length && incstore.count <= 0; i++)
            {
                ref StationStore store = ref sc.storage[i];
                if (store.itemId == 1143 || store.itemId <= 0 || store.count <= 0) continue;

                int needinc = store.count * incAbility - store.inc;
                int needNumber = Math.Min((int)Math.Ceiling(needinc / 296.0), incstore.count);
                store.inc += incstore.count >= needNumber ? needinc : incstore.count * 296;
                incstore.count -= needNumber;
                incstore.inc = Math.Min(incstore.count * incAbility, incstore.inc);
            }
        }

        /// <summary>
        /// 物流站满货物
        /// </summary>
        /// <param name="sc"></param>
        private static void StationFullItemCount(StationComponent sc)
        {
            for (int i = 0; i < sc.storage.Length; i++)
            {
                if (sc.storage[i].itemId <= 0)
                    continue;
                sc.storage[i].count = sc.storage[i].max;
            }
        }

        /// <summary>
        /// 物流站垃圾站
        /// </summary>
        /// <param name="sc"></param>
        /// <param name="pdId"></param>
        private static void StationTrashMethod(StationComponent sc, int pdId)
        {
            for (int i = 0; i < sc.storage.Length && sc.energy > 0; i++)
            {
                ref StationStore store = ref sc.storage[i];
                int itemID = store.itemId;
                if (itemID <= 0) continue;
                int trashnum = store.count;
                if (sc.energy > trashnum * 10000)
                {
                    AddStatInfo(pdId, itemID, trashnum, false);
                    sc.storage[i].count -= trashnum;
                    if (!Stationfullenergy.Value)
                        sc.energy -= trashnum * 10000;
                    if (!noneedtrashsand.Value)
                        player.SetSandCount(player.sandCount + trashnum * 100);
                }
            }
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