using Febucci.UI.Effects;
using UnityEditor;

namespace Febucci.UI.Core
{
    [CustomEditor(typeof(AnimationScriptableBase), true)]
    class AnimScriptableDrawer : Editor
    {
        GenericSharedDrawer drawer = new GenericSharedDrawer(true);
        public override void OnInspectorGUI()
        {
            drawer.OnInspectorGUI(serializedObject);
        }
    }
}