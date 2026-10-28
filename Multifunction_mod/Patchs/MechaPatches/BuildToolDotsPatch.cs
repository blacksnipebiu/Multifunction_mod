using HarmonyLib;
using UnityEngine;
using static Multifunction_mod.Multifunction;

namespace Multifunction_mod.Patchs.MechaPatches
{
    /// <summary>
    /// 建筑吸附点数量：BuildTool_Click 初始化时按配置的建筑数量最大值重建吸附点数组。
    /// 常驻补丁（无独立开关），由调用方在启动时把 Enable 置为 true。
    /// 注意：类不能声明为 static —— PatchToggle&lt;T&gt; 的类型实参不能是静态类型（CS0718）。
    /// </summary>
    public class BuildToolDotsPatch
    {
        public static bool Enable
        {
            get { return PatchToggle<BuildToolDotsPatch>.Enable; }
            set { PatchToggle<BuildToolDotsPatch>.Enable = value; }
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(BuildTool_Click), "_OnInit")]
        public static void DeterminePreviewsPatch(BuildTool_Click __instance)
        {
            __instance.dotsSnapped = new Vector3[Buildmaxlen.Value];
        }
    }
}
