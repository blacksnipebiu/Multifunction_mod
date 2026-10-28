using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Multifunction_mod.Patchs.UI
{
    internal class UIControlPanelPatch
    {
        public static bool Enable
        {
            get { return PatchToggle<UIControlPanelPatch>.Enable; }
            set { PatchToggle<UIControlPanelPatch>.Enable = value; }
        }
        [HarmonyTranspiler]
        [HarmonyPatch(typeof(UIControlPanelStationInspector), "OnDroneIconClick")]
        public static IEnumerable<CodeInstruction> OnDroneIconClickPatch(IEnumerable<CodeInstruction> instructions)
        {
            var codeMacher = new CodeMatcher(instructions).
                MatchForward(false, new CodeMatch(OpCodes.Call, AccessTools.Method(typeof(UIControlPanelStationInspector), "get_isLocal")))
                .Set(OpCodes.Nop, null);
            return codeMacher.InstructionEnumeration();
        }
        [HarmonyTranspiler]
        [HarmonyPatch(typeof(UIControlPanelStationInspector), "OnShipIconClick")]
        public static IEnumerable<CodeInstruction> OnShipIconClickPatch(IEnumerable<CodeInstruction> instructions)
        {
            var codeMacher = new CodeMatcher(instructions).
                MatchForward(false, new CodeMatch(OpCodes.Call, AccessTools.Method(typeof(UIControlPanelStationInspector), "get_isLocal")))
                .Set(OpCodes.Nop, null);
            return codeMacher.InstructionEnumeration();
        }
        [HarmonyTranspiler]
        [HarmonyPatch(typeof(UIControlPanelStationInspector), "OnWarperIconClick")]
        public static IEnumerable<CodeInstruction> OnWarperIconClickPatch(IEnumerable<CodeInstruction> instructions)
        {
            var codeMacher = new CodeMatcher(instructions).
                MatchForward(false, new CodeMatch(OpCodes.Call, AccessTools.Method(typeof(UIControlPanelStationInspector), "get_isLocal")))
                .Set(OpCodes.Nop, null);
            return codeMacher.InstructionEnumeration();
        }

        [HarmonyTranspiler]
        [HarmonyPatch(typeof(UIControlPanelStationStorage), "OnItemIconMouseDown")]
        public static IEnumerable<CodeInstruction> OnItemIconMouseDownPatch(IEnumerable<CodeInstruction> instructions)
        {
            var codeMacher = new CodeMatcher(instructions).
                MatchForward(false, new CodeMatch(OpCodes.Call, AccessTools.Method(typeof(UIControlPanelStationStorage), "get_isLocal")))
                .Set(OpCodes.Nop, null)
                .MatchForward(false, new CodeMatch(OpCodes.Call, AccessTools.Method(typeof(UIControlPanelStationStorage), "get_isLocal")))
                .Set(OpCodes.Nop, null);
            return codeMacher.InstructionEnumeration();
        }

        [HarmonyTranspiler]
        [HarmonyPatch(typeof(UIControlPanelDispenserInspector), "OnItemIconMouseDown")]
        public static IEnumerable<CodeInstruction> OnDispenserItemIconMouseDownPatch(IEnumerable<CodeInstruction> instructions)
        {
            var codeMacher = new CodeMatcher(instructions).
                MatchForward(false, new CodeMatch(OpCodes.Call, AccessTools.Method(typeof(UIControlPanelDispenserInspector), "get_isLocal")))
                .Set(OpCodes.Nop, null)
                .MatchForward(false, new CodeMatch(OpCodes.Call, AccessTools.Method(typeof(UIControlPanelDispenserInspector), "get_isLocal")))
                .Set(OpCodes.Nop, null);
            return codeMacher.InstructionEnumeration();
        }
    }
}
