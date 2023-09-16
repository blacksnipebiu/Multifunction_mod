using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using static Multfunction_mod.Multifunction;
using Vector2 = UnityEngine.Vector2;

namespace Multfunction_mod
{
    public class GUIDraw
    {
        private string stackmultipleStr;
        private string multipelsmeltStr;
        private Multifunction MainFunction;
        private int whichpannel;
        private Vector2 scrollPosition;
        private Vector2 TabscrollPosition;
        private GameObject ui_MultiFunctionPanel;
        private bool firstopen;
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
        public int BaseSize
        {
            get => baseSize;
            set
            {
                baseSize = value;
                scale.Value = value;
                firstopen = true;
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
        public bool TabDisplayingWindow { get; private set; }
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


        private GUIStyle style;
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
            firstopen = true;
            ColorChanged = true;
            stackmultipleStr = StackMultiple.Value.ToString();
            multipelsmeltStr = MULTIPELSMELT.Value.ToString();
            menus = new string[7] { "", "人物", "建筑", "星球", "戴森球", "其它功能", "机甲物流" };
            TextColor = Textcolor.Value;
            MainWindowTextureColor = mainWindowTextureColor_config.Value;
            MainWindowTexture = new Texture2D(10, 10);
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

        public void TabWindowKeyInvoke()
        {
            TabDisplayingWindow = !TabDisplayingWindow;
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

        private bool firstDraw;
        public void Draw()
        {
            if (firstDraw)
            {
                firstDraw = false;
                BaseSize = GUI.skin.label.fontSize;
            }
            englishShow = Localization.language != Language.zhCN;
            bool changesize = false;
            if (DisplayingWindow || TabDisplayingWindow)
            {
                if (Input.GetKey(KeyCode.LeftControl))
                {
                    int t = (int)(Input.GetAxis("Mouse Wheel") * 10);
                    int temp = BaseSize + t;
                    if (Input.GetKeyDown(KeyCode.UpArrow)) { temp++; }
                    if (Input.GetKeyDown(KeyCode.DownArrow)) { temp--; }
                    temp = Math.Max(5, Math.Min(temp, 35));
                    if (temp != BaseSize)
                    {
                        changesize = true;
                    }
                    BaseSize = temp;
                }
            }
            if (firstopen)
            {
                firstopen = false;
                GUI.skin.label.fontSize = BaseSize;
                GUI.skin.button.fontSize = BaseSize;
                GUI.skin.toggle.fontSize = BaseSize;
                GUI.skin.textField.fontSize = BaseSize;
                GUI.skin.textArea.fontSize = BaseSize;
                if (style != null)
                {
                    style.fontSize = BaseSize;
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
                GUI.skin.button.normal.textColor = TextColor;
                GUI.skin.textArea.normal.textColor = TextColor;
                GUI.skin.textField.normal.textColor = TextColor;
                GUI.skin.toggle.normal.textColor = TextColor;
                GUI.skin.toggle.onNormal.textColor = TextColor;
                ColorChanged = false;
                style = new GUIStyle
                {
                    wordWrap = true,
                    fontSize = baseSize - 2,
                };
                style.normal.textColor = TextColor;
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
            if (TabDisplayingWindow && LDB.items != null && !changesize)
            {
                TabItemWindow();
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
        /// Tab物品列表
        /// </summary>
        /// <param name="windId"></param>
        void TabItemWindow()
        {
            GUILayout.BeginHorizontal(new[] { GUILayout.Width(Screen.width), GUILayout.Height(Screen.height) });
            GUILayout.FlexibleSpace();
            TabscrollPosition = GUILayout.BeginScrollView(TabscrollPosition);
            GUILayout.BeginVertical();
            int itemsNum = LDB.items.Length;
            float buttonsize = BaseSize * 3;
            int lineNum = 10;
            int line = itemsNum / lineNum + ((itemsNum % lineNum) > 0 ? 1 : 0);
            var tempbuttonstyle = new GUIStyle(GUI.skin.button)
            {
                margin = new RectOffset(0, 0, 0, 0)
            };
            for (int i = 0; i < line; i++)
            {
                GUILayout.BeginHorizontal();
                for (int j = 0; j < lineNum; j++)
                {
                    int index = i * lineNum + j;
                    if (index == itemsNum) break;
                    var item = LDB.items.dataArray[index];
                    if (GUILayout.Button(item.iconSprite.texture, tempbuttonstyle, iconbuttonoptions))
                    {
                        Itemdelete_bool = false;
                        if (Input.GetKey(KeyCode.LeftControl))
                        {
                            GameMain.mainPlayer.TryAddItemToPackage(item.ID, StorageComponent.itemStackCount[item.ID], 0, true);
                        }
                        else
                        {
                            GameMain.mainPlayer.TryAddItemToPackage(item.ID, 1, 0, true);
                        }
                    }
                }
                GUILayout.EndHorizontal();
            }
            GUILayout.EndVertical();
            GUILayout.EndScrollView();
            GUILayout.EndHorizontal();
        }

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
                if (result.Contains("."))
                {
                    float.TryParse(Regex.Replace(t, @"^[^0-9]+(.[^0-9]{2})?$", ""), out temp);
                }
                else
                {
                    float.TryParse(Regex.Replace(t, @"^[^0-9]", ""), out temp);
                }
            }
            GUILayout.Label(propertyName.getTranslate(), style);
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
                    propertydata.ReactorPowerGen = PropertyDataUIDraw(propertydata.ReactorPowerGen, 1000000, 500000000, "核心功率", 1);
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
                QuickHandcraft.Value = GUILayout.Toggle(QuickHandcraft.Value, "机甲制造MAX".getTranslate());
                QuickPlayerMine.Value = GUILayout.Toggle(QuickPlayerMine.Value, "机甲采矿MAX".getTranslate());
                ItemList_bool.Value = GUILayout.Toggle(ItemList_bool.Value, "物品列表(Tab)".getTranslate());
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

                StationStoExtra.Value = (int)GUILayout.HorizontalSlider(StationStoExtra.Value, 0, 100, GUILayout.Width(heightdis * 4));
                GUILayout.Label("额外存储倍率".getTranslate() + ":" + StationStoExtra.Value, style);

                Stationfullenergy.Value = GUILayout.Toggle(Stationfullenergy.Value, "永久满电".getTranslate());
                StationfullCount = GUILayout.Toggle(StationfullCount, "要啥有啥".getTranslate());
                StationMaxproliferator.Value = GUILayout.Toggle(StationMaxproliferator.Value, "无限增产".getTranslate());
                StationSpray.Value = GUILayout.Toggle(StationSpray.Value, "内置喷涂".getTranslate());
                StationPowerGen.Value = GUILayout.Toggle(StationPowerGen.Value, "内置发电".getTranslate());
                Station_infiniteWarp_bool.Value = GUILayout.Toggle(Station_infiniteWarp_bool.Value, "无限翘曲".getTranslate());
                build_gascol_noequator.Value = GUILayout.Toggle(build_gascol_noequator.Value, "无需赤道造采集器".getTranslate());
                build_tooclose_bool.Value = GUILayout.Toggle(build_tooclose_bool.Value, "强行近距离建造物流站".getTranslate());
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
                    autochangestationname.Value = GUILayout.Toggle(autochangestationname.Value, "自动改名".getTranslate());
                    GUILayout.EndHorizontal();
                    GUILayout.BeginHorizontal();
                    GUILayout.Space(BaseSize);

                    GUILayout.BeginVertical();
                    GUILayout.Label("采矿速率".getTranslate() + ":" + Stationminenumber.Value, style);
                    Stationminenumber.Value = (int)GUILayout.HorizontalSlider(Stationminenumber.Value, 1, 100, GUILayout.Width(heightdis * 4));
                    GUILayout.EndVertical();
                    GUILayout.EndHorizontal();
                }

                GUILayout.EndVertical();

                GUILayout.EndHorizontal();

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

                GUILayout.Label("其它建筑功能".getTranslate() + ":", style);
                {
                    GUILayout.BeginHorizontal();

                    GUILayout.Space(BaseSize);

                    GUILayout.BeginVertical();
                    Buildmaxlen.Value = (int)GUILayout.HorizontalSlider(Buildmaxlen.Value, 15, 100, GUILayout.Width(heightdis * 4));
                    GUILayout.Label(Buildmaxlen.Value + " " + "建造数量最大值".getTranslate(), style);
                    BuildNotime_bool.Value = GUILayout.Toggle(BuildNotime_bool.Value, "建筑秒完成".getTranslate());
                    blueprintpastenoneed_bool.Value = GUILayout.Toggle(blueprintpastenoneed_bool.Value, "蓝图建造无需材料".getTranslate());
                    ArchitectMode.Value = GUILayout.Toggle(ArchitectMode.Value, "建筑师模式".getTranslate());
                    DriftBuildings = GUILayout.Toggle(DriftBuildings, "建筑抬升".getTranslate());
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
                GUILayout.EndVertical();

            }
            {
                GUILayout.BeginVertical();
                Quantumtransport_bool.Value = GUILayout.Toggle(Quantumtransport_bool.Value, "量子传输站".getTranslate());
                if (Quantumtransport_bool.Value)
                {
                    Quantumtransportpdwarp_bool.Value = GUILayout.Toggle(Quantumtransportpdwarp_bool.Value, "星球级翘曲全面供应".getTranslate());
                    Quantumtransportstarwarp_bool.Value = GUILayout.Toggle(Quantumtransportstarwarp_bool.Value, "星系级翘曲全面供应".getTranslate());
                    autochangeQuantumstationname = GUILayout.Toggle(autochangeQuantumstationname, "自动改名\"星球量子传输站\"".getTranslate());
                    autochangeQuantumStarstationname = GUILayout.Toggle(autochangeQuantumStarstationname, "自动改名\"星系量子传输站\"".getTranslate());
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
                    if (autochangeQuantumstationname) autochangeQuantumStarstationname = false;
                    if (autochangeQuantumStarstationname) autochangeQuantumstationname = false;
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
            string[] tempstr = new string[8] { "生成矿物", "删除矿物", "移动单矿", "移动矿堆", "不排列", "整理所有矿", "切割矿脉", "还原海洋" };
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
                }
                tempvalue = GUILayout.Toggle(tempvalue, tempstr[i].getTranslate());
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
                        GUILayout.Label("整理为".getTranslate() + veinlines.Value + "行".getTranslate());
                        veinlines.Value = (int)GUILayout.HorizontalSlider(veinlines.Value, 1, 20);
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
                        GUILayout.Label("切割出".getTranslate() + changeveinsposx.Value + "个".getTranslate());
                        changeveinsposx.Value = (int)GUILayout.HorizontalSlider(changeveinsposx.Value, 2, 72);

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
                }
            }
            GUILayout.EndVertical();
            GUILayout.Space(heightdis);
            GUILayout.BeginVertical();
            var minbuttonoptions = new GUILayoutOption[2] { GUILayout.Height(heightdis), GUILayout.MinWidth(heightdis * 4) };
            if (GUILayout.Button(LDB.items.Select(LDB.veins.Select(veintype).MiningItem).name, minbuttonoptions))
            {
                dropdownbutton = !dropdownbutton;
                addveinbool = false;
            }
            if (dropdownbutton)
            {
                for (int i = 1; i <= 14; i++)
                {
                    if (GUILayout.Button(LDB.items.Select(LDB.veins.Select(i).MiningItem).name, minbuttonoptions))
                    {
                        dropdownbutton = !dropdownbutton;
                        veintype = i;
                    }
                }
            }
            GUILayout.EndVertical();

            GUILayout.Space(heightdis);
            GUILayout.BeginVertical();
            tempstr = new string[10] { !restorewater ? "铺平整个星球" : "还原全部海洋", "铺平整个星球(地基)", "铺平整个星球(自定义颜色)", "掩埋全部矿", "删除全部矿", "超密铺采集器", "删除当前星球所有建筑", "删除当前星球所有建筑(不掉落)", "初始化当前星球", "初始化当前星球(不要海洋)" };
            for (int i = 0; i < tempstr.Length; i++)
            {
                if (GUILayout.Button(tempstr[i].getTranslate(), GUILayout.Height(heightdis)))
                {
                    switch (i)
                    {
                        case 0: MainFunction.OnSetBase(0); break;
                        case 1: MainFunction.OnSetBase(1); break;
                        case 2: MainFunction.OnSetBase(2); break;
                        case 3: MainFunction.BuryAllvein(); break;
                        case 4: MainFunction.RemoveAllvein(); break;
                        case 5: MainFunction.SetMaxGasStation(); break;
                        case 6: MainFunction.RemoveAllBuild(0); break;
                        case 7: MainFunction.RemoveAllBuild(1); break;
                        case 8: MainFunction.RemoveAllBuild(2); break;
                        case 9: MainFunction.RemoveAllBuild(3); break;
                    }
                }
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
            QuickabortSwarm.Value = GUILayout.Toggle(QuickabortSwarm.Value, "太阳帆秒吸收".getTranslate());
            if (alwaysemission.Value != GUILayout.Toggle(alwaysemission.Value, "全球打帆".getTranslate()))
            {
                alwaysemission.Value = !alwaysemission.Value;
                alwaysemissiontemp = alwaysemission.Value;
            }
            if (RandomEmission.Value != GUILayout.Toggle(RandomEmission.Value, "间隔发射".getTranslate()))
            {
                RandomEmission.Value = !RandomEmission.Value;
                MainFunction.RandomEmissionEjectorSilo();
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
                    sunlight_bool.Value = GUILayout.Toggle(sunlight_bool.Value, "夜灯".getTranslate());
                    CloseUIAbnormalityTip.Value = GUILayout.Toggle(CloseUIAbnormalityTip.Value, "关闭异常提示".getTranslate());
                    InspectDisNoLimit.Value = GUILayout.Toggle(InspectDisNoLimit.Value, "操作范围不受限制".getTranslate());
                    if (InspectDisNoLimit.Value)
                    {
                        GameMain.mainPlayer.mecha.buildArea = 200;
                    }
                    else
                    {
                        GameMain.mainPlayer.mecha.buildArea = 80;
                    }
                    Maxproliferator.Value = GUILayout.Toggle(Maxproliferator.Value, "增产点数上限10".getTranslate());
                    incAbility = Maxproliferator.Value ? 10 : 4;
                    pasteanyway = GUILayout.Toggle(pasteanyway, "蓝图强制粘贴".getTranslate());
                    PasteBuildAnyWay = GUILayout.Toggle(PasteBuildAnyWay, "建筑铺设无需条件".getTranslate());
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
                    GUILayout.BeginHorizontal();
                    GUILayout.Label("堆叠倍率".getTranslate() + ":", style);
                    stackmultipleStr = Regex.Replace(GUILayout.TextField(stackmultipleStr, 10, GUILayout.MinWidth(heightdis * 3)), @"[^0-9]", "");
                    int multiple = IntParseLimit(stackmultipleStr, 1, 5000000);
                    stackmultipleStr = multiple.ToString();
                    if (GUILayout.Button("设置".getTranslate()))
                    {
                        MainFunction.SetmultipleItemStatck(multiple);
                    }
                    GUILayout.EndHorizontal();
                    GUILayout.BeginHorizontal();
                    GUILayout.Label("冶炼倍数".getTranslate() + ":", style);
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
                    if (GUILayout.Button("GC".getTranslate()))
                    {
                        GC.Collect();
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
