#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Overlays;
using UnityEditor.SceneManagement;
using UnityEngine.UIElements;

[Overlay(typeof(SceneView), "Scene Shortcut")]
public class CustomOverlay : Overlay
{
    public override VisualElement CreatePanelContent()
    {
        var root = new VisualElement();
        root.style.flexDirection = FlexDirection.Column;
        root.style.paddingTop = 2;

        // ==== 버튼 추가 함수 ====
        void AddButton(string label, string path)
        {
            var btn = new Button(() =>
            {
                if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                {
                    EditorSceneManager.OpenScene(path);
                }
            });

            btn.text = label;
            btn.style.marginBottom = 2;
            root.Add(btn);
        }

        // ==== 원하는 씬 버튼 추가 ====
        AddButton("Main Scene", "Assets/Scenes/Main.unity");
        AddButton("CH0 Scene", "Assets/Scenes/CH0.unity");
        AddButton("CH1 Scene", "Assets/Scenes/CH1.unity");
        AddButton("CH2 Scene", "Assets/Scenes/CH2.unity");

        return root;
    }
}
#endif
