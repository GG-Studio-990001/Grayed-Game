using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Runtime.CH3.TRPG
{
    public enum Difficulty
    {
        None,
        Normal,
        Hard,
        Extreme
    }

    public enum ResultGrade
    {
        BigSuccess,
        Success,
        Fail,
        BigFail
    }

    public struct DiceResult
    {
        public int Sum;
        public ResultGrade Grade;
        public bool IsSuccess;
    }

    public class Stat
    {
        public string Name { get; }
        public int Value { get; }

        public Stat(string name, int value)
        {
            Name = name;
            Value = value;
        }
    }

    public class TrpgLine : MonoBehaviour
    {
        [Header("=Result=")]
        [SerializeField] private GameObject _resultPanel;
        [SerializeField] private GameObject _diceRollObjects;
        [SerializeField] private DiceRoll _dice10;
        [SerializeField] private DiceRoll _dice1;
        [SerializeField] private TextMeshProUGUI _resultTxt_0;
        [SerializeField] private TextMeshProUGUI _resultTxt_1;
        [SerializeField] private TextMeshProUGUI _resultTxt_2;
        private string characterName = "Dollar";

        private readonly Dictionary<string, Stat> _stats = new()
        {
            { "Stamina", new Stat("Stamina", 20) },
            { "Strength", new Stat("Strength", 25) },
            { "Intelligence", new Stat("Intelligence", 30) },
            { "Mana", new Stat("Mana", 35) },
            { "Charisma", new Stat("Charisma", 40) },
            { "Luck", new Stat("Luck", 45) }
        };

        public void StartRoll(string type, Difficulty difficulty, int val, Action<string> onComplete)
        {
            if (_stats.TryGetValue(type, out var stat))
            {
                StartCoroutine(RollDiceRoutine(stat, difficulty, val, onComplete));
            }
            else
            {
                Debug.LogError($"Unknown stat type: {type}");
            }
        }

        private IEnumerator RollDiceRoutine(Stat stat, Difficulty difficulty, int val, Action<string> onComplete)
        {
            _resultPanel.SetActive(true);
            _diceRollObjects.SetActive(true);

            _resultTxt_0.text = stat.Name + $" ({difficulty})\n{stat.Value} / 100";
            _dice10.RollDice();
            while (_dice10.DiceFaceNum == -1) yield return null;
            int d10 = _dice10.DiceFaceNum;
            _resultTxt_1.text = $"{d10} + __ = ___";

            _dice1.RollDice();
            while (_dice1.DiceFaceNum == -1) yield return null;
            int d1 = _dice1.DiceFaceNum;
            _resultTxt_1.text = $"{d10} + {d1} = ___";

            int total = d10 + d1;
            total = (total == 0) ? 100 : total;

            yield return new WaitForSeconds(1f);
            _resultTxt_1.text = $"{d10} + {d1} = {total}";

            DiceResult result = EvaluateDiceResult(total, stat.Value, difficulty);
            string resultKey = $"{characterName}_{val / 10}_{val % 10}_{result.Grade}";

            yield return new WaitForSeconds(1f);
            _resultTxt_2.text = "결과: " + result.Grade;

            yield return new WaitForSeconds(2f);
            _resultPanel.SetActive(false);
            _diceRollObjects.SetActive(false);

            onComplete?.Invoke(resultKey);
        }

        private DiceResult EvaluateDiceResult(int total, int statValue, Difficulty difficulty)
        {
            ResultGrade grade;
            bool isSuccess = false;

            if (total == 1)
            {
                grade = ResultGrade.BigSuccess;
                isSuccess = true;
            }
            else if (total >= 97)
            {
                grade = ResultGrade.BigFail;
                isSuccess = false;
            }
            else
            {
                // 세부 성공 판정
                bool extreme = total <= statValue / 5;
                bool hard = total <= statValue / 2;
                bool normal = total <= statValue;

                isSuccess = (difficulty switch
                {
                    Difficulty.Extreme => extreme,
                    Difficulty.Hard => hard || extreme,
                    Difficulty.Normal => normal || hard || extreme,
                    _ => false
                });

                grade = isSuccess ? ResultGrade.Success : ResultGrade.Fail;
            }

            return new DiceResult
            {
                Sum = total,
                Grade = grade,
                IsSuccess = isSuccess
            };
        }
    }
}