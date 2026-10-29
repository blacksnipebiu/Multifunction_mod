namespace Multifunction_mod.Meta
{
    /// <summary>
    /// 模组元数据：插件标识、名称、版本。
    /// 修改版本号时，请同步 <c>Meta/manifest.json</c> 里的 <c>version_number</c>。
    /// </summary>
    public static class ModInfo
    {
        /// <summary>BepInEx 插件唯一标识（也用于 Harmony 实例 id 前缀）</summary>
        public const string GUID = "cn.blacksnipe.dsp.Multfuntion_mod";

        /// <summary>插件显示名称</summary>
        public const string NAME = "Multfuntion_mod";

        /// <summary>版本号，需与 manifest.json 的 version_number 一致</summary>
        public const string VERSION = "4.0.8";
    }
}
