using UnityEngine;

namespace Runtime.CH3.Main
{
    /// <summary>
    /// 조건부로 인스펙터에서 필드를 숨기거나 표시하는 속성
    /// </summary>
    public class ConditionalHideAttribute : PropertyAttribute
    {
        public string conditionalSourceField;
        public bool hideInInspector;
        
        public ConditionalHideAttribute(string conditionalSourceField, bool hideInInspector = true)
        {
            this.conditionalSourceField = conditionalSourceField;
            this.hideInInspector = hideInInspector;
        }
    }
}

