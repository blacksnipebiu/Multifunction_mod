using HarmonyLib;
using static Multifunction_mod.Multifunction;

namespace Multifunction_mod.Patchs.MechaPatches
{
    /// <summary>
    /// 机甲采矿 MAX：手动采矿时把采矿 Tick 直接拉到最大，植物与矿脉都瞬间采完。
    /// 注意：类不能声明为 static —— PatchToggle&lt;T&gt; 的类型实参不能是静态类型（CS0718）。
    /// </summary>
    public class QuickMinePatch
    {
        public static bool Enable
        {
            get { return PatchToggle<QuickMinePatch>.Enable; }
            set { PatchToggle<QuickMinePatch>.Enable = value; }
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(PlayerAction_Mine), "GameTick")]
        public static void PlayerAction_MineGameTickPatch(ref PlayerAction_Mine __instance)
        {
            if (__instance.miningId > 0 && GameMain.localPlanet != null)
            {
                var factory = __instance.player.factory;
                if (__instance.miningType == EObjectType.Vegetable)
                {
                    VegeData vegeData = factory.GetVegeData(__instance.miningId);
                    VegeProto vegeProto = LDB.veges.Select(vegeData.protoId);
                    if (vegeProto != null)
                    {
                        __instance.miningTick = LDB.veges.Select(factory.GetVegeData(__instance.miningId).protoId).MiningTime * 10000;
                    }
                }
                else if (__instance.miningType == EObjectType.Vein)
                {
                    VeinData veinData = factory.GetVeinData(__instance.miningId);
                    VeinProto veinProto = LDB.veins.Select((int)veinData.type);
                    if (veinProto != null)
                    {
                        __instance.miningTick = veinProto.MiningTime * 10000;
                    }
                }
            }
        }
    }
}
