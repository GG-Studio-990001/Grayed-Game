using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;

public class TurnOff : MonoBehaviour
{
    // TODO: 클래스명 더 적합한 걸로 변경..

    [SerializeField] private Volume _volume;
    [SerializeField] private RectTransform _blackLeft;
    [SerializeField] private RectTransform _blackRight;
    private CinematicBars _cinematicBar;
    private Noise _noise;

    void Start()
    {
        if (_volume != null)
        {
            _volume.profile.TryGet(out _cinematicBar);
            _volume.profile.TryGet(out _noise);
        }

        StartCoroutine(nameof(TurnOffTV));
    }

    IEnumerator TurnOffTV()
    {
        _cinematicBar.amount.value = 0f;

        yield return new WaitForSeconds(1f);

        DOTween.To(() => _cinematicBar.amount.value, x => _cinematicBar.amount.value = x, 0.5f, 0.5f);
        yield return new WaitForSeconds(0.5f);

        CinematicBar2();
        yield return new WaitForSeconds(0.5f);

        _noise.enable.value = false;
    }

    private void CinematicBar2()
    {
        _blackLeft.DOMoveX(0, 0.5f).SetEase(Ease.Linear);
        _blackRight.DOMoveX(0, 0.5f).SetEase(Ease.Linear);
    }
}