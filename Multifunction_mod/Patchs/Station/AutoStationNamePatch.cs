using HarmonyLib;
using static Multifunction_mod.Multifunction;

namespace Multifunction_mod.Patchs.Station
{
    /// <summary>
    /// 物流站自动改名：新建的物流站按配置写入名字。
    /// </summary>
    public class AutoStationNamePatch
    {
        public static bool Enable
        {
            get { return PatchToggle<AutoStationNamePatch>.Enable; }
            set { PatchToggle<AutoStationNamePatch>.Enable = value; }
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(PlanetFactory), "CreateEntityLogicComponents")]
        public static void CreateEntityLogicComponentsPatch(PlanetFactory __instance, int entityId, PrefabDesc desc, int prebuildId)
        {
            if (desc.isStation && !string.IsNullOrEmpty(AutoChangeStationName.Value))
            {
                __instance.WriteExtraInfoOnPrebuild(prebuildId, AutoChangeStationName.Value.getTranslate());
            }
        }
    }
}
