using UnityEngine;

namespace Runtime.CH4
{
    public class SwitchLocation2 : SwitchLocation
    {
        [SerializeField] private GameObject[] InTempleObjs;
        [SerializeField] private GameObject[] BackstreetObjs;
        [SerializeField] private GameObject[] BaseObjs;
        [SerializeField] private GameObject[] StorageObjs;
        [SerializeField] private GameObject[] InStorageObjs;

        protected override void Awake()
        {
            base.Awake();

            // 상속받은 locationMap에 새 장소 추가
            locationMap[Ch4Ch2Locations.InTemple] = InTempleObjs;
            locationMap[Ch4Ch2Locations.Backstreet] = BackstreetObjs;
            locationMap[Ch4Ch2Locations.Base] = BaseObjs;
            locationMap[Ch4Ch2Locations.Storage] = StorageObjs;
            locationMap[Ch4Ch2Locations.InStorage] = InStorageObjs;

            locationName[Ch4Ch2Locations.InTemple] = "5_신전 방";
            locationName[Ch4Ch2Locations.Backstreet] = "6_골목길";
            locationName[Ch4Ch2Locations.Base] = "7_기지";
            locationName[Ch4Ch2Locations.Storage] = "8_창고";
            locationName[Ch4Ch2Locations.InStorage] = "9_창고내부";
        }

        public override void StartLevel()
        {
            Teleport(Ch4Ch2Locations.InStorage, -1); // 초기 위치 변경
        }
    }
}