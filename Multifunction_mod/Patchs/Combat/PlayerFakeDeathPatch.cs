using HarmonyLib;
using System;
using static Multifunction_mod.Multifunction;

namespace Multifunction_mod.Patchs.Combat
{
    /// <summary>
    /// 玩家假死：黑雾认为本地玩家不存在、不可见，且不会被玩家目标仇恨。
    /// </summary>
    public class PlayerFakeDeathPatch
    {
        public static bool Enable
        {
            get { return PatchToggle<PlayerFakeDeathPatch>.Enable; }
            set { PatchToggle<PlayerFakeDeathPatch>.Enable = value; }
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(EnemyDFHiveSystem), "GameTickLogic")]
        public static void EnemyDFHiveSystemGameTickLogic(EnemyDFHiveSystem __instance)
        {
            if (__instance.isLocal)
            {
                __instance.local_player_exist_alive = false;
                __instance.local_player_in_range = false;
            }
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(HatredList), "HateTarget", new Type[] { typeof(ETargetType), typeof(int), typeof(int), typeof(int), typeof(EHatredOperation), })]
        public static bool HatredListHateTarget(ETargetType type)
        {
            if (type == ETargetType.Player)
            {
                return false;
            }
            return true;
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(EnemyDFGroundSystem), "GameTickLogic_Prepare")]
        public static void EnemyDFGroundSystemGameTickLogic(EnemyDFGroundSystem __instance)
        {
            if (__instance.isLocalLoaded)
            {
                __instance.local_player_exist_alive = false;
            }
        }
    }
}
