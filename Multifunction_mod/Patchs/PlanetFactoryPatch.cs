using HarmonyLib;
using static Multifunction_mod.Multifunction;

namespace Multifunction_mod.Patchs
{
    public class PlanetFactoryPatch
    {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(PlanetFactory), "ComputeFlattenTerrainReform")]
        public static void PlanetFactoryNoComsumeSand(ref int costSandCount)
        {
            if (!InfiniteSand.Value)
            {
                return;
            }
            if (GameMain.mainPlayer != null && GameMain.mainPlayer.sandCount < int.MaxValue)
            {
                GameMain.mainPlayer.SetSandCount(int.MaxValue);
            }
            costSandCount = 0;
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(PlanetFactory), "UpgradeEntityWithComponents")]
        public static void UpgradeEntityWithComponentsPatch(int entityId, PlanetFactory __instance)
        {
            if (!Buildingnoconsume.Value || entityId == 0 || __instance.entityPool[entityId].id == 0)
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

        [HarmonyPrefix]
        [HarmonyPatch(typeof(PlanetFactory), "TakeBackItemsInEntity")]
        public static bool TakeBackItemsInEntityPatch()
        {
            return !entityitemnoneed;
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

        [HarmonyPrefix]
        [HarmonyPatch(typeof(ConstructionSystem), "GameTick")]
        public static void ConstructionSystemGameTick(ConstructionSystem __instance)
        {
            if (BuildNotime_bool.Value)
            {
                PlanetFactory planetFactory = player?.factory;
                if (GameMain.localPlanet == null || planetFactory == null)
                {
                    return;
                }

                PrebuildData[] prebuildPool = planetFactory.prebuildPool;
                if (planetFactory.prebuildCount <= 0)
                {
                    return;
                }

                int num = 0;
                PlanetFactory.batchBuild = true;
                HighStopwatch highStopwatch = new HighStopwatch();
                highStopwatch.Begin();
                planetFactory.BeginFlattenTerrain();
                for (int i = planetFactory.prebuildCursor - 1; i > 0; i--)
                {
                    if (prebuildPool[i].itemRequired > 0 && !Infinitething.Value && !ArchitectMode.Value) continue;
                    if (prebuildPool[i].id == i && !prebuildPool[i].isDestroyed)
                    {
                        planetFactory.BuildFinally(GameMain.mainPlayer, prebuildPool[i].id, false);
                        num++;
                    }
                }

                planetFactory.EndFlattenTerrain();

                PlanetFactory.batchBuild = false;
                if (num > 0)
                {
                    GameMain.localPlanet.physics?.raycastLogic?.NotifyBatchObjectRemove();
                    GameMain.localPlanet.audio?.SetPlanetAudioDirty();
                }
            }
        }


    }
}
