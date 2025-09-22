using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Runtime.CH4
{
    public class SwitchLocation : MonoBehaviour
    {
        [SerializeField] private GameObject[] EntranceObjs;
        [SerializeField] private GameObject[] SquareObjs;
        [SerializeField] private GameObject[] CaveObjs;
        [SerializeField] private GameObject[] TempleObjs;
        [SerializeField] private GameObject[] DefaultObjs;

        private Dictionary<Ch4Ch2Locations, GameObject[]> locationMap;
        private Dictionary<Ch4Ch2Locations, string> locationName;
        private GameObject[] lastLocation;
        private Ch4Ch2Locations lastVal;
        private int lastIdx = 0;

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

            locationName = new Dictionary<Ch4Ch2Locations, string>
            {
                { Ch4Ch2Locations.Entrance, "1_마을입구" },
                { Ch4Ch2Locations.Square,  "2_광장" },
                { Ch4Ch2Locations.Cave,  "3_동굴" },
                { Ch4Ch2Locations.Temple,  "4_신전" }
            };

            Debug.Log("[SwitchLocation] Awake - locationMap 초기화 완료");
        }

        private void Start()
        {
            Debug.Log("[SwitchLocation] Start - 초기 위치 Entrance로 설정");
            Teleport(Ch4Ch2Locations.Entrance, -1);
        }

        public void Teleport(Ch4Ch2Locations loc, int idx)
        {
            // 장소 이동
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

            // 발판 켜기
            if (idx == -1)
                return;
            else
            {
                DefaultObjs[lastIdx].gameObject.SetActive(false);
                DefaultObjs[idx].gameObject.SetActive(true);
                DefaultObjs[idx].GetComponentInChildren<TextMeshPro>().text = locationName[loc];
                lastIdx = idx;
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
