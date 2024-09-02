using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using SLGDefines;

public class BuildingItem : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
    [Header("ItemValue")]
    [SerializeField] private SLGBuildingType _buildingtype;
    public Vector2 _reqAsset;
    [TextArea]
    [SerializeField] private string _discrtiption;

    [Header("DisplayComponents")]
    [SerializeField] private Image _dispArea = default;
    [SerializeField] private GameObject _dimImage = default;
    [SerializeField] private TextMeshProUGUI _discrtiptionText = default;
    [SerializeField] private Image _buildingStateImg = default;

    private bool _dispVisible = false;
    BuildingState _buildingState = BuildingState.Able;

    private void Start()
    {
        _discrtiptionText.text = _discrtiption;
    }

    private void OnEnable()
    {
        _dispVisible = false;
        OnDispFadeInOut(false);
        RefreshItem();
    }
    private void OnDisable()
    {
        _dispVisible = false;
        OnDispFadeInOut(false);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (_dispVisible)
        {
            return;
        }

        OnDispFadeInOut(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (_dispVisible == false)
        {
            return;
        }

        OnDispFadeInOut(false);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        SLGActionComponent SLGAction = FindAnyObjectByType<SLGActionComponent>();
        if(SLGAction != null)
        {
            if(_buildingState != BuildingState.Constructed && _buildingState != BuildingState.Locked)
            {
                SLGAction.ShowArrowObject(_buildingtype);
            }
        }
    }

    void OnDispFadeInOut(bool bVisible)
    {
        if(_dispArea == null)
        {
            return;
        }

        _dispArea.gameObject.SetActive(bVisible);
        _dispVisible = bVisible;
    }
    
    public void RefreshItem ()
    {
        RefreshBuildingState();

        bool bInActive = _buildingState == BuildingState.Constructed || _buildingState == BuildingState.Locked;
        if (_dimImage != null)
        {
            _dimImage.gameObject.SetActive(bInActive);
        }
        if(_buildingStateImg != null)
        {
            _buildingStateImg.gameObject.SetActive(bInActive);
        }
    }
    private void RefreshBuildingState()
    {
        switch (_buildingtype)
        {
            case SLGBuildingType.MamagoCompany:
                if (Managers.Data.SLGProgressData >= Runtime.ETC.SLGProgress.EndConstruction)
                {
                    _buildingState = BuildingState.Constructed;
                    return;
                }
                break;
            case SLGBuildingType.Bridge:
                if (Managers.Data.SLGBridgeRebuild)
                {
                    _buildingState = BuildingState.Constructed;
                    return;
                }
                break;
            case SLGBuildingType.R2Mon:
                _buildingState = BuildingState.Locked;
                return;
            case SLGBuildingType.DollarStatue:
                _buildingState = BuildingState.Impossible;
                return;
        }
        if (_reqAsset.x <= Managers.Data.SLGWoodCount && _reqAsset.y <= Managers.Data.SLGStoneCount)
        {
            _buildingState = BuildingState.Able;
        }
        else
        {
            _buildingState = BuildingState.Impossible;
        }

    }
}
