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
        [SerializeField] private PMKeyBinder _keyBinder;

        [Header("=Item=")]
        [SerializeField] private Transform _coins;
        [SerializeField] private Transform _vacuums;

        public bool CanRapleyWin { get; private set; } = true;
        private int _remainingCoin;
        private int _rapleyCoin;
        private int _pacmomCoin;
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
            _remainingCoin = GetFieldCoins();
            SetRapleyCoin(0);
            SetPacmomCoin(0);
        }
        #endregion

        private void SetFeildCoin(int coins)
        {
            _remainingCoin = coins;
            _ui.ShowRemainingCoins(coins);
        }

        private void SetRapleyCoin(int coins)
        {
            _rapleyCoin = coins;
            _ui.ShowRapleyScore(coins);
        }

        private void SetPacmomCoin(int coins)
        {
            _pacmomCoin = coins;
            _ui.ShowPacmomScore(coins);

            if (CanRapleyWin && !CanWin())
            {
                CanRapleyWin = false;
                _ui.ShowRestartTxt();
                _keyBinder.ActiveRestart();
            }
        }

        public void RapleyCoin1Up()
        {
            SetRapleyCoin(_rapleyCoin + 1);
            SetFeildCoin(_remainingCoin - 1);
        }

        public void PacmomCoin1Up()
        {
            SetPacmomCoin(_pacmomCoin + 1);
            SetFeildCoin(_remainingCoin - 1);
        }

        public bool CanWin()
        {
            return (_rapleyCoin + _remainingCoin) > _pacmomCoin;
        }

        public void TakeHalfCoins()
        {
            int coins = _rapleyCoin / 2;
            SetRapleyCoin(_rapleyCoin - coins);
            SetPacmomCoin(_pacmomCoin + coins);
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

            if (_rapleyCoin > _pacmomCoin)
            {
                _ending.RapleyWin(_rapleyCoin);
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
    }
}