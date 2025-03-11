using TMPro;
using UnityEngine;
using UnityEngine.UI;
using SLGDefines;
using System;

public class BuildingDetailViewWindow : MonoBehaviour
{
    [Header("DisplayComponents")]
    [SerializeField] private GameObject _contentParent = default;
    [SerializeField] private Button closeButton = default;

    //Common
    [SerializeField] private GameObject _buildingImg = default;
    [SerializeField] private GameObject _section;
    [SerializeField] private TextMeshProUGUI _titleText = default;

    //Asset
    [SerializeField] private TextMeshProUGUI woodText = default;
    [SerializeField] private TextMeshProUGUI stoneText = default;
    [SerializeField] private Button constructButton = default;

    //Acceleate
    [SerializeField] private Button accelerateBtn = default;
    [SerializeField] private TextMeshProUGUI coinCostText = default;
    [SerializeField] private TextMeshProUGUI TimeText = default;

    SLGBuildingType _currentBuildingType;
    SLGBuildingObject _currentDisplayBuilding;
    SLGActionComponent _SLGAction;
    bool _bTimeSpending = false;

    void Update()
    {
        if (_bTimeSpending)
        {
            RefreshAccelerateSection();
        }
    }

    public void SetCurrentBuilding (SLGBuildingObject InBuilding)
    {
        if(InBuilding == null)
        {
            return;
        }
        _currentBuildingType = InBuilding.GetBuildingData().GetBuildingType();
        _currentDisplayBuilding = InBuilding;
        RefreshWindowInfo();
    }
    void Start()
    {
        closeButton.onClick.AddListener(OnClickCloseButton);
        constructButton.onClick.AddListener(OnClickConstructButton);
        accelerateBtn.onClick.AddListener(OnClickAccelerateButton);
    }

    private void RefreshWindowInfo()
    {
        _SLGAction = FindAnyObjectByType<SLGActionComponent>().GetComponent<SLGActionComponent>();
        if (_SLGAction == null)
        {
            Debug.LogError("Can't Find SLGActionComponent");
            return;
        }

        if (_currentDisplayBuilding == null)
        {
            return;
        }

        if (_buildingImg != null)
        {
            Image _imgComp = _buildingImg.GetComponent<Image>();
            if (_imgComp != null)
            {
                _imgComp.sprite = _currentDisplayBuilding.GetBuildingData().GetDetailViewImage();
            }
        }

        if(_titleText)
        {
            _titleText.text = _currentDisplayBuilding.GetBuildingData().GetBuildingName();
        }

        if (_section != null)
        {
            SubObjectSwitcher _Switcher = _section.GetComponent<SubObjectSwitcher>();
            if (_Switcher != null)
            {
                if (_currentDisplayBuilding.GetProgress() == SLGBuildingProgress.Constructing)
                {
                    _Switcher.SetActiveSubObject(1);
                    _bTimeSpending = true;
                    RefreshAccelerateSection();
                }
                else 
                //if (_currentDisplayBuilding.GetProgress() == SLGBuildingProgress.BeforeConstruct)
                {
                    _Switcher.SetActiveSubObject(0);
                    RefreshAssetSection();
                }
            }
        }
    }

    void RefreshAssetSection()
    {
        if (_SLGAction == null)
        {
            return;
        }

        int _wood = Managers.Data.Common.Wood;
        int _stone = Managers.Data.Common.Stone;

        Vector2 _targetAssetCost = _currentDisplayBuilding.GetBuildingData().GetReqAsset();
        if (woodText != null)
        {
            woodText.text = _wood.ToString() + "/" + _targetAssetCost.x;
            woodText.color = _wood < _targetAssetCost.x ? Color.red : Color.blue;
        }

        if (stoneText)
        {
            stoneText.text = _stone.ToString() + "/" + _targetAssetCost.y;
            stoneText.color = _stone < _targetAssetCost.y ? Color.red : Color.blue;
        }
    }

    void RefreshAccelerateSection()
    {
        int DurationTimeSec = (int)(((DateTime.Now - DateTime.MinValue).TotalSeconds) - _currentDisplayBuilding.GetConstructBeginTime());
        int RemainedTimeSec = _currentDisplayBuilding.GetBuildingData().NeededConstrcutionTimeSec - DurationTimeSec;
        if (RemainedTimeSec >= 0)
        {
            int Hour = RemainedTimeSec / (60 * 60);
            RemainedTimeSec -= 60 * 60 * Hour;

            int Min = RemainedTimeSec / (60);
            int Sec = RemainedTimeSec % (60);
            TimeText.text = Hour.ToString() + ":" + Min.ToString() + ":" + Sec.ToString();
        }
        else
        {
            _bTimeSpending = false;
            CloseWindow();
        }

        coinCostText.text = Managers.Data.Common.Coin.ToString() + "/" + _currentDisplayBuilding.GetBuildingData().GetReqCoin().ToString();
        coinCostText.color = Managers.Data.Common.Coin < _currentDisplayBuilding.GetBuildingData().GetReqCoin() ? Color.red : Color.blue;
    }

    private void OnClickCloseButton()
    {
        CloseWindow();
    }
    void OnClickConstructButton()
    {
        if (_SLGAction != null)
        {
            if(_SLGAction.CanConstructBuilding(_currentBuildingType))
            {
                CloseWindow();
                _SLGAction.MoveOnNextBuildingState(_currentBuildingType, SLGBuildingProgress.Constructing);
            }
        }
    }

    void OnClickAccelerateButton()
    {
        if (_SLGAction != null)
        {
            if (_SLGAction.CanAccelerateBuilding(_currentBuildingType))
            {
                CloseWindow();
                _SLGAction.PlayCutScene(_currentBuildingType);
            }
        }
    }

    public void OpenWindow()
    {
        Managers.Data.InGameKeyBinder.PlayerInputDisable();
    }

    void CloseWindow()
    {
        if (_contentParent)
        {
            _contentParent.SetActive(false);
            Managers.Data.InGameKeyBinder.PlayerInputEnable();
        }
    }
}
