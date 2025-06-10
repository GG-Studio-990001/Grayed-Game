using UnityEngine;
using System;

namespace Runtime.CH3.Dancepace
{
    public class EffectManager : MonoBehaviour
    {
        [Header("이펙트 프리팹/파티클")]
        [SerializeField] private ParticleSystem heartParticlePrefab;
        [SerializeField] private GameObject coinEffectPrefab;
        [Header("관중석 스팟")]
        [SerializeField] private Transform[] audienceSpots;

        public void SpawnHeartParticles(JudgmentType type)
        {
            int min = 0, max = 0;
            switch (type)
            {
                case JudgmentType.Great:
                    min = 3; max = 4;
                    break;
                case JudgmentType.Good:
                    min = 1; max = 2;
                    break;
                case JudgmentType.Bad:
                    min = 0; max = 0;
                    break;
            }
            int count = UnityEngine.Random.Range(min, max + 1);
            for (int i = 0; i < count; i++)
            {
                if (audienceSpots.Length == 0 || heartParticlePrefab == null) return;
                var spot = audienceSpots[UnityEngine.Random.Range(0, audienceSpots.Length)];
                var particle = Instantiate(heartParticlePrefab, spot.position, Quaternion.identity);
                particle.Play();
            }
        }

        public void SpawnCoinEffect(Vector3 position)
        {
            if (coinEffectPrefab != null)
                Instantiate(coinEffectPrefab, position, Quaternion.identity);
        }
    }
} 