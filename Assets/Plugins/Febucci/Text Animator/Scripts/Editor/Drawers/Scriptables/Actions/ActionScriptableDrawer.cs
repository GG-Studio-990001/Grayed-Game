using Febucci.UI.Actions;
using UnityEditor;

namespace Febucci.UI.Core
{
    [CustomEditor(typeof(ActionScriptableBase), true)]
    class ActionScriptableDrawer : Editor
    {
        GenericSharedDrawer drawer = new GenericSharedDrawer(true);
        public override void OnInspectorGUI()
        {
            drawer.OnInspectorGUI(serializedObject);
        }
    }
}