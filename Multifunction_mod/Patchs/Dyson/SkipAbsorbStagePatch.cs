using HarmonyLib;
using static Multifunction_mod.Multifunction;

namespace Multifunction_mod.Patchs.Dyson
{
    /// <summary>
    /// 跳过太阳帆吸收阶段：直接取用过期队列里的太阳帆并移除，省去吸收过程的等待。
    /// 注意：类不能声明为 static —— PatchToggle&lt;T&gt; 的类型实参不能是静态类型（CS0718）。
    /// </summary>
    public class SkipAbsorbStagePatch
    {
        public static bool Enable
        {
            get { return PatchToggle<SkipAbsorbStagePatch>.Enable; }
            set { PatchToggle<SkipAbsorbStagePatch>.Enable = value; }
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(DysonSwarm), "AbsorbSail")]
        public static bool DysonSwarmPatch1(ref DysonSwarm __instance, ref bool __result, DysonNode node)
        {
            if (__instance.expiryCursor == __instance.expiryEnding)
            {
                __result = false;
                return false;
            }
            __instance.expiryEnding--;
            if (__instance.expiryEnding < 0)
            {
                __instance.expiryEnding += __instance.sailCapacity;
            }
            int num = __instance.expiryEnding;
            int index = __instance.expiryOrder[num].index;
            if (__instance.expiryOrder[num].time == 0)
            {
                Assert.CannotBeReached();
                __result = false;
                return false;
            }
            __instance.expiryOrder[num].time = 0;
            __instance.expiryOrder[num].index = 0;
            if (node != null && node.ConstructCp() != null)
            {
                __instance.dysonSphere.productRegister[11903]++;
            }
            __instance.RemoveSolarSail(index);
            return false;
        }
    }
}
