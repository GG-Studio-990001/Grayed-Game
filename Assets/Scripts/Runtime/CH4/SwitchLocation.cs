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
        [SerializeField] protected GameObject player;
        [SerializeField] protected GameObject DefaultObjParent;
        [SerializeField] protected GameObject[] DefaultObjs;

        protected Dictionary<Ch4S2Locations, GameObject[]> locationMap;
        protected GameObject[] lastLocation;
        protected Ch4S2Locations lastVal;
        protected int lastIdx = -1;

        protected Ch4S2Locations playerLastLocation; // 플레이어가 마지막으로 있던 위치

        // 기본 4곳 순서
        protected Ch4S2Locations[] order = {
            Ch4S2Locations.Entrance,
            Ch4S2Locations.Square,
            Ch4S2Locations.Cave,
            Ch4S2Locations.Temple
        };

        protected virtual void Awake()
        {
            locationMap = new Dictionary<Ch4S2Locations, GameObject[]>
            {
                { Ch4S2Locations.Entrance, EntranceObjs },
                { Ch4S2Locations.Square, SquareObjs },
                { Ch4S2Locations.Cave, CaveObjs },
                { Ch4S2Locations.Temple, TempleObjs }
            };
        }

        public virtual void StartLevel()
        {
            Teleport(Ch4S2Locations.Entrance, -1);
        }

        public void Teleport(Ch4S2Locations loc, int idx)
        {
            if (idx != -1)
            {
                if (lastIdx != -1) DefaultObjs[lastIdx].SetActive(false);
                DefaultObjs[idx].SetActive(true);
                DefaultObjs[idx].GetComponentInChildren<TextMeshPro>().text = loc.GetName();
                lastIdx = idx;
            }

            // 플레이어 위치 기록
            playerLastLocation = loc;

            MoveTo(loc);
        }

        protected void MoveTo(Ch4S2Locations loc)
        {
            if (!locationMap.TryGetValue(loc, out var location)) return;

            // 이전 위치 끄기
            if (lastLocation != null)
                foreach (var obj in lastLocation) obj.SetActive(false);

            // 새 위치 켜기
            foreach (var obj in location) obj.SetActive(true);

            // 플레이어 켜기: 마지막으로 있던 위치일 때만
            bool playerHere = loc == playerLastLocation;
            if (player != null)
                player.SetActive(playerHere);

            // DefaultObjs도 플레이어 위치일 때만 켜기
            if (DefaultObjParent != null)
            {
                DefaultObjParent.SetActive(playerHere);
            }

            lastLocation = location;
            lastVal = loc;
        }

        public void MoveLeft()
        {
            int curIdx = System.Array.IndexOf(order, lastVal);
            int nextIdx = (curIdx - 1 + order.Length) % order.Length;
            MoveTo(order[nextIdx]);
        }

        public void MoveRight()
        {
            int curIdx = System.Array.IndexOf(order, lastVal);
            int nextIdx = (curIdx + 1) % order.Length;
            MoveTo(order[nextIdx]);
        }
    }
}
