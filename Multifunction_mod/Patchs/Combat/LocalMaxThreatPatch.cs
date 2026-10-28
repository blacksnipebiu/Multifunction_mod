using HarmonyLib;
using static Multifunction_mod.Multifunction;

namespace Multifunction_mod.Patchs.Combat
{
    /// <summary>
    /// 玩家所在星球永远满威胁：本地星球的黑雾基地威胁度拉满。
    /// </summary>
    public class LocalMaxThreatPatch
    {
        public static bool Enable
        {
            get { return PatchToggle<LocalMaxThreatPatch>.Enable; }
            set { PatchToggle<LocalMaxThreatPatch>.Enable = value; }
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(DFGBaseComponent), "UpdateFactoryThreat")]
        public static void DFGBaseComponentPreLogicTick(DFGBaseComponent __instance, ref float power_threat_factor, ref float player_threat)
        {
            if (__instance.evolve.threat < __instance.evolve.maxThreat && __instance.evolve.waveTicks == 0)
            {
                if (GameMain.localPlanet != null && __instance.groundSystem.factory.planet.index == GameMain.localPlanet.index)
                {
                    __instance.evolve.threat = __instance.evolve.maxThreat * 2 + 1;
                    __instance.evolve.threatshr = __instance.evolve.maxThreat * 2 + 1;
                }
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(DFGBaseComponent), "UpdateFactoryThreat")]
        public static void DFGBaseComponentLogicTick(DFGBaseComponent __instance, ref float power_threat_factor, ref float player_threat)
        {
            if (__instance.evolve.threat < __instance.evolve.maxThreat && __instance.evolve.waveTicks == 0)
            {
                if (GameMain.localPlanet != null && __instance.groundSystem.factory.planet.index == GameMain.localPlanet.index)
                {
                    __instance.evolve.threat = __instance.evolve.maxThreat * 2 + 1;
                    __instance.evolve.threatshr = __instance.evolve.maxThreat * 2 + 1;
                }
            }
        }
    }
}
