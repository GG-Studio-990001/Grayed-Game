using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

public class EscapeController : MonoBehaviour
{
    [SerializeField] private Volume _volume;
    [SerializeField] private Transform _blackLeft;
    [SerializeField] private Transform _blackRight;
    [SerializeField] private Transform _light;
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

        CinematicBar1();
        yield return new WaitForSeconds(0.5f);

        CinematicBar2();
        yield return new WaitForSeconds(0.4f);

        CinematicBar3();
        yield return new WaitForSeconds(0.1f);

        _noise.enable.value = false;
    }

    private void CinematicBar1()
    {
        DOTween.To(() => _cinematicBar.amount.value, x => _cinematicBar.amount.value = x, 0.5f, 0.5f);
    }

    private void CinematicBar2()
    {
        _blackLeft.DOMoveX(-62.5f, 0.4f).SetEase(Ease.Linear);
        _blackRight.DOMoveX(62.5f, 0.4f).SetEase(Ease.Linear);
        _light.DOScale(new Vector3(0.6f, 0.6f, 0.6f), 0.4f).SetEase(Ease.Linear);
    }

    private void CinematicBar3()
    {
        _light.DOScale(new Vector3(0f, 0f, 0f), 0.1f).SetEase(Ease.Linear);
    }

    // 총 2초동안 진행된다
    // 1초 동안은 지직거리고
    // 0.5초동안은 상하로 수축한다.
    // 나머지 0.5초 동안 빛이 생겼다 없어지고
    // 동시에 좌우로도 수축한다.
}