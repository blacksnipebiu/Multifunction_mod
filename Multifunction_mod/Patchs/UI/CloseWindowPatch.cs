using HarmonyLib;
using static Multifunction_mod.Multifunction;

namespace Multifunction_mod.Patchs.UI
{
    /// <summary>
    /// 关闭窗口快捷键：按 E 时若主界面窗口正在显示，则关闭它并吞掉本次按键。
    /// </summary>
    public class CloseWindowPatch
    {
        public static bool Enable
        {
            get { return PatchToggle<CloseWindowPatch>.Enable; }
            set { PatchToggle<CloseWindowPatch>.Enable = value; }
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(UIGame), "On_E_Switch")]
        public static bool CloseWindowPatchMethod()
        {
            if (guidraw.DisplayingWindow)
            {
                guidraw.CloseMainWindow();
                return false;
            }
            return true;
        }
    }
}
