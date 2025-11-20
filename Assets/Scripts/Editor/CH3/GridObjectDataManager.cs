using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

namespace Runtime.CH3.Main
{
    /// <summary>
    /// GridObject 데이터를 관리하고 에디터 툴에서 사용할 수 있도록 하는 매니저
    /// </summary>
    public static class GridObjectDataManager
    {
        private static Dictionary<string, CH3_LevelData> _dataCache;
        
        /// <summary>
        /// 모든 CH3_LevelData를 찾아서 캐시에 로드합니다.
        /// </summary>
        public static void LoadAllData()
        {
            _dataCache = new Dictionary<string, CH3_LevelData>();
            
            string[] guids = AssetDatabase.FindAssets("t:CH3_LevelData");
            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                CH3_LevelData data = AssetDatabase.LoadAssetAtPath<CH3_LevelData>(path);
                if (data != null && !string.IsNullOrEmpty(data.id))
                {
                    if (_dataCache.ContainsKey(data.id))
                    {
                        Debug.LogWarning($"중복된 id 발견: {data.id} (경로: {path})");
                    }
                    else
                    {
                        _dataCache[data.id] = data;
                    }
                }
            }
        }
        
        /// <summary>
        /// id로 레벨 데이터를 가져옵니다.
        /// </summary>
        public static CH3_LevelData GetDataById(string id)
        {
            if (_dataCache == null || _dataCache.Count == 0)
            {
                LoadAllData();
            }
            
            if (_dataCache != null && _dataCache.TryGetValue(id, out CH3_LevelData data))
            {
                return data;
            }
            
            return null;
        }
        
        /// <summary>
        /// 모든 레벨 데이터 ID 목록을 반환합니다.
        /// </summary>
        public static List<string> GetAllIds()
        {
            if (_dataCache == null || _dataCache.Count == 0)
            {
                LoadAllData();
            }
            
            return _dataCache != null ? _dataCache.Keys.ToList() : new List<string>();
        }
        
        /// <summary>
        /// 레벨 데이터로부터 GameObject를 생성합니다.
        /// </summary>
        public static GameObject CreateObjectFromData(CH3_LevelData data, Vector3 position)
        {
            if (data == null || !data.IsValid())
            {
                Debug.LogError("유효하지 않은 레벨 데이터입니다!");
                return null;
            }
            
            // Assets 폴더에서 "2.5D_Object" 프리팹 찾기
            GameObject basePrefab = Find2_5D_ObjectPrefab();
            if (basePrefab == null)
            {
                Debug.LogError("Assets 폴더에서 '2.5D_Object' 프리팹을 찾을 수 없습니다!");
                return null;
            }
            
            // 프리팹 인스턴스화
            GameObject rootObject = PrefabUtility.InstantiatePrefab(basePrefab) as GameObject;
            if (rootObject == null)
            {
                Debug.LogError("2.5D_Object 프리팹을 인스턴스화할 수 없습니다!");
                return null;
            }
            
            rootObject.transform.position = position;
            
            // GridObject 컴포넌트 추가 (타입과 isBreakable에 따라 적절한 컴포넌트 선택)
            GridObject gridObject = AddGridObjectComponent(rootObject, data.objectType, data);
            
            if (gridObject == null)
            {
                Debug.LogError($"GridObject 컴포넌트를 추가할 수 없습니다: {data.objectType}");
                Object.DestroyImmediate(rootObject);
                return null;
            }
            
            // 데이터 적용
            ApplyDataToObject(rootObject, data);
            
            // 자식 Sprite 오브젝트의 스프라이트 교체
            ReplaceSpriteInChild(rootObject, data);
            
            return rootObject;
        }
        
        /// <summary>
        /// Assets 폴더에서 "2.5D_Object" 프리팹을 찾습니다.
        /// </summary>
        private static GameObject Find2_5D_ObjectPrefab()
        {
            // Assets 폴더 전체에서 "2.5D_Object" 이름의 프리팹 찾기
            string[] guids = AssetDatabase.FindAssets("2.5D_Object t:Prefab");
            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                if (prefab != null && prefab.name == "2.5D_Object")
                {
                    return prefab;
                }
            }
            
            // 찾지 못한 경우 경고
            Debug.LogWarning("Assets 폴더에서 '2.5D_Object' 프리팹을 찾을 수 없습니다. Assets 폴더에 '2.5D_Object'라는 이름의 프리팹이 있는지 확인하세요.");
            return null;
        }
        
        /// <summary>
        /// 오브젝트 타입과 데이터에 따라 적절한 컴포넌트를 추가합니다.
        /// isBreakable이 true이면 Breakable, false이면 Structure를 추가합니다.
        /// </summary>
        private static GridObject AddGridObjectComponent(GameObject obj, GridObjectType objectType, CH3_LevelData data = null)
        {
            GridObject gridObject = null;
            
            switch (objectType)
            {
                case GridObjectType.Structure:
                    // isBreakable을 체크하여 Breakable 또는 Structure 선택
                    if (data != null && data.isBreakable)
                    {
                        gridObject = obj.AddComponent<Breakable>();
                    }
                    else
                    {
                        gridObject = obj.AddComponent<Structure>();
                    }
                    break;
                    
                case GridObjectType.Ore:
                    gridObject = obj.AddComponent<Ore>();
                    break;
                    
                case GridObjectType.Breakable:
                    gridObject = obj.AddComponent<Breakable>();
                    break;
                    
                case GridObjectType.EventArea:
                    gridObject = obj.AddComponent<EventArea>();
                    break;
                    
                case GridObjectType.Teleporter:
                    gridObject = obj.AddComponent<Teleporter>();
                    break;
                    
                case GridObjectType.NPC:
                    gridObject = obj.AddComponent<NPC>();
                    break;
                    
                case GridObjectType.BlockedArea:
                default:
                    gridObject = obj.AddComponent<GridObject>();
                    break;
            }
            
            return gridObject;
        }
        
        /// <summary>
        /// 레벨 데이터를 GridObject에 적용합니다.
        /// </summary>
        private static void ApplyDataToObject(GameObject obj, CH3_LevelData data)
        {
            // SerializedObject를 사용하여 설정 (리플렉션보다 안전함)
            SerializedObject so = new SerializedObject(obj);
            
            // GridObject 기본 설정
            var gridObject = obj.GetComponent<GridObject>();
            if (gridObject != null)
            {
                SerializedObject gridSo = new SerializedObject(gridObject);
                gridSo.FindProperty("objectType").enumValueIndex = (int)data.objectType;
                gridSo.FindProperty("tileSize").vector2IntValue = data.TileSize;
                gridSo.FindProperty("gridPositionMode").enumValueIndex = (int)data.gridPositionMode;
                
                // UseInspectorPosition일 때 gridPosition 설정
                if (data.gridPositionMode == GridObject.GridPositionInitializationMode.UseInspectorPosition)
                {
                    gridSo.FindProperty("gridPosition").vector2IntValue = data.gridPosition;
                }
                
                gridSo.FindProperty("useCustomY").boolValue = data.useCustomY;
                gridSo.FindProperty("customY").floatValue = data.customY;
                gridSo.FindProperty("applyInitialGridSorting").boolValue = data.applyInitialGridSorting;
                gridSo.FindProperty("gridSortingScale").intValue = data.gridSortingScale;
                gridSo.ApplyModifiedProperties();
            }
            
            // Structure 설정
            var structure = obj.GetComponent<Structure>();
            if (structure != null)
            {
                SerializedObject structureSo = new SerializedObject(structure);
                structureSo.FindProperty("isBlocking").boolValue = data.isBlocking;
                structureSo.ApplyModifiedProperties();
            }
            
            // MineableObject 설정
            var mineable = obj.GetComponent<MineableObject>();
            if (mineable != null)
            {
                SerializedObject mineableSo = new SerializedObject(mineable);
                mineableSo.FindProperty("maxMiningCount").intValue = data.maxMiningCount;
                mineableSo.FindProperty("miningStageSprites").arraySize = data.miningStageSprites != null ? data.miningStageSprites.Length : 0;
                for (int i = 0; i < data.miningStageSprites?.Length; i++)
                {
                    mineableSo.FindProperty("miningStageSprites").GetArrayElementAtIndex(i).objectReferenceValue = data.miningStageSprites[i];
                }
                mineableSo.FindProperty("itemPrefab").objectReferenceValue = data.itemPrefab;
                mineableSo.FindProperty("minDropCount").intValue = data.minDropCount;
                mineableSo.FindProperty("maxDropCount").intValue = data.maxDropCount;
                mineableSo.FindProperty("dropRadius").floatValue = data.dropRadius;
                mineableSo.FindProperty("enableColliderOnStart").boolValue = data.enableColliderOnStart;
                mineableSo.ApplyModifiedProperties();
            }
            
            // InteractableGridObject 설정
            var interactable = obj.GetComponent<InteractableGridObject>();
            if (interactable != null)
            {
                SerializedObject interactableSo = new SerializedObject(interactable);
                interactableSo.FindProperty("interactionRange").floatValue = data.interactionRange;
                interactableSo.ApplyModifiedProperties();
            }
        }
        
        /// <summary>
        /// 자식 Sprite 오브젝트의 스프라이트를 교체합니다.
        /// 2.5D_Object 프리팹에는 이미 Sprite 자식 오브젝트가 있다고 가정합니다.
        /// </summary>
        private static void ReplaceSpriteInChild(GameObject rootObject, CH3_LevelData data)
        {
            // 자식 오브젝트에서 "Sprite" 이름의 오브젝트 찾기
            Transform spriteTransform = rootObject.transform.Find("Sprite");
            if (spriteTransform == null)
            {
                // 대소문자 구분 없이 찾기
                foreach (Transform child in rootObject.transform)
                {
                    if (child.name.Equals("Sprite", System.StringComparison.OrdinalIgnoreCase))
                    {
                        spriteTransform = child;
                        break;
                    }
                }
            }
            
            if (spriteTransform == null)
            {
                Debug.LogWarning($"2.5D_Object의 자식에 'Sprite' 오브젝트를 찾을 수 없습니다. Sprite 오브젝트가 있는지 확인하세요.");
                return;
            }
            
            // SpriteRenderer 찾기
            SpriteRenderer spriteRenderer = spriteTransform.GetComponent<SpriteRenderer>();
            if (spriteRenderer == null)
            {
                Debug.LogWarning($"Sprite 오브젝트에 SpriteRenderer 컴포넌트가 없습니다.");
                return;
            }
            
            // 데이터에서 Sprite 교체
            if (data.sprite != null)
            {
                spriteRenderer.sprite = data.sprite;
            }
            
            // GridObject에 자식 오브젝트 참조 설정 (이미 존재하는 경우)
            var gridObject = rootObject.GetComponent<GridObject>();
            if (gridObject != null)
            {
                SerializedObject gridSo = new SerializedObject(gridObject);
                
                // GridVolume 찾기
                Transform gridVolumeTransform = rootObject.transform.Find("GridVolume");
                if (gridVolumeTransform == null)
                {
                    foreach (Transform child in rootObject.transform)
                    {
                        if (child.name.Equals("GridVolume", System.StringComparison.OrdinalIgnoreCase))
                        {
                            gridVolumeTransform = child;
                            break;
                        }
                    }
                }
                
                if (gridVolumeTransform != null)
                {
                    gridSo.FindProperty("gridVolumeTransform").objectReferenceValue = gridVolumeTransform;
                }
                
                gridSo.FindProperty("spriteTransform").objectReferenceValue = spriteTransform;
                gridSo.FindProperty("autoBindChildren").boolValue = false; // 수동으로 설정했으므로 자동 바인딩 비활성화
                gridSo.ApplyModifiedProperties();
            }
            
            // Collider 추가 (MineableObject인 경우, 없으면 추가)
            var mineable = rootObject.GetComponent<MineableObject>();
            if (mineable != null)
            {
                BoxCollider collider = spriteTransform.GetComponent<BoxCollider>();
                if (collider == null)
                {
                    collider = spriteTransform.gameObject.AddComponent<BoxCollider>();
                }
                collider.isTrigger = false;
                
                // Sprite 크기에 맞게 Collider 크기 조정
                if (spriteRenderer.sprite != null)
                {
                    Bounds spriteBounds = spriteRenderer.sprite.bounds;
                    collider.size = spriteBounds.size;
                    collider.center = spriteBounds.center;
                }
            }
            
            // Structure인 경우 Collider 추가 (선택사항, 없으면 추가)
            var structure = rootObject.GetComponent<Structure>();
            if (structure != null && data.isBlocking)
            {
                BoxCollider collider = spriteTransform.GetComponent<BoxCollider>();
                if (collider == null)
                {
                    collider = spriteTransform.gameObject.AddComponent<BoxCollider>();
                }
                collider.isTrigger = false;
                
                if (spriteRenderer.sprite != null)
                {
                    Bounds spriteBounds = spriteRenderer.sprite.bounds;
                    collider.size = spriteBounds.size;
                    collider.center = spriteBounds.center;
                }
            }
            
            // Breakable인 경우 UI Prefab을 자식 오브젝트로 생성
            var breakable = rootObject.GetComponent<Breakable>();
            if (breakable != null && data.uiPrefab != null)
            {
                // 이미 UI Prefab이 자식으로 있는지 확인
                bool uiExists = false;
                foreach (Transform child in rootObject.transform)
                {
                    if (PrefabUtility.GetCorrespondingObjectFromSource(child.gameObject) == data.uiPrefab)
                    {
                        uiExists = true;
                        break;
                    }
                }
                
                // 없으면 생성
                if (!uiExists)
                {
                    GameObject uiObject = PrefabUtility.InstantiatePrefab(data.uiPrefab) as GameObject;
                    if (uiObject != null)
                    {
                        uiObject.transform.SetParent(rootObject.transform);
                        uiObject.transform.localPosition = Vector3.zero;
                    }
                }
            }
        }
        
    }
}

