using SLGDefines;
using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Runtime.CH1.Main.Controller;
using Runtime.ETC;
using UnityEngine.UI;
using Runtime.CH1.Main.Dialogue;
using DG.Tweening;
using Yarn.Unity;
using System.Linq;
using Runtime.CH1.Main.Stage;

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
        MamagoTranslateWindow,
        MAX,
    }

    public enum BuildingState
    {
        Able,
        Impossible,
        Locked,
        Constructed,
    }

    public enum SLGBuildingProgress
    {
        None = 0, //Not Visible
        BeforeConstruct,
        Constructing, //Can Acceleate
        PlayCutScene,
        EndConstruct
    }

    public enum SLGBuildingType
    {
        MamagoCompany = 0,
        Bridge =1,
        R2Mon =2,
        DollarStatue=3,
        Max
    }

    public enum SLGProgress
    {
        None = 0,
        AfterCutscene,
        ModeOpen, //SLG 처음 모드 오픈 (튜토리얼 진행)
        Proceeding,
        ModeClose, //SLG 모드 끝남
        Max
    }
}

public class SLGActionComponent : MonoBehaviour
{
    [Header("SLGSprites")]
    public List<Sprite> SLGPopupSprites;
    [SerializeField] private Texture2D cursorTexture;
    [SerializeField] private Sprite ArrowTexture;

    [Header("UICanvas")]
    [SerializeField] private GameObject _SLGCanvas;
    [SerializeField] private GameObject _buildingConstructUI;
    [SerializeField] private GameObject _MamagoTranslateUI;
    [SerializeField] private GameObject _buildingListUI;
    private Button _buildingButton;

    //HUD
    [SerializeField] private TextMeshProUGUI UI_WoodText;
    [SerializeField] private TextMeshProUGUI UI_StoneText;
    [SerializeField] private TextMeshProUGUI UI_CoinText;
    [SerializeField] private GameObject UI_WoodImg;
    [SerializeField] private GameObject UI_StoneImg;

    [Header("SLGRelatedObjects")]
    [SerializeField] private SLGBuilding[] _SLGBuildingData; //Setting Data In Editor
    private SLGBuildingObject[] _SLGBuildingObjects;

    [SerializeField] private GameObject _sponSpots;
    [SerializeField] private GameObject SLGConstructionInteractionObject;
    [SerializeField] private GameObject SLGBridgeConstructionInteractionObject;
    [SerializeField] private GameObject SLGMaMagoGateColider;
    public GameObject[] SLGSubObjects;
    public GameObject SLGTriggerObject;
    [SerializeField] private GameObject _player;
    private SLGInteractionObject[] _cachedObjects;

    // 럭키 등장용
    [SerializeField] private DialogueRunner _luckyDialogue;
    // 마마고 상호작용용
    [SerializeField] private Ch1DialogueController _dialogue;

    [Header("SLGData")]
    private GameObject _arrowObject;

    //read from SaveData
    private int _spawnCount = 0;
    private SLGProgress _SLGProgressInfo;
    private int _wood = 0;
    private int _stone = 0;

    private CH1CommonData CH1Data;
    private float _spawnTime = 0.0f;

    [Header("ForTutorial")]
    private bool _waitAssetInput = false;
    private bool _waitWindowInput = false;
    [SerializeField] private GameObject _luckyBlocker;

    [Header("SLGConstData")]
    //CONST Value 
    const int IncreaseAssetCount = 10;
    const int MaxSpawnCount = 3;
    const float SpawnTime = 3.0f;

    void Start()
    {
        InitSLGData();
        InitBuildingData();
        _buildingConstructUI.SetActive(false);
        SLGMaMagoGateColider.SetActive(false);

        _cachedObjects = _sponSpots.GetComponentsInChildren<SLGInteractionObject>();

        RefreshAllFieldObject();
        RefreshAllUI();

        if(IsInSLGMode())
        {
            InitMap();
        }

        GameObject mainObject = FindObjectOfType<Ch1MainSystemController>().gameObject;
        if (mainObject != null)
        {
            CH1Data = mainObject.GetComponent<CH1CommonData>();
        }
    }

    private void Update()
    {
        foreach (SLGBuildingObject _building in _SLGBuildingObjects)
        {
            if (_building.GetProgress() == SLGBuildingProgress.Constructing)
            {
                int DurationTimeSec = (int)(((DateTime.Now - DateTime.MinValue).TotalSeconds) - _building.GetConstructBeginTime());
                int RemainedTimeSec = _building.GetBuildingData().NeededConstrcutionTimeSec - DurationTimeSec;
                if (RemainedTimeSec < 1)
                {
                    //연출이 나와야 할듯?
                    MoveOnNextBuildingState(_building.GetBuildingData().GetBuildingType(), SLGBuildingProgress.PlayCutScene);
                }
            }
        }
        if (_SLGProgressInfo >= SLGProgress.ModeOpen && _SLGProgressInfo < SLGProgress.ModeClose)
        {
            if (_spawnCount < MaxSpawnCount)
            {
                _spawnTime += Time.deltaTime;
                if (_spawnTime >= SpawnTime)
                {
                    SpawnRandomObject();
                    _spawnTime = 0.0f;
                }
            }
        }
    }

    public void OnSLGInit()
    {
        MoveOnNextProgress();
        _SLGCanvas.SetActive(true);
        _buildingListUI.SetActive(true);

        if (SLGConstructionInteractionObject != null)
        {
            SLGConstructionInteractionObject.SetActive(true);
        }

        RefreshAllFieldObject();
        InitMap();

        foreach (SLGBuildingObject _building in _SLGBuildingObjects)
        {
            if (_building.GetBuildingData().GetBuildingType() <= SLGBuildingType.Bridge)
            {
                _building.SetProgress(SLGBuildingProgress.BeforeConstruct);
            }
        }

        _luckyDialogue.StartDialogue("LuckySlg");
    }

    public void MoveOnNextProgress()
    {
        _SLGProgressInfo++;

        switch (_SLGProgressInfo)
        {
            case SLGProgress.None:
                break;
            //TODO Flow 
            case SLGProgress.ModeClose:
                EndSLGMode();
                RefreshAllUI();
                break;
        }

        // SLG팩 획득 ~ 럭키 설명 다 듣기 안하면 SLG 팩 획득 이전으로 돌아가야함
        if (_SLGProgressInfo != SLGProgress.ModeOpen)
        {
            WriteSLGData();
        }
    }

    #region SceneObjectHandle
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

    public void MoveOnNextBuildingState(SLGBuildingType InType, SLGBuildingProgress InProgress)
    {
        if (_SLGBuildingObjects.Length < (int)InType)
        {
            return;
        }

        SLGBuildingObject _targetBuilding = _SLGBuildingObjects[(int)InType];
        if(_targetBuilding == null)
        {
            return;
        }
        if (_targetBuilding.GetProgress() == InProgress)
        {
            return;
        }

        _targetBuilding.ChangeBuildingState(InProgress);

        switch (InProgress)
        {
            case SLGBuildingProgress.BeforeConstruct:
                break;
            case SLGBuildingProgress.Constructing:
                _wood -= (int)_targetBuilding.GetBuildingData().GetReqAsset().x;
                _stone -= (int)_targetBuilding.GetBuildingData().GetReqAsset().y;
                break;
            case SLGBuildingProgress.PlayCutScene:
                if (Managers.Data.CH1.PacmomCoin >= _targetBuilding.GetBuildingData().GetReqCoin())
                {
                    Managers.Data.CH1.PacmomCoin -= _targetBuilding.GetBuildingData().GetReqCoin();
                }
                PlayCutScene(InType);
                break;
            case SLGBuildingProgress.EndConstruct:
                EndConstruction(InType);
                break;
            default: break;
        }

        RefreshBuildingObjects();
        RefreshAllUI();
        WriteSLGData();
    }
    void RefreshObjectWhenMovingStage()
    {
        RefreshAllFieldObject();
    }

    void RefreshAllUI ()
    {
        _SLGCanvas.SetActive(IsInSLGMode());
        _buildingListUI.SetActive(IsInSLGMode());
        Cursor.SetCursor(IsInSLGMode()? cursorTexture : null, Vector2.zero, CursorMode.Auto);
        RefreshHudInfo();
    }

    void RefreshAllFieldObject()
    {
        //SubObject
        bool _subObjectVisible = _SLGProgressInfo >= SLGProgress.AfterCutscene;
        foreach (GameObject _subObject in SLGSubObjects)
        {
            _subObject.SetActive(_subObjectVisible);
        }

        SLGTriggerObject.SetActive(_SLGProgressInfo == SLGProgress.AfterCutscene);
        _sponSpots.SetActive(IsInSLGMode());

        RefreshBuildingObjects();

        //Construct Object
        
    }

    public void RefreshBuildingObjects()
    {
        //공용 함수로 정리할 수 없을까..
        //Mamago Object
        if (_SLGBuildingObjects.Length > (int)SLGBuildingType.MamagoCompany)
        {
            SLGBuildingObject _mamagoBuilding = _SLGBuildingObjects[(int)SLGBuildingType.MamagoCompany];
            if (_mamagoBuilding != null)
            {
                bool _mamagoConstructed = _mamagoBuilding.GetProgress() >= SLGBuildingProgress.EndConstruct;
                SLGConstructionInteractionObject.SetActive(IsInSLGMode() && _mamagoConstructed == false);
                SLGMaMagoGateColider.SetActive(_mamagoConstructed);

                if (_mamagoBuilding.GetBuildingData().GetFieldObject() != null)
                {
                    SubObjectSwitcher _subObjectSwitcher = _mamagoBuilding.GetBuildingData().GetFieldObject().GetComponent<SubObjectSwitcher>();
                    if (_subObjectSwitcher != null)
                    {
                        switch (_mamagoBuilding.GetProgress())
                        {
                            case SLGBuildingProgress.None:
                            case SLGBuildingProgress.BeforeConstruct:
                                _subObjectSwitcher.SetActiveSubObject(0);
                                break;
                            case SLGBuildingProgress.Constructing:
                                _subObjectSwitcher.SetActiveSubObject(1);
                                break;
                            case SLGBuildingProgress.PlayCutScene:
                            case SLGBuildingProgress.EndConstruct:
                                _subObjectSwitcher.SetActiveSubObject(2);
                                break;
                        }
                    }
                }
            }
        }

        //Bridge Object
        if (_SLGBuildingObjects.Length > (int)SLGBuildingType.Bridge)
        {
            SLGBuildingObject _bridgeBuilding = _SLGBuildingObjects[(int)SLGBuildingType.Bridge];
            if (_bridgeBuilding != null)
            {
                bool _bridgeConstructed = _bridgeBuilding.GetProgress() >= SLGBuildingProgress.EndConstruct;
                SLGBridgeConstructionInteractionObject.SetActive(IsInSLGMode() && _bridgeConstructed == false);

                if (_bridgeBuilding.GetBuildingData().GetFieldObject() != null)
                {
                    SubObjectSwitcher _subObjectSwitcher = _bridgeBuilding.GetBuildingData().GetFieldObject().GetComponent<SubObjectSwitcher>();
                    if (_subObjectSwitcher != null)
                    {
                        switch (_bridgeBuilding.GetProgress())
                        {
                            case SLGBuildingProgress.None:
                                _subObjectSwitcher.SetActiveSubObject(Managers.Data.CH1.Scene >= 3 ? 1 : 0);
                                break;
                            case SLGBuildingProgress.BeforeConstruct:
                            case SLGBuildingProgress.Constructing:
                            case SLGBuildingProgress.PlayCutScene:
                                _subObjectSwitcher.SetActiveSubObject(1);
                                break;
                            case SLGBuildingProgress.EndConstruct:
                                _subObjectSwitcher.SetActiveSubObject(0);
                                break;

                        }
                    }
                }
            }
        }
    }
    public void PlayCutScene (SLGBuildingType InType)
    {
        //건설 끝난 다음의 개별 처리 담당
        Ch1DialogueController Ch1DC = GameObject.FindObjectOfType<Ch1DialogueController>();
        if (Ch1DC != null)
        {
            if (InType == SLGBuildingType.MamagoCompany)
            {
                _dialogue.MamagoThanks(); // 마마고 연출 시작
            }
            else if (InType == SLGBuildingType.Bridge)
            {
                Ch1DC.StartCh1MainDialogue("RebuildBridge");
            }
        }
    }

    void EndConstruction(SLGBuildingType InType)
    {
    }

    private void EndSLGMode()
    {
        StageMover _stageMover = FindAnyObjectByType<StageMover>().GetComponent<StageMover>();
        if (_stageMover != null)
        {
            _stageMover.OnStageMove.RemoveListener(RefreshObjectWhenMovingStage);
        }
    }

    private void SpawnRandomObject()
    {
        if (_SLGProgressInfo <SLGProgress.ModeOpen || _SLGProgressInfo > SLGProgress.ModeClose)
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
    #endregion

    #region DataHandle
    private void InitSLGData()
    {
        _SLGProgressInfo = Managers.Data.CH1.SLGProgressData;
        _wood = Managers.Data.CH1.SLGWoodCount;
        _stone = Managers.Data.CH1.SLGStoneCount;
    }
    private void WriteSLGData()
    {
        Managers.Data.CH1.SLGProgressData = _SLGProgressInfo;
        Managers.Data.CH1.SLGWoodCount = _wood;
        Managers.Data.CH1.SLGStoneCount = _stone;

        foreach (SLGBuildingObject _building in _SLGBuildingObjects)
        {
            _building.SaveBuildingData();
        }

        Managers.Data.SaveGame();
    }
    #endregion

    #region BuildingHandle
    void InitBuildingData()
    {
        SLGBuildingProgress[] _progressInfo = Managers.Data.CH1.SLGBuildingProgressData;
        long[] _constructTime = Managers.Data.CH1.SLGConstructionBeginTime;

        _SLGBuildingObjects = new SLGBuildingObject[_SLGBuildingData.Length];
        foreach (SLGBuilding _buildingData in _SLGBuildingData)
        {
            SLGBuildingType _buildingType = _buildingData.GetBuildingType();
            _SLGBuildingObjects[(int)_buildingType] = new SLGBuildingObject();
            SLGBuildingObject _building = _SLGBuildingObjects[(int)_buildingType];
            if (_SLGBuildingData.Length > (int)_buildingType)
            {
                _building.SetBuildingData(_SLGBuildingData[(int)_buildingType]);
            }
            if (_progressInfo.Length > (int)_buildingType)
            {
                _building.SetProgress(_progressInfo[(int)_buildingType]);
            }
            if (_constructTime.Length > (int)_buildingType)
            {
                _building.SetConstructBeginTime(_constructTime[(int)_buildingType]);
            }
            _SLGBuildingObjects.Append(_building);
        }
    }
    public SLGBuildingObject GetBuildingObject(SLGBuildingType InType)
    {
        if (_SLGBuildingObjects.Length < (int)InType)
        {
            Debug.LogError("[SLGActionComponent] No Building Objects");
            return null;
        }
        return _SLGBuildingObjects[(int)InType];
    }
    public bool CanConstructBuilding(SLGBuildingType InType)
    {
        if (_SLGBuildingData.Length < (int)InType)
        {
            Debug.LogError("Invalid Building");
            return false;
        }

        Vector2 NeededAsset = _SLGBuildingData[(int)InType].GetReqAsset();
        return _wood >= (int)NeededAsset.x && _stone >= (int)NeededAsset.y;
    }

    public bool CanAccelerateBuilding(SLGBuildingType InType)
    {
        if (_SLGBuildingObjects.Length < (int)InType)
        {
            Debug.LogError("Invalid Building");
            return false;
        }

        int _currentCount = Managers.Data.CH1.PacmomCoin;
        SLGBuildingObject _building = _SLGBuildingObjects[(int)InType];
        if(_building == null)
        {
            return false;
        }
        return _currentCount >= _building.GetBuildingData(). GetReqCoin();
    }

    public SLGBuildingProgress GetCurrentBuildingState(SLGBuildingType InType)
    {
        if(_SLGBuildingObjects == null)
        {
            return SLGBuildingProgress.None;
        }
        if (_SLGBuildingObjects.Length < (int)InType)
        {
            Debug.LogWarning("Not Intended to Construct Building");
            return SLGBuildingProgress.None;
        }
        return _SLGBuildingObjects[(int)InType].GetProgress();
    }

    public long GetBuildingConstructBeginTime(SLGBuildingType InType)
    {
        if (_SLGBuildingObjects.Length < (int)InType)
        {
            Debug.LogWarning("Not Intended to Construct Building");
            return -1;
        }
        if (GetCurrentBuildingState(InType) != SLGBuildingProgress.Constructing)
        {
            Debug.LogWarning("Not Started to Construct Building");
            return -1;
        }

        return _SLGBuildingObjects[(int)InType].GetConstructBeginTime();
    }
    #endregion

    #region UIHandle
    public bool IsInSLGMode()
    {
        return _SLGProgressInfo >= SLGProgress.ModeOpen && _SLGProgressInfo < SLGProgress.ModeClose;
    }

    public bool CanInteract ()
    {
        return IsShowingWindow() == false && _luckyDialogue.IsDialogueRunning == false && _waitAssetInput == false && _waitWindowInput == false;
    }

    public bool IsInInputTutorial()
    {
        return _waitAssetInput;
    }

    public bool IsShowingWindow()
    {
        return _buildingConstructUI.activeSelf;
    }

    public void RefreshBuildingListWnd()
    {
        if (_SLGProgressInfo <= SLGProgress.ModeClose)
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

    public void RefreshCoinText()
    {
        if (UI_CoinText != null)
        {
            UI_CoinText.text = Managers.Data.CH1.PacmomCoin.ToString();
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
        else if (InType == SLGObjectType.STONE)
        {
            animTargetImg = UI_StoneImg;
        }
        else
        {
            return;
        }

        if (animTargetImg != null)
        {
            RectTransform transform = animTargetImg.GetComponent<RectTransform>();
            if (transform != null)
            {
                transform.DOComplete();
                transform.localScale = new Vector3(0.25f, 0.25f, 1.0f);
                transform.DOScale(0.33f, 0.5f).SetEase(Ease.Linear).SetLoops(2, LoopType.Yoyo);
            }
        }
    }
    #endregion

    #region InteractionObjectHandle
    public Sprite GetInteractionSprite(SLGObjectType InObjectType)
    {
        if ((int)InObjectType >= SLGPopupSprites.Count)
        {
            return null;
        }

        return SLGPopupSprites[(int)InObjectType];
    }
    public void ProcessObjectInteraction(SLGObjectType InObjectType)
    {
        if (_buildingConstructUI.activeSelf)
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
                    _buildingConstructUI.SetActive(true);
                    BuildingDetailViewWindow _buildingDetail = _buildingConstructUI.GetComponent<BuildingDetailViewWindow>();
                    if (_buildingDetail != null)
                    {
                        _buildingDetail.OpenWindow();
                        _buildingDetail.SetCurrentBuilding(GetBuildingObject(SLGBuildingType.MamagoCompany));
                    }
                    Managers.Sound.Play(Sound.SFX, "SLG/SLG_Click_SFX");
                    break;
                }
            case SLGObjectType.BridgeConstructWindow:
                {
                    _buildingConstructUI.SetActive(true);
                    BuildingDetailViewWindow _buildingDetail = _buildingConstructUI.GetComponent<BuildingDetailViewWindow>();
                    if (_buildingDetail != null)
                    {
                        _buildingDetail.OpenWindow();
                        _buildingDetail.SetCurrentBuilding(GetBuildingObject(SLGBuildingType.Bridge));
                    }
                    Managers.Sound.Play(Sound.SFX, "SLG/SLG_Click_SFX");
                }
                break;
            case SLGObjectType.MamagoTranslateWindow:
                {
                    _MamagoTranslateUI.SetActive(true);
                    Managers.Sound.Play(Sound.SFX, "SLG/SLG_Click_SFX");
                }
                break;
            default: break;
        }
        if(IsInInputTutorial())
        {
            AfterAssetInput();
        }
    }
    #endregion

    #region CutScene_Related

    public void WaitAssetInput()
    {
        Cursor.SetCursor(cursorTexture, Vector2.zero, CursorMode.Auto);
        _waitAssetInput = true;
    }

    public void AfterAssetInput()
    {
        _waitAssetInput = false;
        _luckyDialogue.Stop();
        _luckyDialogue.StartDialogue("LuckySlgAfterAssetInput");
    }

    public void WaitWindowInput()
    {
        SLGBuildingListWindow _buildingListWindow = _buildingListUI.GetComponent<SLGBuildingListWindow>();
        if (_buildingListWindow!= null && _buildingListWindow._HUDButton != null)
        {
            _waitWindowInput = true;
            _buildingButton = _buildingListWindow._HUDButton;
            _buildingButton.onClick.AddListener(AfterWindowInput);
            if(_luckyBlocker)
            {
                _luckyBlocker.SetActive(false);
            }
        }
        else
        {
            Debug.LogError("Can't Find Button ! Can't Progress Tutorial.");
        }
    }

    public void AfterWindowInput()
    {
        if (_buildingButton != null)
        {
            _buildingButton.onClick.RemoveListener(AfterWindowInput);
            if (_luckyBlocker)
            {
                _luckyBlocker.SetActive(true);
            }
        }
        _waitWindowInput = false;
        _luckyDialogue.Stop();
        _luckyDialogue.StartDialogue("LuckySlgAfterWindowInput");
    }

    public void GetSLGPack()
    {
        Managers.Sound.Play(Sound.SFX, "SLG/SLG_Get_SFX");
        SLGTriggerObject.SetActive(false);
    }
    public void ShowArrowObject(SLGBuildingType type)
    {
        if(_SLGBuildingObjects.Length < (int)type)
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
                SLGBuildingObject _buildingObj = _SLGBuildingObjects[(int)type];
                if (_buildingObj != null)
                {
                    ArrowAction.SetTargetPos(_buildingObj.GetBuildingData().GetBuildingPos());
                }
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
        ObjectFadeInOutComponent FadeInOutAction = _arrowObject.AddComponent<ObjectFadeInOutComponent>();
        if(FadeInOutAction != null)
        {
            FadeInOutAction.SetTargetComponent(_arrowObject);
        }
    }
    #endregion
}
