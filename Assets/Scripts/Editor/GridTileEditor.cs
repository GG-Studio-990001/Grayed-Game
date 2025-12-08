//#if UNITY_EDITOR 전처리로 전체 에디터 스크립트를 플레이어 빌드에서 제외
#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Runtime.CH3.Main;

/// <summary>
/// 기획자용 그리드 타일 에디터
/// Scene 뷰에서 마우스로 오브젝트를 쉽게 배치할 수 있도록 도와주는 에디터 윈도우
/// </summary>
public class GridTileEditor : EditorWindow
{
    // 설치 가능한 오브젝트 타입
    public enum PlacedObjectType
    {
        Block,          // 차단 오브젝트 (빨간색)
        EventArea,      // 연출구역 (파란색)
        Teleporter,     // 텔레포터 (노란색)
        SpawnPoint,     // 플레이어 스폰 포인트 (청록, GridSystem에 좌표 저장)
        Object          // 데이터 기반 오브젝트 (CH3_LevelData 사용, isBreakable로 자동 판단, 보라색)
    }

    [Header("Prefab Settings")]
    [SerializeField] private GameObject blockPrefab;
    [SerializeField] private GameObject eventAreaPrefab;
    [SerializeField] private GameObject teleporterPrefab;
    
    [Header("Data-Based Object Settings")]
    [SerializeField] private string objectDataId = "";
    [SerializeField] private CH3_LevelData selectedLevelData;

    [Header("Grid Settings")]
    [SerializeField] private GridSystem gridSystem;
    [SerializeField] private Transform gridParent;
    [SerializeField] private bool showGridGizmos = true;
    [SerializeField] private bool showCoordinates = true;
    [SerializeField] private float gridLineAlpha = 0.8f;

    [Header("Visual Settings")]
    // 색상 상수
    private static readonly Color BLOCK_COLOR = Color.red;
    private static readonly Color EVENT_AREA_COLOR = Color.blue;
    private static readonly Color TELEPORTER_COLOR = Color.yellow;
    private static readonly Color SPAWN_POINT_COLOR = new Color(0f, 1f, 1f, 1f); // 청록
    private static readonly Color OBJECT_COLOR = new Color(0.8f, 0f, 0.8f, 1f); // 보라색
    private static readonly Color GRID_LINE_COLOR = Color.white;

    [Header("Editor State")]
    [SerializeField] private PlacedObjectType currentObjectType = PlacedObjectType.Block;
    [SerializeField] private bool isDragging = false;
    [SerializeField] private bool isLeftDragging = false;
    [SerializeField] private bool isRightDragging = false;
    [SerializeField] private Vector2Int lastDragGridPosition;
    [SerializeField] private HashSet<Vector2Int> placedPositions = new HashSet<Vector2Int>();
    private bool isToolActive = true;

    // 캐시
    private GridSystem cachedGridSystem;
    private GUIStyle coordinateStyle;
    // 그리드 점유 캐시 (기즈모 성능 최적화)
    private HashSet<Vector2Int> occupiedGridPositions = new HashSet<Vector2Int>();
    private int lastChildCount = -1;

    [MenuItem("Tools/Grid Tile Editor")]
    public static void ShowWindow()
    {
        GetWindow<GridTileEditor>("Grid Tile Editor");
    }

    private void OnEnable()
    {
        SceneView.duringSceneGui += OnSceneGUI;

        // 에디터 카메라 고정
        Tools.current = Tool.None;
        SetCameraForGridView();

        // 자동 설정
        AutoSetup();
        
        // 데이터 매니저 초기화
        GridObjectDataManager.LoadAllData();
    }

    private void OnDisable()
    {
        SceneView.duringSceneGui -= OnSceneGUI;
        
        // 기본 도구 복원
        Tools.current = Tool.Move;
    }


    private void SetCameraForGridView()
    {
        SceneView sceneView = SceneView.lastActiveSceneView;
        if (sceneView == null) return;

        // 그리드 크기 계산
        Vector2Int gridSize = GetGridSize();
        float cellSize = GetCellSize();

        // 그리드 중심점 계산
        Vector3 gridCenter = GetGridCenter();

        // 카메라 거리
        float maxDimension = Mathf.Max(gridSize.x, gridSize.y) * cellSize;
        float cameraDistance = maxDimension;

        // 탑다운 각도
        Vector3 cameraPosition = gridCenter + new Vector3(0, cameraDistance, -cameraDistance * 0.5f);

        sceneView.pivot = gridCenter;
        sceneView.rotation = Quaternion.LookRotation(gridCenter - cameraPosition, Vector3.up);

        sceneView.size = cameraDistance;

        // 강제 Repaint 제거: 불필요한 에디터 부하 방지
    }

    private Vector2Int GetGridSize()
    {
        if (cachedGridSystem != null)
        {
            try
            {
                int size = cachedGridSystem.ActualGridSize;
                return new Vector2Int(size, size);
            }
            catch { }
        }
        return new Vector2Int(21, 21);
    }

    private float GetCellSize()
    {
        if (cachedGridSystem != null)
        {
            try { return cachedGridSystem.CellWidth; } catch { }
        }
        return 1f;
    }

    private Vector3 GetGridCenter()
    {
        if (cachedGridSystem != null)
        {
            return cachedGridSystem.transform.position;
        }
        return Vector3.zero;
    }

    private void AutoSetup()
    {
        // 1. GridSystem 찾기
        FindGridSystem();
        
        // 2. GridParent 자동 설정
        if (gridParent == null)
        {
            GameObject obstacleParent = GameObject.FindGameObjectWithTag("Obstacle");
            if (obstacleParent != null)
            {
                gridParent = obstacleParent.transform;
            }
        }
        
        // 3. Prefab들 자동 바인딩
        AutoFindPrefabs();
    }

    private void FindGridSystem()
    {
        // 인스펙터에서 할당된 GridSystem이 있으면 우선 사용
        if (gridSystem != null && gridSystem.gameObject != null)
        {
            cachedGridSystem = gridSystem;
            return;
        }

        // 인스펙터에서 할당되지 않았으면 자동으로 찾기
        if (cachedGridSystem == null || cachedGridSystem.gameObject == null)
        {
            cachedGridSystem = FindObjectOfType<GridSystem>();
        }
    }

    private void OnGUI()
    {
        FindGridSystem();
        
        EditorGUILayout.LabelField("Grid Tile Editor", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        // Prefab 설정
        EditorGUILayout.LabelField("Prefab Settings", EditorStyles.boldLabel);
        blockPrefab = (GameObject)EditorGUILayout.ObjectField("Block Prefab", blockPrefab, typeof(GameObject), false);
        eventAreaPrefab = (GameObject)EditorGUILayout.ObjectField("Event Area Prefab", eventAreaPrefab, typeof(GameObject), false);
        teleporterPrefab = (GameObject)EditorGUILayout.ObjectField("Teleporter Prefab", teleporterPrefab, typeof(GameObject), false);

        EditorGUILayout.Space();

        // 그리드 설정
        EditorGUILayout.LabelField("Grid Settings", EditorStyles.boldLabel);
        gridParent = (Transform)EditorGUILayout.ObjectField("Grid Parent", gridParent, typeof(Transform), true);
        showGridGizmos = EditorGUILayout.Toggle("Show Grid Gizmos", showGridGizmos);
        showCoordinates = EditorGUILayout.Toggle("Show Coordinates", showCoordinates);
        gridLineAlpha = EditorGUILayout.Slider("Grid Line Alpha", gridLineAlpha, 0f, 1f);

        EditorGUILayout.Space();

        // 시각적 설정 (색상은 고정값 사용)
        EditorGUILayout.LabelField("Visual Settings", EditorStyles.boldLabel);
        EditorGUILayout.HelpBox("색상: Block(빨강), EventArea(파랑), Teleporter(노랑), Object(보라)", MessageType.Info);

        EditorGUILayout.Space();


        EditorGUILayout.Space();

        // 현재 오브젝트 타입 선택
        EditorGUILayout.LabelField("Placement Settings", EditorStyles.boldLabel);
        currentObjectType = (PlacedObjectType)EditorGUILayout.EnumPopup("Current Object Type", currentObjectType);
        
        // Object 타입 선택 시 데이터 ID 입력
        if (currentObjectType == PlacedObjectType.Object)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Object ID:", GUILayout.Width(100));
            string newId = EditorGUILayout.TextField(objectDataId);
            if (newId != objectDataId)
            {
                objectDataId = newId;
                selectedLevelData = GridObjectDataManager.GetDataById(objectDataId);
            }
            
            if (GUILayout.Button("검색", GUILayout.Width(60)))
            {
                selectedLevelData = GridObjectDataManager.GetDataById(objectDataId);
                if (selectedLevelData == null)
                {
                    EditorUtility.DisplayDialog("오류", $"ID '{objectDataId}'에 해당하는 데이터를 찾을 수 없습니다.", "확인");
                }
            }
            EditorGUILayout.EndHorizontal();
            
            // 선택된 데이터 정보 표시
            if (selectedLevelData != null)
            {
                EditorGUILayout.HelpBox($"선택된 데이터: {selectedLevelData.id} ({selectedLevelData.objectType}, {selectedLevelData.sizeX}x{selectedLevelData.sizeY})", MessageType.Info);
            }
            else if (!string.IsNullOrEmpty(objectDataId))
            {
                EditorGUILayout.HelpBox("데이터를 찾을 수 없습니다.", MessageType.Warning);
            }
        }

        EditorGUILayout.Space();

        // 유틸리티 버튼들
        EditorGUILayout.LabelField("Utilities", EditorStyles.boldLabel);
        
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Clear All Objects"))
        {
            ClearAllObjects();
        }
        if (GUILayout.Button("Fill All Blocks"))
        {
            FillAllBlocks();
        }

        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space();

        // GridSystem 정보 표시 생략

        // 사용법 안내
        EditorGUILayout.HelpBox(
            "사용법:\n" +
            "• 좌클릭: 오브젝트 설치, 우클릭: 오브젝트 삭제\n" +
            "• 드래그: 연속 설치 또는 삭제\n",
            MessageType.Info);

        // 유효성 검사
        ValidateSetup();
    }

    private void ValidateSetup()
    {
        if (gridSystem == null && cachedGridSystem == null)
        {
            EditorGUILayout.HelpBox("Grid System이 설정되지 않았습니다! 인스펙터에서 할당하거나 'Auto Find Grid System' 버튼을 사용하세요.", MessageType.Warning);
        }
        else if (cachedGridSystem == null)
        {
            EditorGUILayout.HelpBox("Grid System을 찾을 수 없습니다! 에디터 모드에서는 기본 그리드로 작동합니다.", MessageType.Warning);
        }
        else
        {
            EditorGUILayout.HelpBox("Grid System이 정상적으로 초기화되었습니다.", MessageType.Info);
        }

        GameObject currentPrefab = GetCurrentPrefab();
        if (currentPrefab == null && currentObjectType != PlacedObjectType.SpawnPoint && currentObjectType != PlacedObjectType.Object)
        {
            EditorGUILayout.HelpBox($"현재 선택된 {currentObjectType} Prefab이 설정되지 않았습니다!", MessageType.Warning);
        }
    }

    private GameObject GetCurrentPrefab()
    {
        switch (currentObjectType)
        {
            case PlacedObjectType.Block:
                return blockPrefab;
            case PlacedObjectType.EventArea:
                return eventAreaPrefab;
            case PlacedObjectType.Teleporter:
                return teleporterPrefab;
            case PlacedObjectType.SpawnPoint:
                return null; // 프리팹 사용 안 함
            case PlacedObjectType.Object:
                return null; // 데이터 기반 오브젝트는 프리팹 사용 안 함 (Sprite 직접 사용)
            default:
                return null;
        }
    }

    private Color GetCurrentColor()
    {
        switch (currentObjectType)
        {
            case PlacedObjectType.Block:
                return BLOCK_COLOR;
            case PlacedObjectType.EventArea:
                return EVENT_AREA_COLOR;
            case PlacedObjectType.Teleporter:
                return TELEPORTER_COLOR;
            case PlacedObjectType.SpawnPoint:
                return SPAWN_POINT_COLOR;
            case PlacedObjectType.Object:
                return OBJECT_COLOR;
            default:
                return Color.white;
        }
    }

    private void OnSceneGUI(SceneView sceneView)
    {
        // 플레이 모드에서는 타일 에디터 기즈모/입력을 비활성화하여 GridSystem 기즈모와 겹치지 않도록 함
        if (Application.isPlaying)
        {
            return;
        }
        FindGridSystem();
        
        Event e = Event.current;
        Vector3 mouseWorldPos = GetMouseWorldPosition(e.mousePosition);

        // 그리드 기즈모
        if (showGridGizmos)
        {
            DrawGridGizmos();
        }

        if (mouseWorldPos != Vector3.zero)
        {
            Vector2Int gridPos = SafeWorldToGrid(mouseWorldPos);
            Vector3 snappedWorldPos = SafeGridToWorld(gridPos);

            // 좌표 표시
            if (showCoordinates)
            {
                DrawCoordinates(gridPos, snappedWorldPos);
            }

            // 미리보기 그리기 (도구가 활성화되었을 때만)
            if (isToolActive)
            {
                DrawPreview(snappedWorldPos);
            }

            // 마우스 입력 처리 (도구가 활성화되었을 때만)
            if (isToolActive)
            {
                HandleMouseInput(e, snappedWorldPos, gridPos);
            }
        }

        // 강제 Repaint는 비활성화 (에디터 부하 방지)
    }

    // 에디터 전용 안전 좌표 변환 (GridSystem 미초기화 시에도 동작)
    private Vector2Int SafeWorldToGrid(Vector3 worldPos)
    {
        Transform origin = cachedGridSystem != null ? cachedGridSystem.transform : null;
        Vector3 basePos = origin != null ? origin.position : Vector3.zero;
        float cell = GetCellSize();

        // 방향 벡터: GridSystem 내부 정렬(_right/_forward) 미초기화 대비, Transform 기준으로 계산
        Vector3 right = origin != null ? origin.right : Vector3.right;
        Vector3 forward = origin != null ? origin.forward : Vector3.forward;
        right = Vector3.ProjectOnPlane(right, Vector3.up).normalized;
        forward = Vector3.ProjectOnPlane(forward, Vector3.up).normalized;
        if (right.sqrMagnitude < 1e-6f) right = Vector3.right;
        if (forward.sqrMagnitude < 1e-6f) forward = Vector3.forward;

        Vector3 relative = worldPos - basePos;
        float x = Vector3.Dot(relative, right) / Mathf.Max(cell, 1e-6f);
        float z = Vector3.Dot(relative, forward) / Mathf.Max(cell, 1e-6f);
        return new Vector2Int(Mathf.RoundToInt(x), Mathf.RoundToInt(z));
    }

    private Vector3 SafeGridToWorld(Vector2Int gridPos)
    {
        Transform origin = cachedGridSystem != null ? cachedGridSystem.transform : null;
        Vector3 basePos = origin != null ? origin.position : Vector3.zero;
        float cell = GetCellSize();

        Vector3 right = origin != null ? origin.right : Vector3.right;
        Vector3 forward = origin != null ? origin.forward : Vector3.forward;
        right = Vector3.ProjectOnPlane(right, Vector3.up).normalized;
        forward = Vector3.ProjectOnPlane(forward, Vector3.up).normalized;
        if (right.sqrMagnitude < 1e-6f) right = Vector3.right;
        if (forward.sqrMagnitude < 1e-6f) forward = Vector3.forward;

        Vector3 world = basePos + right * (gridPos.x * cell) + forward * (gridPos.y * cell);
        return new Vector3(world.x, 0f, world.z);
    }

    private bool IsValidGridPositionEditor(Vector2Int gridPos)
    {
        Vector2Int size = GetGridSize();
        int half = size.x / 2;
        return gridPos.x >= -half && gridPos.x <= half && gridPos.y >= -half && gridPos.y <= half;
    }

    private Vector3 GetMouseWorldPosition(Vector2 mousePosition)
    {
        Ray ray = HandleUtility.GUIPointToWorldRay(mousePosition);
        RaycastHit hit;

        // 먼저 Physics Raycast 시도
        if (Physics.Raycast(ray, out hit))
        {
            return hit.point;
        }

        // Physics Raycast가 실패하면 평면과의 교차점 계산
        Vector3 gridSystemPos = Vector3.zero;
        if (cachedGridSystem != null)
        {
            gridSystemPos = cachedGridSystem.transform.position;
        }

        Plane gridPlane = new Plane(Vector3.up, gridSystemPos);
        if (gridPlane.Raycast(ray, out float distance))
        {
            return ray.GetPoint(distance);
        }

        // 모든 방법이 실패하면 카메라에서 일정 거리 떨어진 위치 반환
        return ray.GetPoint(10f);
    }

    private void DrawGridGizmos()
    {
        // GridSystem이 없어도 기본 그리드 표시
        if (cachedGridSystem == null)
        {
            DrawDefaultGrid();
            return;
        }
        // 실제 GridSystem 파라미터 참조
        int actualGridSize;
        float cellWidth;
        try { actualGridSize = cachedGridSystem.ActualGridSize; } catch { actualGridSize = 21; }
        try { cellWidth = cachedGridSystem.CellWidth; } catch { cellWidth = 1f; }

        Handles.color = new Color(GRID_LINE_COLOR.r, GRID_LINE_COLOR.g, GRID_LINE_COLOR.b, gridLineAlpha);
        
        int halfSize = actualGridSize / 2;
        int startX = -halfSize;
        int startY = -halfSize;
        int endX = halfSize;
        int endY = halfSize;

        // 세로선 그리기
        for (int x = startX; x <= endX; x++)
        {
            Vector3 startPos = SafeGridToWorld(new Vector2Int(x, startY));
            Vector3 endPos = SafeGridToWorld(new Vector2Int(x, endY));
            Handles.DrawLine(startPos, endPos);
        }

        // 가로선 그리기
        for (int y = startY; y <= endY; y++)
        {
            Vector3 startPos = SafeGridToWorld(new Vector2Int(startX, y));
            Vector3 endPos = SafeGridToWorld(new Vector2Int(endX, y));
            Handles.DrawLine(startPos, endPos);
        }

        // 셀 표시
        for (int x = startX; x <= endX; x++)
        {
            for (int y = startY; y <= endY; y++)
            {
                Vector2Int gridPosition = new Vector2Int(x, y);
                Vector3 worldPosition = SafeGridToWorld(gridPosition);

                bool isBlocked = IsPositionBlockedByEditorObjects(worldPosition);

                if (isBlocked)
                {
                    Handles.color = new Color(0.5f, 0.5f, 0.5f, 0.8f);
                    Handles.DrawWireCube(worldPosition, new Vector3(0.9f, 0.05f, 0.9f));
                }
                else if (gridPosition == Vector2Int.zero)
                {
                    Handles.color = Color.green;
                    Handles.DrawWireDisc(worldPosition, Vector3.up, 0.15f);
                }
                else
                {
                    Handles.color = Color.yellow;
                    Handles.DrawWireDisc(worldPosition, Vector3.up, 0.1f);
                }
            }
        }

        // SpawnPoint 기즈모 (GridSystem 기반)
        if (cachedGridSystem != null)
        {
            try
            {
                var has = cachedGridSystem.HasPlayerSpawn;
                if (has)
                {
                    Vector2Int sp = cachedGridSystem.PlayerSpawnGrid;
                    Vector3 spWorld = SafeGridToWorld(sp);
                    Handles.color = SPAWN_POINT_COLOR;
                    Handles.DrawSolidDisc(spWorld + Vector3.up * 0.02f, Vector3.up, 0.25f);
                    Handles.ArrowHandleCap(0, spWorld + Vector3.up * 0.02f, Quaternion.LookRotation(Vector3.forward, Vector3.up), 0.5f, EventType.Repaint);
                }
            }
            catch {}
        }
    }

    private void DrawDefaultGrid()
    {
        // 기본 그리드 (GridSystem 없음)
        Handles.color = new Color(GRID_LINE_COLOR.r, GRID_LINE_COLOR.g, GRID_LINE_COLOR.b, gridLineAlpha);
        
        int gridSize = 21; // 더 큰 그리드 크기 (21x21)
        int halfSize = gridSize / 2;
        float cellWidth = 1f; // 기본 셀 크기
        
        // 그리드 라인 그리기
        for (int x = -halfSize; x <= halfSize; x++)
        {
            Vector3 startPos = new Vector3(x * cellWidth, 0, -halfSize * cellWidth);
            Vector3 endPos = new Vector3(x * cellWidth, 0, halfSize * cellWidth);
            Handles.DrawLine(startPos, endPos);
        }
        
        for (int y = -halfSize; y <= halfSize; y++)
        {
            Vector3 startPos = new Vector3(-halfSize * cellWidth, 0, y * cellWidth);
            Vector3 endPos = new Vector3(halfSize * cellWidth, 0, y * cellWidth);
            Handles.DrawLine(startPos, endPos);
        }
        
        // 각 셀의 중심점 표시 (성능을 위해 일부만 표시)
        for (int x = -halfSize; x <= halfSize; x += 2) // 2칸마다 표시
        {
            for (int y = -halfSize; y <= halfSize; y += 2) // 2칸마다 표시
            {
                Vector3 worldPosition = new Vector3(x * cellWidth, 0, y * cellWidth);
                
                if (x == 0 && y == 0)
                {
                    Handles.color = Color.green;
                    Handles.DrawWireDisc(worldPosition, Vector3.up, 0.15f);
                }
                else
                {
                    Handles.color = Color.yellow;
                    Handles.DrawWireDisc(worldPosition, Vector3.up, 0.1f);
                }
            }
        }
    }

    private void RebuildOccupiedCacheIfNeeded()
    {
        if (gridParent == null)
        {
            occupiedGridPositions.Clear();
            lastChildCount = -1;
            return;
        }
        if (lastChildCount == gridParent.childCount && occupiedGridPositions.Count > 0)
        {
            return; // 변화 없음
        }
        occupiedGridPositions.Clear();
        foreach (Transform child in gridParent)
        {
            if (child == null) continue;
            if (child.GetComponent<BlockedArea>() == null) continue;
            Vector2Int pos = SafeWorldToGrid(child.position);
            occupiedGridPositions.Add(pos);
        }
        lastChildCount = gridParent.childCount;
    }

    private bool IsPositionBlockedByEditorObjects(Vector3 worldPosition)
    {
        if (gridParent == null) return false;
        RebuildOccupiedCacheIfNeeded();
        Vector2Int pos = SafeWorldToGrid(worldPosition);
        return occupiedGridPositions.Contains(pos);
    }

    private void DrawCoordinates(Vector2Int gridPos, Vector3 worldPos)
    {
        if (coordinateStyle == null)
        {
            coordinateStyle = new GUIStyle();
            coordinateStyle.normal.textColor = Color.white;
            coordinateStyle.fontSize = 12;
            coordinateStyle.alignment = TextAnchor.MiddleCenter;
            coordinateStyle.normal.background = MakeTex(1, 1, new Color(0, 0, 0, 0.7f));
        }

        // 마우스 위치의 좌표를 항상 표시
        Vector3 labelPos = worldPos + Vector3.up * 0.2f;
        Handles.Label(labelPos, $"({gridPos.x}, {gridPos.y})", coordinateStyle);
    }

    private Texture2D MakeTex(int width, int height, Color col)
    {
        Color[] pix = new Color[width * height];
        for (int i = 0; i < pix.Length; i++)
            pix[i] = col;

        Texture2D result = new Texture2D(width, height);
        result.SetPixels(pix);
        result.Apply();
        return result;
    }

    private void DrawPreview(Vector3 position)
    {
        Color previewColor = GetCurrentColor();
        previewColor.a = 0.5f;

        Handles.color = previewColor;
        
        // 타입별 미리보기
        switch (currentObjectType)
        {
            case PlacedObjectType.Block:
                // Block
                Handles.DrawWireCube(position + Vector3.up * 0.05f, new Vector3(0.9f, 0.1f, 0.9f));
                break;
                
            case PlacedObjectType.EventArea:
                // EventArea
                Handles.DrawWireDisc(position + Vector3.up * 0.1f, Vector3.up, 0.4f);
                break;
                
            case PlacedObjectType.Teleporter:
                // Teleporter
                Handles.DrawWireCube(position + Vector3.up * 0.5f, Vector3.one * 0.8f);
                break;
                
            case PlacedObjectType.SpawnPoint:
                // SpawnPoint
                Handles.DrawWireDisc(position + Vector3.up * 0.2f, Vector3.up, 0.25f);
                Handles.DrawLine(position + Vector3.up * 0.01f + Vector3.left * 0.2f, position + Vector3.up * 0.01f + Vector3.right * 0.2f);
                Handles.DrawLine(position + Vector3.up * 0.01f + Vector3.forward * 0.2f, position + Vector3.up * 0.01f + Vector3.back * 0.2f);
                break;
                
            case PlacedObjectType.Object:
                // Object - 데이터 기반 오브젝트
                if (selectedLevelData != null)
                {
                    Vector2Int tileSize = selectedLevelData.TileSize;
                    Vector3 size = new Vector3(tileSize.x * 0.9f, 0.1f, tileSize.y * 0.9f);
                    Handles.DrawWireCube(position + Vector3.up * 0.05f, size);
                }
                else
                {
                    Handles.DrawWireCube(position + Vector3.up * 0.05f, new Vector3(0.9f, 0.1f, 0.9f));
                }
                break;
        }
    }

    private void HandleMouseInput(Event e, Vector3 worldPos, Vector2Int gridPos)
    {
        switch (e.type)
        {
            case EventType.MouseDown:
                if (e.button == 0) // 좌클릭 - 오브젝트 설치
                {
                    isDragging = true;
                    isLeftDragging = true;
                    isRightDragging = false;
                    lastDragGridPosition = gridPos;
                    placedPositions.Clear();
                    PlaceObject(worldPos, gridPos);
                    e.Use();
                }
                else if (e.button == 1) // 우클릭 - 오브젝트 삭제
                {
                    isDragging = true;
                    isLeftDragging = false;
                    isRightDragging = true;
                    lastDragGridPosition = gridPos;
                    placedPositions.Clear();
                    RemoveObject(worldPos, gridPos);
                    e.Use();
                }
                break;

            case EventType.MouseDrag:
                if (isDragging)
                {
                    // 그리드 위치가 변경되었을 때만 처리
                    if (gridPos != lastDragGridPosition)
                    {
                        if (isLeftDragging) // 좌클릭 드래그 - 연속 설치
                        {
                            PlaceObject(worldPos, gridPos);
                        }
                        else if (isRightDragging) // 우클릭 드래그 - 연속 삭제
                        {
                            RemoveObject(worldPos, gridPos);
                        }
                        lastDragGridPosition = gridPos;
                    }
                    e.Use();
                }
                break;

            case EventType.MouseUp:
                if (e.button == 0 || e.button == 1)
                {
                    isDragging = false;
                    isLeftDragging = false;
                    isRightDragging = false;
                    placedPositions.Clear();
                    e.Use();
                }
                break;
        }
    }

    


    private void PlaceObject(Vector3 worldPos, Vector2Int gridPos)
    {
        if (!placedPositions.Add(gridPos)) return; // 중복 방지

        // SpawnPoint는 프리팹을 놓지 않고, 기존 오브젝트 유무와 무관하게 좌표만 기록
        if (currentObjectType == PlacedObjectType.SpawnPoint)
        {
            if (!IsValidGridPositionEditor(gridPos)) return;
            if (cachedGridSystem != null)
            {
                Undo.RecordObject(cachedGridSystem, "Set Player Spawn");
                cachedGridSystem.SetPlayerSpawnGrid(gridPos);
                EditorUtility.SetDirty(cachedGridSystem);
            }
            return;
        }

        // Object 타입은 데이터 기반으로 생성
        if (currentObjectType == PlacedObjectType.Object)
        {
            if (selectedLevelData == null || !selectedLevelData.IsValid())
            {
                Debug.LogWarning("유효한 ID가 아닙니다!");
                return;
            }
            
            // 이미 해당 위치에 오브젝트가 있는지 확인
            if (IsObjectAtPosition(worldPos)) return;
            
            // 그리드 유효성 검사
            if (!IsValidGridPositionEditor(gridPos)) return;
            
            // 데이터 기반 오브젝트 생성
            GameObject newObject = GridObjectDataManager.CreateObjectFromData(selectedLevelData, worldPos);
            if (newObject == null) return;
            
            // 부모 설정
            if (gridParent != null)
            {
                newObject.transform.SetParent(gridParent);
            }
            
            // customY 적용 (데이터에 useCustomY가 true인 경우)
            if (selectedLevelData.useCustomY)
            {
                Vector3 pos = newObject.transform.position;
                pos.y = selectedLevelData.customY;
                newObject.transform.position = pos;
            }
            
            // Tile 타입인 경우 Rotation X를 90도로 설정
            if (selectedLevelData.objectType == GridObjectType.Tile)
            {
                Vector3 rotation = newObject.transform.rotation.eulerAngles;
                rotation.x = 90f;
                newObject.transform.rotation = Quaternion.Euler(rotation);
            }
            
            // 이름 설정
            newObject.name = $"{selectedLevelData.id}_{gridPos.x}_{gridPos.y}";
            
            Undo.RegisterCreatedObjectUndo(newObject, $"Place Object ({selectedLevelData.id})");
            return;
        }

        // 기존 프리팹 기반 오브젝트 생성
        GameObject prefab = GetCurrentPrefab();
        if (prefab == null) return;

        // 이미 해당 위치에 오브젝트가 있는지 확인
        if (IsObjectAtPosition(worldPos)) return;

        // 그리드 유효성 검사 (가능한 경우)
        if (!IsValidGridPositionEditor(gridPos)) return;

        // EventArea는 Block된 위치에 배치할 수 없음
        if (currentObjectType == PlacedObjectType.EventArea)
        {
            if (IsPositionBlockedByEditorObjects(worldPos))
            {
                return; // Block된 위치에는 EventArea 배치 불가
            }
        }

        // 오브젝트 생성
        GameObject newObject2 = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
        if (newObject2 == null) return;

        // 부모 설정
        if (gridParent != null)
        {
            newObject2.transform.SetParent(gridParent);
        }

        // 위치 설정
        newObject2.transform.position = SafeGridToWorld(gridPos);
        newObject2.name = $"{currentObjectType}_{gridPos.x}_{gridPos.y}";

        // GridObject 최소 초기화
        InitializeGridObject(newObject2, gridPos);

        Undo.RegisterCreatedObjectUndo(newObject2, $"Place {currentObjectType}");
    }

    private void RemoveExistingSpawnPoint()
    {
        if (gridParent == null) return;
        List<GameObject> toRemove = new List<GameObject>();
        foreach (Transform child in gridParent)
        {
            if (child == null) continue;
            if (child.name.StartsWith("SpawnPoint_"))
            {
                toRemove.Add(child.gameObject);
                continue;
            }
        // 프리팹 기반 스폰을 더 이상 사용하지 않음
        }
        foreach (var go in toRemove)
        {
            Undo.DestroyObjectImmediate(go);
        }
    }

    private void InitializeGridObject(GameObject obj, Vector2Int gridPos)
    {
        // GridObject가 있으면 위치만 동기화 (리플렉션 제거)
        GridObject gridObject = obj.GetComponent<GridObject>();
        if (gridObject != null)
        {
            // 가능한 경우 GridSystem 좌표로 스냅, 실패 시 기본 좌표 사용
            Vector3 worldPos = SafeGridToWorld(gridPos);
            gridObject.transform.position = worldPos;
        }

        // 타입별 추가 컴포넌트 보장
        switch (currentObjectType)
        {
            case PlacedObjectType.Block:
                var blockedArea = obj.GetComponent<BlockedArea>();
                if (blockedArea == null)
                {
                    obj.AddComponent<BlockedArea>();
                }
                break;

            case PlacedObjectType.EventArea:
                var eventArea = obj.GetComponent<EventArea>();
                if (eventArea == null)
                {
                    eventArea = obj.AddComponent<EventArea>();
                    eventArea.SetTriggerRadius(1f);
                }
                break;

            case PlacedObjectType.Teleporter:
                var teleporter = obj.GetComponent<Teleporter>();
                if (teleporter == null)
                {
                    obj.AddComponent<Teleporter>();
                }
                break;
        }
    }

    private void AutoFindPrefabs()
    {
        // Assets/_CH3/Prefab 폴더에서 프리팹들을 자동으로 찾아서 할당
        string prefabFolderPath = "Assets/_CH3/Prefab";
        
        // Block 프리팹 찾기
        blockPrefab = FindPrefabInFolder(prefabFolderPath, "Block");
        
        // EventArea 프리팹 찾기
        eventAreaPrefab = FindPrefabInFolder(prefabFolderPath, "EventArea");
        
        // Teleporter 프리팹 찾기
        teleporterPrefab = FindPrefabInFolder(prefabFolderPath, "Teleporter");
    }

    private GameObject FindPrefabInFolder(string folderPath, string prefabName)
    {
        // 폴더에서 해당 이름의 프리팹을 찾기
        string[] guids = AssetDatabase.FindAssets($"{prefabName} t:Prefab", new[] { folderPath });
        
        foreach (string guid in guids)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);
            
            if (prefab != null && prefab.name == prefabName)
            {
                return prefab;
            }
        }
        
        return null;
    }


    private void RemoveObject(Vector3 worldPos, Vector2Int gridPos)
    {
        if (!placedPositions.Add(gridPos)) return; // 중복 방지

        GameObject objectToRemove = GetObjectAtPosition(worldPos);
        if (objectToRemove != null)
        {
            Undo.DestroyObjectImmediate(objectToRemove);
        }
    }

    private bool IsObjectAtPosition(Vector3 worldPos)
    {
        return GetObjectAtPosition(worldPos) != null;
    }

    private GameObject GetObjectAtPosition(Vector3 worldPos)
    {
        if (gridParent == null) return null;

        foreach (Transform child in gridParent)
        {
            if (Vector3.Distance(child.position, worldPos) < 0.05f) // 더 정확한 거리 체크
            {
                return child.gameObject;
            }
        }
        return null;
    }

    private void ClearAllObjects()
    {
        if (gridParent == null) return;

        List<GameObject> objectsToDestroy = new List<GameObject>();
        foreach (Transform child in gridParent)
        {
            objectsToDestroy.Add(child.gameObject);
        }

        foreach (GameObject obj in objectsToDestroy)
        {
            Undo.DestroyObjectImmediate(obj);
        }
    }

    private void FillAllBlocks()
    {
        if (blockPrefab == null || gridParent == null) return;

        int actualGridSize;
        float cellWidth;
        try { actualGridSize = cachedGridSystem != null ? cachedGridSystem.ActualGridSize : 21; } catch { actualGridSize = 21; }
        try { cellWidth = cachedGridSystem != null ? cachedGridSystem.CellWidth : 1f; } catch { cellWidth = 1f; }

        int halfSize = actualGridSize / 2;
        // 기존 배치된 오브젝트를 좌표 기준으로 한 번만 수집하여 O(N^2) 탐색 방지
        HashSet<Vector2Int> occupied = BuildOccupiedGridPositions();

        int undoGroup = Undo.GetCurrentGroup();
        Undo.SetCurrentGroupName("Fill All Blocks");
        int processed = 0;
        int total = (halfSize * 2 + 1) * (halfSize * 2 + 1);
        try
        {
            for (int x = -halfSize; x <= halfSize; x++)
            {
                for (int y = -halfSize; y <= halfSize; y++)
                {
                    Vector2Int gridPos = new Vector2Int(x, y);
                    if (!IsValidGridPositionEditor(gridPos)) { processed++; continue; }
                    if (occupied.Contains(gridPos)) { processed++; continue; }

                    Vector3 worldPos = SafeGridToWorld(gridPos);

                    GameObject newObject = PrefabUtility.InstantiatePrefab(blockPrefab, gridParent) as GameObject;
                    if (newObject == null) { processed++; continue; }

                    newObject.transform.position = worldPos;
                    newObject.name = $"Block_{gridPos.x}_{gridPos.y}";

                    var blockedArea = newObject.GetComponent<BlockedArea>();
                    if (blockedArea == null)
                    {
                        newObject.AddComponent<BlockedArea>();
                    }

                    Undo.RegisterCreatedObjectUndo(newObject, "Fill Block");
                    processed++;
                }
            }
        }
        finally
        {
            Undo.CollapseUndoOperations(undoGroup);
        }
    }

    private HashSet<Vector2Int> BuildOccupiedGridPositions()
    {
        HashSet<Vector2Int> occupied = new HashSet<Vector2Int>();
        if (gridParent == null) return occupied;
        foreach (Transform child in gridParent)
        {
            Vector2Int pos = SafeWorldToGrid(child.position);
            occupied.Add(pos);
        }
        return occupied;
    }
}
#endif