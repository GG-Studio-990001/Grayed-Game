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
        Teleporter      // 텔레포터 (노란색)
    }

    [Header("Prefab Settings")]
    [SerializeField] private GameObject blockPrefab;
    [SerializeField] private GameObject eventAreaPrefab;
    [SerializeField] private GameObject teleporterPrefab;

    [Header("Grid Settings")]
    [SerializeField] private GridSystem gridSystem;
    [SerializeField] private Transform gridParent;
    [SerializeField] private bool showGridGizmos = true;
    [SerializeField] private bool showCoordinates = true;
    [SerializeField] private float gridLineAlpha = 0.8f;

    [Header("Visual Settings")]
    // 색상은 고정값으로 사용
    private static readonly Color BLOCK_COLOR = Color.red;
    private static readonly Color EVENT_AREA_COLOR = Color.blue;
    private static readonly Color TELEPORTER_COLOR = Color.yellow;
    private static readonly Color GRID_LINE_COLOR = Color.white;

    [Header("Editor State")]
    [SerializeField] private PlacedObjectType currentObjectType = PlacedObjectType.Block;
    [SerializeField] private bool isDragging = false;
    [SerializeField] private bool isLeftDragging = false;
    [SerializeField] private bool isRightDragging = false;
    [SerializeField] private Vector2Int lastDragGridPosition;
    [SerializeField] private HashSet<Vector2Int> placedPositions = new HashSet<Vector2Int>();
    private bool isToolActive = true; // 도구는 항상 활성화

    // 최적화를 위한 캐시
    private GridSystem cachedGridSystem;
    private GUIStyle coordinateStyle;

    [MenuItem("Tools/Grid Tile Editor")]
    public static void ShowWindow()
    {
        GetWindow<GridTileEditor>("Grid Tile Editor");
    }

    private void OnEnable()
    {
        SceneView.duringSceneGui += OnSceneGUI;
        
        // Unity Tools 비활성화 및 카메라 설정
        Tools.current = Tool.None;
        SetCameraForGridView();
        
        // 자동으로 모든 설정
        AutoSetup();
    }

    private void OnDisable()
    {
        SceneView.duringSceneGui -= OnSceneGUI;
        
        // Unity Tools 복원 (기본 도구로)
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
        
        // 그리드 전체가 보이도록 카메라 거리 계산
        float maxDimension = Mathf.Max(gridSize.x, gridSize.y) * cellSize;
        float cameraDistance = maxDimension; // 여유를 두고 2배
        
        // 카메라를 그리드 위쪽에서 내려다보는 각도로 설정
        Vector3 cameraPosition = gridCenter + new Vector3(0, cameraDistance, -cameraDistance * 0.5f);
        
        // 카메라 위치 및 회전 설정
        sceneView.pivot = gridCenter;
        sceneView.rotation = Quaternion.LookRotation(gridCenter - cameraPosition, Vector3.up);
        
        // 카메라 거리 설정
        sceneView.size = cameraDistance;
        
        // Scene View 새로고침
        sceneView.Repaint();
    }

    private Vector2Int GetGridSize()
    {
        if (cachedGridSystem != null && IsGridSystemInitialized())
        {
            try
            {
                // GridSystem에서 그리드 크기 가져오기
                var actualGridSizeField = typeof(GridSystem).GetField("_actualGridSize", 
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                if (actualGridSizeField != null)
                {
                    var value = actualGridSizeField.GetValue(cachedGridSystem);
                    if (value is Vector2Int vector2Int)
                    {
                        return vector2Int;
                    }
                    else if (value is Vector2 vector2)
                    {
                        return new Vector2Int((int)vector2.x, (int)vector2.y);
                    }
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogWarning($"GridSystem에서 그리드 크기를 가져오는 중 오류 발생: {ex.Message}");
            }
        }
        
        // 기본 그리드 크기 (21x21)
        return new Vector2Int(21, 21);
    }

    private float GetCellSize()
    {
        if (cachedGridSystem != null && IsGridSystemInitialized())
        {
            try
            {
                // GridSystem에서 셀 크기 가져오기
                var cellWidthField = typeof(GridSystem).GetField("cellWidth", 
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                if (cellWidthField != null)
                {
                    var value = cellWidthField.GetValue(cachedGridSystem);
                    if (value is float floatValue)
                    {
                        return floatValue;
                    }
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogWarning($"GridSystem에서 셀 크기를 가져오는 중 오류 발생: {ex.Message}");
            }
        }
        
        // 기본 셀 크기
        return 1f;
    }

    private Vector3 GetGridCenter()
    {
        if (cachedGridSystem != null && IsGridSystemInitialized())
        {
            try
            {
                // GridSystem에서 그리드 중심점 가져오기
                var gridCenterProperty = typeof(GridSystem).GetProperty("GridCenter");
                if (gridCenterProperty != null)
                {
                    var value = gridCenterProperty.GetValue(cachedGridSystem);
                    if (value is Vector3 vector3)
                    {
                        return vector3;
                    }
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogWarning($"GridSystem에서 그리드 중심점을 가져오는 중 오류 발생: {ex.Message}");
            }
        }
        
        // 기본 그리드 중심점
        return Vector3.zero;
    }

    private void AutoSetup()
    {
        // 1. GridSystem 자동 찾기 및 초기화
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
            TryInitializeGridSystem();
            return;
        }

        // 인스펙터에서 할당되지 않았으면 자동으로 찾기
        if (cachedGridSystem == null || cachedGridSystem.gameObject == null)
        {
            cachedGridSystem = FindObjectOfType<GridSystem>();
            if (cachedGridSystem != null)
            {
                TryInitializeGridSystem();
            }
        }
    }

    private void TryInitializeGridSystem()
    {
        if (cachedGridSystem == null) return;

        // 이미 초기화되었으면 스킵
        if (IsGridSystemInitialized()) return;

        // 에디터 모드에서 필요한 의존성들을 먼저 설정
        SetupGridSystemDependencies();

        // 에디터 모드에서 임시로 초기화 시도
        try
        {
            var initializeMethod = typeof(GridSystem).GetMethod("Initialize", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            
            if (initializeMethod != null)
            {
                initializeMethod.Invoke(cachedGridSystem, null);
                //Debug.Log("GridSystem을 에디터 모드에서 임시로 초기화했습니다.");
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogWarning($"GridSystem 초기화 실패: {ex.Message}");
            Debug.LogWarning($"Inner Exception: {ex.InnerException?.Message}");
        }
    }

    private void SetupGridSystemDependencies()
    {
        if (cachedGridSystem == null) return;

        // mainCamera 설정
        var mainCameraField = typeof(GridSystem).GetField("mainCamera", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        
        if (mainCameraField != null)
        {
            var mainCamera = mainCameraField.GetValue(cachedGridSystem);
            if (mainCamera == null)
            {
                // SceneView의 카메라를 사용하거나 기본 카메라 찾기
                Camera sceneCamera = SceneView.lastActiveSceneView?.camera;
                if (sceneCamera == null)
                {
                    sceneCamera = Camera.main;
                }
                if (sceneCamera == null)
                {
                    sceneCamera = FindObjectOfType<Camera>();
                }
                
                if (sceneCamera != null)
                {
                    mainCameraField.SetValue(cachedGridSystem, sceneCamera);
                }
            }
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
        gridSystem = (GridSystem)EditorGUILayout.ObjectField("Grid System", gridSystem, typeof(GridSystem), true);
        gridParent = (Transform)EditorGUILayout.ObjectField("Grid Parent", gridParent, typeof(Transform), true);
        showGridGizmos = EditorGUILayout.Toggle("Show Grid Gizmos", showGridGizmos);
        showCoordinates = EditorGUILayout.Toggle("Show Coordinates", showCoordinates);
        gridLineAlpha = EditorGUILayout.Slider("Grid Line Alpha", gridLineAlpha, 0f, 1f);

        EditorGUILayout.Space();

        // 시각적 설정 (색상은 고정값 사용)
        EditorGUILayout.LabelField("Visual Settings", EditorStyles.boldLabel);
        EditorGUILayout.HelpBox("색상: Block(빨강), EventArea(파랑), Teleporter(노랑)", MessageType.Info);

        EditorGUILayout.Space();


        EditorGUILayout.Space();

        // 현재 오브젝트 타입 선택
        EditorGUILayout.LabelField("Placement Settings", EditorStyles.boldLabel);
        currentObjectType = (PlacedObjectType)EditorGUILayout.EnumPopup("Current Object Type", currentObjectType);

        EditorGUILayout.Space();

        // 유틸리티 버튼들
        EditorGUILayout.LabelField("Utilities", EditorStyles.boldLabel);
        
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Clear All Objects"))
        {
            ClearAllObjects();
        }

        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space();

        // GridSystem 정보 표시
        //DisplayGridSystemInfo();

        // 사용법 안내
        EditorGUILayout.HelpBox(
            "사용법:\n" +
            "• 좌클릭: 오브젝트 설치, 우클릭: 오브젝트 삭제\n" +
            "• 좌클릭하고 마우스 휠 드래그: 연속 설치\n" +
            "• 우클릭 드래그: 연속 삭제\n",
            MessageType.Info);

        // 유효성 검사
        ValidateSetup();
    }

    private void DisplayGridSystemInfo()
    {
        if (cachedGridSystem != null)
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Grid System Info", EditorStyles.boldLabel);
            
            // GridSystem의 실제 정보 표시
            var gridSizeField = typeof(GridSystem).GetField("gridSize", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var actualGridSizeField = typeof(GridSystem).GetField("_actualGridSize", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var cellWidthField = typeof(GridSystem).GetField("cellWidth", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            
            int gridSize = 9;
            int actualGridSize = 9;
            float cellWidth = 1f;
            
            if (gridSizeField != null) gridSize = (int)gridSizeField.GetValue(cachedGridSystem);
            if (actualGridSizeField != null) actualGridSize = (int)actualGridSizeField.GetValue(cachedGridSystem);
            if (cellWidthField != null) cellWidth = (float)cellWidthField.GetValue(cachedGridSystem);
            
            EditorGUILayout.LabelField($"Grid Size: {gridSize} → {actualGridSize} (actual)");
            EditorGUILayout.LabelField($"Cell Width: {cellWidth}");
            EditorGUILayout.LabelField($"Grid Center: {cachedGridSystem.GridCenter}");
        }
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
        if (currentPrefab == null)
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
            default:
                return Color.white;
        }
    }

    private void OnSceneGUI(SceneView sceneView)
    {
        FindGridSystem();
        
        Event e = Event.current;
        Vector3 mouseWorldPos = GetMouseWorldPosition(e.mousePosition);

        // 그리드 기즈모 그리기 (GridSystem이 없어도 기본 그리드 표시)
        if (showGridGizmos)
        {
            DrawGridGizmos();
        }

        if (mouseWorldPos != Vector3.zero)
        {
            Vector2Int gridPos;
            Vector3 snappedWorldPos;

            if (cachedGridSystem != null && IsGridSystemInitialized())
            {
                // GridSystem이 있고 초기화된 경우
                gridPos = cachedGridSystem.WorldToGridPosition(mouseWorldPos);
                snappedWorldPos = cachedGridSystem.GridToWorldPosition(gridPos);
            }
            else
            {
                // GridSystem이 없거나 초기화되지 않은 경우 - 기본 그리드 좌표 계산
                gridPos = new Vector2Int(
                    Mathf.RoundToInt(mouseWorldPos.x),
                    Mathf.RoundToInt(mouseWorldPos.z)
                );
                snappedWorldPos = new Vector3(gridPos.x, 0, gridPos.y);
            }

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

        sceneView.Repaint();
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

        // GridSystem의 실제 그리드 정보 사용 (리플렉션)
        var actualGridSizeField = typeof(GridSystem).GetField("_actualGridSize", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var gridSizeField = typeof(GridSystem).GetField("gridSize", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var cellWidthField = typeof(GridSystem).GetField("cellWidth", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        
        int actualGridSize = 9; // 기본값
        int gridSize = 9; // 기본값
        float cellWidth = 1f; // 기본값
        
        if (actualGridSizeField != null)
        {
            actualGridSize = (int)actualGridSizeField.GetValue(cachedGridSystem);
        }
        else if (gridSizeField != null)
        {
            gridSize = (int)gridSizeField.GetValue(cachedGridSystem);
            actualGridSize = (gridSize % 2 == 0) ? gridSize - 1 : gridSize;
        }
        
        if (cellWidthField != null)
        {
            cellWidth = (float)cellWidthField.GetValue(cachedGridSystem);
        }

        // GridSystem과 동일한 형식으로 그리드 그리기
        Handles.color = new Color(GRID_LINE_COLOR.r, GRID_LINE_COLOR.g, GRID_LINE_COLOR.b, gridLineAlpha);
        
        int halfSize = actualGridSize / 2;
        int startX = -halfSize;
        int startY = -halfSize;
        int endX = halfSize;
        int endY = halfSize;

        // 세로선 그리기
        for (int x = startX; x <= endX; x++)
        {
            Vector3 startPos, endPos;
            try
            {
                startPos = cachedGridSystem.GridToWorldPosition(new Vector2Int(x, startY));
                endPos = cachedGridSystem.GridToWorldPosition(new Vector2Int(x, endY));
            }
            catch (System.Exception)
            {
                // GridSystem이 초기화되지 않았을 때는 기본 위치 계산
                Vector3 gridCenter = cachedGridSystem.transform.position;
                startPos = gridCenter + new Vector3(x * cellWidth, 0, startY * cellWidth);
                endPos = gridCenter + new Vector3(x * cellWidth, 0, endY * cellWidth);
            }
            Handles.DrawLine(startPos, endPos);
        }

        // 가로선 그리기
        for (int y = startY; y <= endY; y++)
        {
            Vector3 startPos, endPos;
            try
            {
                startPos = cachedGridSystem.GridToWorldPosition(new Vector2Int(startX, y));
                endPos = cachedGridSystem.GridToWorldPosition(new Vector2Int(endX, y));
            }
            catch (System.Exception)
            {
                // GridSystem이 초기화되지 않았을 때는 기본 위치 계산
                Vector3 gridCenter = cachedGridSystem.transform.position;
                startPos = gridCenter + new Vector3(startX * cellWidth, 0, y * cellWidth);
                endPos = gridCenter + new Vector3(endX * cellWidth, 0, y * cellWidth);
            }
            Handles.DrawLine(startPos, endPos);
        }

        // 각 셀의 중심점과 좌표 표시 (GridSystem과 동일한 형식)
        for (int x = startX; x <= endX; x++)
        {
            for (int y = startY; y <= endY; y++)
            {
                Vector2Int gridPosition = new Vector2Int(x, y);
                Vector3 worldPosition;
                
                try
                {
                    worldPosition = cachedGridSystem.GridToWorldPosition(gridPosition);
                }
                catch (System.Exception)
                {
                    // GridSystem이 초기화되지 않았을 때는 기본 위치 계산
                    Vector3 gridCenter = cachedGridSystem.transform.position;
                    worldPosition = gridCenter + new Vector3(x * cellWidth, 0, y * cellWidth);
                }

                // 차단된 셀 표시 (GridSystem과 동일한 색상)
                bool isBlocked = false;
                if (IsGridSystemInitialized())
                {
                    try
                    {
                        isBlocked = cachedGridSystem.IsCellBlocked(gridPosition);
                    }
                    catch (System.Exception)
                    {
                        // GridSystem이 아직 초기화되지 않았을 때는 무시
                    }
                }
                else
                {
                    // GridSystem이 초기화되지 않았어도 에디터에서 배치된 Block 오브젝트 확인
                    isBlocked = IsPositionBlockedByEditorObjects(worldPosition);
                }

                if (isBlocked)
                {
                    Handles.color = new Color(0.5f, 0.5f, 0.5f, 0.8f);
                    Handles.DrawWireCube(worldPosition, new Vector3(0.9f, 0.05f, 0.9f));
                }
                // (0,0) 좌표는 다른 색상으로 표시
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
    }

    private void DrawDefaultGrid()
    {
        // 기본 그리드 그리기 (GridSystem이 없을 때)
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

    private bool IsGridSystemInitialized()
    {
        if (cachedGridSystem == null) return false;
        
        // blockedCells 배열이 초기화되었는지 확인
        var blockedCellsField = typeof(GridSystem).GetField("blockedCells", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        if (blockedCellsField != null)
        {
            var blockedCells = blockedCellsField.GetValue(cachedGridSystem);
            return blockedCells != null;
        }
        
        return false;
    }

    private bool IsPositionBlockedByEditorObjects(Vector3 worldPosition)
    {
        if (gridParent == null) return false;

        // GridParent의 자식 오브젝트들 중에서 Block 타입인 것들 확인
        foreach (Transform child in gridParent)
        {
            if (Vector3.Distance(child.position, worldPosition) < 0.1f)
            {
                // BlockedArea 컴포넌트가 있으면 차단된 것으로 간주
                if (child.GetComponent<BlockedArea>() != null)
                {
                    return true;
                }
            }
        }
        return false;
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
        
        // 오브젝트 타입에 따라 다른 기즈모 표시
        switch (currentObjectType)
        {
            case PlacedObjectType.Block:
                // Block은 바닥에 붙어있는 형태로 표시
                Handles.DrawWireCube(position + Vector3.up * 0.05f, new Vector3(0.9f, 0.1f, 0.9f));
                break;
                
            case PlacedObjectType.EventArea:
                // EventArea는 원형으로 표시
                Handles.DrawWireDisc(position + Vector3.up * 0.1f, Vector3.up, 0.4f);
                break;
                
            case PlacedObjectType.Teleporter:
                // Teleporter는 큐브로 표시
                Handles.DrawWireCube(position + Vector3.up * 0.5f, Vector3.one * 0.8f);
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

    private Vector2Int GetGridPositionFromWorld(Vector3 worldPos)
    {
        if (cachedGridSystem != null && IsGridSystemInitialized())
        {
            return cachedGridSystem.WorldToGridPosition(worldPos);
        }
        else
        {
            return new Vector2Int(
                Mathf.RoundToInt(worldPos.x),
                Mathf.RoundToInt(worldPos.z)
            );
        }
    }


    private void PlaceObject(Vector3 worldPos, Vector2Int gridPos)
    {
        if (!placedPositions.Add(gridPos)) return; // 중복 방지

        GameObject prefab = GetCurrentPrefab();
        if (prefab == null) return;

        // 이미 해당 위치에 오브젝트가 있는지 확인
        if (IsObjectAtPosition(worldPos)) return;

        // 그리드 유효성 검사 (GridSystem이 있을 때만)
        if (cachedGridSystem != null && IsGridSystemInitialized())
        {
            if (!cachedGridSystem.IsValidGridPosition(gridPos)) return;
        }

        // EventArea는 Block된 위치에 배치할 수 없음
        if (currentObjectType == PlacedObjectType.EventArea)
        {
            if (IsPositionBlockedByEditorObjects(worldPos))
            {
                return; // Block된 위치에는 EventArea 배치 불가
            }
        }

        // 오브젝트 생성
        GameObject newObject = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
        if (newObject == null) return;

        // 부모 설정
        if (gridParent != null)
        {
            newObject.transform.SetParent(gridParent);
        }

        // 위치 설정
        newObject.transform.position = worldPos;
        newObject.name = $"{currentObjectType}_{gridPos.x}_{gridPos.y}";

        // GridObject 컴포넌트 초기화
        InitializeGridObject(newObject, gridPos);

        Undo.RegisterCreatedObjectUndo(newObject, $"Place {currentObjectType}");
    }

    private void InitializeGridObject(GameObject obj, Vector2Int gridPos)
    {
        // GridObject 컴포넌트가 있다면 초기화
        GridObject gridObject = obj.GetComponent<GridObject>();
        if (gridObject != null)
        {
            // GridSystem이 유효한지 확인하고 필요하면 초기화 시도
            if (cachedGridSystem != null && cachedGridSystem.gameObject != null)
            {
                // 아직 초기화되지 않았으면 시도
                if (!IsGridSystemInitialized())
                {
                    TryInitializeGridSystem();
                }
                
                // 초기화된 경우에만 정상 초기화 시도
                if (IsGridSystemInitialized())
                {
                // gridManager를 먼저 설정 (리플렉션 사용)
                var gridManagerField = typeof(GridObject).GetField("gridManager", 
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                if (gridManagerField != null)
                {
                    gridManagerField.SetValue(gridObject, cachedGridSystem);
                }

                // 에디터 모드에서는 Initialize 메서드 대신 수동으로 설정
                // (Initialize 메서드에서 MinimapManager 등 의존성 문제로 인한 에러 방지)
                SetupGridObjectManually(gridObject, gridPos);
                return;
                }
            }
            
            // GridSystem이 없거나 초기화 실패한 경우 - 에디터 모드용 기본 설정
            // 에디터 모드에서는 정상적인 상황이므로 로그를 출력하지 않음
            
            // 기본 위치는 이미 설정되어 있으므로 추가 설정만 수행
            // GridObject의 gridPosition 필드만 설정
            var gridPositionField = typeof(GridObject).GetField("gridPosition", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (gridPositionField != null)
            {
                gridPositionField.SetValue(gridObject, gridPos);
            }
        }

        // 특정 타입별 추가 초기화
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
                    // 기본 설정
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
        
        // 조용히 바인딩 (로그 없이)
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

    private void SetupGridObjectManually(GridObject gridObject, Vector2Int gridPos)
    {
        // 수동으로 GridObject의 기본 필드들 설정
        var gridPositionField = typeof(GridObject).GetField("gridPosition", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        if (gridPositionField != null)
        {
            gridPositionField.SetValue(gridObject, gridPos);
        }

        // 월드 위치 설정
        Vector3 worldPos;
        if (cachedGridSystem != null)
        {
            try
            {
                worldPos = cachedGridSystem.GridToWorldPosition(gridPos);
            }
            catch (System.Exception)
            {
                // GridSystem이 제대로 작동하지 않으면 기본 위치 사용
                worldPos = new Vector3(gridPos.x, 0, gridPos.y);
            }
        }
        else
        {
            worldPos = new Vector3(gridPos.x, 0, gridPos.y);
        }
        gridObject.transform.position = worldPos;

        // SpriteRenderer 설정 (있는 경우)
        var spriteRendererField = typeof(GridObject).GetField("spriteRenderer", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        if (spriteRendererField != null)
        {
            var spriteRenderer = gridObject.GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                spriteRendererField.SetValue(gridObject, spriteRenderer);
            }
        }

        // MinimapManager 설정 (있는 경우)
        var minimapManagerField = typeof(GridObject).GetField("minimapManager", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        if (minimapManagerField != null)
        {
            var minimapManagerType = System.Type.GetType("Runtime.CH3.Main.MinimapManager");
            if (minimapManagerType != null)
            {
                var minimapManagerInstance = FindObjectOfType(minimapManagerType);
                if (minimapManagerInstance != null)
                {
                    minimapManagerField.SetValue(gridObject, minimapManagerInstance);
                }
            }
        }

        // SortingOrderObject 설정 (있는 경우)
        var sortingOrderObject = gridObject.GetComponent<SortingOrderObject>();
        if (sortingOrderObject != null)
        {
            // 뒤(y가 작을수록)일수록 더 높은 정렬이 되도록 부호 반전
            sortingOrderObject.SetBaseOrder(-gridPos.y * 100); // 기본 정렬 스케일
        }
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
}
#endif