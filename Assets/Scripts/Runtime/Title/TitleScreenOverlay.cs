using UnityEngine;

namespace Runtime.CH1.Title
{
    public class TitleScreenOverlay : MonoBehaviour
    {
        // 캔버스 사이즈가 조정되는 잠깐의 시간동안 화면 가려주는 클래스

        [SerializeField] private GameObject _blackImg;
        [SerializeField] private GameObject _versionTxt;
        [SerializeField] private GameObject[] _particles;

        private void OnEnable()
        {
            SetTitleVisibility(false);

            Invoke(nameof(ShowTitle), 0.2f);
        }

        private void ShowTitle()
        {
            SetTitleVisibility(true);
        }

        private void SetTitleVisibility(bool show)
        {
            _blackImg.SetActive(!show);
            _versionTxt.SetActive(show);
            foreach (GameObject dust in _particles)
                dust.SetActive(show);
        }
    }
}