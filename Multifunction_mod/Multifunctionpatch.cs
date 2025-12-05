using HarmonyLib;
using Multifunction_mod.Patchs;
using static Multifunction_mod.Multifunction;

namespace Multifunction_mod
{
    public class Multifunctionpatch
    {
        public static Harmony harmony;
        public static int ability = 4;

        public static void Patchallmethod()
        {
            harmony = new Harmony(GUID);
            var m = typeof(StorageComponent).GetMethods();
            foreach (var i in m)
            {
                if (i.Name == "TakeTailItems" && i.ReturnType == typeof(void))
                {
                    var prefix = typeof(Multifunctionpatch).GetMethod("TakeTailItemsPatch");
                    harmony.Patch(i, new HarmonyMethod(prefix));
                    break;
                }
            }

            harmony.PatchAll(typeof(Multifunction_modGameLogic));
            harmony.PatchAll(typeof(DysonPatch));
            harmony.PatchAll(typeof(PlayerPatch));
            harmony.PatchAll(typeof(PowerSystemPatch));
            harmony.PatchAll(typeof(PlanetFactoryPatch));
            harmony.PatchAll(typeof(StorageComponentPatch));
            harmony.PatchAll(typeof(StationComponentPatch));
            harmony.PatchAll(typeof(FactorySystemPatch));
            harmony.PatchAll(typeof(CargoTrafficPatch));
            harmony.PatchAll(typeof(SomePatch));
            harmony.PatchAll(typeof(UIPatch));
            harmony.PatchAll(typeof(PlanetTransportPatch));
            harmony.PatchAll(typeof(TankComponentPatch));
            harmony.PatchAll(typeof(SpraycoaterComponentPatch));
            harmony.PatchAll(typeof(CombatPatch));
            harmony.PatchAll(typeof(BuildTool_ClickPatch));
        }

        public static bool TakeTailItemsPatch(StorageComponent __instance, ref int itemId)
        {
            if (ArchitectMode.Value)
            {
                if (itemId <= 0 || itemId >= 6007 || __instance == null || __instance.id != GameMain.mainPlayer.package.id) return true;
                if (LDB.items.Select(itemId).CanBuild) return false;
            }
            return true;
        }

    }

}
