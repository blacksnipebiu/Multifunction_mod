using HarmonyLib;
using static Multifunction_mod.Multifunction;

namespace Multifunction_mod.Patchs.MechaPatches
{
    /// <summary>
    /// 无翘曲器曲速：机甲使用翘曲器时始终返回成功，没有翘曲器也能曲速飞行。
    /// 注意：类不能声明为 static —— PatchToggle&lt;T&gt; 的类型实参不能是静态类型（CS0718）。
    /// </summary>
    public class NoWarperPatch
    {
        public static bool Enable
        {
            get { return PatchToggle<NoWarperPatch>.Enable; }
            set { PatchToggle<NoWarperPatch>.Enable = value; }
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(Mecha), "UseWarper")]
        public static void EnoughWarperPatch(ref bool __result)
        {
            __result = true;
        }
    }
}
