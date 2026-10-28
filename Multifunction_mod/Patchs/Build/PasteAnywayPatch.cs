using HarmonyLib;
using UnityEngine;

namespace Multifunction_mod.Patchs.Build
{
    /// <summary>建筑铺设无需条件：插入机/附加物/传送带跳过建造条件检查</summary>
    public class PasteAnywayPatch
    {
        public static bool Enable
        {
            get { return PatchToggle<PasteAnywayPatch>.Enable; }
            set { PatchToggle<PasteAnywayPatch>.Enable = value; }
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(BuildTool_Inserter), "CheckBuildConditions")]
        public static bool BuildTool_InserterCheckBuildConditionsPatch(ref bool __result)
        {
            __result = true;
            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(BuildTool_Addon), "CheckBuildConditions")]
        public static bool BuildTool_AddonCheckBuildConditionsPatch(ref bool __result)
        {
            __result = true;
            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(BuildTool_Path), "CheckBuildConditions")]
        public static bool BuildTool_PathCheckBuildConditionsPatch(ref bool __result, BuildTool_Path __instance)
        {
            int count = __instance.buildPreviews.Count;
            float num39 = __instance.altitude;
            float num40 = __instance.altitude;
            float num41 = __instance.tilt;
            float num42 = __instance.tilt;

            if (count > 0)
            {
                num39 = (__instance.buildPreviews[0].lpos.magnitude - __instance.planet.realRadius - 0.2f) / 1.3333333f;
                num40 = (__instance.buildPreviews[count - 1].lpos.magnitude - __instance.planet.realRadius - 0.2f) / 1.3333333f;
                num39 = Mathf.Round(num39 * 100f) / 100f;
                num40 = Mathf.Round(num40 * 100f) / 100f;
                if ((double)num39 < 0.041)
                {
                    num39 = 0f;
                }
                if ((double)num40 < 0.041)
                {
                    num40 = 0f;
                }
                num41 = __instance.buildPreviews[0].tilt;
                num42 = __instance.buildPreviews[count - 1].tilt;
            }
            if (num39 > 0f || num40 > 0f || num41 != 0f || num42 != 0f)
            {
                BuildModel model3 = __instance.actionBuild.model;
                model3.cursorText += "<size=12>";
                if (num39 > 0f || num40 > 0f)
                {
                    string arg;
                    if (num39 == num40)
                    {
                        arg = string.Format("{0:0.##}", num39);
                    }
                    else
                    {
                        arg = string.Format("{0:0.##}\u2006～\u2006{1:0.##}", num39, num40);
                    }
                    BuildModel model4 = __instance.actionBuild.model;
                    model4.cursorText += string.Format("传送带高度提示".Translate(), arg);
                }
                if (num41 != 0f || num42 != 0f)
                {
                    string arg2;
                    if (num41 == num42)
                    {
                        arg2 = string.Format("{0:+0.#;-0.#;0}°", -num41);
                    }
                    else
                    {
                        arg2 = string.Format("{0:+0.#;-0.#;0}\u2006～\u2006{1:+0.#;-0.#;0}", -num41, -num42);
                    }
                    BuildModel model5 = __instance.actionBuild.model;
                    model5.cursorText += string.Format("传送带倾斜提示".Translate(), arg2);
                }
                BuildModel model6 = __instance.actionBuild.model;
                model6.cursorText += "</size>";
            }
            __result = true;
            return false;
        }
    }
}
