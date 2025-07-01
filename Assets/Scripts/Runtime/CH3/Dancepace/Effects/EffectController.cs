using UnityEngine;
using Runtime.ETC;
using Runtime.CH3.Dancepace;

namespace Runtime.CH3.Dancepace
{
    public class EffectController : MonoBehaviour
    {
        [Header("이펙트 프리팹")]
        [SerializeField] private GameObject heartPrefab;
        
        [Header("관중석 스팟")]
        [SerializeField] private Transform audienceSpot;

        public void SpawnHeartParticles(EJudgmentType type)
        {
            if (audienceSpot == null || heartPrefab == null) return;

            var box = audienceSpot.GetComponent<BoxCollider2D>();
            if (box == null)
            {
                Debug.LogWarning("audienceSpot에 BoxCollider2D가 없습니다.");
                return;
            }

            int count = GetHeartParticleCount(type);
            if (count == 0) return;

            var bounds = box.bounds;
            for (int i = 0; i < count; i++)
            {
                float randomX = UnityEngine.Random.Range(bounds.min.x, bounds.max.x);
                float randomY = UnityEngine.Random.Range(bounds.min.y, bounds.max.y);
                Vector2 randomLocalPosition = new Vector2(randomX, randomY);


                var heartObj = Instantiate(heartPrefab);
                heartObj.transform.localPosition = randomLocalPosition;
                var heartEffect = heartObj.GetComponent<HeartEffect>();
                if (heartEffect != null)
                {
                    heartEffect.PlayEffect();
                }
            }
        }

        private int GetHeartParticleCount(EJudgmentType type)
        {
            return type switch
            {
                EJudgmentType.Perfect => UnityEngine.Random.Range(3, 6),
                EJudgmentType.Great => UnityEngine.Random.Range(1, 3),
                EJudgmentType.Bad => 0,
                _ => 0
            };
        }
    }
}