using UnityEngine;
using UnityEngine.SceneManagement;

namespace Runtime.CH1.Main
{
    public class PacmomSkip : MonoBehaviour
    {
        private void Update()
        {
            gameObject.SetActive(!(Managers.Data.CH1.Scene > 1));
        }

        public void PMSkipBtn()
        {
            Managers.Data.CH1.IsPacmomPlayed = true;
            Managers.Data.CH1.IsPacmomCleared = true;
            Managers.Data.CH1.PacmomCoin = 300;
            Managers.Data.SaveGame();
            SceneManager.LoadScene("CH1");
        }
    }
}