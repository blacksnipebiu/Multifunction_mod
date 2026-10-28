using HarmonyLib;
using static Multifunction_mod.Multifunction;

namespace Multifunction_mod.Patchs.UI
{
    /// <summary>
    /// 启用传送：星图选中星球时显示快速旅行按钮，点击后直接快速旅行到该星球。
    /// </summary>
    public class StarmapTeleportPatch
    {
        public static bool Enable
        {
            get { return PatchToggle<StarmapTeleportPatch>.Enable; }
            set { PatchToggle<StarmapTeleportPatch>.Enable = value; }
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(UIStarmap), "UpdateCursorView")]
        public static void UIStarmapUpdateCursorView(UIStarmap __instance)
        {
            if (__instance.focusPlanet != null)
            {
                Player mainPlayer = GameMain.mainPlayer;
                __instance.fastTravelButton.gameObject.SetActive(mainPlayer.planetId != __instance.focusPlanet.planet.id && !mainPlayer.warping);
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(UIStarmap), "OnFastTravelButtonClick")]
        public static void UIStarmapOnFastTravelButtonClick(UIStarmap __instance)
        {
            if (__instance.focusPlanet != null)
            {
                GameMain.mainPlayer.controller.actionSail.StartFastTravelToPlanet(__instance.focusPlanet.planet);
            }
        }
    }
}
