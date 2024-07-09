using Febucci.UI.Effects;
using UnityEditor;

namespace Febucci.UI.Core
{
    [CustomEditor(typeof(AnimationsDatabase), true)]
    class AnimDatabaseScriptableDrawer : Editor
    {
        DatabaseSharedDrawer drawer = new DatabaseSharedDrawer();

        public override void OnInspectorGUI()
        {
            drawer.OnInspectorGUI(serializedObject);
        }
    }
}