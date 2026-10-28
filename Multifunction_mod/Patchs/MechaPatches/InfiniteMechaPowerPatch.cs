using HarmonyLib;
using static Multifunction_mod.Multifunction;

namespace Multifunction_mod.Patchs.MechaPatches
{
    /// <summary>
    /// 无限机甲能量：机甲消耗能量时直接补满核心能量并返回最大值，不再扣除能量。
    /// 注意：类不能声明为 static —— PatchToggle&lt;T&gt; 的类型实参不能是静态类型（CS0718）。
    /// </summary>
    public class InfiniteMechaPowerPatch
    {
        public static bool Enable
        {
            get { return PatchToggle<InfiniteMechaPowerPatch>.Enable; }
            set { PatchToggle<InfiniteMechaPowerPatch>.Enable = value; }
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(Mecha), "UseEnergy")]
        public static bool InfiniteplayerpowerPatch(Mecha __instance, ref float __result)
        {
            __result = 1;
            __instance.coreEnergy = __instance.coreEnergyCap;
            return false;
        }
    }
}
