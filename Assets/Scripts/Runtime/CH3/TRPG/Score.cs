using TMPro;
using UnityEngine;

namespace Runtime.CH3.TRPG
{
    public class Score : MonoBehaviour
    {
        [SerializeField] private DiceRoll _dice;
        [SerializeField] private TextMeshProUGUI _scoreText;

        private void Update()
        {
            if (_dice.DiceFaceNum != -1)
                _scoreText.text = _dice.DiceFaceNum.ToString();
        }
    }
}