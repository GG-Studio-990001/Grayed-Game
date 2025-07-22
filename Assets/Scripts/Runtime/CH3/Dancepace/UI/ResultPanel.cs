using TMPro;
using UnityEngine;

namespace Runtime.CH3.Dancepace
{
    public class ResultPanel : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI scoreText;
        [SerializeField] private TextMeshProUGUI gradeText;
        [SerializeField] private TextMeshProUGUI perfectText;
        [SerializeField] private TextMeshProUGUI greatText;
        [SerializeField] private TextMeshProUGUI badText;

        public void SetText(int score, int perfect, int great, int bad)
        {
            switch (score)
            {
                case >= 15:
                    gradeText.text = "S";
                    break;
                case >= 10:
                    gradeText.text = "A";
                    break;
                case >= 5:
                    gradeText.text = "B";
                    break;
                default:
                    gradeText.text = "F";
                    break;
            }

            scoreText.text = score.ToString();
            perfectText.text = perfect.ToString();
            greatText.text = great.ToString();
            badText.text = bad.ToString();
        }
    }
}