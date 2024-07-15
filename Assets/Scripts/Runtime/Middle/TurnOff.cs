using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

public class TurnOff : MonoBehaviour
{
    [SerializeField] private Volume _volume;
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

        yield return new WaitForSeconds(1.5f);

        _noise.enable.value = false;
        DOTween.To(() => _cinematicBar.amount.value, x => _cinematicBar.amount.value = x, 1f, 0.5f);
    }
}