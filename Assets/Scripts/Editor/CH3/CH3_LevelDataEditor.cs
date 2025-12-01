using UnityEngine;
using UnityEditor;

namespace Runtime.CH3.Main
{
    [CustomEditor(typeof(CH3_LevelData))]
    public class CH3_LevelDataEditor : Editor
    {
        private SerializedProperty idProperty;
        private SerializedProperty devProperty;
        private SerializedProperty sizeXProperty;
        private SerializedProperty sizeYProperty;
        private SerializedProperty spriteProperty;
        private SerializedProperty uiPrefabProperty;
        private SerializedProperty objectTypeProperty;
        
        // Building
        private SerializedProperty isBuildingProperty;
        private SerializedProperty maxBuildProperty;
        private SerializedProperty buildCurrencyProperty;
        private SerializedProperty productionItemsProperty;
        private SerializedProperty productionIntervalProperty;
        private SerializedProperty maxProductionProperty;
        
        // Structure
        private SerializedProperty isBlockingProperty;
        
        // Mining
        private SerializedProperty isBreakableProperty;
        private SerializedProperty maxDropProperty;
        private SerializedProperty dropCurrencyProperty;
        
        // Mineable
        private SerializedProperty maxMiningCountProperty;
        private SerializedProperty miningStageSpritesProperty;
        private SerializedProperty itemPrefabProperty;
        private SerializedProperty minDropCountProperty;
        private SerializedProperty maxDropCountProperty;
        private SerializedProperty dropRadiusProperty;
        
        // Respawn
        private SerializedProperty isRespawnProperty;
        private SerializedProperty respawnDelayMinProperty;
        private SerializedProperty respawnDelayMaxProperty;
        
        // Grid Position
        private SerializedProperty gridPositionModeProperty;
        private SerializedProperty gridPositionProperty;
        private SerializedProperty useCustomYProperty;
        private SerializedProperty customYProperty;
        
        // Sorting
        private SerializedProperty applyInitialGridSortingProperty;
        private SerializedProperty gridSortingScaleProperty;
        
        // Interaction
        private SerializedProperty interactionRangeProperty;
        private SerializedProperty enableColliderOnStartProperty;
        
        private void OnEnable()
        {
            idProperty = serializedObject.FindProperty("id");
            devProperty = serializedObject.FindProperty("dev");
            sizeXProperty = serializedObject.FindProperty("sizeX");
            sizeYProperty = serializedObject.FindProperty("sizeY");
            spriteProperty = serializedObject.FindProperty("sprite");
            uiPrefabProperty = serializedObject.FindProperty("uiPrefab");
            objectTypeProperty = serializedObject.FindProperty("objectType");
            
            isBuildingProperty = serializedObject.FindProperty("isBuilding");
            maxBuildProperty = serializedObject.FindProperty("maxBuild");
            buildCurrencyProperty = serializedObject.FindProperty("buildCurrency");
            productionItemsProperty = serializedObject.FindProperty("productionItems");
            productionIntervalProperty = serializedObject.FindProperty("productionInterval");
            maxProductionProperty = serializedObject.FindProperty("maxProduction");
            
            isBlockingProperty = serializedObject.FindProperty("isBlocking");
            
            isBreakableProperty = serializedObject.FindProperty("isBreakable");
            maxDropProperty = serializedObject.FindProperty("maxDrop");
            dropCurrencyProperty = serializedObject.FindProperty("dropCurrency");
            
            maxMiningCountProperty = serializedObject.FindProperty("maxMiningCount");
            miningStageSpritesProperty = serializedObject.FindProperty("miningStageSprites");
            itemPrefabProperty = serializedObject.FindProperty("itemPrefab");
            minDropCountProperty = serializedObject.FindProperty("minDropCount");
            maxDropCountProperty = serializedObject.FindProperty("maxDropCount");
            dropRadiusProperty = serializedObject.FindProperty("dropRadius");
            
            isRespawnProperty = serializedObject.FindProperty("isRespawn");
            respawnDelayMinProperty = serializedObject.FindProperty("respawnDelayMin");
            respawnDelayMaxProperty = serializedObject.FindProperty("respawnDelayMax");
            
            gridPositionModeProperty = serializedObject.FindProperty("gridPositionMode");
            gridPositionProperty = serializedObject.FindProperty("gridPosition");
            useCustomYProperty = serializedObject.FindProperty("useCustomY");
            customYProperty = serializedObject.FindProperty("customY");
            
            applyInitialGridSortingProperty = serializedObject.FindProperty("applyInitialGridSorting");
            gridSortingScaleProperty = serializedObject.FindProperty("gridSortingScale");
            
            interactionRangeProperty = serializedObject.FindProperty("interactionRange");
            enableColliderOnStartProperty = serializedObject.FindProperty("enableColliderOnStart");
        }
        
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            
            EditorGUILayout.LabelField("Basic Info", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(idProperty);
            EditorGUILayout.PropertyField(devProperty);
            
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Grid Settings", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(sizeXProperty);
            EditorGUILayout.PropertyField(sizeYProperty);
            
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Prefab References", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(spriteProperty);
            EditorGUILayout.PropertyField(uiPrefabProperty);
            
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Object Type", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(objectTypeProperty);
            
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Structure Settings", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(isBlockingProperty);
            
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Mining Settings", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(isBreakableProperty);
            
            if (isBreakableProperty.boolValue)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(maxDropProperty);
                EditorGUILayout.PropertyField(dropCurrencyProperty);
                EditorGUI.indentLevel--;
                
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Mineable Settings", EditorStyles.boldLabel);
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(maxMiningCountProperty);
                EditorGUILayout.PropertyField(miningStageSpritesProperty);
                EditorGUILayout.PropertyField(itemPrefabProperty);
                EditorGUILayout.PropertyField(minDropCountProperty);
                EditorGUILayout.PropertyField(maxDropCountProperty);
                EditorGUILayout.PropertyField(dropRadiusProperty);
                EditorGUI.indentLevel--;
                
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Respawn Settings", EditorStyles.boldLabel);
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(isRespawnProperty);
                EditorGUILayout.PropertyField(respawnDelayMinProperty);
                EditorGUILayout.PropertyField(respawnDelayMaxProperty);
                EditorGUI.indentLevel--;
            }
            
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Building Settings", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(isBuildingProperty);
            
            if (isBuildingProperty.boolValue)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(maxBuildProperty);
                EditorGUILayout.PropertyField(buildCurrencyProperty);
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Production Settings", EditorStyles.boldLabel);
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(productionItemsProperty);
                EditorGUILayout.PropertyField(productionIntervalProperty);
                EditorGUILayout.PropertyField(maxProductionProperty);
                EditorGUI.indentLevel--;
                EditorGUI.indentLevel--;
            }
            
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Grid Position Settings", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(gridPositionModeProperty);
            
            // UseInspectorPosition일 때만 gridPosition 표시
            if (gridPositionModeProperty.enumValueIndex == (int)GridObject.GridPositionInitializationMode.UseInspectorPosition)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(gridPositionProperty);
                EditorGUI.indentLevel--;
            }
            
            EditorGUILayout.PropertyField(useCustomYProperty);
            EditorGUILayout.PropertyField(customYProperty);
            
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Sorting Settings", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(applyInitialGridSortingProperty);
            EditorGUILayout.PropertyField(gridSortingScaleProperty);
            
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Interaction Settings", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(interactionRangeProperty);
            EditorGUILayout.PropertyField(enableColliderOnStartProperty);
            
            serializedObject.ApplyModifiedProperties();
        }
    }
}

