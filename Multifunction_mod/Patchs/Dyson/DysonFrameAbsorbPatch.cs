using HarmonyLib;
using static Multifunction_mod.Multifunction;

namespace Multifunction_mod.Patchs.Dyson
{
    /// <summary>
    /// 太阳帆帧吸收：每帧对满壳层节点按“每帧吸收个数”连续建造太阳帆，加速太阳帆吸收。
    /// 注意：类不能声明为 static —— PatchToggle&lt;T&gt; 的类型实参不能是静态类型（CS0718）。
    /// </summary>
    public class DysonFrameAbsorbPatch
    {
        public static bool Enable
        {
            get { return PatchToggle<DysonFrameAbsorbPatch>.Enable; }
            set { PatchToggle<DysonFrameAbsorbPatch>.Enable = value; }
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(DysonSphereLayer), "GameTick")]
        public static void DysonSphereLayerGameTick(ref DysonSphereLayer __instance, long gameTick)
        {
            int num = (int)(gameTick % 120L);
            for (int i = 1; i < __instance.nodeCursor; i++)
            {
                DysonNode dysonNode = __instance.nodePool[i];
                if (dysonNode?.id == i && dysonNode.id % 120 == num && dysonNode.sp == dysonNode.spMax)
                {
                    for (int j = 1; j <= Solarsailsabsorbeveryframe.Value; j++)
                    {
                        dysonNode.OrderConstructCp(gameTick, __instance.dysonSphere.swarm);
                    }
                }
            }
        }
    }
}
