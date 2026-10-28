using HarmonyLib;
using Multifunction_mod.Meta;
using Multifunction_mod.Patchs;
using Multifunction_mod.Patchs.Build;
using Multifunction_mod.Patchs.Combat;
using Multifunction_mod.Patchs.Core;
using Multifunction_mod.Patchs.Dyson;
using Multifunction_mod.Patchs.MechaPatches;
using Multifunction_mod.Patchs.Misc;
using Multifunction_mod.Patchs.Planet;
using Multifunction_mod.Patchs.Power;
using Multifunction_mod.Patchs.Station;
using Multifunction_mod.Patchs.Storage;
using Multifunction_mod.Patchs.UI;
using static Multifunction_mod.Multifunction;

namespace Multifunction_mod
{
    public class Multifunctionpatch
    {
        public static Harmony harmony;

        public static void Patchallmethod()
        {
            harmony = new Harmony(ModInfo.GUID);
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

            //仍按整类挂载的补丁（内部逻辑较复杂/共享辅助方法，后续再拆）
            harmony.PatchAll(typeof(GameLifecyclePatch));
            harmony.PatchAll(typeof(CargoTrafficPatch));
            harmony.PatchAll(typeof(StationComponentPatch));
            harmony.PatchAll(typeof(FactorySystemPatch));
            harmony.PatchAll(typeof(PlanetTransportPatch));

            //可精细控制的补丁：初始 Enable 跟随配置，配置变化时动态挂载/卸载
            #region Dyson

            DysonRadiusPatch.Enable = ChangeDysonradius.Value;
            ChangeDysonradius.SettingChanged += (_, _) => DysonRadiusPatch.Enable = ChangeDysonradius.Value;

            DysonFrameAbsorbPatch.Enable = QuickabortSwarm.Value;
            QuickabortSwarm.SettingChanged += (_, _) => DysonFrameAbsorbPatch.Enable = QuickabortSwarm.Value;

            SkipAbsorbStagePatch.Enable = quickabsorbsolar.Value;
            quickabsorbsolar.SettingChanged += (_, _) => SkipAbsorbStagePatch.Enable = quickabsorbsolar.Value;

            EjectAnywayPatch.Enable = alwaysemission.Value;
            alwaysemission.SettingChanged += (_, _) => EjectAnywayPatch.Enable = alwaysemission.Value;

            SkipBulletPatch.Enable = cancelsolarbullet.Value;
            cancelsolarbullet.SettingChanged += (_, _) => SkipBulletPatch.Enable = cancelsolarbullet.Value;

            #endregion

            #region MechaPatches

            NoWarperPatch.Enable = noneedwarp.Value;
            noneedwarp.SettingChanged += (_, _) => NoWarperPatch.Enable = noneedwarp.Value;

            InfiniteMechaPowerPatch.Enable = Infiniteplayerpower.Value;
            Infiniteplayerpower.SettingChanged += (_, _) => InfiniteMechaPowerPatch.Enable = Infiniteplayerpower.Value;

            QuickHandcraftPatch.Enable = QuickHandcraft.Value;
            QuickHandcraft.SettingChanged += (_, _) => QuickHandcraftPatch.Enable = QuickHandcraft.Value;

            QuickMinePatch.Enable = QuickPlayerMine.Value;
            QuickPlayerMine.SettingChanged += (_, _) => QuickMinePatch.Enable = QuickPlayerMine.Value;

            InspectDistancePatch.Enable = InspectDisNoLimit.Value;
            InspectDisNoLimit.SettingChanged += (_, _) => InspectDistancePatch.Enable = InspectDisNoLimit.Value;

            DismantleNoDropToPackagePatch.Enable = dismantle_but_nobuild.Value;
            dismantle_but_nobuild.SettingChanged += (_, _) => DismantleNoDropToPackagePatch.Enable = dismantle_but_nobuild.Value;

            //常驻补丁
            BuildToolDotsPatch.Enable = true;
            BlueprintPasteIgnoreConditionPatch.Enable = pasteanyway;

            #endregion

            #region Combat

            LockPlayerHpPatch.Enable = LockPlayerHp.Value;
            LockPlayerHp.SettingChanged += (_, _) => LockPlayerHpPatch.Enable = LockPlayerHp.Value;

            LockBuildHpPatch.Enable = LockBuildHp.Value;
            LockBuildHp.SettingChanged += (_, _) => LockBuildHpPatch.Enable = LockBuildHp.Value;

            LockEnemyHpPatch.Enable = LockEnemyHp.Value;
            LockEnemyHp.SettingChanged += (_, _) => LockEnemyHpPatch.Enable = LockEnemyHp.Value;

            PlayerQuickAttackPatch.Enable = PlayerQuickAttack.Value;
            PlayerQuickAttack.SettingChanged += (_, _) => PlayerQuickAttackPatch.Enable = PlayerQuickAttack.Value;

            KillEnemyQueuePatch.Enable = ClickKillEnemy.Value;
            ClickKillEnemy.SettingChanged += (_, _) => KillEnemyQueuePatch.Enable = ClickKillEnemy.Value;

            SpaceClearThreatPatch.Enable = SpaceAlwaysClearThreat.Value;
            SpaceAlwaysClearThreat.SettingChanged += (_, _) => SpaceClearThreatPatch.Enable = SpaceAlwaysClearThreat.Value;

            SpaceMaxThreatPatch.Enable = SpaceAlwaysMaxThreat.Value;
            SpaceAlwaysMaxThreat.SettingChanged += (_, _) => SpaceMaxThreatPatch.Enable = SpaceAlwaysMaxThreat.Value;

            LocalClearThreatPatch.Enable = LocalAlwaysClearThreat.Value;
            LocalAlwaysClearThreat.SettingChanged += (_, _) => LocalClearThreatPatch.Enable = LocalAlwaysClearThreat.Value;

            LocalMaxThreatPatch.Enable = LocalAlwaysMaxThreat.Value;
            LocalAlwaysMaxThreat.SettingChanged += (_, _) => LocalMaxThreatPatch.Enable = LocalAlwaysMaxThreat.Value;

            DarkFogQuickBuildPatch.Enable = DarkFogBuilderQuickBuild.Value;
            DarkFogBuilderQuickBuild.SettingChanged += (_, _) => DarkFogQuickBuildPatch.Enable = DarkFogBuilderQuickBuild.Value;

            TurretSupernovaPatch.Enable = TurrentKeepSuperNoval.Value;
            TurrentKeepSuperNoval.SettingChanged += (_, _) => TurretSupernovaPatch.Enable = TurrentKeepSuperNoval.Value;

            PlayerFakeDeathPatch.Enable = PlayerFakeDeath.Value;
            PlayerFakeDeath.SettingChanged += (_, _) => PlayerFakeDeathPatch.Enable = PlayerFakeDeath.Value;

            #endregion

            #region Build / Planet / Power

            InfiniteSandPatch.Enable = InfiniteSand.Value;
            InfiniteSand.SettingChanged += (_, _) => InfiniteSandPatch.Enable = InfiniteSand.Value;

            BuildingNoConsumePatch.Enable = Buildingnoconsume.Value;
            BuildingNoConsumePowerPatch.Enable = Buildingnoconsume.Value;
            Buildingnoconsume.SettingChanged += (_, _) =>
            {
                BuildingNoConsumePatch.Enable = Buildingnoconsume.Value;
                BuildingNoConsumePowerPatch.Enable = Buildingnoconsume.Value;
            };

            InstantBuildPatch.Enable = BuildNotime_bool.Value;
            BuildNotime_bool.SettingChanged += (_, _) => InstantBuildPatch.Enable = BuildNotime_bool.Value;

            BuildConditionPatch.Enable = PasteBuildAnyWay || build_station_nocondition.Value;
            build_station_nocondition.SettingChanged += (_, _) => BuildConditionPatch.Enable = PasteBuildAnyWay || build_station_nocondition.Value;

            AutoStationNamePatch.Enable = !string.IsNullOrEmpty(AutoChangeStationName.Value);
            AutoChangeStationName.SettingChanged += (_, _) => AutoStationNamePatch.Enable = !string.IsNullOrEmpty(AutoChangeStationName.Value);

            InfiniteStarPowerPatch.Enable = InfineteStarPower.Value;
            InfineteStarPower.SettingChanged += (_, _) => InfiniteStarPowerPatch.Enable = InfineteStarPower.Value;

            WindTurbineUnlimitedEnergyPatch.Enable = WindturbinesUnlimitedEnergy.Value;
            WindturbinesUnlimitedEnergy.SettingChanged += (_, _) => WindTurbineUnlimitedEnergyPatch.Enable = WindturbinesUnlimitedEnergy.Value;

            PlanetPowerCoverPatch.Enable = PlanetPower_bool.Value;
            PlanetPower_bool.SettingChanged += (_, _) => PlanetPowerCoverPatch.Enable = PlanetPower_bool.Value;

            WindTurbineCoverPatch.Enable = Windturbinescovertheglobe.Value;
            Windturbinescovertheglobe.SettingChanged += (_, _) => WindTurbineCoverPatch.Enable = Windturbinescovertheglobe.Value;

            #endregion

            #region Storage / Misc

            StackMultiplePatch.Enable = StackMultiple.Value > 1;
            StackMultiple.SettingChanged += (_, _) => StackMultiplePatch.Enable = StackMultiple.Value > 1;

            ArchitectModeItemCountPatch.Enable = ArchitectMode.Value;
            ArchitectMode.SettingChanged += (_, _) => ArchitectModeItemCountPatch.Enable = ArchitectMode.Value;

            TankMaxProliferatorPatch.Enable = TankMaxproliferator.Value;
            TankMaxproliferator.SettingChanged += (_, _) => TankMaxProliferatorPatch.Enable = TankMaxproliferator.Value;

            MaxProliferatorPatch.Enable = Maxproliferator.Value;
            Maxproliferator.SettingChanged += (_, _) => MaxProliferatorPatch.Enable = Maxproliferator.Value;

            DispenserFullEnergyPatch.Enable = Stationfullenergy.Value;
            Stationfullenergy.SettingChanged += (_, _) => DispenserFullEnergyPatch.Enable = Stationfullenergy.Value;

            TankContentAllPatch.Enable = Tankcontentall.Value;
            Tankcontentall.SettingChanged += (_, _) => TankContentAllPatch.Enable = Tankcontentall.Value;

            //常驻补丁 + bool 型开关（由 GUI 负责同步 Enable）
            UnlimitedMetadataPatch.Enable = Property9999999;

            #endregion

            #region UI

            CloseWindowPatch.Enable = true;

            InstantItemPatch.Enable = isInstantItem.Value;
            isInstantItem.SettingChanged += (_, _) => InstantItemPatch.Enable = isInstantItem.Value;

            UIStationStorageExtraPatch.Enable = StationStoExtra.Value > 0;
            StationStoExtra.SettingChanged += (_, _) => UIStationStorageExtraPatch.Enable = StationStoExtra.Value > 0;

            StarmapTeleportPatch.Enable = EnableTp.Value;
            EnableTp.SettingChanged += (_, _) => StarmapTeleportPatch.Enable = EnableTp.Value;

            CloseAbnormalityTipPatch.Enable = CloseUIAbnormalityTip.Value;
            CloseUIAbnormalityTip.SettingChanged += (_, _) => CloseAbnormalityTipPatch.Enable = CloseUIAbnormalityTip.Value;

            //bool 型开关（由 GUI 负责同步 Enable）
            TechClickUnlockPatch.Enable = unlockpointtech;

            #endregion
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
