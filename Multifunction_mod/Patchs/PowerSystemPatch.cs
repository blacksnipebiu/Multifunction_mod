using HarmonyLib;
using static Multifunction_mod.Multifunction;

namespace Multifunction_mod.Patchs
{
    public class PowerSystemPatch
    {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(PowerSystem), "NewGeneratorComponent")]
        public static void PowerSystemNewGeneratorComponent(ref int __result, PowerSystem __instance)
        {
            if (InfineteStarPower.Value && __instance.genPool[__result].fuelMask == 4)
            {
                __instance.genPool[__result].fuelId = 1803;
                __instance.genPool[__result].fuelCount = 100;
                __instance.genPool[__result].fuelEnergy = long.MaxValue;
                __instance.genPool[__result].genEnergyPerTick = 1000000000000;
            }
            if (WindturbinesUnlimitedEnergy.Value && __instance.genPool[__result].wind)
            {
                __instance.genPool[__result].genEnergyPerTick = 100_000_000_0000;
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(PowerSystem), "NewConsumerComponent")]
        public static void PowerSystemNewConsumerComponent(ref int __result, PowerSystem __instance)
        {
            if (!Buildingnoconsume.Value)
            {
                return;
            }
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

        [HarmonyPrefix]
        [HarmonyPatch(typeof(PowerSystem), "NewNodeComponent")]
        public static void NewNodeComponentPatchPrefix(PowerSystem __instance, ref int entityId, ref float conn, ref float cover)
        {
            var itemID = __instance.factory.entityPool[entityId].protoId;
            if (PlanetPower_bool.Value && itemID == 2210)
            {
                cover = GameMain.localPlanet.realRadius * 4;
                if (farconnectdistance)
                {
                    conn = GameMain.localPlanet.realRadius * 1.5f;
                    farconnectdistance = false;
                }
            }
            else if (Windturbinescovertheglobe.Value && itemID == 2203)
            {
                cover = GameMain.localPlanet.realRadius * 4;
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(PowerSystem), "NewNodeComponent")]
        public static void NewNodeComponentPatchPostfix(ref int __result, PowerSystem __instance)
        {
            if (!Buildingnoconsume.Value || GameMain.localPlanet.factory.entityPool[__instance.nodePool[__result].entityId].stationId > 0)
            {
                return;
            }
            __instance.nodePool[__result].requiredEnergy = 0;
            __instance.nodePool[__result].idleEnergyPerTick = 0;
        }

    }
}
