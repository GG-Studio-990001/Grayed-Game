using UnityEngine;
using System;

namespace Runtime.CH3.Dancepace
{
    public class DPEffectManager : MonoBehaviour
    {
        [Header("이펙트 프리팹/파티클")]
        [SerializeField] private ParticleSystem heartParticlePrefab;
        [SerializeField] private GameObject coinEffectPrefab;
        
        [Header("관중석 스팟")]
        [SerializeField] private Transform[] audienceSpots;

        [Header("이펙트 설정")]
        [SerializeField] private float heartParticleLifetime = 2f;
        [SerializeField] private float coinEffectLifetime = 1f;

        public void SpawnHeartParticles(JudgmentType type)
        {
            if (audienceSpots.Length == 0 || heartParticlePrefab == null) return;

            int count = GetHeartParticleCount(type);
            for (int i = 0; i < count; i++)
            {
                var spot = GetRandomAudienceSpot();
                var particle = Instantiate(heartParticlePrefab, spot.position, Quaternion.identity);
                particle.Play();
                Destroy(particle.gameObject, heartParticleLifetime);
            }
        }

        public void SpawnCoinEffect(Vector3 position)
        {
            if (coinEffectPrefab == null) return;
            
            var effect = Instantiate(coinEffectPrefab, position, Quaternion.identity);
            Destroy(effect, coinEffectLifetime);
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

        private Transform GetRandomAudienceSpot()
        {
            return audienceSpots[UnityEngine.Random.Range(0, audienceSpots.Length)];
        }
    }
} 