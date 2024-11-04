using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectFadeInOutComponent : MonoBehaviour
{
    SpriteRenderer _renderer;
    [SerializeField] private float fadeDeltaTime = 0.01f;


    public void SetTargetComponent(GameObject InObject)
    {
        _renderer = InObject.GetComponent<SpriteRenderer>();
    }

    public void PlayFadeIn(float InTime)
    {
        if(_renderer != null)
        {
            StartCoroutine(FadeInCoroutine(InTime));
        }
    }

    public void PlayFadeOut(float InTime)
    {
        if (_renderer != null)
        {
            StartCoroutine(FadeOutCoroutine(InTime));
        }
    }

    private IEnumerator FadeInCoroutine(float duration)
    {
        float time = 0;
        while (time < duration)
        {
            time += fadeDeltaTime;
            Color _newColor = _renderer.color;
            _newColor.a = 1 - time / duration;
            _renderer.color = _newColor;
            yield return new WaitForSeconds(fadeDeltaTime);
        }
    }

    private IEnumerator FadeOutCoroutine(float duration)
    {
        float time = 0;
        while (time < duration)
        {
            time += fadeDeltaTime;
            Color _newColor = _renderer.color;
            _newColor.a = time/ duration;
            _renderer.color = _newColor;
            yield return new WaitForSeconds(fadeDeltaTime);
        }
    }
}
