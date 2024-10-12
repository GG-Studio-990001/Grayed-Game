using UnityEngine;
using UnityEngine.UI;

namespace Runtime.Common
{
    public class SettingUITab : MonoBehaviour
    {
        [SerializeField] private Image[] _tabs;
        [SerializeField] private Sprite[] _tabSprs;
        [SerializeField] private GameObject[] _panels;
        private bool _isSoundSelected = true;

        private void Start()
        {
            _tabs[1].gameObject.SetActive(Managers.Data.CH1.TranslatorCount != 0);
        }

        public void ShowTransTab()
        {
            _tabs[1].gameObject.SetActive(true);
        }

        public void ActiveOption()
        {
            if (_isSoundSelected)
                return;

            SelectTab(0);
        }

        public void ActiveTrans()
        {
            if (!_isSoundSelected)
                return;

            SelectTab(1);
        }

        private void SelectTab(int idx)
        {
            _isSoundSelected = idx == 0;
            ActiveTab(idx);
            InactiveTab(1 - idx);
        }

        private void ActiveTab(int idx)
        {
            _tabs[idx].sprite = _tabSprs[1];
            SetPos(idx, true);
            _panels[idx].SetActive(true);
        }

        private void InactiveTab(int idx)
        {
            _tabs[idx].sprite = _tabSprs[0];
            SetPos(idx, false);
            _panels[idx].SetActive(false);
        }

        private void SetPos(int idx, bool selected)
        {
            Vector2 pos = _tabs[idx].rectTransform.anchoredPosition;
            pos.x += selected ? -30 : 30;
            _tabs[idx].rectTransform.anchoredPosition = pos;
        }
    }
}