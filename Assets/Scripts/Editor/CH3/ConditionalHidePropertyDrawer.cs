using UnityEngine;
using UnityEditor;

namespace Runtime.CH3.Main
{
    [CustomPropertyDrawer(typeof(ConditionalHideAttribute))]
    public class ConditionalHidePropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            ConditionalHideAttribute condHAtt = (ConditionalHideAttribute)attribute;
            bool enabled = GetConditionalHideAttributeResult(condHAtt, property);
            
            if (enabled)
            {
                EditorGUI.PropertyField(position, property, label, true);
            }
        }
        
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            ConditionalHideAttribute condHAtt = (ConditionalHideAttribute)attribute;
            bool enabled = GetConditionalHideAttributeResult(condHAtt, property);
            
            if (enabled)
            {
                return EditorGUI.GetPropertyHeight(property, label);
            }
            else
            {
                // 숨길 때는 높이를 0으로 반환 (공간도 차지하지 않음)
                return 0f;
            }
        }
        
        private bool GetConditionalHideAttributeResult(ConditionalHideAttribute condHAtt, SerializedProperty property)
        {
            bool enabled = true;
            
            // propertyPath에서 현재 필드 이름을 찾아서 조건 필드 경로로 교체
            string propertyPath = property.propertyPath;
            
            // 리스트나 배열의 경우 Array.size, Array.data[0] 같은 경로가 있을 수 있으므로 처리
            // 예: "productionItems.Array.size" -> "productionItems"를 찾아서 "isBuilding"으로 교체
            string basePath = propertyPath;
            if (basePath.Contains(".Array"))
            {
                basePath = basePath.Substring(0, basePath.IndexOf(".Array"));
            }
            
            string[] pathParts = basePath.Split('.');
            
            // 마지막 부분(현재 필드 이름)을 조건 필드 이름으로 교체
            if (pathParts.Length > 0)
            {
                pathParts[pathParts.Length - 1] = condHAtt.conditionalSourceField;
                string conditionPath = string.Join(".", pathParts);
                
                SerializedProperty sourcePropertyValue = property.serializedObject.FindProperty(conditionPath);
                
                if (sourcePropertyValue != null)
                {
                    enabled = sourcePropertyValue.boolValue;
                }
                else
                {
                    // 경로를 찾지 못하면 직접 찾기 시도
                    sourcePropertyValue = property.serializedObject.FindProperty(condHAtt.conditionalSourceField);
                    if (sourcePropertyValue != null)
                    {
                        enabled = sourcePropertyValue.boolValue;
                    }
                    else
                    {
                        // 디버그 모드에서만 경고 출력 (리스트 필드의 경우 정상적으로 발생할 수 있음)
                        // Debug.LogWarning($"ConditionalHide: '{condHAtt.conditionalSourceField}' 필드를 찾을 수 없습니다. PropertyPath: {propertyPath}");
                    }
                }
            }
            else
            {
                // 경로를 찾지 못하면 직접 찾기 시도
                SerializedProperty sourcePropertyValue = property.serializedObject.FindProperty(condHAtt.conditionalSourceField);
                if (sourcePropertyValue != null)
                {
                    enabled = sourcePropertyValue.boolValue;
                }
            }
            
            return enabled;
        }
    }
}

