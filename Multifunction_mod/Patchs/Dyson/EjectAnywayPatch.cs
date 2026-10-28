using HarmonyLib;
using Multifunction_mod.Meta;
using System.Collections.Generic;
using System.Reflection.Emit;
using static Multifunction_mod.Multifunction;

namespace Multifunction_mod.Patchs.Dyson
{
    /// <summary>
    /// 全球打帆：改写弹射器更新逻辑，忽略轨道/瞄准限制，任意位置都能持续发射太阳帆。
    /// 注意：类不能声明为 static —— PatchToggle&lt;T&gt; 的类型实参不能是静态类型（CS0718）。
    /// </summary>
    public class EjectAnywayPatch
    {
        public static string Patchid => ModInfo.GUID + "." + nameof(EjectAnywayPatch);

        public static bool Enable
        {
            get { return PatchToggle<EjectAnywayPatch>.Enable; }
            set { PatchToggle<EjectAnywayPatch>.Enable = value; }
        }

        [HarmonyTranspiler, HarmonyPriority(Priority.Last)]
        [HarmonyPatch(typeof(EjectorComponent), nameof(EjectorComponent.InternalUpdate))]
        private static IEnumerable<CodeInstruction> EjectorComponent_InternalUpdate_Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            var matcher = new CodeMatcher(instructions, generator);
            matcher.End().MatchBack(
                true,
                new CodeMatch(OpCodes.Ldarg_0),
                    new CodeMatch(OpCodes.Ldfld, AccessTools.Field(typeof(EjectorComponent), nameof(EjectorComponent.autoOrbit)))
            ).Insert(
                 //设置 this.targetState = EjectorComponent.ETargetState.OK
                new CodeInstruction(OpCodes.Ldarg_0), // 加载 this
                    new CodeInstruction(OpCodes.Ldc_I4_1),  // 加载 EjectorComponent.ETargetState.OK
                    new CodeInstruction(OpCodes.Stfld, AccessTools.Field(typeof(EjectorComponent), nameof(EjectorComponent.targetState))), // 存储到 this.targetState

                    new CodeInstruction(OpCodes.Ldc_I4_1), // 加载 true
                    new CodeInstruction(OpCodes.Stloc_3) // 存储到 flag
            );
            return matcher.InstructionEnumeration();
        }
    }
}
