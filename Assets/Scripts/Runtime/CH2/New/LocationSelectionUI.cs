using Runtime.CH2.Main;
using Runtime.InGameSystem;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Runtime.CH2.Location
{
    public class LocationSelectionUI2 : MonoBehaviour
    {
        // 클래스명 서로 바뀜...
        [NonSerialized] public TurnController2 TurnController;
        [SerializeField] private BGSpriteSwitcher _bgSpriteSwitcher;
        [SerializeField] private FadeController _fadeController;
        [SerializeField] private TextMeshProUGUI _locationTxt;
        [SerializeField] private Transform _locationOptions;
        [SerializeField] private GameObject _optionBtnPrefab;

        public void MoveLocation()
        {
            FadeOut();
            Invoke(nameof(FadeIn), 1f);
        }

        private void FadeOut()
        {
            _locationOptions.gameObject.SetActive(false);
            _fadeController.StartFadeOut();
        }

        public void FadeIn()
        {
            SetLocationTxt();
            _bgSpriteSwitcher.SetBG();
            _fadeController.StartFadeIn();
        }

        private void SetLocationTxt()
        {
            _locationTxt.text = Managers.Data.CH2.Location;
        }

        public void SetLocationOptions(List<string> loc)
        {
            MakeOptions(loc.Count);
            _locationOptions.gameObject.SetActive(true);

            for (int i = 0; i < loc.Count; i++)
            {
                if (i >= _locationOptions.childCount)
                {
                    Debug.LogError("Not enough buttons created.");
                    return;
                }

                Button btn = _locationOptions.GetChild(i).GetComponent<Button>();
                btn.onClick.RemoveAllListeners();
                int index = i;  // Closure 문제 해결
                btn.onClick.AddListener(() => TurnController.AdvanceTurnAndMoveLocation(loc[index]));
                btn.onClick.AddListener(() => _fadeController.StartFadeOut());

                TextMeshProUGUI btnTxt = btn.GetComponentInChildren<TextMeshProUGUI>();
                btnTxt.text = loc[index];
            }
        }

        private void MakeOptions(int cnt)
        {
            int currentCount = _locationOptions.childCount;

            if (currentCount == cnt)
                return;

            if (currentCount > cnt)
            {
                for (int i = currentCount - 1; i >= cnt; i--)
                {
                    Destroy(_locationOptions.GetChild(i).gameObject);
                }
            }
            else
            {
                for (int i = 0; i < cnt - currentCount; i++)
                {
                    Instantiate(_optionBtnPrefab, _locationOptions);
                }
            }
        }
    }
}