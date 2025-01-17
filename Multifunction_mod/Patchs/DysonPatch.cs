using HarmonyLib;
using static Multifunction_mod.Multifunction;

namespace Multifunction_mod.Patchs
{
    public class DysonPatch
    {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(DysonSphere), "Init")]
        public static void DysonSphereInit(ref DysonSphere __instance)
        {
            if (ChangeDysonradius.Value)
            {
                __instance.minOrbitRadius = 100;
                __instance.maxOrbitRadius = MaxOrbitRadiusConfig.Value;
            }
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(DysonSphereLayer), "GameTick")]
        public static void DysonSphereLayerGameTick(ref DysonSphereLayer __instance, long gameTick)
        {
            if (!QuickabortSwarm.Value)
                return;
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

        [HarmonyPrefix]
        [HarmonyPatch(typeof(DysonSwarm), "AbsorbSail")]
        public static bool DysonSwarmPatch1(ref DysonSwarm __instance, ref bool __result, DysonNode node)
        {
            if (!quickabsorbsolar.Value)
            {
                return true;
            }
            if (__instance.expiryCursor == __instance.expiryEnding)
            {
                __result = false;
                return false;
            }
            __instance.expiryEnding--;
            if (__instance.expiryEnding < 0)
            {
                __instance.expiryEnding += __instance.sailCapacity;
            }
            int num = __instance.expiryEnding;
            int index = __instance.expiryOrder[num].index;
            if (__instance.expiryOrder[num].time == 0)
            {
                Assert.CannotBeReached();
                __result = false;
                return false;
            }
            __instance.expiryOrder[num].time = 0;
            __instance.expiryOrder[num].index = 0;
            if (node != null && node.ConstructCp() != null)
            {
                __instance.dysonSphere.productRegister[11903]++;
            }
            __instance.RemoveSolarSail(index);
            return false;
        }

        //[HarmonyPostfix]
        //[HarmonyPatch(typeof(MilkyWayWebClient), "SendReportRequest")]
        //public static void test()
        //{
        //    long num = (long)AccountData.me.userId;
        //    if (num == 0L)
        //    {
        //        num = (long)GameMain.data.gameDesc.galaxySeed + 10000000000L;
        //    }
        //    if (num == 0L)
        //    {
        //        return;
        //    }
        //    long userId = (long)GameMain.data.account.userId;
        //    int build = GameConfig.gameVersion.Build;
        //    long gameTick = GameMain.gameTick;
        //    double timeSinceStart = GlobalObject.timeSinceStart;
        //    int opCounter = GlobalObject.opCounter;
        //    double num2 = PerformanceMonitor.timeCostsShowing[1] * 1000.0;
        //    string text = "";
        //    int num3 = Math.Min(GameMain.multithreadSystem.usedThreadCnt, SystemInfo.processorCount);
        //    long num4 = PerformanceMonitor.dataLengths[1];
        //    string text2 = "";
        //    int num5 = (int)(FPSController.averageFPS + 0.5);
        //    int num6 = (int)(FPSController.averageUPS + 0.5);
        //    for (int i = 2; i < 43; i++)
        //    {
        //        double num7 = PerformanceMonitor.timeCostsShowing[i] * 1000.0;
        //        if (num7 > 0.0001)
        //        {
        //            ECpuWorkEntry ecpuWorkEntry = (ECpuWorkEntry)i;
        //            string text3 = ecpuWorkEntry.ToString() + "-" + num7.ToString("0.0000");
        //            text3 += "|";
        //            text += text3;
        //        }
        //    }
        //    for (int j = 2; j < 39; j++)
        //    {
        //        long num8 = PerformanceMonitor.dataLengths[j];
        //        if (num8 > 0L)
        //        {
        //            ESaveDataEntry esaveDataEntry = (ESaveDataEntry)j;
        //            string text4 = esaveDataEntry.ToString() + "-" + num8.ToString("0");
        //            text4 += "|";
        //            text2 += text4;
        //        }
        //    }

        //    var url = string.Format("{0}{1}?user_id={2}&owner_id={3}&version={4}&game_tick={5}&game_time={6:0.00}&game_exp={7}&cpu_time={8:0.0000}&cpu_detail={9}&thread_count={10}&data_len={11}&data_detail={12}&fps={13}&ups={14}&pwd=41917", new object[]
        //    {
        //    MilkyWayWebClient.galaxyServerAddress,
        //    MilkyWayWebClient.uxReportApi,
        //    num,
        //    userId,
        //    build,
        //    gameTick,
        //    timeSinceStart,
        //    opCounter,
        //    num2,
        //    text,
        //    num3,
        //    num4,
        //    text2,
        //    num5,
        //    num6
        //    });
        //    Console.WriteLine(url);
        //}

        [HarmonyPrefix]
        [HarmonyPatch(typeof(DysonSwarm), "GameTick")]
        public static void DysonSwarmPatch2(ref DysonSwarm __instance, long time)
        {
            if (playcancelsolarbullet && __instance.bulletCursor > 1)
            {
                int tempnum = 1;
                for (int i = 1; i < __instance.bulletCursor; i++)
                {
                    if (__instance.bulletPool[i].id == i)
                    {
                        tempnum++;
                    }
                    if (tempnum > 1) break;
                }
                if (tempnum == 1)
                {
                    __instance.bulletCursor = 1;
                    __instance.bulletCapacity = 128;
                    __instance.bulletRecycleCursor = 1;
                    __instance.bulletRecycle = new int[128];
                    __instance.bulletPool = new SailBullet[128];
                }
            }
            for (int i = tempsails.Count - 1; i >= 0; i--)
            {
                Tempsail tempsail = tempsails[i];
                if (tempsail != null)
                {
                    if (__instance.starData.index != tempsail.index) continue;
                    __instance.AddSolarSail(tempsail.ss, tempsail.orbitid, tempsail.time + time);
                }
                tempsails.RemoveAt(i);
            }
        }
    }
}