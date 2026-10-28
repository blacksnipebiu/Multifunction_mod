using HarmonyLib;
using static Multifunction_mod.Multifunction;

namespace Multifunction_mod.Patchs.Combat
{
    /// <summary>
    /// 太空永远0威胁：黑雾巢穴的威胁度持续清零。
    /// </summary>
    public class SpaceClearThreatPatch
    {
        public static bool Enable
        {
            get { return PatchToggle<SpaceClearThreatPatch>.Enable; }
            set { PatchToggle<SpaceClearThreatPatch>.Enable = value; }
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(EnemyDFHiveSystem), "GameTickLogic")]
        public static void EnemyDFHiveSystemPreGameTickLogic(EnemyDFHiveSystem __instance)
        {
            __instance.evolve.threat = 0;
            __instance.evolve.threatshr = 0;
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(EnemyDFHiveSystem), "GameTickLogic")]
        public static void EnemyDFHiveSystemGameTickLogic(EnemyDFHiveSystem __instance)
        {
            __instance.evolve.threat = 0;
            __instance.evolve.threatshr = 0;
        }
    }
}
