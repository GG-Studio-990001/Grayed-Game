using UnityEngine;

namespace Febucci.UI
{
    [System.Serializable]
    internal class TextAnimatorInstallationData : ScriptableObject
    {
        [SerializeField] internal string latestVersion = "None"; //stores the latest version
    }
}