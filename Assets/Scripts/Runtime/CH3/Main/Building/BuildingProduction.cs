using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Runtime.CH3.Main
{
    /// <summary>
    /// 건물 생산 시스템 - 설치된 건물이 재료를 생산하는 컴포넌트
    /// </summary>
    public class BuildingProduction : MonoBehaviour
    {
        private CH3_LevelData _buildingData;
        private Inventory _inventory;
        private Coroutine _productionCoroutine;
        private bool _isProducing = false;
        
        // 생산된 아이템 임시 저장소 (Item -> 개수)
        private Dictionary<Item, int> _producedItems = new Dictionary<Item, int>();
        
        public event System.Action OnProductionChanged;
        
        /// <summary>
        /// 생산 시작
        /// </summary>
        public void StartProduction(CH3_LevelData buildingData)
        {
            if (buildingData == null)
            {
                Debug.LogWarning("BuildingProduction: buildingData가 null입니다!");
                return;
            }
            
            _buildingData = buildingData;
            
            if (_buildingData.productionItems == null || _buildingData.productionItems.Count == 0)
            {
                Debug.Log($"BuildingProduction: 생산할 아이템이 설정되지 않았습니다. ({_buildingData.id})");
                return;
            }
            
            if (_buildingData.productionInterval <= 0)
            {
                Debug.LogWarning($"BuildingProduction: 생산 간격이 0 이하입니다. ({_buildingData.id})");
                return;
            }
            
            if (_inventory == null)
            {
                _inventory = FindObjectOfType<Inventory>();
            }
            
            if (_inventory == null)
            {
                Debug.LogWarning("BuildingProduction: 인벤토리를 찾을 수 없습니다!");
                return;
            }
            
            if (!_isProducing)
            {
                _isProducing = true;
                _productionCoroutine = StartCoroutine(ProductionLoop());
            }
        }
        
        /// <summary>
        /// 생산 중지
        /// </summary>
        public void StopProduction()
        {
            _isProducing = false;
            if (_productionCoroutine != null)
            {
                StopCoroutine(_productionCoroutine);
                _productionCoroutine = null;
            }
        }
        
        private IEnumerator ProductionLoop()
        {
            while (_isProducing && _buildingData != null)
            {
                yield return new WaitForSeconds(_buildingData.productionInterval);
                
                if (!_isProducing) break;
                ProduceResources();
            }
        }
        
        private void ProduceResources()
        {
            if (_buildingData == null || _buildingData.productionItems == null) return;
            
            foreach (var productionData in _buildingData.productionItems)
            {
                if (productionData.item == null)
                {
                    Debug.LogWarning($"BuildingProduction: 생산 아이템이 null입니다!");
                    continue;
                }
                
                Item item = productionData.item;
                int currentCount = _producedItems.TryGetValue(item, out int count) ? count : 0;
                int maxProduction = _buildingData.maxProduction > 0 ? _buildingData.maxProduction : int.MaxValue;
                
                if (currentCount >= maxProduction)
                {
                    continue;
                }
                
                int canAdd = Mathf.Min(productionData.amount, maxProduction - currentCount);
                if (canAdd > 0)
                {
                    if (_producedItems.ContainsKey(item))
                    {
                        _producedItems[item] += canAdd;
                    }
                    else
                    {
                        _producedItems[item] = canAdd;
                    }
                    
                    OnProductionChanged?.Invoke();
                }
            }
        }
        
        /// <summary>
        /// 생산된 아이템 개수 반환
        /// </summary>
        public Dictionary<Item, int> GetProducedItems()
        {
            return new Dictionary<Item, int>(_producedItems);
        }
        
        /// <summary>
        /// 특정 아이템의 생산 개수 반환
        /// </summary>
        public int GetProducedCount(Item item)
        {
            return _producedItems.TryGetValue(item, out int count) ? count : 0;
        }
        
        /// <summary>
        /// 생산된 아이템을 인벤토리로 수거하고 0으로 초기화
        /// </summary>
        public bool RecoverProducedItems()
        {
            if (_inventory == null)
            {
                _inventory = FindObjectOfType<Inventory>();
            }
            
            if (_inventory == null)
            {
                Debug.LogWarning("BuildingProduction: 인벤토리를 찾을 수 없습니다!");
                return false;
            }
            
            bool allSuccess = true;
            var itemsToClear = new List<Item>();
            
            foreach (var kvp in _producedItems)
            {
                Item item = kvp.Key;
                int count = kvp.Value;
                
                if (item == null || count <= 0) continue;
                
                if (_inventory.TryAdd(item, count))
                {
                    itemsToClear.Add(item);
                }
                else
                {
                    Debug.LogWarning($"BuildingProduction: {item.itemName} {count}개를 인벤토리에 추가하는데 실패했습니다. 인벤토리가 가득 찼을 수 있습니다.");
                    allSuccess = false;
                }
            }
            
            foreach (var item in itemsToClear)
            {
                _producedItems.Remove(item);
            }
            
            if (itemsToClear.Count > 0)
            {
                OnProductionChanged?.Invoke();
            }
            
            return allSuccess;
        }
        
        
        private void OnDestroy()
        {
            StopProduction();
        }
    }
}

