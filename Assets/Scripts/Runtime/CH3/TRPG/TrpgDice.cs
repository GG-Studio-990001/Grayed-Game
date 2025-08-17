using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Runtime.CH3.TRPG
{
    public enum Stat
    {
        NONE, // 없음
        STR,  // 근력
        INT,  // 지능
        LUK,  // 행운
        CHA,  // 매력
        HP,   // 체력
        MP    // 마력
    }

    public enum Difficulty
    {
        NONE,     // 없음
        EASY,     // 쉬움
        NORMAL,   // 보통 성공
        HARD,     // 어려운 성공
        EXTREME   // 극단적 성공
    }

    public enum ResultVal
    {
        BigSuccess,
        Success,
        Fail,
        BigFail
    }

    public struct DiceResult
    {
        public int Sum;
        public ResultVal Result;
        public bool IsSuccess;
    }

    public class TrpgDice : MonoBehaviour
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

        private readonly Dictionary<Stat, int> _myStats = new()
        {
            { Stat.NONE, 0 }, // 체력
            { Stat.STR, 60 }, // 근력
            { Stat.INT, 55 }, // 지능
            { Stat.LUK, 50 }, // 행운
            { Stat.CHA, 45 }, // 매력
            { Stat.HP, 40 }, // 체력
            { Stat.MP, 35 }, // 마력
        };

        private void InitResult()
        {
            _dice10.DiceInit();
            _dice1.DiceInit();
            _resultTxt_0.text = "";
            _resultTxt_1.text = "";
            _resultTxt_2.text = "";
        }

        public void StartRoll(Stat stat, Difficulty difficulty, Action<ResultVal> onComplete)
        {
            if (stat == Stat.NONE)
            {
                onComplete?.Invoke(ResultVal.Success);
                return;
            }

            StartCoroutine(RollDiceRoutine(stat, difficulty, onComplete));
        }

        private IEnumerator RollDiceRoutine(Stat stat, Difficulty difficulty, Action<ResultVal> onComplete)
        {
            int statVal = _myStats[stat];

            InitResult();
            _resultPanel.SetActive(true);
            _diceRollObjects.SetActive(true);

            _resultTxt_0.text = stat.ToString() + $" ({difficulty})\n{statVal} / 100";
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

            DiceResult diceResult = EvaluateDiceResult(total, statVal, difficulty);
            ResultVal resultKey = diceResult.Result;
            Debug.Log(resultKey);

            yield return new WaitForSeconds(1f);
            _resultTxt_2.text = "결과: " + resultKey;

            yield return new WaitForSeconds(2f);
            _resultPanel.SetActive(false);
            _diceRollObjects.SetActive(false);

            onComplete?.Invoke(resultKey);
        }

        private DiceResult EvaluateDiceResult(int total, int statValue, Difficulty difficulty)
        {
            ResultVal result;
            bool isSuccess = false;

            if (total == 1)
            {
                result = ResultVal.BigSuccess;
            }
            else if (total >= 97)
            {
                result = ResultVal.BigFail;
            }
            else
            {
                // 세부 성공 판정
                bool extreme = total <= statValue / 5;
                bool hard = total <= statValue / 2;
                bool normal = total <= statValue;

                isSuccess = (difficulty switch
                {
                    Difficulty.EXTREME => extreme,
                    Difficulty.HARD => hard || extreme,
                    Difficulty.NORMAL => normal || hard || extreme,
                    _ => false
                });

                result = isSuccess ? ResultVal.Success : ResultVal.Fail;
            }

            return new DiceResult
            {
                Sum = total,
                Result = result,
                IsSuccess = isSuccess
            };
        }
    }
}