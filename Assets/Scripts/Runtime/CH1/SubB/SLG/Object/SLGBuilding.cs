using Runtime.CH1.Pacmom;
using SLGDefines;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public struct SLGBuilding
{
    [Header("BuildingData")]
    [SerializeField] private SLGBuildingType _buildingType;
    [SerializeField] private string _buildingName;
    [SerializeField] private Vector2 _buildingPos;
    [SerializeField] private Vector2 _reqAsset;
    [SerializeField] private int _reqCoin;
    public int NeededConstrcutionTimeSec;

    [Header("Resources")]
    [SerializeField] Sprite _detailViewImage;

    [Header("RelatedObjects")]
    [SerializeField] private GameObject _fieldObj; // May Have ObjectSwitcher


    public SLGBuildingType GetBuildingType() { return _buildingType; }
    public string GetBuildingName() { return _buildingName; }

    public GameObject GetFieldObject() { return _fieldObj; }

    public Vector2 GetBuildingPos() { return _buildingPos; }
    public Vector2 GetReqAsset() { return _reqAsset; }
    public int GetReqCoin() { return _reqCoin; }
    public Sprite GetDetailViewImage() { return _detailViewImage; }
}
public class SLGBuildingObject
{
    SLGBuilding _buildingData;
    long _constructionBeginTime;
    private SLGBuildingProgress _buildingProgress;


    public SLGBuildingProgress GetProgress() { return _buildingProgress; }
    public void SetProgress(SLGBuildingProgress InProgress) { _buildingProgress = InProgress; }
    public long GetConstructBeginTime() { return _constructionBeginTime; }
    public void SetConstructBeginTime(long InBeginTime) { _constructionBeginTime = InBeginTime; }

    public void SetBuildingData(SLGBuilding InData) { _buildingData = InData;  }
    public SLGBuilding GetBuildingData() { return _buildingData; }

    public void SaveBuildingData()
    {
        Managers.Data.CH1.SLGConstructionBeginTime.SetValue(_constructionBeginTime, (int)_buildingData.GetBuildingType());
        Managers.Data.CH1.SLGBuildingProgressData.SetValue(_buildingProgress, (int)_buildingData.GetBuildingType());
    }

    public BuildingState GetBuildingState()
    {
        switch (_buildingData.GetBuildingType())
        {
            case SLGBuildingType.MamagoCompany:
            case SLGBuildingType.Bridge:
                if (_buildingProgress >= SLGBuildingProgress.EndConstruct)
                {
                    return BuildingState.Constructed;
                }
                break;
            case SLGBuildingType.R2Mon:
                return BuildingState.Locked;
            case SLGBuildingType.DollarStatue:
                return BuildingState.Impossible;
        }
        if (_buildingData.GetReqAsset().x <= Managers.Data.CH1.SLGWoodCount && _buildingData.GetReqAsset().y <= Managers.Data.CH1.SLGStoneCount)
        {
            return BuildingState.Able;
        }
        return BuildingState.Impossible;
    }
    public void ChangeBuildingState(SLGBuildingProgress InProgress)
    {
        _buildingProgress = InProgress;
        switch (_buildingProgress)
        {
            case SLGBuildingProgress.None: break;
            case SLGBuildingProgress.BeforeConstruct: break;
            case SLGBuildingProgress.Constructing:
                _constructionBeginTime = (long)((DateTime.Now - DateTime.MinValue).TotalSeconds);
                break;
            case SLGBuildingProgress.EndConstruct: break;
            default: break;
        }
    }
}