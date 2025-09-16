using UnityEngine;

namespace Runtime.CH4
{
    public enum Ch4Ch2Locations
    {
        Entrance = 1,
        Square,
        Cave,
        Temple,
    }

    public class PlatformInfo : MonoBehaviour
    {
        public Ch4Ch2Locations TargetLocation;
        public int Idx;
    }
}