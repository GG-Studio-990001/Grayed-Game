using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Runtime.CH4
{
    public class SwitchLocation : MonoBehaviour
    {
        [SerializeField] protected GameObject[] EntranceObjs;
        [SerializeField] protected GameObject[] SquareObjs;
        [SerializeField] protected GameObject[] CaveObjs;
        [SerializeField] protected GameObject[] TempleObjs;
        [SerializeField] protected GameObject[] DefaultObjs;

        protected Dictionary<Ch4Ch2Locations, GameObject[]> locationMap;
        protected Dictionary<Ch4Ch2Locations, string> locationName;
        protected GameObject[] lastLocation;
        protected Ch4Ch2Locations lastVal;
        protected int lastIdx = -1;

        protected virtual void Awake()
        {
            locationMap = new Dictionary<Ch4Ch2Locations, GameObject[]>
        {
            { Ch4Ch2Locations.Entrance, EntranceObjs },
            { Ch4Ch2Locations.Square, SquareObjs },
            { Ch4Ch2Locations.Cave, CaveObjs },
            { Ch4Ch2Locations.Temple, TempleObjs }
        };

            // TODO: 리팩터링, 장소 텍스트가 여러 곳에서 사용됨
            locationName = new Dictionary<Ch4Ch2Locations, string>
        {
            { Ch4Ch2Locations.Entrance, "1_마을입구" },
            { Ch4Ch2Locations.Square,  "2_광장" },
            { Ch4Ch2Locations.Cave,  "3_동굴" },
            { Ch4Ch2Locations.Temple,  "4_신전" }
        };
        }

        public virtual void StartLevel()
        {
            Teleport(Ch4Ch2Locations.Entrance, -1);
        }

        public void Teleport(Ch4Ch2Locations loc, int idx)
        {
            if (lastVal == loc) return;
            if (locationMap.TryGetValue(loc, out var objs)) MoveTo(objs);

            if (idx != -1)
            {
                if (lastIdx != -1) DefaultObjs[lastIdx].SetActive(false);
                DefaultObjs[idx].SetActive(true);
                DefaultObjs[idx].GetComponentInChildren<TextMeshPro>().text = locationName[loc];
                lastIdx = idx;
            }

            lastVal = loc;
        }

        protected void MoveTo(GameObject[] location)
        {
            if (lastLocation != null)
                foreach (var obj in lastLocation) obj.SetActive(false);
            foreach (var obj in location) obj.SetActive(true);
            lastLocation = location;
        }
    }
}
