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
            _target.SetActive(false); // 오브젝트를 숨기기
        }

        private IEnumerator BlinkTarget()
        {
            while (true)
            {
                _target.SetActive(true);
                yield return new WaitForSeconds(_showTime);
                _target.SetActive(false);
                yield return new WaitForSeconds(_hideTime);
            }
        }
    }
}