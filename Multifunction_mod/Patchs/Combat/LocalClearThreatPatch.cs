using HarmonyLib;
using static Multifunction_mod.Multifunction;

namespace Multifunction_mod.Patchs.Combat
{
    /// <summary>
    /// 星球永远0威胁：地面黑雾基地的威胁度与威胁因子持续清零。
    /// </summary>
    public class LocalClearThreatPatch
    {
        public static bool Enable
        {
            get { return PatchToggle<LocalClearThreatPatch>.Enable; }
            set { PatchToggle<LocalClearThreatPatch>.Enable = value; }
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(DFGBaseComponent), "UpdateFactoryThreat")]
        public static void DFGBaseComponentPreLogicTick(DFGBaseComponent __instance, ref float power_threat_factor, ref float player_threat)
        {
            power_threat_factor = 0;
            player_threat = 0;
            __instance.evolve.threat = 0;
            __instance.evolve.threatshr = 0;
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(DFGBaseComponent), "UpdateFactoryThreat")]
        public static void DFGBaseComponentLogicTick(DFGBaseComponent __instance, ref float power_threat_factor, ref float player_threat)
        {
            power_threat_factor = 0;
            player_threat = 0;
            __instance.evolve.threat = 0;
            __instance.evolve.threatshr = 0;
        }
    }
}
