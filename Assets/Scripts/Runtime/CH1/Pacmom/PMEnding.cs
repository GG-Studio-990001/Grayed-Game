using Runtime.ETC;
using Runtime.InGameSystem;
using System.Collections;
using TMPro;
using UnityEngine;

namespace Runtime.CH1.Pacmom
{
    public class PMEnding : MonoBehaviour
    {
        [SerializeField]
        private PMShader _postProcessing;
        [SerializeField]
        private SceneSystem _sceneSystem;
        [SerializeField]
        private GameObject _timeline3;
        [SerializeField]
        private SpriteControl[] _spriteControls = new SpriteControl[6];
        [SerializeField]
        private TextMeshProUGUI _resultCoinTxt;
        private SceneTransform _sceneTransform;

        public void RapleyWin(int reward)
        {
            GetRewardCoin(reward);
            SaveGameClear(true);

            Debug.Log("라플리 승리");
            _timeline3.SetActive(true);
        }

        private void GetRewardCoin(int finalScore)
        {
            _resultCoinTxt.text = "x" + finalScore.ToString();
            Managers.Data.PacmomCoin += finalScore;
        }

        public void PacmomWin()
        {
            SaveGameClear(false);
            
            Debug.Log("팩맘 승리");
            StartCoroutine(nameof(PacmomGameOver));
        }

        private void SaveGameClear(bool clear)
        {
            if (!Managers.Data.IsPacmomPlayed)
                Managers.Data.IsPacmomPlayed = true;

            Managers.Data.IsPacmomCleared = clear;

            Managers.Data.SaveGame();
        }

        public void GetAllNormalSprite()
        {
            for (int i = 0; i < _spriteControls.Length; i++)
            {
                _spriteControls[i].GetNormalSprite();
            }
        }

        public void GetAllVacuumModeSprite()
        {
            for (int i = 0; i < _spriteControls.Length; i++)
            {
                _spriteControls[i].GetVacuumModeSprite();
            }
        }

        IEnumerator PacmomGameOver()
        {
            Time.timeScale = 0;
            Managers.Sound.Play(Sound.SFX, "Pacmom/Pacmom_SFX_Fail");
            _postProcessing.ChangeBleedAmount();

            yield return new WaitForSecondsRealtime(2f);

            Time.timeScale = 1f;

            CancelInvoke();
            _sceneSystem.LoadScene("Pacmom");
        }

        public void ExitPacmom()
        {
            CancelInvoke();
            _postProcessing.gameObject.SetActive(false);

            _sceneTransform = FindObjectOfType<SceneTransform>();
            _sceneTransform.EscapeFromScene("CH1");
        }
    }
}