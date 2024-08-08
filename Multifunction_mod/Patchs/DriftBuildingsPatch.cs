using HarmonyLib;
using static Multifunction_mod.Multifunction;

namespace Multifunction_mod.Patchs
{
    public class DriftBuildingsPatch
    {
        public static bool IsPatched;
        [HarmonyPrefix]
        [HarmonyPatch(typeof(BuildTool_Click), "UpdatePreviewModels")]
        public static void Prefix(ref BuildTool_Click __instance)
        {
            if (!DriftBuildings)
                return;
            for (int i = 0; i < __instance.buildPreviews.Count; i++)
            {
                var bp = __instance.buildPreviews[i];
                bp.lpos *= DriftBuildingHeight;
                bp.lpos2 *= DriftBuildingHeight;
            }
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(BuildTool_BlueprintPaste), "CreatePrebuilds")]
        public static void BuildTool_BlueprintPasteCreatePrebuildsPatch(ref BuildTool_BlueprintPaste __instance)
        {
            if (!DriftBuildings)
                return;
            for (int i = 0; i < __instance.bpCursor; i++)
            {
                var bp = __instance.bpPool[i];
                if (bp.desc.isBelt)
                {
                    bp.lpos *= DriftBuildingHeight;
                    bp.lpos2 *= DriftBuildingHeight;
                }
            }
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(BuildTool_BlueprintPaste), "UpdatePreviewModels")]
        public static void BuildTool_BlueprintPastePrefix(ref BuildTool_BlueprintPaste __instance)
        {
            if (!DriftBuildings)
                return;
            for (int i = 0; i < __instance.bpCursor; i++)
            {
                var bp = __instance.bpPool[i];
                if (bp.desc.isBelt)
                {
                    bp.lpos *= DriftBuildingHeight;
                    bp.lpos2 *= DriftBuildingHeight;
                }
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(BuildTool_BlueprintPaste), "UpdatePreviewModels")]
        public static void BuildTool_BlueprintPastePostfix(ref BuildTool_BlueprintPaste __instance)
        {
            if (!DriftBuildings)
                return;
            for (int i = 0; i < __instance.bpCursor; i++)
            {
                var bp = __instance.bpPool[i];
                if (bp.desc.isBelt)
                {
                    bp.lpos /= DriftBuildingHeight;
                    bp.lpos2 /= DriftBuildingHeight;
                }

            }
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(BuildTool_BlueprintPaste), "AlterBPGPUIModel")]
        public static void BuildTool_BlueprintPasteAlterBPGPUIModelPatch(ref BuildPreview _bp)
        {
            if (!DriftBuildings)
                return;
            _bp.lpos *= DriftBuildingHeight;
            _bp.lpos2 *= DriftBuildingHeight;
        }
    }
}
