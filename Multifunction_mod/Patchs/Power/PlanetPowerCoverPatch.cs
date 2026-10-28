using HarmonyLib;
using static Multifunction_mod.Multifunction;

namespace Multifunction_mod.Patchs.Power
{
    /// <summary>星球级电力覆盖：电力感应塔覆盖全球（可选超远连接距离）</summary>
    public class PlanetPowerCoverPatch
    {
        public static bool Enable
        {
            get { return PatchToggle<PlanetPowerCoverPatch>.Enable; }
            set { PatchToggle<PlanetPowerCoverPatch>.Enable = value; }
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(PowerSystem), "NewNodeComponent")]
        public static void NewNodeComponentPatchPrefix(PowerSystem __instance, ref int entityId, ref float conn, ref float cover)
        {
            var itemID = __instance.factory.entityPool[entityId].protoId;
            if (itemID == 2210)
            {
                cover = GameMain.localPlanet.realRadius * 4;
                if (farconnectdistance)
                {
                    conn = GameMain.localPlanet.realRadius * 1.5f;
                    farconnectdistance = false;
                }
            }
        }
    }
}
