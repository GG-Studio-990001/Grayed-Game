using Febucci.UI.Actions;
using Febucci.UI.Core.Parsing;
using UnityEditor;

namespace Febucci.UI.Core
{
    [CustomEditor(typeof(ActionDatabase), true)]
    class ActionDatabaseScriptableDrawer : Editor
    {
        DatabaseSharedDrawer drawer = new DatabaseSharedDrawer();

        public override void OnInspectorGUI()
        {
            drawer.OnInspectorGUI(serializedObject);
        }
    }
}