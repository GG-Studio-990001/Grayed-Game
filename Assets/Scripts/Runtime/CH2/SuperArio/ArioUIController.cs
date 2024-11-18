using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Runtime.CH2.SuperArio
{
    public class ArioUIController : MonoBehaviour
    {
        [SerializeField] private TMP_Text _restartText;
        [SerializeField] private TMP_Text _stageText;
        [SerializeField] private TMP_Text _coinText;
        [SerializeField] private TMP_Text _obstacleText;
        [SerializeField] private Image _itemImg;
        [SerializeField] private ArioHeartsUI _hearts;

        public void ActiveRestartText(bool isRestart)
        {
            if(isRestart)
                _restartText.gameObject.SetActive(true);
            else
                _restartText.gameObject.SetActive(false);
        }
    
        public void ChangeHeartUI(int count)
        {
            _hearts.ChangeHeartUI(count);
        }

        public void ChangeStageText(string text)
        {
            _stageText.text = text;
        }

        public void ChangeCoinText(string text)
        {
            _coinText.text = text;
        }

        public void ChangeObstacleText(int count)
        {
            _obstacleText.text = count.ToString();
        }

        public void ChangeItemSprite(bool isUse)
        {
            _itemImg.enabled = !isUse;
        }
    }
}

