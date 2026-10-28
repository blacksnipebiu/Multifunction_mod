using HarmonyLib;
using static Multifunction_mod.Multifunction;

namespace Multifunction_mod.Patchs.Power
{
    /// <summary>建筑不耗电：新的用电设备/电力节点耗能清零（能量场发生器除外）</summary>
    public class BuildingNoConsumePowerPatch
    {
        public static bool Enable
        {
            get { return PatchToggle<BuildingNoConsumePowerPatch>.Enable; }
            set { PatchToggle<BuildingNoConsumePowerPatch>.Enable = value; }
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(PowerSystem), "NewConsumerComponent")]
        public static void PowerSystemNewConsumerComponent(ref int __result, PowerSystem __instance)
        {
            int entityId = __instance.consumerPool[__result].entityId;
            int modelIndex = __instance.factory.entityPool[entityId].modelIndex;

            if (modelIndex > 0)
            {
                ModelProto modelProto = LDB.models.modelArray[modelIndex];
                if (modelProto?.prefabDesc != null && modelProto.prefabDesc.isFieldGenerator)
                {
                    return;
                }
            }
            __instance.consumerPool[__result].requiredEnergy = 0;
            __instance.consumerPool[__result].idleEnergyPerTick = 0;
            __instance.consumerPool[__result].workEnergyPerTick = 0;
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(PowerSystem), "NewNodeComponent")]
        public static void NewNodeComponentPatchPostfix(ref int __result, PowerSystem __instance)
        {
            if (GameMain.localPlanet.factory.entityPool[__instance.nodePool[__result].entityId].stationId > 0)
            {
                return;
            }
            __instance.nodePool[__result].requiredEnergy = 0;
            __instance.nodePool[__result].idleEnergyPerTick = 0;
        }
    }
}
