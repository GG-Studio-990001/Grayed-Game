using SLGDefines;
using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Runtime.CH1.Main.Controller;
using Runtime.ETC;
using Runtime.CH1.Main.Dialogue;
using DG.Tweening;
using Yarn.Unity;

namespace SLGDefines
{ 
    public enum SLGObjectType {
        //Asset
        WOOD = 0,
        STONE = 1,
        ASSETMAX = 2,
        //Window
        ConstructWindow,
        BridgeConstructWindow,
        MAX,
    }

    enum BuildingState
    {
        Able,
        Impossible,
        Locked,
        Constructed,
    }

    public enum SLGBuildingType
    {
        MamagoCompany = 0,
        Bridge =1,
        R2Mon =2,
        DollarStatue=3,
    }
}

public class SLGActionComponent : MonoBehaviour
{
    public List<Sprite> SLGPopupSprites;
    [SerializeField] private Texture2D cursorTexture;
    [SerializeField] private Sprite ArrowTexture;

    [SerializeField] private GameObject _SLGCanvas;
    [SerializeField] private GameObject _constructUI;
    [SerializeField] private GameObject _bridgeConstructUI;
    [SerializeField] private GameObject _buildingListUI;
    [SerializeField] private TextMeshProUGUI UI_WoodText;
    [SerializeField] private TextMeshProUGUI UI_StoneText;
    [SerializeField] private TextMeshProUGUI UI_CoinText;
    [SerializeField] private GameObject UI_WoodImg;
    [SerializeField] private GameObject UI_StoneImg;

    [SerializeField] private GameObject Wnd_CostSection;
    [SerializeField] private GameObject Wnd_AccelerateSection;

    [SerializeField] private TextMeshProUGUI Wnd_WoodText;
    [SerializeField] private TextMeshProUGUI Wnd_StoneText;
    [SerializeField] private TextMeshProUGUI Wnd_TimeText;
    [SerializeField] private TextMeshProUGUI Wnd_CoinCostText;
    [SerializeField] private UnityEngine.UI.Button Wnd_ConstructionBtn;
    [SerializeField] private UnityEngine.UI.Button Wnd_AccelerateBtn;
    [SerializeField] private UnityEngine.UI.Button Wnd_CloseBtn;

    [SerializeField] private GameObject _sponSpots;
    [SerializeField] private GameObject SLGConstructionObject;
    [SerializeField] private GameObject SLGBridgeConstructionObject;
    [SerializeField] private GameObject SLGConstructionSite;
    [SerializeField] private GameObject SLGMaMagoGate;
    [SerializeField] private GameObject SLGMaMagoGateColider;
    public GameObject[] SLGSubObjects;
    public GameObject SLGTriggerObject;

    [SerializeField] private GameObject _player;
    private GameObject _arrowObject;
    [SerializeField] Vector2[] _buildingPos;

    private SLGInteractionObject[] _cachedObjects;
    private int _spawnCount = 0;
    private SLGProgress SLGProgressInfo;
    private long SLGConstructionBeginTime;

    private CH1CommonData CH1Data;

    private int _wood = 0;
    private int _stone = 0;
    private float _spawnTime = 0.0f;
    private bool _rebuildBridge = false;

    //CONST Value 
    const int IncreaseAssetCount = 10;
    const int MaxSpawnCount = 3;
    const int NeededAssetCount = 30;
    const int NeededConstrcutionTimeSec = 60 * 60 * 24;
    const float SpawnTime = 3.0f;
    const int NeededCoinCount = 100;
    [SerializeField] public Vector2 BridgeNeededAssetCount;

    // 럭키 등장용
    [SerializeField] private DialogueRunner _luckyDialogue;
    // 마마고 상호작용용
    [SerializeField] private Ch1DialogueController _dialogue;

    private void Awake()
    {
        //SLGProgressInfo = SLGProgress.None;
    }

    void Start()
    {
        // Managers.Data.LoadGame();
        SLGProgressInfo = Managers.Data.SLGProgressData;
        SLGConstructionBeginTime = Managers.Data.SLGConstructionBeginTime;
        _rebuildBridge = Managers.Data.SLGBridgeRebuild;

        _cachedObjects = _sponSpots.GetComponentsInChildren<SLGInteractionObject>();
        Wnd_ConstructionBtn.onClick.AddListener(OnClickConstructBtn);
        Wnd_CloseBtn.onClick.AddListener(OnClickCloseBtn);
        Wnd_AccelerateBtn.onClick.AddListener(OnClickAccelerateBtn);
        _constructUI.SetActive(false);
        _bridgeConstructUI.SetActive(false);
        _buildingListUI.SetActive(false);

        SLGBridgeConstructionObject.SetActive(false);
        SLGConstructionSite.SetActive(true);
        SLGMaMagoGate.SetActive(false);
        SLGMaMagoGateColider.SetActive(false);
        _sponSpots.SetActive(false);
        SLGTriggerObject.SetActive(false);

        PreInitSubObject();

        GameObject mainObject = FindObjectOfType<Ch1MainSystemController>().gameObject;
        if (mainObject != null)
        {
            CH1Data = mainObject.GetComponent<CH1CommonData>();
        }

         if (SLGProgressInfo == SLGProgress.None)
        {
            //오브젝트 표시 타이밍 제어가 필요해보임. 조건이 너무 많음
            SLGTriggerObject.SetActive(Managers.Data.Scene >= 4);
            _SLGCanvas.SetActive(false);
        }
        else if(SLGProgressInfo >= SLGProgress.ModeClose)
        {
            EndSLGMode();
        }
        else
        {
            SLGResume();
            InitMap();
            _sponSpots.SetActive(true);

            if (SLGProgressInfo >= SLGProgress.EndConstruction)
            {
                EndMainConstruction();
            }
        }
    }

    private void Update()
    {
        if (SLGProgressInfo == SLGProgress.Constructing)
        {
            int DurationTimeSec = (int)(((DateTime.Now - DateTime.MinValue).TotalSeconds) - SLGConstructionBeginTime);
            int RemainedTimeSec = NeededConstrcutionTimeSec - DurationTimeSec;
            if (RemainedTimeSec >= 0)
            {
                if (_constructUI.activeSelf)
                {
                    int Hour = RemainedTimeSec / (60 * 60);
                    RemainedTimeSec -= 60 * 60 * Hour;

                    int Min = RemainedTimeSec / (60);
                    int Sec = RemainedTimeSec % (60);
                    Wnd_TimeText.text = Hour.ToString() + ":" + Min.ToString() + ":" + Sec.ToString();
                }
            }
            else
            {
                //연출이 나와야 할듯?
                EndMainConstruction();
            }
        }
        if(SLGProgressInfo >= SLGProgress.BeforeConstruction && SLGProgressInfo < SLGProgress.ModeClose)
        {
            if(_spawnCount < MaxSpawnCount)
            {
                _spawnTime += Time.deltaTime;
                if(_spawnTime >= SpawnTime)
                {
                    SpawnRandomObject();
                    _spawnTime = 0.0f;
                }
            }
        }
    }

    public bool IsInSLGMode()
    {
        return SLGProgressInfo > SLGProgress.None;
    }

    public bool CanInteract ()
    {
        return IsShowingWindow() == false && _luckyDialogue.IsDialogueRunning == false;
    }

    public bool IsShowingWindow()
    {
        return _constructUI.activeSelf || _bridgeConstructUI.activeSelf;
    }

    public Sprite GetInteractionSprite(SLGObjectType InObjectType)
    {
        if ((int)InObjectType >= SLGPopupSprites.Count)
        {
            return null;
        }

        return SLGPopupSprites[(int)InObjectType];
    }

    public void SLGPause()
    {
        _SLGCanvas.SetActive(false);
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        Managers.Data.SLGProgressData = SLGProgressInfo;
    }

    public void SLGResume()
    {
        _SLGCanvas.SetActive(true);
        SLGTriggerObject.SetActive(false);
        _sponSpots.SetActive(true);
        SLGConstructionObject.SetActive(true);

        if(Managers.Data.SLGBridgeRebuild == false)
        {
            SLGBridgeConstructionObject.SetActive(true);
        }
        _buildingListUI.SetActive(true);
        _wood = Managers.Data.SLGWoodCount;
        _stone = Managers.Data.SLGStoneCount;

        Cursor.SetCursor(cursorTexture, Vector2.zero, CursorMode.Auto);
        RefreshHudInfo();
    }
    public void ProcessObjectInteraction(SLGObjectType InObjectType)
    {
        if (_constructUI.activeSelf || _bridgeConstructUI.activeSelf)
        {
            return;
        }
        switch (InObjectType)
        {
            case SLGObjectType.WOOD:
                {
                    _wood += IncreaseAssetCount;
                    UI_WoodText.text = _wood.ToString();
                    _spawnCount--;
                    Managers.Sound.Play(Sound.SFX, "SLG/SLG_Wood_SFX");
                    PlayGainAnim(InObjectType);
                    WriteSLGData();
                    break;
                }
            case SLGObjectType.STONE:
                {
                    _stone += IncreaseAssetCount;
                    UI_StoneText.text = _stone.ToString();
                    Managers.Sound.Play(Sound.SFX, "SLG/SLG_Stone_SFX");
                    _spawnCount--;
                    WriteSLGData();
                    PlayGainAnim(InObjectType);
                    break;
                }
            case SLGObjectType.ConstructWindow:
                {
                    _constructUI.SetActive(true);
                    Managers.Data.InGameKeyBinder.PlayerInputDisable();
                    if (SLGProgressInfo == SLGProgress.Constructing)
                    {
                        RefreshAccelerateWnd();
                    }
                    else if(SLGProgressInfo == SLGProgress.BeforeConstruction)
                    {
                        RefreshConstructionWnd();
                    }
                    Managers.Sound.Play(Sound.SFX, "SLG/SLG_Click_SFX");
                    break;
                }
            case SLGObjectType.BridgeConstructWindow:
                {
                    _bridgeConstructUI.SetActive(true);
                    Managers.Sound.Play(Sound.SFX, "SLG/SLG_SFX_Click");
                }
                break;
            default: break;
        }
    }

    public void GetSLGPack()
    {
        Managers.Sound.Play(Sound.SFX, "SLG/SLG_Get_SFX");
        SLGTriggerObject.SetActive(false);
    }

    public void OnSLGInit()
    {
        MoveOnNextProgress();
        _SLGCanvas.SetActive(true);
        _buildingListUI.SetActive(true);
        _sponSpots.SetActive(true);

        Cursor.SetCursor(cursorTexture, Vector2.zero, CursorMode.Auto);

        if (SLGConstructionObject != null)
        {
            SLGConstructionObject.SetActive(true);
        }

        SLGBridgeConstructionObject.SetActive(!Managers.Data.SLGBridgeRebuild);

        _constructUI.SetActive(false);
        _bridgeConstructUI.SetActive(false);
        RefreshCoinText();

        InitMap();

        _luckyDialogue.StartDialogue("LuckySlg");
    }

    public void PreInitSubObject()
    {
        bool bEnable = Managers.Data.Scene >= 4;
        foreach (GameObject _subObject in SLGSubObjects)
        {
            _subObject.SetActive(bEnable);
        }
    }

    private void InitMap()
    {
        foreach (SLGInteractionObject Object in _cachedObjects)
        {
            Object.gameObject.SetActive(false);
        }
        for (int i = 0; i < Mathf.Min(MaxSpawnCount, _cachedObjects.Length); i++)
        {
            SpawnRandomObject();
        }
    }

    private void SpawnRandomObject()
    {
        if (SLGProgressInfo > SLGProgress.EndConstruction)
        {
            return;
        }

        while (true)
        {
            int RandomNum = (UnityEngine.Random.Range(0, _cachedObjects.Length));
            if (!_cachedObjects[RandomNum]._isActive)
            {
                SLGObjectType NewType = (SLGObjectType)(UnityEngine.Random.Range(0, (int)SLGObjectType.ASSETMAX));
                _cachedObjects[RandomNum].InitInteractionData(NewType);
                _cachedObjects[RandomNum].gameObject.SetActive(true);
                _spawnCount++;
                break;
            }
        }
    }


    public void RefreshHudInfo()
    {
        RefreshCoinText();
        UI_WoodText.text = _wood.ToString();
        UI_StoneText.text = _stone.ToString();

        SLGBuildingListWindow BuildingWnd = _buildingListUI.GetComponent<SLGBuildingListWindow>();
        if (BuildingWnd != null)
        {
            BuildingWnd.RefreshBuildingInfo();
        }
    }

    private void RefreshAccelerateWnd()
    {
        if (_constructUI != null && SLGProgressInfo == SLGProgress.Constructing)
        {
            Wnd_AccelerateSection.gameObject.SetActive(true);
            Wnd_CostSection.gameObject.SetActive(false);

            Wnd_CoinCostText.text = Managers.Data.PacmomCoin.ToString() + "/" + NeededCoinCount.ToString();
            Wnd_CoinCostText.color = Managers.Data.PacmomCoin < NeededCoinCount ? Color.red : Color.blue;
        }
    }
    private void RefreshConstructionWnd()
    {
        if (_constructUI != null && SLGProgressInfo == SLGProgress.BeforeConstruction)
        {
            Wnd_AccelerateSection.gameObject.SetActive(false);
            Wnd_CostSection.gameObject.SetActive(true);

            Wnd_WoodText.text = _wood.ToString() + "/" + NeededAssetCount.ToString();
            Wnd_StoneText.text = _stone.ToString() + "/" + NeededAssetCount.ToString();

            Wnd_WoodText.color = _wood < NeededAssetCount ? Color.red : Color.blue;
            Wnd_StoneText.color = _stone < NeededAssetCount ? Color.red : Color.blue;
        }
    }

    private void RefreshCoinText()
    {
        if (UI_CoinText != null)
        {
            UI_CoinText.text = Managers.Data.PacmomCoin.ToString();
        }
    }
    private void OnClickAccelerateBtn()
    {
        GameObject mainObject = FindObjectOfType<Ch1MainSystemController>().gameObject;
        if (mainObject != null)
        {
            if (Managers.Data.PacmomCoin >= NeededCoinCount)
            {
                Managers.Data.PacmomCoin -= NeededCoinCount;
                RefreshCoinText();
                _constructUI.SetActive(false);
                Managers.Data.InGameKeyBinder.PlayerInputEnable();

                _dialogue.MamagoThanks(); // 마마고 연출 시작
            }
            else
            {
                Debug.Log("건설 불가능");
            }
        }
    }
    private void OnClickCloseBtn()
    {
        if (_constructUI != null)
        {
            _constructUI.SetActive(false);
            Managers.Data.InGameKeyBinder.PlayerInputEnable();
        }
    }

    private void OnClickConstructBtn()
    {
        if (_wood >= NeededAssetCount && _stone >= NeededAssetCount)
        {
            _wood -= NeededAssetCount;
            _stone -= NeededAssetCount;
            RefreshHudInfo();
            MoveOnNextProgress();
        }
        else
        {
            Debug.Log("건설불가능");
        }
    }

    private void PlayGainAnim(SLGObjectType InType)
    {
        //만약 획득 애니메이션 통일해서 쓸거면 이런 애니메이션 관련은 공용함수로 만들어도될듯
        GameObject animTargetImg;
        if (InType == SLGObjectType.WOOD)
        {
            animTargetImg = UI_WoodImg;
        }
        else if(InType ==SLGObjectType.STONE)
        {
            animTargetImg = UI_StoneImg;
        }
        else
        {
            return;
        }

        if(animTargetImg != null)
        {
            RectTransform transform = animTargetImg.GetComponent<RectTransform>();
            if(transform != null)
            {
                transform.DOComplete();
                transform.localScale = new Vector3(0.25f,0.25f,1.0f);
                transform.DOScale(0.33f, 0.5f).SetEase(Ease.Linear).SetLoops(2, LoopType.Yoyo);
            }
        }
    }

    private void WriteSLGData()
    {
        Managers.Data.SLGProgressData = SLGProgressInfo;
        Managers.Data.SLGConstructionBeginTime = SLGConstructionBeginTime;
        Managers.Data.SLGWoodCount = _wood;
        Managers.Data.SLGStoneCount = _stone;
        Managers.Data.SLGBridgeRebuild = _rebuildBridge;

        Managers.Data.SaveGame();
    }

    public void MoveOnNextProgress()
    {
        SLGProgressInfo++;

        if (SLGProgressInfo <= SLGProgress.ModeClose)
        {
            if (_buildingListUI != null)
            {
                _buildingListUI.SetActive(true);
                SLGBuildingListWindow BuildingWnd = _buildingListUI.GetComponent<SLGBuildingListWindow>();
                if (BuildingWnd != null)
                {
                    BuildingWnd.RefreshBuildingInfo();
                }
            }
        }

        switch (SLGProgressInfo) 
        {
            case SLGProgress.None:
                break;
            case SLGProgress.BeforeConstruction:
                RefreshConstructionWnd();
                break;
            case SLGProgress.Constructing:
                SLGConstructionBeginTime = (long)((DateTime.Now - DateTime.MinValue).TotalSeconds);
                RefreshAccelerateWnd();
                break;
            case SLGProgress.EndConstruction:
                EndMainConstruction();
                break;
            case SLGProgress.ModeClose:
                EndSLGMode();
                break;  
        }

        // SLG팩 획득 ~ 럭키 설명 다 듣기 안하면 SLG 팩 획득 이전으로 돌아가야함
        if (SLGProgressInfo > SLGProgress.ModeOpen)
        {
            WriteSLGData();
        }
    }

    public void EndMainConstruction()
    {
        //함수를 공용으로 쓰고 있어서 기존의 함수 유지
        SLGConstructionObject.SetActive(false);
        SLGConstructionSite.SetActive(false);

        SLGMaMagoGate.SetActive(true);
        SLGMaMagoGateColider.SetActive(true);
    }

    public void SetBuildCutSceneObjects()
    {
        SLGConstructionObject.SetActive(false);
        SLGConstructionSite.SetActive(false);

        SLGMaMagoGate.SetActive(true);
        SLGMaMagoGateColider.SetActive(false);
    }

    private void EndSLGMode()
    {
        SLGConstructionObject.SetActive(false);
        SLGConstructionSite.SetActive(false);
        SLGBridgeConstructionObject.SetActive(false);

        _sponSpots.SetActive(false);
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);

        MoveOnNextProgress();
    }

    public bool RebuildBridge()
    {
        if (_wood >= (int)BridgeNeededAssetCount.x && _stone >= (int)BridgeNeededAssetCount.y)
        {
            Ch1DialogueController Ch1DC = GameObject.FindObjectOfType<Ch1DialogueController>();
            if (Ch1DC != null)
            {
                Ch1DC.StartCh1MainDialogue("RebuildBridge");

                _rebuildBridge = true;
                _wood -= (int)BridgeNeededAssetCount.x;
                _stone -= (int)BridgeNeededAssetCount.y;
                WriteSLGData();

                SLGBridgeConstructionObject.SetActive(false);

                RefreshHudInfo();
                return true;
            }
        }
        else
        {
            Debug.Log("다리 건설 불가");
            return false;
        }
        return false;
    }
    public void ShowArrowObject(SLGBuildingType type)
    {
        if(_buildingPos.Length < (int)type)
        {
            return;
        }
        if (_buildingListUI)
        {
            SLGBuildingListWindow BuildingWnd = _buildingListUI.GetComponent<SLGBuildingListWindow>();
            if (BuildingWnd != null)
            {
                BuildingWnd.HideWindow();
            }
        }

        CreateArrowObject();
        if(_arrowObject)
        {
            SLGArrowObject ArrowAction = _arrowObject.GetComponent<SLGArrowObject>();

            if(ArrowAction != null)
            {
                ArrowAction.SetTargetPos(_buildingPos[(int)type]);
            }
        }
    }

    private void CreateArrowObject()
    {
        if(_arrowObject)
        {
            Destroy(_arrowObject);
        }

        _arrowObject = new GameObject();
        SpriteRenderer Renderer = _arrowObject.AddComponent<SpriteRenderer>();

        if (ArrowTexture)
        {
            Renderer.sprite = ArrowTexture;
        }

        Renderer.sortingLayerName = "UI";
        SLGArrowObject ArrowAction = _arrowObject.AddComponent<SLGArrowObject>();
        if (_player)
        {
            _arrowObject.transform.parent = _player.transform;
            _arrowObject.transform.localPosition = new Vector3(0, 1, 0);
        }
    }
}
