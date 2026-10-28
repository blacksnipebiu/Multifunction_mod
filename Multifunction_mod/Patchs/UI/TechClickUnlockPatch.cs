using HarmonyLib;
using Multifunction_mod.GameModify;
using static Multifunction_mod.Multifunction;

namespace Multifunction_mod.Patchs.UI
{
    /// <summary>
    /// 科技点击解锁：点选科技时补足其消耗统计并逐级解锁到满级。
    /// 说明：方法体沿用原 UIPatch 中的 TechnologyModify.AddConsumeItemToTotal(...) 调用。
    /// </summary>
    public class TechClickUnlockPatch
    {
        public static bool Enable
        {
            get { return PatchToggle<TechClickUnlockPatch>.Enable; }
            set { PatchToggle<TechClickUnlockPatch>.Enable = value; }
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(UITechTree), "_OnUpdate")]
        public static void UITechTreeSelectTechPatch(UITechTree __instance)
        {
            if (__instance.selected == null)
            {
                return;
            }
            TechProto techProto = __instance.selected.techProto;
            if (GameMain.history.techStates[techProto.ID].unlocked || techProto.MaxLevel > 10) return;
            if (techProto.Level < techProto.MaxLevel)
            {
                for (int level = 1; level < techProto.MaxLevel; ++level)
                {
                    for (int i = 0; i < techProto.itemArray.Length; i++)
                    {
                        TechnologyModify.AddConsumeItemToTotal(techProto.Items[i], (int)(techProto.ItemPoints[i] * techProto.GetHashNeeded(techProto.Level) / 3600));
                    }
                    GameMain.history.UnlockTechFunction(techProto.UnlockFunctions[0], techProto.UnlockValues[0], level);
                }
                GameMain.history.UnlockTech(techProto.ID);
            }
            if (!GameMain.history.techStates[techProto.ID].unlocked)
            {
                for (int i = 0; i < techProto.itemArray.Length; i++)
                {
                    TechnologyModify.AddConsumeItemToTotal(techProto.Items[i], (int)(techProto.ItemPoints[i] * techProto.GetHashNeeded(techProto.Level) / 3600));
                }
                GameMain.history.UnlockTech(techProto.ID);
            }
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(UITechTree), "UpdateScale")]
        public static bool UITechTreeUpdateScalePatch(UITechTree __instance)
        {
            if (__instance.selected != null || __instance.centerViewNode != null) return false;
            return true;
        }
    }
}
