using UnityEngine;
using System.Collections.Generic;

namespace Runtime.CH3.Main
{
    /// <summary>
    /// 재화 관리 시스템 - 인벤토리 기반 재화 관리
    /// </summary>
    public class CurrencyManager : MonoBehaviour
    {
        public static CurrencyManager Instance { get; private set; }
        
        [Header("References")]
        [SerializeField] private Inventory inventory;
        
        // 재화 타입과 Item 매핑 (인스펙터에서 설정)
        [System.Serializable]
        public class CurrencyItemMapping
        {
            public ECurrencyData currencyType;
            public Item item;
        }
        
        [SerializeField] private List<CurrencyItemMapping> currencyMappings = new List<CurrencyItemMapping>();
        
        private Dictionary<ECurrencyData, Item> _currencyToItemMap = new Dictionary<ECurrencyData, Item>();
        
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
                return;
            }
            
            if (inventory == null)
            {
                inventory = FindObjectOfType<Inventory>();
            }
            
            foreach (var mapping in currencyMappings)
            {
                if (mapping.item != null)
                {
                    _currencyToItemMap[mapping.currencyType] = mapping.item;
                }
            }
        }
        
        /// <summary>
        /// 재화 보유량 확인
        /// </summary>
        public int GetCurrencyAmount(ECurrencyData currencyType)
        {
            if (!_currencyToItemMap.TryGetValue(currencyType, out Item item))
            {
                return 0;
            }
            
            if (inventory == null) return 0;
            
            int total = 0;
            for (int i = 0; i < inventory.Slots.Count; i++)
            {
                var slot = inventory.GetSlot(i);
                if (slot != null && slot.item == item)
                {
                    total += slot.count;
                }
            }
            
            return total;
        }
        
        /// <summary>
        /// 재화 소모 시도
        /// </summary>
        public bool TryConsumeCurrency(ECurrencyData currencyType, int amount)
        {
            if (amount <= 0) return true;
            
            if (!_currencyToItemMap.TryGetValue(currencyType, out Item item))
            {
                return false;
            }
            
            if (inventory == null) return false;
            
            int remaining = amount;
            
            for (int i = 0; i < inventory.Slots.Count && remaining > 0; i++)
            {
                var slot = inventory.GetSlot(i);
                if (slot != null && slot.item == item && slot.count > 0)
                {
                    int consume = Mathf.Min(remaining, slot.count);
                    if (inventory.TryConsumeAt(i, consume))
                    {
                        remaining -= consume;
                    }
                }
            }
            
            return remaining == 0;
        }
        
        /// <summary>
        /// 재화 보유 여부 확인
        /// </summary>
        public bool HasCurrency(ECurrencyData currencyType, int amount)
        {
            return GetCurrencyAmount(currencyType) >= amount;
        }
        
        /// <summary>
        /// 여러 재화 보유 여부 확인
        /// </summary>
        public bool HasCurrencies(List<CurrencyData> currencies)
        {
            if (currencies == null || currencies.Count == 0) return true;
            
            foreach (var currency in currencies)
            {
                if (!HasCurrency(currency.currency, currency.amount))
                {
                    return false;
                }
            }
            
            return true;
        }
        
        /// <summary>
        /// 여러 재화 소모 시도
        /// </summary>
        public bool TryConsumeCurrencies(List<CurrencyData> currencies)
        {
            if (currencies == null || currencies.Count == 0) return true;
            
            if (!HasCurrencies(currencies))
            {
                return false;
            }
            
            foreach (var currency in currencies)
            {
                if (!TryConsumeCurrency(currency.currency, currency.amount))
                {
                    return false;
                }
            }
            
            return true;
        }
        
        /// <summary>
        /// 재화 추가 시도
        /// </summary>
        public bool TryAddCurrency(ECurrencyData currencyType, int amount)
        {
            if (amount <= 0) return true;
            
            if (!_currencyToItemMap.TryGetValue(currencyType, out Item item))
            {
                Debug.LogWarning($"CurrencyManager: {currencyType}에 해당하는 Item 매핑이 없습니다!");
                return false;
            }
            
            if (inventory == null)
            {
                Debug.LogWarning("CurrencyManager: 인벤토리를 찾을 수 없습니다!");
                return false;
            }
            
            return inventory.TryAdd(item, amount);
        }
        
        /// <summary>
        /// 여러 재화 추가 시도
        /// </summary>
        public bool TryAddCurrencies(List<CurrencyData> currencies)
        {
            if (currencies == null || currencies.Count == 0) return true;
            
            bool allSuccess = true;
            foreach (var currency in currencies)
            {
                if (!TryAddCurrency(currency.currency, currency.amount))
                {
                    allSuccess = false;
                }
            }
            
            return allSuccess;
        }
    }
}

