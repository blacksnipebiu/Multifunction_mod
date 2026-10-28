using HarmonyLib;
using static Multifunction_mod.Multifunction;

namespace Multifunction_mod.Patchs.Storage
{
    /// <summary>
    /// 增产点数上限 10：喷涂机更新期间临时把 incAbility 提到 10，结束后还原。
    /// </summary>
    public class MaxProliferatorPatch
    {
        /// <summary>本次更新前的 incAbility，prefix 写入、postfix 还原</summary>
        private static int savedAbility;

        public static bool Enable
        {
            get { return PatchToggle<MaxProliferatorPatch>.Enable; }
            set { PatchToggle<MaxProliferatorPatch>.Enable = value; }
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(SpraycoaterComponent), "InternalUpdate")]
        public static void SpraycoaterComponentInternalUpdatePrefix(ref SpraycoaterComponent __instance)
        {
            savedAbility = __instance.incAbility;
            __instance.incAbility = 10;
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(SpraycoaterComponent), "InternalUpdate")]
        public static void SpraycoaterComponentInternalUpdatePostfix(ref SpraycoaterComponent __instance)
        {
            if (__instance.incAbility <= 4)
            {
                return;
            }
            __instance.incAbility = savedAbility;
        }
    }
}
