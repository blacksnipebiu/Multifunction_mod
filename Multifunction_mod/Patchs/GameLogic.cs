using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Multifunction_mod.Patchs
{
    internal class GameLogic
    {
        public static Action OnDataLoaded;
        public static Action OnGameBegin;
        public static Action OnGameEnd;

        [HarmonyPostfix]
        [HarmonyPatch(typeof(VFPreload), "InvokeOnLoadWorkEnded")]
        public static void VFPreload_InvokeOnLoadWorkEnded_Postfix()
        {
            OnDataLoaded?.Invoke();
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
