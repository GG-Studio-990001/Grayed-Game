using System.Collections;
using UnityEngine;

namespace Runtime.CH1.Main
{
    public class NewBlink : MonoBehaviour
    {
        [SerializeField] private GameObject _target;
        [SerializeField] private float _showTime = 0.5f;
        [SerializeField] private float _hideTime = 0.5f;
        private Coroutine _blinkingCoroutine;

        public void StartBlinking()
        {
            _blinkingCoroutine ??= StartCoroutine(BlinkTarget());
        }

        public void StopBlinking()
        {
            if (_blinkingCoroutine != null)
            {
                StopCoroutine(_blinkingCoroutine);
                _blinkingCoroutine = null;
            }
            _target.SetActive(false);
        }

        private IEnumerator BlinkTarget()
        {
            while (true)
            {
                // 설정창이 켜졌을 때도 작동해야 하니 타임스케일에 영향 안받도록 수정
                _target.SetActive(true);
                yield return new WaitForSecondsRealtime(_showTime);
                _target.SetActive(false);
                yield return new WaitForSecondsRealtime(_hideTime);
            }
        }
    }
}