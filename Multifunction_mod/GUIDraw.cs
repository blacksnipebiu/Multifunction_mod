using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using static Multifunction_mod.Multifunction;
using static Multifunction_mod.Multifunctionpatch;
using Vector2 = UnityEngine.Vector2;

namespace Multifunction_mod
{
    public class GUIDraw
    {
        private bool firstDraw;
        private string stackmultipleStr;
        private string multipelsmeltStr;
        private Multifunction MainFunction;
        private int whichpannel;
        private Vector2 scrollPosition;
        private Vector2 TabscrollPosition;
        private GameObject ui_MultiFunctionPanel;
        private bool RefreshBaseSize;
        private bool englishShow;
        private bool ChangePlayerbool;
        private int baseSize;
        private int heightdis;
        private float mainWindow_x;
        private float mainWindow_y;
        private float mainWindowWidth;
        private float mainWindowHeight;
        private string[] menus;
        private Color textColor;
        private Color mainWindowTextureColor;
        private Texture2D mainWindowTexture;
        public bool ColorChanged;
        public bool MouseInWindow;
        public int BaseSize
        {
            get => baseSize;
            set
            {
                baseSize = value;
                scale.Value = value;
                RefreshBaseSize = true;
                heightdis = value * 2;
            }
        }
        public float MainWindow_x
        {
            get => mainWindow_x;
            set
            {
                mainWindow_x = value;
                Multifunction.MainWindow_x_config.Value = value;
            }
        }
        public float MainWindow_y
        {
            get => mainWindow_y;
            set
            {
                mainWindow_y = value;
                Multifunction.MainWindow_y_config.Value = value;
            }
        }
        public float MainWindowWidth
        {
            get => mainWindowWidth;
            set
            {
                mainWindowWidth = value;
                Multifunction.MainWindow_width.Value = value;
            }
        }
        public float MainWindowHeight
        {
            get => mainWindowHeight;
            set
            {
                mainWindowHeight = value;
                Multifunction.MainWindow_height.Value = value;
            }
        }
        public bool DisplayingWindow { get; private set; }
        public Color TextColor
        {
            get => textColor;
            set
            {
                ColorChanged = true;
                textColor = value;
                Multifunction.Textcolor.Value = value;
            }
        }
        public Color MainWindowTextureColor
        {
            get => mainWindowTextureColor;
            set
            {
                mainWindowTextureColor = value;
                mainWindowTextureColor_config.Value = value;
            }
        }
        public Texture2D MainWindowTexture
        {
            get => mainWindowTexture;
            set
            {
                mainWindowTexture = value;
            }
        }


        GUIStyle style;
        GUIStyle normalStyle;
        GUIStyle selectedIconStyle;
        Texture2D selectedTexture;
        private GUIStyle labelmarginStyle;
        private GUIStyle selectedButtonStyle;
        private GUILayoutOption[] slideroptions;
        private GUILayoutOption[] textfieldoptions;
        private GUILayoutOption[] menusbuttonoptions;
        private GUILayoutOption[] iconbuttonoptions;
        private float MainWindow_x_move;
        private float MainWindow_y_move;
        private float temp_MainWindow_x;
        private float temp_MainWindow_y;
        private bool moving;
        private bool leftscaling;
        private bool rightscaling;
        private bool bottomscaling;

        public GUIDraw(int baseSize, GameObject panel, Multifunction multifunction)
        {
            BaseSize = baseSize;
            ui_MultiFunctionPanel = panel;
            MainFunction = multifunction;
            Init();
        }

        void Init()
        {
            whichpannel = 1;
            scrollPosition[0] = 0;
            TabscrollPosition[0] = 0;
            RefreshBaseSize = true;
            ColorChanged = true;
            stackmultipleStr = StackMultiple.Value.ToString();
            multipelsmeltStr = MULTIPELSMELT.Value.ToString();
            menus = new string[7] { "", "人物", "建筑", "星球", "戴森球", "其它功能", "机甲物流" };
            TextColor = Textcolor.Value;
            MainWindowTextureColor = mainWindowTextureColor_config.Value;
            MainWindowTexture = new Texture2D(10, 10);
            selectedTexture = new Texture2D(1, 1);
            selectedTexture.SetPixel(0, 0, new Color(50f / 255, 50f / 255, 50f / 255));
            selectedTexture.Apply();
            mainWindowTexture.SetPixels(Enumerable.Repeat(MainWindowTextureColor, 100).ToArray());
            mainWindowTexture.Apply();
            mainWindowWidth = MainWindow_width.Value;
            mainWindowHeight = MainWindow_height.Value;
            textColor = Textcolor.Value;
            MainWindow_x = MainWindow_x_config.Value;
            MainWindow_y = MainWindow_y_config.Value;
        }

        public void MainWindowKeyInvoke()
        {
            DisplayingWindow = !DisplayingWindow;
            if (DisplayingWindow)
            {
                firstDraw = true;
            }
            ui_MultiFunctionPanel.SetActive(DisplayingWindow);
        }

        public void OpenMainWindow()
        {
            DisplayingWindow = true;
            ui_MultiFunctionPanel.SetActive(true);
        }

        public void CloseMainWindow()
        {
            DisplayingWindow = false;
            ui_MultiFunctionPanel.SetActive(false);
        }

        public void Draw()
        {
            if (firstDraw)
            {
                firstDraw = false;
                BaseSize = GUI.skin.label.fontSize;
            }
            englishShow = Localization.language != Language.zhCN;
            if (DisplayingWindow)
            {
                if (Input.GetKey(KeyCode.LeftControl))
                {
                    int t = (int)(Input.GetAxis("Mouse Wheel") * 10);
                    int temp = BaseSize + t;
                    if (Input.GetKeyDown(KeyCode.UpArrow)) { temp++; }
                    if (Input.GetKeyDown(KeyCode.DownArrow)) { temp--; }
                    temp = Math.Max(5, Math.Min(temp, 35));
                    BaseSize = temp;
                }
            }
            if (RefreshBaseSize)
            {
                RefreshBaseSize = false;
                GUI.skin.label.fontSize = BaseSize;
                GUI.skin.button.fontSize = BaseSize;
                GUI.skin.toggle.fontSize = BaseSize;
                GUI.skin.textField.fontSize = BaseSize;
                GUI.skin.textArea.fontSize = BaseSize;
                if (style != null)
                {
                    style.fontSize = BaseSize;
                    labelmarginStyle.fontSize = BaseSize;
                }
                selectedButtonStyle = new GUIStyle(GUI.skin.button);
                selectedButtonStyle.normal.textColor = new Color32(215, 186, 245, 255);
                slideroptions = new[] { GUILayout.Width(heightdis * 6) };
                textfieldoptions = new[] { GUILayout.Width(heightdis * 3) };
                menusbuttonoptions = new[] { GUILayout.Height(heightdis), GUILayout.Width(heightdis * 4) };
                iconbuttonoptions = new[] { GUILayout.Height(heightdis), GUILayout.Width(heightdis) };
            }
            if (ColorChanged)
            {
                ColorChanged = false;
                GUI.skin.button.normal.textColor = TextColor;
                GUI.skin.textArea.normal.textColor = TextColor;
                GUI.skin.textField.normal.textColor = TextColor;
                GUI.skin.toggle.normal.textColor = TextColor;
                GUI.skin.toggle.onNormal.textColor = TextColor;
                style = new GUIStyle
                {
                    wordWrap = true,
                    fontSize = baseSize - 2,
                };
                style.normal.textColor = TextColor;
                normalStyle = new GUIStyle();
                selectedIconStyle = new GUIStyle();
                selectedIconStyle.normal.background = selectedTexture;
                labelmarginStyle = new GUIStyle(style)
                {
                    margin = new RectOffset(0, 0, 3, 0)
                };
            }
            if (DisplayingWindow)
            {
                UIPanelSet();
                MoveWindow();
                Scaling_window();
                Rect windowRect1 = new Rect(MainWindow_x, MainWindow_y, MainWindowWidth, MainWindowHeight);
                GUI.DrawTexture(windowRect1, mainWindowTexture);
                GUI.Window(20210218, windowRect1, MainWindow, "OP面板".getTranslate() + "(" + VERSION + ")" + "ps:ctrl+↑↓");
                if (MainWindow_x < 0 || MainWindow_x > Screen.width)
                {
                    MainWindow_x = 100;
                }
                if (MainWindow_y < 0 || MainWindow_y > Screen.height)
                {
                    MainWindow_y = 100;
                }
            }
        }

        private void UIPanelSet()
        {
            var rt = ui_MultiFunctionPanel.GetComponent<RectTransform>();
            var Canvasrt = UIRoot.instance.overlayCanvas.GetComponent<RectTransform>();
            float CanvaswidthMultiple = Canvasrt.sizeDelta.x * 1.0f / Screen.width;
            float CanvasheightMultiple = Canvasrt.sizeDelta.y * 1.0f / Screen.height;
            rt.sizeDelta = new Vector2(CanvaswidthMultiple * MainWindowWidth, CanvasheightMultiple * MainWindowHeight);
            rt.localPosition = new Vector2(-Canvasrt.sizeDelta.x / 2 + MainWindow_x * CanvaswidthMultiple, Canvasrt.sizeDelta.y / 2 - MainWindow_y * CanvasheightMultiple - rt.sizeDelta.y);
        }

        #region 窗口UI

        /// <summary>
        /// 主控面板
        /// </summary>
        /// <param name="winId"></param>
        void MainWindow(int winId)
        {
            GUILayout.BeginArea(new Rect(10, 20, MainWindowWidth, MainWindowHeight));
            {
                GUILayout.BeginVertical();

                GUILayout.BeginHorizontal();
                for (int i = 1; i <= 6; i++)
                {
                    if (whichpannel == i)
                    {
                        GUILayout.Button(menus[i].getTranslate(), selectedButtonStyle, menusbuttonoptions);
                    }
                    else
                    {
                        if (GUILayout.Button(menus[i].getTranslate(), menusbuttonoptions)) { whichpannel = i; }
                    }
                }
                GUILayout.EndHorizontal();

                #region 面板内容
                scrollPosition = GUILayout.BeginScrollView(scrollPosition, new[] { GUILayout.Width(MainWindowWidth - 20), GUILayout.Height(MainWindowHeight - heightdis * 2) });
                switch (whichpannel)
                {
                    case 1:
                        PlayerPannel();
                        break;
                    case 2:
                        BuildPannel();
                        break;
                    case 3:
                        PlanetPannel();
                        break;
                    case 4:
                        DysonPannel();
                        break;
                    case 5:
                        OtherPannel();
                        break;
                    case 6:
                        LogisticsPannel();
                        break;
                }
                GUILayout.EndScrollView();
                #endregion
                GUILayout.EndVertical();
            }


            GUILayout.EndArea();

        }

        /// <summary>
        /// 属性条渲染
        /// </summary>
        /// <param name="num"></param>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <param name="propertyName"></param>
        /// <param name="resulttype"></param>
        /// <returns></returns>
        public float PropertyDataUIDraw(float num, float left, float right, string propertyName, int resulttype = 0)
        {
            GUILayout.BeginHorizontal();
            float temp = GUILayout.HorizontalSlider(num, left, right, slideroptions);
            string result;
            if (resulttype == 0)
            {
                result = (int)temp + "";
            }
            else if (resulttype == 1)
            {
                result = TGMKinttostring(GameMain.mainPlayer.mecha.reactorPowerGen, "W");
            }
            else
            {
                result = string.Format("{0:N2}", temp);
            }
            string t = GUILayout.TextField(result, textfieldoptions);
            if (t != result)
            {
                if (resulttype != 0)
                {
                    float.TryParse(Regex.Replace(t, @"^[^0-9]+(.[^0-9]{2})?$", ""), out temp);
                }
                else
                {
                    float.TryParse(Regex.Replace(t, @"^[^0-9]", ""), out temp);
                }
            }
            GUILayout.Label(propertyName.getTranslate(), labelmarginStyle);

            GUILayout.EndHorizontal();
            temp = Math.Max(left, Math.Min(right, temp));
            return temp;
        }

        /// <summary>
        /// 玩家面板
        /// </summary>
        void PlayerPannel()
        {
            //左侧UI
            GUILayout.BeginHorizontal();

            GUILayout.BeginVertical(new[] { GUILayout.Width(englishShow ? 18 * heightdis : 14 * heightdis) });
            if (GameMain.mainPlayer != null && propertyData != null)
            {
                var propertydata = propertyData;
                if (ChangePlayerbool)
                {
                    propertydata.WalkSpeed = PropertyDataUIDraw(propertydata.WalkSpeed, 1, 100, "走路速度", 2);
                    propertydata.TechSpeed = PropertyDataUIDraw(propertydata.TechSpeed, 1, 10000, "研发速度");
                    propertydata.DroneSpeed = PropertyDataUIDraw(propertydata.DroneSpeed, 1, 100, "小飞机速度");
                    propertydata.DroneMovement = PropertyDataUIDraw(propertydata.DroneMovement, 1, 100, "小飞机任务点数");
                    propertydata.DroneCount = PropertyDataUIDraw(propertydata.DroneCount, 1, 400, "小飞机数量");
                    propertydata.MaxSailSpeed = PropertyDataUIDraw(propertydata.MaxSailSpeed, 1, 10, "最大航行速度");
                    propertydata.MaxWarpSpeed = PropertyDataUIDraw(propertydata.MaxWarpSpeed, 1, 100, "最大曲速");
                    propertydata.BuildArea = PropertyDataUIDraw(propertydata.BuildArea, 80, 400, "建造范围");
                    propertydata.ReactorPowerGen = PropertyDataUIDraw(propertydata.ReactorPowerGen, 800_000, 500_000_000, "核心功率", 1);
                    propertydata.LogisticCourierSpeed = PropertyDataUIDraw(propertydata.LogisticCourierSpeed, 1, 100, "配送机速度");
                    propertydata.LogisticDroneSpeedScale = PropertyDataUIDraw(propertydata.LogisticDroneSpeedScale, 1, 100, "运输机速度", 2);
                    propertydata.LogisticShipSpeedScale = PropertyDataUIDraw(propertydata.LogisticShipSpeedScale, 1, 100, "运输船速度", 2);
                    propertydata.LogisticCourierCarries = PropertyDataUIDraw(propertydata.LogisticCourierCarries, 1, 10000, "配送机载量");
                    propertydata.LogisticDroneCarries = PropertyDataUIDraw(propertydata.LogisticDroneCarries, 20, 10000, "运输机载量");
                    propertydata.LogisticShipCarries = PropertyDataUIDraw(propertydata.LogisticShipCarries, 100, 100000, "运输船载量");
                    propertydata.MiningSpeedScale = PropertyDataUIDraw(propertydata.MiningSpeedScale, 1, 100, "采矿机速度倍率", 2);
                    propertydata.InserterStackCount = PropertyDataUIDraw(propertydata.InserterStackCount, 1, 100, "极速分拣器数量");
                    propertydata.LabLevel = PropertyDataUIDraw(propertydata.LabLevel, 2, 100, "建筑堆叠高度");
                    propertydata.StorageLevel = PropertyDataUIDraw(propertydata.StorageLevel, 1, 20, "货物集装数量");
                    propertydata.StackSizeMultiplier = PropertyDataUIDraw(propertydata.StackSizeMultiplier, 1, 20, "物流背包堆叠倍率");
                }
                else
                {
                    PropertyDataUIDraw(propertydata.WalkSpeed, 1, 100, "走路速度", 2);
                    PropertyDataUIDraw(propertydata.TechSpeed, 1, 10000, "研发速度");
                    PropertyDataUIDraw(propertydata.DroneSpeed, 1, 100, "小飞机速度");
                    PropertyDataUIDraw(propertydata.DroneMovement, 1, 100, "小飞机任务点数");
                    PropertyDataUIDraw(propertydata.DroneCount, 1, 400, "小飞机数量");
                    PropertyDataUIDraw(propertydata.MaxSailSpeed, 1, 10, "最大航行速度");
                    PropertyDataUIDraw(propertydata.MaxWarpSpeed, 1, 100, "最大曲速");
                    PropertyDataUIDraw(propertydata.BuildArea, 80, 400, "建造范围");
                    PropertyDataUIDraw(propertydata.ReactorPowerGen, 1000000, 500000000, "核心功率", 1);
                    PropertyDataUIDraw(propertydata.LogisticCourierSpeed, 1, 100, "配送机速度");
                    PropertyDataUIDraw(propertydata.LogisticDroneSpeedScale, 1, 100, "运输机速度", 2);
                    PropertyDataUIDraw(propertydata.LogisticShipSpeedScale, 1, 100, "运输船速度", 2);
                    PropertyDataUIDraw(propertydata.LogisticCourierCarries, 1, 10000, "配送机载量");
                    PropertyDataUIDraw(propertydata.LogisticDroneCarries, 20, 10000, "运输机载量");
                    PropertyDataUIDraw(propertydata.LogisticShipCarries, 100, 100000, "运输船载量");
                    PropertyDataUIDraw(propertydata.MiningSpeedScale, 1, 100, "采矿机速度倍率", 2);
                    PropertyDataUIDraw(propertydata.InserterStackCount, 1, 100, "极速分拣器数量");
                    PropertyDataUIDraw(propertydata.LabLevel, 2, 100, "建筑堆叠高度");
                    PropertyDataUIDraw(propertydata.StorageLevel, 1, 20, "货物集装数量");
                    PropertyDataUIDraw(propertydata.StackSizeMultiplier, 1, 20, "物流背包堆叠倍率");
                }
            }
            GUILayout.EndVertical();

            //右侧UI
            GUILayout.BeginVertical();
            {
                Infinitething.Value = GUILayout.Toggle(Infinitething.Value, "无限物品".getTranslate());
                InfiniteSand.Value = GUILayout.Toggle(InfiniteSand.Value, "无限沙土".getTranslate());
                lockpackage_bool.Value = GUILayout.Toggle(lockpackage_bool.Value, "锁定背包".getTranslate());
                Property9999999 = GUILayout.Toggle(Property9999999, "无限元数据".getTranslate());
                DroneNoenergy_bool.Value = GUILayout.Toggle(DroneNoenergy_bool.Value, "小飞机不耗能".getTranslate());
                Infiniteplayerpower.Value = GUILayout.Toggle(Infiniteplayerpower.Value, "无限机甲能量".getTranslate());
                unlockpointtech = GUILayout.Toggle(unlockpointtech, "科技点击解锁".getTranslate());
                dismantle_but_nobuild.Value = GUILayout.Toggle(dismantle_but_nobuild.Value, "不往背包放东西".getTranslate());
                if (QuickHandcraft.Value != GUILayout.Toggle(QuickHandcraft.Value, "机甲制造MAX".getTranslate()))
                {
                    QuickHandcraft.Value = !QuickHandcraft.Value;
                    if (QuickHandcraft.Value)
                    {
                        GameMain.mainPlayer.mecha.replicateSpeed = 100;
                    }
                    else
                    {
                        GameMain.mainPlayer.mecha.replicateSpeed = Configs.freeMode.mechaMiningSpeed;
                    }
                }
                if (QuickPlayerMine.Value != GUILayout.Toggle(QuickPlayerMine.Value, "机甲采矿MAX".getTranslate()))
                {
                    QuickPlayerMine.Value = !QuickPlayerMine.Value;
                    if (QuickPlayerMine.Value)
                    {
                        GameMain.mainPlayer.mecha.miningSpeed = 100;
                    }
                    else
                    {
                        GameMain.mainPlayer.mecha.miningSpeed = Configs.freeMode.mechaReplicateSpeed;
                    }
                }
                isInstantItem.Value = GUILayout.Toggle(isInstantItem.Value, "直接获取物品".getTranslate());
                noneedwarp.Value = GUILayout.Toggle(noneedwarp.Value, "无需翘曲器曲速飞行".getTranslate());
            }
            {
                if (GUILayout.Button((ChangePlayerbool ? "停止修改" : "启动修改").getTranslate())) ChangePlayerbool = !ChangePlayerbool;
                if (GUILayout.Button("清空背包".getTranslate())) MainFunction.Clearpackage();
                if (GUILayout.Button("初始化玩家".getTranslate())) MainFunction.InitPlayer();

                GUILayout.BeginHorizontal();
                GUILayout.Button("背包".getTranslate(), GUILayout.Height(heightdis * 2));
                GUILayout.BeginVertical();
                if (GUILayout.Button("加一行".getTranslate(), GUILayout.Height(heightdis))) MainFunction.Operatepackagesize(1, 0);
                if (GUILayout.Button("加一列".getTranslate(), GUILayout.Height(heightdis))) MainFunction.Operatepackagesize(0, 1);
                GUILayout.EndVertical();
                GUILayout.BeginVertical();
                if (GUILayout.Button("减一行".getTranslate(), GUILayout.Height(heightdis))) MainFunction.Operatepackagesize(1, 0, false);
                if (GUILayout.Button("减一列".getTranslate(), GUILayout.Height(heightdis))) MainFunction.Operatepackagesize(0, 1, false);
                GUILayout.EndVertical();
                GUILayout.EndHorizontal();

                if (GUILayout.Button("解锁全部科技".getTranslate() + "(" + "补充原料".getTranslate() + ")")) MainFunction.UnlockallTech(true);
                if (GUILayout.Button("解锁全部科技".getTranslate())) MainFunction.UnlockallTech();
                if (GUILayout.Button("回退无穷科技".getTranslate())) MainFunction.lockTech();

            }

            GUILayout.EndVertical();
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();


        }

        /// <summary>
        /// 建筑面板
        /// </summary>
        void BuildPannel()
        {
            GUILayout.BeginHorizontal();
            {
                GUILayout.BeginVertical();

                GUILayout.Label("物流站功能".getTranslate() + ":", style);

                GUILayout.BeginHorizontal();
                GUILayout.Space(BaseSize);

                GUILayout.BeginVertical();

                GUILayout.Label("额外存储倍率".getTranslate() + ":" + StationStoExtra.Value, style);
                StationStoExtra.Value = (int)GUILayout.HorizontalSlider(StationStoExtra.Value, 0, 100, GUILayout.Width(heightdis * 4));

                Stationfullenergy.Value = GUILayout.Toggle(Stationfullenergy.Value, "永久满电".getTranslate());
                StationfullCount = GUILayout.Toggle(StationfullCount, "要啥有啥".getTranslate());
                StationMaxproliferator.Value = GUILayout.Toggle(StationMaxproliferator.Value, "无限增产".getTranslate());
                StationSpray.Value = GUILayout.Toggle(StationSpray.Value, "内置喷涂".getTranslate());
                StationPowerGen.Value = GUILayout.Toggle(StationPowerGen.Value, "内置发电".getTranslate());
                Station_infiniteWarp_bool.Value = GUILayout.Toggle(Station_infiniteWarp_bool.Value, "无限翘曲".getTranslate());
                build_station_nocondition.Value = GUILayout.Toggle(build_station_nocondition.Value, "建造无需条件".getTranslate());
                GUILayout.Label("以下功能需改名".getTranslate() + ":", style);
                StationfullCount_bool.Value = GUILayout.Toggle(StationfullCount_bool.Value, "星球无限供货机".getTranslate());
                StationTrash.Value = GUILayout.Toggle(StationTrash.Value, "垃圾站".getTranslate());
                if (StationTrash.Value)
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Space(BaseSize);
                    noneedtrashsand.Value = GUILayout.Toggle(noneedtrashsand.Value, "不需要沙土".getTranslate());
                    GUILayout.EndHorizontal();
                }
                StationMiner.Value = GUILayout.Toggle(StationMiner.Value, "星球矿机".getTranslate());
                if (StationMiner.Value)
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Space(BaseSize);

                    GUILayout.BeginVertical();
                    GUILayout.Label("采矿速率".getTranslate() + ":" + Stationminenumber.Value, style);
                    Stationminenumber.Value = (int)GUILayout.HorizontalSlider(Stationminenumber.Value, 1, 100, GUILayout.Width(heightdis * 4));
                    GUILayout.EndVertical();
                    GUILayout.EndHorizontal();
                }
                StationSprayer.Value = GUILayout.Toggle(StationSprayer.Value, "喷涂加工厂".getTranslate());
                StationMinerSmelter.Value = GUILayout.Toggle(StationMinerSmelter.Value, "星球熔炉矿机".getTranslate());
                if (StationMinerSmelter.Value)
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Space(BaseSize);

                    GUILayout.BeginVertical();
                    GUILayout.Label("等价高级熔炉数量".getTranslate() + ":" + StationMinerSmelterNum.Value, style);
                    int tempStationMinerSmelterNum = (int)GUILayout.HorizontalSlider(StationMinerSmelterNum.Value, 30, 1000, GUILayout.Width(heightdis * 4));
                    if (tempStationMinerSmelterNum % 100 <= tempStationMinerSmelterNum % 30)
                    {
                        StationMinerSmelterNum.Value = tempStationMinerSmelterNum / 100 * 100;
                    }
                    else
                    {
                        StationMinerSmelterNum.Value = tempStationMinerSmelterNum / 30 * 30;
                    }
                    GUILayout.EndVertical();
                    GUILayout.EndHorizontal();
                }
                GUILayout.Label("自动改名".getTranslate() + ":", style);
                string[] stationNames = new string[] { "星球无限供货机", "垃圾站", "星球矿机", "喷涂加工厂", "星球熔炉矿机", "星球量子传输站", "星系量子传输站" };
                for (int i = 0; i < stationNames.Length; i++)
                {
                    bool temp = GUILayout.Toggle(AutoChangeStationName.Value == stationNames[i], stationNames[i].getTranslate());
                    if (temp && AutoChangeStationName.Value != stationNames[i])
                    {
                        AutoChangeStationName.Value = stationNames[i];
                    }
                    else if (!temp && AutoChangeStationName.Value == stationNames[i])
                    {
                        AutoChangeStationName.Value = "";
                    }
                }

                GUILayout.EndVertical();

                GUILayout.EndHorizontal();

                GUILayout.EndVertical();
            }
            {
                GUILayout.BeginVertical();

                GUILayout.Label("其它建筑功能".getTranslate() + ":", style);
                {
                    GUILayout.BeginHorizontal();

                    GUILayout.Space(BaseSize);

                    GUILayout.BeginVertical();
                    GUILayout.Label("建造数量最大值".getTranslate() + ":" + Buildmaxlen.Value, style);
                    int tempBuildmaxlen = (int)GUILayout.HorizontalSlider(Buildmaxlen.Value, 15, 100, GUILayout.Width(heightdis * 4));
                    if (tempBuildmaxlen != Buildmaxlen.Value)
                    {
                        Buildmaxlen.Value = tempBuildmaxlen;
                        GameMain.mainPlayer.controller.actionBuild.clickTool.dotsSnapped = new Vector3[tempBuildmaxlen];
                    }
                    InspectDisNoLimit.Value = GUILayout.Toggle(InspectDisNoLimit.Value, "操作范围不受限制".getTranslate());
                    BuildNotime_bool.Value = GUILayout.Toggle(BuildNotime_bool.Value, "建筑秒完成".getTranslate());
                    blueprintpastenoneed_bool.Value = GUILayout.Toggle(blueprintpastenoneed_bool.Value, "蓝图建造无需材料".getTranslate());
                    ArchitectMode.Value = GUILayout.Toggle(ArchitectMode.Value, "建筑师模式".getTranslate());
                    if (DriftBuildings != GUILayout.Toggle(DriftBuildings, "建筑抬升".getTranslate()))
                    {
                        DriftBuildings = !DriftBuildings;
                        if (DriftBuildings && !DriftBuildingsPatch.IsPatched)
                        {
                            DriftBuildingsPatch.IsPatched = true;
                            harmony.PatchAll(typeof(DriftBuildingsPatch));
                        }
                    }
                    if (DriftBuildings) GUILayout.Label("抬升层数:" + DriftBuildingLevel, style);
                    if (BeltSignalFunction.Value != GUILayout.Toggle(BeltSignalFunction.Value, "传送带信号功能".getTranslate()))
                    {
                        BeltSignalFunction.Value = !BeltSignalFunction.Value;
                        if (BeltSignalFunction.Value) MainFunction.InitBeltSignalDiction();
                    }
                    GUILayout.EndVertical();

                    GUILayout.EndHorizontal();
                }

                GUILayout.Label("电力设备".getTranslate() + ":", style);
                {

                    GUILayout.BeginHorizontal();
                    GUILayout.Space(BaseSize);

                    GUILayout.BeginVertical();

                    Buildingnoconsume.Value = GUILayout.Toggle(Buildingnoconsume.Value, "新建设备不耗电".getTranslate());
                    InfineteStarPower.Value = GUILayout.Toggle(InfineteStarPower.Value, "人造恒星无限能源".getTranslate());
                    PlanetPower_bool.Value = GUILayout.Toggle(PlanetPower_bool.Value, "星球电网(人造恒星)".getTranslate());
                    if (PlanetPower_bool.Value)
                    {
                        GUILayout.BeginHorizontal();
                        GUILayout.Space(BaseSize);
                        GUILayout.BeginVertical();
                        PlanetPower_bool.Value = GUILayout.Toggle(PlanetPower_bool.Value, "覆盖全球".getTranslate());
                        farconnectdistance = GUILayout.Toggle(farconnectdistance, "超长连接".getTranslate());
                        GUILayout.EndVertical();
                        GUILayout.EndHorizontal();
                    }
                    WindturbinesUnlimitedEnergy.Value = GUILayout.Toggle(WindturbinesUnlimitedEnergy.Value, "风力涡轮机无限能源".getTranslate());
                    Windturbinescovertheglobe.Value = GUILayout.Toggle(Windturbinescovertheglobe.Value, "风力涡轮机覆盖全球".getTranslate());
                    GUILayout.EndVertical();

                    GUILayout.EndHorizontal();
                }

                GUILayout.Label("储液站功能".getTranslate() + ":", style);
                GUILayout.BeginHorizontal();
                GUILayout.Space(BaseSize);
                GUILayout.BeginVertical();
                Tankcontentall.Value = GUILayout.Toggle(Tankcontentall.Value, "储液站任意存".getTranslate());
                Infinitestoragetank.Value = GUILayout.Toggle(Infinitestoragetank.Value, "无限储液站".getTranslate());
                TankMaxproliferator.Value = GUILayout.Toggle(TankMaxproliferator.Value, "储液站无限增产".getTranslate());
                GUILayout.EndVertical();
                GUILayout.EndHorizontal();

                GUILayout.EndVertical();

            }
            {
                GUILayout.BeginVertical();
                Quantumtransport_bool.Value = GUILayout.Toggle(Quantumtransport_bool.Value, "量子传输站".getTranslate());
                if (Quantumtransport_bool.Value)
                {
                    Quantumtransportpdwarp_bool.Value = GUILayout.Toggle(Quantumtransportpdwarp_bool.Value, "星球级翘曲全面供应".getTranslate());
                    Quantumtransportstarwarp_bool.Value = GUILayout.Toggle(Quantumtransportstarwarp_bool.Value, "星系级翘曲全面供应".getTranslate());
                    Quantumtransportbuild_bool.Value = GUILayout.Toggle(Quantumtransportbuild_bool.Value, "星球级材料供应".getTranslate());

                    if (Quantumtransportbuild_bool.Value)
                    {
                        GUILayout.BeginVertical();
                        GUILayout.BeginHorizontal();
                        QuantumtransportCollectorSupply.Value = GUILayout.Toggle(QuantumtransportCollectorSupply.Value, "采集器拿取");
                        GUILayout.EndHorizontal();

                        GUILayout.BeginHorizontal();
                        QuantumtransportstationSupply.Value = GUILayout.Toggle(QuantumtransportstationSupply.Value, "物流站供应");
                        QuantumtransportstationDemand.Value = GUILayout.Toggle(QuantumtransportstationDemand.Value, "物流站拿取");
                        GUILayout.EndHorizontal();

                        GUILayout.BeginHorizontal();
                        QuantumtransportminerDemand.Value = GUILayout.Toggle(QuantumtransportminerDemand.Value, "矿机拿取");
                        GUILayout.EndHorizontal();

                        GUILayout.BeginHorizontal();
                        QuantumtransportsiloDemand.Value = GUILayout.Toggle(QuantumtransportsiloDemand.Value, "发射井弹射器供应");
                        GUILayout.EndHorizontal();

                        GUILayout.BeginHorizontal();
                        QuantumtransportlabSupply.Value = GUILayout.Toggle(QuantumtransportlabSupply.Value, "研究站供应");
                        QuantumtransportlabDemand.Value = GUILayout.Toggle(QuantumtransportlabDemand.Value, "研究站拿取");
                        GUILayout.EndHorizontal();

                        GUILayout.BeginHorizontal();
                        QuantumtransportpowerSupply.Value = GUILayout.Toggle(QuantumtransportpowerSupply.Value, "电力设备供应");
                        QuantumtransportpowerDemand.Value = GUILayout.Toggle(QuantumtransportpowerDemand.Value, "电力设备拿取");
                        GUILayout.EndHorizontal();

                        GUILayout.BeginHorizontal();
                        QuantumtransportassembleSupply.Value = GUILayout.Toggle(QuantumtransportassembleSupply.Value, "组装机供应");
                        QuantumtransportassembleDemand.Value = GUILayout.Toggle(QuantumtransportassembleDemand.Value, "组装机拿取");
                        GUILayout.EndHorizontal();

                        GUILayout.EndVertical();
                    }
                    var tempoptions = new[] { GUILayout.Height(heightdis), GUILayout.Width(heightdis * 5) };
                    GUILayout.BeginHorizontal();
                    int tempplanetquamaxpowerpertick = (int)GUILayout.HorizontalSlider(planetquamaxpowerpertick.Value, 60, 10000, tempoptions);
                    GUILayout.Label(tempplanetquamaxpowerpertick > 1000 ? tempplanetquamaxpowerpertick / 1000 + "G" : tempplanetquamaxpowerpertick + "MW " + "星球量子充电功率".getTranslate(), style);
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    int tempstarquamaxpowerpertick = (int)GUILayout.HorizontalSlider(starquamaxpowerpertick.Value, 60, 10000, tempoptions);
                    GUILayout.Label(tempstarquamaxpowerpertick > 1000 ? tempstarquamaxpowerpertick / 1000 + "G" : tempstarquamaxpowerpertick + "MW " + "星系量子充电功率".getTranslate(), style);
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    Quantumenergy.Value = (int)GUILayout.HorizontalSlider(Quantumenergy.Value, 0, 1_000_000, tempoptions);
                    GUILayout.Label(TGMKinttostring(Quantumenergy.Value, "J") + " " + "量子耗能/个".getTranslate(), style);
                    GUILayout.EndHorizontal();

                    if (tempplanetquamaxpowerpertick != planetquamaxpowerpertick.Value || tempstarquamaxpowerpertick != starquamaxpowerpertick.Value)
                    {
                        changequapowerpertick = true;
                        planetquamaxpowerpertick.Value = tempplanetquamaxpowerpertick;
                        starquamaxpowerpertick.Value = tempstarquamaxpowerpertick;
                    }
                }
                GUILayout.EndVertical();
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }

        /// <summary>
        /// 星球面板
        /// </summary>
        void PlanetPannel()
        {
            GUILayout.Label("\"生成矿物\":鼠标左键生成矿物，鼠标右键取消。\"删除矿物\"：按x键进入拆除模式可拆除矿物。".getTranslate(), style);
            GUILayout.BeginHorizontal();
            GUILayout.BeginVertical();
            var veinControlProperty = MainFunction.veinproperty;
            veinControlProperty.DeleteVein = GUILayout.Toggle(veinControlProperty.DeleteVein, "删除矿物".getTranslate());
            veinControlProperty.Changeveinpos = GUILayout.Toggle(veinControlProperty.Changeveinpos, "移动单矿".getTranslate());
            veinControlProperty.Changeveingrouppos = GUILayout.Toggle(veinControlProperty.Changeveingrouppos, "移动矿堆".getTranslate());
            veinControlProperty.NotTidyVein = GUILayout.Toggle(veinControlProperty.NotTidyVein, "不排列".getTranslate());
            veinControlProperty.GetallVein = GUILayout.Toggle(veinControlProperty.GetallVein, "整理所有矿".getTranslate());
            GUILayout.Label("整理为".getTranslate() + veinControlProperty.VeinLines + "行".getTranslate());
            veinControlProperty.VeinLines = (int)GUILayout.HorizontalSlider(veinControlProperty.VeinLines, 1, 20);
            veinControlProperty.Changexveinspos = GUILayout.Toggle(veinControlProperty.Changexveinspos, "切割矿脉".getTranslate());
            GUILayout.Label("切割出".getTranslate() + veinControlProperty.CuttingVeinNumbers + "个".getTranslate());
            veinControlProperty.CuttingVeinNumbers = (int)GUILayout.HorizontalSlider(veinControlProperty.CuttingVeinNumbers, 2, 72);
            GUILayout.EndVertical();
            GUILayout.Space(heightdis);
            GUILayout.BeginVertical();

            var squareoptions = new GUILayoutOption[] { GUILayout.Height(heightdis), GUILayout.Width(heightdis) };
            int columnnum = 8;
            int veinNums = LDB.veins.dataArray.Length;
            int lines = veinNums / columnnum + ((veinNums % columnnum) > 0 ? 1 : 0);
            for (int i = 0; i < lines; i++)
            {
                GUILayout.BeginHorizontal();
                for (int j = 0; j < columnnum; j++)
                {
                    int index = i * columnnum + j + 1;
                    if (index == veinNums + 1) break;
                    if (GUILayout.Button(LDB.items?.Select(LDB.veins.Select(index).MiningItem)?.iconSprite?.texture, veinControlProperty.VeinType == index ? selectedIconStyle : normalStyle, squareoptions))
                    {
                        veinControlProperty.VeinType = veinControlProperty.VeinType != index ? index : 0;
                    }
                }

                GUILayout.EndHorizontal();
            }
            GUILayout.BeginHorizontal();
            GUILayout.Label("生成矿物模式".getTranslate());
            bool tempPress = GUILayout.Toggle(veinControlProperty.AddVeinMode == 0, "点按".getTranslate());
            bool tempJustPress = GUILayout.Toggle(veinControlProperty.AddVeinMode == 1, "按压".getTranslate());
            GUILayout.EndHorizontal();
            if (tempPress && veinControlProperty.AddVeinMode == 1)
            {
                veinControlProperty.AddVeinMode = 0;
            }
            else if (tempJustPress && veinControlProperty.AddVeinMode == 0)
            {
                veinControlProperty.AddVeinMode = 1;
            }
            if (veinControlProperty.VeinType == 7)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label("添加油井速率".getTranslate());
                for (int i = 0; i < 3; i++)
                {
                    bool temp = GUILayout.Toggle(veinControlProperty.OilAddIntervalBool[i], veinControlProperty.OilAddIntervalValue[i] + "/s");
                    if (temp != veinControlProperty.OilAddIntervalBool[i] && temp)
                    {
                        veinControlProperty.SetOilAddInterval(i);
                    }
                }
                GUILayout.EndHorizontal();
            }
            else
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label("添加矿脉数量".getTranslate());
                bool temp500 = GUILayout.Toggle(veinControlProperty.AddVeinNumber == 500, "500");
                bool tempInfinite = GUILayout.Toggle(veinControlProperty.AddVeinNumber != 500, "无穷".getTranslate());
                if (temp500 && veinControlProperty.AddVeinNumber != 500)
                {
                    veinControlProperty.AddVeinNumber = 500;
                }
                else if (tempInfinite && veinControlProperty.AddVeinNumber == 500)
                {
                    veinControlProperty.AddVeinNumber = 1000000000;
                }
                GUILayout.EndHorizontal();
            }
            GUILayout.EndVertical();

            GUILayout.Space(heightdis);
            GUILayout.BeginVertical();
            restorewater = GUILayout.Toggle(restorewater, "还原海洋".getTranslate());
            GUILayout.BeginHorizontal();
            GUILayout.Label("地基类型");
            if (GUILayout.Button("<"))
            {
                MainFunction.ReformType--;
                if (MainFunction.ReformType < 0)
                {
                    MainFunction.ReformType = 7;
                }
            }
            GUILayout.Label(MainFunction.ReformType.ToString());
            if (GUILayout.Button(">"))
            {
                MainFunction.ReformType++;
                MainFunction.ReformType %= 8;
            }

            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.Label("颜色类型");
            if (GUILayout.Button("<"))
            {
                MainFunction.ReformColor--;
                if (MainFunction.ReformColor < 0)
                {
                    MainFunction.ReformColor = 31;
                }
            }
            GUILayout.Label(MainFunction.ReformColor.ToString());
            if (GUILayout.Button(">"))
            {
                MainFunction.ReformColor++;
                MainFunction.ReformColor %= 32;
            }
            ReformTypeConfig.Value = MainFunction.ReformType;
            ReformColorConfig.Value = MainFunction.ReformColor;

            GUILayout.EndHorizontal();
            var tempstr = new string[8] { !restorewater ? "铺平整个星球" : "还原全部海洋", "掩埋全部矿", "删除全部矿", "超密铺采集器", "删除当前星球所有建筑", "删除当前星球所有建筑(不掉落)", "初始化当前星球", "初始化当前星球(不要海洋)" };
            for (int i = 0; i < tempstr.Length; i++)
            {
                if (GUILayout.Button(tempstr[i].getTranslate(), GUILayout.Height(heightdis)))
                {
                    switch (i)
                    {
                        case 0: MainFunction.OnSetBase(0); break;
                        case 1: veinControlProperty.BuryAllvein(); break;
                        case 2: veinControlProperty.RemoveAllvein(); break;
                        case 3: MainFunction.SetMaxGasStation(); break;
                        case 4: MainFunction.RemoveAllBuild(0); break;
                        case 5: MainFunction.RemoveAllBuild(1); break;
                        case 6: MainFunction.RemoveAllBuild(2); break;
                        case 7: MainFunction.RemoveAllBuild(3); break;
                    }
                }
            }
            MainFunction.currentPlanetWaterType = GUILayout.TextField(MainFunction.currentPlanetWaterType, GUILayout.MinWidth(heightdis * 3));
            if (GUILayout.Button("设置当前星球海洋类型".getTranslate(), GUILayout.Height(heightdis)))
            {
                MainFunction.SetWaterType();
            }
            if (GUILayout.Button("恢复所有星球海洋类型".getTranslate(), GUILayout.Height(heightdis)))
            {
                MainFunction.RestoreWaterType();
            }
            GUILayout.EndVertical();
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }


        /// <summary>
        /// 戴森球面板
        /// </summary>
        void DysonPannel()
        {
            GUILayout.Label("注意事项:戴森云和戴森壳不要出现一层轨道都没有的情况(用前存档)".getTranslate(), style);
            GUILayout.BeginHorizontal();
            GUILayout.BeginVertical();
            if (!(GameMain.localStar == null || GameMain.data.dysonSpheres == null || GameMain.data.dysonSpheres[GameMain.localStar.index] == null))
            {
                DysonSphere ds = GameMain.data.dysonSpheres[GameMain.localStar.index];
                List<int> layerlist = new List<int>();
                foreach (DysonSphereLayer dysonSphereLayer in GameMain.data.dysonSpheres[GameMain.localStar.index].layersIdBased)
                {
                    if (dysonSphereLayer != null)
                        layerlist.Add(dysonSphereLayer.id);
                }
                GUILayout.Label("点击删除下列戴森壳层级".getTranslate(), style);
                GUILayout.BeginHorizontal();
                for (int i = 1; i <= 5; i++)
                {
                    bool contain = layerlist.Contains(i);
                    bool buttonclick = GUILayout.Button(contain ? i.ToString() : "", iconbuttonoptions);
                    if (buttonclick && contain)
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
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();
                for (int i = 6; i <= 10; i++)
                {
                    bool contain = layerlist.Contains(i);
                    bool buttonclick = GUILayout.Button(contain ? i.ToString() : "", iconbuttonoptions);
                    if (buttonclick && contain)
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
                GUILayout.EndHorizontal();
                if (GUILayout.Button("初始化当前戴森球".getTranslate(), GUILayout.Height(heightdis)))
                {
                    if (GameMain.localStar != null && GameMain.data.dysonSpheres[GameMain.localStar.index] != null)
                    {
                        int index = GameMain.localStar.index;
                        GameMain.data.dysonSpheres[index] = new DysonSphere();
                        GameMain.data.dysonSpheres[index].Init(GameMain.data, GameMain.localStar);
                        GameMain.data.dysonSpheres[index].ResetNew();
                    }
                }
                if (GUILayout.Button("瞬间完成戴森球(用前存档)".getTranslate(), GUILayout.Height(heightdis)))
                {
                    MainFunction.FinishDysonShell();
                }
            }
            quickEjector = GUILayout.Toggle(quickEjector, "极速轨道弹射器(慎用)".getTranslate());
            quicksilo = GUILayout.Toggle(quicksilo, "极速垂直发射井(慎用)".getTranslate());
            if (cancelsolarbullet.Value != GUILayout.Toggle(cancelsolarbullet.Value, "跳过太阳帆子弹阶段".getTranslate()))
            {
                cancelsolarbullet.Value = !cancelsolarbullet.Value;
                playcancelsolarbullet = cancelsolarbullet.Value;
            }
            quickabsorbsolar.Value = GUILayout.Toggle(quickabsorbsolar.Value, "跳过太阳帆吸收阶段".getTranslate());
            QuickabortSwarm.Value = GUILayout.Toggle(QuickabortSwarm.Value, "太阳帆帧吸收".getTranslate() + ":" + Solarsailsabsorbeveryframe.Value);
            if (QuickabortSwarm.Value)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Space(10);
                Solarsailsabsorbeveryframe.Value = (int)GUILayout.HorizontalSlider(Solarsailsabsorbeveryframe.Value, 1, 100);
                GUILayout.EndHorizontal();
            }
            if (alwaysemission.Value != GUILayout.Toggle(alwaysemission.Value, "全球打帆".getTranslate()))
            {
                alwaysemission.Value = !alwaysemission.Value;
                alwaysemissiontemp = alwaysemission.Value;
            }
            var temp = GUILayout.Toggle(ChangeDysonradius.Value, "开放戴森壳半径上下限(用前存档)".getTranslate());
            GUILayout.Label("最小半径".getTranslate() + ":100", style);
            GUILayout.Label("最大半径".getTranslate() + ":" + TGMKinttostring(MaxOrbitRadiusConfig.Value, ""), style);
            var tempvalue = Regex.Replace(GUILayout.TextField(MaxOrbitRadiusConfig.Value + "", 10), @"[^0-9]", "");
            MaxOrbitRadiusConfig.Value = IntParseLimit(tempvalue, 100000, 10_000_000);
            if (temp != ChangeDysonradius.Value)
            {
                ChangeDysonradius.Value = temp;
                var selectdyson = UIRoot.instance?.uiGame?.dysonEditor?.selection?.viewDysonSphere;
                foreach (var dyson in GameMain.data?.dysonSpheres)
                {
                    if (dyson == null)
                        continue;
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
            GUILayout.EndVertical();
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }

        /// <summary>
        /// 其它面板
        /// </summary>
        void OtherPannel()
        {
            GUILayout.BeginHorizontal();
            GUILayout.BeginVertical();
            {
                GUILayout.Label("以下设置需要进入存档".getTranslate(), style);
                if (GameMain.mainPlayer != null)
                {
                    CloseUIAbnormalityTip.Value = GUILayout.Toggle(CloseUIAbnormalityTip.Value, "关闭异常提示".getTranslate());
                    Maxproliferator.Value = GUILayout.Toggle(Maxproliferator.Value, "增产点数上限10".getTranslate());
                    incAbility = Maxproliferator.Value ? 10 : 4;
                    pasteanyway = GUILayout.Toggle(pasteanyway, "蓝图强制粘贴".getTranslate());
                    if (PasteBuildAnyWay != GUILayout.Toggle(PasteBuildAnyWay, "建筑铺设无需条件".getTranslate()))
                    {
                        PasteBuildAnyWay = !PasteBuildAnyWay;
                        if (PasteBuildAnyWay && !PasteAnywayPatch.IsPatched)
                        {
                            PasteAnywayPatch.IsPatched = true;
                            harmony.PatchAll(typeof(PasteAnywayPatch));
                        }
                    }
                    if (closeallcollider != GUILayout.Toggle(closeallcollider, "关闭所有碰撞体".getTranslate()))
                    {
                        closeallcollider = !closeallcollider;
                        ColliderPool.instance.gameObject.SetActive(!closeallcollider);
                    }
                    if (allhandcraft.Value != GUILayout.Toggle(allhandcraft.Value, "全部手搓".getTranslate()))
                    {
                        allhandcraft.Value = !allhandcraft.Value;
                        refreshLDB = true;
                    }
                    if (quickproduce.Value != GUILayout.Toggle(quickproduce.Value, "快速生产".getTranslate()))
                    {
                        quickproduce.Value = !quickproduce.Value;
                        refreshLDB = true;
                    }
                    GUILayout.Label("堆叠倍率".getTranslate() + ":", style);
                    GUILayout.BeginHorizontal();
                    stackmultipleStr = Regex.Replace(GUILayout.TextField(stackmultipleStr, 10, GUILayout.MinWidth(heightdis * 3)), @"[^0-9]", "");
                    int multiple = IntParseLimit(stackmultipleStr, 1, 5000000);
                    stackmultipleStr = multiple.ToString();
                    if (GUILayout.Button("设置".getTranslate()))
                    {
                        MainFunction.SetmultipleItemStatck(multiple);
                    }
                    GUILayout.EndHorizontal();
                    GUILayout.Label("冶炼倍数".getTranslate() + ":", style);
                    GUILayout.BeginHorizontal();
                    multipelsmeltStr = Regex.Replace(GUILayout.TextField(multipelsmeltStr, 10, GUILayout.MinWidth(heightdis * 3)), @"[^0-9]", "");
                    multiple = IntParseLimit(multipelsmeltStr, 1, 100);
                    multipelsmeltStr = multiple.ToString();
                    if (GUILayout.Button("设置".getTranslate()))
                    {
                        MainFunction.Setmultiplesmelt(multiple);
                    }
                    GUILayout.EndHorizontal();
                    if (GUILayout.Button("取消所有勾选".getTranslate()))
                    {
                        MainFunction.CancelAllToggle();
                    }
                }
            }
            GUILayout.EndVertical();
            GUILayout.Space(heightdis);
            GUILayout.BeginVertical();
            {
                GUILayout.Label("窗口颜色及透明度".getTranslate() + " RGBA", style);
                GUILayout.BeginHorizontal();
                float textureColorR = GUILayout.HorizontalSlider(mainWindowTextureColor.r, 0, 1, GUILayout.MinWidth(4 * heightdis));
                GUILayout.Label("R:" + string.Format("{0:N2}", textureColorR), style);
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                float textureColorG = GUILayout.HorizontalSlider(mainWindowTextureColor.g, 0, 1, GUILayout.MinWidth(4 * heightdis));
                GUILayout.Label("G:" + string.Format("{0:N2}", textureColorG), style);
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                float textureColorB = GUILayout.HorizontalSlider(mainWindowTextureColor.b, 0, 1, GUILayout.MinWidth(4 * heightdis));
                GUILayout.Label("B:" + string.Format("{0:N2}", textureColorB), style);
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                float textureColorA = GUILayout.HorizontalSlider(mainWindowTextureColor.a, 0, 1, GUILayout.MinWidth(4 * heightdis));
                GUILayout.Label("A:" + string.Format("{0:N2}", textureColorA), style);
                GUILayout.EndHorizontal();

                if (textureColorR != mainWindowTextureColor.r || textureColorG != mainWindowTextureColor.g || textureColorB != mainWindowTextureColor.b || textureColorA != mainWindowTextureColor.a)
                {
                    mainWindowTextureColor = new Color(textureColorR, textureColorG, textureColorB, textureColorA);

                    for (int i = 0; i < MainWindowTexture.width; i++)
                    {
                        for (int j = 0; j < MainWindowTexture.height; j++)
                        {
                            MainWindowTexture.SetPixel(i, j, mainWindowTextureColor);
                        }
                    }
                    MainWindowTexture.Apply();
                }

                GUILayout.Label("窗口字体颜色".getTranslate() + " RGB", style);
                GUILayout.BeginHorizontal();
                float textcolorr = GUILayout.HorizontalSlider(TextColor.r, 0, 1, GUILayout.MinWidth(4 * heightdis));
                GUILayout.Label("R:" + string.Format("{0:N2}", textcolorr), style);
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();
                float textcolorg = GUILayout.HorizontalSlider(TextColor.g, 0, 1, GUILayout.MinWidth(4 * heightdis));
                GUILayout.Label("G:" + string.Format("{0:N2}", textcolorg), style);
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();
                float textcolorb = GUILayout.HorizontalSlider(TextColor.b, 0, 1, GUILayout.MinWidth(4 * heightdis));
                GUILayout.Label("B:" + string.Format("{0:N2}", textcolorb), style);
                GUILayout.EndHorizontal();

                if (textcolorr != TextColor.r || textcolorg != TextColor.g || textcolorb != TextColor.b)
                {
                    TextColor = new Color(textcolorr, textcolorg, textcolorb);
                }
                ChangeQuickKey = GUILayout.Toggle(ChangeQuickKey, !ChangeQuickKey ? "改变窗口快捷键".getTranslate() : "点击确认".getTranslate());
                GUILayout.Label("快捷键".getTranslate() + ":" + tempShowWindow.ToString());
            }
            GUILayout.EndVertical();
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }

        /// <summary>
        /// 机甲物流面板
        /// </summary>
        void LogisticsPannel()
        {
            GUILayout.BeginHorizontal();
            GUILayout.BeginVertical();
            {
                GUILayout.Label("如果启动机甲物流，物流背包没有解锁会自动解锁一列。", style);
                GUILayout.Label("物流背包里的设置可以直接从全星系的存储设备中获取或回收", style);
                GUILayout.Label("设置机甲物流专用星后，只有名字是\"机甲物流\"的星球才会进行物流操作", style);
                GUILayoutOption[] options = new[] { GUILayout.Height(heightdis) };
                if (Mechalogistics_bool.Value != GUILayout.Toggle(Mechalogistics_bool.Value, "强化机甲物流".getTranslate(), options))
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
            }
            GUILayout.EndVertical();
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }

        #endregion

        #region 窗口操作
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

        /// <summary>
        /// 移动窗口
        /// </summary>
        private void MoveWindow()
        {
            if (leftscaling || rightscaling || bottomscaling) return;
            Vector2 temp = Input.mousePosition;
            bool horizontal = MainWindow_x <= temp.x && MainWindow_x + MainWindowWidth >= temp.x;
            bool vertical = Screen.height >= MainWindow_y + temp.y && Screen.height <= MainWindowHeight + MainWindow_y + temp.y;
            MouseInWindow = horizontal && vertical;
            if (temp.x > MainWindow_x && temp.x < MainWindow_x + MainWindowWidth && Screen.height - temp.y > MainWindow_y && Screen.height - temp.y < MainWindow_y + 20)
            {
                if (Input.GetMouseButton(0))
                {
                    if (!moving)
                    {
                        MainWindow_x_move = MainWindow_x;
                        MainWindow_y_move = MainWindow_y;
                        temp_MainWindow_x = temp.x;
                        temp_MainWindow_y = Screen.height - temp.y;
                    }
                    moving = true;
                    MainWindow_x = MainWindow_x_move + temp.x - temp_MainWindow_x;
                    MainWindow_y = MainWindow_y_move + (Screen.height - temp.y) - temp_MainWindow_y;
                }
                else
                {
                    moving = false;
                    temp_MainWindow_x = MainWindow_x;
                    temp_MainWindow_y = MainWindow_y;
                }
            }
            else if (moving)
            {
                moving = false;
                MainWindow_x = MainWindow_x_move + temp.x - temp_MainWindow_x;
                MainWindow_y = MainWindow_y_move + (Screen.height - temp.y) - temp_MainWindow_y;
            }
            MainWindow_y = Math.Max(10, Math.Min(Screen.height - 10, MainWindow_y));
            MainWindow_x = Math.Max(10, Math.Min(Screen.width - 10, MainWindow_x));
        }

        /// <summary>
        /// 改变窗口大小
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="window_x"></param>
        /// <param name="window_y"></param>
        private void Scaling_window()
        {
            Vector2 temp = Input.mousePosition;
            float x = MainWindowWidth;
            float y = MainWindowHeight;
            if (Input.GetMouseButton(0))
            {
                if ((temp.x + 10 > MainWindow_x && temp.x - 10 < MainWindow_x) && (Screen.height - temp.y >= MainWindow_y && Screen.height - temp.y <= MainWindow_y + y) || leftscaling)
                {
                    x -= temp.x - MainWindow_x;
                    MainWindow_x = temp.x;
                    leftscaling = true;
                    rightscaling = false;
                }
                if ((temp.x + 10 > MainWindow_x + x && temp.x - 10 < MainWindow_x + x) && (Screen.height - temp.y >= MainWindow_y && Screen.height - temp.y <= MainWindow_y + y) || rightscaling)
                {
                    x += temp.x - MainWindow_x - x;
                    rightscaling = true;
                    leftscaling = false;
                }
                if ((Screen.height - temp.y + 10 > y + MainWindow_y && Screen.height - temp.y - 10 < y + MainWindow_y) && (temp.x >= MainWindow_x && temp.x <= MainWindow_x + x) || bottomscaling)
                {
                    y += Screen.height - temp.y - (MainWindow_y + y);
                    bottomscaling = true;
                }
            }
            if (Input.GetMouseButtonUp(0))
            {
                rightscaling = false;
                leftscaling = false;
                bottomscaling = false;
            }
            MainWindowWidth = x;
            MainWindowHeight = y;
        }
        #endregion

        /// <summary>
        /// 整形转化，设置上下限
        /// </summary>
        /// <param name="valueStr"></param>
        /// <param name="lowlimit"></param>
        /// <param name="highlimit"></param>
        /// <returns></returns>
        public int IntParseLimit(string valueStr, int lowlimit, int highlimit)
        {
            int.TryParse(valueStr, out int value);
            return Math.Max(lowlimit, Math.Min(value, highlimit));
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
