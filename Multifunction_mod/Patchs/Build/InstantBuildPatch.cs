using HarmonyLib;
using static Multifunction_mod.Multifunction;

namespace Multifunction_mod.Patchs.Build
{
    /// <summary>
    /// 建筑秒完成：把当前星球的预建造直接 BuildFinally。
    /// </summary>
    public class InstantBuildPatch
    {
        public static bool Enable
        {
            get { return PatchToggle<InstantBuildPatch>.Enable; }
            set { PatchToggle<InstantBuildPatch>.Enable = value; }
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(ConstructionSystem), "GameTick")]
        public static void ConstructionSystemGameTick(ConstructionSystem __instance)
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
