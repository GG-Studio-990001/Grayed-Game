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
            locationMap[Ch4S2Locations.InTemple] = InTempleObjs;
            locationMap[Ch4S2Locations.Backstreet] = BackstreetObjs;
            locationMap[Ch4S2Locations.Base] = BaseObjs;
            locationMap[Ch4S2Locations.Storage] = StorageObjs;
            locationMap[Ch4S2Locations.InStorage] = InStorageObjs;
        }

        public override void StartLevel()
        {
            Teleport(Ch4S2Locations.InStorage, -1); // 초기 위치 변경
        }
    }
}