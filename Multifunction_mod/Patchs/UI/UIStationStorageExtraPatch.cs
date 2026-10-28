using HarmonyLib;
using static Multifunction_mod.Multifunction;

namespace Multifunction_mod.Patchs.UI
{
    /// <summary>
    /// 运输站额外储量：按 StationStoExtra 配置增加物流站的仓储格数。
    /// </summary>
    public class UIStationStorageExtraPatch
    {
        public static bool Enable
        {
            get { return PatchToggle<UIStationStorageExtraPatch>.Enable; }
            set { PatchToggle<UIStationStorageExtraPatch>.Enable = value; }
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(UIStationStorage), "GetAdditionStorage")]
        public static void UIStationWindow_OnOpen(UIStationStorage __instance, ref int __result)
        {
            if (__instance.station.isCollector)
                return;
            int basemax = __instance.station.isStellar ? 10000 : 5000;
            __result += StationStoExtra.Value * basemax;
        }
    }
}
