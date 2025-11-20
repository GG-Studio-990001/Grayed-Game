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
            
            // 매핑 딕셔너리 초기화
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
            
            // 인벤토리에서 해당 재화 아이템 찾아서 소모
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
            
            // 먼저 모든 재화가 충분한지 확인
            if (!HasCurrencies(currencies))
            {
                return false;
            }
            
            // 모든 재화 소모
            foreach (var currency in currencies)
            {
                if (!TryConsumeCurrency(currency.currency, currency.amount))
                {
                    return false;
                }
            }
            
            return true;
        }
    }
}

