using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Reflection.Emit;
using UnityEngine;
using static Multifunction_mod.Multifunction;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

namespace Multifunction_mod.Patchs
{
    public class SomePatch
    {


        [HarmonyPrefix]
        [HarmonyPatch(typeof(DispenserComponent), "InternalTick")]
        public static void DispenserComponentInternalTickPrefix(ref DispenserComponent __instance)
        {
            if (!Stationfullenergy.Value)
            {
                return;
            }
            __instance.energy = __instance.energyMax;
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(PropertySystem), "GetItemTotalProperty")]
        public static void GetItemTotalPropertyPatch(ref int __result)
        {
            if (!Property9999999)
                return;
            __result = int.MaxValue;
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(ItemProto), "isFluid")]
        public static void ItemProtoisFluidPatch(ref bool __result)
        {
            if (!Tankcontentall.Value)
            {
                return;
            }
            __result = true;
        }

    }
}
