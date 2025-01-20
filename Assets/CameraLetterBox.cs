using UnityEngine;
using DG.Tweening;
using Cinemachine;

public class CameraLetterbox : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera _virtualCamera;
    [SerializeField] private RectTransform _topBar;
    [SerializeField] private RectTransform _bottomBar;
    [SerializeField] private RectTransform _leftBar;
    [SerializeField] private RectTransform _rightBar;
    [SerializeField] private float _transitionDuration = 0.5f;
    [SerializeField] private Ease _easeType = Ease.InOutQuad;

    public enum AspectRatio
    {
        Ratio_4_3,     // 4:3
        Ratio_8_7,    // 8:7
        Ratio_21_9    // 21:9
    }

    [SerializeField] private AspectRatio _targetAspectRatio = AspectRatio.Ratio_4_3;
    private Sequence _currentSequence;
    private float _initialOrthographicSize;

    private void Awake()
    {
        if (_virtualCamera == null)
            _virtualCamera = FindObjectOfType<CinemachineVirtualCamera>();
            
        _initialOrthographicSize = _virtualCamera.m_Lens.OrthographicSize;
    }

    private void Start()
    {
        SetAspectRatio(_targetAspectRatio, true);
    }

    public void SetAspectRatio(AspectRatio ratio, bool immediate = false)
    {
        float targetRatio;
        float defaultRatio = 16f / 9f;
        
        switch (ratio)
        {
            case AspectRatio.Ratio_4_3:
                targetRatio = 4f / 3f;  // 약 1.33:1
                break;
            case AspectRatio.Ratio_8_7:
                targetRatio = 8f / 7f;
                break;
            case AspectRatio.Ratio_21_9:
                targetRatio = 21f / 9f;
                break;
            default:
                targetRatio = 8f / 7f;
                break;
        }

        float currentRatio = (float)Screen.width / Screen.height;

        if (_currentSequence != null && _currentSequence.IsPlaying())
        {
            _currentSequence.Kill();
        }

        // 시네머신 카메라 OrthographicSize 조정값 계산
        float targetOrthoSize = _initialOrthographicSize;
        if (ratio == AspectRatio.Ratio_8_7)
        {
            targetOrthoSize = _initialOrthographicSize * (defaultRatio / targetRatio);
        }
        else if (ratio == AspectRatio.Ratio_21_9)
        {
            targetOrthoSize = _initialOrthographicSize;
        }

        if (currentRatio > targetRatio)
        {
            float targetWidth = Screen.height * targetRatio;
            float barWidth = (Screen.width - targetWidth) / 2f;

            if (immediate)
            {
                _leftBar.sizeDelta = new Vector2(barWidth, _leftBar.sizeDelta.y);
                _rightBar.sizeDelta = new Vector2(barWidth, _rightBar.sizeDelta.y);
                _topBar.sizeDelta = new Vector2(_topBar.sizeDelta.x, 0);
                _bottomBar.sizeDelta = new Vector2(_bottomBar.sizeDelta.x, 0);
                _virtualCamera.m_Lens.OrthographicSize = targetOrthoSize;
            }
            else
            {
                _currentSequence = DOTween.Sequence();

                _currentSequence.Join(_topBar.DOSizeDelta(new Vector2(_topBar.sizeDelta.x, 0), _transitionDuration).SetEase(_easeType));
                _currentSequence.Join(_bottomBar.DOSizeDelta(new Vector2(_bottomBar.sizeDelta.x, 0), _transitionDuration).SetEase(_easeType));
                _currentSequence.Join(_leftBar.DOSizeDelta(new Vector2(barWidth, _leftBar.sizeDelta.y), _transitionDuration).SetEase(_easeType));
                _currentSequence.Join(_rightBar.DOSizeDelta(new Vector2(barWidth, _rightBar.sizeDelta.y), _transitionDuration).SetEase(_easeType));

                // 시네머신 카메라 크기 애니메이션
                _currentSequence.Join(DOTween.To(() => _virtualCamera.m_Lens.OrthographicSize,
                    value => _virtualCamera.m_Lens.OrthographicSize = value,
                    targetOrthoSize, _transitionDuration).SetEase(_easeType));
            }
        }
        else
        {
            float targetHeight = Screen.width / targetRatio;
            float barHeight = (Screen.height - targetHeight) / 2f;

            if (immediate)
            {
                _topBar.sizeDelta = new Vector2(_topBar.sizeDelta.x, barHeight);
                _bottomBar.sizeDelta = new Vector2(_bottomBar.sizeDelta.x, barHeight);
                _leftBar.sizeDelta = new Vector2(0, _leftBar.sizeDelta.y);
                _rightBar.sizeDelta = new Vector2(0, _rightBar.sizeDelta.y);
                _virtualCamera.m_Lens.OrthographicSize = targetOrthoSize;
            }
            else
            {
                _currentSequence = DOTween.Sequence();

                _currentSequence.Join(_leftBar.DOSizeDelta(new Vector2(0, _leftBar.sizeDelta.y), _transitionDuration).SetEase(_easeType));
                _currentSequence.Join(_rightBar.DOSizeDelta(new Vector2(0, _rightBar.sizeDelta.y), _transitionDuration).SetEase(_easeType));
                _currentSequence.Join(_topBar.DOSizeDelta(new Vector2(_topBar.sizeDelta.x, barHeight), _transitionDuration).SetEase(_easeType));
                _currentSequence.Join(_bottomBar.DOSizeDelta(new Vector2(_bottomBar.sizeDelta.x, barHeight), _transitionDuration).SetEase(_easeType));

                // 시네머신 카메라 크기 애니메이션
                _currentSequence.Join(DOTween.To(() => _virtualCamera.m_Lens.OrthographicSize,
                    value => _virtualCamera.m_Lens.OrthographicSize = value,
                    targetOrthoSize, _transitionDuration).SetEase(_easeType));
            }
        }
    }

    public void Set8to7Ratio()
    {
        SetAspectRatio(AspectRatio.Ratio_8_7);
    }

    public void Set21to9Ratio()
    {
        SetAspectRatio(AspectRatio.Ratio_21_9);
    }
    public void Set4to3Ratio()
    {
        SetAspectRatio(AspectRatio.Ratio_4_3);
    }

    private void OnDestroy()
    {
        if (_currentSequence != null)
        {
            _currentSequence.Kill();
        }
    }
}