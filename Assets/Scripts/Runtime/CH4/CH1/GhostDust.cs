using UnityEngine;
using DG.Tweening;

namespace CH4.CH1
{
    public class GhostDust : MonoBehaviour
    {
        [SerializeField] private Vector3 _pos1;
        [SerializeField] private Vector3 _pos2;
        private float _duration = 1.5f;

        private void Start()
        {
            transform.localPosition = _pos1;

            transform.DOLocalMove(_pos2, _duration)
                .SetEase(Ease.Linear)
                .SetLoops(-1, LoopType.Yoyo);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                Debug.Log("GhostDust hit Player");
            }
        }
    }
}
