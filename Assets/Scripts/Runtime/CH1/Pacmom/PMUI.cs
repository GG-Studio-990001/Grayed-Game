using Runtime.ETC;
using System.Collections;
using TMPro;
using UnityEngine;

namespace Runtime.CH1.Pacmom
{
    public class PMUI : MonoBehaviour
    {
        public PMData Data;
        [SerializeField] private Blink _blink;
        [SerializeField] private TextMeshProUGUI _feildCoinTxt;
        [SerializeField] private TextMeshProUGUI _pacmomScoreTxt;
        [SerializeField] private TextMeshProUGUI _rapleyScoreTxt;

        public void ShowRestartTxt()
        {
            _blink.enabled = true;
        }

        public void ShowRemainingCoins(int coins)
        {
            _feildCoinTxt.text = coins + " coins left";
        }    

        public void ShowPacmomScore(int newScore)
        {
            StartCoroutine(nameof(ChangePacmomScore), newScore);
        }

        public void ShowRapleyScore(int newScore)
        {
            StartCoroutine(nameof(ChangeRapleyScore), newScore);
        }

        private IEnumerator ChangePacmomScore(int newScore)
        {
            string scoreStr = _pacmomScoreTxt.text[1..];
            int score = int.Parse(scoreStr);
            float changeTime = Data.GetChangeScoreTime(newScore - score);

            while (score != newScore)
            {
                score += (score < newScore ? 1 : -1);
                _pacmomScoreTxt.text = "x" + score.ToString();
                yield return new WaitForSeconds(changeTime);
            }
        }

        private IEnumerator ChangeRapleyScore(int newScore)
        {
            string scoreStr = _rapleyScoreTxt.text[1..];
            int score = int.Parse(scoreStr);
            float changeTime = Data.GetChangeScoreTime(newScore - score);

            while (score != newScore)
            {
                score += (score < newScore ? 1 : -1);
                _rapleyScoreTxt.text = "x" + score.ToString();
                yield return new WaitForSeconds(changeTime);
            }
        }
    }
}