using BepInEx;
using BepInEx.Configuration;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.EventSystems;
using static Multfunction_mod.Constant;

namespace Multfunction_mod
{
    [BepInPlugin(GUID, NAME, VERSION)]
    [BepInProcess(GAME_PROCESS)]
    public class Multifunction : BaseUnityPlugin
    {
        public const string GUID = "cn.blacksnipe.dsp.Multfuntion_mod";
        public const string NAME = "Multfuntion_mod";
        public const string VERSION = "2.3.1";
        public const string GAME_PROCESS = "DSPGAME.exe";
        #region 临时变量
        public Light SunLight;
        public Texture2D mytexture;
        public GameObject MultiFunctionPanel;
        public GUIStyle style = new GUIStyle();
        public static Player player;
        public static GameObject ui_MultiFunctionPanel;
        public List<string> BPPathList = new List<string>();
        public List<string> BPFileNameList = new List<string>();
        public List<int[]> packageitemlist = new List<int[]>();
        public static List<StationComponent> SuperStation = new List<StationComponent>();
        public static List<StationComponent> StarSuperStation = new List<StationComponent>();
        public static List<Tempsail> tempsails = new List<Tempsail>();
        public static List<ItemProto> itemProtos = new List<ItemProto>();
        public static List<RecipeProto> RecipeList = new List<RecipeProto>();
        public static Dictionary<int, int> tmp_levelChanges;
        public VeinData pointveindata;
        public KeyboardShortcut tempShowWindow;
        public int maxheight;
        public int maxwidth;
        public int heightdis;
        public int togglesize;
        public int oillowerlimit;
        public int whichpannel = 1;
        public int veintype = 1;
        public int[] mechalogistics;
        public int[] mechalogisticsneed;
        public static int DriftBuildingLevel;
        public static int productmaxindex;
        public float MainWindow_x;
        public float MainWindow_y;
        public float MainWindow_x_move;
        public float MainWindow_y_move;

        public float tempx1;
        public float tempy1;
        public float MechaLogisticsTime;
        public float playtrashtime;
        public float startsuperstationtime;
        public float StationMinerTime;
        public float playwithtrashtimemult = 1;
        public static float buildheight = 1;
        public float[] warpstationqua;
        public float[] warpsuperstationqua;
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
        public bool playwithtrash;
        public bool trashplaying;
        public bool dropdownbutton;
        public bool addveinbool;
        public bool buildnotimecolddown;
        public bool SandBoxMode;
        public bool blueprintPasteToolActive;
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
        public string stackmultiple1 = "";
        public string multipelsmelt1 = "";
        public string watertype = "";
        public static Queue<int[]> addStatQueue = new Queue<int[]>();
        public static Dictionary<int, int> watertypePlanetArray = new Dictionary<int, int>();
        public static Dictionary<int, int> OrwatertypePlanetArray = new Dictionary<int, int>();
        public static Dictionary<int, int[]> StarSuperStationItemidstore = new Dictionary<int, int[]>();
        public static Dictionary<int, Dictionary<int, int[]>> PlanetSuperStationItemidstore = new Dictionary<int, Dictionary<int, int[]>>();
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
        public static ConfigEntry<Boolean> NotTidyVein;
        public static ConfigEntry<Boolean> changexveinspos;
        public static ConfigEntry<Boolean> StationMaxproliferator;
        public static ConfigEntry<Boolean> Mechalogistics_bool;
        public static ConfigEntry<Boolean> MechalogStoragerecycle_bool;
        public static ConfigEntry<Boolean> MechalogStorageprovide_bool;
        public static ConfigEntry<Boolean> MechalogStationrecycle_bool;
        public static ConfigEntry<Boolean> MechalogStationprovide_bool;
        public static ConfigEntry<Boolean> Infinitestoragetank;
        public static ConfigEntry<Boolean> Quantumtransportstation_bool;
        public static ConfigEntry<Boolean> Quantumtransportminer_bool;
        public static ConfigEntry<Boolean> Quantumtransportsilo_bool;
        public static ConfigEntry<Boolean> Quantumtransportlab_bool;
        public static ConfigEntry<Boolean> Quantumtransportpower_bool;
        public static ConfigEntry<Boolean> Quantumtransportassemble_bool;
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
        public static ConfigEntry<Boolean> changeveinposbool;
        public static ConfigEntry<Boolean> MechalogisticsPlanet_bool;
        public static ConfigEntry<Boolean> StationMiner;
        public static ConfigEntry<Boolean> autochangestationname;
        public static ConfigEntry<Boolean> changeveingroupposbool;
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
        public static ConfigEntry<Boolean> QuantumStarstationmax;
        public static ConfigEntry<Boolean> getallVein_bool;
        public static ConfigEntry<Boolean> Buildingnoconsume;
        public static ConfigEntry<Boolean> Stationfullenergy;
        public static ConfigEntry<Boolean> BeltSignalFunction;
        public static ConfigEntry<Boolean> Tankcontentall;
        public static ConfigEntry<Boolean> ArchitectMode;
        public static ConfigEntry<Boolean> quickabsorbsolar;
        public static ConfigEntry<Boolean> cancelsolarbullet;
        public static ConfigEntry<Boolean> alwaysemission;
        public static ConfigEntry<Boolean> CloseUIpanel;
        #endregion

        void Start()
        {
            Multifunctionpatch.patchallmethod();

            AssetBundle assetBundle = AssetBundle.LoadFromStream(Assembly.GetExecutingAssembly().GetManifestResourceStream("Multifunction_mod.multifunctionpanel"));
            MultiFunctionPanel = assetBundle.LoadAsset<GameObject>("MultiFunctionPanel");
            preparedraw();
            startsuperstationtime = Time.time;
            StationMinerTime = Time.time;

            MainWindow_x_move = 710;
            MainWindow_y_move = 390;
            tempx1 = MainWindow_x;
            tempy1 = MainWindow_y;

            //初始化配置
            {
                Quantumtransport_bool = Config.Bind("量子传输站", "Quantumtransport_bool", false);
                Quantumtransportbuild_bool = Config.Bind("星球级材料供应", "Quantumtransportbuild_bool", true);
                Quantumtransportstation_bool = Config.Bind("星球级物流站材料供应", "Quantumtransportstation_bool", true);
                Quantumtransportminer_bool = Config.Bind("星球级矿机材料供应", "Quantumtransportminer_bool", true);
                Quantumtransportsilo_bool = Config.Bind("星球级发射井弹射器材料供应", "Quantumtransportsilo_bool", true);
                Quantumtransportlab_bool = Config.Bind("星球级研究站材料供应", "Quantumtransportlab_bool", true);
                Quantumtransportpower_bool = Config.Bind("星球级电力设备材料供应", "Quantumtransportpower_bool", true);
                Quantumtransportassemble_bool = Config.Bind("星球级组装机材料供应", "Quantumtransportassemble_bool", true);
                Quantumtransportstarwarp_bool = Config.Bind("星系级翘曲全面供应", "Quantumtransportstarwarp_bool", false);
                Quantumtransportpdwarp_bool = Config.Bind("星球级翘曲全面供应", "Quantumtransportpdwarp_bool", false);
                scale = Config.Bind("大小适配", "scale", 16);


                ArchitectMode = Config.Bind("建筑师模式", "ArchitectMode", false);
                Quantumenergy = Config.Bind("量子耗能", "Quantumenergy", 1000000);
                StationfullCount_bool = Config.Bind("星球无限供货机", "StationfullCount_bool", false);
                InfineteStarPower = Config.Bind("人造卫星无限能源", "InfineteStarPower", false);
                Infinitestoragetank = Config.Bind("无限储液站", "Infinitestoragetank", false);
                Tankcontentall = Config.Bind("储液站任意存", "Tankcontentall", false);
                allhandcraft = Config.Bind("全部手搓", "allhandcraft", false);
                quickproduce = Config.Bind("快速生产", "quickproduce", false);
                noneedwarp = Config.Bind("无翘曲器曲速", "noneedwarp", false);
                watertypePlanet = Config.Bind("星球海洋类型", "watertypePlanet", "");

                Mechalogneed = Config.Bind("机甲物流需求情况", "Mechalogneed", "");
                changeveinsposx = Config.Bind("切割矿脉数量", "changeveinsposx", 3);
                veinlines = Config.Bind("矿物行数", "veinlines", 3);
                changexveinspos = Config.Bind("切割矿脉", "changexveinspos", false);
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
                Infiniteplayerpower = Config.Bind("无限机甲能量", "Infiniteplayerpower", false);
                deleteveinbool = Config.Bind("删除矿物", "deleteveinbool", false);
                StationMiner = Config.Bind("星球矿机", "stationmineropen", false);
                StationTrash = Config.Bind("星球垃圾箱", "stationtrashopen", false);
                autochangestationname = Config.Bind("自动改名", "autochangestationname", false);
                changeveinposbool = Config.Bind("改变单矿位置", "changeveinposbool", false);
                changeveingroupposbool = Config.Bind("改变矿堆位置", "changeveingroupposbool", false);
                getallVein_bool = Config.Bind("获取所有矿", "getallVein_bool", false);
                Buildingnoconsume = Config.Bind("全设备不耗电", "Buildingnoconsume", false);
                Stationfullenergy = Config.Bind("物流站永久满电", "Stationfullenergy", false);

                build_gascol_noequator = Config.Bind("采集器无视赤道", "build_gascol_noequator", false);
                lockpackage_bool = Config.Bind("锁定背包", "lockpackage_bool", false);
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
                StationStoExtra = Config.Bind("运输站额外储量", "StationStoExtra", 0);
                StackMultiple = Config.Bind("堆叠倍数", "StackMultiple", 1);
                Stationminenumber = Config.Bind("星球矿机速率", "Stationminenumber", 1);
                MULTIPELSMELT = Config.Bind("冶炼倍数", "mutiplesmelt", 1);
                Buildmaxlen = Config.Bind("建筑数量最大值", "Buildmaxlen", 15);
                starquamaxpowerpertick = Config.Bind("实时修改星球量子充电功率", "starquamaxpowerpertick", 60f);
                planetquamaxpowerpertick = Config.Bind("实时修改星系量子充电功率", "planetquamaxpowerpertick", 60f);
                QuantumStarstationmax = Config.Bind("星系量子存储有上限", "QuantumStarstationmax", false);
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

            InvokeRepeating("autocleanTrash", 1, 30);
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
            MultifunctionTranslate.regallTranslate();
        }

        void Update()
        {
            maxheight = Screen.height;
            maxwidth = Screen.width;
            ChangeQuickKeyMethod();
            RecipeCustomization();
            RestoreWaterMethod();
            FirstStartGame();
            QuickKeyOpenWindow();
            AfterGameStart();
            if (Input.GetKey(KeyCode.F10) && Input.GetKeyDown(KeyCode.LeftShift))
            {
                temp = !temp;
                //GameSave.LoadCurrentGameInResource(18);
            }
            if (player != null && player.controller != null && player.controller.cmd.type == ECommand.Build && player.controller.actionBuild != null)
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
            if (Quantumtransport_bool.Value && Time.time - startsuperstationtime < 1)
            {
                startsuperstationtime = Time.time;
                takeitemfromstarsuperstation();
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
            for (int i = 0; i < itemProtos.Count; i++)
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
                GUILayout.BeginArea(new Rect(0, 0, heightdis * 25, heightdis));
                {
                    GUILayout.BeginHorizontal();
                    if (GUILayout.Button("人物".getTranslate(), new[] { GUILayout.Height(heightdis), GUILayout.Width(heightdis * 4) })) { whichpannel = 1; }
                    if (GUILayout.Button("建筑".getTranslate(), new[] { GUILayout.Height(heightdis), GUILayout.Width(heightdis * 4) })) { whichpannel = 2; }
                    if (GUILayout.Button("星球".getTranslate(), new[] { GUILayout.Height(heightdis), GUILayout.Width(heightdis * 4) })) { whichpannel = 3; }
                    if (GUILayout.Button("戴森球".getTranslate(), new[] { GUILayout.Height(heightdis), GUILayout.Width(heightdis * 4) })) { whichpannel = 4; }
                    if (GUILayout.Button("其它功能".getTranslate(), new[] { GUILayout.Height(heightdis), GUILayout.Width(heightdis * 4) })) { whichpannel = 5; }
                    if (GUILayout.Button("机甲物流".getTranslate(), new[] { GUILayout.Height(heightdis), GUILayout.Width(heightdis * 4) })) { whichpannel = 6; }
                    GUILayout.EndHorizontal();
                }
                GUILayout.EndArea();

                GUILayout.BeginArea(new Rect(0, heightdis, MainWindow_width.Value, MainWindow_height.Value));
                if (whichpannel == 1) PlayerPannel();
                if (whichpannel == 2) BuildPannel();
                if (whichpannel == 3) PlanetPannel();
                if (whichpannel == 4) DysonPannel();
                if (whichpannel == 5) OtherPannel();
                if (whichpannel == 6) LogisticsPannel();
                GUILayout.EndArea();
            }


            GUILayout.EndArea();

        }

        public void PlayerPannel()
        {
            bool english = Localization.language != Language.zhCN;
            GUILayout.BeginArea(new Rect(0, 10, 40 + (english ? heightdis * 31 : heightdis * 27), 20 * heightdis));
            int tempwidth;
            {
                GUILayout.BeginArea(new Rect(0, 0, 35 + (english ? heightdis * 17 : heightdis * 13), 20 * heightdis));
                for (int i = 0; i < 20 && player != null; i++)
                {
                    float left, right;
                    string str;
                    getLRstrValue(i, out left, out right, out str);
                    float temp = GUI.HorizontalSlider(new Rect(10 + (english ? heightdis * 8 : heightdis * 4), heightdis * i + 5, heightdis * 6, heightdis), ChangeValueArray[i], left, right);
                    string result = "";
                    if (i == 11 && player.mecha != null) result += TGMKinttostring(player.mecha.reactorPowerGen, "W");
                    else if (i == 0 || i == 16) result = string.Format("{0:N2}", temp);
                    else result = (int)temp + "";
                    GUI.Label(new Rect(10, heightdis * i, english ? heightdis * 8 : heightdis * 4, heightdis), str.getTranslate() + ":");
                    if (temp == ChangeValueArray[i])
                    {
                        if ((i == 0 || i == 16) && !float.TryParse(Regex.Replace(GUI.TextField(new Rect(30 + (english ? heightdis * 14 : heightdis * 10), heightdis * i, heightdis * 3, heightdis), result), @"^[^0-9]+(.[^0-9]{2})?$", ""), out temp))
                        {
                            temp = ChangeValueArray[i];
                        }
                        else if (i != 0 && i != 11 && i != 16 && !float.TryParse(Regex.Replace(GUI.TextField(new Rect(30 + (english ? heightdis * 14 : heightdis * 10), heightdis * i, heightdis * 3, heightdis), result), @"^[^0-9]", ""), out temp))
                        {
                            temp = ChangeValueArray[i];
                        }
                        else if (i == 11)
                        {
                            GUI.Label(new Rect(30 + (english ? heightdis * 14 : heightdis * 10), heightdis * i, heightdis * 3, heightdis), result);
                        }
                    }
                    if (temp < left) temp = left;
                    if (temp > right) temp = right;

                    OnChangeValue(temp, i);
                }
                GUILayout.EndArea();
            }
            tempwidth = english ? 40 + heightdis * 17 : 40 + heightdis * 13;
            {
                GUILayout.BeginArea(new Rect(tempwidth, 0, heightdis * 8, 23 * heightdis));
                string[] tempstr = new string[12] { "小飞机不耗能", "无需翘曲器曲速飞行", "无限物品", "锁定背包", "无限机甲能量", "不往背包放东西", "建筑秒完成", "蓝图建造无需材料", "科技点击解锁", "物品列表(Tab)", "建筑师模式", "9999999元数据" };

                for (int i = 0; i < tempstr.Length; i++)
                {
                    bool tempvalue = false;
                    switch (i)
                    {
                        case 0: tempvalue = DroneNoenergy_bool.Value; break;
                        case 1: tempvalue = noneedwarp.Value; break;
                        case 2: tempvalue = Infinitething.Value; break;
                        case 3: tempvalue = lockpackage_bool.Value; break;
                        case 4: tempvalue = Infiniteplayerpower.Value; break;
                        case 5: tempvalue = dismantle_but_nobuild.Value; break;
                        case 6: tempvalue = BuildNotime_bool.Value; break;
                        case 7: tempvalue = blueprintpastenoneed_bool.Value; break;
                        case 8: tempvalue = unlockpointtech; break;
                        case 9: tempvalue = ItemList_bool.Value; break;
                        case 10: tempvalue = ArchitectMode.Value; break;
                        case 11: tempvalue = Property9999999; break;
                    }
                    tempvalue = GUI.Toggle(new Rect(0, heightdis * i, heightdis * 12, heightdis), tempvalue, tempstr[i].getTranslate());
                    switch (i)
                    {
                        case 0: DroneNoenergy_bool.Value = tempvalue; break;
                        case 1: noneedwarp.Value = tempvalue; break;
                        case 2: Infinitething.Value = tempvalue; break;
                        case 3: lockpackage_bool.Value = tempvalue; break;
                        case 4: Infiniteplayerpower.Value = tempvalue; break;
                        case 5: dismantle_but_nobuild.Value = tempvalue; break;
                        case 6: BuildNotime_bool.Value = tempvalue; break;
                        case 7: blueprintpastenoneed_bool.Value = tempvalue; break;
                        case 8: unlockpointtech = tempvalue; break;
                        case 9: ItemList_bool.Value = tempvalue; break;
                        case 10: ArchitectMode.Value = tempvalue; break;
                        case 11: Property9999999 = tempvalue; break;
                    }
                }
                tempstr = new string[7] { ChangePlayerbool ? "停止修改" : "启动修改", "清空背包", "初始化玩家", "增加背包大小", "减少背包大小", "解锁全部科技", "回退无穷科技" };
                for (int i = 0; i < tempstr.Length; i++)
                {
                    if (GUI.Button(new Rect(0, heightdis * (13 + i), english && i == 7 ? heightdis * 10 : (i == 7 ? heightdis * 7 : heightdis * 6), heightdis), tempstr[i].getTranslate()))
                    {
                        switch (i)
                        {
                            case 0: ChangePlayerbool = !ChangePlayerbool; break;
                            case 1: Clearpackage(); break;
                            case 2: InitPlayer(); break;
                            case 3: Addpackagesize(); break;
                            case 4: Reducepackagesize(); break;
                            case 5: UnlockallTech(); break;
                            case 6: lockTech(); break;
                        }
                    }
                }

                GUILayout.EndArea();


            }

            GUILayout.EndArea();


        }

        public void BuildPannel()
        {
            bool english = Localization.language != Language.zhCN;
            {
                GUILayout.BeginArea(new Rect(0, 10, english ? heightdis * 13 + 10 : heightdis * 10 + 10, heightdis * 25));

                Buildmaxlen.Value = (int)GUI.HorizontalSlider(new Rect(0, 5, heightdis * 5, heightdis), Buildmaxlen.Value, 15, 100);
                GUI.Label(new Rect(10 + heightdis * 5, 0, heightdis * 8, heightdis), Buildmaxlen.Value + " " + "建筑数量最大值".getTranslate(), style);
                StationStoExtra.Value = (int)GUI.HorizontalSlider(new Rect(0, 5 + heightdis, heightdis * 5, heightdis), StationStoExtra.Value, 0, 100);
                GUI.Label(new Rect(10 + heightdis * 5, heightdis, heightdis * 8, heightdis), StationStoExtra.Value + " " + "运输站存储倍率".getTranslate(), style);
                string[] tempstr = new string[19] { "实时更改全部运输站存储倍率", "无限储液站", "星球无限供货机", "物流站要啥有啥", "无需赤道造采集器", "强行近距离建造物流站", "星际运输站无限曲速", "人造恒星无限能源", "极速轨道弹射器(慎用)", "极速垂直发射井(慎用)", "星球电网(人造恒星)", "覆盖全球", "超长连接", "新建设备不耗电", "物流站永久满电", "物流站无限增产", "传送带信号功能", "储液站任意存", "建筑抬升" };

                for (int i = 0; i < tempstr.Length; i++)
                {
                    bool tempvalue = false;

                    switch (i)
                    {
                        case 0: tempvalue = RefreshStationStorage; break;
                        case 1: tempvalue = Infinitestoragetank.Value; break;
                        case 2: tempvalue = StationfullCount_bool.Value; break;
                        case 3: tempvalue = StationfullCount; break;
                        case 4: tempvalue = build_gascol_noequator.Value; break;
                        case 5: tempvalue = build_tooclose_bool.Value; break;
                        case 6: tempvalue = Station_infiniteWarp_bool.Value; break;
                        case 7: tempvalue = InfineteStarPower.Value; break;
                        case 8: tempvalue = quickEjector; break;
                        case 9: tempvalue = quicksilo; break;
                        case 10: tempvalue = PlanetPower_bool.Value; break;
                        case 11: tempvalue = PlanetPower_bool.Value; break;
                        case 12: tempvalue = farconnectdistance; break;
                        case 13: tempvalue = Buildingnoconsume.Value; break;
                        case 14: tempvalue = Stationfullenergy.Value; break;
                        case 15: tempvalue = StationMaxproliferator.Value; break;
                        case 16: tempvalue = BeltSignalFunction.Value; break;
                        case 17: tempvalue = Tankcontentall.Value; break;
                        case 18: tempvalue = DriftBuildings; break;
                    }
                    tempvalue = GUI.Toggle(new Rect(i >= 11 && i <= 12 ? 20 : 0, heightdis * (i + 2), heightdis * 13, heightdis), tempvalue, tempstr[i].getTranslate());
                    switch (i)
                    {
                        case 0: RefreshStationStorage = tempvalue; break;
                        case 1: Infinitestoragetank.Value = tempvalue; break;
                        case 2: StationfullCount_bool.Value = tempvalue; break;
                        case 3: StationfullCount = tempvalue; break;
                        case 4: build_gascol_noequator.Value = tempvalue; break;
                        case 5: build_tooclose_bool.Value = tempvalue; break;
                        case 6: Station_infiniteWarp_bool.Value = tempvalue; break;
                        case 7: InfineteStarPower.Value = tempvalue; break;
                        case 8: quickEjector = tempvalue; break;
                        case 9: quicksilo = tempvalue; break;
                        case 10: PlanetPower_bool.Value = tempvalue; break;
                        case 11: PlanetPower_bool.Value = tempvalue; break;
                        case 12: farconnectdistance = tempvalue; break;
                        case 13: Buildingnoconsume.Value = tempvalue; break;
                        case 14: Stationfullenergy.Value = tempvalue; break;
                        case 15: StationMaxproliferator.Value = tempvalue; break;
                        case 16:
                            if (tempvalue != BeltSignalFunction.Value)
                            {
                                BeltSignalFunction.Value = tempvalue;
                                if (tempvalue)
                                {
                                    InitBeltSignalDiction();
                                }
                            }
                            break;
                        case 17: Tankcontentall.Value = tempvalue; break;
                        case 18: DriftBuildings = tempvalue; break;
                    }
                }
                GUI.Label(new Rect(0, heightdis * 21, heightdis * 13, heightdis), "抬升层数:" + DriftBuildingLevel);
                GUILayout.EndArea();
            }
            {
                GUILayout.BeginArea(new Rect(english ? heightdis * 13 + 10 : heightdis * 10 + 10, 10, heightdis * 21 + 10, heightdis * 20));
                int tempplanetquamaxpowerpertick = (int)GUI.HorizontalSlider(new Rect(0, 5, heightdis * 5, heightdis), planetquamaxpowerpertick.Value, 60, 10000);
                GUI.Label(new Rect(heightdis * 5, 0, heightdis * 15, togglesize), tempplanetquamaxpowerpertick > 1000 ? tempplanetquamaxpowerpertick / 1000 + "G" : tempplanetquamaxpowerpertick + "MW " + "星球量子充电功率".getTranslate(), style);
                int tempstarquamaxpowerpertick = (int)GUI.HorizontalSlider(new Rect(0, 5 + heightdis, heightdis * 5, heightdis), starquamaxpowerpertick.Value, 60, 10000);
                GUI.Label(new Rect(heightdis * 5, heightdis, heightdis * 15, togglesize), tempstarquamaxpowerpertick > 1000 ? tempstarquamaxpowerpertick / 1000 + "G" : tempstarquamaxpowerpertick + "MW " + "星系量子充电功率".getTranslate(), style);
                Quantumenergy.Value = (int)GUI.HorizontalSlider(new Rect(0, 5 + heightdis * 2, heightdis * 5, heightdis), Quantumenergy.Value, 0, 1000000);
                GUI.Label(new Rect(heightdis * 5, heightdis * 2, heightdis * 15, togglesize), TGMKinttostring(Quantumenergy.Value, "J") + " " + "量子耗能/个".getTranslate(), style);


                if (tempplanetquamaxpowerpertick != planetquamaxpowerpertick.Value || tempstarquamaxpowerpertick != starquamaxpowerpertick.Value)
                {
                    changequapowerpertick = true;
                    planetquamaxpowerpertick.Value = tempplanetquamaxpowerpertick;
                    starquamaxpowerpertick.Value = tempstarquamaxpowerpertick;
                }
                string[] tempstr = new string[7] { "量子传输站", "星球级翘曲全面供应", "星系级翘曲全面供应", "星系量子不突破上限", "自动改名\"星球量子传输站\"", "自动改名\"星系量子传输站\"", "星球级材料供应" };
                for (int i = 0; i < 7; i++)
                {
                    bool tempvalue = false;

                    switch (i)
                    {
                        case 0: tempvalue = Quantumtransport_bool.Value; break;
                        case 1: tempvalue = Quantumtransportpdwarp_bool.Value; break;
                        case 2: tempvalue = Quantumtransportstarwarp_bool.Value; break;
                        case 3: tempvalue = QuantumStarstationmax.Value; break;
                        case 4: tempvalue = autochangeQuantumstationname; break;
                        case 5: tempvalue = autochangeQuantumStarstationname; break;
                        case 6: tempvalue = Quantumtransportbuild_bool.Value; break;
                    }
                    tempvalue = GUI.Toggle(new Rect(0, heightdis * (i + 3), heightdis * 15, heightdis), tempvalue, tempstr[i].getTranslate());
                    switch (i)
                    {
                        case 0: Quantumtransport_bool.Value = tempvalue; break;
                        case 1: Quantumtransportpdwarp_bool.Value = tempvalue; break;
                        case 2: Quantumtransportstarwarp_bool.Value = tempvalue; break;
                        case 3: QuantumStarstationmax.Value = tempvalue; break;
                        case 4:
                            autochangeQuantumstationname = tempvalue;
                            if (tempvalue) autochangeQuantumStarstationname = false;
                            break;
                        case 5:
                            autochangeQuantumStarstationname = tempvalue;
                            if (tempvalue) autochangeQuantumstationname = false;
                            break;
                        case 6:
                            Quantumtransportbuild_bool.Value = tempvalue;
                            if (tempvalue)
                            {
                                string[] tempstr1 = new string[6] { "星球级物流站材料供应", "星球级矿机材料供应", "星球级发射井弹射器材料供应", "星球级研究站材料供应", "星球级电力设备材料供应", "星球级组装机材料供应" };
                                for (int j = 1; j <= 6; j++)
                                {
                                    bool tempvalue1 = false;
                                    switch (j)
                                    {
                                        case 1: tempvalue1 = Quantumtransportstation_bool.Value; break;
                                        case 2: tempvalue1 = Quantumtransportminer_bool.Value; break;
                                        case 3: tempvalue1 = Quantumtransportsilo_bool.Value; break;
                                        case 4: tempvalue1 = Quantumtransportlab_bool.Value; break;
                                        case 5: tempvalue1 = Quantumtransportpower_bool.Value; break;
                                        case 6: tempvalue1 = Quantumtransportassemble_bool.Value; break;
                                    }
                                    tempvalue1 = GUI.Toggle(new Rect(20, heightdis * (i + j + 3), heightdis * 15, heightdis), tempvalue1, tempstr1[j - 1].getTranslate());
                                    switch (j)
                                    {
                                        case 1: Quantumtransportstation_bool.Value = tempvalue1; break;
                                        case 2: Quantumtransportminer_bool.Value = tempvalue1; break;
                                        case 3: Quantumtransportsilo_bool.Value = tempvalue1; break;
                                        case 4: Quantumtransportlab_bool.Value = tempvalue1; break;
                                        case 5: Quantumtransportpower_bool.Value = tempvalue1; break;
                                        case 6: Quantumtransportassemble_bool.Value = tempvalue1; break;
                                    }
                                }
                            }
                            break;
                    }
                }


                GUILayout.EndArea();
            }
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
                    case 2: tempvalue = changeveinposbool.Value; break;
                    case 3: tempvalue = changeveingroupposbool.Value; break;
                    case 4: tempvalue = NotTidyVein.Value; break;
                    case 5: tempvalue = getallVein_bool.Value; break;
                    case 6: tempvalue = changexveinspos.Value; break;
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
                            changeveinposbool.Value = false;
                            changeveingroupposbool.Value = false;
                            getallVein_bool.Value = false;
                            changexveinspos.Value = false;
                        }
                        break;
                    case 1: deleteveinbool.Value = tempvalue; break;
                    case 2:
                        changeveinposbool.Value = tempvalue;
                        if (changeveinposbool.Value)
                        {
                            addveinbool = false;
                            changeveingroupposbool.Value = false;
                            getallVein_bool.Value = false;
                            changexveinspos.Value = false;
                        }
                        break;
                    case 3:
                        changeveingroupposbool.Value = tempvalue;
                        if (changeveingroupposbool.Value)
                        {
                            addveinbool = false;
                            changeveinposbool.Value = false;
                            getallVein_bool.Value = false;
                            changexveinspos.Value = false;
                        }
                        break;
                    case 4:
                        NotTidyVein.Value = tempvalue;
                        break;
                    case 5:
                        width++;
                        GUI.Label(new Rect(0, heightdis * (i + width++), english ? heightdis * 9 : heightdis * 5, heightdis), "整理为".getTranslate() + veinlines.Value + "行".getTranslate());
                        veinlines.Value = (int)GUI.HorizontalSlider(new Rect(0, heightdis * (i + width), english ? heightdis * 9 : heightdis * 5, heightdis), veinlines.Value, 1, 20);
                        getallVein_bool.Value = tempvalue;
                        if (getallVein_bool.Value)
                        {
                            addveinbool = false;
                            changeveinposbool.Value = false;
                            changeveingroupposbool.Value = false;
                            changexveinspos.Value = false;
                        }
                        break;
                    case 6:
                        width++;
                        GUI.Label(new Rect(0, heightdis * (i + width++), english ? heightdis * 9 : heightdis * 5, heightdis), "切割出".getTranslate() + changeveinsposx.Value + "个".getTranslate());
                        changeveinsposx.Value = (int)GUI.HorizontalSlider(new Rect(0, heightdis * (i + width), english ? heightdis * 9 : heightdis * 5, heightdis), changeveinsposx.Value, 2, 72);

                        changexveinspos.Value = tempvalue;
                        if (changexveinspos.Value)
                        {
                            addveinbool = false;
                            changeveinposbool.Value = false;
                            changeveingroupposbool.Value = false;
                            getallVein_bool.Value = false;
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
            if (cancelsolarbullet.Value != GUI.Toggle(new Rect(0, heightdis * line++, heightdis * 10, heightdis), cancelsolarbullet.Value, "跳过太阳帆子弹阶段".getTranslate()))
            {
                cancelsolarbullet.Value = !cancelsolarbullet.Value;
                playcancelsolarbullet = cancelsolarbullet.Value;
            }
            quickabsorbsolar.Value = GUI.Toggle(new Rect(0, heightdis * line++, heightdis * 10, heightdis), quickabsorbsolar.Value, "跳过太阳帆吸收阶段".getTranslate());
            if (alwaysemission.Value != GUI.Toggle(new Rect(0, heightdis * line++, heightdis * 10, heightdis), alwaysemission.Value, "全球打帆".getTranslate()))
            {
                alwaysemission.Value = !alwaysemission.Value;
                alwaysemissiontemp = alwaysemission.Value;
            }

        }

        public void OtherPannel()
        {
            bool english = Localization.language != Language.zhCN;
            GUILayout.BeginArea(new Rect(10, 10, 10 + (english ? heightdis * 12 : heightdis * 10), heightdis * 17));
            {
                playwithtrashtimemult = (int)GUI.HorizontalSlider(new Rect(0, 5, heightdis * 5, heightdis), playwithtrashtimemult, 1, 100);
                GUI.Label(new Rect(heightdis * 5, 0, heightdis * 7, heightdis), "丢垃圾速率".getTranslate() + ":" + (int)playwithtrashtimemult, style);

                int lines = 0;
                Rect t = new Rect(0, heightdis, english ? heightdis * 12 : heightdis * 10, heightdis);
                GUI.Label(RectChanged(t, heightdis * lines++, 2), "以下设置需要进入存档".getTranslate(), style);
                playwithtrash = GUI.Toggle(RectChanged(t, heightdis * lines++, 2), playwithtrash, "丢垃圾(整活用)".getTranslate());
                sunlight_bool.Value = GUI.Toggle(RectChanged(t, heightdis * lines++, 2), sunlight_bool.Value, "夜灯".getTranslate());
                if (SandBoxMode != GUI.Toggle(RectChanged(t, heightdis * lines++, 2), SandBoxMode, "创造模式".getTranslate()))
                {
                    SandBoxMode = !SandBoxMode;
                    GameMain.sandboxToolsEnabled = SandBoxMode;
                }
                pasteanyway = GUI.Toggle(RectChanged(t, heightdis * lines++, 2), pasteanyway, "蓝图强制粘贴".getTranslate());
                PasteBuildAnyWay = GUI.Toggle(RectChanged(t, heightdis * lines++, 2), PasteBuildAnyWay, "建筑铺设无需条件".getTranslate());
                if (closeallcollider != GUI.Toggle(RectChanged(t, heightdis * lines++, 2), closeallcollider, "关闭所有碰撞体".getTranslate()))
                {
                    closeallcollider = !closeallcollider;
                    ColliderPool.instance.gameObject.SetActive(!closeallcollider);
                }

                if (!Infinitething.Value || !dismantle_but_nobuild.Value) playwithtrash = false;

                stackmultiple1 = Regex.Replace(GUI.TextField(RectChanged(t, heightdis * lines++, 2), stackmultiple1, 10), @"[^0-9]", "");
                if (GUI.Button(RectChanged(t, heightdis * lines++, 2), "设置堆叠倍率".getTranslate()))
                {
                    StackMultiple.Value = int.Parse(stackmultiple1);
                    ItemProto[] dataArray = LDB.items.dataArray;
                    for (int j = 0; j < dataArray.Length; j++)
                    {
                        StorageComponent.itemStackCount[dataArray[j].ID] = dataArray[j].StackSize * StackMultiple.Value;
                    }
                }
                if (GUI.Button(RectChanged(t, heightdis * lines++, 2), "成就重新检测".getTranslate()))
                {
                    GameMain.data.abnormalData.runtimeDatas = new AbnormalityRuntimeData[3000];
                }
                GUI.Label(RectChanged(t, heightdis * lines++, 2), "以下设置需要重启游戏".getTranslate(), style);
                multipelsmelt1 = Regex.Replace(GUI.TextField(RectChanged(t, heightdis * lines++, 2), multipelsmelt1, 10), @"[^0-9]", "");
                if (GUI.Button(RectChanged(t, heightdis * lines++, 2), "设置冶炼倍数".getTranslate())) MULTIPELSMELT.Value = int.Parse(multipelsmelt1);
                allhandcraft.Value = GUI.Toggle(RectChanged(t, heightdis * lines++, 2), allhandcraft.Value, "全部手搓".getTranslate());
                quickproduce.Value = GUI.Toggle(RectChanged(t, heightdis * lines++, 2), quickproduce.Value, "快速生产".getTranslate());
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
            bool english = Localization.language != Language.zhCN;
            GUILayout.BeginArea(new Rect(10, 10, MainWindow_width.Value, MainWindow_height.Value));
            int tempwidth = 0;
            int tempheight = english ? 1 : 0;
            int tempwidthdis = english ? heightdis * 8 : heightdis * 4;
            Mechalogistics_bool.Value = GUI.Toggle(new Rect(0, 0, tempwidthdis, heightdis), Mechalogistics_bool.Value, "机甲物流(beta)".getTranslate());
            tempwidth += tempwidthdis;
            MechalogisticsPlanet_bool.Value = GUI.Toggle(new Rect(tempwidth, 0, tempwidthdis, heightdis), MechalogisticsPlanet_bool.Value, "机甲物流专用星".getTranslate());
            tempwidth = english ? 0 : 2 * tempwidthdis;
            MechalogStoragerecycle_bool.Value = GUI.Toggle(new Rect(tempwidth, tempheight * heightdis, tempwidthdis, heightdis), MechalogStoragerecycle_bool.Value, "回收使用储物仓".getTranslate());
            tempwidth += tempwidthdis;
            MechalogStorageprovide_bool.Value = GUI.Toggle(new Rect(tempwidth, tempheight * heightdis, tempwidthdis, heightdis), MechalogStorageprovide_bool.Value, "需求使用储物仓".getTranslate());
            tempwidth += tempwidthdis;
            MechalogStationrecycle_bool.Value = GUI.Toggle(new Rect(tempwidth, tempheight * heightdis, tempwidthdis, heightdis), MechalogStationrecycle_bool.Value, "回收使用物流站".getTranslate());
            tempwidth += tempwidthdis;
            MechalogStationprovide_bool.Value = GUI.Toggle(new Rect(tempwidth, tempheight * heightdis, tempwidthdis, heightdis), MechalogStationprovide_bool.Value, "需求使用物流站".getTranslate());


            GUILayout.BeginArea(new Rect(0, 10 + (tempheight + 1) * heightdis, MainWindow_width.Value, MainWindow_height.Value));
            tempwidth = 0;
            tempheight = 0;
            for (int i = 0; i < itemProtos.Count; i++)
            {
                ItemProto ip = itemProtos[i];
                GUIStyle style = new GUIStyle();
                GUI.Button(new Rect(tempwidth, heightdis * tempheight, heightdis * 2, heightdis * 2), ip.iconSprite.texture, style);
                if (GUI.Button(new Rect(tempwidth, heightdis * (tempheight + 2), heightdis * 2, heightdis), (mechalogistics[i] == 0 ? "储存" : (mechalogistics[i] == -1 ? "回收" : "需求")).getTranslate()))
                {
                    mechalogistics[i]++;
                    if (mechalogistics[i] > 1) mechalogistics[i] = -1;
                    Savemechalogisticsneed();
                }

                if (mechalogistics[i] == 1)
                {
                    int temp = mechalogisticsneed[i];
                    int.TryParse(Regex.Replace(GUI.TextField(new Rect(tempwidth, heightdis * (tempheight + 3), heightdis * 2, heightdis), mechalogisticsneed[i] + ""), @"[^0-9]", ""), out temp);
                    if (temp != mechalogisticsneed[i])
                    {
                        mechalogisticsneed[i] = temp;
                        Savemechalogisticsneed();
                    }
                }
                tempwidth += heightdis * 2;
                if (tempwidth > MainWindow_width.Value - 4 * heightdis)
                {
                    tempheight += 4;
                    tempwidth = 0;
                }
            }
            GUILayout.EndArea();
            GUILayout.EndArea();
        }

        #endregion

        #region 量子传输站
        public void takeitemfromstarsuperstation()
        {
            if (GameMain.data == null || GameMain.data.galacticTransport == null || GameMain.data.galacticTransport.stationPool == null) return;
            StarSuperStation = new List<StationComponent>();
            SuperStation = new List<StationComponent>();
            StarSuperStationItemidstore = new Dictionary<int, int[]>();
            PlanetSuperStationItemidstore = new Dictionary<int, Dictionary<int, int[]>>();
            foreach (StationComponent sc in GameMain.data.galacticTransport.stationPool)
            {
                if (sc == null || sc.storage == null || sc.name == null || sc.isCollector) continue;
                if (sc.name.Equals("星系量子传输站"))
                {
                    StarSuperStation.Add(sc);
                    for (int i = 0; i < sc.storage.Length; i++)
                    {
                        if (sc.storage[i].itemId > 0)
                        {
                            if (!StarSuperStationItemidstore.ContainsKey(sc.storage[i].itemId))
                                StarSuperStationItemidstore.Add(sc.storage[i].itemId, new int[] { StarSuperStation.Count - 1, i });
                            if (sc.storage[i].count < 0)
                                sc.storage[i].count = 0;
                        }
                    }
                    if (changequapowerpertick)
                    {
                        try
                        {
                            GameMain.galaxy.PlanetById(sc.planetId).factory.powerSystem.consumerPool[sc.pcId].workEnergyPerTick = (long)starquamaxpowerpertick.Value * 16667;
                        }
                        catch
                        {

                        }
                    }
                }
                if (sc.name.Equals("星球量子传输站"))
                {
                    SuperStation.Add(sc);
                    if (changequapowerpertick)
                    {
                        try
                        {
                            GameMain.galaxy.PlanetById(sc.planetId).factory.powerSystem.consumerPool[sc.pcId].workEnergyPerTick = (long)planetquamaxpowerpertick.Value * 16667;
                        }
                        catch
                        {

                        }
                    }

                }
            }
            foreach (StationComponent sc in GameMain.data.galacticTransport.stationPool)
            {
                if (sc == null) continue;
                if (sc.isCollector)
                {
                    sc.storage[0].count -= doitemfromStarsuper(sc.storage[0].itemId, sc.storage[0].count, 0, false)[0];
                    sc.storage[1].count -= doitemfromStarsuper(sc.storage[1].itemId, sc.storage[1].count, 0, false)[0];
                }
            }
            changequapowerpertick = false;
            if (StarSuperStation.Count == 0 && SuperStation.Count == 0) return;
            if (Math.Max(StarSuperStation.Count, SuperStation.Count) > warpstationqua.Length - 50)
                SetSuperStationCapacity(Math.Max(StarSuperStation.Count, SuperStation.Count) + 50);

            for (int j = 0; j < SuperStation.Count; j++)
            {
                StationComponent sc = SuperStation[j];
                for (int i = 0; i < sc.storage.Length; i++)
                {
                    if (sc.storage[i].itemId > 0)
                    {
                        if (!PlanetSuperStationItemidstore.ContainsKey(sc.planetId))
                            PlanetSuperStationItemidstore.Add(sc.planetId, new Dictionary<int, int[]>());
                        if (!PlanetSuperStationItemidstore[sc.planetId].ContainsKey(sc.storage[i].itemId))
                            PlanetSuperStationItemidstore[sc.planetId].Add(sc.storage[i].itemId, new int[] { j, i });
                        if (sc.storage[i].remoteLogic == ELogisticStorage.Demand && sc.storage[i].count < sc.storage[i].max)
                        {
                            if (sc.storage[i].max > sc.storage[i].count)
                            {
                                int[] takeitem = doitemfromStarsuper(sc.storage[i].itemId, sc.storage[i].max - sc.storage[i].count);
                                sc.storage[i].count += takeitem[0];
                                sc.storage[i].inc += takeitem[1];
                            }
                            if (sc.storage[i].max < sc.storage[i].count)
                            {
                                int[] takeitem = doitemfromStarsuper(sc.storage[i].itemId, sc.storage[i].count - sc.storage[i].max, sc.storage[i].inc, false);
                                sc.storage[i].count -= takeitem[0];
                                sc.storage[i].inc -= takeitem[1];
                            }
                        }

                        if (sc.storage[i].count < 0)
                            sc.storage[i].count = 0;
                    }
                }
                if (Quantumtransportpdwarp_bool.Value && sc.warperCount == 0)
                    sc.warperCount += doitemfromStarsuper(1210, 50, 0)[0];
            }
            for (int j = 0; j < StarSuperStation.Count; j++)
            {
                if (Quantumtransportstarwarp_bool.Value && StarSuperStation[j].warperCount == 0)
                    StarSuperStation[j].warperCount += doitemfromStarsuper(1210, 50, 0)[0];
                StationComponent sc = StarSuperStation[j];
            }
            if (Quantumtransportbuild_bool.Value)
                SuperStationProvide();

        }
        public void StationSupplyDemand(int pdid)
        {
            PlanetData pd = GameMain.galaxy.PlanetById(pdid);
            foreach (StationComponent sc in pd.factory.transport.stationPool)
            {
                if (sc != null && sc.storage != null && !SuperStation.Contains(sc) && !StarSuperStation.Contains(sc) && sc.planetId == pdid)
                {
                    for (int i = 0; i < sc.storage.Length; i++)
                    {
                        if (sc.storage[i].itemId > 0 && sc.storage[i].count > 0)
                        {
                            if (sc.storage[i].localLogic == ELogisticStorage.Supply)
                            {
                                int[] takeitem = doitemfromPlanetSuper(sc.storage[i].itemId, sc.storage[i].count, pdid, sc.storage[i].inc, false);
                                sc.storage[i].count -= takeitem[0];
                                sc.storage[i].inc -= takeitem[1];
                            }
                            else if (sc.storage[i].count > sc.storage[i].max)
                            {
                                int[] takeitem = doitemfromPlanetSuper(sc.storage[i].itemId, sc.storage[i].count - sc.storage[i].max, pdid, sc.storage[i].inc, false);
                                sc.storage[i].count -= takeitem[0];
                                sc.storage[i].inc -= takeitem[1];
                            }
                            else
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
        public void MinerSupplyDemand(FactorySystem fs, int pdid)
        {
            foreach (MinerComponent mc in fs.minerPool)
            {
                if (mc.id > 0 && mc.entityId > 0 && mc.productCount > 0 && mc.productId > 0)
                {
                    fs.minerPool[mc.id].productCount -= doitemfromPlanetSuper(mc.productId, mc.productCount, pdid, 0, false)[0];
                }
            }
        }
        public void SiloComponentSupplyDemand(FactorySystem fs, int pdid)
        {
            foreach (SiloComponent sc in fs.siloPool)
            {
                if (sc.id > 0 && sc.entityId > 0)
                {
                    if (sc.bulletCount <= 1)
                    {
                        int[] takeitem = doitemfromPlanetSuper(1503, 20, pdid);
                        fs.siloPool[sc.id].bulletCount = takeitem[0];
                        fs.siloPool[sc.id].bulletInc = takeitem[1];
                    }

                }
            }
            foreach (EjectorComponent ec in fs.ejectorPool)
            {
                if (ec.id > 0 && ec.entityId > 0)
                {
                    if (ec.bulletCount <= 1)
                    {
                        int[] takeitem = doitemfromPlanetSuper(1501, 20, pdid);
                        fs.ejectorPool[ec.id].bulletCount = takeitem[0];
                        fs.ejectorPool[ec.id].bulletInc = takeitem[1];
                    }
                }
            }
        }
        public void AssemblerSupplyDemand(FactorySystem fs, int pdid)
        {
            foreach (AssemblerComponent ac in fs.assemblerPool)
            {
                if (ac.id > 0 && ac.entityId > 0 && ac.recipeId > 0)
                {
                    for (int i = 0; i < ac.productCounts.Length; i++)
                    {
                        if (ac.produced[i] > 0) ac.produced[i] -= doitemfromPlanetSuper(ac.products[i], ac.produced[i], pdid, 0, false)[0];
                        if (ac.produced[i] > 500) ac.produced[i] = 200;
                    }
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
                    //if (condition) ac.UpdateNeeds();
                }
            }
        }
        public void LabSupplyDemand(FactorySystem fs, int pdid)
        {
            foreach (LabComponent lc in fs.labPool)
            {
                if (lc.id > 0 && lc.entityId > 0)
                {
                    if (lc.researchMode)
                    {
                        for (int i = 0; i < lc.matrixPoints.Length; i++)
                        {
                            if (lc.matrixServed[i] <= 36000)
                            {
                                int[] takeitem = doitemfromPlanetSuper(6001 + i, 20, pdid);
                                lc.matrixServed[i] += takeitem[0] * 3600;
                                lc.matrixIncServed[i] += takeitem[1] * 3600;
                            }
                        }
                    }
                    if (lc.matrixMode)
                    {
                        for (int i = 0; i < lc.productCounts.Length; i++)
                        {
                            if (lc.produced[i] > 0) lc.produced[i] -= doitemfromPlanetSuper(lc.products[i], lc.produced[i], pdid, 0, false)[0];
                        }
                        for (int i = 0; i < lc.served.Length; i++)
                        {
                            if (lc.served[i] <= lc.requireCounts[i] * 2)
                            {
                                int[] takeitem = doitemfromPlanetSuper(lc.requires[i], 20, pdid);
                                lc.served[i] += takeitem[0];
                                lc.incServed[i] += takeitem[1];
                            }
                        }
                        //if (condition) lc.UpdateNeedsAssemble();
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
                        if (pgc.productCount >= 1) fs.factory.powerSystem.genPool[pgc.id].productCount -= doitemfromPlanetSuper(1208, (int)pgc.productCount, pdid, 0, false)[0];
                        if (pgc.catalystPoint == 0)
                        {
                            int[] takeitem = doitemfromPlanetSuper(1209, 5, pdid);
                            fs.factory.powerSystem.genPool[pgc.id].catalystPoint += takeitem[0] * 3600;
                            fs.factory.powerSystem.genPool[pgc.id].catalystIncPoint += takeitem[1] * 3600;
                        }
                    }
                    else
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

                if (Quantumtransportstation_bool.Value) StationSupplyDemand(pdid);
                if (Quantumtransportminer_bool.Value) MinerSupplyDemand(fs, pdid);
                if (Quantumtransportsilo_bool.Value) SiloComponentSupplyDemand(fs, pdid);
                if (Quantumtransportlab_bool.Value) LabSupplyDemand(fs, pdid);
                if (Quantumtransportpower_bool.Value) PowerSupplyDemand(fs, pdid);
                if (Quantumtransportassemble_bool.Value) AssemblerSupplyDemand(fs, pdid);
            }
        }
        public int[] doitemfromStarsuper(int itemid, int itemcount, int inc = 0, bool take = true)
        {
            if (!StarSuperStationItemidstore.ContainsKey(itemid) || itemcount == 0) return new int[2] { 0, 0 };
            long energycost = Quantumenergy.Value;
            StationComponent sc = StarSuperStation[StarSuperStationItemidstore[itemid][0]];
            int index = StarSuperStationItemidstore[itemid][0];
            int i = StarSuperStationItemidstore[itemid][1];
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
                if (sc.energy < count * energycost)
                    count = Math.Min(count, (int)(sc.energy / energycost));
                if (!Stationfullenergy.Value)
                    sc.energy -= count * energycost;
                if (count == 0) return new int[2] { 0, 0 };
                int num1 = sc.storage[i].inc / sc.storage[i].count;
                int num2 = sc.storage[i].inc - num1 * sc.storage[i].count;
                sc.storage[i].count -= count;
                int num3 = num2 - sc.storage[i].count;
                int num4 = num3 > 0 ? num1 * count + num3 : num1 * count;
                sc.storage[i].inc -= num4;
                inc = num4;
            }
            else
            {
                int inputinc;
                if (sc.energy < count * energycost)
                {
                    int p = count - Math.Min(count, (int)(sc.energy / energycost));
                    int num1 = inc / count;
                    int num2 = inc - num1 * count;
                    count -= p;
                    int num3 = num2 - count;
                    int num4 = num3 > 0 ? num1 * p + num3 : num1 * p;
                    inc -= num4;
                    inputinc = num4;
                }
                else
                {
                    inputinc = inc;
                    inc = 0;
                }
                if (sc.storage[i].count + count <= sc.storage[i].max)
                {
                    sc.storage[i].count += count;
                    sc.storage[i].inc += inputinc;
                }
                else if (!QuantumStarstationmax.Value)
                {
                    sc.storage[i].max = sc.storage[i].count + count;
                    sc.storage[i].count = sc.storage[i].max;
                    sc.storage[i].inc += inputinc;
                }
                else if (sc.storage[i].count < sc.storage[i].max)
                {
                    count = Math.Min(count, sc.storage[i].max - sc.storage[i].count);
                    sc.storage[i].count += count;
                    sc.storage[i].inc += inputinc;
                }
                else count = 0;
                if (!Stationfullenergy.Value)
                    sc.energy -= count * energycost;
            }
            if (sc.storage[4].itemId > 0 && sc.storage[4].count > 0)
            {
                ItemProto ip = LDB.items.Select(sc.storage[4].itemId);
                if (ip.HeatValue > 0 && sc.energyMax > sc.energy)
                {
                    int num = Math.Min(sc.storage[4].count, (int)((sc.energyMax - sc.energy) / ip.HeatValue));
                    if (!Stationfullenergy.Value)
                        sc.energy += num * ip.HeatValue;
                    sc.storage[4].count -= num;
                    AddStatInfo(sc.planetId, ip.ID, num, false);
                }
            }
            return new int[2] { count, StationMaxproliferator.Value ? 4 * count : inc };
        }

        public int[] doitemfromPlanetSuper(int itemid, int itemcount, int planetid, int inc = 0, bool take = true)
        {
            if (!PlanetSuperStationItemidstore.ContainsKey(planetid) || !PlanetSuperStationItemidstore[planetid].ContainsKey(itemid) || itemcount == 0) return new int[2] { 0, 0 };
            long energycost = Quantumenergy.Value;
            StationComponent sc = SuperStation[PlanetSuperStationItemidstore[planetid][itemid][0]];
            int index = PlanetSuperStationItemidstore[planetid][itemid][0];
            int i = PlanetSuperStationItemidstore[planetid][itemid][1];

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
                if (count > sc.storage[i].count)
                {
                    int download = count - sc.storage[i].count;
                    int[] downloaded = doitemfromStarsuper(itemid, download);
                    count = sc.storage[i].count + downloaded[0];
                    inc = sc.storage[i].inc + downloaded[1];
                    sc.storage[i].count = 0;
                    sc.storage[i].inc = 0;
                }
                else
                {
                    int num1 = sc.storage[i].inc / sc.storage[i].count;
                    int num2 = sc.storage[i].inc - num1 * sc.storage[i].count;
                    sc.storage[i].count -= count;
                    int num3 = num2 - sc.storage[i].count;
                    int num4 = num3 > 0 ? num1 * count + num3 : num1 * count;
                    sc.storage[i].inc -= num4;
                    inc = num4;
                }
                if (!Stationfullenergy.Value)
                    sc.energy -= count * energycost;
            }
            else
            {
                int inputinc;
                if (sc.energy < count * energycost)
                {
                    int p = count - Math.Min(count, (int)(sc.energy / energycost));
                    int num1 = inc / count;
                    int num2 = inc - num1 * count;
                    count -= p;
                    int num3 = num2 - count;
                    int num4 = num3 > 0 ? num1 * p + num3 : num1 * p;
                    inc -= num4;
                    inputinc = num4;
                }
                else
                {
                    inputinc = inc;
                    inc = 0;
                }
                if (sc.storage[i].count + count >= sc.storage[i].max)
                {
                    int upload = sc.storage[i].count + count - sc.storage[i].max;
                    int[] uploaded = doitemfromStarsuper(itemid, upload, inputinc, false);
                    count -= upload - uploaded[0];
                    inc -= uploaded[1];
                    sc.storage[i].count = sc.storage[i].max;
                    sc.storage[i].inc += uploaded[1];
                }
                else
                {
                    sc.storage[i].count += count;
                    sc.storage[i].inc += inputinc;
                }
                if (!Stationfullenergy.Value)
                    sc.energy -= count * energycost;
            }
            if (sc.storage[4].itemId > 0 && sc.storage[4].count > 0)
            {
                ItemProto ip = LDB.items.Select(sc.storage[4].itemId);
                if (ip.HeatValue > 0 && sc.energyMax > sc.energy)
                {
                    int num = Math.Min(sc.storage[4].count, (int)((sc.energyMax - sc.energy) / ip.HeatValue));
                    sc.energy += num * ip.HeatValue;
                    sc.storage[4].count -= num;

                    AddStatInfo(sc.planetId, ip.ID, num);
                }
            }
            return new int[2] { count, StationMaxproliferator.Value ? 4 * count : inc };


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
                RecipeList = new List<RecipeProto>(LDB.recipes.dataArray);
                itemProtos = new List<ItemProto>(LDB.items.dataArray);
                mechalogistics = new int[itemProtos.Count];
                mechalogisticsneed = new int[itemProtos.Count];

                if ("".Equals(Mechalogneed.Value) || Mechalogneed.Value.Length == 0)
                {
                    for (int i = 0; i < itemProtos.Count; i++)
                    {
                        mechalogistics[i] = 0;
                        mechalogisticsneed[i] = 0;
                    }
                }
                else
                {
                    Loadmechalogisticsneed();
                }
                first = false;
                ready = true;
                //冶炼设施倍率
                Changesmelt();
                //更改配方相关
                ChangeRecipe();
                //更改物品相关
                ChangeItemstack();
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
                    playcancelsolarbullet = false;
                    alwaysemissiontemp = false;
                }
                if (GameMain.instance.running && !FinallyInit)
                {
                    OrwatertypePlanetArray = new Dictionary<int, int>();
                    Beltsignal = new Dictionary<int, Dictionary<int, int>>();
                    Beltsignalnumberoutput = new Dictionary<int, Dictionary<int, int>>();
                    productmaxindex = 0;
                    SuperStation = new List<StationComponent>();
                    foreach (StarData sd in GameMain.galaxy.stars)
                    {
                        foreach (PlanetData pd in sd.planets)
                        {
                            if (pd.factory == null || pd.factory.factorySystem == null) continue;

                            FactorySystem fs = pd.factory.factorySystem;
                            if (pd.factory.transport != null)
                            {
                                foreach (StationComponent sc in fs.factory.transport.stationPool)
                                {
                                    if (sc != null && sc.entityId > 0 && sc.name != null && sc.name.Equals("星球量子传输站"))
                                    {
                                        SuperStation.Add(sc);
                                    }
                                    if (sc != null && sc.entityId > 0 && sc.name != null && sc.name.Equals("星系量子传输站"))
                                    {
                                        StarSuperStation.Add(sc);
                                    }
                                }
                            }
                        }
                    }

                    warpstationqua = new float[SuperStation.Count >= 200 ? SuperStation.Count + 100 : 200];
                    warpsuperstationqua = new float[StarSuperStation.Count >= 200 ? StarSuperStation.Count + 100 : 200];
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
                    Playwithtrashset();
                    Sunlightset();
                    SetDronenocomsume();
                    MechaLogisticsMethod();
                    InfiniteAllThingInPackage();

                    if (Infiniteplayerpower.Value)
                    {
                        player.mecha.coreEnergy = player.mecha.coreEnergyCap;
                    }
                    if (BuildNotime_bool.Value && player.controller.actionBuild.clickTool.factory != null)
                    {
                        if (!buildnotimecolddown && player.controller.actionBuild.clickTool.factory.prebuildCount > 0)
                        {
                            foreach (PrebuildData pd in player.controller.actionBuild.clickTool.factory.prebuildPool)
                            {
                                if (pd.itemRequired > 0 && !Infinitething.Value && !ArchitectMode.Value) continue;
                                if (pd.id <= 0) continue;
                                player.controller.actionBuild.clickTool.factory.BuildFinally(player, pd.id);
                            }
                        }
                    }
                }

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
            GameHistoryData historyData = GameMain.history;
            player.mecha.coreEnergyCap = 100000000;
            player.mecha.coreLevel = 0;
            player.mecha.miningSpeed = 1;
            player.mecha.walkSpeed = 6;
            player.mecha.jumpSpeed = 32;
            player.mecha.droneCount = 3;
            player.mecha.droneMovement = 1;
            player.mecha.droneSpeed = 6;
            player.mecha.maxSailSpeed = 1000;
            player.mecha.maxWarpSpeed = 480000;
            player.mecha.buildArea = 80;
            player.mecha.replicateSpeed = 1;
            player.mecha.reactorPowerGen = 800000;
            historyData.stationPilerLevel = 1;
            historyData.logisticDroneCarries = 25;
            historyData.logisticDroneSpeed = 8;
            historyData.logisticShipCarries = 200;
            historyData.logisticShipSailSpeed = 600;
            historyData.logisticDroneSpeedScale = 1;
            historyData.logisticShipSpeedScale = 1;
            historyData.logisticShipWarpSpeed = 120000;
            historyData.inserterStackCount = 1;
            historyData.logisticShipWarpDrive = false;
            historyData.miningCostRate = 1;
            historyData.miningSpeedScale = 1;
            historyData.techSpeed = 1;
            historyData.storageLevel = 2;
            historyData.labLevel = 3;
            historyData.dysonNodeLatitude = 0;
            historyData.solarEnergyLossRate = 0.7f;
            historyData.solarSailLife = 5400;
            historyData.localStationExtraStorage = 0;
            historyData.remoteStationExtraStorage = 0;
            int packagesize = 40;
            foreach (TechProto tp in new List<TechProto>(LDB.techs.dataArray))
            {
                if (tp.Level < tp.MaxLevel)
                {
                    string t = "";
                    for (int i = tp.Level; i < GameMain.history.techStates[tp.ID].curLevel + (GameMain.history.techStates[tp.ID].curLevel >= tp.MaxLevel ? 1 : 0); i++)
                        for (int j = 0; j < tp.UnlockFunctions.Length; j++)
                        {
                            t += tp.UnlockFunctions[j] + " " + tp.UnlockValues[j].ToString() + " ";
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
                        if (tp.UnlockFunctions[index] == 5) packagesize += 10;
                    }
                }
            }
            player.package.SetSize(packagesize);
        }

        public void MechaLogisticsMethod()
        {
            if (!Mechalogistics_bool.Value || Time.time - MechaLogisticsTime < 1) return;
            MechaLogisticsTime = Time.time;
            StorageComponent package = player.package;
            for (int i = 0; i < mechalogistics.Length; i++)
            {
                if (mechalogistics[i] == 0) continue;
                int itemid = itemProtos[i].ID;
                if (mechalogistics[i] == 1)
                {
                    if (!MechalogStationprovide_bool.Value && !MechalogStorageprovide_bool.Value) continue;
                    if (package.grids[package.size - 1].itemId != 0) continue;
                    int remain = 0;
                    for (int index = 0; index < package.size; ++index)
                    {
                        if (package.grids[index].itemId == itemid)
                            remain += LDB.items.Select(itemid).StackSize - package.grids[index].count;
                        else if (package.grids[index].itemId == 0)
                            remain += LDB.items.Select(itemid).StackSize;
                    }
                    int need = mechalogisticsneed[i] - package.GetItemCount(itemid);
                    need = remain > need ? need : remain;
                    int count = 0;
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
                                    int tempitemid = itemid;
                                    int inc1 = 0;
                                    sc.TakeItem(ref tempitemid, ref temp, out inc1);
                                    count += temp;
                                    need -= temp;
                                    inc += inc1;
                                    if (count >= need) break;
                                }
                                if (pd.factory.factoryStorage != null && pd.factory.factoryStorage.tankPool != null && count < need)
                                {
                                    foreach (TankComponent tc in pd.factory.factoryStorage.tankPool)
                                    {
                                        if (tc.id > 0 && tc.fluidId > 0 && tc.fluidCount > 0 && itemid == tc.fluidId)
                                        {
                                            int temp = tc.fluidCount > need ? need : tc.fluidCount;

                                            int num = (int)((double)tc.fluidInc / (double)tc.fluidCount + 0.5) * temp;
                                            pd.factory.factoryStorage.tankPool[tc.id].fluidCount -= temp;
                                            pd.factory.factoryStorage.tankPool[tc.id].fluidInc -= num;

                                            inc += num;
                                            count += temp;
                                            need -= temp;
                                            if (count >= need) break;
                                        }
                                    }
                                }
                                if (count >= need) break;
                            }
                            if (pd.factory.factoryStorage != null && pd.factory.factoryStorage.storagePool != null && MechalogStorageprovide_bool.Value && count < need)
                            {
                                foreach (StorageComponent sc in pd.factory.factoryStorage.storagePool)
                                {
                                    if (sc == null) continue;
                                    int inc1 = 0;
                                    int temp = sc.TakeItem(itemid, need, out inc1);
                                    count += temp;
                                    inc += inc1;
                                    need -= temp;
                                    if (count >= need) break;
                                }
                                if (count >= need) break;
                            }
                        }
                        if (count >= need) break;
                    }
                    player.TryAddItemToPackage(itemid, count, inc, false);
                }
                else
                {
                    if (!MechalogStationrecycle_bool.Value && !MechalogStoragerecycle_bool.Value) continue;
                    int recyclenum = package.GetItemCount(itemid);
                    int temprecycle = recyclenum;
                    int inc1 = 0;
                    package.TakeItem(itemid, recyclenum, out inc1);
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
                                    temprecycle -= sc.AddItem(itemid, temprecycle, inc1);
                                    if (temprecycle == 0) break;
                                }

                                if (pd.factory.factoryStorage != null && pd.factory.factoryStorage.tankPool != null && temprecycle > 0)
                                {
                                    foreach (TankComponent tc in pd.factory.factoryStorage.tankPool)
                                    {
                                        if (temprecycle == 0) break;
                                        if (tc.id > 0 && tc.fluidId > 0 && tc.fluidCount > 0 && itemid == tc.fluidId)
                                        {
                                            if (tc.fluidCount > 10000)
                                            {
                                                pd.factory.factoryStorage.tankPool[tc.id].fluidCount += temprecycle;
                                                pd.factory.factoryStorage.tankPool[tc.id].fluidInc += inc1;
                                                temprecycle = 0;
                                            }
                                            else
                                            {
                                                int temp = tc.fluidCapacity - tc.fluidCount > temprecycle ? temprecycle : tc.fluidCapacity - tc.fluidCount;
                                                temprecycle -= temp;
                                                pd.factory.factoryStorage.tankPool[tc.id].fluidCount += temp;
                                            }
                                        }
                                    }
                                }
                                if (temprecycle == 0) break;
                            }
                            if (pd.factory.factoryStorage != null && pd.factory.factoryStorage.storagePool != null && MechalogStoragerecycle_bool.Value && temprecycle > 0)
                            {
                                foreach (StorageComponent sc in pd.factory.factoryStorage.storagePool)
                                {
                                    if (sc == null) continue;
                                    int inc = 0;
                                    temprecycle -= sc.AddItem(itemid, temprecycle, inc1, out inc);
                                    if (temprecycle == 0) break;
                                }
                                if (temprecycle == 0) break;
                            }
                        }
                        if (temprecycle == 0) break;
                    }

                }
            }
        }

        public void Savemechalogisticsneed()
        {
            string result = "";
            for (int i = 0; i < itemProtos.Count; i++)
            {
                result += mechalogistics[i] + "," + mechalogisticsneed[i];
                result += i == itemProtos.Count - 1 ? "" : ";";
            }
            Mechalogneed.Value = result;
        }

        public void Loadmechalogisticsneed()
        {
            string[] t1 = Mechalogneed.Value.Split(';');
            if (t1.Length != itemProtos.Count)
            {
                for (int i = 0; i < itemProtos.Count; i++)
                {
                    mechalogistics[i] = 0;
                    mechalogisticsneed[i] = 0;
                }
            }
            else
            {
                for (int i = 0; i < itemProtos.Count; i++)
                {
                    mechalogistics[i] = int.Parse(t1[i].Split(',')[0]);
                    mechalogisticsneed[i] = int.Parse(t1[i].Split(',')[1]);
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
            GameHistoryData historyData = GameMain.history;
            if (player == null || GameMain.mainPlayer != player || refreshPlayerData)
            {
                refreshPlayerData = false;
                player = GameMain.mainPlayer;
                Mecha mecha = player.mecha;
                ChangeValueArray[0] = mecha.walkSpeed;
                ChangeValueArray[1] = GameMain.history.techSpeed;
                ChangeValueArray[2] = mecha.droneSpeed;
                ChangeValueArray[3] = mecha.droneMovement;
                ChangeValueArray[4] = mecha.droneCount;
                ChangeValueArray[5] = mecha.miningSpeed;
                ChangeValueArray[6] = mecha.replicateSpeed;
                ChangeValueArray[7] = mecha.maxSailSpeed / 1000;
                ChangeValueArray[8] = mecha.maxWarpSpeed / 480000;
                ChangeValueArray[9] = mecha.jumpSpeed;
                ChangeValueArray[10] = mecha.buildArea;
                ChangeValueArray[11] = (float)mecha.reactorPowerGen;
                ChangeValueArray[12] = historyData.logisticDroneSpeedScale > 0 ? historyData.logisticDroneSpeedScale : 3;
                ChangeValueArray[13] = historyData.logisticShipSpeedScale > 0 ? historyData.logisticShipSpeedScale : 5;
                ChangeValueArray[14] = historyData.logisticDroneCarries > 0 ? historyData.logisticDroneCarries : 100;
                ChangeValueArray[15] = historyData.logisticShipCarries > 0 ? historyData.logisticShipCarries : 100;
                ChangeValueArray[16] = historyData.miningSpeedScale;
                ChangeValueArray[17] = historyData.inserterStackCount;
                ChangeValueArray[18] = historyData.labLevel;
                ChangeValueArray[19] = historyData.storageLevel > 4 ? 4 : historyData.storageLevel;
                packageitemlist = new List<int[]>();
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
                right = 100;
                str = "采矿速度";
                return;
            }
            if (i == 6)
            {
                left = 1;
                right = 100;
                str = "制造速度";
                return;
            }
            if (i == 7)
            {
                left = 1;
                right = 10;
                str = "最大航行速度";
                return;
            }
            if (i == 8)
            {
                left = 1;
                right = 100;
                str = "最大曲速";
                return;
            }
            if (i == 9)
            {
                left = 32;
                right = 3200;
                str = "跳跃速度";
                return;
            }
            if (i == 10)
            {
                left = 80;
                right = 400;
                str = "建造范围";
                return;
            }
            if (i == 11)
            {
                left = 1000000;
                right = 500000000;
                str = "核心功率" + "(W)";
                return;
            }
            if (i == 12)
            {
                left = 1;
                right = 100;
                str = "运输机速度";
                return;
            }
            if (i == 13)
            {
                left = 1;
                right = 100;
                str = "运输船速度";
                return;
            }
            if (i == 14)
            {
                left = 1;
                right = 10000;
                str = "运输机载量";
                return;
            }
            if (i == 15)
            {
                left = 100;
                right = 100000;
                str = "运输船载量";
                return;
            }
            if (i == 16)
            {
                left = 1;
                right = 100;
                str = "采矿机速度倍率";
                return;
            }
            if (i == 17)
            {
                left = 1;
                right = 100;
                str = "极速分拣器数量";
                return;
            }
            if (i == 18)
            {
                left = 2;
                right = 100;
                str = "建筑堆叠高度";
                return;
            }
            if (i == 19)
            {
                left = 1;
                right = 20;
                str = "货物集装数量";
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
                if (valueId == 14)
                {
                    ChangeValueArray[valueId] = (int)(value / 50) * 50;
                }
                else if (valueId == 15)
                {
                    ChangeValueArray[valueId] = (int)(value / 500) * 500;
                }
                switch (valueId)
                {
                    case 0: SetWalkSpeed(); break;
                    case 1: SetTechSpeed(); break;
                    case 2: SetDroneSpeed(); break;
                    case 3: SetDroneMoveMent(); break;
                    case 4: SetDroneCount(); break;
                    case 5: SetminingSpeed(); break;
                    case 6: SetreplicateSpeed(); break;
                    case 7: SetMaxsailSpeed(); break;
                    case 8: SetMaxWarpSpeed(); break;
                    case 9: SetJumpSpeed(); break;
                    case 10: SetBuildarea(); break;
                    case 11: SetreactorPowerGen(); break;
                    case 12: SetlogisticDroneSpeedScale(); break;
                    case 13: SetlogisticShipSpeedScale(); break;
                    case 14: SetlogisticDroneCarries(); break;
                    case 15: SetlogisticShipCarries(); break;
                    case 16: SetminingSpeedScale(); break;
                    case 17: SetInserterStack(); break;
                    case 18: SetBuildStack(); break;
                    case 19: SetStationPilerStack(); break;
                }
            }

        }

        public void BluePrintpasteNoneed()
        {
            if (!blueprintpastenoneed_bool.Value || GameMain.localPlanet == null || GameMain.localPlanet.factory == null) return;
            for (int i = 0; i < GameMain.localPlanet.factory.prebuildPool.Length; i++)
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
                            if (sc.isStellar && Station_infiniteWarp_bool.Value)
                                sc.warperCount = 50;
                            for (int i = 0; i < sc.storage.Length; i++)
                            {
                                if (StationfullCount && sc.storage[i].itemId > 0)
                                {
                                    sc.storage[i].count = sc.storage[i].max;
                                }
                                if (StationStoExtra.Value >= 0)
                                {
                                    if (sc.storage[i].max == 10000 + GameMain.history.remoteStationExtraStorage)
                                    {
                                        sc.storage[i].max += StationStoExtra.Value * 10000;
                                    }
                                    if (RefreshStationStorage && sc.storage[i].max % 10000 == 0 && sc.storage[i].max != 0)
                                    {
                                        sc.storage[i].max = 10000 + GameMain.history.remoteStationExtraStorage + StationStoExtra.Value * 10000;
                                    }
                                    if (sc.isCollector)
                                    {
                                        if (sc.storage[i].max == 5000)
                                        {
                                            sc.storage[i].max += StationStoExtra.Value * 5000;
                                        }
                                        if (RefreshStationStorage && sc.storage[i].max % 5000 == 0 && sc.storage[i].max != 0)
                                        {
                                            sc.storage[i].max = 5000 + GameMain.history.localStationExtraStorage + StationStoExtra.Value * 5000;
                                        }
                                    }
                                }
                            }
                            if (sc.name != null)
                            {
                                if (StationfullCount_bool.Value && sc.name.Equals("星球无限供货机"))
                                {
                                    for (int i = 0; i < sc.storage.Length; i++)
                                    {
                                        if (sc.storage[i].itemId > 0)
                                        {
                                            sc.storage[i].count = sc.storage[i].max;
                                        }
                                    }
                                }
                                if (StationMiner.Value && (sc.name.Equals("Station_miner") || sc.name.Equals("星球矿机")))
                                {
                                    int lastIndex = sc.storage.Length != 3 ? 4 : 2;
                                    if ((Time.time - StationMinerTime) > 1)
                                    {
                                        for (int i = 0; i < sc.storage.Length && sc.energy > 0; i++)
                                        {
                                            int itemId = sc.storage[i].itemId;
                                            if (itemId > 0 && sc.storage[i].count <= sc.storage[i].max)
                                            {
                                                int veinNumbers = GetNumberOfVein(itemId, pdId);
                                                int pointminenum = veinNumbers * Stationminenumber.Value;
                                                if (veinNumbers > 0 && pointminenum == 0) pointminenum = 1;
                                                else if (veinNumbers == 0 && itemId != GameMain.galaxy.PlanetById(pdId).waterItemId) continue;

                                                if (Stationfullenergy.Value || sc.energy - pointminenum * GameMain.history.miningSpeedScale * 5000 > 0)
                                                {
                                                    int minenum = StationMine(itemId, pointminenum, pdId);
                                                    AddStatInfo(pdId, itemId, minenum, true);
                                                    sc.AddItem(itemId, minenum, 0);
                                                    if (!Stationfullenergy.Value)
                                                        sc.energy -= minenum * 5000;
                                                }
                                            }
                                        }
                                    }
                                    if (sc.storage[lastIndex].itemId > 0 && sc.storage[lastIndex].count > 0)
                                    {
                                        long heatValue = LDB.items.Select(sc.storage[lastIndex].itemId).HeatValue;
                                        if (heatValue > 0 && sc.energyMax > sc.energy + heatValue)
                                        {
                                            sc.energy += heatValue;
                                            sc.storage[lastIndex].count--;
                                            AddStatInfo(pdId, sc.storage[lastIndex].itemId, 1, false);
                                        }
                                    }
                                }

                                if (StationTrash.Value && (sc.name.Equals("垃圾站") || sc.name.Equals("Station_trash")))
                                {
                                    for (int i = 0; i < sc.storage.Length && sc.energy > 0; i++)
                                    {
                                        if (sc.storage[i].itemId > 0)
                                        {
                                            int trashnum = sc.storage[i].count;
                                            if (sc.energy - trashnum * 10000 > 0)
                                            {
                                                AddStatInfo(pdId, sc.storage[i].itemId, trashnum, false);
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
            pd.CalcVeinAmounts(ref veinAmounts);
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
        public void AddStatInfo(int planetId, int itemId, int itemNum, bool produce = true)
        {
            PlanetData pd = GameMain.galaxy.PlanetById(planetId);
            if (pd == null || pd.factory == null) return;
            addStatQueue.Enqueue(new int[] { pd.factoryIndex, itemId, itemNum, produce ? 1 : 0 });
        }


        #region 矿脉管理
        public void ControlVein()
        {
            if (player == null) return;
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
            if ((addveinbool || changeveingroupposbool.Value || changeveinposbool.Value) && Input.GetMouseButton(1))
            {
                addveinbool = false;
                changeveingroupposbool.Value = false;
                //changeveinposbool.Value = false;
            }
            if (Input.GetMouseButton(0) && !player.controller.actionBuild.dismantleTool.active)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    pointveindata = getveinbymouse();
                }
                if (pointveindata.amount != 0)
                {
                    if (getallVein_bool.Value)
                    {
                        getallVein(pointveindata);
                    }
                    else if (changeveingroupposbool.Value)
                    {
                        changeveingrouppos(pointveindata);
                    }
                    else if (changexveinspos.Value)
                    {
                        changexveins(pointveindata);
                    }
                    else if (changeveinposbool.Value)
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
            if (pd == null || pd.type == EPlanetType.Gas) return;
            RaycastHit raycastHit1;
            if (pd == null || !Physics.Raycast(player.controller.mainCamera.ScreenPointToRay(Input.mousePosition), out raycastHit1, 800f, 8720, (QueryTriggerInteraction)2))
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
            if (pd == null || pd.type == EPlanetType.Gas) return new VeinData();
            RaycastHit raycastHit1;
            if (pd == null || !Physics.Raycast(player.controller.mainCamera.ScreenPointToRay(Input.mousePosition), out raycastHit1, 800f, 8720, (QueryTriggerInteraction)2))
                return new VeinData();
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
                    for (int i = 0; i < GameMain.localPlanet.factory.vegePool.Length; i++)
                        planet.factory.RemoveVegeWithComponents(i);
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
                    planet.factory = planetFactory;
                    planet.factoryIndex = index;
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
            player.SetSandCount(10000000);
        }

        public void Addpackagesize()
        {
            if (player == null) return;
            player.package.SetSize(player.package.size + UIRoot.instance.uiGame.inventory.colCount);
        }

        public void Reducepackagesize()
        {
            if (player == null) return;
            player.package.SetSize(player.package.grids.Length - UIRoot.instance.uiGame.inventory.colCount);
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
                                GameMain.history.UnlockTechFunction(techProto.UnlockFunctions[index], techProto.UnlockValues[index], level);
                        }
                    }
                    if (!GameMain.history.techStates[techProto.ID].unlocked)
                    {
                        for (int i = 0; i < techProto.itemArray.Length; i++)
                        {
                            AddComsumeItemtoTotal(techProto.Items[i], (int)(techProto.ItemPoints[i] * techProto.GetHashNeeded(techProto.Level) / 3600));
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



            //配方信息查询
            foreach (RecipeProto rp in RecipeList)
            {
                if (allhandcraft.Value)
                {
                    rp.Handcraft = true;
                }

                if (quickproduce.Value)
                {
                    rp.TimeSpend = 1;
                }
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

        public void Changesmelt()
        {
            for (int i = 0; i < RecipeList.Count; ++i)
            {
                if (RecipeList[i].Type == ERecipeType.Smelt)
                {
                    for (int j = 0; j < RecipeList[i].ItemCounts.Length; ++j)
                        RecipeList[i].ItemCounts[j] *= MULTIPELSMELT.Value;
                    for (int j = 0; j < RecipeList[i].ResultCounts.Length; ++j)
                        RecipeList[i].ResultCounts[j] *= MULTIPELSMELT.Value;
                }
            }
        }

        /// <summary>
        /// 物品相关修改
        /// </summary>
        public void ChangeItemstack()
        {
            foreach (ItemProto ip in itemProtos)
            {
                //Debug.Log(ip.ID + " " + ip.Name);
                //ip.StackSize *= StackMultiple.Value;
                //if (ip.prefabDesc!=null)
                //{
                //    Debug.Log(ip.name+" "+ ip.prefabDesc.isInserter);
                //}
                //if (ip.CanBuild)
                //{
                //    ip.BuildInGas = true;
                //    if (ip.prefabDesc != null)
                //    {
                //        ip.prefabDesc.workEnergyPerTick = 0;
                //        ip.prefabDesc.idleEnergyPerTick = 0;
                //    }
                //}
                //string t = ip.Description + ";" + ip.ID + ";" + ip.GridIndex + ";" + ip.HeatValue + ";" + ip.Potential + ";" + ip.StackSize;

                //Logger.LogInfo(t);
            }
        }

        public void InfiniteAllThingInPackage()
        {
            if (!Infinitething.Value) return;
            if (player.package.grids.Length < itemProtos.Count)
            {
                player.package.SetSize((itemProtos.Count / 10 + 1) * 10);
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
            player.SetSandCount(10000000);
        }

        public void play_with_trash()
        {
            if (player == null) return;
            StorageComponent.GRID[] grids = player.package.grids;
            for (int i = 0; i < 10; i++)
            {
                if (grids[grids.Length - 1 - i].count != 0)
                {
                    player.ThrowTrash(grids[grids.Length - 1 - i].itemId, 1, 0, 0);
                }
                else
                {
                    break;
                }
            }
        }

        public void Playwithtrashset()
        {
            if (playwithtrash)
            {
                if (!trashplaying)
                {
                    playtrashtime = (int)playwithtrashtimemult;
                    InvokeRepeating("play_with_trash", 1, 1 / playwithtrashtimemult);
                    trashplaying = true;
                }
                if (playtrashtime != (int)playwithtrashtimemult)
                {
                    CancelInvoke("play_with_trash");
                    trashplaying = false;
                }
            }
            else if (trashplaying)
            {
                CancelInvoke("play_with_trash");
                trashplaying = false;
            }
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
            if (player != null && player.mecha != null)
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

        public void SetWalkSpeed()
        {
            if (player != null && player.mecha != null)
            {
                GameMain.mainPlayer.mecha.walkSpeed = ChangeValueArray[0];
            }
        }
        public void SetTechSpeed()
        {
            if (GameMain.history != null)
            {
                GameMain.history.techSpeed = (int)ChangeValueArray[1];
            }
        }
        public void SetDroneSpeed()
        {
            if (player != null && player.mecha != null)
            {
                GameMain.mainPlayer.mecha.droneSpeed = ChangeValueArray[2];
            }
        }
        public void SetDroneMoveMent()
        {
            if (player != null && player.mecha != null)
            {
                player.mecha.droneMovement = (int)ChangeValueArray[3];
            }
        }
        public void SetDroneCount()
        {
            if (player != null && player.mecha != null)
            {
                player.mecha.droneCount = (int)ChangeValueArray[4];
            }
        }
        public void SetminingSpeed()
        {
            if (player != null && player.mecha != null)
            {
                player.mecha.miningSpeed = (int)ChangeValueArray[5];
            }
        }
        public void SetreplicateSpeed()
        {
            if (player != null && player.mecha != null)
            {
                player.mecha.replicateSpeed = (int)ChangeValueArray[6];
            }
        }
        public void SetMaxsailSpeed()
        {
            if (player != null && player.mecha != null)
            {
                player.mecha.maxSailSpeed = (int)ChangeValueArray[7] * 1000;
            }
        }
        public void SetMaxWarpSpeed()
        {
            if (player != null && player.mecha != null)
            {
                player.mecha.maxWarpSpeed = (int)ChangeValueArray[8] * 1000 * 480;
            }
        }
        public void SetJumpSpeed()
        {
            if (player != null && player.mecha != null)
            {
                player.mecha.jumpSpeed = (int)ChangeValueArray[9];
            }
        }
        public void SetBuildarea()
        {
            if (player != null && player.mecha != null)
            {
                player.mecha.buildArea = ChangeValueArray[10];
            }
        }
        public void SetreactorPowerGen()
        {
            if (player != null && player.mecha != null)
            {
                player.mecha.reactorPowerGen = ChangeValueArray[11];
            }
        }
        public void SetminingSpeedScale()
        {
            if (GameMain.history != null)
            {
                GameMain.history.miningSpeedScale = ChangeValueArray[16];
            }
        }
        public void SetlogisticShipCarries()
        {
            if (GameMain.history != null)
            {
                GameMain.history.logisticShipCarries = (int)(ChangeValueArray[15] / 100) * 100;
            }
        }
        public void SetlogisticDroneCarries()
        {
            if (GameMain.history != null)
            {
                GameMain.history.logisticDroneCarries = (int)(ChangeValueArray[14] / 50) * 50;
            }
        }
        public void SetlogisticDroneSpeedScale()
        {
            if (GameMain.history != null)
            {
                GameMain.history.logisticDroneSpeedScale = (int)ChangeValueArray[12];
            }
        }
        public void SetlogisticShipSpeedScale()
        {
            if (GameMain.history != null)
            {
                GameMain.history.logisticShipSpeedScale = (int)ChangeValueArray[13];
            }
        }
        public void SetInserterStack()
        {
            if (player == null) return;
            GameMain.history.inserterStackCount = (int)ChangeValueArray[17];
        }
        public void SetBuildStack()
        {
            if (player == null) return;
            GameMain.history.storageLevel = (int)ChangeValueArray[18];
            GameMain.history.labLevel = (int)ChangeValueArray[18];
        }
        public void SetStationPilerStack()
        {
            if (player == null) return;
            GameMain.history.stationPilerLevel = (int)ChangeValueArray[19];
        }
        #endregion

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