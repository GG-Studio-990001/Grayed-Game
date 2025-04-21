using System.Collections;
using TMPro;
using UnityEngine;
using Yarn.Unity;

namespace Runtime.CH3.TRPG
{
    public class TrpgDialogue : MonoBehaviour
    {
        [Header("=Script=")]
        [SerializeField] private DialogueRunner _runner;
        [SerializeField] private GameObject _resultPanel;
        [SerializeField] private TextMeshProUGUI _resultTxt;

        private void Awake()
        {
            _runner.AddCommandHandler<int>("RollDice", RollDice);
        }

        private void RollDice(int val)
        {
            StartCoroutine(ShowResult(val));
        }

        private IEnumerator ShowResult(int val)
        {
            if (val == 1)
            {
                yield return null;
                _runner.StartDialogue($"Warlocker_1_{val}");
                yield break;
            }
            
            _resultPanel.SetActive(true);

            string type = (val == 2 ? "근력" : "지능");
            int dice = Random.Range(1, 101); // 끝값 제외
            bool result = dice <= 40;
            string resultStr = (result ? "성공" : "실패");
            string resultL = (result ? "S" : "F");

            _resultTxt.text = type + ": 40\n";
            yield return new WaitForSeconds(1f);
            _resultTxt.text += "주사위 결과: " + dice;
            yield return new WaitForSeconds(1f);
            _resultTxt.text += "\n결과: " + resultStr;
            yield return new WaitForSeconds(3f);

            _resultPanel.SetActive(false);

            _runner.StartDialogue($"Warlocker_1_{val}_{resultL}");
        }
    }
}