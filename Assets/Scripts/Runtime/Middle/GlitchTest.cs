using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

namespace Runtime.Middle
{
    public class GlitchTest : MonoBehaviour
    {
        [SerializeField] private Volume _volume;

        public void GlitchOn()
        {
            // 역접속
            StartCoroutine(nameof(ActiveGlitch));
        }

        IEnumerator ActiveGlitch()
        {
            Debug.Log("Glitch On");
            _volume.weight = 0.59f;
            yield return new WaitForSeconds(1f);
            _volume.weight = 0;
            Debug.Log("Glitch Off");
        }
    }
}