using HarmonyLib;

namespace Multifunction_mod.Patchs.Build
{
    /// <summary>
    /// 拆除不回收：Enable=true 期间，拆除建筑不把物品放回背包。
    /// 只在 BuildModify.RemoveAllBuild(..., 1) 执行期间临时开启，用完立即关闭。
    /// </summary>
    public class DismantleNoItemPatch
    {
        public static bool Enable
        {
            get { return PatchToggle<DismantleNoItemPatch>.Enable; }
            set { PatchToggle<DismantleNoItemPatch>.Enable = value; }
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(PlanetFactory), "TakeBackItemsInEntity")]
        public static bool TakeBackItemsInEntityPatch()
        {
            return false;
        }
    }
}
