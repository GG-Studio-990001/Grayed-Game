using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ObstacleSpawnData
{
    public string Stage;        // 스테이지 이름 (예: "1-1")
    public float Speed;         // 속도
    public int ObstacleTypes;   // 장애물 종류 개수
    public int ObstacleCount;      // 출력 횟수
}

[CreateAssetMenu(fileName = "ObstacleSpawnDataSet", menuName = "SuperArio/ObstacleSpawnDataSet", order = 1)]
public class ObstacleSpawnDataSet : ScriptableObject
{
    public List<ObstacleSpawnData> DataList; // 장애물 데이터 리스트
}