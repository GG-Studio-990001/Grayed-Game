using TMPro;
using UnityEngine;

namespace Runtime.CH3.Dancepace
{
    public class ResultPanel : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI scoreText;

        public void SetText(int score)
        {
            scoreText.text = score.ToString();
        }
    }
}