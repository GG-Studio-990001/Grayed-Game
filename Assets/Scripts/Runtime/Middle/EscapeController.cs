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

        yield return new WaitForSeconds(1.2f);

        VerticalCinematicBar();
        yield return new WaitForSeconds(0.2f);

        HorizontalCinematicBar();
        yield return new WaitForSeconds(0.2f);

        LightAppear();
        yield return new WaitForSeconds(0.3f);

        LightDisappear();
        yield return new WaitForSeconds(0.1f);

        _noise.enable.value = false;
    }

    private void VerticalCinematicBar()
    {
        DOTween.To(() => _cinematicBar.amount.value, x => _cinematicBar.amount.value = x, 0.5f, 0.2f);
    }

    private void HorizontalCinematicBar()
    {
        _blackLeft.DOMoveX(-62.5f, 0.2f);
        _blackRight.DOMoveX(62.5f, 0.2f);
    }

    private void LightAppear()
    {
        _light.DOScale(new Vector3(0.5f, 0.5f, 0.5f), 0.3f).SetEase(Ease.Linear);
    }

    private void LightDisappear()
    {
        _light.DOScale(new Vector3(0f, 0f, 0f), 0.1f).SetEase(Ease.Linear);
    }
}