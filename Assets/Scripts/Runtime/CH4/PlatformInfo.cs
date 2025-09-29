using TMPro;
using UnityEngine;

namespace Runtime.CH4
{
    public enum Ch4S2Locations
    {
        Entrance = 1,
        Square,
        Cave,
        Temple,
        InTemple,
        Backstreet,
        Base,
        Storage,
        InStorage,
    }

    public class PlatformInfo : MonoBehaviour
    {
        public Ch4S2Locations TargetLocation;
        public int Idx;
        public TextMeshPro Txt;
    }
}