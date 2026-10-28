using HarmonyLib;
using static Multifunction_mod.Multifunction;

namespace Multifunction_mod.Patchs.Combat
{
    /// <summary>
    /// 玩家所在星系永远满威胁：本地星系的黑雾巢穴威胁度拉满。
    /// </summary>
    public class SpaceMaxThreatPatch
    {
        public static bool Enable
        {
            get { return PatchToggle<SpaceMaxThreatPatch>.Enable; }
            set { PatchToggle<SpaceMaxThreatPatch>.Enable = value; }
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(EnemyDFHiveSystem), "GameTickLogic")]
        public static void EnemyDFHiveSystemPreGameTickLogic(EnemyDFHiveSystem __instance)
        {
            if (__instance.evolve.threat < __instance.evolve.maxThreat && __instance.evolve.waveTicks == 0)
            {
                if (GameMain.localStar != null && __instance.starData.index == GameMain.localStar.index)
                {
                    __instance.evolve.threat = __instance.evolve.maxThreat * 2 + 1;
                    __instance.evolve.threatshr = __instance.evolve.maxThreat * 2 + 1;
                }
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(EnemyDFHiveSystem), "GameTickLogic")]
        public static void EnemyDFHiveSystemGameTickLogic(EnemyDFHiveSystem __instance)
        {
            if (__instance.evolve.threat < __instance.evolve.maxThreat && __instance.evolve.waveTicks == 0)
            {
                if (GameMain.localStar != null && __instance.starData.index == GameMain.localStar.index)
                {
                    __instance.evolve.threat = __instance.evolve.maxThreat * 2 + 1;
                    __instance.evolve.threatshr = __instance.evolve.maxThreat * 2 + 1;
                }
            }
        }
    }
}
