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
            
            Vector3 worldPosition = gridSystem.GridToWorldPosition(gridPosition);
            GameObject rootObject = Object.Instantiate(_basePrefab);
            rootObject.transform.position = worldPosition;
            
            GridObject gridObject;
            if (data.isBuilding)
            {
                gridObject = rootObject.AddComponent<Producer>();
            }
            else
            {
                gridObject = AddGridObjectComponent(rootObject, data.objectType, data);
            }
            
            if (gridObject == null)
            {
                Debug.LogError($"GridObject 컴포넌트를 추가할 수 없습니다: {data.objectType}");
                Object.Destroy(rootObject);
                return null;
            }
            
            ApplyDataToObject(rootObject, data, gridPosition);
            
            // Producer의 경우 objectType을 명시적으로 설정 (InitializeFromData 이후에 설정)
            if (data.isBuilding)
            {
                gridObject.SetObjectType(GridObjectType.Producer);
            }
            
            ReplaceSpriteInChild(rootObject, data);
            gridObject.Initialize(gridPosition);
            
            if (data.isBuilding)
            {
                BuildingProduction production = rootObject.GetComponent<BuildingProduction>();
                if (production == null)
                {
                    production = rootObject.AddComponent<BuildingProduction>();
                }
                production.StartProduction(data);
            }
            
            return rootObject;
        }
        
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
        
        private static void ApplyDataToObject(GameObject obj, CH3_LevelData data, Vector2Int gridPosition)
        {
            var gridObject = obj.GetComponent<GridObject>();
            if (gridObject != null)
            {
                gridObject.InitializeFromData(data);
            }
        }
        
        private static void ReplaceSpriteInChild(GameObject rootObject, CH3_LevelData data)
        {
            Transform spriteTransform = CH3Utils.FindChildByNameIgnoreCase(rootObject.transform, "Sprite");
            if (spriteTransform == null)
            {
                Debug.LogWarning($"2.5D_Object의 자식에 'Sprite' 오브젝트를 찾을 수 없습니다.");
                return;
            }
            
            SpriteRenderer spriteRenderer = spriteTransform.GetComponent<SpriteRenderer>();
            if (spriteRenderer == null)
            {
                Debug.LogWarning($"Sprite 오브젝트에 SpriteRenderer 컴포넌트가 없습니다.");
                return;
            }
            
            if (data.sprite != null)
            {
                spriteRenderer.sprite = data.sprite;
            }
            
            var gridObject = rootObject.GetComponent<GridObject>();
            if (gridObject != null)
            {
                Transform gridVolumeTransform = CH3Utils.FindChildByNameIgnoreCase(rootObject.transform, "GridVolume");
                gridObject.SetChildReferences(spriteTransform, gridVolumeTransform, false);
            }
            
            UpdateSpriteCollider(spriteTransform, spriteRenderer, data);
            
            var breakable = rootObject.GetComponent<Breakable>();
            if (breakable != null && data.uiPrefab != null)
            {
                bool uiExists = false;
                foreach (Transform child in rootObject.transform)
                {
                    if (child.name == data.uiPrefab.name || child.name.Contains(data.uiPrefab.name))
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
        
        private static void UpdateSpriteCollider(Transform spriteTransform, SpriteRenderer spriteRenderer, CH3_LevelData data)
        {
            if (spriteTransform == null || spriteRenderer == null) return;
            
            BoxCollider collider = spriteTransform.GetComponent<BoxCollider>();
            if (collider == null) return;
            
            CH3Utils.UpdateColliderToSprite(collider, spriteRenderer);
            
            var gridObject = spriteTransform.parent?.GetComponent<GridObject>();
            if (gridObject != null)
            {
                CH3Utils.SetColliderTriggerByGridObject(collider, gridObject, data);
            }
            else
            {
                collider.isTrigger = false;
            }
        }
    }
}

