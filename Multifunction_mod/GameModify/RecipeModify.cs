using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace Multifunction_mod.GameModify
{
    /// <summary>
    /// 配方相关的游戏修改：快速生产、全部手搓。
    /// </summary>
    public static class RecipeModify
    {
        /// <summary>快速生产时所有配方的周期（刻）。游戏 60 刻 = 1 秒，10 刻约 0.17 秒</summary>
        public const int QuickProduceTimeSpend = 10;

        private static RecipeProto[] originRecipes;
        private static int[] originExtraTimeSpends;

        private static FieldInfo recipeTimeSpendField;
        private static FieldInfo recipeExtraTimeSpendField;
        private static bool recipeExecuteFieldsResolved;

        /// <summary>
        /// 设置全局物品堆叠倍率：itemStackCount[itemId] = 原始堆叠 × multiple。
        /// 注意该数组是 StorageComponent 的静态表，玩家背包、配送背包、箱子、物流站等所有存储实例共用。
        /// 多倍率会立即影响之后新放入格子的堆叠上限；已在格子里的 stackSize 需要重新进档（Import）才刷新。
        /// </summary>
        public static void SetItemStackMultiple(int multiple)
        {
            if (LDB.items == null || LDB.items.dataArray == null || StorageComponent.itemStackCount == null)
            {
                return;
            }

            if (multiple < 1)
            {
                multiple = 1;
            }

            ItemProto[] items = LDB.items.dataArray;
            for (int i = 0; i < items.Length; i++)
            {
                ItemProto item = items[i];
                StorageComponent.itemStackCount[item.ID] = item.StackSize * multiple;
            }
        }

        /// <summary>
        /// 重建熔炉配方索引：原料物品 ID → 使用它的熔炼配方列表。
        /// 一次遍历、每次查表只做一次哈希（TryGetValue），不会像 ContainsKey+索引器那样查两次。
        /// </summary>
        public static void RebuildSmeltRecipeIndex(Dictionary<int, List<RecipeProto>> smeltRecipes)
        {
            if (smeltRecipes == null || LDB.recipes == null || LDB.recipes.dataArray == null)
            {
                return;
            }

            smeltRecipes.Clear();
            RecipeProto[] recipes = LDB.recipes.dataArray;
            for (int r = 0; r < recipes.Length; r++)
            {
                RecipeProto recipe = recipes[r];
                if (recipe.Type != ERecipeType.Smelt)
                {
                    continue;
                }

                int[] items = recipe.Items;
                for (int i = 0; i < items.Length; i++)
                {
                    int itemId = items[i];
                    if (!smeltRecipes.TryGetValue(itemId, out List<RecipeProto> list))
                    {
                        list = new List<RecipeProto>(1);
                        smeltRecipes[itemId] = list;
                    }
                    list.Add(recipe);
                }
            }
        }

        /// <summary>
        /// 记录配方原始数据。应在每次进入存档、第一次应用配方修改之前调用。
        /// 原始耗时同时来自两处：<see cref="RecipeProto.TimeSpend"/>（手搓/界面读取）
        /// 和 <see cref="RecipeProto.recipeExecuteData"/> 缓存（工厂设备真正读取）。
        /// </summary>
        public static void CaptureRecipeOriginals()
        {
            if (LDB.recipes == null || LDB.recipes.dataArray == null)
            {
                return;
            }

            RecipeProto[] recipes = LDB.recipes.dataArray;
            originRecipes = new RecipeProto[recipes.Length];
            originExtraTimeSpends = new int[recipes.Length];
            for (int i = 0; i < recipes.Length; i++)
            {
                RecipeProto recipe = recipes[i];
                originRecipes[i] = new RecipeProto
                {
                    TimeSpend = recipe.TimeSpend,
                    Handcraft = recipe.Handcraft
                };

                RecipeExecuteData executeData = GetRecipeExecuteData(recipe);
                originExtraTimeSpends[i] = executeData != null ? executeData.extraTimeSpend : 0;
            }
        }

        /// <summary>
        /// 应用配方修改（快速生产、全部手搓）。可以重复调用，重复调用等价于按当前开关重设一次。
        /// </summary>
        /// <param name="allHandcraft">所有配方可手搓</param>
        /// <param name="quickProduce">快速生产</param>
        public static void ApplyRecipeModifications(bool allHandcraft, bool quickProduce)
        {
            if (LDB.recipes == null || LDB.recipes.dataArray == null || originRecipes == null)
            {
                return;
            }

            RecipeProto[] recipes = LDB.recipes.dataArray;
            for (int i = 0; i < recipes.Length && i < originRecipes.Length; i++)
            {
                RecipeProto recipe = recipes[i];
                recipe.Handcraft = allHandcraft || originRecipes[i].Handcraft;

                //手搓队列（ForgeTask）和界面读的是 RecipeProto.TimeSpend，这里继续同步
                int timeSpend = quickProduce ? QuickProduceTimeSpend : originRecipes[i].TimeSpend;
                recipe.TimeSpend = timeSpend;

                //工厂设备每个 tick 读的是 RecipeProto.recipeExecuteData 里缓存的对象，
                //只改 RecipeProto.TimeSpend 对产线没有作用，必须同时改缓存里的周期
                RecipeExecuteData executeData = GetRecipeExecuteData(recipe);
                if (executeData == null)
                {
                    continue;
                }

                int originExtraTimeSpend = originExtraTimeSpends != null && i < originExtraTimeSpends.Length
                    ? originExtraTimeSpends[i]
                    : executeData.extraTimeSpend;

                //没有额外产出耗时的配方（原值为 0）不动这个字段，避免误伤
                if (originExtraTimeSpend > 0)
                {
                    int extraTimeSpend = quickProduce
                        ? ScaleCycleTime(originExtraTimeSpend, originRecipes[i].TimeSpend, timeSpend)
                        : originExtraTimeSpend;
                    SetRecipeExecuteCycleTime(executeData, timeSpend, extraTimeSpend);
                }
                else
                {
                    SetRecipeExecuteCycleTime(executeData, timeSpend);
                }
            }
        }

        /// <summary>
        /// 解析 RecipeExecuteData 的周期字段。游戏里这两个字段是 readonly，只能反射写入
        /// </summary>
        private static void ResolveRecipeExecuteFields()
        {
            if (recipeExecuteFieldsResolved)
            {
                return;
            }

            recipeExecuteFieldsResolved = true;
            try
            {
                Type type = typeof(RecipeExecuteData);
                recipeTimeSpendField = type.GetField("timeSpend", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                recipeExtraTimeSpendField = type.GetField("extraTimeSpend", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            }
            catch (Exception e)
            {
                Debug.Log("快速生产：解析配方周期字段失败 " + e.Message);
            }
        }

        /// <summary>
        /// 取设备运行时真正读取的配方执行数据（配方表反序列化时构建的静态缓存）
        /// </summary>
        private static RecipeExecuteData GetRecipeExecuteData(RecipeProto recipe)
        {
            if (recipe == null)
            {
                return null;
            }

            var cache = RecipeProto.recipeExecuteData;
            if (cache == null)
            {
                return null;
            }

            RecipeExecuteData data;
            return cache.TryGetValue(recipe.ID, out data) ? data : null;
        }

        /// <summary>
        /// 修改缓存对象里的周期耗时。已有设备持有同一个对象引用，改完立即生效
        /// </summary>
        private static void SetRecipeExecuteCycleTime(RecipeExecuteData data, int timeSpend, int? extraTimeSpend = null)
        {
            if (data == null)
            {
                return;
            }

            ResolveRecipeExecuteFields();
            try
            {
                if (recipeTimeSpendField != null)
                {
                    recipeTimeSpendField.SetValue(data, timeSpend);
                }

                if (recipeExtraTimeSpendField != null && extraTimeSpend.HasValue)
                {
                    recipeExtraTimeSpendField.SetValue(data, extraTimeSpend.Value);
                }
            }
            catch (Exception e)
            {
                Debug.Log("快速生产：写入配方周期失败 " + e.Message);
            }
        }

        /// <summary>
        /// 按原比例缩放周期，用于额外产出耗时，保证额外产出与主产出的比例不变
        /// </summary>
        private static int ScaleCycleTime(int value, int originTimeSpend, int newTimeSpend)
        {
            if (value <= 0 || originTimeSpend <= 0)
            {
                return value;
            }

            long scaled = (long)value * newTimeSpend / originTimeSpend;
            if (scaled < 1)
            {
                scaled = 1;
            }

            return (int)scaled;
        }
    }
}
