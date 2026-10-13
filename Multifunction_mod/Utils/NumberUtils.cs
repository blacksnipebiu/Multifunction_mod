using System;

namespace Multifunction_mod.Utils
{
    public static class NumberUtils
    {
        public static float ConvertToNumber(string input)
        {
            // 提取数字部分
            string numberString = "";
            string tempinput = "";
            bool dotContain = false;
            for (int i = 0; i < input.Length; i++)
            {
                var c = input[i];
                if (Char.IsDigit(c))
                {
                    numberString += c;
                }
                else if (c == '.')
                {
                    if (!dotContain)
                    {
                        dotContain = true;
                        numberString += c;
                    }
                }
                else
                {
                    tempinput += c;
                }
            }
            input = tempinput;
            // 提取单位部分
            string unitString = input.Replace(numberString, "").Replace(".", "").Replace("W", "").Replace("J", "");

            // 将数字字符串转换为双精度浮点数
            if (float.TryParse(numberString, out float number))
            {
                // 根据单位进行相应的转换
                switch (unitString)
                {
                    case "":
                        break;
                    case "K":
                        number *= 1_000;
                        break;
                    case "M":
                        number *= 1_000_000;
                        break;
                    case "G":
                        number *= 1_000_000_000;
                        break;
                    case "T":
                        number *= 1_000_000_000_000;
                        break;
                        // 其他单位的转换逻辑可以在这里添加
                }
                return number;
            }
            return 0f;
        }
        public static int IntParseLimit(string valueStr, int lowlimit, int highlimit)
        {
            int.TryParse(valueStr, out int value);
            return Math.Max(lowlimit, Math.Min(value, highlimit));
        }

        public static string TGMKinttostring(double num, string unit)
        {
            double tempcoreEnergyCap = num;
            int t = 0;
            if (num < 1000)
                return num + unit;
            while (tempcoreEnergyCap >= 999.9)
            {
                t += 1;
                tempcoreEnergyCap /= 1000;
            }
            string coreEnergyCap = string.Format("{0:N2}", tempcoreEnergyCap);
            if (t == 0) return coreEnergyCap + "" + unit;
            if (t == 1) return coreEnergyCap + "K" + unit;
            if (t == 2) return coreEnergyCap + "M" + unit;
            if (t == 3) return coreEnergyCap + "G" + unit;
            if (t == 4) return coreEnergyCap + "T" + unit;

            return "";
        }
    }
}
