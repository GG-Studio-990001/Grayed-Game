using UnityEditor;
using UnityEngine;

namespace Febucci.UI.Core.Editors
{
    [CustomEditor(typeof(TypewriterByCharacter), true)]
    class TypewriterByCharacterDrawer : TypewriterCoreDrawer
    {
        SerializedProperty waitForNormalChars;
        SerializedProperty waitLong;
        SerializedProperty waitMiddle;
        SerializedProperty avoidMultiplePunctuactionWait;
        SerializedProperty waitForNewLines;
        SerializedProperty waitForLastCharacter;

        PropertyWithDifferentLabel useTypewriterWaitForDisappearances;
        PropertyWithDifferentLabel disappearanceWaitTime;
        PropertyWithDifferentLabel disappearanceSpeedMultiplier;

        protected override void OnEnable()
        {
            base.OnEnable();

            waitForNormalChars = serializedObject.FindProperty("waitForNormalChars");
            waitLong = serializedObject.FindProperty("waitLong");
            waitMiddle = serializedObject.FindProperty("waitMiddle");
            avoidMultiplePunctuactionWait = serializedObject.FindProperty("avoidMultiplePunctuactionWait");
            waitForNewLines = serializedObject.FindProperty("waitForNewLines");
            waitForLastCharacter = serializedObject.FindProperty("waitForLastCharacter");
            useTypewriterWaitForDisappearances = new PropertyWithDifferentLabel(serializedObject, "useTypewriterWaitForDisappearances", "Use Typewriter Wait Times");
            disappearanceSpeedMultiplier = new PropertyWithDifferentLabel(serializedObject, "disappearanceSpeedMultiplier", "Typewriter Speed Multiplier");
            disappearanceWaitTime = new PropertyWithDifferentLabel(serializedObject, "disappearanceWaitTime", "Disappearances Wait");
        }

        protected override string[] GetPropertiesToExclude()
        {
            string[] newProperties = new string[] {
                "script",
                "waitForNormalChars",
                "waitLong",
                "waitMiddle",
                "avoidMultiplePunctuactionWait",
                "waitForNewLines",
                "waitForLastCharacter",
                "useTypewriterWaitForDisappearances",
                "disappearanceSpeedMultiplier",
                "disappearanceWaitTime"
            };

            string[] baseProperties = base.GetPropertiesToExclude();

            string[] mergedArray = new string[newProperties.Length + baseProperties.Length];

            for (int i = 0; i < baseProperties.Length; i++)
            {
                mergedArray[i] = baseProperties[i];
            }

            for (int i = 0; i < newProperties.Length; i++)
            {
                mergedArray[i + baseProperties.Length] = newProperties[i];
            }

            return mergedArray;
        }

        protected override void OnTypewriterSectionGUI()
        {
            EditorGUILayout.PropertyField(waitForNormalChars);
            EditorGUILayout.PropertyField(waitLong);
            EditorGUILayout.PropertyField(waitMiddle);

            EditorGUILayout.PropertyField(avoidMultiplePunctuactionWait);
            EditorGUILayout.PropertyField(waitForNewLines);
            EditorGUILayout.PropertyField(waitForLastCharacter);
        }

        protected override void OnDisappearanceSectionGUI()
        {
            useTypewriterWaitForDisappearances.PropertyField();

            if (useTypewriterWaitForDisappearances.property.boolValue)
                disappearanceSpeedMultiplier.PropertyField();
            else
                disappearanceWaitTime.PropertyField();

        }
    }
}