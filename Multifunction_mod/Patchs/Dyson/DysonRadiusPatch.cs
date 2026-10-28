using HarmonyLib;
using static Multifunction_mod.Multifunction;

namespace Multifunction_mod.Patchs.Dyson
{
    /// <summary>
    /// 戴森球半径上下限：初始化戴森球时把轨道半径下限固定为 100，上限改为配置的最大半径。
    /// 注意：类不能声明为 static —— PatchToggle&lt;T&gt; 的类型实参不能是静态类型（CS0718）。
    /// </summary>
    public class DysonRadiusPatch
    {
        public static bool Enable
        {
            get { return PatchToggle<DysonRadiusPatch>.Enable; }
            set { PatchToggle<DysonRadiusPatch>.Enable = value; }
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(DysonSphere), "Init")]
        public static void DysonSphereInit(ref DysonSphere __instance)
        {
            __instance.minOrbitRadius = 100;
            __instance.maxOrbitRadius = MaxOrbitRadiusConfig.Value;
        }
    }
}
