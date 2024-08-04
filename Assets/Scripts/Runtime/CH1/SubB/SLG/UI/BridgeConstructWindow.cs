using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BridgeConstructWindow : MonoBehaviour
{
    [Header("DisplayComponents")]
    [SerializeField] private GameObject _contentParent = default;
    [SerializeField] private Button closeButton = default;
    [SerializeField] private TextMeshProUGUI woodText = default;
    [SerializeField] private TextMeshProUGUI stoneText = default;
    [SerializeField] private Button constructButton = default;

    SLGActionComponent _SLGAction;

    void Awake()
    {
        _SLGAction = FindAnyObjectByType<SLGActionComponent>().GetComponent<SLGActionComponent>();
        if (_SLGAction == null)
        {
            Debug.LogError("Can't Find SLGActionComponent");
        }
    }

    private void OnEnable()
    {
        RefreshWindowInfo();
        Managers.Data.InGameKeyBinder.PlayerInputDisable();
    }

    private void OnDisable()
    {
        Managers.Data.InGameKeyBinder.PlayerInputEnable();
    }

    void Start()
    {
        
        closeButton.onClick.AddListener(OnClickCloseButton);
        constructButton.onClick.AddListener(OnClickConstructButton);
    }


    private void RefreshWindowInfo()
    {
        int _wood = Managers.Data.SLGWoodCount;
        int _stone = Managers.Data.SLGStoneCount;

        Vector2 BridgeAssetCost = _SLGAction.BridgeNeededAssetCount;

        if (woodText != null)
        {
            woodText.text = _wood.ToString() + "/" + BridgeAssetCost.x;
            woodText.color = _wood < BridgeAssetCost.x ? Color.red : Color.black;
        }

        if (stoneText)
        {
            stoneText.text = _stone.ToString() + "/" + BridgeAssetCost.y;
            stoneText.color = _stone < BridgeAssetCost.y ? Color.red : Color.black;
        }
    }
    private void OnClickCloseButton()
    {
        if (_contentParent)
        {
            _contentParent.SetActive(false);
        }
    }
    void OnClickConstructButton()
    {
        if (_SLGAction != null)
        {
            if(_SLGAction.RebuildBridge())
            {
                if (_contentParent)
                {
                    _contentParent.SetActive(false);
                }
            }
        }
    }
}
