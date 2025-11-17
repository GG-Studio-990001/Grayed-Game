using UnityEngine;
using UnityEditor;

namespace Runtime.CH3.Main
{
    /// <summary>
    /// 그리드 타일 에디터 툴에서 사용할 오브젝트 생성 윈도우
    /// </summary>
    public class GridObjectCreatorWindow : EditorWindow
    {
        private string selectedId = "";
        private Vector2 scrollPosition;
        private CH3_LevelData selectedData;
        
        [MenuItem("CH3/Grid Object Creator")]
        public static void ShowWindow()
        {
            GetWindow<GridObjectCreatorWindow>("Grid Object Creator");
        }
        
        private void OnEnable()
        {
            GridObjectDataManager.LoadAllData();
        }
        
        private void OnGUI()
        {
            EditorGUILayout.LabelField("Grid Object Creator", EditorStyles.boldLabel);
            EditorGUILayout.Space();
            
            // ID 입력 필드
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Object ID:", GUILayout.Width(100));
            selectedId = EditorGUILayout.TextField(selectedId);
            
            if (GUILayout.Button("검색", GUILayout.Width(60)))
            {
                selectedData = GridObjectDataManager.GetDataById(selectedId);
                if (selectedData == null)
                {
                    EditorUtility.DisplayDialog("오류", $"ID '{selectedId}'에 해당하는 데이터를 찾을 수 없습니다.", "확인");
                }
            }
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.Space();
            
            // 데이터 목록 표시
            EditorGUILayout.LabelField("사용 가능한 ID 목록:", EditorStyles.boldLabel);
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, GUILayout.Height(200));
            
            var allIds = GridObjectDataManager.GetAllIds();
            foreach (var id in allIds)
            {
                if (GUILayout.Button(id, EditorStyles.miniButton))
                {
                    selectedId = id;
                    selectedData = GridObjectDataManager.GetDataById(id);
                }
            }
            
            EditorGUILayout.EndScrollView();
            
            EditorGUILayout.Space();
            
            // 선택된 데이터 정보 표시
            if (selectedData != null)
            {
                EditorGUILayout.LabelField("선택된 데이터 정보:", EditorStyles.boldLabel);
                EditorGUILayout.LabelField($"ID: {selectedData.id}");
                EditorGUILayout.LabelField($"타입: {selectedData.objectType}");
                EditorGUILayout.LabelField($"크기: {selectedData.sizeX}x{selectedData.sizeY}");
                
                EditorGUILayout.Space();
                
                // 생성 버튼
                if (GUILayout.Button("오브젝트 생성", GUILayout.Height(30)))
                {
                    CreateObject();
                }
            }
            else if (!string.IsNullOrEmpty(selectedId))
            {
                EditorGUILayout.HelpBox("데이터를 찾을 수 없습니다. ID를 확인해주세요.", MessageType.Warning);
            }
        }
        
        private void CreateObject()
        {
            if (selectedData == null)
            {
                EditorUtility.DisplayDialog("오류", "데이터가 선택되지 않았습니다.", "확인");
                return;
            }
            
            // 씬 뷰의 중앙 위치에 생성
            Vector3 spawnPosition = Vector3.zero;
            if (SceneView.lastActiveSceneView != null)
            {
                spawnPosition = SceneView.lastActiveSceneView.pivot;
            }
            
            GameObject newObject = GridObjectDataManager.CreateObjectFromData(selectedData, spawnPosition);
            
            if (newObject != null)
            {
                Selection.activeGameObject = newObject;
                SceneView.FrameLastActiveSceneView();
            }
        }
    }
}

