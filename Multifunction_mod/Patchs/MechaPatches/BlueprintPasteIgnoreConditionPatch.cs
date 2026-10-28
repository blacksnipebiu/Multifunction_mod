using HarmonyLib;
using static Multifunction_mod.Multifunction;

namespace Multifunction_mod.Patchs.MechaPatches
{
    /// <summary>
    /// 蓝图强制粘贴：蓝图建筑不满足建造条件时也直接通过检查（含矿脉建筑时仍走原逻辑）。
    /// 注意：类不能声明为 static —— PatchToggle&lt;T&gt; 的类型实参不能是静态类型（CS0718）。
    /// </summary>
    public class BlueprintPasteIgnoreConditionPatch
    {
        public static bool Enable
        {
            get { return PatchToggle<BlueprintPasteIgnoreConditionPatch>.Enable; }
            set { PatchToggle<BlueprintPasteIgnoreConditionPatch>.Enable = value; }
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(BuildTool_BlueprintPaste), "CheckBuildConditions")]
        public static bool Prefix(BuildTool_BlueprintPaste __instance, ref bool __result)
        {
            if (__instance.bpPool != null)
            {
                for (int i = 0; i < __instance.bpPool.Length; i++)
                {
                    var prefab = __instance.bpPool[i]?.item?.prefabDesc;
                    if (prefab != null && prefab.veinMiner)
                    {
                        return true;
                    }
                }
                __result = true;
                return false;
            }
            return true;
        }
    }
}
