using UnityEngine;
using UnityEngine.UI;

namespace Runtime.ETC
{
    [RequireComponent(typeof(Button))]
    public class ToggleTargetButton : MonoBehaviour
    {
        [SerializeField] private GameObject _toggleTarget;
        private Button _button;

        private void Awake()
        {
            _button = GetComponent<Button>();
            _button.onClick.AddListener(OnToggle);
        }

        private void OnToggle()
        {
            _toggleTarget.SetActive(!_toggleTarget.activeSelf);
        }
    }
}