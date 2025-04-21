using Runtime.CH3.Main;
using UnityEngine;
using System.Collections.Generic;

namespace Runtime.CH3.Main
{
    // 오브젝트 타입 열거형
    public enum GridObjectType
    {
        Structure,
        BlockedArea,
        NPC,
        Ore 
    }

    // 오브젝트 생성을 담당하는 팩토리
    public class GridObjectFactory : MonoBehaviour
    {
        [System.Serializable]
        private class PrefabData
        {
            public GridObjectType type;
            public GameObject prefab;
        }

        [SerializeField] private List<PrefabData> prefabList;
        private Dictionary<GridObjectType, GameObject> prefabDictionary;

        private static GridObjectFactory instance;
        public static GridObjectFactory Instance => instance;

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
                InitializePrefabDictionary();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void InitializePrefabDictionary()
        {
            prefabDictionary = new Dictionary<GridObjectType, GameObject>();
            foreach (var data in prefabList)
            {
                prefabDictionary[data.type] = data.prefab;
            }
        }

        public IGridObject CreateObject(GridObjectType type, Vector2Int gridPosition)
        {
            if (!prefabDictionary.TryGetValue(type, out GameObject prefab))
            {
                Debug.LogError($"Prefab not found for type: {type}");
                return null;
            }

            GameObject instance = Instantiate(prefab);
            IGridObject gridObject = instance.GetComponent<IGridObject>();
            
            if (gridObject == null)
            {
                Debug.LogError($"Created object does not implement IGridObject: {type}");
                Destroy(instance);
                return null;
            }

            gridObject.Initialize(gridPosition);
            return gridObject;
        }
    }
}