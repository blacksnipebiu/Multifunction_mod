using HarmonyLib;
using static Multifunction_mod.Multifunction;

namespace Multifunction_mod.Patchs.MechaPatches
{
    /// <summary>
    /// 拆除不添加至背包：拆除建筑时不把物品放入背包（直接销毁），物流条/建筑返还走原逻辑。
    /// 注意：类不能声明为 static —— PatchToggle&lt;T&gt; 的类型实参不能是静态类型（CS0718）。
    /// </summary>
    public class DismantleNoDropToPackagePatch
    {
        public static bool Enable
        {
            get { return PatchToggle<DismantleNoDropToPackagePatch>.Enable; }
            set { PatchToggle<DismantleNoDropToPackagePatch>.Enable = value; }
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(Player), "TryAddItemToPackage")]
        public static bool PlayerTryAddItemToPackage(int itemId)
        {
            if (itemId == 1099)
            {
                return true;
            }
            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(Player), "SendItemToPlayer")]
        public static bool PlayerSendItemToPlayer(ref int itemId, ref int itemCount, ref int itemInc, bool toPackage, ItemBundle sentList)
        {
            if (itemId == 1099)
            {
                return true;
            }

            if (itemId > 0 && itemCount > 0 && toPackage)
            {
                sentList?.Alter(itemId, itemCount);
                itemId = 0;
                itemCount = 0;
                itemInc = 0;
                return false;
            }
            return true;
        }
    }
}
