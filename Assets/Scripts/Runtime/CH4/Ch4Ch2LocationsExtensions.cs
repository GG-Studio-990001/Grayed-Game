using System.Collections.Generic;

namespace Runtime.CH4
{
    public static class Ch4Ch2LocationsExtensions
    {
        private static readonly Dictionary<Ch4Ch2Locations, string> _names = new()
        {
            { Ch4Ch2Locations.Entrance, "1_마을입구" },
            { Ch4Ch2Locations.Square, "2_광장" },
            { Ch4Ch2Locations.Cave, "3_동굴" },
            { Ch4Ch2Locations.Temple, "4_신전" },
            { Ch4Ch2Locations.InTemple, "5_신전 방" },
            { Ch4Ch2Locations.Backstreet, "6_골목길" },
            { Ch4Ch2Locations.Base, "7_기지" },
            { Ch4Ch2Locations.Storage, "8_창고" },
            { Ch4Ch2Locations.InStorage, "9_창고내부" },
        };

        public static string GetName(this Ch4Ch2Locations loc)
        {
            return _names.TryGetValue(loc, out var name) ? name : loc.ToString();
        }
    }
}
