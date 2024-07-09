using System.Text;
using UnityEngine;
using UnityEditor;

namespace Febucci.UI.Core
{
    [CustomEditor(typeof(Core.TAnimCore), true)]
    class TAnimCoreDrawer : Editor
    {
        SerializedProperty m_Script;

        SerializedProperty typewriterStartsAutomatically;

        SerializedProperty referenceFontSize;
        SerializedProperty useDynamicScaling;
        
        SerializedProperty timeScale;
        SerializedProperty animationLoop;
        SerializedProperty isResettingTimeOnNewText;


        SerializedProperty defaultTagsMode;
        SerializedProperty defaultAppearancesTags;
        SerializedProperty defaultBehaviorsTags;
        SerializedProperty defaultDisappearancesTags;

        SerializedProperty useDefaultDatabases;
        
        SerializedProperty databaseBehaviorsField;
        DatabaseSharedDrawer databaseBehaviorsDrawer;
        SerializedProperty databaseAppearancesField;
        DatabaseSharedDrawer databaseAppearancesDrawer;
        SerializedProperty databaseActionsField;
        DatabaseSharedDrawer databaseActionsDrawer;

        SerializedProperty useDefaultStylesheet;
        SerializedProperty styleSheetField;
        
        Core.TAnimCore script;
        static string[] excludedProperties = new string[]
        {
            "m_Script",
            "_text",
            "databaseBehaviors",
            "databaseAppearances",
            "defaultAppearancesTags",
            "defaultBehaviorsTags",
            "defaultDisappearancesTags",
            "typewriterStartsAutomatically",
            nameof(TAnimCore.referenceFontSize),
            nameof(TAnimCore.useDynamicScaling),
            nameof(TAnimCore.defaultTagsMode),
            nameof(TAnimCore.timeScale),
            nameof(TAnimCore.animationLoop),
            nameof(TAnimCore.isResettingTimeOnNewText),
            "databaseActions",
            "useDefaultDatabases",
            nameof(TAnimCore.useDefaultStyleSheet),
            "styleSheet"
        };

        void OnEnable()
        {
            script = (Core.TAnimCore)target;

            m_Script = serializedObject.FindProperty("m_Script");

            typewriterStartsAutomatically = serializedObject.FindProperty("typewriterStartsAutomatically");

            useDefaultDatabases = serializedObject.FindProperty("useDefaultDatabases");
            databaseBehaviorsField = serializedObject.FindProperty("databaseBehaviors");
            databaseAppearancesField = serializedObject.FindProperty("databaseAppearances");
            databaseActionsField = serializedObject.FindProperty("databaseActions");

            databaseBehaviorsDrawer = new DatabaseSharedDrawer();
            databaseAppearancesDrawer = new DatabaseSharedDrawer();
            databaseActionsDrawer = new DatabaseSharedDrawer();
            
            defaultTagsMode = serializedObject.FindProperty(nameof(Febucci.UI.Core.TAnimCore.defaultTagsMode));
            defaultAppearancesTags = serializedObject.FindProperty("defaultAppearancesTags");
            defaultBehaviorsTags = serializedObject.FindProperty("defaultBehaviorsTags");
            defaultDisappearancesTags = serializedObject.FindProperty("defaultDisappearancesTags");
            
            useDefaultStylesheet = serializedObject.FindProperty(nameof(Febucci.UI.Core.TAnimCore.useDefaultStyleSheet));
            styleSheetField = serializedObject.FindProperty("styleSheet");

            referenceFontSize = serializedObject.FindProperty(nameof(Febucci.UI.Core.TAnimCore.referenceFontSize));
            useDynamicScaling = serializedObject.FindProperty(nameof(Febucci.UI.Core.TAnimCore.useDynamicScaling));
            timeScale = serializedObject.FindProperty(nameof(Febucci.UI.Core.TAnimCore.timeScale));
            animationLoop = serializedObject.FindProperty(nameof(Febucci.UI.Core.TAnimCore.animationLoop));
            isResettingTimeOnNewText = serializedObject.FindProperty(nameof(Febucci.UI.Core.TAnimCore.isResettingTimeOnNewText));

            gui_visibleCharacters = new GUIContent("Visible Characters", null,
                $"Range of visible characters in the text.\nTo modify this via script, set \"{nameof(TAnimCore.firstVisibleCharacter)}\" and \"{nameof(TAnimCore.maxVisibleCharacters)}\"");
            
            EditorApplication.playModeStateChanged += OnPlayModeChanged;
            RegisterUndoRedraw();
            UnregisterPlayback();
        }

        void OnPlayModeChanged(PlayModeStateChange stateChange)
        {
            if(stateChange == PlayModeStateChange.ExitingEditMode)
                UnregisterPlayback();
        }
        
        private void OnDisable()
        {
            EditorApplication.playModeStateChanged -= OnPlayModeChanged;
            UnregisterPlayback();
            UnregisterUndoRedraw();
        }

        private void OnDestroy()
        {
            UnregisterPlayback();
            UnregisterUndoRedraw();
        }

        #region Undo Redo

        private bool undoRedoRegistered;

        void RegisterUndoRedraw()
        {
            if (undoRedoRegistered)
                return;

            undoRedoRegistered = true;
            Undo.undoRedoPerformed += UndoRedraw;
        }

        void UndoRedraw()
        {
            OnDisable();
            OnEnable();
            Repaint();
            runInEditMode = false;
        }

        void UnregisterUndoRedraw()
        {
            if (!undoRedoRegistered)
                return;

            undoRedoRegistered = false;
            Undo.undoRedoPerformed -= UndoRedraw;

        }


        #endregion
        
        #region Playback
        
        string textBeforePreview;
        string textDuringPreview;

        private bool runInEditMode;
        bool isPlaying => Application.isPlaying || runInEditMode;

        private GUIContent gui_visibleCharacters;
        Vector2 playbackCharsScrollView;
        bool extraPlaybackControls;
        void DrawPlayback()
        {
            void HookPlaybackEvent()
            {
                script.time.RestartTime();
                
                if (runInEditMode) RegisterPlayback();
                else UnregisterPlayback();
            }
            
            //--- Playback Toolbar ---
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(Application.isPlaying ? "Playback info" : "Preview in Edit Mode", GUILayout.Width(120));
            GUI.enabled = !Application.isPlaying;
            if(GUILayout.Button(runInEditMode ? TexturesLoader.StopIcon : TexturesLoader.PlayIcon, EditorStyles.label, GUILayout.Width(20), GUILayout.Height(20)))
            {
                runInEditMode = !runInEditMode;
                HookPlaybackEvent();
            }

            GUI.enabled = runInEditMode && !Application.isPlaying;
            if(GUILayout.Button(TexturesLoader.SaveIcon, EditorStyles.label, GUILayout.Width(20), GUILayout.Height(20)))
            {
                textBeforePreview = textDuringPreview;
                runInEditMode = false;
                HookPlaybackEvent();
            }
            GUI.enabled = true;
            
            EditorGUILayout.EndHorizontal();

            //---Visible characters---
            if (isPlaying)
            {
                //---Text---
                if (!Application.isPlaying)
                {
                    EditorGUI.BeginChangeCheck();

                    textDuringPreview = EditorGUILayout.TextArea(textDuringPreview, GUILayout.MinHeight(50));
                    if (EditorGUI.EndChangeCheck())
                    {
                        script.SetText(textDuringPreview);
                        EditorUtility.SetDirty(script);
                    }
                }

                //---Visible characters---
                int charCount = script.CharactersCount;

                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                //First visible character
                EditorGUI.BeginChangeCheck();
                float minValue = script.firstVisibleCharacter;
                float maxValue = script.maxVisibleCharacters;
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.MinMaxSlider(gui_visibleCharacters, ref minValue, ref maxValue, 0, charCount);
                if (EditorGUI.EndChangeCheck())
                {
                    script.firstVisibleCharacter = Mathf.RoundToInt(minValue);
                    script.maxVisibleCharacters = Mathf.RoundToInt(maxValue);
                }

                EditorGUILayout.Space();
                int intMinValue = script.firstVisibleCharacter;
                int intMaxValue = script.maxVisibleCharacters;
                EditorGUI.BeginChangeCheck();
                intMinValue = EditorGUILayout.DelayedIntField(GUIContent.none, intMinValue, GUILayout.Width(30));
                EditorGUILayout.LabelField("/", GUILayout.Width(10));
                intMaxValue = EditorGUILayout.DelayedIntField(GUIContent.none, intMaxValue, GUILayout.Width(30));
                if(EditorGUI.EndChangeCheck())
                {
                    script.firstVisibleCharacter = Mathf.Clamp(intMinValue, 0, charCount);
                    script.maxVisibleCharacters = Mathf.Clamp(intMaxValue, 0, charCount);
                }
                
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PrefixLabel("Time passed:");
                if(GUILayout.Button(TexturesLoader.RestartIcon, EditorStyles.label, GUILayout.Width(20), GUILayout.Height(20)))
                {
                    script.time.RestartTime();
                }
                EditorGUILayout.LabelField(script.time.timeSinceStart.ToString("F2"), EditorStyles.boldLabel);
                
                EditorGUILayout.EndHorizontal();

                GUI.color = extraPlaybackControls ? Color.gray : Color.white;
                if (GUILayout.Button("Extra Visibility Controls", EditorStyles.helpBox))
                {
                    extraPlaybackControls = !extraPlaybackControls;
                }
                GUI.color = Color.white;
                
                if (extraPlaybackControls)
                {
                    const float boxSize = 12;
                    
                    if(script.WordsCount > 20) 
                        EditorGUILayout.HelpBox("Displaying only the first 20 words to optimize performance", MessageType.None);

                    playbackCharsScrollView = EditorGUILayout.BeginScrollView(playbackCharsScrollView);
                    
                    EditorGUILayout.BeginHorizontal();
                    for (int w = 0; w < script.WordsCount && w < 20; w++) //max X words for performance
                    {
                        EditorGUILayout.BeginVertical();
                        if (GUILayout.Button(script.Words[w].text, EditorStyles.miniButton))
                        {
                            script.SetVisibilityWord(w, !script.Characters[script.Words[w].firstCharacterIndex].isVisible);
                        }

                        EditorGUILayout.BeginHorizontal();
                        for (int i = script.Words[w].firstCharacterIndex; i <= script.Words[w].lastCharacterIndex; i++) 
                        {
                            script.Characters[i].isVisible = EditorGUILayout.Toggle(script.Characters[i].isVisible,
                                GUILayout.Width(boxSize));
                        }
                        EditorGUILayout.EndHorizontal();

                        EditorGUILayout.EndVertical();
                    }
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.EndScrollView();
                }

                EditorGUILayout.EndVertical();
            }
            
            EditorGUILayout.EndVertical();
        }

        bool registeredPlayback;

        void RegisterPlayback()
        {
            if (registeredPlayback) return;
            registeredPlayback = true;

            if (script)
            {
                textBeforePreview = script.GetOriginalTextFromSource();
                textDuringPreview = textBeforePreview;
                if (textDuringPreview.StartsWith("<noparse></noparse>"))
                    textDuringPreview = textDuringPreview.Remove(0, 19);

                //resets text and databases regardless
                script.ForceDatabaseRefresh();
                script.SetText(textDuringPreview);
            }

            lastTime = EditorApplication.timeSinceStartup;
            EditorApplication.update += UpdatePlayback;
        }

        void UnregisterPlayback()
        {
            if (!registeredPlayback) return;
            registeredPlayback = false;

            if (script)
            {
                // unfocus text area field to prevent having the same text when it appears again
                if (textDuringPreview != string.Empty)
                    GUIUtility.keyboardControl = -1;
                
                script.SetTextToSource(textBeforePreview);
                textBeforePreview = string.Empty;
                textDuringPreview = string.Empty;
            }

            EditorApplication.update -= UpdatePlayback;
        }

        private double lastTime = 0;
        void UpdatePlayback()
        {
            script.Animate((float)(EditorApplication.timeSinceStartup - lastTime));
            lastTime = EditorApplication.timeSinceStartup;
            EditorApplication.QueuePlayerLoopUpdate();
            Repaint();
        }
        
        #endregion
        
        #region Default Tags
        bool drawDefaultBehaviorTags = false;
        bool drawDefaultAppearancesTags = false;
        bool drawDefaultDisappearancesTags = false;

        void ForceDatabaseRefresh()
        {
            serializedObject.ApplyModifiedProperties();
            script.ForceDatabaseRefresh();
        }
        
        void DrawDefaultTags()
        {
            const string helpConstantTags = "How many of these effects will be applied to the entire text";
            const string helpFallbackTags = "How many of these effects will be applied to a letter, in case there aren't others of the same category.";

            void DrawDefaultArray<T>(ref bool enabledOption, string name, SerializedProperty array,
                Database<T> database) where T : ScriptableObject, ITagProvider
            {
                int size = array.arraySize;
                EditorGUILayout.BeginHorizontal();
                enabledOption = EditorGUILayout.Foldout(enabledOption, name + $" [{size} enabled]", true);

                bool areAllTagsValid = true;

                bool IsTagValid(string tag)
                {
                    // hackyly returns valid on an empty tag, to allow user to add new tags without error
                    return string.IsNullOrEmpty(tag) ||
                           database.ContainsKey(tag.Split(' ')[0]); //splits in case of attributes
                }

                // Displays icon if any of the tags is not found in the database
                if (database)
                {

                    for (int i = 0; i < array.arraySize && areAllTagsValid; i++)
                    {
                        if (!IsTagValid(array.GetArrayElementAtIndex(i).stringValue)) areAllTagsValid = false;
                    }

                    if (!areAllTagsValid)
                    {
                        GUILayout.Box(TexturesLoader.WarningIcon, EditorStyles.label, GUILayout.Width(20),
                            GUILayout.Height(20));
                    }
                }

                EditorGUILayout.EndHorizontal();
                if (enabledOption)
                {
                    EditorGUI.indentLevel++; //--begin of foldout

                    if (!areAllTagsValid)
                    {
                        StringBuilder sb = new StringBuilder();
                        foreach (var effect in database.Data)
                        {
                            if(!effect) continue;
                            if(string.IsNullOrEmpty(effect.TagID)) continue;
                            sb.Append(effect.TagID);
                            sb.Append(" ");
                        }

                        EditorGUILayout.HelpBox(
                            $"The tags with the warning icons will not be recognized by Text Animator. Accepted tags: {sb}",
                            MessageType.Warning);
                    }

                    GUI.enabled = false;
                    EditorGUILayout.LabelField(
                        (defaultTagsMode.intValue == (int)Core.TAnimCore.DefaultTagsMode.Fallback
                            ? helpFallbackTags
                            : helpConstantTags), EditorStyles.wordWrappedMiniLabel);
                    GUI.enabled = true;

                    //Edits array size
                    EditorGUI.BeginChangeCheck();
                    size = EditorGUILayout.IntField("Effects Count", size);
                    if (EditorGUI.EndChangeCheck())
                    {
                        bool increasing = size > array.arraySize;
                        size = Mathf.Clamp(size, 0, size + 1);
                        array.arraySize = size;

                        if (increasing && size > 1) //new element is empty
                        {
                            array.GetArrayElementAtIndex(size - 1).stringValue = string.Empty;
                        }

                        ForceDatabaseRefresh();
                    }

                    Vector2 scrollPos = Vector2.zero;
                    if (size > 0)
                    {
                        GUI.enabled = false;
                        EditorGUILayout.LabelField(
                            "Choose one effect per array element, e.g. 'wave'.\nModifiers are also accepted, e.g. 'wave a=2'",
                            EditorStyles.wordWrappedMiniLabel);
                        GUI.enabled = true;
                        EditorGUI.indentLevel++; //--begin of array

                        GUIContent temp = new GUIContent();
                        string tempString;
                        for (int i = 0; i < array.arraySize; i++)
                        {
                            var element = array.GetArrayElementAtIndex(i);
                            temp.text = "Effect #" + (i + 1);
                            tempString = element.stringValue;
                            // Displays a nice set of button the user can choose recognized tags from

                            EditorGUILayout.BeginHorizontal();
                            EditorGUI.BeginChangeCheck();
                            tempString = EditorGUILayout.DelayedTextField(temp, tempString);
                            if (EditorGUI.EndChangeCheck())
                            {
                                element.stringValue = tempString;
                                ForceDatabaseRefresh();
                            }

                            if (database && !IsTagValid(tempString))
                            {
                                GUILayout.Box(TexturesLoader.WarningIcon, EditorStyles.label, GUILayout.Width(20),
                                    GUILayout.Height(20));
                            }

                            EditorGUILayout.EndHorizontal();
                        }

                        EditorGUI.indentLevel--; //--end of array
                    }

                    EditorGUI.indentLevel--; //--end of foldout
                }
            }

            EditorGUILayout.LabelField("Default Tags", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(defaultTagsMode, true);
            if(EditorGUI.EndChangeCheck())
                ForceDatabaseRefresh();

            if (useDefaultDatabases.boolValue)
            {
                var settings = TextAnimatorSettings.Instance;
                if (settings)
                {
                    DrawDefaultArray(ref drawDefaultAppearancesTags, "Appearances", defaultAppearancesTags, settings.appearances.defaultDatabase);
                    DrawDefaultArray(ref drawDefaultBehaviorTags, "Behaviors", defaultBehaviorsTags, settings.behaviors.defaultDatabase);
                    DrawDefaultArray(ref drawDefaultDisappearancesTags, "Disappearances", defaultDisappearancesTags, settings.appearances.defaultDatabase);
                }
            }
            else
            {
                DrawDefaultArray(ref drawDefaultAppearancesTags, "Appearances", defaultAppearancesTags, script.DatabaseAppearances);
                DrawDefaultArray(ref drawDefaultBehaviorTags, "Behaviors", defaultBehaviorsTags, script.DatabaseBehaviors);
                DrawDefaultArray(ref drawDefaultDisappearancesTags, "Disappearances", defaultDisappearancesTags, script.DatabaseAppearances);
            }

            EditorGUI.indentLevel--;
        }
#endregion

        #region  Databases
        bool editBehaviors = false;
        bool editAppearances = false;
        bool editActions = false;
        TextAnimatorSettings settings;
        SerializedObject settingsObject;

        void CacheSettingsObject()
        {
            if (!settings) settings = TextAnimatorSettings.Instance;
            if(!settings) return;
            if(settingsObject == null) settingsObject = new SerializedObject(settings);
        }

        void DrawSettingsFixErrorLabel()
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.HelpBox("Text Animator Settings not found. Please re-run the setup wizard or click the following button.", MessageType.Error);
            if (GUILayout.Button("Fix it for me", GUILayout.Width(80)))
            {
                TextAnimatorSetupWindow.FixSettingsFileNotFound();
            }
            EditorGUILayout.EndHorizontal();
        }
        
        void DrawDatabases()
        {
            EditorGUILayout.LabelField("Edit Effects & Actions", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(useDefaultDatabases);
            GUI.enabled = false;
            EditorGUILayout.LabelField(useDefaultDatabases.boolValue ? "Editing databases for every component that uses default settings." : "Editing databases assigned only to this component.", EditorStyles.wordWrappedMiniLabel);
            GUI.enabled = true;
            EditorGUILayout.EndHorizontal();

            void DrawDatabaseField(ref bool foldoutToggle, string foldoutName, SerializedProperty field, DatabaseSharedDrawer drawer)
            {
                foldoutToggle = EditorGUILayout.Foldout(foldoutToggle, foldoutName, true);

                if (foldoutToggle)
                {
                    EditorGUI.indentLevel++;
                    EditorGUI.BeginChangeCheck();
                    GUI.enabled = !useDefaultDatabases.boolValue;
                    EditorGUILayout.PropertyField(field);
                    GUI.enabled = true;
                    if (EditorGUI.EndChangeCheck())
                        ForceDatabaseRefresh();
                    drawer.OnInspectorGUI(field);
                    EditorGUI.indentLevel--;
                }
            }

            if (useDefaultDatabases.boolValue)
            {
                CacheSettingsObject();

                if (!settings)
                {
                    DrawSettingsFixErrorLabel();
                }
                else
                {
                    //Draws default settings instead
                    SerializedProperty databaseBehaviorsField = settingsObject.FindProperty(nameof(settings.behaviors)).FindPropertyRelative(nameof(settings.behaviors.defaultDatabase));
                    SerializedProperty databaseAppearancesField = settingsObject.FindProperty(nameof(settings.appearances)).FindPropertyRelative(nameof(settings.appearances.defaultDatabase));
                    SerializedProperty databaseActionsField = settingsObject.FindProperty(nameof(settings.actions)).FindPropertyRelative(nameof(settings.actions.defaultDatabase));
                    DrawDatabaseField(ref editAppearances, "Appearances/Disappearances", databaseAppearancesField, databaseAppearancesDrawer);
                    DrawDatabaseField(ref editBehaviors, "Behaviors", databaseBehaviorsField, databaseBehaviorsDrawer);
                    DrawDatabaseField(ref editActions, "Actions", databaseActionsField, databaseActionsDrawer);
                }
            }
            else
            {
                DrawDatabaseField(ref editAppearances, "Appearances/Disappearances", databaseAppearancesField, databaseAppearancesDrawer);
                DrawDatabaseField(ref editBehaviors, "Behaviors", databaseBehaviorsField, databaseBehaviorsDrawer);
                DrawDatabaseField(ref editActions, "Actions", databaseActionsField, databaseActionsDrawer);
            }
            
            EditorGUI.indentLevel--;
        }

        #endregion

        #region StyleSheet

        void DrawStyleSheet()
        {
            EditorGUILayout.LabelField("Style Sheet", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(useDefaultStylesheet);
            GUI.enabled = false;
            EditorGUILayout.LabelField(useDefaultStylesheet.boolValue ? "Using the StyleSheet found in your Settings Scriptable Object" : "Using the specific StyleSheet reference below", EditorStyles.wordWrappedMiniLabel);
            GUI.enabled = true;
            EditorGUILayout.EndHorizontal();

            if (useDefaultStylesheet.boolValue)
            {
                CacheSettingsObject();

                if (!settings)
                {
                    DrawSettingsFixErrorLabel();
                }
                else
                {
                    GUI.enabled = false;
                    var defaultSettingsField = settingsObject.FindProperty(nameof(settings.defaultStyleSheet));
                    EditorGUILayout.PropertyField(defaultSettingsField);
                    GUI.enabled = true;
                }
            }
            else
            {
                EditorGUILayout.PropertyField(styleSheetField);
            }
            
            EditorGUI.indentLevel--;
        }

        #endregion
        
        #region Main Settings
        void DrawMainSettings()
        {
            EditorGUILayout.LabelField("Main Settings", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(typewriterStartsAutomatically);
            if(typewriterStartsAutomatically.boolValue) 
                EditorGUILayout.LabelField("(Remember to add a Typewriter component!)", EditorStyles.wordWrappedMiniLabel);

            EditorGUILayout.PropertyField(useDynamicScaling);
            if(useDynamicScaling.boolValue)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(referenceFontSize);
                EditorGUI.indentLevel--;
            }

            EditorGUILayout.PropertyField(animationLoop);
            if ((int)animationLoop.intValue != (int)AnimationLoop.Script)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(timeScale);
                EditorGUI.indentLevel--;
            }

            EditorGUILayout.PropertyField(isResettingTimeOnNewText);

            EditorGUI.indentLevel--;
        }
        #endregion

        void DrawHelpers()
        {
            //EditorGUILayout.LabelField("Helpers", EditorStyles.boldLabel);
            
            EditorGUI.indentLevel++;
            CacheSettingsObject();
            EditorGUILayout.BeginHorizontal();
            
            GUI.enabled = settings;
            if (GUILayout.Button("Select Global Settings", EditorStyles.helpBox))
            {
                Selection.activeObject = settings;
                EditorGUIUtility.PingObject(settings);
            }
            GUI.enabled = true;
            
            if (GUILayout.Button("Open Documentation", EditorStyles.helpBox))
            {
                Application.OpenURL("https://www.febucci.com/text-animator-unity/docs/");
            }
            
            if (GUILayout.Button("Join Discord", EditorStyles.helpBox))
            {
                Application.OpenURL(TextAnimatorSetupWindow.url_discord);
            }

            EditorGUILayout.EndHorizontal();
            
            EditorGUI.indentLevel--;
        }
        
        public override void OnInspectorGUI()
        {
            GUI.enabled = false;
            EditorGUILayout.PropertyField(m_Script);
            GUI.enabled = true;
            //--- Draws Text Animator ---

            DrawPlayback();
            EditorGUILayout.Space();

            DrawMainSettings();
            EditorGUILayout.Space();

            DrawDefaultTags();
            EditorGUILayout.Space();
            
            DrawDatabases();
            EditorGUILayout.Space();

            DrawStyleSheet();
            EditorGUILayout.Space();
            
            DrawHelpers();
            EditorGUILayout.Space();
            
            //--- Draws the rest ---
            //(in case of custom inspector from child classes etc.)
            DrawPropertiesExcluding(serializedObject, excludedProperties);

            if(serializedObject.hasModifiedProperties)
                serializedObject.ApplyModifiedProperties();
        }
    }

}