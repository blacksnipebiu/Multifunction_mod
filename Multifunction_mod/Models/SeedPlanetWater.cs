using System.Collections.Generic;
using System.Linq;

namespace Multifunction_mod.Models
{
    public class SeedPlanetWater
    {
        public long seedKey64 { get; set; }
        public Dictionary<int, int> waterTypes { get; set; }

        public SeedPlanetWater()
        {
            waterTypes = new Dictionary<int, int>();
        }

        public SeedPlanetWater(long seed)
        {
            seedKey64 = seed;
            waterTypes = new Dictionary<int, int>();
        }

        public string ToStr()
        {
            string result = seedKey64 + ",";
            result += string.Join("", waterTypes.Select(x => x.Key + ":" + x.Value + "-"));
            return result;
        }
    }
}
