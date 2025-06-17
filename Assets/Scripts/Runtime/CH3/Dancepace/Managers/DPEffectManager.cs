using UnityEngine;
using System;

namespace Runtime.CH3.Dancepace
{
    public class DPEffectManager : MonoBehaviour
    {
        [Header("이펙트 프리팹")]
        [SerializeField] private GameObject heartPrefab;
        
        [Header("관중석 스팟")]
        [SerializeField] private Transform audienceSpot;

        public void SpawnHeartParticles(JudgmentType type)
        {
            if (audienceSpot == null || heartPrefab == null) return;

            int count = GetHeartParticleCount(type);
            if (count == 0) return;

            for (int i = 0; i < count; i++)
            {
                // audienceSpot의 영역 내에서 랜덤한 위치 계산
                Vector3 spotPosition = audienceSpot.position;
                Vector3 spotScale = audienceSpot.localScale;
                
                float randomX = spotPosition.x + UnityEngine.Random.Range(-spotScale.x/2, spotScale.x/2);
                float randomY = spotPosition.y + UnityEngine.Random.Range(-spotScale.y/2, spotScale.y/2);
                Vector2 randomPosition = new Vector2(randomX, randomY);

                var heartObj = Instantiate(heartPrefab, randomPosition, Quaternion.identity);
                var heartEffect = heartObj.GetComponent<HeartEffect>();
                if (heartEffect != null)
                {
                    heartEffect.PlayEffect();
                }
            }
        }

        private int GetHeartParticleCount(JudgmentType type)
        {
            return type switch
            {
                JudgmentType.Great => UnityEngine.Random.Range(3, 5),
                JudgmentType.Good => UnityEngine.Random.Range(1, 3),
                JudgmentType.Bad => 0,
                _ => 0
            };
        }

        private void OnDrawGizmos()
        {
            if (audienceSpot == null) return;

            // Transform의 영역 표시
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(audienceSpot.position, audienceSpot.localScale);
        }
    }
}