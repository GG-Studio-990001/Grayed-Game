using UnityEngine;
using System.Collections.Generic;

namespace Runtime.CH3.Main
{
    /// <summary>
    /// 런타임에서 CH3_LevelData로부터 GameObject를 생성하는 팩토리
    /// </summary>
    public static class BuildingObjectFactory
    {
        private static GameObject _basePrefab;
        
        /// <summary>
        /// 기본 프리팹 설정 (BuildingSystem에서 호출)
        /// </summary>
        public static void SetBasePrefab(GameObject prefab)
        {
            _basePrefab = prefab;
        }
        
        /// <summary>
        /// 레벨 데이터로부터 GameObject를 생성합니다.
        /// </summary>
        public static GameObject CreateObjectFromData(CH3_LevelData data, Vector2Int gridPosition, GridSystem gridSystem)
        {
            if (data == null || !data.IsValid())
            {
                Debug.LogError("유효하지 않은 레벨 데이터입니다!");
                return null;
            }
            
            if (_basePrefab == null)
            {
                Debug.LogError("기본 프리팹이 설정되지 않았습니다!");
                return null;
            }
            
            if (gridSystem == null)
            {
                Debug.LogError("GridSystem이 없습니다!");
                return null;
            }
            
            // 월드 위치 계산
            Vector3 worldPosition = gridSystem.GridToWorldPosition(gridPosition);
            
            // 프리팹 인스턴스화
            GameObject rootObject = Object.Instantiate(_basePrefab);
            rootObject.transform.position = worldPosition;
            
            // GridObject 컴포넌트 추가
            GridObject gridObject = AddGridObjectComponent(rootObject, data.objectType, data);
            
            if (gridObject == null)
            {
                Debug.LogError($"GridObject 컴포넌트를 추가할 수 없습니다: {data.objectType}");
                Object.Destroy(rootObject);
                return null;
            }
            
            // 데이터 적용
            ApplyDataToObject(rootObject, data, gridPosition);
            
            // 자식 Sprite 오브젝트의 스프라이트 교체
            ReplaceSpriteInChild(rootObject, data);
            
            // GridObject 초기화
            gridObject.Initialize(gridPosition);
            
            return rootObject;
        }
        
        /// <summary>
        /// 오브젝트 타입과 데이터에 따라 적절한 컴포넌트를 추가합니다.
        /// </summary>
        private static GridObject AddGridObjectComponent(GameObject obj, GridObjectType objectType, CH3_LevelData data = null)
        {
            GridObject gridObject = null;
            
            switch (objectType)
            {
                case GridObjectType.Structure:
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
        private static void ApplyDataToObject(GameObject obj, CH3_LevelData data, Vector2Int gridPosition)
        {
            var gridObject = obj.GetComponent<GridObject>();
            if (gridObject != null)
            {
                // 리플렉션을 사용하여 필드 설정 (protected 필드 접근)
                var type = typeof(GridObject);
                
                var tileSizeField = type.GetField("tileSize", 
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                tileSizeField?.SetValue(gridObject, data.TileSize);
                
                var gridPositionModeField = type.GetField("gridPositionMode",
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                gridPositionModeField?.SetValue(gridObject, data.gridPositionMode);
                
                var useCustomYField = type.GetField("useCustomY",
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                useCustomYField?.SetValue(gridObject, data.useCustomY);
                
                var customYField = type.GetField("customY",
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                customYField?.SetValue(gridObject, data.customY);
                
                var applyInitialGridSortingField = type.GetField("applyInitialGridSorting",
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                applyInitialGridSortingField?.SetValue(gridObject, data.applyInitialGridSorting);
                
                var gridSortingScaleField = type.GetField("gridSortingScale",
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                gridSortingScaleField?.SetValue(gridObject, data.gridSortingScale);
            }
            
            // Structure 설정
            var structure = obj.GetComponent<Structure>();
            if (structure != null)
            {
                var isBlockingField = typeof(Structure).GetField("isBlocking",
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                if (isBlockingField != null)
                {
                    isBlockingField.SetValue(structure, data.isBlocking);
                }
            }
            
            // MineableObject 설정
            var mineable = obj.GetComponent<MineableObject>();
            if (mineable != null)
            {
                var maxMiningCountField = typeof(MineableObject).GetField("maxMiningCount",
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                if (maxMiningCountField != null)
                {
                    maxMiningCountField.SetValue(mineable, data.maxMiningCount);
                }
                
                var miningStageSpritesField = typeof(MineableObject).GetField("miningStageSprites",
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                if (miningStageSpritesField != null)
                {
                    miningStageSpritesField.SetValue(mineable, data.miningStageSprites);
                }
                
                var itemPrefabField = typeof(MineableObject).GetField("itemPrefab",
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                if (itemPrefabField != null)
                {
                    itemPrefabField.SetValue(mineable, data.itemPrefab);
                }
                
                var minDropCountField = typeof(MineableObject).GetField("minDropCount",
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                if (minDropCountField != null)
                {
                    minDropCountField.SetValue(mineable, data.minDropCount);
                }
                
                var maxDropCountField = typeof(MineableObject).GetField("maxDropCount",
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                if (maxDropCountField != null)
                {
                    maxDropCountField.SetValue(mineable, data.maxDropCount);
                }
                
                var dropRadiusField = typeof(MineableObject).GetField("dropRadius",
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                if (dropRadiusField != null)
                {
                    dropRadiusField.SetValue(mineable, data.dropRadius);
                }
                
                var enableColliderOnStartField = typeof(MineableObject).GetField("enableColliderOnStart",
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                if (enableColliderOnStartField != null)
                {
                    enableColliderOnStartField.SetValue(mineable, data.enableColliderOnStart);
                }
            }
            
            // InteractableGridObject 설정
            var interactable = obj.GetComponent<InteractableGridObject>();
            if (interactable != null)
            {
                var interactionRangeField = typeof(InteractableGridObject).GetField("interactionRange",
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                if (interactionRangeField != null)
                {
                    interactionRangeField.SetValue(interactable, data.interactionRange);
                }
            }
        }
        
        /// <summary>
        /// 자식 Sprite 오브젝트의 스프라이트를 교체합니다.
        /// </summary>
        private static void ReplaceSpriteInChild(GameObject rootObject, CH3_LevelData data)
        {
            // 자식 오브젝트에서 "Sprite" 이름의 오브젝트 찾기
            Transform spriteTransform = rootObject.transform.Find("Sprite");
            if (spriteTransform == null)
            {
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
                Debug.LogWarning($"2.5D_Object의 자식에 'Sprite' 오브젝트를 찾을 수 없습니다.");
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
            
            // GridObject에 자식 오브젝트 참조 설정
            var gridObject = rootObject.GetComponent<GridObject>();
            if (gridObject != null)
            {
                var spriteTransformField = typeof(GridObject).GetField("spriteTransform",
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                if (spriteTransformField != null)
                {
                    spriteTransformField.SetValue(gridObject, spriteTransform);
                }
                
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
                    var gridVolumeTransformField = typeof(GridObject).GetField("gridVolumeTransform",
                        System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                    if (gridVolumeTransformField != null)
                    {
                        gridVolumeTransformField.SetValue(gridObject, gridVolumeTransform);
                    }
                }
                
                var autoBindChildrenField = typeof(GridObject).GetField("autoBindChildren",
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                if (autoBindChildrenField != null)
                {
                    autoBindChildrenField.SetValue(gridObject, false);
                }
            }
            
            // Collider 추가
            var mineable = rootObject.GetComponent<MineableObject>();
            var structure = rootObject.GetComponent<Structure>();
            if (mineable != null || (structure != null && data.isBlocking))
            {
                AddBoxColliderToSprite(spriteTransform, spriteRenderer);
            }
            
            // Breakable인 경우 UI Prefab을 자식 오브젝트로 생성
            var breakable = rootObject.GetComponent<Breakable>();
            if (breakable != null && data.uiPrefab != null)
            {
                bool uiExists = false;
                foreach (Transform child in rootObject.transform)
                {
                    if (child.name == data.uiPrefab.name || 
                        (child.name.Contains(data.uiPrefab.name)))
                    {
                        uiExists = true;
                        break;
                    }
                }
                
                if (!uiExists)
                {
                    GameObject uiObject = Object.Instantiate(data.uiPrefab);
                    uiObject.transform.SetParent(rootObject.transform);
                    uiObject.transform.localPosition = Vector3.zero;
                }
            }
        }
        
        /// <summary>
        /// Sprite 오브젝트에 BoxCollider를 추가합니다.
        /// </summary>
        private static void AddBoxColliderToSprite(Transform spriteTransform, SpriteRenderer spriteRenderer)
        {
            if (spriteTransform == null || spriteRenderer == null) return;
            
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
    }
}

