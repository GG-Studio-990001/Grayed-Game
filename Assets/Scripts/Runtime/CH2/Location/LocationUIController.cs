using Runtime.CH2.Main;
using Runtime.ETC;
using Runtime.InGameSystem;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Runtime.CH2.Location
{
    public class LocationUIController : MonoBehaviour
    {
        [NonSerialized] public TurnController TurnController;
        [SerializeField] private LocationBgController _locationBgController;
        [SerializeField] private FadeController _fadeController;
        [SerializeField] private Transform _locationOptions;
        [SerializeField] private GameObject _optionBtnPrefab;
        [SerializeField] private LocationUIAnimator _locationUIAnimator;

        public void MoveLocation() // FadeIn&Out
        {
            FadeOut();
            Invoke(nameof(StartLocation), 1f);
        }

        public void StartLocation() // Only FadeIn
        {
            _fadeController.SetFadeImg();
            Invoke(nameof(SetLocation), 0.5f);
            Invoke(nameof(FadeIn), 0.5f);
        }

        private void FadeOut()
        {
            _fadeController.StartFadeOut();
        }

        public void FadeIn()
        {
            _fadeController.StartFadeIn();
        }

        private void SetLocation()
        {
            _locationOptions.gameObject.SetActive(false);
            _locationBgController.SetLocationUI();
            _locationBgController.SetBG();
            _locationUIAnimator.AnimateUI();
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
                btn.interactable = true; // 비활 풀기
                int index = i; // Closure 문제 해결

                TextMeshProUGUI btnTxt = btn.GetComponentInChildren<TextMeshProUGUI>();
                if (_locationBgController.LocationTexts.TryGetValue(loc[index], out string locationText))
                {
                    btnTxt.text = locationText;
                }

                btn.onClick.AddListener(() =>
                {
                    DisableAllButtons(); // 클릭 시 모든 버튼 비활성화

                    Managers.Sound.Play(Sound.SFX, "CH2/SFX_Click");
                    TurnController.SetLocation(loc[index]);
                    TurnController.StartDialogue();
                });
            }
        }


        private void DisableAllButtons()
        {
            foreach (Transform child in _locationOptions)
            {
                Button btn = child.GetComponent<Button>();
                if (btn != null)
                {
                    btn.interactable = false; // 버튼 비활성화
                }
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