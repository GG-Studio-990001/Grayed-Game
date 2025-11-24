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
            
            if (_buildingData.productionCurrency == null || _buildingData.productionCurrency.Count == 0)
            {
                Debug.Log($"BuildingProduction: 생산할 재화가 설정되지 않았습니다. ({_buildingData.id})");
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
            if (_buildingData == null || _buildingData.productionCurrency == null) return;
            
            if (CurrencyManager.Instance != null)
            {
                bool success = CurrencyManager.Instance.TryAddCurrencies(_buildingData.productionCurrency);
                if (success)
                {
                    Debug.Log($"건물 생산 완료: {_buildingData.id}");
                }
                else
                {
                    Debug.LogWarning($"건물 생산 실패 (인벤토리 가득참?): {_buildingData.id}");
                }
            }
            else
            {
                Debug.LogWarning("BuildingProduction: CurrencyManager를 찾을 수 없습니다!");
            }
        }
        
        private void OnDestroy()
        {
            StopProduction();
        }
    }
}

