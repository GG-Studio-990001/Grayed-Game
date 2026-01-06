using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Runtime.ETC;

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
        [SerializeField] private Image _resultImg;
        [SerializeField] private Sprite[] _resultSprs;
        // private string characterName = "Dollar";

        private readonly Dictionary<Stat, int> _myStats = new()
        {
            { Stat.NONE, 0 },
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
            // _resultTxt_0.text = "";
            _resultTxt_1.text = "";
            // _resultTxt_2.text = "";
            _resultImg.gameObject.SetActive(false);
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

            int targetScore = (int)ReturnTargetScore(statVal, difficulty);
            // 스탯: 난이도: 목표: 
            // _resultTxt_0.text = $"스탯: {stat} / 난이도: {difficulty}\n목표: {targetScore} 이하";
            _dice10.RollDice();
            while (_dice10.DiceFaceNum == -1) yield return null;
            int d10 = _dice10.DiceFaceNum;
            _resultTxt_1.text = $"{d10} + __ = ___";
            Managers.Sound.Play(Sound.SFX, "CH3/CoC/CH3_SFX_CoC_Number");

            _dice1.RollDice();
            while (_dice1.DiceFaceNum == -1) yield return null;
            int d1 = _dice1.DiceFaceNum;
            _resultTxt_1.text = $"{d10} + {d1} = ___";
            Managers.Sound.Play(Sound.SFX, "CH3/CoC/CH3_SFX_CoC_Number");

            int total = d10 + d1;
            total = (total == 0) ? 100 : total;

            yield return new WaitForSeconds(1f);
            _resultTxt_1.text = $"{d10} + {d1} = {total}";
            Managers.Sound.Play(Sound.SFX, "CH3/CoC/CH3_SFX_CoC_Number");

            DiceResult diceResult = EvaluateDiceResult(total, statVal, difficulty);
            ResultVal resultKey = diceResult.Result;
            Debug.Log(resultKey);

            yield return new WaitForSeconds(1f);
            // _resultTxt_2.text = "결과: " + resultKey;
            _resultImg.gameObject.SetActive(true);
            int resultIdx = (int)diceResult.Result;
            _resultImg.sprite = _resultSprs[resultIdx];
            switch (resultIdx)
            {
                case 0:
                     Managers.Sound.Play(Sound.SFX, "CH3/CoC/CH3_SFX_CoC_Critical");
                    Debug.Log("대성공");
                    break;
                case 1:
                    Managers.Sound.Play(Sound.SFX, "CH3/CoC/CH3_SFX_CoC_Success");
                    Debug.Log("성공");
                    break;
                case 2:
                    Managers.Sound.Play(Sound.SFX, "CH3/CoC/CH3_SFX_CoC_Fail");
                    break;
                default:
                    Managers.Sound.Play(Sound.SFX, "CH3/CoC/CH3_SFX_CoC_Fumble");
                    break;
            }

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
                // 목표 점수 계산
                float targetScore = ReturnTargetScore(statValue, difficulty);

                isSuccess = total <= targetScore;
                result = isSuccess ? ResultVal.Success : ResultVal.Fail;
            }

            return new DiceResult
            {
                Sum = total,
                Result = result,
                IsSuccess = isSuccess
            };
        }

        public int GetTargetScore(Stat stat, Difficulty difficulty)
        {
            if (stat == Stat.NONE) return 0;
            return Mathf.FloorToInt(ReturnTargetScore(_myStats[stat], difficulty));
        }

        float ReturnTargetScore(int statValue, Difficulty difficulty)
        {
            switch (difficulty)
            {
                case Difficulty.EXTREME:
                    return statValue / 5;
                case Difficulty.HARD:
                    return statValue / 2;
                case Difficulty.NORMAL:
                    return statValue;
                default:
                    return statValue;
            }
        }
    }
}