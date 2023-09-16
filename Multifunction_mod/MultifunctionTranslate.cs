using System.Collections.Generic;

namespace Multfunction_mod
{
    public static class MultifunctionTranslate
    {
        private static Dictionary<string, string> TranslateDict = new Dictionary<string, string>();
        public static string getTranslate(this string s) => Localization.language != Language.zhCN && TranslateDict.ContainsKey(s) && TranslateDict[s].Length > 0 ? TranslateDict[s] : s;
        public static void regallTranslate()
        {
            TranslateDict.Clear();
            TranslateDict.Add("OP面板", "OP Pannel");
            TranslateDict.Add("人物", "Player");
            TranslateDict.Add("建筑", "Building");
            TranslateDict.Add("星球", "Planet");
            TranslateDict.Add("戴森球", "DysonSphere");
            TranslateDict.Add("其它功能", "Other");
            TranslateDict.Add("机甲物流", "Mechalogistics");
            TranslateDict.Add("防止面板穿透", "ClosePanel");

            TranslateDict.Add("走路速度", "Move speed");
            TranslateDict.Add("研发速度", "Research speed");
            TranslateDict.Add("小飞机速度", "Construction drones flight speed");
            TranslateDict.Add("小飞机任务点数", "Construction drone task count");
            TranslateDict.Add("小飞机数量", "Construction drones");
            TranslateDict.Add("采矿速度", "Mecha mining speed");
            TranslateDict.Add("制造速度", "Mecha produce speed");
            TranslateDict.Add("最大航行速度", "Max sail speed");
            TranslateDict.Add("最大曲速", "Max warp speed");
            TranslateDict.Add("跳跃速度", "Jump speed");
            TranslateDict.Add("建造范围", "Building range");
            TranslateDict.Add("核心功率", "Core Generation");
            TranslateDict.Add("运输机速度", "Logistics drone flight speed");
            TranslateDict.Add("运输船速度", "Logistics vessel navigate speed");
            TranslateDict.Add("运输机载量", "Logistics drone carrying capacity");
            TranslateDict.Add("运输船载量", "Logistics vessel carrying capacity");
            TranslateDict.Add("采矿机速度倍率", "Mining speed");
            TranslateDict.Add("极速分拣器数量", "Sorter cargo stacking");
            TranslateDict.Add("建筑堆叠高度", "Vertical construction");
            TranslateDict.Add("货物集装数量", "Output Cargo Stack Count");
            TranslateDict.Add("小飞机不耗能", "Construction drones no consumption");
            TranslateDict.Add("无需翘曲器曲速飞行", "Mecha don't need space warper");
            TranslateDict.Add("无限物品", "Infinite item");
            TranslateDict.Add("锁定背包", "Lock the package");
            TranslateDict.Add("建筑师模式", "ArchitectMode");
            TranslateDict.Add("无限机甲能量", "Infinite mecha energy");
            TranslateDict.Add("不往背包放东西", "Don't put things in the player's package");
            TranslateDict.Add("建筑秒完成", "Construction completed in seconds");
            TranslateDict.Add("蓝图建造无需材料", "No materials required for blueprint construction");
            TranslateDict.Add("科技点击解锁", "Technology click to unlock");
            TranslateDict.Add("物品列表(Tab)", "ItemList(Tab)");
            TranslateDict.Add("物流背包堆叠倍率", "Delivery Package Stack Size");

            TranslateDict.Add("以下功能需改名", "The following features need to be renamed");
            TranslateDict.Add("无限元数据", "Infinite Property");
            TranslateDict.Add("无限沙土", "Infinite Sand");
            TranslateDict.Add("停止修改", "Stop change value");
            TranslateDict.Add("启动修改", "Start change value");
            TranslateDict.Add("清空背包", "Clear the package");
            TranslateDict.Add("初始化玩家", "Initialize player");
            TranslateDict.Add("增加背包大小", "Add package size");
            TranslateDict.Add("减少背包大小", "Reduce package size");
            TranslateDict.Add("解锁全部科技", "Unlock all tech.");
            TranslateDict.Add("回退无穷科技", "Lock all Infinite tech.");
            TranslateDict.Add("初始化太阳帆轨道", "Initialize the solar sail orbit");
            TranslateDict.Add("点击删除下列戴森壳层级", "Click to delete the following Dyson shell layers");
            TranslateDict.Add("注意事项:戴森云和戴森壳不要出现一层轨道都没有的情况(用前存档)", "Note: The Dyson cloud and Dyson shell do not have a layer of orbits (save before use)");

            TranslateDict.Add("建造数量最大值", "Max buildings num");
            TranslateDict.Add("额外存储倍率", "Station storage extra multiply");
            TranslateDict.Add("物流站功能", "Station storage functions");
            TranslateDict.Add("储液站功能", "Storage tank functions");
            TranslateDict.Add("其它建筑功能", "Other building functions");
            TranslateDict.Add("实时更改全部物流站存储倍率", "Real-time changes Station storage max");
            TranslateDict.Add("无限储液站", "Infinite storage tank");
            TranslateDict.Add("星球无限供货机", "Planet infinite item station");
            TranslateDict.Add("要啥有啥", "All stations infinite item");
            TranslateDict.Add("无需赤道造采集器", "Collector no equatorial condition");
            TranslateDict.Add("强行近距离建造物流站", "Station no too close condition");
            TranslateDict.Add("无限翘曲", "Station infinite space warpers");
            TranslateDict.Add("人造恒星无限能源", "Artificial star infinite power");
            TranslateDict.Add("风力涡轮机无限能源", "Wind Turbines Unlimited Energy");
            TranslateDict.Add("极速轨道弹射器(慎用)", "Extremely quick Ejector(Danger)");
            TranslateDict.Add("极速垂直发射井(慎用)", "Extremely quick Silo(Danger");
            TranslateDict.Add("星球电网(人造恒星)", "Plnaet Power(Artificial star)");
            TranslateDict.Add("电力设备", "Electrical equipment");
            TranslateDict.Add("覆盖全球", "Global coverage");
            TranslateDict.Add("超长连接", "Long connection");
            TranslateDict.Add("新建设备不耗电", "New buildings dont't require power");
            TranslateDict.Add("永久满电", "Stations full energy");
            TranslateDict.Add("无限增产", "Station Max proliferator");
            TranslateDict.Add("传送带信号功能", "BeltSignal Function");
            TranslateDict.Add("储液站任意存", "Anything can store in water tank");
            TranslateDict.Add("量子耗能/个", "Quantum comsume energy each");
            TranslateDict.Add("量子传输站", "Quantum Transmission Station");
            TranslateDict.Add("星球级材料供应", "Planet Material supply");
            TranslateDict.Add("星球级物流站材料供应", "Planet Station Material supply");
            TranslateDict.Add("星球级矿机材料供应", "Planet Mining facility Material supply");
            TranslateDict.Add("星球级发射井弹射器材料供应", "Planet Silo and Ejector Material supply");
            TranslateDict.Add("星球级研究站材料供应", "Planet Lab Material supply");
            TranslateDict.Add("星球级电力设备材料供应", "Planet Power facility Material supply");
            TranslateDict.Add("星球级组装机材料供应", "Planet Assmeblers Material supply");
            TranslateDict.Add("星球级翘曲全面供应", "Planet Quantum Transmission Station warper supply");
            TranslateDict.Add("星系级翘曲全面供应", "Star Quantum Transmission Station warper supply");
            TranslateDict.Add("自动改名\"星球量子传输站\"", "auto. change name\"星球量子传输站\"");
            TranslateDict.Add("自动改名\"星系量子传输站\"", "auto. change name\"星系量子传输站\"");
            TranslateDict.Add("星系量子不突破上限", "Star Quantum Transmission Station don't exceed the upper limit");
            TranslateDict.Add("星球量子充电功率", "Planet Quantum Transmission Station max charging power");
            TranslateDict.Add("星系量子充电功率", "Star Quantum Transmission Station max charging power");

            TranslateDict.Add("\"生成矿物\":鼠标左键生成矿物，鼠标右键取消。\"删除矿物\"：按x键进入拆除模式可拆除矿物。", "\"Generate Vein\":left mouse button generate vein，right mouse button cancel.\"delete vein\"：Press the x key to enter the removal mode to delete the vein");
            TranslateDict.Add("生成矿物", "Generate Vein");
            TranslateDict.Add("移动单矿", "Change position of the vein");
            TranslateDict.Add("移动矿堆", "Change position of the veingroup");
            TranslateDict.Add("获取所有矿", "Get all veins and change");
            TranslateDict.Add("删除矿物", "delete vein");
            TranslateDict.Add("还原海洋", "Restore the ocean");
            TranslateDict.Add("整理所有矿", "Organize all mines");
            TranslateDict.Add("切割出", "Cut out");
            TranslateDict.Add("整理为", "Organized as");
            TranslateDict.Add("个", "veins");
            TranslateDict.Add("行", "lines");
            TranslateDict.Add("无穷矿", "Infinite vein");
            TranslateDict.Add("铺平整个星球", "Pave the whole planet");
            TranslateDict.Add("还原全部海洋", "Restore the whole ocean");
            TranslateDict.Add("铺平整个星球(地基)", "Pave the whole planet(Foundation)");
            TranslateDict.Add("铺平整个星球(自定义颜色)", "Pave the whole planet(Custom color)");
            TranslateDict.Add("掩埋全部矿", "bury all veins");
            TranslateDict.Add("删除全部矿", "delete all veins");
            TranslateDict.Add("删除当前星球所有建筑", "Dismantle all buildings");
            TranslateDict.Add("删除当前星球所有建筑(不掉落)", "Dismantle all buildings(no drop)");
            TranslateDict.Add("星球矿机", "Station_miner");
            TranslateDict.Add("自动改名", "auto. change name\"Station_miner\"");
            TranslateDict.Add("星球矿机无消耗", "Station_miner no comsumption veins");
            TranslateDict.Add("采矿速率", "mining speed");
            TranslateDict.Add("垃圾站", "Station_trash");
            TranslateDict.Add("不需要沙土", "no need for soil pile");
            TranslateDict.Add("改变海洋类型", "Change water type");
            TranslateDict.Add("还原所有海洋类型", "Restore all water types");

            TranslateDict.Add("初始化当前戴森球", "Clear current DysonSphere");
            TranslateDict.Add("瞬间完成戴森球(用前存档)", "Finish DysonSphere(Remember to save)");
            TranslateDict.Add("保存戴森球", "save DysonSphere");
            TranslateDict.Add("打开戴森球蓝图文件夹", "Open DysonSphere Blueprint folder");
            TranslateDict.Add("导入戴森球", "Load DysonSphere");
            TranslateDict.Add("跳过太阳帆吸收阶段", "Skip the solar sail absorption phase");
            TranslateDict.Add("跳过太阳帆子弹阶段", "Skip the solar sail bullet stage");
            TranslateDict.Add("太阳帆秒吸收", "Quick abort solarsail");
            TranslateDict.Add("全球打帆", "Sail anywhere in the planet");
            TranslateDict.Add("间隔发射", "Eject average");
            TranslateDict.Add("开放戴森壳半径上下限(用前存档)", "Open the upper and lower limits of the Dyson shell radius (save before use)");

            TranslateDict.Add("最大半径", "maxOrbitRadius");
            TranslateDict.Add("最小半径", "minOrbitRadius");
            TranslateDict.Add("戴森云和戴森壳至少要有一层，否则会出bug", "Dyson swarm must have at least one orbit.Dyson shell must have al least one layer");

            TranslateDict.Add("丢垃圾速率", "throw trash speed");
            TranslateDict.Add("以下设置需要进入存档", "The follow config need to load game");
            TranslateDict.Add("丢垃圾(整活用)", "throw trash");
            TranslateDict.Add("夜灯", "Sunlight");
            TranslateDict.Add("蓝图强制粘贴", "Enforce to paste blueprint");
            TranslateDict.Add("成就重新检测", "Recheck Achievements");
            TranslateDict.Add("以下设置需要重启游戏", "The follow config need restart the game");
            TranslateDict.Add("设置", "Set");
            TranslateDict.Add("冶炼倍数", "Multiple smelt");
            TranslateDict.Add("堆叠倍率", "Multiple stack");
            TranslateDict.Add("操作范围不受限制", "Inspect no limit");
            TranslateDict.Add("全部手搓", "All item can handcraft");
            TranslateDict.Add("无材料生产", "No material produce");
            TranslateDict.Add("快速生产", "Quick produce");

            TranslateDict.Add("窗口颜色及透明度", "Window color and alpha");
            TranslateDict.Add("应用", "Confirm");
            TranslateDict.Add("窗口字体颜色", "Window font color");
            TranslateDict.Add("快捷键", "QuickKey");
            TranslateDict.Add("改变窗口快捷键", "Change QuickKey");
            TranslateDict.Add("点击确认", "Click to confirm");


            TranslateDict.Add("储存", "Store");
            TranslateDict.Add("需求", "Need");
            TranslateDict.Add("回收", "Recycle");
            TranslateDict.Add("机甲物流专用星", "Mecha logistics Planet");
            TranslateDict.Add("回收使用储物仓", "Recycle use storage");
            TranslateDict.Add("需求使用储物仓", "Provide use storage");
            TranslateDict.Add("回收使用物流站", "Recycle use station");
            TranslateDict.Add("需求使用物流站", "Provide use station");
        }
    }
}
