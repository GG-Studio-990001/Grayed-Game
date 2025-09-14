using System.Collections.Generic;
using UnityEngine;

namespace Runtime.CH4
{
    public class SwitchLocation : MonoBehaviour
    {
        [SerializeField] private GameObject[] EntranceObjs;
        [SerializeField] private GameObject[] SquareObjs;
        [SerializeField] private GameObject[] CaveObjs;
        [SerializeField] private GameObject[] TempleObjs;

        private Dictionary<Ch4Ch2Locations, GameObject[]> locationMap;
        private GameObject[] lastLocation;
        private Ch4Ch2Locations lastVal;

        private void Awake()
        {
            // 장소와 오브젝트 배열 매핑
            locationMap = new Dictionary<Ch4Ch2Locations, GameObject[]>
            {
                { Ch4Ch2Locations.Entrance, EntranceObjs },
                { Ch4Ch2Locations.Square, SquareObjs },
                { Ch4Ch2Locations.Cave, CaveObjs },
                { Ch4Ch2Locations.Temple, TempleObjs }
            };

            Debug.Log("[SwitchLocation] Awake - locationMap 초기화 완료");
        }

        private void Start()
        {
            Debug.Log("[SwitchLocation] Start - 초기 위치 Entrance로 설정");
            Teleport(Ch4Ch2Locations.Entrance);
        }

        public void Teleport(Ch4Ch2Locations loc)
        {
            Debug.Log($"[SwitchLocation] Teleport 요청: {loc}");

            if (lastVal == loc)
            {
                Debug.Log($"[SwitchLocation] 이미 {loc}에 있음 → 무시");
                return;
            }

            if (locationMap.TryGetValue(loc, out var objs))
            {
                MoveTo(objs);
                Debug.Log($"[SwitchLocation] Teleport 성공: {lastVal} → {loc}");
                lastVal = loc;
            }
            else
            {
                Debug.LogWarning($"[SwitchLocation] {loc} 에 해당하는 오브젝트 배열이 없습니다.");
            }
        }

        private void MoveTo(GameObject[] location)
        {
            if (lastLocation != null)
            {
                foreach (GameObject obj in lastLocation)
                {
                    obj.SetActive(false);
                }
            }

            foreach (GameObject obj in location)
            {
                obj.SetActive(true);
            }

            lastLocation = location;
        }
    }
}
