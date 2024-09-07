using System.Collections;
using UnityEngine;

namespace Runtime.CH1.Pacmom
{
    public class PMData : MonoBehaviour
    {
        #region 선언
        private PMController _controller;

        [Header("=Contoller=")]
        [SerializeField] private PMUI _ui;
        [SerializeField] private PMEnding _ending;

        [Header("=Item=")]
        [SerializeField] private Transform _coins;
        [SerializeField] private Transform _vacuums;

        private int _totalCoins;
        private int _rapleyScore;
        private int _pacmomScore;
        private readonly float _normalWaitTime = 0.03f;
        private readonly float _fasterWaitTime = 0.02f;
        #endregion

        #region Awake & Start
        private void Awake()
        {
            _controller = GetComponent<PMController>();
            _ui.Data = this;

            foreach (Transform coin in _coins)
            {
                coin.GetComponent<Coin>().Controller = _controller;
            }

            foreach (Transform vacuum in _vacuums)
            {
                vacuum.GetComponent<Vacuum>().Controller = _controller;
            }
        }

        private void Start()
        {
            _totalCoins = GetFieldCoins();
            SetRapleyScore(0);
            SetPacmomScore(0);
        }
        #endregion

        #region Score
        private void SetFeildCoin(int coins)
        {
            _totalCoins = coins;
            _ui.ShowRemainingCoins(coins);
        }

        private void SetRapleyScore(int score)
        {
            _rapleyScore = score;
            _ui.ShowRapleyScore(score);
        }

        private void SetPacmomScore(int score)
        {
            _pacmomScore = score;
            _ui.ShowPacmomScore(score);
        }

        public void RapleyScore1Up()
        {
            SetRapleyScore(_rapleyScore + 1);
            SetFeildCoin(_totalCoins - 1);
        }

        public void PacmomScore1Up()
        {
            SetPacmomScore(_pacmomScore + 1);
            SetFeildCoin(_totalCoins - 1);
        }
        #endregion

        #region About Coins
        public void TakeHalfCoins()
        {
            int score = _rapleyScore / 2;
            SetRapleyScore(_rapleyScore - score);
            SetPacmomScore(_pacmomScore + score);
        }

        public float GetChangeScoreTime(int diff)
        {
            diff = Mathf.Abs(diff);

            if (diff >= 50)
                return _fasterWaitTime;
            else
                return _normalWaitTime;
        }

        public void ChooseAWinner()
        {
            StartCoroutine(nameof(VictoryOrDefeat));
        }

        private IEnumerator VictoryOrDefeat()
        {
            yield return new WaitForSeconds(1f);

            Managers.Sound.StopSFX();
            _controller.DialogueStop();

            if (_rapleyScore > _pacmomScore)
            {
                _ending.RapleyWin(_rapleyScore);
            }
            else
            {
                _ending.PacmomWin();
            }
        }

        public int GetFieldCoins()
        {
            int coins = 0;
            foreach (Transform coin in _coins)
            {
                if (coin.gameObject.activeSelf)
                {
                    coins++;
                }
            }
            return coins;
        }

        public bool HasRemainingCoins()
        {
            foreach (Transform coin in _coins)
            {
                if (coin.gameObject.activeSelf)
                {
                    return true;
                }
            }
            return false;
        }
        #endregion
    }
}