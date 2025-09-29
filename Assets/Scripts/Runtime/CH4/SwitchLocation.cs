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
                DefaultObjs[idx].GetComponentInChildren<TextMeshPro>().text = loc.GetName();
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
