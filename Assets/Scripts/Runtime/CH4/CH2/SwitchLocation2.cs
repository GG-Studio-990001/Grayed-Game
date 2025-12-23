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

            order = new Ch4S2Locations[]
            {
                Ch4S2Locations.Entrance,
                Ch4S2Locations.Square,
                Ch4S2Locations.Cave,
                Ch4S2Locations.Temple,
                Ch4S2Locations.InTemple,
                Ch4S2Locations.Backstreet,
                Ch4S2Locations.Base,
                Ch4S2Locations.Storage,
                Ch4S2Locations.InStorage
            };

            locationMap[Ch4S2Locations.InTemple] = InTempleObjs;
            locationMap[Ch4S2Locations.Backstreet] = BackstreetObjs;
            locationMap[Ch4S2Locations.Base] = BaseObjs;
            locationMap[Ch4S2Locations.Storage] = StorageObjs;
            locationMap[Ch4S2Locations.InStorage] = InStorageObjs;
        }

        public override void StartLevel()
        {
            Teleport(Ch4S2Locations.InStorage, -1); // 초기 위치
        }
    }
}
