using System;
using System.Collections.Generic;
using UnityEngine;

namespace Multifunction_mod.API
{
    public class ExternalMenu
    {
        public string Name { get; set; }
        public Action OnDraw { get; set; }
    }

    public static class MultifunctionAPI
    {
        private static List<ExternalMenu> _externalMenus = new List<ExternalMenu>();

        public static List<ExternalMenu> ExternalMenus => _externalMenus;

        /// <summary>
        /// 清除所有已注册的菜单
        /// </summary>
        public static void ClearMenus()
        {
            _externalMenus.Clear();
        }

        /// <summary>
        /// 注册一个新的菜单页
        /// </summary>
        /// <param name="name">菜单名称</param>
        /// <param name="onDraw">绘制UI的回调函数 (GUILayout)</param>
        public static void RegisterMenu(string name, Action onDraw)
        {
            if (string.IsNullOrEmpty(name) || onDraw == null)
            {
                Debug.LogWarning("[MultifunctionAPI] RegisterMenu failed: name or onDraw is null.");
                return;
            }

            // 避免重复注册同名菜单
            if (_externalMenus.Exists(m => m.Name == name))
            {
                Debug.LogWarning($"[MultifunctionAPI] Menu '{name}' is already registered.");
                return;
            }

            _externalMenus.Add(new ExternalMenu { Name = name, OnDraw = onDraw });
            Debug.Log($"[MultifunctionAPI] Registered menu: {name}");
        }

        /// <summary>
        /// 移除菜单页
        /// </summary>
        /// <param name="name"></param>
        public static void UnregisterMenu(string name)
        {
            _externalMenus.RemoveAll(m => m.Name == name);
        }
    }
}
