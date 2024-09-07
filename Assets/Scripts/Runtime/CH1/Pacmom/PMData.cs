using System.Collections;
using UnityEngine;

namespace Runtime.CH1.Pacmom
{
    public class PMData : MonoBehaviour
    {
        #region 선언
        private PMController _gameController;

        [Header("=Contoller=")]
        [SerializeField] private PMUI _ui;
        [SerializeField] private PMEnding _ending;

        [Header("=Item=")]
        [SerializeField] private Transform _coins;
        [SerializeField] private Transform _vacuums;

        private int _rapleyScore;
        private int _pacmomScore;
        private readonly float _normalWaitTime = 0.03f;
        private readonly float _fasterWaitTime = 0.02f;
        #endregion

        #region Awake & Start
        private void Awake()
        {
            _gameController = GetComponent<PMController>();
            _ui.DataController = this;

            foreach (Transform coin in _coins)
            {
                coin.GetComponent<Coin>().GameController = _gameController;
            }

            foreach (Transform vacuum in _vacuums)
            {
                vacuum.GetComponent<Vacuum>().GameController = _gameController;
            }
        }

        private void Start()
        {
            SetRapleyScore(0);
            SetPacmomScore(0);
        }
        #endregion

        #region Score & Lives
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
        }

        public void PacmomScore1Up()
        {
            SetPacmomScore(_pacmomScore + 1);
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
            _gameController.DialogueStop();

            if (_rapleyScore > _pacmomScore)
            {
                _ending.RapleyWin(_rapleyScore);
            }
            else
            {
                _ending.PacmomWin();
            }
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