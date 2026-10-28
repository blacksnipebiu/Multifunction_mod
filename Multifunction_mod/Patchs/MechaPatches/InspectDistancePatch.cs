using HarmonyLib;
using static Multifunction_mod.Multifunction;

namespace Multifunction_mod.Patchs.MechaPatches
{
    /// <summary>
    /// 操作范围不受限制：取对象选择距离时直接返回 400，扩大可交互范围。
    /// 注意：类不能声明为 static —— PatchToggle&lt;T&gt; 的类型实参不能是静态类型（CS0718）。
    /// </summary>
    public class InspectDistancePatch
    {
        public static bool Enable
        {
            get { return PatchToggle<InspectDistancePatch>.Enable; }
            set { PatchToggle<InspectDistancePatch>.Enable = value; }
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(PlayerAction_Inspect), "GetObjectSelectDistance")]
        public static void PlayerAction_InspectPatch(ref float __result)
        {
            __result = 400;
        }
    }
}
