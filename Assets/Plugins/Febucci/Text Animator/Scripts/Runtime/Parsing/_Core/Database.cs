using System.Collections.Generic;

namespace Febucci.UI.Core
{
    /// <summary>
    /// Caches information about tag providers, so that
    /// it's easier to access them
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [System.Serializable]
    public class Database<T> : UnityEngine.ScriptableObject where T : UnityEngine.ScriptableObject, ITagProvider
    {
        bool built;

        void OnEnable()
        {
            //Prevents database from not refreshing on
            //different domain reload settings
            built = false;
        }

        [UnityEngine.SerializeField] System.Collections.Generic.List<T> data = new List<T>(); 
        public System.Collections.Generic.List<T> Data => data;

        public void Add(T element)
        {
            if(data == null) data = new System.Collections.Generic.List<T>();
            data.Add(element);

            // at runtime adds directly on database as well, without needing to rebuild
            if (built && UnityEngine.Application.isPlaying)
            {
                string tag = element.TagID;
                if (dictionary.ContainsKey(tag))
                    UnityEngine.Debug.LogError($"Text Animator: Tag {tag} is already present in the database. Skipping...");
                else
                    dictionary.Add(tag, element);
            }
            else
            {
                built = false;
            }
        }

        Dictionary<string, T> dictionary;

        public void ForceBuildRefresh()
        {
            built = false;
            BuildOnce();
        }

        public void BuildOnce()
        {
            if(built) return;
            built = true;

            if(dictionary == null)
                dictionary = new Dictionary<string, T>();
            else
                dictionary.Clear();

            string tagId;
            foreach (var source in data)
            {
                if(!source)
                    continue;
                
                tagId = source.TagID;

                if (string.IsNullOrEmpty(tagId))
                {
                    UnityEngine.Debug.LogError($"Text Animator: Tag is null or empty. Skipping...");
                    continue;
                }
                
                if (dictionary.ContainsKey(tagId))
                {
                    UnityEngine.Debug.LogError($"Text Animator: Tag {tagId} is already present in the database. Skipping...");
                    continue;
                }

                dictionary.Add(tagId, source);
            }
            
            OnBuildOnce();
        }

        protected virtual void OnBuildOnce() { }

        public bool ContainsKey(string key)
        {
            BuildOnce();
            return dictionary.ContainsKey(key);
        }

        public T this[string key]
        {
            get
            {
                BuildOnce();
                return dictionary[key];
            }
        }

        public void DestroyImmediate(bool databaseOnly = false)
        {
            if (!databaseOnly)
            {
                foreach (var element in data)
                {
                    UnityEngine.Object.DestroyImmediate(element);
                }
            }

            UnityEngine.Object.DestroyImmediate(this);
        }
    }
}