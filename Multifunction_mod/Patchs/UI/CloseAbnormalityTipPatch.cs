using HarmonyLib;

namespace Multifunction_mod.Patchs.UI
{
    /// <summary>
    /// 关闭异常提示：打开异常提示窗口时自动关闭。
    /// </summary>
    public class CloseAbnormalityTipPatch
    {
        public static bool Enable
        {
            get { return PatchToggle<CloseAbnormalityTipPatch>.Enable; }
            set { PatchToggle<CloseAbnormalityTipPatch>.Enable = value; }
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(UIAbnormalityTip), "_OnOpen")]
        public static void UIAbnormalityTipDeterminePatch(UIAbnormalityTip __instance)
        {
            __instance._Close();
        }
    }
}
