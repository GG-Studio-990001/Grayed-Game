using TMPro;
using UnityEngine;

namespace Runtime.ETC
{
    public class VersionReader : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _versionText;

        void Start()
        {
            ReadVersion();
        }

        private void ReadVersion()
        {
            string projectVersion = Application.version;
            _versionText.text = "v" + projectVersion;
            Debug.Log("Project Version: " + projectVersion);
        }
    }
}