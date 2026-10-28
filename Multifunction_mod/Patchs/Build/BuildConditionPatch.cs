using HarmonyLib;
using static Multifunction_mod.Multifunction;

namespace Multifunction_mod.Patchs.Build
{
    /// <summary>
    /// 建造无需条件（铺设 / 物流站）：跳过 BuildTool_Click 的 CheckBuildConditions 检查。
    /// 由 PasteBuildAnyWay 与 build_station_nocondition 两个开关共同控制，Enable 由调用方绑定为两者相或。
    /// </summary>
    public class BuildConditionPatch
    {
        public static bool Enable
        {
            get { return PatchToggle<BuildConditionPatch>.Enable; }
            set { PatchToggle<BuildConditionPatch>.Enable = value; }
        }

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
