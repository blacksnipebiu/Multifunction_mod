using HarmonyLib;
using static Multifunction_mod.Multifunction;

namespace Multifunction_mod.Patchs.Planet
{
    /// <summary>
    /// 无限沙土：整平地形不消耗沙土，并把玩家沙土补满。
    /// </summary>
    public class InfiniteSandPatch
    {
        public static bool Enable
        {
            get { return PatchToggle<InfiniteSandPatch>.Enable; }
            set { PatchToggle<InfiniteSandPatch>.Enable = value; }
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(PlanetFactory), "ComputeFlattenTerrainReform")]
        public static void PlanetFactoryNoComsumeSand(ref int costSandCount)
        {
            if (GameMain.mainPlayer != null && GameMain.mainPlayer.sandCount < int.MaxValue)
            {
                GameMain.mainPlayer.SetSandCount(int.MaxValue, ESandSource.Other);
            }
            costSandCount = 0;
        }
    }
}
