using HarmonyLib;
using static Multifunction_mod.Multifunction;

namespace Multifunction_mod.Patchs.Power
{
    /// <summary>风力涡轮机覆盖全球</summary>
    public class WindTurbineCoverPatch
    {
        public static bool Enable
        {
            get { return PatchToggle<WindTurbineCoverPatch>.Enable; }
            set { PatchToggle<WindTurbineCoverPatch>.Enable = value; }
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(PowerSystem), "NewNodeComponent")]
        public static void NewNodeComponentPatchPrefix(PowerSystem __instance, ref int entityId, ref float conn, ref float cover)
        {
            var itemID = __instance.factory.entityPool[entityId].protoId;
            if (itemID == 2203)
            {
                cover = GameMain.localPlanet.realRadius * 4;
            }
        }
    }
}
