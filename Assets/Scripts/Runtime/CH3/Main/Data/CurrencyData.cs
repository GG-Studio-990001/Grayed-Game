using UnityEngine;

namespace Runtime.CH3.Main
{
    /// <summary>
    /// 재화 데이터 구조체
    /// 건설 비용, 드랍 리소스 등에 사용
    /// </summary>
    [System.Serializable]
    public class CurrencyData
    {
        [Tooltip("재화 타입")]
        public ECurrencyData currency;
        
        [Tooltip("재화 개수")]
        public int amount;
        
        [Tooltip("드랍 확률 (0~1, 드랍에만 사용)")]
        [Range(0f, 1f)]
        public float ratio = 1f;
        
        public CurrencyData()
        {
            currency = ECurrencyData.ResourceDefault;
            amount = 1;
            ratio = 1f;
        }
        
        public CurrencyData(ECurrencyData currency, int amount, float ratio = 1f)
        {
            this.currency = currency;
            this.amount = amount;
            this.ratio = ratio;
        }
    }
}

