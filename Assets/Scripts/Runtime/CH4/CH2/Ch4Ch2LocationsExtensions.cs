using System.Collections.Generic;

namespace Runtime.CH4
{
    public static class Ch4S2LocationsExtensions
    {
        private static readonly Dictionary<Ch4S2Locations, string> _names = new()
        {
            { Ch4S2Locations.Entrance, "마을입구" },
            { Ch4S2Locations.Square, "광장" },
            { Ch4S2Locations.Cave, "동굴" },
            { Ch4S2Locations.Temple, "신전" },
            { Ch4S2Locations.InTemple, "신전 방" },
            { Ch4S2Locations.Backstreet, "골목길" },
            { Ch4S2Locations.Base, "기지" },
            { Ch4S2Locations.Storage, "창고" },
            { Ch4S2Locations.InStorage, "창고내부" },
        };

        public static string GetName(this Ch4S2Locations loc)
        {
            return _names.TryGetValue(loc, out var name) ? name : loc.ToString();
        }
    }
}
