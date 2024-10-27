using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using SLGDefines;

public class SLGBuildingListWindow : MonoBehaviour
{
    [Header("DisplayComponents")]
    [SerializeField] private GameObject _contentParent = default;
    public Button _HUDButton = default;
    [SerializeField] List<BuildingItem> _buildingItems = new List<BuildingItem>();

    [Header("AnimationValue")]
    [SerializeField] private Vector2 _windowTopPos;
    [SerializeField] private Vector2 _windowDownPos;
    [SerializeField] private float AnimTime;

    private bool _windowVisible = false; //결과적으로 만들고 싶은 visible 상태

    private void OnEnable()
    {
        _HUDButton.onClick.AddListener(OnClickHUDButton);
    }

    private void OnDisable()
    {
        _HUDButton.onClick.RemoveListener(OnClickHUDButton);
    }

    void Start()
    {
        _windowVisible = false;
        RefreshBuildingInfo();
        ChangeWindowVisiblity(false, true);
    }
    public void HideWindow()
    {
        if (_windowVisible)
        {
            ChangeWindowVisiblity(false);
        }
    }

    private void OnClickHUDButton()
    {
        ChangeWindowVisiblity(!_windowVisible);
    }

    private void ChangeWindowVisiblity (bool bVisible, bool bDirect = false)
    {
        RectTransform transform = _contentParent.GetComponent<RectTransform>();
        if (transform == null)
        {
            Debug.LogWarning("BuildingList Window Transform is Null");
            return;
        }
        _windowVisible = bVisible;
        Vector2 StartPos = _windowVisible ? _windowDownPos : _windowTopPos;
        Vector2 EndPos = _windowVisible ? _windowTopPos : _windowDownPos;

        if(bDirect)
        {
            transform.anchoredPosition = EndPos;
        }
        else 
        {
            transform.anchoredPosition = StartPos;
            transform.DOLocalMove(EndPos, AnimTime);
        }
    }

    public void RefreshBuildingInfo()
    {
        foreach (BuildingItem Item in _buildingItems)
        {
            Item.RefreshItem();
        }
    }
}
