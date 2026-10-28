using System;
using HarmonyLib;
using Multifunction_mod.Meta;

namespace Multifunction_mod.Patchs
{
    /// <summary>
    /// 通用补丁开关：Enable=true 时挂上 T 里的所有 Harmony 补丁，false 时卸载。
    /// 每个 T 使用独立的 Harmony 实例，互不影响，便于按功能精细控制补丁的加载/卸载。
    /// </summary>
    public static class PatchToggle<T> where T : class
    {
        private static readonly object gate = new object();
        private static Harmony harmony;
        private static bool enable;

        public static bool Enable
        {
            get { return enable; }
            set
            {
                lock (gate)
                {
                    if (enable == value) return;
                    enable = value;
                    if (value)
                    {
                        harmony = new Harmony(ModInfo.GUID + "." + typeof(T).Name);
                        harmony.PatchAll(typeof(T));
                    }
                    else if (harmony != null)
                    {
                        harmony.UnpatchSelf();
                        harmony = null;
                    }
                }
            }
        }
    }
}
