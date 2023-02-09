using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using static Multfunction_mod.Constant;
using static Multfunction_mod.Multifunctionpatch;
using Debug = UnityEngine.Debug;

namespace Multfunction_mod
{
    [BepInPlugin(GUID, NAME, VERSION)]
    public class Multifunction : BaseUnityPlugin
    {
        public const string GUID = "cn.blacksnipe.dsp.Multfuntion_mod";
        public const string NAME = "Multfuntion_mod";
        public const string VERSION = "2.7.2";
        #region 临时变量
        private Vector2 scrollPosition;
        public Light SunLight;
        public Texture2D mytexture;
        public GameObject MultiFunctionPanel;
        public ItemProto[] itemProtos;
        public RecipeProto[] RecipeList;
        public GUIStyle style = new GUIStyle();
        public static Player player;
        public static GameObject ui_MultiFunctionPanel;
        public List<string> BPPathList = new List<string>();
        public List<string> BPFileNameList = new List<string>();
        public List<int[]> packageitemlist = new List<int[]>();
        public static List<int> SuperStation = new List<int>();
        public static List<int> StarSuperStation = new List<int>();
        public static List<Tempsail> tempsails = new List<Tempsail>();
        public static Dictionary<int, int> tmp_levelChanges;
        public VeinData pointveindata;
        public KeyboardShortcut tempShowWindow;
        public int maxheight => Screen.height;
        public int maxwidth => Screen.width;
        public int heightdis;
        public int togglesize;
        public int oillowerlimit;
        public int whichpannel = 1;
        public int veintype = 1;
        public static int DriftBuildingLevel;
        public static int productmaxindex;
        public static int incAbility = 4;
        public float MainWindow_x;
        public float MainWindow_y;
        public float MainWindow_x_move;
        public float MainWindow_y_move;

        public float tempx1;
        public float tempy1;
        public float InfiniteAllThingTime;
        public float MechaLogisticsTime;
        public float startsuperstationtime;
        public float StationMinerTime;
        public static float buildheight = 1;
        public static float[] warpstationqua;
        public static float[] warpsuperstationqua;
        public float[] ChangeValueArray = new float[100];
        public bool firstopen = true;
        public bool firstrestorewater = true;
        public bool firstchangetextcolor = true;
        public bool loadbutton;
        public bool refreshPlayerData;
        public bool RefreshStationStorage;
        public bool Window_moving;
        public bool changescale;
        public bool changequapowerpertick;
        public bool ChangePlayerbool;
        public bool StationfullCount;
        public bool ChangeQuickKey;
        public bool ChangingQuickKey;
        public bool leftscaling;
        public bool rightscaling;
        public bool topscaling;
        public bool bottomscaling;
        public bool dropdownbutton;
        public bool addveinbool;
        public bool buildnotimecolddown;
        public bool SandBoxMode;
        public bool blueprintPasteToolActive;
        public bool changexveinspos;
        public bool changeveinposbool;
        public bool changeveingroupposbool;
        public bool getallVein_bool;
        public static int EjectorNumber;
        public static int SiloNumber;
        public static bool first = true;
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
        public static bool Itemdelete_bool;
        public static bool farconnectdistance;
        public static bool pasteanyway;
        public static bool PasteBuildAnyWay;
        public static bool closeallcollider;
        public static bool unlockpointtech;
        public static bool autochangeQuantumstationname;
        public static bool autochangeQuantumStarstationname;
        public static bool stopUniverse;
        public static bool TempEjectorRandomEmission;
        public static bool TempSiloRandomEmission;
        public string stackmultiple1 = "";
        public string multipelsmelt1 = "";
        public string watertype = "";
        //public static Queue<int[]> addStatQueue = new Queue<int[]>();
        public static Dictionary<int, Dictionary<int, long>> addStatDic = new Dictionary<int, Dictionary<int, long>>();
        public static Dictionary<int, Dictionary<int, long>> consumeStatDic = new Dictionary<int, Dictionary<int, long>>();
        public static Dictionary<int, int> watertypePlanetArray = new Dictionary<int, int>();
        public static Dictionary<int, int> OrwatertypePlanetArray = new Dictionary<int, int>();
        public static Dictionary<int, int[]> StarSuperStationItemidstore = new Dictionary<int, int[]>();
        public static Dictionary<int, Dictionary<int, List<int[]>>> PlanetSuperStationItemidstore = new Dictionary<int, Dictionary<int, List<int[]>>>();
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
        public static ConfigEntry<float> mytexturer;
        public static ConfigEntry<float> mytextureg;
        public static ConfigEntry<float> mytextureb;
        public static ConfigEntry<float> textcolorr;
        public static ConfigEntry<float> textcolorg;
        public static ConfigEntry<float> textcolorb;
        public static ConfigEntry<float> mytexturea;
        public static ConfigEntry<float> MainWindow_width;
        public static ConfigEntry<float> MainWindow_height;
        public static ConfigEntry<float> MainWindow_x_config;
        public static ConfigEntry<float> MainWindow_y_config;
        public static ConfigEntry<float> starquamaxpowerpertick;
        public static ConfigEntry<float> planetquamaxpowerpertick;
        public static ConfigEntry<string> watertypePlanet;
        public static ConfigEntry<string> Mechalogneed;
        public static ConfigEntry<bool> InfiniteSand;
        public static ConfigEntry<Boolean> NotTidyVein;
        public static ConfigEntry<Boolean> InspectDisNoLimit;
        public static ConfigEntry<Boolean> RandomEmission;
        public static ConfigEntry<Boolean> StationMaxproliferator;
        public static ConfigEntry<Boolean> Mechalogistics_bool;
        public static ConfigEntry<Boolean> MechalogStoragerecycle_bool;
        public static ConfigEntry<Boolean> MechalogStorageprovide_bool;
        public static ConfigEntry<Boolean> MechalogStationrecycle_bool;
        public static ConfigEntry<Boolean> MechalogStationprovide_bool;
        public static ConfigEntry<Boolean> Infinitestoragetank;
        public static ConfigEntry<Boolean> QuickHandcraft;
        public static ConfigEntry<Boolean> QuickPlayerMine;
        public static ConfigEntry<Boolean> TankMaxproliferator;
        public static ConfigEntry<Boolean> Quantumtransport_bool;
        public static ConfigEntry<Boolean> Quantumtransportbuild_bool;
        public static ConfigEntry<Boolean> Quantumtransportpdwarp_bool;
        public static ConfigEntry<Boolean> Quantumtransportstarwarp_bool;
        public static ConfigEntry<Boolean> noneedtrashsand;
        public static ConfigEntry<Boolean> InfineteStarPower;
        public static ConfigEntry<Boolean> PlanetPower_bool;
        public static ConfigEntry<Boolean> allhandcraft;
        public static ConfigEntry<Boolean> quickproduce;
        public static ConfigEntry<Boolean> lockpackage_bool;
        public static ConfigEntry<Boolean> blueprintpastenoneed_bool;
        public static ConfigEntry<Boolean> noneedwarp;
        public static ConfigEntry<Boolean> Infinitething;
        public static ConfigEntry<Boolean> Infiniteplayerpower;
        public static ConfigEntry<Boolean> deleteveinbool;
        public static ConfigEntry<Boolean> MechalogisticsPlanet_bool;
        public static ConfigEntry<Boolean> StationMiner;
        public static ConfigEntry<Boolean> autochangestationname;
        public static ConfigEntry<Boolean> dismantle_but_nobuild;
        public static ConfigEntry<Boolean> build_gascol_noequator;
        public static ConfigEntry<Boolean> StationTrash;
        public static ConfigEntry<Boolean> build_tooclose_bool;
        public static ConfigEntry<Boolean> sunlight_bool;
        public static ConfigEntry<Boolean> DroneNoenergy_bool;
        public static ConfigEntry<Boolean> BuildNotime_bool;
        public static ConfigEntry<Boolean> Station_infiniteWarp_bool;
        public static ConfigEntry<Boolean> Station_miner_noconsume_bool;
        public static ConfigEntry<Boolean> StationfullCount_bool;
        public static ConfigEntry<Boolean> ItemList_bool;
        public static ConfigEntry<Boolean> Buildingnoconsume;
        public static ConfigEntry<Boolean> Stationfullenergy;
        public static ConfigEntry<Boolean> BeltSignalFunction;
        public static ConfigEntry<Boolean> Tankcontentall;
        public static ConfigEntry<Boolean> ArchitectMode;
        public static ConfigEntry<Boolean> quickabsorbsolar;
        public static ConfigEntry<Boolean> cancelsolarbullet;
        public static ConfigEntry<Boolean> QuickabortSwarm;
        public static ConfigEntry<Boolean> alwaysemission;
        public static ConfigEntry<Boolean> StationSpray;
        public static ConfigEntry<Boolean> Maxproliferator;
        public static ConfigEntry<Boolean> StationPowerGen;
        public static ConfigEntry<Boolean> ChangeDysonradius;
        public static ConfigEntry<Boolean> CloseUIpanel;
        public static ConfigEntry<Boolean> CloseUIAbnormalityTip;
        public static ConfigEntry<Boolean> QuantumtransportstationSupply;
        public static ConfigEntry<Boolean> QuantumtransportlabSupply;
        public static ConfigEntry<Boolean> QuantumtransportpowerSupply;
        public static ConfigEntry<Boolean> QuantumtransportassembleSupply;
        public static ConfigEntry<Boolean> QuantumtransportstationDemand;
        public static ConfigEntry<Boolean> QuantumtransportminerDemand;
        public static ConfigEntry<Boolean> QuantumtransportsiloDemand;
        public static ConfigEntry<Boolean> QuantumtransportlabDemand;
        public static ConfigEntry<Boolean> QuantumtransportpowerDemand;
        public static ConfigEntry<Boolean> QuantumtransportassembleDemand;
        #endregion

        void Start()
        {
            preparedraw();
            Patchallmethod();
            MultifunctionTranslate.regallTranslate();
            AssetBundle assetBundle = AssetBundle.LoadFromStream(Assembly.GetExecutingAssembly().GetManifestResourceStream("Multifunction_mod.multifunctionpanel"));
            MultiFunctionPanel = assetBundle.LoadAsset<GameObject>("MultiFunctionPanel");
            startsuperstationtime = Time.time;
            StationMinerTime = Time.time;
            InfiniteAllThingTime = Time.time;

            MainWindow_x_move = 710;
            MainWindow_y_move = 390;
            tempx1 = MainWindow_x;
            tempy1 = MainWindow_y;
            scrollPosition[0] = 0;

            //初始化配置
            {
                Quantumtransport_bool = Config.Bind("量子传输站", "Quantumtransport_bool", false);
                Quantumtransportstarwarp_bool = Config.Bind("星系级翘曲全面供应", "Quantumtransportstarwarp_bool", false);
                Quantumtransportpdwarp_bool = Config.Bind("星球级翘曲全面供应", "Quantumtransportpdwarp_bool", false);
                Quantumtransportbuild_bool = Config.Bind("星球级材料供应", "Quantumtransportbuild_bool", true);
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
                Infinitestoragetank = Config.Bind("无限储液站", "Infinitestoragetank", false);
                TankMaxproliferator = Config.Bind("储液站无限增产", "TankMaxproliferator", false);
                Tankcontentall = Config.Bind("储液站任意存", "Tankcontentall", false);
                allhandcraft = Config.Bind("全部手搓", "allhandcraft", false);
                quickproduce = Config.Bind("快速生产", "quickproduce", false);
                noneedwarp = Config.Bind("无翘曲器曲速", "noneedwarp", false);
                watertypePlanet = Config.Bind("星球海洋类型", "watertypePlanet", "");

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
                Station_miner_noconsume_bool = Config.Bind("星球矿机无消耗", "Station_miner_noconsume_bool", false);
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
                CloseUIAbnormalityTip= Config.Bind("关闭异常提示", "CloseUIAbnormalityTip", false);
                mytexturer = Config.Bind("窗口材质r", "mytexturer", 0f);
                mytextureg = Config.Bind("窗口材质g", "mytextureg", 0f);
                mytextureb = Config.Bind("窗口材质b", "mytextureb", 0f);
                textcolorr = Config.Bind("字体颜色r", "textcolorr", 1f);
                textcolorg = Config.Bind("字体颜色g", "textcolorg", 1f);
                textcolorb = Config.Bind("字体颜色b", "textcolorb", 1f);
                mytexturea = Config.Bind("窗口材质a", "mytexturea", 1f);

                MainWindow_x_config = Config.Bind("第一窗口x", "xl_SimpleUI_1_x_config", 448.0f);
                MainWindow_y_config = Config.Bind("第一窗口y", "xl_SimpleUI_1_y_config", 199.0f);
                MainWindow_width = Config.Bind("第一窗口宽度", "xl_SimpleUI_1_x_config", 1400.0f);
                MainWindow_height = Config.Bind("第一窗口高度", "xl_SimpleUI_1_y_config", 1000.0f);
                CloseUIpanel = Config.Bind("关闭面板", "CloseUIpanel", false);
                WindowQuickKey = Config.Bind("打开窗口快捷键", "Key", new BepInEx.Configuration.KeyboardShortcut(KeyCode.Alpha1, KeyCode.LeftAlt));
            }


            incAbility = Maxproliferator.Value ? 10 : 4;
            MainWindow_x = MainWindow_x_config.Value;
            MainWindow_y = MainWindow_y_config.Value;
            oillowerlimit = (int)(0.1 / VeinData.oilSpeedMultiplier);
            mytexture = new Texture2D(10, 10);
            for (int i = 0; i < mytexture.width; i++)
            {
                for (int j = 0; j < mytexture.height; j++)
                {
                    mytexture.SetPixel(i, j, new Color(mytexturer.Value, mytextureg.Value, mytextureb.Value, mytexturea.Value));
                }
            }
            mytexture.Apply();
            stackmultiple1 = StackMultiple.Value.ToString();
            multipelsmelt1 = MULTIPELSMELT.Value.ToString();
            tempShowWindow = WindowQuickKey.Value;

            for (int i = 0; i < 100; i++)
            {
                ChangeValueArray[i] = 0.0f;
            }
            //初始化海洋
            foreach (string k in watertypePlanet.Value.Split(';'))
            {
                if (k.Length == 0) break;
                string[] t = k.Split(',');
                watertypePlanetArray.Add(int.Parse(t[0]), int.Parse(t[1]));
            }
            ThreadPool.QueueUserWorkItem(o =>
            {
                while (true)
                {
                    while (Quantumtransport_bool.Value)
                    {
                        try
                        {
                            takeitemfromstarsuperstation();
                        }
                        catch (Exception ex)
                        {
                            Debug.Log(ex.ToString());
                        }
                        Thread.Sleep(1000);
                    }
                    Thread.Sleep(1000);
                }
            });
        }

        void Update()
        {
            ChangeQuickKeyMethod();
            RecipeCustomization();
            RestoreWaterMethod();
            FirstStartGame();
            QuickKeyOpenWindow();
            AfterGameStart();
            if (Input.GetKey(KeyCode.F10) && Input.GetKeyDown(KeyCode.LeftShift))
            {

            }
            if (ItemDisplayingWindow)
            {
                if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.LeftShift))
                {
                    Itemdelete_bool = true;
                    player.SetHandItems(0, 0);
                }
                else
                {
                    Itemdelete_bool = false;
                }
            }
            else
            {
                Itemdelete_bool = false;
            }
        }

        public void OnGUI()
        {
            heightdis = GUI.skin.button.fontSize * 2;
            if (changescale || firstopen)
            {
                changescale = false;
                firstopen = false;
                GUI.skin.label.fontSize = scale.Value;
                GUI.skin.button.fontSize = scale.Value;
                GUI.skin.toggle.fontSize = scale.Value;
                GUI.skin.textField.fontSize = scale.Value;
                GUI.skin.textArea.fontSize = scale.Value;
            }
            else if (!changescale && GUI.skin.toggle.fontSize != scale.Value)
            {
                scale.Value = GUI.skin.toggle.fontSize;
            }
            if (firstchangetextcolor)
            {
                GUI.skin.button.normal.textColor = new Color(textcolorr.Value, textcolorg.Value, textcolorb.Value);
                GUI.skin.textArea.normal.textColor = new Color(textcolorr.Value, textcolorg.Value, textcolorb.Value);
                GUI.skin.textField.normal.textColor = new Color(textcolorr.Value, textcolorg.Value, textcolorb.Value);
                GUI.skin.toggle.normal.textColor = new Color(textcolorr.Value, textcolorg.Value, textcolorb.Value);
                GUI.skin.toggle.onNormal.textColor = new Color(textcolorr.Value, textcolorg.Value, textcolorb.Value);
                firstchangetextcolor = false;
            }
            if (DisplayingWindow)
            {
                var rt = ui_MultiFunctionPanel.GetComponent<RectTransform>();
                rt.sizeDelta = new Vector2(MainWindow_width.Value, MainWindow_height.Value);
                rt.localPosition = new Vector2(-Screen.width / 2 + MainWindow_x, Screen.height / 2 - MainWindow_y - MainWindow_height.Value);
                Rect windowRect1 = new Rect(MainWindow_x, MainWindow_y, MainWindow_width.Value, MainWindow_height.Value);
                if (leftscaling || rightscaling || topscaling || bottomscaling) { }
                else
                {
                    Move_Window(ref MainWindow_x, ref MainWindow_y, ref MainWindow_x_move, ref MainWindow_y_move, ref Window_moving, ref tempx1, ref tempy1, MainWindow_width.Value);
                }
                GUI.DrawTexture(new Rect(MainWindow_x, MainWindow_y, MainWindow_width.Value, MainWindow_height.Value), mytexture);
                Scale_Window(MainWindow_width.Value, MainWindow_height.Value, ref MainWindow_x, ref MainWindow_y);
                windowRect1 = GUI.Window(20210218, windowRect1, MainWindow, "OP面板".getTranslate() + "(" + VERSION + ")" + "ps:ctrl+↑↓");
                MainWindow_x_config.Value = MainWindow_x;
                MainWindow_y_config.Value = MainWindow_y;
                if (MainWindow_x < 0 || MainWindow_x > maxwidth)
                {
                    MainWindow_x_config.Value = 100;
                }
                if (MainWindow_y < 0 || MainWindow_y > maxheight)
                {
                    MainWindow_y_config.Value = 77;
                }
            }
            if (ItemDisplayingWindow)
            {
                Rect windowRect2 = new Rect(maxwidth * 3 / 4, 100, maxwidth / 4, maxheight);
                windowRect2 = GUI.Window(20210219, windowRect2, TabItemWindow, "");
            }
        }

        #region 窗口操作

        /// <summary>
        /// 移动窗口
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="x_move"></param>
        /// <param name="y_move"></param>
        /// <param name="movewindow"></param>
        /// <param name="tempx"></param>
        /// <param name="tempy"></param>
        /// <param name="x_width"></param>
        public void Move_Window(ref float x, ref float y, ref float x_move, ref float y_move, ref bool movewindow, ref float tempx, ref float tempy, float x_width)
        {
            Vector2 temp = Input.mousePosition;
            if (temp.x > x && temp.x < x + x_width && maxheight - temp.y > y && maxheight - temp.y < y + 20)
            {
                if (Input.GetMouseButton(0))
                {
                    if (!movewindow)
                    {
                        x_move = x;
                        y_move = y;
                        tempx = temp.x;
                        tempy = maxheight - temp.y;
                    }
                    movewindow = true;
                    x = x_move + temp.x - tempx;
                    y = y_move + (maxheight - temp.y) - tempy;
                }
                else
                {
                    movewindow = false;
                    tempx = x;
                    tempy = y;
                }
            }
            else if (movewindow)
            {
                movewindow = false;
                x = x_move + temp.x - tempx;
                y = y_move + (maxheight - temp.y) - tempy;
            }
        }

        /// <summary>
        /// 缩放窗口
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="x_move"></param>
        /// <param name="y_move"></param>
        public void Scale_Window(float x, float y, ref float x_move, ref float y_move)
        {
            Vector2 temp = Input.mousePosition;
            if (Input.GetMouseButton(0))
            {
                if ((temp.x + 10 > x_move && temp.x - 10 < x_move) && (maxheight - temp.y >= y_move && maxheight - temp.y <= y_move + y) || leftscaling)
                {
                    x -= temp.x - x_move;
                    x_move = temp.x;
                    leftscaling = true;
                    rightscaling = false;
                }
                if ((temp.x + 10 > x_move + x && temp.x - 10 < x_move + x) && (maxheight - temp.y >= y_move && maxheight - temp.y <= y_move + y) || rightscaling)
                {
                    x += temp.x - x_move - x;
                    rightscaling = true;
                    leftscaling = false;
                }
                if ((maxheight - temp.y + 10 > y + y_move && maxheight - temp.y - 10 < y + y_move) && (temp.x >= x_move && temp.x <= x_move + x) || bottomscaling)
                {
                    y += maxheight - temp.y - (y_move + y);
                    bottomscaling = true;
                }
                if (rightscaling || leftscaling)
                {
                    if ((maxheight - temp.y + 10 > y_move && maxheight - temp.y - 10 < y_move) && (temp.x >= x_move && temp.x <= x_move + x) || topscaling)
                    {
                        y -= maxheight - temp.y - y_move;
                        y_move = maxheight - temp.y;
                        topscaling = true;
                    }
                }
            }
            if (Input.GetMouseButtonUp(0))
            {
                rightscaling = false;
                leftscaling = false;
                bottomscaling = false;
                topscaling = false;
            }
            MainWindow_width.Value = x;
            MainWindow_height.Value = y;
        }

        /// <summary>
        /// Rect变形
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="changevalue"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public Rect RectChanged(Rect rect, int changevalue, int index)
        {
            if (index <= 0 || index > 4) return rect;
            switch (index)
            {
                case 1: rect.x += changevalue; break;
                case 2: rect.y += changevalue; break;
                case 3: rect.width += changevalue; break;
                case 4: rect.height += changevalue; break;
            }
            return rect;
        }
        #endregion

        #region 窗口UI

        /// <summary>
        /// Tab物品列表
        /// </summary>
        /// <param name="windId"></param>
        public void TabItemWindow(int windId)
        {
            if (LDB.items == null) return;
            GUILayout.BeginArea(new Rect(0, 0, maxwidth / 4, maxheight));
            int size = maxwidth / 40;
            int height = 0;
            for (int i = 0; i < itemProtos.Length; i++)
            {
                if (i != 0 && i % 10 == 0) height++;
                if (GUI.Button(new Rect(i % 10 * size, height * size, size, size), itemProtos[i].iconSprite.texture))
                {
                    Itemdelete_bool = false;
                    if (Input.GetKey(KeyCode.LeftControl))
                    {
                        player.TryAddItemToPackage(itemProtos[i].ID, StorageComponent.itemStackCount[itemProtos[i].ID], 0, true);
                    }
                    else
                    {
                        player.TryAddItemToPackage(itemProtos[i].ID, 1, 0, true);
                    }
                }
            }

            GUILayout.EndArea();
        }

        /// <summary>
        /// 主控面板
        /// </summary>
        /// <param name="winId"></param>
        public void MainWindow(int winId)
        {
            togglesize = GUI.skin.toggle.fontSize + 3;
            style.wordWrap = true;
            style.normal.textColor = new Color(textcolorr.Value, textcolorg.Value, textcolorb.Value);
            style.fontSize = GUI.skin.toggle.fontSize;
            GUILayout.BeginArea(new Rect(10, 20, MainWindow_width.Value, MainWindow_height.Value));

            {
                GUILayout.BeginVertical();

                GUILayout.BeginHorizontal(new[] { GUILayout.Height(heightdis)});
                if (GUILayout.Button("人物".getTranslate(), new[] { GUILayout.Height(heightdis), GUILayout.Width(heightdis * 4) })) { whichpannel = 1; }
                if (GUILayout.Button("建筑".getTranslate(), new[] { GUILayout.Height(heightdis), GUILayout.Width(heightdis * 4) })) { whichpannel = 2; }
                if (GUILayout.Button("星球".getTranslate(), new[] { GUILayout.Height(heightdis), GUILayout.Width(heightdis * 4) })) { whichpannel = 3; }
                if (GUILayout.Button("戴森球".getTranslate(), new[] { GUILayout.Height(heightdis), GUILayout.Width(heightdis * 4) })) { whichpannel = 4; }
                if (GUILayout.Button("其它功能".getTranslate(), new[] { GUILayout.Height(heightdis), GUILayout.Width(heightdis * 4) })) { whichpannel = 5; }
                if (GUILayout.Button("机甲物流".getTranslate(), new[] { GUILayout.Height(heightdis), GUILayout.Width(heightdis * 4) })) { whichpannel = 6; }
                GUILayout.EndHorizontal();
                if (whichpannel == 6) LogisticsPannel();
                else
                {
                    scrollPosition = GUILayout.BeginScrollView(scrollPosition, new[] { GUILayout.Width(MainWindow_width.Value - 20), GUILayout.Height(MainWindow_height.Value - heightdis * 2) });
                    if (whichpannel == 1) PlayerPannel();
                    if (whichpannel == 2) BuildPannel();
                    if (whichpannel == 3) PlanetPannel();
                    if (whichpannel == 4) DysonPannel();
                    if (whichpannel == 5) OtherPannel();
                    GUILayout.EndScrollView();
                }
                GUILayout.EndVertical();
            }


            GUILayout.EndArea();

        }

        /// <summary>
        /// 玩家面板
        /// </summary>
        public void PlayerPannel()
        {
            bool english = Localization.language != Language.zhCN;
            //左侧UI
            GUILayout.BeginHorizontal();

            GUILayout.BeginVertical(new[] { GUILayout.ExpandWidth(false) ,GUILayout.Width(english?18*heightdis:14*heightdis)});
            if (player != null)
            {
                for (int i = 0; i < 20; i++)
                {
                    GUILayout.BeginHorizontal(new[] { GUILayout.ExpandWidth(false) });
                    getLRstrValue(i, out float left, out float right, out string str);

                    float temp = GUILayout.HorizontalSlider(ChangeValueArray[i], left, right, new[] { GUILayout.Height(heightdis), GUILayout.Width(heightdis * 6) });
                    string result = "";
                    if (i == 8 && player.mecha != null) result += TGMKinttostring(player.mecha.reactorPowerGen, "W");
                    else if (i == 0 || i==10 || i==11 || i == 15) result = string.Format("{0:N2}", temp);
                    else result = (int)temp + "";
                    if (temp == ChangeValueArray[i] && i != 8)
                    {
                        string t = GUILayout.TextField(result, new[] { GUILayout.Height(heightdis), GUILayout.Width(heightdis * 3) });
                        if (result.Contains("."))
                        {
                            if(!float.TryParse(Regex.Replace(t, @"^[^0-9]+(.[^0-9]{2})?$", ""), out temp))
                            {
                                temp = ChangeValueArray[i];
                            }
                        }
                        else if (!float.TryParse(Regex.Replace(t, @"^[^0-9]", ""), out temp))
                        {
                            temp = ChangeValueArray[i];
                        }
                    }
                    else if (i == 8)
                    {
                        GUILayout.Label(result, style, new[] { GUILayout.Height(heightdis), GUILayout.Width(heightdis * 3) });
                    }
                    GUILayout.Label(str.getTranslate(), style, GUILayout.ExpandWidth(false));
                    OnChangeValue(Math.Max(Math.Min(temp, right), left), i);
                    GUILayout.EndHorizontal();
                }
            }
            GUILayout.EndVertical();

            GUILayout.BeginVertical();
            GUILayoutOption[] options = new[] { GUILayout.Height(heightdis), GUILayout.ExpandWidth(false), GUILayout.MinWidth(heightdis * 7) };
            {
                Infinitething.Value = GUILayout.Toggle(Infinitething.Value, "无限物品".getTranslate(), options);
                InfiniteSand.Value = GUILayout.Toggle(InfiniteSand.Value, "无限沙土".getTranslate(), options);
                lockpackage_bool.Value = GUILayout.Toggle(lockpackage_bool.Value, "锁定背包".getTranslate(), options);
                Property9999999 = GUILayout.Toggle(Property9999999, "无限元数据".getTranslate(), options);
                DroneNoenergy_bool.Value = GUILayout.Toggle(DroneNoenergy_bool.Value, "小飞机不耗能".getTranslate(), options);
                Infiniteplayerpower.Value = GUILayout.Toggle(Infiniteplayerpower.Value, "无限机甲能量".getTranslate(), options);
                unlockpointtech = GUILayout.Toggle(unlockpointtech, "科技点击解锁".getTranslate(), options);
                dismantle_but_nobuild.Value = GUILayout.Toggle(dismantle_but_nobuild.Value, "不往背包放东西".getTranslate(), options);
                QuickHandcraft.Value = GUILayout.Toggle(QuickHandcraft.Value, "机甲制造MAX".getTranslate(), options);
                QuickPlayerMine.Value = GUILayout.Toggle(QuickPlayerMine.Value, "机甲采矿MAX".getTranslate(), options);
                ItemList_bool.Value = GUILayout.Toggle(ItemList_bool.Value, "物品列表(Tab)".getTranslate(), options); 
                noneedwarp.Value = GUILayout.Toggle(noneedwarp.Value, "无需翘曲器曲速飞行".getTranslate(), options);
            }
            {
                var tempoptions = new[] { GUILayout.ExpandWidth(false), GUILayout.Height(heightdis) };
                if(GUILayout.Button((ChangePlayerbool ? "停止修改" : "启动修改").getTranslate(), options)) ChangePlayerbool = !ChangePlayerbool;
                if(GUILayout.Button("清空背包".getTranslate(), options)) Clearpackage();
                if (GUILayout.Button("初始化玩家".getTranslate(), options)) InitPlayer();

                GUILayout.BeginHorizontal(GUILayout.Height(heightdis * 2), GUILayout.ExpandWidth(false), GUILayout.MinWidth(heightdis * 7));
                GUILayout.Button("背包".getTranslate(), new[] { GUILayout.Height(heightdis*2)});
                GUILayout.BeginVertical(new[] { GUILayout.ExpandWidth(false) });
                if (GUILayout.Button("加一行".getTranslate(), tempoptions)) Operatepackagesize(1,0);
                if (GUILayout.Button("加一列".getTranslate(), tempoptions)) Operatepackagesize(0, 1);
                GUILayout.EndVertical();
                GUILayout.BeginVertical(new[] { GUILayout.ExpandWidth(false) });
                if (GUILayout.Button("减一行".getTranslate(), tempoptions)) Operatepackagesize(1, 0,false);
                if (GUILayout.Button("减一列".getTranslate(), tempoptions)) Operatepackagesize(0, 1,false);
                GUILayout.EndVertical();
                GUILayout.EndHorizontal();

                if (GUILayout.Button("解锁全部科技".getTranslate(), options)) UnlockallTech();
                if (GUILayout.Button("回退无穷科技".getTranslate(), options)) lockTech();

            }

            GUILayout.EndVertical();
            GUILayout.EndHorizontal();


        }

        public void BuildPannel()
        {
            bool english = Localization.language != Language.zhCN;
            GUILayout.BeginHorizontal();
            {
                GUILayout.BeginVertical(new[] {GUILayout.Width(english? heightdis*13:heightdis*10)});
                GUILayoutOption[] options = new[] { GUILayout.Height(heightdis), GUILayout.ExpandWidth(false), GUILayout.MinWidth(heightdis * 10) };

                GUILayout.BeginHorizontal(GUILayout.ExpandWidth(false));
                Buildmaxlen.Value = (int)GUILayout.HorizontalSlider(Buildmaxlen.Value, 15, 100, new[] {GUILayout.Height(heightdis),GUILayout.Width(heightdis * 5), GUILayout.ExpandWidth(false) });
                GUILayout.Label(Buildmaxlen.Value + " " + "建筑数量最大值".getTranslate(), style, GUILayout.ExpandWidth(false));
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal(GUILayout.ExpandWidth(false));
                StationStoExtra.Value = (int)GUILayout.HorizontalSlider(StationStoExtra.Value, 0, 100, new[] { GUILayout.Height(heightdis), GUILayout.Width(heightdis * 5), GUILayout.ExpandWidth(false) });
                GUILayout.Label(StationStoExtra.Value + " " + "运输站存储倍率".getTranslate(), style, GUILayout.ExpandWidth(false));
                GUILayout.EndHorizontal();
                RefreshStationStorage           = GUILayout.Toggle(RefreshStationStorage, "实时更改全部运输站存储倍率".getTranslate(), options);
                Tankcontentall.Value            = GUILayout.Toggle(Tankcontentall.Value, "储液站任意存".getTranslate(), options);
                Infinitestoragetank.Value       = GUILayout.Toggle(Infinitestoragetank.Value, "无限储液站".getTranslate(), options);
                TankMaxproliferator.Value       = GUILayout.Toggle(TankMaxproliferator.Value, "储液站无限增产".getTranslate(), options);
                StationfullCount_bool.Value     = GUILayout.Toggle(StationfullCount_bool.Value, "星球无限供货机".getTranslate(), options);
                Stationfullenergy.Value         = GUILayout.Toggle(Stationfullenergy.Value, "物流站永久满电".getTranslate(), options);
                StationfullCount                = GUILayout.Toggle(StationfullCount, "物流站要啥有啥".getTranslate(), options);
                StationMaxproliferator.Value    = GUILayout.Toggle(StationMaxproliferator.Value, "物流站无限增产".getTranslate(), options);
                StationSpray.Value              = GUILayout.Toggle(StationSpray.Value, "物流站内置喷涂".getTranslate(), options);
                StationPowerGen.Value           = GUILayout.Toggle(StationPowerGen.Value, "物流站内置发电".getTranslate(), options);
                Station_infiniteWarp_bool.Value = GUILayout.Toggle(Station_infiniteWarp_bool.Value, "物流站无限翘曲".getTranslate(), options);
                build_gascol_noequator.Value    = GUILayout.Toggle(build_gascol_noequator.Value, "无需赤道造采集器".getTranslate(), options);
                build_tooclose_bool.Value       = GUILayout.Toggle(build_tooclose_bool.Value, "强行近距离建造物流站".getTranslate(), options);
                InfineteStarPower.Value         = GUILayout.Toggle(InfineteStarPower.Value, "人造恒星无限能源".getTranslate(), options);
                PlanetPower_bool.Value          = GUILayout.Toggle(PlanetPower_bool.Value, "星球电网(人造恒星)".getTranslate(), options);
                if (PlanetPower_bool.Value)
                {
                    PlanetPower_bool.Value = GUILayout.Toggle(PlanetPower_bool.Value, "    覆盖全球".getTranslate(), options);
                    farconnectdistance = GUILayout.Toggle(farconnectdistance, "    超长连接".getTranslate(), options);
                }

                GUILayout.EndVertical();
            }
            {
                GUILayout.BeginVertical(GUILayout.ExpandWidth(false));
                GUILayoutOption[] options = new[] { GUILayout.Height(heightdis), GUILayout.ExpandWidth(false), GUILayout.MinWidth(heightdis * 7) };
                BuildNotime_bool.Value = GUILayout.Toggle(BuildNotime_bool.Value, "建筑秒完成".getTranslate(), options);
                blueprintpastenoneed_bool.Value = GUILayout.Toggle(blueprintpastenoneed_bool.Value, "蓝图建造无需材料".getTranslate(), options);
                ArchitectMode.Value = GUILayout.Toggle(ArchitectMode.Value, "建筑师模式".getTranslate(), options);
                Buildingnoconsume.Value = GUILayout.Toggle(Buildingnoconsume.Value, "新建设备不耗电".getTranslate(), new[] { GUILayout.Height(heightdis) });
                DriftBuildings = GUILayout.Toggle(DriftBuildings, "建筑抬升".getTranslate(), new[] { GUILayout.Height(heightdis) });
                if (DriftBuildings) GUILayout.Label("抬升层数:" + DriftBuildingLevel, new[] { GUILayout.Height(heightdis) });
                BeltSignalFunction.Value = GUILayout.Toggle(BeltSignalFunction.Value, "传送带信号功能".getTranslate(), new[] { GUILayout.Height(heightdis) });
                Quantumtransport_bool.Value = GUILayout.Toggle(Quantumtransport_bool.Value, "量子传输站".getTranslate(), new[] { GUILayout.Height(heightdis) });
                if (Quantumtransport_bool.Value)
                {
                    Quantumtransportpdwarp_bool.Value = GUILayout.Toggle(Quantumtransportpdwarp_bool.Value, "星球级翘曲全面供应".getTranslate(), new[] { GUILayout.Height(heightdis) });
                    Quantumtransportstarwarp_bool.Value = GUILayout.Toggle(Quantumtransportstarwarp_bool.Value, "星系级翘曲全面供应".getTranslate(), new[] { GUILayout.Height(heightdis) });
                    autochangeQuantumstationname = GUILayout.Toggle(autochangeQuantumstationname, "自动改名\"星球量子传输站\"".getTranslate(), new[] { GUILayout.Height(heightdis) });
                    autochangeQuantumStarstationname = GUILayout.Toggle(autochangeQuantumStarstationname, "自动改名\"星系量子传输站\"".getTranslate(), new[] { GUILayout.Height(heightdis) });
                    Quantumtransportbuild_bool.Value = GUILayout.Toggle(Quantumtransportbuild_bool.Value, "星球级材料供应".getTranslate(), new[] { GUILayout.Height(heightdis) });

                    if (Quantumtransportbuild_bool.Value)
                    {
                        options = new[] { GUILayout.Height(heightdis), GUILayout.ExpandWidth(false), GUILayout.MinWidth(heightdis * 5) };
                        GUILayout.BeginVertical();
                        GUILayout.BeginHorizontal();
                        QuantumtransportstationSupply.Value = GUILayout.Toggle(QuantumtransportstationSupply.Value, "物流站供应", options);
                        QuantumtransportstationDemand.Value = GUILayout.Toggle(QuantumtransportstationDemand.Value, "物流站拿取", options);
                        GUILayout.EndHorizontal();

                        GUILayout.BeginHorizontal();
                        QuantumtransportminerDemand.Value = GUILayout.Toggle(QuantumtransportminerDemand.Value, "矿机拿取", options);
                        GUILayout.EndHorizontal();

                        GUILayout.BeginHorizontal();
                        QuantumtransportsiloDemand.Value = GUILayout.Toggle(QuantumtransportsiloDemand.Value, "发射井弹射器供应", options);
                        GUILayout.EndHorizontal();

                        GUILayout.BeginHorizontal();
                        QuantumtransportlabSupply.Value = GUILayout.Toggle(QuantumtransportlabSupply.Value, "研究站供应", options);
                        QuantumtransportlabDemand.Value = GUILayout.Toggle(QuantumtransportlabDemand.Value, "研究站拿取", options);
                        GUILayout.EndHorizontal();

                        GUILayout.BeginHorizontal();
                        QuantumtransportpowerSupply.Value = GUILayout.Toggle(QuantumtransportpowerSupply.Value, "电力设备供应", options);
                        QuantumtransportpowerDemand.Value = GUILayout.Toggle(QuantumtransportpowerDemand.Value, "电力设备拿取", options);
                        GUILayout.EndHorizontal();

                        GUILayout.BeginHorizontal();
                        QuantumtransportassembleSupply.Value = GUILayout.Toggle(QuantumtransportassembleSupply.Value, "组装机供应", options);
                        QuantumtransportassembleDemand.Value = GUILayout.Toggle(QuantumtransportassembleDemand.Value, "组装机拿取", options);
                        GUILayout.EndHorizontal();

                        GUILayout.EndVertical();
                    }

                    GUILayout.BeginHorizontal(GUILayout.ExpandWidth(false));
                    int tempplanetquamaxpowerpertick = (int)GUILayout.HorizontalSlider(planetquamaxpowerpertick.Value, 60, 10000, new[] {GUILayout.Height(heightdis), GUILayout.Width(heightdis * 5) });
                    GUILayout.Label(tempplanetquamaxpowerpertick > 1000 ? tempplanetquamaxpowerpertick / 1000 + "G" : tempplanetquamaxpowerpertick + "MW " + "星球量子充电功率".getTranslate(), style, GUILayout.ExpandWidth(false));
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal(GUILayout.ExpandWidth(false));
                    int tempstarquamaxpowerpertick = (int)GUILayout.HorizontalSlider( starquamaxpowerpertick.Value, 60, 10000, new[] { GUILayout.Height(heightdis), GUILayout.Width(heightdis * 5) });
                    GUILayout.Label(tempstarquamaxpowerpertick > 1000 ? tempstarquamaxpowerpertick / 1000 + "G" : tempstarquamaxpowerpertick + "MW " + "星系量子充电功率".getTranslate(), style, GUILayout.ExpandWidth(false));
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal(GUILayout.ExpandWidth(false));
                    Quantumenergy.Value = (int)GUILayout.HorizontalSlider(Quantumenergy.Value, 0, 1000000, new[] { GUILayout.Height(heightdis), GUILayout.Width(heightdis * 5) });
                    GUILayout.Label(TGMKinttostring(Quantumenergy.Value, "J") + " " + "量子耗能/个".getTranslate(), style, GUILayout.ExpandWidth(false));
                    GUILayout.EndHorizontal();

                    if (tempplanetquamaxpowerpertick != planetquamaxpowerpertick.Value || tempstarquamaxpowerpertick != starquamaxpowerpertick.Value)
                    {
                        changequapowerpertick = true;
                        planetquamaxpowerpertick.Value = tempplanetquamaxpowerpertick;
                        starquamaxpowerpertick.Value = tempstarquamaxpowerpertick;
                    }
                }

                GUILayout.EndVertical();

                if (BeltSignalFunction.Value) InitBeltSignalDiction();
                if (autochangeQuantumstationname) autochangeQuantumStarstationname = false;
                if (autochangeQuantumStarstationname) autochangeQuantumstationname = false;
            }
            GUILayout.EndHorizontal();
        }

        public void PlanetPannel()
        {
            GUILayout.Label("\"生成矿物\":鼠标左键生成矿物，鼠标右键取消。\n\"删除矿物\"：按x键进入拆除模式可拆除矿物。\n可点击下面的按钮更改矿物，数字框为生成矿数量".getTranslate(), style);
            GUILayout.BeginArea(new Rect(0, heightdis * 2, 24 * heightdis, 20 * heightdis));
            bool english = Localization.language != Language.zhCN;
            string[] tempstr = new string[13] { "生成矿物", "删除矿物", "移动单矿", "移动矿堆", "不排列", "整理所有矿", "切割矿脉", "还原海洋", Localization.language != Language.zhCN ? "Station_trash" : "垃圾站", "不需要沙土", "星球矿机", "自动改名", "星球矿机无消耗" };

            int width = 0;
            for (int i = 0; i < tempstr.Length; i++)
            {
                bool tempvalue = false;
                switch (i)
                {
                    case 0: tempvalue = addveinbool; break;
                    case 1: tempvalue = deleteveinbool.Value; break;
                    case 2: tempvalue = changeveinposbool; break;
                    case 3: tempvalue = changeveingroupposbool; break;
                    case 4: tempvalue = NotTidyVein.Value; break;
                    case 5: tempvalue = getallVein_bool; break;
                    case 6: tempvalue = changexveinspos; break;
                    case 7: tempvalue = restorewater; break;
                    case 8: tempvalue = StationTrash.Value; break;
                    case 9: tempvalue = noneedtrashsand.Value; break;
                    case 10: tempvalue = StationMiner.Value; break;
                    case 11: tempvalue = autochangestationname.Value; break;
                    case 12: tempvalue = Station_miner_noconsume_bool.Value; break;
                }
                tempvalue = GUI.Toggle(new Rect(i == 9 || i == 11 || i == 12 ? 10 : 0, heightdis * (i + width), english ? heightdis * 9 : heightdis * 5, heightdis), tempvalue, tempstr[i].getTranslate());
                switch (i)
                {
                    case 0:
                        addveinbool = tempvalue;
                        if (addveinbool)
                        {
                            changeveinposbool = false;
                            changeveingroupposbool = false;
                            getallVein_bool = false;
                            changexveinspos = false;
                        }
                        break;
                    case 1: deleteveinbool.Value = tempvalue; break;
                    case 2:
                        changeveinposbool = tempvalue;
                        if (changeveinposbool)
                        {
                            addveinbool = false;
                            changeveingroupposbool = false;
                            getallVein_bool = false;
                            changexveinspos = false;
                        }
                        break;
                    case 3:
                        changeveingroupposbool = tempvalue;
                        if (changeveingroupposbool)
                        {
                            addveinbool = false;
                            changeveinposbool = false;
                            getallVein_bool = false;
                            changexveinspos = false;
                        }
                        break;
                    case 4:
                        NotTidyVein.Value = tempvalue;
                        break;
                    case 5:
                        width++;
                        GUI.Label(new Rect(0, heightdis * (i + width++), english ? heightdis * 9 : heightdis * 5, heightdis), "整理为".getTranslate() + veinlines.Value + "行".getTranslate());
                        veinlines.Value = (int)GUI.HorizontalSlider(new Rect(0, heightdis * (i + width), english ? heightdis * 9 : heightdis * 5, heightdis), veinlines.Value, 1, 20);
                        getallVein_bool = tempvalue;
                        if (getallVein_bool)
                        {
                            addveinbool = false;
                            changeveinposbool = false;
                            changeveingroupposbool = false;
                            changexveinspos = false;
                        }
                        break;
                    case 6:
                        width++;
                        GUI.Label(new Rect(0, heightdis * (i + width++), english ? heightdis * 9 : heightdis * 5, heightdis), "切割出".getTranslate() + changeveinsposx.Value + "个".getTranslate());
                        changeveinsposx.Value = (int)GUI.HorizontalSlider(new Rect(0, heightdis * (i + width), english ? heightdis * 9 : heightdis * 5, heightdis), changeveinsposx.Value, 2, 72);

                        changexveinspos = tempvalue;
                        if (changexveinspos)
                        {
                            addveinbool = false;
                            changeveinposbool = false;
                            changeveingroupposbool = false;
                            getallVein_bool = false;
                        }

                        break;
                    case 7: restorewater = tempvalue; break;
                    case 8: StationTrash.Value = tempvalue; break;
                    case 9: noneedtrashsand.Value = tempvalue; break;
                    case 10: StationMiner.Value = tempvalue; break;
                    case 11: autochangestationname.Value = tempvalue; break;
                    case 12: Station_miner_noconsume_bool.Value = tempvalue; break;
                }
            }
            int tempheight = width + tempstr.Length;
            GUI.Label(new Rect(0, heightdis * tempheight++, english ? heightdis * 9 : heightdis * 5, heightdis), "星球矿机采矿速率".getTranslate());
            Stationminenumber.Value = (int)GUI.HorizontalSlider(new Rect(0, heightdis * tempheight, english ? heightdis * 9 : heightdis * 5, heightdis), Stationminenumber.Value, 1, 100);
            GUI.Label(new Rect(0, heightdis * tempheight + togglesize, english ? heightdis * 9 : heightdis * 5, heightdis), Stationminenumber.Value + "");

            stopUniverse = GUI.Toggle(new Rect(english ? heightdis * 9 : heightdis * 5, 0, heightdis * 4, heightdis), stopUniverse, "停止公转自转".getTranslate());
            if (GUI.Button(new Rect(english ? heightdis * 9 : heightdis * 5, heightdis, heightdis * 4, heightdis), LDB.items.Select(LDB.veins.Select(veintype).MiningItem).name))
            {
                dropdownbutton = !dropdownbutton;
                addveinbool = false;
            }
            if (dropdownbutton)
            {
                for (int i = 1; i <= 14; i++)
                {
                    if (GUI.Button(new Rect(english ? heightdis * 9 : heightdis * 5, heightdis * (i + 1), heightdis * 4, heightdis), LDB.items.Select(LDB.veins.Select(i).MiningItem).name))
                    {
                        dropdownbutton = !dropdownbutton;
                        veintype = i;
                    }
                }
            }
            tempstr = new string[13] { !restorewater ? "铺平整个星球" : "还原全部海洋", "铺平整个星球(地基)", "铺平整个星球(自定义颜色)", "掩埋全部矿", "删除全部矿", "超密铺采集器", "删除当前星球所有建筑", "删除当前星球所有建筑(不掉落)", "初始化当前星球", "初始化当前星球(不要海洋)", watertype, "改变海洋类型", "还原所有海洋类型" };
            for (int i = 0; i < tempstr.Length; i++)
            {
                Rect temprect = new Rect(english ? heightdis * 13 : heightdis * 11, heightdis * i, english ? heightdis * 10 : heightdis * 8, heightdis);
                if (i == 10)
                    watertype = GUI.TextField(temprect, tempstr[i].getTranslate());
                else if (GUI.Button(temprect, tempstr[i].getTranslate()))
                {
                    switch (i)
                    {
                        case 0: OnSetBase(0); break;
                        case 1: OnSetBase(1); break;
                        case 2: OnSetBase(2); break;
                        case 3: BuryAllvein(); break;
                        case 4: RemoveAllvein(); break;
                        case 5: SetMaxGasStation(); break;
                        case 6: RemoveAllBuild(0); break;
                        case 7: RemoveAllBuild(1); break;
                        case 8: RemoveAllBuild(2); break;
                        case 9: RemoveAllBuild(3); break;

                        case 11: Changewatertype(); break;
                        case 12:
                            foreach (KeyValuePair<int, int> wap in OrwatertypePlanetArray)
                            {
                                GameMain.galaxy.PlanetById(wap.Key).waterItemId = wap.Value;
                            }
                            watertypePlanet.Value = "";
                            watertypePlanetArray = new Dictionary<int, int>();
                            break;
                    }
                }
            }
            GUILayout.EndArea();
        }

        public void DysonPannel()
        {
            bool english = Localization.language != Language.zhCN;
            int line = 0; 
            GUI.Label(new Rect(0, line++ * heightdis, MainWindow_width.Value, heightdis * 2), "注意事项:戴森云和戴森壳不要出现一层轨道都没有的情况(用前存档)".getTranslate());
            if (!(GameMain.localStar == null || GameMain.data.dysonSpheres == null || GameMain.data.dysonSpheres[GameMain.localStar.index] == null))
            {
                DysonSphere ds = GameMain.data.dysonSpheres[GameMain.localStar.index];
                List<int> layerlist = new List<int>();
                foreach (DysonSphereLayer dysonSphereLayer in GameMain.data.dysonSpheres[GameMain.localStar.index].layersIdBased)
                {
                    if (dysonSphereLayer != null)
                        layerlist.Add(dysonSphereLayer.id);
                }
                GUI.Label(new Rect(0, line++ * heightdis, MainWindow_width.Value, heightdis * 2), "点击删除下列戴森壳层级".getTranslate());
                for (int i = 1; i <= 10; i++)
                {
                    if (layerlist.Contains(i) && GUI.Button(new Rect((i - 1) * heightdis * 2, heightdis * line, heightdis * 2, heightdis * 2), i.ToString()))
                    {
                        for (int id = 1; id < ds.rocketCursor; ++id)
                        {
                            if (ds.rocketPool[id].id == id)
                            {
                                ds.RemoveDysonRocket(id);
                            }
                        }
                        ds.RemoveLayer(i);
                    }
                }
                line += 3;
                if (GUI.Button(new Rect(0, heightdis * line++, heightdis * 10, heightdis * 2), "初始化当前戴森球".getTranslate()))
                {
                    if (GameMain.localStar != null && GameMain.data.dysonSpheres[GameMain.localStar.index] != null)
                    {
                        int index = GameMain.localStar.index;
                        GameMain.data.dysonSpheres[index] = new DysonSphere();
                        GameMain.data.dysonSpheres[index].Init(GameMain.data, GameMain.localStar);
                        GameMain.data.dysonSpheres[index].ResetNew();
                    }
                }
                line++;
                if (GUI.Button(new Rect(0, heightdis * line++, heightdis * 10, heightdis * 2), "瞬间完成戴森球(用前存档)".getTranslate()))
                {
                    FinishDysonShell();
                }
                line += 2;
            }
            quickEjector = GUI.Toggle(new Rect(0, heightdis * line++, heightdis * 10, heightdis), quickEjector, "极速轨道弹射器(慎用)".getTranslate());
            quicksilo = GUI.Toggle(new Rect(0, heightdis * line++, heightdis * 10, heightdis), quicksilo, "极速垂直发射井(慎用)".getTranslate());
            if (cancelsolarbullet.Value != GUI.Toggle(new Rect(0, heightdis * line++, heightdis * 10, heightdis), cancelsolarbullet.Value, "跳过太阳帆子弹阶段".getTranslate()))
            {
                cancelsolarbullet.Value = !cancelsolarbullet.Value;
                playcancelsolarbullet = cancelsolarbullet.Value;
            }
            quickabsorbsolar.Value = GUI.Toggle(new Rect(0, heightdis * line++, heightdis * 10, heightdis), quickabsorbsolar.Value, "跳过太阳帆吸收阶段".getTranslate());
            QuickabortSwarm.Value = GUI.Toggle(new Rect(0, heightdis * line++, heightdis * 10, heightdis), QuickabortSwarm.Value, "太阳帆秒吸收".getTranslate());
            if (alwaysemission.Value != GUI.Toggle(new Rect(0, heightdis * line++, heightdis * 10, heightdis), alwaysemission.Value, "全球打帆".getTranslate()))
            {
                alwaysemission.Value = !alwaysemission.Value;
                alwaysemissiontemp = alwaysemission.Value;
            }
            if(RandomEmission.Value != GUI.Toggle(new Rect(0, heightdis * line++, heightdis * 10, heightdis), RandomEmission.Value, "间隔发射".getTranslate()))
            {
                RandomEmission.Value = !RandomEmission.Value;
                RandomEmissionEjectorSilo();
            }
            var temp = GUI.Toggle(new Rect(0, heightdis * line++, heightdis * 10, heightdis), ChangeDysonradius.Value, "开放戴森壳半径上下限(用前存档)".getTranslate());
            GUI.Label(new Rect(0, heightdis * line++, heightdis * 10, heightdis), "最小半径".getTranslate() + ":100");
            GUI.Label(new Rect(0, heightdis * line++, heightdis * 10, heightdis), "最大半径".getTranslate() + ":"+TGMKinttostring(MaxOrbitRadiusConfig.Value,""));
            var tempvalue = Regex.Replace(GUI.TextField(new Rect(0, heightdis * line++, heightdis * 10, heightdis), MaxOrbitRadiusConfig.Value+"", 10), @"[^0-9]", "");
            if(int.TryParse(tempvalue,out int result))
            {
                if (result < 100000)
                {
                    result = 100000;
                }
                else if (result > 10_000_000)
                {
                    result = 10_000_000;
                }
                MaxOrbitRadiusConfig.Value = result;
            }
            if(temp != ChangeDysonradius.Value)
            {
                ChangeDysonradius.Value = temp;
                var selectdyson = UIRoot.instance?.uiGame?.dysonEditor?.selection?.viewDysonSphere;
                foreach (var dyson in GameMain.data?.dysonSpheres)
                {
                    if (dyson != null)
                    {
                        if (temp)
                        {
                            dyson.maxOrbitRadius = MaxOrbitRadiusConfig.Value;
                            dyson.minOrbitRadius = 100;
                        }
                        else
                        {
                            dyson.minOrbitRadius = dyson.starData.physicsRadius * 1.5f;
                            if (dyson.minOrbitRadius < 4000f)
                            {
                                dyson.minOrbitRadius = 4000f;
                            }
                            dyson.maxOrbitRadius = dyson.defOrbitRadius * 2f;
                            if (dyson.starData.type == EStarType.GiantStar)
                            {
                                dyson.minOrbitRadius *= 0.6f;
                            }
                            dyson.minOrbitRadius = Mathf.Ceil(dyson.minOrbitRadius / 100f) * 100f;
                            dyson.maxOrbitRadius = Mathf.Round(dyson.maxOrbitRadius / 100f) * 100f;
                        }
                        if (selectdyson == dyson)
                        {
                            UIRoot.instance.uiGame.dysonEditor.controlPanel.OnViewStarChange(dyson);
                        }
                    }
                }
            }
        }

        public void OtherPannel()
        {
            bool english = Localization.language != Language.zhCN;
            if (player == null) return;
            GUILayout.BeginArea(new Rect(10, 10, 10 + (english ? heightdis * 12 : heightdis * 10), heightdis * 17));
            {
                int lines = 0;
                Rect t = new Rect(0, 0, english ? heightdis * 12 : heightdis * 10, heightdis);
                GUI.Label(RectChanged(t, heightdis * lines++, 2), "以下设置需要进入存档".getTranslate(), style);
                sunlight_bool.Value = GUI.Toggle(RectChanged(t, heightdis * lines++, 2), sunlight_bool.Value, "夜灯".getTranslate());
                CloseUIAbnormalityTip.Value = GUI.Toggle(RectChanged(t, heightdis * lines++, 2), CloseUIAbnormalityTip.Value, "关闭异常提示".getTranslate());
                InspectDisNoLimit.Value = GUI.Toggle(RectChanged(t, heightdis * lines++, 2), InspectDisNoLimit.Value, "操作范围不受限制".getTranslate());
                if (InspectDisNoLimit.Value)
                {
                    player.mecha.buildArea = 200;
                }
                else
                {
                    player.mecha.buildArea = 80;
                }
                Maxproliferator.Value = GUI.Toggle(RectChanged(t, heightdis * lines++, 2), Maxproliferator.Value, "增产点数上限10".getTranslate());
                incAbility = Maxproliferator.Value?10:4;
                pasteanyway = GUI.Toggle(RectChanged(t, heightdis * lines++, 2), pasteanyway, "蓝图强制粘贴".getTranslate());
                PasteBuildAnyWay = GUI.Toggle(RectChanged(t, heightdis * lines++, 2), PasteBuildAnyWay, "建筑铺设无需条件".getTranslate());
                if (closeallcollider != GUI.Toggle(RectChanged(t, heightdis * lines++, 2), closeallcollider, "关闭所有碰撞体".getTranslate()))
                {
                    closeallcollider = !closeallcollider;
                    ColliderPool.instance.gameObject.SetActive(!closeallcollider);
                }
                allhandcraft.Value = GUI.Toggle(RectChanged(t, heightdis * lines++, 2), allhandcraft.Value, "全部手搓".getTranslate());
                if (allhandcraft.Value)
                {
                    for (int i = 0; i < LDB.recipes.dataArray.Length; i++)
                    {
                        LDB.recipes.dataArray[i].Handcraft = true;
                    }
                }
                quickproduce.Value = GUI.Toggle(RectChanged(t, heightdis * lines++, 2), quickproduce.Value, "快速生产".getTranslate());
                if (quickproduce.Value)
                {
                    for (int i = 0; i < LDB.recipes.dataArray.Length; i++)
                    {
                        LDB.recipes.dataArray[i].TimeSpend = 1;
                    }
                }
                stackmultiple1 = Regex.Replace(GUI.TextField(RectChanged(t, heightdis * lines++, 2), stackmultiple1, 10), @"[^0-9]", "");
                if (GUI.Button(RectChanged(t, heightdis * lines++, 2), "设置堆叠倍率".getTranslate()))
                {
                    StackMultiple.Value = int.Parse(stackmultiple1);
                    ChangeItemstack();
                }
                multipelsmelt1 = Regex.Replace(GUI.TextField(RectChanged(t, heightdis * lines++, 2), multipelsmelt1, 10), @"[^0-9]", "");
                if (GUI.Button(RectChanged(t, heightdis * lines++, 2), "设置冶炼倍数".getTranslate()))
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
                    MULTIPELSMELT.Value = int.Parse(multipelsmelt1);
                    if (MULTIPELSMELT.Value < 1)
                    {
                        MULTIPELSMELT.Value = 1;
                        multipelsmelt1 = "1";
                    }else if (MULTIPELSMELT.Value > 100)
                    {
                        MULTIPELSMELT.Value = 100;
                        multipelsmelt1 = "100";
                    }
                    ChangeRecipe();
                }
                if (GUI.Button(RectChanged(t, heightdis * lines++, 2), "成就重新检测".getTranslate()))
                {
                    GameMain.data.abnormalData.runtimeDatas = new AbnormalityRuntimeData[3000];
                }
                if (GUI.Button(RectChanged(t, heightdis * lines++, 2), "取消所有勾选".getTranslate()))
                {
                    CancelAllToggle();
                }
            }
            GUILayout.EndArea();
            GUILayout.BeginArea(new Rect(20 + (english ? heightdis * 12 : heightdis * 10), 10, heightdis * 10, heightdis * 15));
            {
                int lines = 0;
                GUI.Label(new Rect(0, heightdis * lines++, heightdis * 10, heightdis), "窗口颜色及透明度".getTranslate() + " RGBA", style);
                mytexturer.Value = GUI.HorizontalSlider(new Rect(heightdis * 2, heightdis * lines, heightdis * 5, heightdis), mytexturer.Value, 0, 1);
                GUI.Label(new Rect(0, heightdis * lines++ - 5, heightdis * 2, heightdis), "R:" + string.Format("{0:N2}", mytexturer.Value), style);
                mytextureg.Value = GUI.HorizontalSlider(new Rect(heightdis * 2, heightdis * lines, heightdis * 5, heightdis), mytextureg.Value, 0, 1);
                GUI.Label(new Rect(0, heightdis * lines++ - 5, heightdis * 2, heightdis), "G:" + string.Format("{0:N2}", mytextureg.Value), style);
                mytextureb.Value = GUI.HorizontalSlider(new Rect(heightdis * 2, heightdis * lines, heightdis * 5, heightdis), mytextureb.Value, 0, 1);
                GUI.Label(new Rect(0, heightdis * lines++ - 5, heightdis * 2, heightdis), "B:" + string.Format("{0:N2}", mytextureb.Value), style);
                mytexturea.Value = GUI.HorizontalSlider(new Rect(heightdis * 2, heightdis * lines, heightdis * 5, heightdis), mytexturea.Value, 0, 1);
                GUI.Label(new Rect(0, heightdis * lines++ - 5, heightdis * 2, heightdis), "A:" + string.Format("{0:N2}", mytexturea.Value), style);
                if (GUI.Button(new Rect(0, heightdis * lines++, heightdis * 4, heightdis), "应用".getTranslate()))
                {
                    for (int i = 0; i < mytexture.width; i++)
                    {
                        for (int j = 0; j < mytexture.height; j++)
                        {
                            mytexture.SetPixel(i, j, new Color(mytexturer.Value, mytextureg.Value, mytextureb.Value, mytexturea.Value));
                        }
                    }
                    mytexture.Apply();
                }

                GUI.Label(new Rect(0, heightdis * lines++, heightdis * 10, heightdis), "窗口字体颜色".getTranslate() + " RGB", style);
                textcolorr.Value = GUI.HorizontalSlider(new Rect(heightdis * 2, heightdis * lines, heightdis * 5, heightdis), textcolorr.Value, 0, 1);
                GUI.Label(new Rect(0, heightdis * lines++ - 5, heightdis * 2, heightdis), "R:" + string.Format("{0:N2}", textcolorr.Value), style);
                textcolorg.Value = GUI.HorizontalSlider(new Rect(heightdis * 2, heightdis * lines, heightdis * 5, heightdis), textcolorg.Value, 0, 1);
                GUI.Label(new Rect(0, heightdis * lines++ - 5, heightdis * 2, heightdis), "G:" + string.Format("{0:N2}", textcolorg.Value), style);
                textcolorb.Value = GUI.HorizontalSlider(new Rect(heightdis * 2, heightdis * lines, heightdis * 5, heightdis), textcolorb.Value, 0, 1);
                GUI.Label(new Rect(0, heightdis * lines++ - 5, heightdis * 2, heightdis), "B:" + string.Format("{0:N2}", textcolorb.Value), style);

                ChangeQuickKey = GUI.Toggle(new Rect(0, heightdis * lines++, heightdis * 10, heightdis), ChangeQuickKey, !ChangeQuickKey ? "改变窗口快捷键".getTranslate() : "点击确认".getTranslate());
                GUI.TextArea(new Rect(0, heightdis * lines++, heightdis * 6, heightdis), tempShowWindow.ToString());
                GUI.skin.button.normal.textColor = new Color(textcolorr.Value, textcolorg.Value, textcolorb.Value);
                GUI.skin.textArea.normal.textColor = new Color(textcolorr.Value, textcolorg.Value, textcolorb.Value);
                GUI.skin.textField.normal.textColor = new Color(textcolorr.Value, textcolorg.Value, textcolorb.Value);
                GUI.skin.toggle.normal.textColor = new Color(textcolorr.Value, textcolorg.Value, textcolorb.Value);
                GUI.skin.toggle.onNormal.textColor = new Color(textcolorr.Value, textcolorg.Value, textcolorb.Value);

                if (CloseUIpanel.Value != GUI.Toggle(new Rect(0, heightdis * lines++, heightdis * 10, heightdis), CloseUIpanel.Value, "关闭白色面板".getTranslate()))
                {
                    CloseUIpanel.Value = !CloseUIpanel.Value;
                    ui_MultiFunctionPanel.SetActive(!CloseUIpanel.Value);
                }
            }
            GUILayout.EndArea();
        }

        public void LogisticsPannel()
        {
            GUILayout.BeginVertical();
            {
                GUILayout.BeginHorizontal();
                GUILayoutOption[] options = new[] { GUILayout.Height(heightdis) };
                if(Mechalogistics_bool.Value != GUILayout.Toggle(Mechalogistics_bool.Value, "强化机甲物流".getTranslate(), options))
                {
                    Mechalogistics_bool.Value = !Mechalogistics_bool.Value;
                    if (Mechalogistics_bool.Value && !player.deliveryPackage.unlocked)
                    {
                        player.deliveryPackage.unlocked = true;
                        player.deliveryPackage.colCount = 1;
                        player.deliveryPackage.NotifySizeChange();
                    }
                }
                MechalogisticsPlanet_bool.Value = GUILayout.Toggle(MechalogisticsPlanet_bool.Value, "机甲物流专用星".getTranslate(), options);
                MechalogStoragerecycle_bool.Value = GUILayout.Toggle(MechalogStoragerecycle_bool.Value, "回收使用储物仓".getTranslate(), options);
                MechalogStorageprovide_bool.Value = GUILayout.Toggle(MechalogStorageprovide_bool.Value, "需求使用储物仓".getTranslate(), options);
                MechalogStationrecycle_bool.Value = GUILayout.Toggle(MechalogStationrecycle_bool.Value, "回收使用物流站".getTranslate(), options);
                MechalogStationprovide_bool.Value = GUILayout.Toggle(MechalogStationprovide_bool.Value, "需求使用物流站".getTranslate(), options);

                GUILayout.EndHorizontal();
            }
            GUILayout.Label("如果启动机甲物流，物流背包没有解锁会自动解锁一列。");
            GUILayout.Label("物流背包里的设置可以直接从全星系的存储设备中获取或回收");
            GUILayout.Label("设置机甲物流专用星后，只有名字是\"机甲物流\"的星球才会进行物流操作");
            GUILayout.EndVertical();
        }

        #endregion

        #region 量子传输站
        public void takeitemfromstarsuperstation()
        {
            if (GameMain.data?.galacticTransport?.stationPool == null) return;
            StarSuperStation = new List<int>();
            SuperStation = new List<int>();
            StarSuperStationItemidstore = new Dictionary<int, int[]>();
            PlanetSuperStationItemidstore = new Dictionary<int, Dictionary<int, List<int[]>>>();
            for(int i=0;i< GameMain.data.galacticTransport.stationPool.Length;i++)
            {
                StationComponent sc = GameMain.data.galacticTransport.stationPool[i];
                if (sc == null || sc.storage == null || sc.name == null || sc.isVeinCollector || sc.isCollector) continue;
                if (sc.name == "星系量子传输站")
                {
                    StarSuperStation.Add(i);
                    for (int j = 0; j < sc.storage.Length; j++)
                    {
                        if (sc.storage[j].itemId > 0)
                        {
                            if (!StarSuperStationItemidstore.ContainsKey(sc.storage[j].itemId))
                            {
                                StarSuperStationItemidstore.Add(sc.storage[j].itemId, new int[] { StarSuperStation.Count - 1, j });
                            }
                            if (sc.storage[j].count < 0)
                                sc.storage[j].count = 0;
                            if (sc.storage[j].inc < 0)
                                sc.storage[j].inc = 0;
                            else if(sc.storage[j].inc > sc.storage[j].count * 4)
                            {
                                sc.storage[j].inc = sc.storage[j].count * 4;
                            }
                        }
                    }
                    if (changequapowerpertick)
                    {
                        try
                        {
                            GameMain.galaxy.PlanetById(sc.planetId).factory.powerSystem.consumerPool[sc.pcId].workEnergyPerTick = (long)starquamaxpowerpertick.Value * 16667;
                        }
                        catch{}
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
                        catch{}
                    }

                }
            }
            int SuperStationCount = SuperStation.Count;
            int StarSuperStationCount = StarSuperStation.Count;
            if (SuperStationCount == 0 && StarSuperStationCount == 0) return;
            foreach (StationComponent sc in GameMain.data.galacticTransport.stationPool)
            {
                if (sc == null || !sc.isCollector) continue;
                sc.storage[0].count -= doitemfromStarsuper(sc.storage[0].itemId, sc.storage[0].count, 0, false)[0];
                sc.storage[1].count -= doitemfromStarsuper(sc.storage[1].itemId, sc.storage[1].count, 0, false)[0];
            }
            changequapowerpertick = false;
            if (Math.Max(StarSuperStation.Count, SuperStation.Count) > warpstationqua.Length - 50)
                SetSuperStationCapacity(Math.Max(StarSuperStation.Count, SuperStation.Count) + 50);
            for (int j = 0; j < SuperStationCount; j++)
            {
                StationComponent sc = GameMain.data.galacticTransport.stationPool[SuperStation[j]];
                int pdID = sc.planetId;
                for (int i = sc.storage.Length-1; i >= 0; i--)
                {
                    int itemID = sc.storage[i].itemId;
                    if (itemID > 0)
                    {
                        if (!PlanetSuperStationItemidstore.ContainsKey(pdID))
                        {
                            PlanetSuperStationItemidstore.Add(pdID, new Dictionary<int, List<int[]>>());
                        }
                        if (!PlanetSuperStationItemidstore[pdID].ContainsKey(itemID))
                        {
                            PlanetSuperStationItemidstore[pdID].Add(itemID, new List<int[]>() { new int[] { j, i } });
                        }
                        else
                        {
                            PlanetSuperStationItemidstore[pdID][itemID].Add(new int[] { j, i });
                        }
                        if (sc.storage[i].remoteLogic == ELogisticStorage.Demand && sc.storage[i].count < sc.storage[i].max)
                        {
                            if (sc.storage[i].max > sc.storage[i].count)
                            {
                                int[] takeitem = doitemfromStarsuper(itemID, sc.storage[i].max - sc.storage[i].count);
                                sc.storage[i].count += takeitem[0];
                                sc.storage[i].inc += takeitem[1];
                            }
                            else if (sc.storage[i].max < sc.storage[i].count)
                            {
                                int[] takeitem = doitemfromStarsuper(itemID, sc.storage[i].count - sc.storage[i].max, sc.storage[i].inc, false);
                                sc.storage[i].count -= takeitem[0];
                                sc.storage[i].inc -= takeitem[1];
                            }
                        }
                        else if(sc.storage[i].remoteLogic == ELogisticStorage.Supply && sc.storage[i].count > 0)
                        {
                            int[] putitem=doitemfromStarsuper(itemID, sc.storage[i].count, sc.storage[i].inc, false);
                            sc.storage[i].count -= putitem[0];
                            sc.storage[i].inc -= putitem[1];
                        }
                        if (sc.storage[i].count < 0)
                            sc.storage[i].count = 0;
                        if (sc.storage[i].inc < 0)
                            sc.storage[i].inc = 0;
                    }
                }
                if (Quantumtransportpdwarp_bool.Value && sc.warperCount == 0)
                    sc.warperCount += doitemfromStarsuper(1210, 50, 0)[0];
            }
            for (int j = 0; j < StarSuperStationCount; j++)
            {
                StationComponent sc = GameMain.data.galacticTransport.stationPool[StarSuperStation[j]];
                if (Quantumtransportstarwarp_bool.Value && sc.warperCount == 0)
                    sc.warperCount += doitemfromStarsuper(1210, 50, 0)[0];

            }
            if (Quantumtransportbuild_bool.Value)
                SuperStationProvide();

        }
        public void StationSupplyDemand(FactorySystem fs, int pdid)
        {
            foreach (StationComponent sc in fs.factory.transport.stationPool)
            {
                if (sc == null) continue;
                if (sc.storage != null  && sc.name != "星球量子传输站")
                {
                    for (int i = 0; i < sc.storage.Length; i++)
                    {
                        if (sc.storage[i].itemId > 0)
                        {
                            if (QuantumtransportstationDemand.Value)
                            {
                                if(sc.storage[i].localLogic == ELogisticStorage.Supply)
                                {
                                    if (sc.name != "星系量子传输站" && sc.storage[i].count > 0)
                                    {
                                        int[] takeitem = doitemfromPlanetSuper(sc.storage[i].itemId, sc.storage[i].count, pdid, sc.storage[i].inc, false);
                                        sc.storage[i].count -= takeitem[0];
                                        sc.storage[i].inc -= takeitem[1];
                                    }
                                }
                                if(sc.storage[i].localLogic == ELogisticStorage.Demand && sc.storage[i].count > sc.storage[i].max)
                                {
                                    int[] takeitem = doitemfromPlanetSuper(sc.storage[i].itemId, sc.storage[i].count - sc.storage[i].max, pdid, sc.storage[i].inc, false);
                                    sc.storage[i].count -= takeitem[0];
                                    sc.storage[i].inc -= takeitem[1];
                                }
                            }
                            if (QuantumtransportstationSupply.Value)
                            {
                                if (sc.storage[i].localLogic == ELogisticStorage.Demand)
                                {
                                    if (sc.storage[i].count <= sc.storage[i].max)
                                    {
                                        int[] takeitem = doitemfromPlanetSuper(sc.storage[i].itemId, sc.storage[i].max - sc.storage[i].count, pdid);
                                        sc.storage[i].count += takeitem[0];
                                        sc.storage[i].inc += takeitem[1];
                                    }
                                }
                            }
                        }
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
                if (sc.id > 0 && sc.entityId > 0 && sc.bulletCount<=1)
                {
                    int[] takeitem = doitemfromPlanetSuper(1503, 20, pdid);
                    fs.siloPool[sc.id].bulletCount += takeitem[0];
                    fs.siloPool[sc.id].bulletInc += takeitem[1];
                }
            }
            foreach (EjectorComponent ec in fs.ejectorPool)
            {
                if (ec.id > 0 && ec.entityId > 0 && ec.bulletCount<=1)
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
                    else if(QuantumtransportpowerSupply.Value)
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
            if (take)
            {
                count = Math.Min(count, sc.storage[i].count);
                if (sc.energy <= count * energycost && sc.energy > 0)
                {
                    count = Math.Min(count, (int)(sc.energy / energycost));
                    if (count == 0) return new int[2] { 0, 0 };
                }
                inc = Math.Min(count * 4, sc.storage[i].inc);
                sc.energy -= count * energycost;
                sc.storage[i].count -= count;
                sc.storage[i].inc -= inc;
            }
            else
            {
                if (sc.energy >= 0 && sc.energy < count * energycost)
                {
                    count = Math.Min(count, (int)(sc.energy / energycost));
                    if (count == 0) return new int[2] { 0, 0 };
                }
                if (sc.storage[i].count + count <= sc.storage[i].max)
                {
                    inc = Math.Min(count * incAbility, inc);
                    sc.storage[i].count += count;
                    sc.storage[i].inc += inc;
                }
                else if (sc.storage[i].localLogic==ELogisticStorage.None && sc.storage[i].remoteLogic==ELogisticStorage.None && sc.storage[i].max <int.MaxValue-100)
                {
                    if (sc.storage[i].count + count <= int.MaxValue - 100)
                    {
                        inc = Math.Min(count * incAbility, inc);
                        sc.storage[i].max = sc.storage[i].count + count;
                        sc.storage[i].count = sc.storage[i].max;
                        sc.storage[i].inc += inc;
                    }
                    else
                    {
                        count = int.MaxValue - 100 - sc.storage[i].count;
                        sc.storage[i].max = sc.storage[i].count + count;
                        sc.storage[i].count = sc.storage[i].max;
                        sc.storage[i].inc += inc;
                    }
                }
                else
                {
                    count=sc.storage[i].max - sc.storage[i].count;
                    inc = Math.Min(count * incAbility, inc);
                    sc.storage[i].count = sc.storage[i].max;
                    sc.storage[i].inc += inc;
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
                if (sc.storage[storageindex].localLogic != ELogisticStorage.None)
                {
                    if (take && sc.storage[storageindex].localLogic != ELogisticStorage.Supply) continue;
                    if (!take && sc.storage[storageindex].localLogic != ELogisticStorage.Demand) continue;
                }
                long energycost = Stationfullenergy.Value ? 0:Quantumenergy.Value;
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
                    if (count > sc.storage[storageindex].count)
                    {
                        int download = count - sc.storage[storageindex].count;
                        int[] downloaded = doitemfromStarsuper(itemid, download);
                        count = sc.storage[storageindex].count + downloaded[0];
                        inc = sc.storage[storageindex].inc + downloaded[1];
                        sc.storage[storageindex].count = 0;
                        sc.storage[storageindex].inc = 0;
                    }
                    else
                    {
                        inc = Math.Min(count * incAbility, sc.storage[storageindex].inc);
                        sc.storage[storageindex].count -= count;
                        sc.storage[storageindex].inc -= inc;
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
                    if (sc.storage[storageindex].count + count >= sc.storage[storageindex].max)
                    {
                        int upload = sc.storage[storageindex].count + count - sc.storage[storageindex].max;
                        int[] uploaded = doitemfromStarsuper(itemid, upload, inc, false);
                        count -= upload - uploaded[0];
                        inc = Math.Min(count * incAbility, inc);
                        sc.storage[storageindex].count = sc.storage[storageindex].max;
                        sc.storage[storageindex].inc += inc-uploaded[1];
                    }
                    else
                    {
                        sc.storage[storageindex].count += count;
                        sc.storage[storageindex].inc += inc;
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

        /// <summary>
        /// 配方、物品常规初始化
        /// </summary>
        public void RecipeCustomization()
        {
            if (first && LDB.items != null && LDB.recipes != null)
            {
                itemProtos = new ItemProto[LDB.items.dataArray.Length];
                RecipeList = new RecipeProto[LDB.recipes.dataArray.Length];
                Array.Copy(LDB.items.dataArray, itemProtos, itemProtos.Length);
                Array.Copy(LDB.recipes.dataArray, RecipeList, RecipeList.Length);

                first = false;
                ready = true;
            }
        }

        /// <summary>
        /// 加载存档时进行初始化操作
        /// </summary>
        public void FirstStartGame()
        {
            if (GameMain.instance != null)
            {
                if (!GameMain.instance.running)
                {
                    FinallyInit = false;
                    closeallcollider = false;
                    tempsails = new List<Tempsail>();
                    addStatDic = new Dictionary<int, Dictionary<int, long>>();
                    consumeStatDic = new Dictionary<int, Dictionary<int, long>>();
                    playcancelsolarbullet = false;
                    alwaysemissiontemp = false;
                }
                if (GameMain.instance.running && !FinallyInit)
                {
                    OrwatertypePlanetArray = new Dictionary<int, int>();
                    Beltsignal = new Dictionary<int, Dictionary<int, int>>();
                    Beltsignalnumberoutput = new Dictionary<int, Dictionary<int, int>>();
                    productmaxindex = 0;
                    SuperStation = new List<int>();
                    StarSuperStation = new List<int>();
                    warpstationqua = new float[200];
                    warpsuperstationqua = new float[200];
                    RandomEmissionEjectorSilo();
                    for (int i = 0; i < warpstationqua.Length; i++)
                        warpstationqua[i] = Time.time;
                    for (int i = 0; i < warpsuperstationqua.Length; i++)
                        warpsuperstationqua[i] = Time.time;
                    if (BeltSignalFunction.Value)
                    {
                        InitBeltSignalDiction();
                    }

                    foreach (KeyValuePair<int, int> wap in watertypePlanetArray)
                    {
                        PlanetData pd = GameMain.galaxy.PlanetById(wap.Key);
                        if (pd == null) continue;
                        if (!OrwatertypePlanetArray.ContainsKey(wap.Key))
                            OrwatertypePlanetArray.Add(wap.Key, pd.waterItemId);
                        pd.waterItemId = wap.Value;
                    }
                    FinallyInit = true;
                    playcancelsolarbullet = cancelsolarbullet.Value;
                    alwaysemissiontemp = alwaysemission.Value;
                    //更改配方相关
                    ChangeRecipe();
                    //更改物品相关
                    ChangeItemstack();
                }
            }

        }

        public void AfterGameStart()
        {
            if (ready && GameMain.history != null && FinallyInit)
            {
                StationComponentSet();
                PlayerDataInit();
                LockPlayerPackage();
                BluePrintpasteNoneed();
                if (player != null)
                {
                    ControlVein();
                    Sunlightset();
                    SetDronenocomsume();
                    MechaLogisticsMethod();
                    InfiniteAllThingInPackage();
                    DriftBuild();
                    InitPatch();
                    if (BuildNotime_bool.Value && GameMain.localPlanet?.factory != null)
                    {
                        if (GameMain.localPlanet.factory.prebuildCount > 0)
                        {
                            GameMain.localPlanet.factory.BeginFlattenTerrain();
                            for (int i = 1; i < GameMain.localPlanet.factory.prebuildCursor; i++)
                            {
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

        private void CancelAllToggle()
        {
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
            Station_miner_noconsume_bool.Value = false;
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
            CloseUIpanel.Value = false;
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
        }

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

        public void QuickKeyOpenWindow()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                DisplayingWindow = false;
                ui_MultiFunctionPanel?.SetActive(DisplayingWindow && CloseUIpanel.Value);
            }
            if (WindowQuickKey.Value.IsDown() && !ChangingQuickKey)
            {
                DisplayingWindow = !DisplayingWindow;
                if (ui_MultiFunctionPanel == null)
                {
                    ui_MultiFunctionPanel = UnityEngine.Object.Instantiate<GameObject>(MultiFunctionPanel, UIRoot.instance.overlayCanvas.transform);
                }
                ui_MultiFunctionPanel?.SetActive(DisplayingWindow && !CloseUIpanel.Value);
            }
            if (DisplayingWindow && Input.GetKey(KeyCode.LeftControl))
            {
                if (Input.GetKeyDown(KeyCode.UpArrow)) { scale.Value++; changescale = true; }
                if (Input.GetKeyDown(KeyCode.DownArrow)) { scale.Value--; changescale = true; }
                if (scale.Value <= 5) scale.Value = 5;
                if (scale.Value > 35) scale.Value = 35;
            }
            if (player != null && player.controller != null && !player.controller.actionBuild.active)
            {
                if (ItemList_bool.Value && Input.GetKeyDown(KeyCode.Tab))
                {
                    ItemDisplayingWindow = !ItemDisplayingWindow;
                }
                if (GameMain.localPlanet == null) ItemDisplayingWindow = false;
            }
            else
            {
                ItemDisplayingWindow = false;
            }
        }

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
        private void SetMaxGasStation()
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
                string t = "";
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

        public void MechaLogisticsMethod()
        {
            if (!Mechalogistics_bool.Value || Time.time - MechaLogisticsTime < 1) return;
            MechaLogisticsTime = Time.time;
            StorageComponent package = player.package;

            for(int i=0;i< player.deliveryPackage.grids.Length; i++)
            {
                var item=player.deliveryPackage.grids[i];
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
                    int need = Math.Min(item.requireCount - exsitcount,remain);
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

                                            int num = (int)((double)tc.fluidInc / (double)tc.fluidCount + 0.5) * temp;
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
                    player.packageUtility.AddItemToAllPackages(itemId, getcount,i, inc, out _);
                }
                if (item.recycleCount < exsitcount)
                {
                    int recyclenum = exsitcount-item.recycleCount;
                    player.packageUtility.TakeItemFromAllPackages(i,ref itemId,ref recyclenum, out int inc1);
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
                        if (countList[i] > int.MaxValue)
                        {
                            GameMain.data.statistics.production.factoryStatPool[GameMain.localPlanet.factory.index].AddProductionToTotalArray(readytocompute[i], int.MaxValue);
                            GameMain.data.statistics.production.factoryStatPool[GameMain.localPlanet.factory.index].AddConsumptionToTotalArray(readytocompute[i], int.MaxValue);
                            countList[i] -= int.MaxValue;
                        }
                        else
                        {
                            GameMain.data.statistics.production.factoryStatPool[GameMain.localPlanet.factory.index].AddProductionToTotalArray(readytocompute[i], (int)countList[i]);
                            GameMain.data.statistics.production.factoryStatPool[GameMain.localPlanet.factory.index].AddConsumptionToTotalArray(readytocompute[i], (int)countList[i]);
                            countList[i] = 0;
                        }
                    }
                }
                catch
                {

                }
            }
        }

        //初始化玩家数据
        public void PlayerDataInit()
        {
            if (refreshPlayerData || player == null || GameMain.mainPlayer != player)
            {
                GameHistoryData historyData = GameMain.history;
                refreshPlayerData = false;
                player = GameMain.mainPlayer;
                Mecha mecha = player.mecha;
                ChangeValueArray[0] = mecha.walkSpeed;
                ChangeValueArray[1] = historyData.techSpeed;
                ChangeValueArray[2] = mecha.droneSpeed;
                ChangeValueArray[3] = mecha.droneMovement;
                ChangeValueArray[4] = mecha.droneCount;
                ChangeValueArray[5] = mecha.maxSailSpeed / 1000;
                ChangeValueArray[6] = mecha.maxWarpSpeed / 480000;
                ChangeValueArray[7] = mecha.buildArea;
                ChangeValueArray[8] = (float)mecha.reactorPowerGen;
                ChangeValueArray[9] = historyData.logisticCourierSpeed > 0 ? historyData.logisticCourierSpeed : 6;
                ChangeValueArray[10] = historyData.logisticDroneSpeedScale > 0 ? historyData.logisticDroneSpeedScale : 3;
                ChangeValueArray[11] = historyData.logisticShipSpeedScale > 0 ? historyData.logisticShipSpeedScale : 5;
                ChangeValueArray[12] = historyData.logisticCourierCarries > 0 ? historyData.logisticDroneCarries : 3;
                ChangeValueArray[13] = historyData.logisticDroneCarries > 0 ? historyData.logisticDroneCarries : 100;
                ChangeValueArray[14] = historyData.logisticShipCarries > 0 ? historyData.logisticShipCarries : 100;
                ChangeValueArray[15] = historyData.miningSpeedScale;
                ChangeValueArray[16] = historyData.inserterStackCount;
                ChangeValueArray[17] = historyData.labLevel;
                ChangeValueArray[18] = historyData.storageLevel > 4 ? 4 : historyData.storageLevel;
                ChangeValueArray[19] = player.deliveryPackage.stackSizeMultiplier;
                packageitemlist = new List<int[]>();
                if (Mechalogistics_bool.Value && !player.deliveryPackage.unlocked)
                {
                    player.deliveryPackage.unlocked = true;
                    player.deliveryPackage.colCount = 1;
                    player.deliveryPackage.NotifySizeChange();
                }
            }

        }

        //设置滑动条的上下限和名称
        public void getLRstrValue(int i, out float left, out float right, out string str)
        {
            if (i == 0)
            {
                left = 1;
                right = 100;
                str = "走路速度";
                return;
            }
            if (i == 1)
            {
                left = 1;
                right = 10000;
                str = "研发速度";
                return;
            }
            if (i == 2)
            {
                left = 1;
                right = 100;
                str = "小飞机速度";
                return;
            }
            if (i == 3)
            {
                left = 1;
                right = 100;
                str = "小飞机任务点数";
                return;
            }
            if (i == 4)
            {
                left = 1;
                right = 400;
                str = "小飞机数量";
                return;
            }
            if (i == 5)
            {
                left = 1;
                right = 10;
                str = "最大航行速度";
                return;
            }
            if (i == 6)
            {
                left = 1;
                right = 100;
                str = "最大曲速";
                return;
            }
            if (i == 7)
            {
                left = 80;
                right = 400;
                str = "建造范围";
                return;
            }
            if (i == 8)
            {
                left = 1000000;
                right = 500000000;
                str = "核心功率";
                return;
            }
            if (i == 9)
            {
                left = 1;
                right = 100;
                str = "配送机速度";
                return;
            }
            if (i == 10)
            {
                left = 1;
                right = 100;
                str = "运输机速度";
                return;
            }
            if (i == 11)
            {
                left = 1;
                right = 100;
                str = "运输船速度";
                return;
            }
            if (i == 12)
            {
                left = 1;
                right = 10000;
                str = "配送机载量";
                return;
            }
            if (i == 13)
            {
                left = 1;
                right = 10000;
                str = "运输机载量";
                return;
            }
            if (i == 14)
            {
                left = 100;
                right = 100000;
                str = "运输船载量";
                return;
            }
            if (i == 15)
            {
                left = 1;
                right = 100;
                str = "采矿机速度倍率";
                return;
            }
            if (i == 16)
            {
                left = 1;
                right = 100;
                str = "极速分拣器数量";
                return;
            }
            if (i == 17)
            {
                left = 2;
                right = 100;
                str = "建筑堆叠高度";
                return;
            }
            if (i == 18)
            {
                left = 1;
                right = 20;
                str = "货物集装数量";
                return;
            }
            if (i == 19)
            {
                left = 1;
                right = 10000;
                str = "物流背包堆叠倍率";
                return;
            }
            left = 1;
            right = 100;
            str = "";
        }

        //当数据变动时调用相应id的操作
        public void OnChangeValue(float value, int valueId)
        {
            if (value != ChangeValueArray[valueId] && ChangePlayerbool)
            {
                ChangeValueArray[valueId] = value;
                if (valueId == 13)
                {
                    ChangeValueArray[valueId] = (int)(value / 50) * 50;
                }
                else if (valueId == 14)
                {
                    ChangeValueArray[valueId] = (int)(value / 500) * 500;
                }
                switch (valueId)
                {
                    case 0: SetWalkSpeed(value); break;
                    case 1: SetTechSpeed(value); break;
                    case 2: SetDroneSpeed(value); break;
                    case 3: SetDroneMoveMent(value); break;
                    case 4: SetDroneCount(value); break;
                    case 5: SetMaxsailSpeed(value); break;
                    case 6: SetMaxWarpSpeed(value); break;
                    case 7: SetBuildarea(value); break;
                    case 8: SetreactorPowerGen(value); break;
                    case 9: SetlogisticCourierSpeedScale(value); break;
                    case 10: SetlogisticDroneSpeedScale(value); break;
                    case 11: SetlogisticShipSpeedScale(value); break;
                    case 12: SetlogisticCourierCarries(value); break;
                    case 13: SetlogisticDroneCarries(value); break;
                    case 14: SetlogisticShipCarries(value); break;
                    case 15: SetminingSpeedScale(value); break;
                    case 16: SetInserterStack(value); break;
                    case 17: SetBuildStack(value); break;
                    case 18: SetStationPilerStack(value); break;
                    case 19: SetDeliveryPackageStackSize(value); break;
                }
            }

        }

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

        //星际物流站设置及星球矿机
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
                        if (sc != null && sc.storage != null)
                        {
                            if(!sc.isCollector && !sc.isVeinCollector)
                            {
                                if (StationSpray.Value)
                                {
                                    StationSprayInc(sc);
                                }
                                if (StationPowerGen.Value || (sc.name!=null && sc.name== "星球矿机"))
                                {
                                    StationPowerGeneration(sc,pdId);
                                }
                                if (sc.isStellar && Station_infiniteWarp_bool.Value)
                                    sc.warperCount = 50;
                            }
                            for (int i = 0; i < sc.storage.Length; i++)
                            {
                                int itemID = sc.storage[i].itemId;
                                int basemax = sc.isStellar ? 10000 : 5000;
                                int extramax = sc.isStellar ? GameMain.history.remoteStationExtraStorage : GameMain.history.localStationExtraStorage;
                                if (StationfullCount && itemID > 0)
                                {
                                    sc.storage[i].count = sc.storage[i].max;
                                }
                                if (StationStoExtra.Value >= 0)
                                {
                                    
                                    if (sc.storage[i].max == basemax + extramax)
                                    {
                                        sc.storage[i].max += StationStoExtra.Value * basemax;
                                    }
                                    if (RefreshStationStorage && sc.storage[i].max % basemax == 0 && sc.storage[i].max != 0)
                                    {
                                        sc.storage[i].max = basemax + extramax + StationStoExtra.Value * basemax;
                                    }
                                }
                            }
                            if (sc.name != null)
                            {
                                if (StationfullCount_bool.Value && sc.name== "星球无限供货机")
                                {
                                    for (int i = 0; i < sc.storage.Length; i++)
                                    {
                                        int itemID = sc.storage[i].itemId;
                                        if (itemID > 0)
                                        {
                                            sc.storage[i].count = sc.storage[i].max;
                                        }
                                    }
                                }
                                if (StationMiner.Value&& !sc.isVeinCollector && (sc.name == "Station_miner" || sc.name == "星球矿机"))
                                {
                                    if ((Time.time - StationMinerTime) > 1)
                                    {
                                        for (int i = 0; i < sc.storage.Length && sc.energy > 0; i++)
                                        {
                                            int itemID = sc.storage[i].itemId;
                                            if (itemID > 0 && sc.storage[i].count <= sc.storage[i].max)
                                            {
                                                int veinNumbers = GetNumberOfVein(itemID, pdId);
                                                int pointminenum = veinNumbers * Stationminenumber.Value;
                                                if (veinNumbers > 0 && pointminenum == 0) pointminenum = 1;
                                                else if (veinNumbers == 0 && itemID != GameMain.galaxy.PlanetById(pdId).waterItemId) continue;

                                                if (Stationfullenergy.Value || sc.energy - pointminenum * GameMain.history.miningSpeedScale * 5000 > 0)
                                                {
                                                    int minenum = StationMine(itemID, pointminenum, pdId);
                                                    if(minenum > 0)
                                                    {
                                                        AddStatInfo(pdId, itemID, minenum);
                                                        sc.AddItem(itemID, minenum, 0);
                                                        if (!Stationfullenergy.Value)
                                                            sc.energy -= minenum * 5000;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }

                                if (StationTrash.Value && (sc.name == "垃圾站" || sc.name == "Station_trash"))
                                {
                                    for (int i = 0; i < sc.storage.Length && sc.energy > 0; i++)
                                    {
                                        int itemID = sc.storage[i].itemId;
                                        if (itemID > 0)
                                        {
                                            int trashnum = sc.storage[i].count;
                                            if (sc.energy - trashnum * 10000 > 0)
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
                                }
                            }
                        }
                    }
                }
            }
            if ((Time.time - StationMinerTime) > 1)
            {
                StationMinerTime = Time.time;
            }
        }

        /// <summary>
        /// 获取目标星球目标矿脉数量
        /// </summary>
        /// <param name="itemid"></param>
        /// <param name="pdid"></param>
        /// <returns></returns>
        public int GetNumberOfVein(int itemid, int pdid)
        {
            int number = 0;
            EVeinType evt = LDB.veins.GetVeinTypeByItemId(itemid);// ItemIdtoVeintype(itemid);
            PlanetData pd = GameMain.galaxy.PlanetById(pdid);
            long[] veinAmounts = new long[64];
            pd.CalcVeinAmounts(ref veinAmounts,new HashSet<int>(), UIRoot.instance.uiGame.veinAmountDisplayFilter);
            if (evt == EVeinType.Oil)
            {
                int collectspeed = (int)(veinAmounts[7] * VeinData.oilSpeedMultiplier + 0.5);
                if (collectspeed > 1) return collectspeed;
            }
            if (pd == null || evt == EVeinType.None)
            {
                return 0;
            }
            foreach (VeinData vd in pd.factory.veinPool)
            {
                if (vd.type == evt)
                {
                    number++;
                }
            }
            return number;
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
                if(planetId==0) return;
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
                    consumeStatDic[planetId][itemId]+=itemNum;
                }
            }
            
        }

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
            Multifunctionpatch.InitPatch();
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
            if (pd == null || pd.type == EPlanetType.Gas || !Physics.Raycast(player.controller.mainCamera.ScreenPointToRay(Input.mousePosition), out RaycastHit raycastHit1, 800f, 8720, (QueryTriggerInteraction)2)) return;
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
            pd.factory.veinGroups[vd.groupIndex].pos = veinPool[vd.id].pos / (pd.realRadius + 2.5f); ;
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
            if (end.y > 193 || end.y < -193) return pointpos;
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

        public void Changewatertype()
        {
            foreach (ItemProto ip in itemProtos)
            {
                if (ip.name.Equals(watertype))
                {
                    if (GameMain.localPlanet != null)
                    {
                        int pdid = GameMain.localPlanet.id;
                        if (GameMain.localPlanet.waterItemId == ip.ID) break;
                        if (watertypePlanetArray.ContainsKey(pdid))
                        {
                            watertypePlanetArray[pdid] = ip.ID;
                            watertypePlanet.Value = "";
                            foreach (KeyValuePair<int, int> wap in watertypePlanetArray)
                            {
                                watertypePlanet.Value += wap.Key + "," + wap.Value + ";";
                            }
                        }
                        else
                        {
                            if (!OrwatertypePlanetArray.ContainsKey(pdid))
                                OrwatertypePlanetArray.Add(pdid, GameMain.localPlanet.waterItemId);
                            watertypePlanetArray.Add(pdid, ip.ID);
                            watertypePlanet.Value += pdid + "," + ip.ID + ";";
                        }
                        GameMain.localPlanet.waterItemId = ip.ID;
                        break;
                    }
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
                    ui_MultiFunctionPanel.SetActive(true);
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
                    ui_MultiFunctionPanel.SetActive(DisplayingWindow && CloseUIpanel.Value);
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

        public int StationMine(int itemid, int minenumber, int pdid)
        {
            int getmine = 0;
            PlanetData pd = GameMain.galaxy.PlanetById(pdid);
            if (pd.waterItemId == itemid)
            {
                return (int)(30 * GameMain.history.miningSpeedScale * Stationminenumber.Value);
            }
            if (Station_miner_noconsume_bool.Value)
                return itemid != 1007 ? (int)(minenumber * GameMain.history.miningSpeedScale / 2) : (int)(minenumber * GameMain.history.miningSpeedScale);
            if (LDB.veins.GetVeinTypeByItemId(itemid) == EVeinType.None || pd == null)
            {
                return 0;
            }
            int neednumber = itemid != 1007 ? (int)(minenumber * GameMain.history.miningCostRate / 2) : (int)(minenumber * GameMain.history.miningCostRate);
            if (minenumber > 0 && neednumber == 0) neednumber = 1;
            foreach (VeinData i in pd.factory.veinPool)
            {
                if (i.type == LDB.veins.GetVeinTypeByItemId(itemid))
                {
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
            }
            return itemid != 1007 ? (int)(getmine * GameMain.history.miningSpeedScale / 2) : (int)(getmine * GameMain.history.miningSpeedScale);
        }

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

        public void Clearpackage()
        {
            if (player == null) return;
            for (int i = 0; i < player.package.grids.Length; i++)
            {
                player.package.grids[i].count = 0;
            }
            player.package.NotifyStorageChange();
        }

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
                packageitemlist = new List<int[]>();
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

        public void Operatepackagesize(int rownum,int colnum,bool add=true)
        {
            if (player == null) return;
            if (add)
            {
                if (rownum > 0)
                {
                    player.package.SetSize(player.package.size / 10 * 10 + 10*rownum);
                }
                if (colnum>0)
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
                if (colnum > 0 && player.deliveryPackage.colCount>colnum)
                {
                    player.deliveryPackage.unlocked = true;
                    player.deliveryPackage.colCount -= colnum;
                    player.deliveryPackage.NotifySizeChange();
                }
            }
        }

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
                                    Debug.Log(techProto.Name);
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
                                Debug.Log(techProto.Name);
                            }
                        }
                        GameMain.history.UnlockTech(techProto.ID);
                    }

                }
            }
        }

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
            bool needMulti = LDB.recipes.dataArray[0].ItemCounts[0] == 1;
            for (int i = 0; i < LDB.recipes.dataArray.Length; i++)
            {
                if (allhandcraft.Value)
                {
                    LDB.recipes.dataArray[i].Handcraft = true;
                }

                if (quickproduce.Value)
                {
                    LDB.recipes.dataArray[i].TimeSpend = 1;
                }
                if (MULTIPELSMELT.Value!=1 && needMulti && LDB.recipes.dataArray[i].Type == ERecipeType.Smelt)
                {
                    for (int j = 0; j < LDB.recipes.dataArray[i].ItemCounts.Length; ++j)
                        LDB.recipes.dataArray[i].ItemCounts[j] *= MULTIPELSMELT.Value;
                    for (int j = 0; j < LDB.recipes.dataArray[i].ResultCounts.Length; ++j)
                        LDB.recipes.dataArray[i].ResultCounts[j] *= MULTIPELSMELT.Value;
                }
            }

            //配方信息查询
            foreach (RecipeProto rp in RecipeList)
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

        /// <summary>
        /// 物品相关修改
        /// </summary>
        public void ChangeItemstack()
        {
            ItemProto[] dataArray = LDB.items.dataArray;
            for (int j = 0; j < dataArray.Length; j++)
            {
                StorageComponent.itemStackCount[dataArray[j].ID] = dataArray[j].StackSize * StackMultiple.Value;
            }
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

        public void InfiniteAllThingInPackage()
        {
            if (!Infinitething.Value || Time.time - InfiniteAllThingTime<1) return;
            InfiniteAllThingTime = Time.time;
            if (player.package.grids.Length < itemProtos.Length)
            {
                player.package.SetSize((itemProtos.Length / 10 + 1) * 10);
            }
            StorageComponent.GRID[] grids = player.package.grids;
            int i = 0;
            foreach (ItemProto ip in itemProtos)
            {
                grids[i].itemId = ip.ID;
                grids[i].count = StorageComponent.itemStackCount[ip.ID];
                grids[i].stackSize = StorageComponent.itemStackCount[ip.ID];
                i++;
                if (i == grids.Length) break;
            }
            player.package.NotifyStorageChange();
        }

        public void Sunlightset()
        {
            if (GameMain.universeSimulator == null || GameMain.universeSimulator.LocalStarSimulator() == null || GameMain.universeSimulator.LocalStarSimulator().sunLight == null) return;
            if (sunlight_bool.Value)
            {
                if (SunLight == null)
                    SunLight = GameMain.universeSimulator.LocalStarSimulator().sunLight;
                if (SunLight != null)
                    SunLight.transform.rotation = Quaternion.LookRotation(-player.transform.up);
                lighton = false;
            }
            else if (!sunlight_bool.Value && SunLight != null && !lighton)
            {
                SunLight.transform.localEulerAngles = new Vector3(0.0f, 180f);
                lighton = true;
            }
        }

        #region 设置属性

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
        public void SetWalkSpeed(float value)
        {
            if (player != null && player.mecha != null)
            {
                GameMain.mainPlayer.mecha.walkSpeed = value;
            }
        }
        public void SetTechSpeed(float value)
        {
            if (GameMain.history != null)
            {
                GameMain.history.techSpeed = (int)value;
            }
        }
        public void SetDroneSpeed(float value)
        {
            if (player != null && player.mecha != null)
            {
                GameMain.mainPlayer.mecha.droneSpeed = value;
            }
        }
        public void SetDroneMoveMent(float value)
        {
            if (player != null && player.mecha != null)
            {
                player.mecha.droneMovement = (int)value;
            }
        }
        public void SetDroneCount(float value)
        {
            if (player != null && player.mecha != null)
            {
                player.mecha.droneCount = (int)value;
            }
        }
        public void SetMaxsailSpeed(float value)
        {
            if (player != null && player.mecha != null)
            {
                player.mecha.maxSailSpeed = (int)value * 1000;
            }
        }
        public void SetMaxWarpSpeed(float value)
        {
            if (player != null && player.mecha != null)
            {
                player.mecha.maxWarpSpeed = (int)value * 1000 * 480;
            }
        }
        public void SetBuildarea(float value)
        {
            if (player != null && player.mecha != null)
            {
                player.mecha.buildArea = value;
            }
        }
        public void SetreactorPowerGen(float value)
        {
            if (player != null && player.mecha != null)
            {
                player.mecha.reactorPowerGen = value;
            }
        }
        public void SetlogisticCourierSpeedScale(float value)
        {
            if (GameMain.history != null)
            {
                GameMain.history.logisticCourierSpeed = (int)value;
            }
        }
        public void SetlogisticDroneSpeedScale(float value)
        {
            if (GameMain.history != null)
            {
                GameMain.history.logisticDroneSpeedScale = value;
            }
        }
        public void SetlogisticShipSpeedScale(float value)
        {
            if (GameMain.history != null)
            {
                GameMain.history.logisticShipSpeedScale = value;
            }
        }
        public void SetlogisticCourierCarries(float value)
        {
            if (GameMain.history != null)
            {
                GameMain.history.logisticCourierCarries = (int)value;
            }
        }

        public void SetlogisticDroneCarries(float value)
        {
            if (GameMain.history != null)
            {
                GameMain.history.logisticDroneCarries = (int)value;
            }
        }
        public void SetlogisticShipCarries(float value)
        {
            if (GameMain.history != null)
            {
                GameMain.history.logisticShipCarries = (int)value;
            }
        }
        public void SetminingSpeedScale(float value)
        {
            if (GameMain.history != null)
            {
                GameMain.history.miningSpeedScale = value;
            }
        }
        public void SetInserterStack(float value)
        {
            if (player == null) return;
            GameMain.history.inserterStackCount = (int)value;
        }
        public void SetBuildStack(float value)
        {
            if (player == null) return;
            GameMain.history.storageLevel = (int)value;
            GameMain.history.labLevel = (int)value;
        }
        public void SetStationPilerStack(float value)
        {
            if (player == null) return;
            GameMain.history.stationPilerLevel = (int)value;
        }

        public void SetDeliveryPackageStackSize(float value)
        {
            if (player == null) return;
            player.deliveryPackage.stackSizeMultiplier = (int)value;
        }
        #endregion

        public static void StationPowerGeneration(StationComponent sc,int planetID=0)
        {
            if(sc == null || sc.storage == null || sc.energy >= sc.energyMax -1000000) return;
            planetID = sc.planetId > 0 ? sc.planetId : planetID;
            if (planetID == 0) return;
            int lastIndex = sc.storage.Length - 1;
            if (sc.storage[lastIndex].itemId > 0 && sc.storage[lastIndex].count > 0)
            {
                ItemProto ip = LDB.items.Select(sc.storage[lastIndex].itemId);
                if (ip.HeatValue > 0 && sc.energy+ip.HeatValue<sc.energyMax)
                {
                    int num = Math.Min(sc.storage[lastIndex].count, (int)((sc.energyMax - sc.energy) / ip.HeatValue));
                    sc.energy += num * ip.HeatValue;
                    sc.storage[lastIndex].count -= num;
                    if (num > 0)
                    {
                        AddStatInfo(planetID, ip.ID, num);
                    }
                }
            }
        }

        public static void StationAssembler(StationComponent sc)
        {
            if (sc == null || sc.storage==null) return;
            int itemId = sc.storage[0].itemId;
            if (itemId > 0)
            {
                var item = LDB.items.Select(itemId);
                if (item.recipes.Count > 0)
                {
                    foreach (var rp in item.recipes)
                    {
                        if(rp.Type == ERecipeType.Smelt && rp.Items.Length==1)
                        {
                            float mult = rp.ItemCounts[0] / rp.ResultCounts[0];
                        }
                    }
                }
            }
        }

        public static void StationSprayInc(StationComponent sc)
        {
            for (int i = 0; i < sc.storage.Length; i++)
            {
                if (sc.storage[i].itemId == 1143 && sc.storage[i].count > 0 )
                {
                    if(sc.storage[i].localLogic != ELogisticStorage.None)
                    {
                        int incAbility = Maxproliferator.Value ? 10 : 4;
                        for (int j = 0; j < sc.storage.Length; j++)
                        {
                            if (j != i && sc.storage[j].itemId > 0 && sc.storage[j].count > 0 && sc.storage[i].count > 0)
                            {
                                int needinc = sc.storage[j].count * incAbility - sc.storage[j].inc;
                                int needNumber = Math.Min((int)Math.Ceiling(needinc / 296.0), sc.storage[i].count);
                                sc.storage[j].inc += sc.storage[i].count >= needNumber ? needinc : sc.storage[i].count * 296;
                                sc.storage[i].count -= needNumber;
                                sc.storage[i].inc = Math.Min(sc.storage[i].count * incAbility, sc.storage[i].inc);
                            }
                        }
                    }
                    return;
                }
            }
        }

        /// <summary>
        /// 设置快捷键
        /// </summary>
        public void setQuickKey()
        {
            bool left = true;
            int[] result = new int[2];
            int num = 0;
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

        /// <summary>
        /// 数量单位转化
        /// </summary>
        /// <param name="num"></param>
        /// <param name="unit"></param>
        /// <returns></returns>
        public string TGMKinttostring(double num, string unit)
        {
            double tempcoreEnergyCap = num;
            int t = 0;
            if (num < 1000)
                return num + unit;
            while (tempcoreEnergyCap >= 1000)
            {
                t += 1;
                tempcoreEnergyCap /= 1000;
            }
            string coreEnergyCap = string.Format("{0:N2}", tempcoreEnergyCap);
            if (t == 0) return coreEnergyCap + "" + unit;
            if (t == 1) return coreEnergyCap + "K" + unit;
            if (t == 2) return coreEnergyCap + "M" + unit;
            if (t == 3) return coreEnergyCap + "G" + unit;
            if (t == 4) return coreEnergyCap + "T" + unit;

            return "";
        }


    }
}