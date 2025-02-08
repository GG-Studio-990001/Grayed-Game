using UnityEngine;
using DG.Tweening;
using Cinemachine;
using Runtime.ETC;
using System;
using UnityEngine.UI;

public class CameraLetterbox : MonoBehaviour
{
    [Header("Camera Settings")]
    [SerializeField] private CinemachineVirtualCamera _virtualCamera;
    [SerializeField] private RectTransform _topBar;
    [SerializeField] private RectTransform _bottomBar;
    [SerializeField] private RectTransform _leftBar;
    [SerializeField] private RectTransform _rightBar;
    [SerializeField] private float _transitionDuration = 0.5f;
    [SerializeField] private Ease _easeType = Ease.InOutQuad;

    // 렌더 텍스처와 카메라 설정
    [SerializeField] private Camera _uiCamera; // UI 카메라
    [SerializeField] private RawImage _gameScreenImage; // 게임 화면을 표시할 RawImage
    [SerializeField] private AspectRatio _targetAspectRatio; // 기본 비율을 8:7로 설정
    
    private Camera _mainCamera;
    private RenderTexture _renderTexture;
    private Sequence _currentSequence;
    private int _baseWidth = 1920;  // 기본 해상도 너비
    private int _baseHeight = 1080; // 기본 해상도 높이

    private void Awake()
    {
        if (_virtualCamera == null)
            _virtualCamera = FindObjectOfType<CinemachineVirtualCamera>();
        _mainCamera = Camera.main;
        InitializeRenderTexture();
        SetAspectRatio(_targetAspectRatio, true);
        //SetAspectRatio(AspectRatio.Ratio_8_7, true);
        //_targetAspectRatio = AspectRatio.Ratio_8_7;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            switch (_targetAspectRatio)
            {
                case AspectRatio.Ratio_8_7:
                    _targetAspectRatio = AspectRatio.Ratio_21_9;
                    SetAspectRatio(_targetAspectRatio);
                    break;
                case AspectRatio.Ratio_21_9:
                    _targetAspectRatio = AspectRatio.Ratio_8_7;
                    SetAspectRatio(_targetAspectRatio);
                    break;
            }
        }
    }

    private void InitializeRenderTexture()
    {
        // 기존 렌더 텍스처가 있다면 제거
        if (_renderTexture != null)
            _renderTexture.Release();

        // 새로운 렌더 텍스처 생성
        _renderTexture = new RenderTexture(_baseWidth, _baseHeight, 24);
        _renderTexture.antiAliasing = 8;

        // 게임 카메라의 타겟을 렌더 텍스처로 설정
        _mainCamera.targetTexture = _renderTexture;

        // UI 카메라는 렌더 텍스처를 사용하지 않음
        _uiCamera.targetTexture = null;

        // RawImage에 렌더 텍스처 할당
        _gameScreenImage.texture = _renderTexture;
    }

    public void SetAspectRatio(AspectRatio ratio, bool immediate = false)
    {
        float targetRatio = 0f;
        switch (ratio)
        {
            case AspectRatio.Ratio_8_7:
                targetRatio = 8f / 7f; // 8:7 비율
                break;
            case AspectRatio.Ratio_21_9:
                targetRatio = 21f / 9f; // 21:9 비율
                break;
        }

        // 레터박스 설정
        SetLetterbox(targetRatio, immediate);
    }

    private void SetLetterbox(float targetRatio, bool immediate)
    {
        float screenRatio = (float)Screen.width / Screen.height;

        if (screenRatio > targetRatio)
        {
            // 좌우 레터박스
            float barWidth = (Screen.width - (Screen.height * targetRatio)) / 2f;
            if (immediate)
            {
                _leftBar.sizeDelta = new Vector2(barWidth, _leftBar.sizeDelta.y);
                _rightBar.sizeDelta = new Vector2(barWidth, _rightBar.sizeDelta.y);
                _topBar.sizeDelta = new Vector2(0, _topBar.sizeDelta.y);
                _bottomBar.sizeDelta = new Vector2(0, _bottomBar.sizeDelta.y);
            }
            else
            {
                _currentSequence = DOTween.Sequence();
                _currentSequence.Join(_leftBar.DOSizeDelta(new Vector2(barWidth, _leftBar.sizeDelta.y), _transitionDuration).SetEase(_easeType));
                _currentSequence.Join(_rightBar.DOSizeDelta(new Vector2(barWidth, _rightBar.sizeDelta.y), _transitionDuration).SetEase(_easeType));
                _currentSequence.Join(_topBar.DOSizeDelta(new Vector2(0, _topBar.sizeDelta.y), _transitionDuration).SetEase(_easeType));
                _currentSequence.Join(_bottomBar.DOSizeDelta(new Vector2(0, _bottomBar.sizeDelta.y), _transitionDuration).SetEase(_easeType));
            }
        }
        else
        {
            float barHeight = (Screen.height - (Screen.width / targetRatio)) / 2f;

            if (immediate)
            {
                _topBar.sizeDelta = new Vector2(_topBar.sizeDelta.x, barHeight);
                _bottomBar.sizeDelta = new Vector2(_bottomBar.sizeDelta.x, barHeight);
                _leftBar.sizeDelta = new Vector2(0, _leftBar.sizeDelta.y);
                _rightBar.sizeDelta = new Vector2(0, _rightBar.sizeDelta.y);
            }
            else
            {
                _currentSequence = DOTween.Sequence();
                _currentSequence.Join(_topBar.DOSizeDelta(new Vector2(_topBar.sizeDelta.x, barHeight), _transitionDuration).SetEase(_easeType));
                _currentSequence.Join(_bottomBar.DOSizeDelta(new Vector2(_bottomBar.sizeDelta.x, barHeight), _transitionDuration).SetEase(_easeType));
                _currentSequence.Join(_leftBar.DOSizeDelta(new Vector2(0, _leftBar.sizeDelta.y), _transitionDuration).SetEase(_easeType));
                _currentSequence.Join(_rightBar.DOSizeDelta(new Vector2(0, _rightBar.sizeDelta.y), _transitionDuration).SetEase(_easeType));
            }
        }
    }

    public void Set8To7Ratio()
    {
        SetAspectRatio(AspectRatio.Ratio_8_7);
    }

    public void Set21To9Ratio()
    {
        SetAspectRatio(AspectRatio.Ratio_21_9);
    }

    private void OnDestroy()
    {
        _currentSequence?.Kill();
        _renderTexture?.Release();
    }
}