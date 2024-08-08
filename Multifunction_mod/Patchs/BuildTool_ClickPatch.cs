using HarmonyLib;
using static Multifunction_mod.Multifunction;

namespace Multifunction_mod.Patchs
{
    public class BuildTool_ClickPatch
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(BuildTool_Click), "CheckBuildConditions")]
        public static bool CheckBuildConditionsPatchPrefix(BuildTool_Click __instance, ref bool __result)
        {
            if (!PasteBuildAnyWay && !build_station_nocondition.Value)
            {
                return true;
            }
            for (int i = 0; i < __instance.buildPreviews.Count; i++)
            {
                var prefab = __instance.buildPreviews[i]?.item?.prefabDesc;
                if (prefab != null && prefab.veinMiner)
                {
                    return true;
                }
                if (build_station_nocondition.Value && !PasteBuildAnyWay && !(prefab.isStation || prefab.isStation || prefab.isCollectStation))
                {
                    return true;
                }
            }
            __result = true;
            return false;
        }
    }
}
