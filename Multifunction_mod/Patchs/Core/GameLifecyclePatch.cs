using System;
using HarmonyLib;

namespace Multifunction_mod.Patchs.Core
{
    /// <summary>
    /// 游戏生命周期事件中枢（原 Multifunction_modGameLogic）。
    /// 顺序：OnDataLoaded → OnBeforeLoadGame → OnAfterLoadGame → OnGameBegin →（游戏运行）→ OnGameEnd。
    /// <list type="bullet">
    /// <item>OnDataLoaded：LDB/预载数据就绪（主菜单可用），由 VFPreload 预载线程结束时触发。</item>
    /// <item>OnBeforeLoadGame：即将开始读取存档文件（参数为存档名）；此时 GameMain.data 还是空的，星球/工厂都未加载。</item>
    /// <item>OnAfterLoadGame：存档数据已导入完成（bool 为是否成功）；星球与工厂组件仍在后台分帧加载，存档名可能为临时名。</item>
    /// <item>OnGameBegin：加载流程全部结束、游戏正式开始（GameLoader 第 10 帧调用 GameMain.Begin 之后），此时玩家/星球/工厂均可用。</item>
    /// <item>OnGameEnd：退出到主菜单之前触发。</item>
    /// </list>
    /// </summary>
    public class GameLifecyclePatch
    {
        /// <summary>预载数据（LDB）加载完成</summary>
        public static Action OnDataLoaded;

        /// <summary>开始读取存档文件前，参数为存档名</summary>
        public static Action<string> OnBeforeLoadGame;

        /// <summary>存档数据导入完成，参数为存档名与是否成功</summary>
        public static Action<string, bool> OnAfterLoadGame;

        /// <summary>游戏正式开始（完整加载结束）</summary>
        public static Action OnGameBegin;

        /// <summary>退出到主菜单前</summary>
        public static Action OnGameEnd;

        [HarmonyPostfix]
        [HarmonyPatch(typeof(VFPreload), "InvokeOnLoadWorkEnded")]
        public static void VFPreload_InvokeOnLoadWorkEnded_Postfix()
        {
            OnDataLoaded?.Invoke();
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(GameSave), "LoadCurrentGame")]
        public static void GameSave_LoadCurrentGame_Prefix(string saveName)
        {
            OnBeforeLoadGame?.Invoke(saveName);
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(GameSave), "LoadCurrentGame")]
        public static void GameSave_LoadCurrentGame_Postfix(string saveName, bool __result)
        {
            OnAfterLoadGame?.Invoke(saveName, __result);
        }

        [HarmonyPostfix, HarmonyPriority(Priority.First)]
        [HarmonyPatch(typeof(GameMain), nameof(GameMain.Begin))]
        public static void GameMain_Begin_Postfix()
        {
            OnGameBegin?.Invoke();
        }

        [HarmonyPostfix, HarmonyPriority(Priority.Last)]
        [HarmonyPatch(typeof(GameMain), nameof(GameMain.End))]
        public static void GameMain_End_Postfix()
        {
            OnGameEnd?.Invoke();
        }
    }
}
