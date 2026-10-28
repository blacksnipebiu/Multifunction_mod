using HarmonyLib;
using static Multifunction_mod.Multifunction;

namespace Multifunction_mod.Patchs.Build
{
    /// <summary>
    /// 建筑不耗电：升级后的建筑（非物流站）工作/待机耗能清零。
    /// </summary>
    public class BuildingNoConsumePatch
    {
        public static bool Enable
        {
            get { return PatchToggle<BuildingNoConsumePatch>.Enable; }
            set { PatchToggle<BuildingNoConsumePatch>.Enable = value; }
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(PlanetFactory), "UpgradeEntityWithComponents")]
        public static void UpgradeEntityWithComponentsPatch(int entityId, PlanetFactory __instance)
        {
            if (entityId == 0 || __instance.entityPool[entityId].id == 0)
                return;
            if (GameMain.localPlanet.factory.entityPool[entityId].stationId > 0)
            {
                return;
            }
            int powerConId = __instance.entityPool[entityId].powerConId;
            if (powerConId <= 0)
            {
                return;
            }
            __instance.powerSystem.consumerPool[powerConId].idleEnergyPerTick = 0;
            __instance.powerSystem.consumerPool[powerConId].workEnergyPerTick = 0;
        }
    }
}
