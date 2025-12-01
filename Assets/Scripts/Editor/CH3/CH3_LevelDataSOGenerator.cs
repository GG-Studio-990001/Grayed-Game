using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;

namespace Runtime.CH3.Main
{
    public class CH3_LevelDataSOGenerator : EditorWindow
    {
        private const string CSV_PATH = "Assets/Resources/Data/CH3/CH3_LevelData.csv";
        private const string LEVEL_DATA_FOLDER = "Assets/Resources/Data/CH3/LevelData";
        private const string ITEM_DATA_FOLDER = "Assets/Resources/Data/CH3/Items";

        [MenuItem("CH3/Generate LevelData & Items from CSV")]
        public static void ShowWindow()
        {
            GetWindow<CH3_LevelDataSOGenerator>("CSV to SO Generator");
        }

        private void OnGUI()
        {
            EditorGUILayout.LabelField("CSV to ScriptableObject Generator", EditorStyles.boldLabel);
            EditorGUILayout.Space();

            EditorGUILayout.LabelField($"CSV Path: {CSV_PATH}", EditorStyles.wordWrappedLabel);
            EditorGUILayout.LabelField($"LevelData Folder: {LEVEL_DATA_FOLDER}", EditorStyles.wordWrappedLabel);
            EditorGUILayout.LabelField($"Item Folder: {ITEM_DATA_FOLDER}", EditorStyles.wordWrappedLabel);
            EditorGUILayout.Space();

            if (GUILayout.Button("Generate All ScriptableObjects", GUILayout.Height(30)))
            {
                GenerateAll();
            }

            EditorGUILayout.Space();

            if (GUILayout.Button("Clear All Generated Files", GUILayout.Height(30)))
            {
                if (EditorUtility.DisplayDialog("경고", "생성된 모든 파일을 삭제하시겠습니까?", "삭제", "취소"))
                {
                    ClearAll();
                }
            }
        }

        private void GenerateAll()
        {
            CH3_LevelDataCSVLoader.ClearCache();
            var csvData = CH3_LevelDataCSVLoader.LoadAllFromCSV();
            if (csvData == null || csvData.Count == 0)
            {
                EditorUtility.DisplayDialog("오류", "CSV 데이터를 불러올 수 없습니다.", "확인");
                return;
            }

            EnsureFoldersExist();

            int levelDataCount = 0;
            int itemCount = 0;
            int linkedCount = 0;

            foreach (var kvp in csvData)
            {
                var csvLevelData = kvp.Value;
                
                CH3_LevelData levelDataSO = CreateOrUpdateLevelData(csvLevelData);
                if (levelDataSO != null)
                {
                    levelDataCount++;
                }

                if (csvLevelData.isBuilding)
                {
                    Item itemSO = CreateOrUpdateItem(csvLevelData, levelDataSO);
                    if (itemSO != null)
                    {
                        itemCount++;
                        if (levelDataSO != null)
                        {
                            linkedCount++;
                        }
                    }
                }
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            EditorUtility.DisplayDialog("완료", 
                $"생성 완료!\n" +
                $"LevelData: {levelDataCount}개\n" +
                $"Item: {itemCount}개\n" +
                $"연동: {linkedCount}개", 
                "확인");
        }

        private void EnsureFoldersExist()
        {
            if (!Directory.Exists(LEVEL_DATA_FOLDER))
            {
                Directory.CreateDirectory(LEVEL_DATA_FOLDER);
                AssetDatabase.Refresh();
            }

            if (!Directory.Exists(ITEM_DATA_FOLDER))
            {
                Directory.CreateDirectory(ITEM_DATA_FOLDER);
                AssetDatabase.Refresh();
            }
        }

        private CH3_LevelData CreateOrUpdateLevelData(CH3_LevelData csvData)
        {
            string assetPath = $"{LEVEL_DATA_FOLDER}/{csvData.id}.asset";
            CH3_LevelData existing = AssetDatabase.LoadAssetAtPath<CH3_LevelData>(assetPath);

            if (existing == null)
            {
                existing = ScriptableObject.CreateInstance<CH3_LevelData>();
                AssetDatabase.CreateAsset(existing, assetPath);
            }

            existing.id = csvData.id;
            existing.dev = csvData.dev;
            existing.sizeX = csvData.sizeX;
            existing.sizeY = csvData.sizeY;
            existing.isBuilding = csvData.isBuilding;
            existing.maxBuild = csvData.maxBuild;
            existing.isBreakable = csvData.isBreakable;
            existing.maxDrop = csvData.maxDrop;
            existing.isRespawn = csvData.isRespawn;
            existing.dropCurrency = new List<CurrencyData>(csvData.dropCurrency);

            EditorUtility.SetDirty(existing);
            return existing;
        }

        private Item CreateOrUpdateItem(CH3_LevelData csvData, CH3_LevelData levelDataSO)
        {
            string assetPath = $"{ITEM_DATA_FOLDER}/{csvData.id}_Item.asset";
            Item existing = AssetDatabase.LoadAssetAtPath<Item>(assetPath);

            if (existing == null)
            {
                existing = ScriptableObject.CreateInstance<Item>();
                AssetDatabase.CreateAsset(existing, assetPath);
            }

            existing.itemName = csvData.dev;
            existing.buildingData = levelDataSO;

            EditorUtility.SetDirty(existing);
            return existing;
        }

        private void ClearAll()
        {
            if (Directory.Exists(LEVEL_DATA_FOLDER))
            {
                string[] files = Directory.GetFiles(LEVEL_DATA_FOLDER, "*.asset");
                foreach (string file in files)
                {
                    AssetDatabase.DeleteAsset(file);
                }
            }

            if (Directory.Exists(ITEM_DATA_FOLDER))
            {
                string[] files = Directory.GetFiles(ITEM_DATA_FOLDER, "*.asset");
                foreach (string file in files)
                {
                    AssetDatabase.DeleteAsset(file);
                }
            }

            AssetDatabase.Refresh();
            EditorUtility.DisplayDialog("완료", "모든 파일이 삭제되었습니다.", "확인");
        }
    }
}

