using HarmonyLib;
using static Multifunction_mod.Multifunction;

namespace Multifunction_mod.Patchs.MechaPatches
{
    /// <summary>
    /// 机甲制造 MAX：机甲手工制造的每个任务在 Tick 时直接把进度补满，瞬间完成。
    /// 注意：类不能声明为 static —— PatchToggle&lt;T&gt; 的类型实参不能是静态类型（CS0718）。
    /// </summary>
    public class QuickHandcraftPatch
    {
        public static bool Enable
        {
            get { return PatchToggle<QuickHandcraftPatch>.Enable; }
            set { PatchToggle<QuickHandcraftPatch>.Enable = value; }
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(MechaForge), "GameTick")]
        public static void Prefix(ref MechaForge __instance)
        {
            if (__instance.tasks.Count > 0)
            {
                __instance.tasks[0].tick = __instance.tasks[0].tickSpend;
            }
        }
    }
}
