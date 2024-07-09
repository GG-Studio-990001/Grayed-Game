using System;
using Febucci.UI.Core;
using UnityEditor;
using UnityEngine;
using System.Linq;
using Febucci.UI.Actions;
using Febucci.UI.Effects;
using Febucci.UI.Styles;

namespace Febucci.UI
{
    public class TextAnimatorSetupWindow : EditorWindow
    {
        const string currentVersion = "2.1.1";
        const string path_defaultInstallation = "Assets/Plugins/Febucci/Text Animator";

        TextAnimatorInstallationData installationData;
        bool settingsFileFound;

        public const string url_discord = "https://discord.gg/j4pySDa5rU";


        /// <summary>
        /// Called whenever the editor is loaded,
        /// e.g. useful for checking if the asset is imported
        /// for the first time or after an update
        /// </summary>
        [InitializeOnLoadMethod]
        internal static void TryShowingWindowOnLoad()
        {
            EditorApplication.delayCall += () => ShowWindow(true);;
        }

        #region Menu Items

        const string menuParent = "Tools/Febucci/TextAnimator/";
        
        [MenuItem(menuParent + "About Window", priority = 1)]
        internal static void Menu_ShowWindowAlways() => ShowWindow(false);
        
        [MenuItem(menuParent + "Utils/Select Settings SO", priority = 11)]
        static void Menu_SelectSettingsScriptable()
        {
            if (TextAnimatorSettings.Instance)
            {
                Selection.activeObject = TextAnimatorSettings.Instance;
            }
            else
            {
                if (EditorUtility.DisplayDialog("Settings not found",
                        "Text Animator's settings file has not been found. Do you want to run the setup and create it automatically?",
                        "Yes", "No"))
                {
                    FixSettingsFileNotFound();
                    Selection.activeObject = TextAnimatorSettings.Instance;
                }
            }
        }


        #endregion

        #region Window

        static Version oldVersion;
        bool shouldUpdate = false;
        static void ShowWindow(bool onlyOnUpdate)
        {
            // already installed
            bool shouldUpdate = false;
            if (IsTextAnimatorInstalled(out var installationGUID))
            {
                string installationPath = AssetDatabase.GUIDToAssetPath(installationGUID);
                var installationData = AssetDatabase.LoadAssetAtPath<TextAnimatorInstallationData>(installationPath);

                // same version installed, no need to show the window
                if (onlyOnUpdate && installationData.latestVersion == currentVersion)
                {
                    return;
                }
                
                Version.TryParse(installationData.latestVersion, out oldVersion);
                shouldUpdate = UpdateProject(installationData,  oldVersion, false);
            }
            else
            {
                //--- First time import ---
                //Does nothing, asking the user to install with one click
            }
            
            //Initializes the asset for the first time
            var window = (TextAnimatorSetupWindow)GetWindow(typeof(TextAnimatorSetupWindow), true,
                "Text Animator Setup", true);

            window.shouldUpdate = shouldUpdate;
            window.maxSize = new Vector2(351, 485);
            window.minSize = window.maxSize;
            window.settingsFileFound = TextAnimatorSettings.Instance;
        }

        bool triedInstallingOnce;
        void OnGUI()
        {
            if (!installationData)
            {
                if (!triedInstallingOnce)
                {
                    EditorGUILayout.LabelField("Installing....");
                    triedInstallingOnce = true;
                    installationData = GetOrCreateInstallationData();
                    Repaint();
                }
                else
                {
                    EditorGUILayout.LabelField("Unable to install package, please try to reopen this window from the Tools->Febucci menu");
                    return;
                } 
            }

            //--- HEADER ---
            GUILayout.Box(TexturesLoader.AboutLogo, EditorStyles.wordWrappedLabel);
            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Welcome!", EditorStyles.boldLabel);

            if (shouldUpdate)
            {
                //--- Updates to new version ---
                using (new EditorGUILayout.HorizontalScope(EditorStyles.helpBox))
                {
                    EditorGUILayout.LabelField(
                        "You have updated to a new version! Do you want us to set up the new things for you?", EditorStyles.wordWrappedLabel);
                    GUI.backgroundColor = Color.green;
                    if (GUILayout.Button("Yes"))
                    {
                        UpdateProject(installationData, oldVersion, true);
                        EditorUtility.DisplayDialog("Text Animator", "Update has been completed. Have fun!", "Yay!");
                        shouldUpdate = false;
                    }

                    GUI.backgroundColor = Color.white;
                    if (GUILayout.Button("No"))
                    {
                        shouldUpdate = false;
                    }
                }
            }
            else
            {
                EditorGUILayout.LabelField(
                    "Thank you for using Text Animator. Have fun bringing your projects to life!",
                    EditorStyles.wordWrappedLabel);
            }

            EditorGUILayout.Space();

            //--- VERSION STATUS ---
            
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel("Version:", EditorStyles.boldLabel);
            EditorGUILayout.LabelField(installationData.latestVersion, EditorStyles.whiteMiniLabel);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();
            
            if (!settingsFileFound)
            {
                FixSettingsFileNotFound();
                settingsFileFound = true;
            }
            
            // --- LINKS etc. ---
            EditorGUILayout.LabelField("Online Resources", EditorStyles.boldLabel);
            EditorGUILayout.LabelField("Here are some useful resources.", EditorStyles.label);

            EditorGUILayout.BeginHorizontal();
            const string baseUrl = "https://febucci.com/text-animator-unity/";
            if (GUILayout.Button("What's New"))
            {
                Application.OpenURL($"{baseUrl}changelog/");
            }
            
            if (GUILayout.Button("Documentation"))
            {
                Application.OpenURL($"{baseUrl}docs/");
            }

            if (GUILayout.Button("Support"))
            {
                Application.OpenURL($"{baseUrl}support/");
            }

            EditorGUILayout.EndHorizontal();

            
            //--Extras--
            EditorGUILayout.LabelField("Extras", EditorStyles.boldLabel);

            EditorGUILayout.LabelField("Would you like to be included in a future Text Animator showcase?",
                EditorStyles.wordWrappedMiniLabel);
            if (GUILayout.Button("-> Submit your game/project"))
                Application.OpenURL("https://www.febucci.com/text-animator-unity/showcase/");
            
            EditorGUILayout.Space(1);
            EditorGUILayout.LabelField("Please consider writing a review for the asset. It takes one minute but it really helps. Thanks!",
                EditorStyles.wordWrappedMiniLabel);
            if (GUILayout.Button("♥ Review on the Asset Store"))
                Application.OpenURL("https://assetstore.unity.com/packages/slug/158707");
            

            GUILayout.Space(5);
            EditorGUILayout.LabelField("Cheers! @febucci", EditorStyles.centeredGreyMiniLabel);
        }

        #endregion

        #region Installation

        static TextAnimatorInstallationData GetOrCreateInstallationData()
        {
            if (IsTextAnimatorInstalled(out var installationGuid))
            {
                return AssetDatabase.LoadAssetAtPath<TextAnimatorInstallationData>(AssetDatabase.GUIDToAssetPath(installationGuid));
            }
            
            var data = _CreateScriptableAssetAtPath<TextAnimatorInstallationData>(path_defaultInstallation + "/Data",
                "InstallationData");
            data.latestVersion = currentVersion;
            EditorUtility.SetDirty(data);
            AssetDatabase.SaveAssets();

            return data;
        }
        
        static bool IsTextAnimatorInstalled(out string installationGUID)
        {
            string[] path = AssetDatabase.FindAssets($"t:{nameof(TextAnimatorInstallationData)}");
            installationGUID = path.Length > 0 ? path[0] : string.Empty;
            return path.Length > 0;
        }

        static bool TryGetInstallationFolder(out string result)
        {
            if (!IsTextAnimatorInstalled(out var installationGUID))
            {
                Debug.LogError("Unable to locate Text Animator's Installation file. Please re-run the setup.");
                result = null;
                return false;
            }

            result = AssetDatabase.GUIDToAssetPath(installationGUID);
            result = result.Substring(0, result.LastIndexOf('/'));
            return true;
        }
        
        #endregion
        
        /// <summary>
        /// Creates built-in effects databases and assigns it to the settings file as default.
        /// </summary>
        /// <remarks>
        /// In case the settings file doesn't exist, it'll be created as well.
        /// In case the built-in effects or databases already exist, they'll be overwritten.
        /// </remarks>
        public static void ResetToBuiltIn()
        {
            //makes sure the asset is installed
            GetOrCreateInstallationData();
            if (!TryGetInstallationFolder(out string installationFolder))
            {
                Debug.LogError("Something went wrong in locating TextAnimator's installation data.");
                return;
            }
            
            var settings = GetOrCreateSettings(installationFolder);
            CreateDefaultDatabases(installationFolder, 
                out var beh,
                out var app,
                out var act,
                out var stylesheet);
            AssignDatabasesToSettings(settings, beh, app, act, stylesheet);
        }

        #region Databases and Tags

        const string fileName_stylesheet = "TextAnimator StyleSheet";
        
        /// <summary>
        /// Creates default effects and actions databases.
        /// </summary>
        /// <param name="installationFolder"></param>
        /// <param name="behaviors"></param>
        /// <param name="appearances"></param>
        /// <param name="actions"></param>
        /// <remarks>In case they already exist, they'll get overwritten.</remarks>
        static void CreateDefaultDatabases(string installationFolder, out AnimationsDatabase behaviors, out AnimationsDatabase appearances, out ActionDatabase actions, out StyleSheetScriptable styleSheet)
        {
            string progressTitle = "Text Animator";
            
            // --- DATABASES ---
            EditorUtility.DisplayProgressBar(progressTitle, "Creating Behaviors Database", 1/5f);
            behaviors = _CreateDatabase<AnimationsDatabase, AnimationScriptableBase>(installationFolder, "Behaviors", "Behaviors Database", EffectCategory.Behaviors);
           
            EditorUtility.DisplayProgressBar(progressTitle, "Creating Appearances Database", 2/5f);
            appearances = _CreateDatabase<AnimationsDatabase, AnimationScriptableBase>(installationFolder, "Appearances", "Appearances Database", EffectCategory.Appearances);
            
            EditorUtility.DisplayProgressBar(progressTitle, "Creating Actions Database", 3/5f);
            actions = _CreateDatabase<ActionDatabase, ActionScriptableBase>(installationFolder, "Actions", "Actions Database", EffectCategory.None);

            EditorUtility.DisplayProgressBar(progressTitle, "Creating Default Style Sheet",4/5f);
            styleSheet = CreateStyleSheet(installationFolder);
            
            AssetDatabase.SaveAssets();
            EditorUtility.ClearProgressBar();
        }
        
        public static DatabaseType _CreateDatabase<DatabaseType, ElementType>(string installationFolder, string folderName, string fileName, EffectCategory category)
            where DatabaseType : Database<ElementType> where ElementType : ScriptableObject, ITagProvider
        {
            string databasePath = installationFolder + $"/{folderName}";

            var database = _CreateScriptableAssetAtPath<DatabaseType>(databasePath, fileName);
            var types = GetScriptableElementsFromAssembly<ElementType>();
            for (var i = 0; i < types.Length; i++)
            {
                if (TryCreatingDefaultTagScriptable(types[i], category, out var so))
                {
                    PlaceScriptableAtPath(so, databasePath);
                    database.Add(so as ElementType);
                }
            }
            EditorUtility.SetDirty(database);
            SerializedObject dat = new SerializedObject(database);
            dat.ApplyModifiedProperties();
            return database;
        }

        static bool TryCreatingDefaultTagScriptable(System.Type type, EffectCategory category,  out ScriptableObject result) 
        {
            var attribute = type.GetCustomAttributes(typeof(TagInfoAttribute), true).FirstOrDefault() as TagInfoAttribute;
            if (attribute == null)
            {
                //skips default elements that do not have EffectInfo attribute
                result = default;
                return false;
            }

            //skips empty tags by design, e.g. Composite animation
            if (string.IsNullOrEmpty(attribute.tagID))
            {
                result = default;
                return false;
            }

            if (attribute is EffectInfoAttribute effectInfo)
            {
                if (!effectInfo.category.HasFlag(category))
                {
                    result = default;
                    return false;
                }
            }

            var so = CreateInstance(type);
            so.name = type.Name;

            //changes scriptable field based on default value attributes
            var defaultValueAttributes =
                type.GetCustomAttributes(typeof(DefaultValueAttribute), true) as DefaultValueAttribute[];
                    
            SerializedObject serializedSo = new SerializedObject(so);
            var tagID = serializedSo.FindProperty("tagID");
                    
            if (defaultValueAttributes != null)
            {
                foreach (var info in defaultValueAttributes)
                {
                    serializedSo.FindProperty(info.variableName).floatValue = info.variableValue;
                }
            }

            tagID.stringValue = attribute.tagID;
            serializedSo.ApplyModifiedProperties();
            result = so;
            return true;
        }

        
        #endregion

        
        #region Settings

        static void AssignDatabasesToSettings(TextAnimatorSettings settings, AnimationsDatabase behaviorsDatabase,
            AnimationsDatabase appearanceDatabase, ActionDatabase actionsDatabase, StyleSheetScriptable stylesheet)
        {
            SerializedObject serialized = new SerializedObject(settings);
            serialized.FindProperty(nameof(settings.actions))
                .FindPropertyRelative(nameof(settings.actions.defaultDatabase)).objectReferenceValue = actionsDatabase;

            serialized.FindProperty(nameof(settings.behaviors))
                    .FindPropertyRelative(nameof(settings.behaviors.defaultDatabase)).objectReferenceValue =
                behaviorsDatabase;

            serialized.FindProperty(nameof(settings.appearances))
                    .FindPropertyRelative(nameof(settings.appearances.defaultDatabase)).objectReferenceValue =
                appearanceDatabase;

            serialized.FindProperty(nameof(settings.defaultStyleSheet)).objectReferenceValue = stylesheet;

            serialized.ApplyModifiedProperties();
            serialized.Update();
        }
        
        
        static TextAnimatorSettings GetOrCreateSettings(string installationFolder)
        {
            if(TextAnimatorSettings.Instance)
                return TextAnimatorSettings.Instance;
                
            return _CreateScriptableAssetAtPath<TextAnimatorSettings>(installationFolder + "/Resources", TextAnimatorSettings.expectedName);
        }



        /// <summary>
        /// Creates a new settings file (with databases) inside the installation's Resources folder.
        /// Also assigns default effects and actions databases,
        /// either looking if they already exist or by creating new ones.
        /// </summary>
        /// <remarks>
        /// P.S. A new settings file will be created even if it already exists but in another folder,
        /// as the user might have moved it for archive/backups purposes.
        /// </remarks>
        public static void FixSettingsFileNotFound()
        {
            GetOrCreateInstallationData();
            if (!TryGetInstallationFolder(out string installationFolder))
            {
                Debug.LogError("Something went wrong in locating TextAnimator's installation data.");
                return;
            }
            
            DatabaseType GetOrCreateDatabase<DatabaseType, ElementType>(string folderName, string fileName, EffectCategory category)
                where DatabaseType : Database<ElementType> where ElementType : ScriptableObject, ITagProvider
            {
                string databaseGuid = AssetDatabase.FindAssets($"t:{nameof(DatabaseType)}").FirstOrDefault();

                if (!string.IsNullOrEmpty(databaseGuid))
                {
                    return AssetDatabase.LoadAssetAtPath<DatabaseType>(AssetDatabase.GUIDToAssetPath(databaseGuid));
                }

                // tries creating new database
                if (TryGetInstallationFolder(out installationFolder))
                {
                    return _CreateDatabase<DatabaseType, ElementType>(installationFolder, folderName, fileName, category);
                }

                return null;
            }

            StyleSheetScriptable GetOrCreateStylesheet()
            {
                string databaseGuid = AssetDatabase.FindAssets($"t:{nameof(StyleSheetScriptable)}").FirstOrDefault();

                if (!string.IsNullOrEmpty(databaseGuid))
                {
                    return AssetDatabase.LoadAssetAtPath<StyleSheetScriptable>(AssetDatabase.GUIDToAssetPath(databaseGuid));
                }

                // tries creating new database
                if (TryGetInstallationFolder(out installationFolder))
                {
                    return CreateStyleSheet(installationFolder);
                }

                return null;
            }

            var settings = GetOrCreateSettings(installationFolder);
            AssignDatabasesToSettings(settings,
                GetOrCreateDatabase<AnimationsDatabase, AnimationScriptableBase>("Behaviors", "Behaviors Database", EffectCategory.Behaviors),
                GetOrCreateDatabase<AnimationsDatabase, AnimationScriptableBase>("Appearances",
                    "Appearances Database", EffectCategory.Appearances),
                GetOrCreateDatabase<ActionDatabase, ActionScriptableBase>("Actions", "Actions Database", EffectCategory.None),
                GetOrCreateStylesheet());
        }

        static StyleSheetScriptable CreateStyleSheet(string installationFolder)
        {
            var result = _CreateScriptableAssetAtPath<StyleSheetScriptable>(installationFolder + $"/Styles", fileName_stylesheet);
            EditorUtility.SetDirty(result);
            return result;
        }

        #endregion

        #region Updating to new versions

        /// <summary>
        /// Checks for stuff and updates some project files if needed
        /// </summary>
        /// <param name="installationData"></param>
        static bool UpdateProject(TextAnimatorInstallationData installationData, Version oldVersion, bool performUpdate)
        {
            if (!TryGetInstallationFolder(out string installationFolder))
            {
                return false;
            }

            bool shouldUpdate = false;
            if (!string.IsNullOrEmpty(installationData.latestVersion))
            {
                // 2.1.0 added Style Sheets
                if (oldVersion < new Version(2, 1, 0))
                {
                    if (performUpdate && TextAnimatorSettings.Instance)
                    {
                        var styleSheet = CreateStyleSheet(installationFolder);
                        TextAnimatorSettings.Instance.defaultStyleSheet = styleSheet;
                        EditorUtility.SetDirty(TextAnimatorSettings.Instance);
                    }

                    shouldUpdate = true;
                }
            }

            installationData.latestVersion = currentVersion;
            EditorUtility.SetDirty(installationData);
            return shouldUpdate;
        }

        #endregion

        #region Utilties

        public static void _CreateDirectoryIfDoesntExist(string path)
        {
            string[] folders = path.Split('/');
            string subPath = folders[0];
            for (int i = 1; i < folders.Length; i++)
            {
                if (!AssetDatabase.IsValidFolder(subPath + '/' + folders[i]))
                    AssetDatabase.CreateFolder(subPath, folders[i]);
                subPath += $"/{folders[i]}";
            }
        }

        static Type[] GetScriptableElementsFromAssembly<T>() where T : ScriptableObject, ITagProvider
        {
            return System.AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(p => typeof(T).IsAssignableFrom(p) && !p.IsInterface && !p.IsAbstract)
                .Where(s => s.Assembly.FullName.StartsWith("Febucci.TextAnimator.Runtime"))
                .ToArray();
        }

        public static T _CreateScriptableAssetAtPath<T>(string path, string objectName) where T : ScriptableObject
        {
            var scriptable = CreateInstance<T>();
            scriptable.name = objectName;
            PlaceScriptableAtPath(scriptable, path);
            return scriptable;
        }

        static void PlaceScriptableAtPath(ScriptableObject scriptable, string path)
        {
            _CreateDirectoryIfDoesntExist(path);
            AssetDatabase.CreateAsset(scriptable, path + $"/{scriptable.name}.asset");
        }

        #endregion

    }
}