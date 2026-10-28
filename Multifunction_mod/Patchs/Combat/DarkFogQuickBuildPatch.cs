using HarmonyLib;
using static Multifunction_mod.Multifunction;

namespace Multifunction_mod.Patchs.Combat
{
    /// <summary>
    /// 黑雾巢穴快速产怪：建造组件的能量、物质、点数补满，建造冷却清零。
    /// </summary>
    public class DarkFogQuickBuildPatch
    {
        public static bool Enable
        {
            get { return PatchToggle<DarkFogQuickBuildPatch>.Enable; }
            set { PatchToggle<DarkFogQuickBuildPatch>.Enable = value; }
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(EnemyBuilderComponent), "LogicTick")]
        public static void EnemyBuilderComponentBuildLogic_Ground(EnemyBuilderComponent __instance)
        {
            __instance.energy = __instance.maxEnergy;
            __instance.matter = __instance.maxMatter;
            __instance.sp = __instance.spMax;
            __instance.buildCDTime = 0;
        }
    }
}
